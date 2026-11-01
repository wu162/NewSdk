import * as fs from 'node:fs';
import * as fsp from 'node:fs/promises';
import * as path from 'node:path';
import * as https from 'node:https';
import { createHash } from 'node:crypto';
import { Transform } from 'node:stream';
import { pipeline } from 'node:stream/promises';
import { HttpsProxyAgent } from 'https-proxy-agent';
import * as yauzl from 'yauzl';

const MAX_DOWNLOAD_BYTES = 1024 * 1024 * 1024;
const MAX_EXTRACTED_BYTES = 4 * 1024 * 1024 * 1024;
const MAX_ENTRIES = 25_000;

export interface CompilerInstallation {
  executablePath: string;
  sha256: string;
  /** Credentials, query parameters and fragments are deliberately omitted. */
  sourceUrl: string;
  installedAt: string;
}

export interface InstallOptions {
  url: string;
  /** Always use ExtensionContext.globalStorageUri.fsPath, never a MOD path. */
  storageDirectory: string;
  expectedSha256?: string;
  /** Retrieve authentication from SecretStorage; never persist headers. */
  headers?: Readonly<Record<string, string>>;
  signal?: AbortSignal;
  proxyUrl?: string;
}

export interface ArchiveInstallOptions extends Omit<InstallOptions, 'url' | 'headers' | 'proxyUrl'> {
  sourceUrl?: string;
}

export interface CompilerValidation {
  executablePath: string;
  deployment: 'self-contained' | 'framework-dependent' | 'native';
}

function checkAbort(signal?: AbortSignal): void {
  if (signal?.aborted) throw new Error('编译器安装已取消。');
}

function parseDownloadUrl(value: string): URL {
  let url: URL;
  try { url = new URL(value); } catch { throw new Error('编译器下载链接无效。'); }
  if (url.protocol !== 'https:') throw new Error('编译器下载必须使用 HTTPS 链接。');
  if (url.username || url.password) throw new Error('下载链接不能包含用户名或密码，请使用认证请求头。');
  return url;
}

function safeSource(value: string): string {
  if (!value) return 'local-archive';
  const url = parseDownloadUrl(value);
  return `${url.origin}${url.pathname}`;
}

function expectedHash(value?: string): string | undefined {
  if (!value) return undefined;
  if (!/^[a-fA-F0-9]{64}$/.test(value)) throw new Error('SHA-256 必须是 64 位十六进制值。');
  return value.toLowerCase();
}

async function download(url: URL, destination: string, options: InstallOptions, redirects = 0,
  headers: Readonly<Record<string, string>> = options.headers ?? {}): Promise<void> {
  checkAbort(options.signal);
  const proxy = options.proxyUrl ?? process.env.HTTPS_PROXY ?? process.env.https_proxy ?? process.env.HTTP_PROXY;
  const response = await new Promise<import('node:http').IncomingMessage>((resolve, reject) => {
    const request = https.get(url, {
      headers: { 'User-Agent': 'NewSdk-Mod-Tools', ...headers },
      agent: proxy ? new HttpsProxyAgent(proxy) : undefined,
      signal: options.signal,
    }, resolve);
    request.setTimeout(30_000, () => request.destroy(new Error('编译器下载超时。')));
    request.on('error', reject);
  });
  if ([301, 302, 303, 307, 308].includes(response.statusCode ?? 0)) {
    const location = response.headers.location;
    response.resume();
    if (!location || redirects >= 5) throw new Error('编译器下载重定向无效或次数过多。');
    let next: URL;
    try { next = parseDownloadUrl(new URL(location, url).toString()); }
    catch { throw new Error('编译器下载重定向链接无效或不安全。'); }
    // A signed CDN redirect may be legitimate; user credentials stay on their original origin.
    const nextHeaders = next.origin === url.origin ? headers : {};
    return download(next, destination, options, redirects + 1, nextHeaders);
  }
  if (response.statusCode !== 200) {
    response.resume();
    throw new Error(`编译器下载失败（HTTP ${response.statusCode ?? 'unknown'}）。`);
  }
  const declaredLength = Number(response.headers['content-length']);
  if (Number.isFinite(declaredLength) && declaredLength > MAX_DOWNLOAD_BYTES) {
    response.destroy();
    throw new Error('编译器 ZIP 超过 1 GiB 限制。');
  }
  let size = 0;
  const limiter = new Transform({ transform(chunk: Buffer, _encoding, callback) {
    size += chunk.length;
    callback(size > MAX_DOWNLOAD_BYTES ? new Error('编译器 ZIP 超过 1 GiB 限制。') : null, chunk);
  } });
  await pipeline(response, limiter, fs.createWriteStream(destination, { flags: 'wx' }), { signal: options.signal });
}

/** Reject Windows and POSIX traversal, links, device names and ambiguous aliases before writing. */
export function validateArchivePath(entryName: string): string {
  if (!entryName || entryName.includes('\0') || entryName.includes('\\') || entryName.startsWith('/') || /^[a-z]:/i.test(entryName)) {
    throw new Error('ZIP 包含不安全的路径。');
  }
  const parts = entryName.replace(/\/$/, '').split('/');
  for (const part of parts) {
    if (!part || part === '.' || part === '..' || /[<>:"|?*\x00-\x1f]/.test(part) || /[. ]$/.test(part)
      || /^(con|prn|aux|nul|com[1-9]|lpt[1-9])(?:\.|$)/i.test(part)) {
      throw new Error('ZIP 包含越界或 Windows 不支持的路径。');
    }
  }
  return parts.join(path.sep);
}

function openZip(archive: string): Promise<yauzl.ZipFile> {
  return new Promise((resolve, reject) => yauzl.open(archive, { lazyEntries: true, validateEntrySizes: true }, (error, zip) => {
    if (error || !zip) reject(new Error('下载内容不是有效的编译器 ZIP 包。'));
    else resolve(zip);
  }));
}

async function extractArchive(archive: string, destination: string, signal?: AbortSignal): Promise<void> {
  const zip = await openZip(archive);
  const entries = new Set<string>();
  let expanded = 0;
  let count = 0;
  try {
    await new Promise<void>((resolve, reject) => {
      let settled = false;
      const finish = (error?: unknown) => {
        if (settled) return;
        settled = true;
        signal?.removeEventListener('abort', onAbort);
        if (error) { zip.close(); reject(error); } else resolve();
      };
      const onAbort = () => finish(new Error('编译器安装已取消。'));
      signal?.addEventListener('abort', onAbort, { once: true });
      zip.on('error', finish);
      zip.on('end', () => finish());
      zip.on('entry', (entry: yauzl.Entry) => {
        void (async () => {
          checkAbort(signal);
          if (++count > MAX_ENTRIES) throw new Error('编译器 ZIP 文件数量超过限制。');
          const mode = (entry.externalFileAttributes >>> 16) & 0xffff;
          if ((mode & 0xf000) === 0xa000) throw new Error('编译器 ZIP 不允许包含符号链接。');
          const relative = validateArchivePath(entry.fileName);
          const key = relative.toLowerCase();
          if (entries.has(key)) throw new Error('编译器 ZIP 包含重复或大小写冲突的路径。');
          entries.add(key);
          expanded += entry.uncompressedSize;
          if (expanded > MAX_EXTRACTED_BYTES) throw new Error('编译器 ZIP 解压后超过 4 GiB 限制。');
          const target = path.resolve(destination, relative);
          if (!target.startsWith(path.resolve(destination) + path.sep)) throw new Error('ZIP 路径越界。');
          if (entry.fileName.endsWith('/')) await fsp.mkdir(target, { recursive: true });
          else {
            await fsp.mkdir(path.dirname(target), { recursive: true });
            const input = await new Promise<NodeJS.ReadableStream>((res, rej) => zip.openReadStream(entry, (error, stream) => {
              if (error || !stream) rej(error ?? new Error('无法读取 ZIP 条目。')); else res(stream);
            }));
            await pipeline(input, fs.createWriteStream(target, { flags: 'wx' }), { signal });
          }
          if (!settled) zip.readEntry();
        })().catch(finish);
      });
      zip.readEntry();
    });
  } finally { zip.close(); }
}

async function walkFiles(directory: string): Promise<string[]> {
  const files: string[] = [];
  for (const entry of await fsp.readdir(directory, { withFileTypes: true })) {
    const target = path.join(directory, entry.name);
    if (entry.isSymbolicLink()) throw new Error('编译器目录不能包含符号链接。');
    if (entry.isDirectory()) files.push(...await walkFiles(target));
    else if (entry.isFile()) files.push(target);
  }
  return files;
}

async function requireFile(file: string): Promise<void> {
  const stat = await fsp.stat(file).catch(() => undefined);
  if (!stat?.isFile()) throw new Error(`编译器缺少运行依赖：${path.basename(file)}`);
}

/** Inspect declared .NET publish dependencies without inventing a fixed package layout. */
export async function validateCompilerExecutable(executablePath: string): Promise<CompilerValidation> {
  const resolved = path.resolve(executablePath);
  if (path.basename(resolved).toLowerCase() !== 'newmodcompiler.exe') throw new Error('请选择 NewModCompiler.exe。');
  await requireFile(resolved);
  const handle = await fsp.open(resolved, 'r');
  try {
    const header = Buffer.alloc(2);
    const { bytesRead } = await handle.read(header, 0, 2, 0);
    if (bytesRead !== 2 || header.toString('ascii') !== 'MZ') throw new Error('NewModCompiler.exe 不是有效的 Windows 可执行文件。');
  } finally { await handle.close(); }
  const directory = path.dirname(resolved);
  const stem = path.basename(resolved, path.extname(resolved));
  const runtimePath = path.join(directory, `${stem}.runtimeconfig.json`);
  const depsPath = path.join(directory, `${stem}.deps.json`);
  const runtimeExists = await fsp.access(runtimePath).then(() => true, () => false);
  const depsExists = await fsp.access(depsPath).then(() => true, () => false);
  if (!runtimeExists && !depsExists) {
    // Reference compiler PublishCompiler.ps1 defines this net48/Win32 runtime contract.
    await requireFile(`${resolved}.config`);
    await requireFile(path.join(directory, 'antlr.runtime.dll'));
    return { executablePath: resolved, deployment: 'framework-dependent' };
  }
  await requireFile(runtimePath);
  await requireFile(depsPath);
  let runtime: { runtimeOptions?: { includedFrameworks?: unknown[]; framework?: unknown; frameworks?: unknown[] } };
  let deps: { runtimeTarget?: { name?: string }; targets?: Record<string, Record<string, Record<string, unknown>>> };
  try {
    runtime = JSON.parse(await fsp.readFile(runtimePath, 'utf8'));
    deps = JSON.parse(await fsp.readFile(depsPath, 'utf8'));
  } catch { throw new Error('编译器运行依赖清单不是有效 JSON。'); }
  const targetName = deps.runtimeTarget?.name;
  const target = targetName ? deps.targets?.[targetName] : undefined;
  if (!target) throw new Error('编译器 deps.json 缺少有效的运行目标。');
  for (const library of Object.values(target)) {
    for (const group of ['runtime', 'native', 'resources']) {
      const assets = library[group];
      if (!assets || typeof assets !== 'object') continue;
      for (const asset of Object.keys(assets)) {
        if (path.posix.basename(asset) === '_._') continue;
        validateArchivePath(asset);
        // dotnet publish flattens runtime/native assets; localized resources retain the culture folder.
        const relative = group === 'resources' ? asset : path.posix.basename(asset);
        const candidates = [path.join(directory, relative), path.join(directory, asset)];
        if (!await Promise.all(candidates.map(candidate => fsp.stat(candidate).then(s => s.isFile(), () => false))).then(v => v.some(Boolean))) {
          throw new Error(`编译器缺少运行依赖：${asset}`);
        }
      }
    }
  }
  const options = runtime.runtimeOptions;
  return { executablePath: resolved, deployment: options?.includedFrameworks?.length ? 'self-contained' : 'framework-dependent' };
}

async function hashFile(file: string, signal?: AbortSignal): Promise<string> {
  const hash = createHash('sha256');
  for await (const chunk of fs.createReadStream(file)) { checkAbort(signal); hash.update(chunk); }
  return hash.digest('hex');
}

/** Install a downloaded archive into immutable, hash-addressed extension storage. */
export async function installCompilerArchive(archivePath: string, options: ArchiveInstallOptions): Promise<CompilerInstallation> {
  checkAbort(options.signal);
  const expected = expectedHash(options.expectedSha256);
  const sha256 = await hashFile(archivePath, options.signal);
  if (expected && sha256 !== expected) throw new Error('编译器 ZIP 的 SHA-256 校验失败。');
  const sourceUrl = safeSource(options.sourceUrl ?? '');
  const installRoot = path.join(options.storageDirectory, 'compilers');
  await fsp.mkdir(installRoot, { recursive: true });
  const staging = await fsp.mkdtemp(path.join(installRoot, '.install-'));
  try {
    const payload = path.join(staging, 'payload');
    await fsp.mkdir(payload);
    await extractArchive(archivePath, payload, options.signal);
    const executables = (await walkFiles(payload)).filter(file => path.basename(file).toLowerCase() === 'newmodcompiler.exe');
    if (executables.length !== 1) throw new Error(`编译器 ZIP 必须包含唯一的 NewModCompiler.exe（发现 ${executables.length} 个）。`);
    await validateCompilerExecutable(executables[0]);
    const executableRelative = path.relative(staging, executables[0]);
    const installedAt = new Date().toISOString();
    await fsp.copyFile(archivePath, path.join(staging, 'compiler.zip'));
    await fsp.writeFile(path.join(staging, 'installation.json'), JSON.stringify({ sha256, sourceUrl, installedAt }, null, 2));
    checkAbort(options.signal);
    const target = path.join(installRoot, sha256);
    try { await fsp.rename(staging, target); }
    catch (error) {
      const code = (error as NodeJS.ErrnoException).code;
      if (code !== 'EEXIST' && code !== 'ENOTEMPTY') throw error;
      // An earlier install of this exact archive is reusable only after dependency validation.
      await validateCompilerExecutable(path.join(target, executableRelative));
    }
    return { executablePath: path.join(target, executableRelative), sha256, sourceUrl, installedAt };
  } finally { await fsp.rm(staging, { recursive: true, force: true }); }
}

export async function installCompiler(options: InstallOptions): Promise<CompilerInstallation> {
  const url = parseDownloadUrl(options.url);
  expectedHash(options.expectedSha256);
  await fsp.mkdir(options.storageDirectory, { recursive: true });
  const staging = await fsp.mkdtemp(path.join(options.storageDirectory, '.download-'));
  const archive = path.join(staging, 'compiler.zip');
  try {
    await download(url, archive, options);
    return await installCompilerArchive(archive, { ...options, sourceUrl: url.toString() });
  } finally { await fsp.rm(staging, { recursive: true, force: true }); }
}
