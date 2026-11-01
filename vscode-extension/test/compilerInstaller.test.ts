import { test } from 'node:test';
import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { createHash } from 'node:crypto';
import { installCompiler, installCompilerArchive, validateArchivePath, validateCompilerExecutable } from '../src/compilerInstaller';

interface ZipEntry { name: string; body: string | Buffer; mode?: number }

function crc32(data: Buffer): number {
  let value = 0xffffffff;
  for (const byte of data) {
    value ^= byte;
    for (let i = 0; i < 8; ++i) value = (value >>> 1) ^ (value & 1 ? 0xedb88320 : 0);
  }
  return (value ^ 0xffffffff) >>> 0;
}

/** Minimal stored ZIP fixture writer keeps tests independent from production ZIP reader. */
function makeZip(entries: ZipEntry[]): Buffer {
  const files: Buffer[] = [];
  const central: Buffer[] = [];
  let offset = 0;
  for (const entry of entries) {
    const name = Buffer.from(entry.name);
    const body = Buffer.isBuffer(entry.body) ? entry.body : Buffer.from(entry.body);
    const header = Buffer.alloc(30);
    header.writeUInt32LE(0x04034b50, 0);
    header.writeUInt16LE(20, 4);
    header.writeUInt32LE(crc32(body), 14);
    header.writeUInt32LE(body.length, 18);
    header.writeUInt32LE(body.length, 22);
    header.writeUInt16LE(name.length, 26);
    files.push(header, name, body);
    const record = Buffer.alloc(46);
    record.writeUInt32LE(0x02014b50, 0);
    record.writeUInt16LE(0x0314, 4);
    record.writeUInt16LE(20, 6);
    record.writeUInt32LE(crc32(body), 16);
    record.writeUInt32LE(body.length, 20);
    record.writeUInt32LE(body.length, 24);
    record.writeUInt16LE(name.length, 28);
    record.writeUInt32LE(((entry.mode ?? 0o100644) << 16) >>> 0, 38);
    record.writeUInt32LE(offset, 42);
    central.push(record, name);
    offset += header.length + name.length + body.length;
  }
  const directory = Buffer.concat(central);
  const end = Buffer.alloc(22);
  end.writeUInt32LE(0x06054b50, 0);
  end.writeUInt16LE(entries.length, 8);
  end.writeUInt16LE(entries.length, 10);
  end.writeUInt32LE(directory.length, 12);
  end.writeUInt32LE(offset, 16);
  return Buffer.concat([...files, directory, end]);
}

async function fixture(t: import('node:test').TestContext, entries: ZipEntry[]) {
  const root = path.resolve('work');
  await fs.mkdir(root, { recursive: true });
  const directory = await fs.mkdtemp(path.join(root, 'installer-test-'));
  t.after(() => fs.rm(directory, { recursive: true, force: true }));
  const archive = path.join(directory, 'package.zip');
  const bytes = makeZip(entries);
  await fs.writeFile(archive, bytes);
  return { directory, archive, bytes, storageDirectory: path.join(directory, 'globalStorage') };
}

const exe = { name: 'release/NewModCompiler.exe', body: 'MZ-test-fixture' };
const runtimeFiles = [exe,
  { name: 'release/NewModCompiler.exe.config', body: '<configuration><startup><supportedRuntime version="v4.0" /></startup></configuration>' },
  { name: 'release/antlr.runtime.dll', body: 'MZ-antlr-fixture' },
];

test('installs one EXE, preserves archive dependencies, and records a credential-free source', async t => {
  const f = await fixture(t, [...runtimeFiles, { name: 'release/data/schema.xml', body: '<schema />' }]);
  const sha256 = createHash('sha256').update(f.bytes).digest('hex');
  const result = await installCompilerArchive(f.archive, {
    storageDirectory: f.storageDirectory, expectedSha256: sha256.toUpperCase(),
    sourceUrl: 'https://downloads.example.test/compiler.zip?token=secret#private',
  });
  assert.equal(result.sha256, sha256);
  assert.equal(result.sourceUrl, 'https://downloads.example.test/compiler.zip');
  assert.equal(await fs.readFile(path.join(path.dirname(result.executablePath), 'data/schema.xml'), 'utf8'), '<schema />');
  assert.equal((await validateCompilerExecutable(result.executablePath)).deployment, 'framework-dependent');
  const repeated = await installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory });
  assert.equal(repeated.executablePath, result.executablePath);
  assert.deepEqual((await fs.readdir(path.join(f.storageDirectory, 'compilers'))), [sha256]);
});

test('rejects a wrong hash before extracting or creating an installation', async t => {
  const f = await fixture(t, [exe]);
  await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory, expectedSha256: '0'.repeat(64) }), /SHA-256/);
  await assert.rejects(fs.stat(f.storageDirectory), { code: 'ENOENT' });
});

test('rejects unsafe Windows/POSIX names', () => {
  for (const name of ['../outside', '/absolute', 'C:/escape', 'nested\\..\\escape', 'safe/../escape', 'safe/NUL.txt', 'file:stream', 'a./file', 'a//file']) {
    assert.throws(() => validateArchivePath(name), /路径/, name);
  }
  assert.equal(validateArchivePath('compiler/runtime/file.dll'), path.join('compiler', 'runtime', 'file.dll'));
});

test('rejects traversing ZIP before any path outside staging can be written', async t => {
  const f = await fixture(t, [exe, { name: '../outside.txt', body: 'bad' }]);
  await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory }), /relative|path|路径|越界/i);
  assert.deepEqual(await fs.readdir(path.join(f.storageDirectory, 'compilers')), []);
  await assert.rejects(fs.stat(path.join(f.directory, 'outside.txt')), { code: 'ENOENT' });
});

test('rejects symbolic links and ambiguous case aliases', async t => {
  for (const entries of [
    [exe, { name: 'release/link', body: '../outside', mode: 0o120777 }],
    [exe, { name: 'release/FILE.dll', body: 'a' }, { name: 'release/file.dll', body: 'b' }],
  ]) {
    const f = await fixture(t, entries);
    await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory }), /符号链接|大小写冲突/);
  }
});

test('rejects zero or multiple compilers and non-EXE content', async t => {
  for (const entries of [[{ name: 'readme.txt', body: 'none' }], [exe, { ...exe, name: 'second/NewModCompiler.exe' }], [{ ...exe, body: 'not a PE executable' }]]) {
    const f = await fixture(t, entries);
    await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory }), /唯一|Windows/);
  }
});

test('checks declared .NET dependencies and preserves a self-contained layout', async t => {
  const dependencyManifest = {
    runtimeTarget: { name: '.NETCoreApp,Version=v8.0/win-x64' },
    targets: { '.NETCoreApp,Version=v8.0/win-x64': {
      'NewModCompiler/1.0': { runtime: { 'NewModCompiler.dll': {} } },
      'runtime/1.0': { native: { 'runtimes/win-x64/native/hostfxr.dll': {} } },
    } },
  };
  const entries = [exe,
    { name: 'release/NewModCompiler.runtimeconfig.json', body: JSON.stringify({ runtimeOptions: { includedFrameworks: [{ name: 'Microsoft.NETCore.App', version: '8.0.0' }] } }) },
    { name: 'release/NewModCompiler.deps.json', body: JSON.stringify(dependencyManifest) },
    { name: 'release/NewModCompiler.dll', body: 'assembly' },
  ];
  const f = await fixture(t, entries);
  await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory }), /hostfxr.dll/);
  await fs.writeFile(f.archive, makeZip([...entries, { name: 'release/hostfxr.dll', body: 'runtime' }]));
  const result = await installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory });
  assert.equal((await validateCompilerExecutable(result.executablePath)).deployment, 'self-contained');
});

test('rejects invalid URLs, insecure transport, URL credentials and invalid hash without downloading', async t => {
  const f = await fixture(t, [exe]);
  for (const url of ['not-a-url', 'http://example.test/compiler.zip', 'https://user:secret@example.test/compiler.zip']) {
    await assert.rejects(installCompiler({ url, storageDirectory: f.storageDirectory }));
  }
  await assert.rejects(installCompiler({ url: 'https://example.test/compiler.zip', storageDirectory: f.storageDirectory, expectedSha256: 'wrong' }), /SHA-256/);
});

test('rejects cancellation without producing a published installation', async t => {
  const f = await fixture(t, [exe]);
  const controller = new AbortController();
  controller.abort();
  await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory, signal: controller.signal }), /取消/);
  await assert.rejects(fs.stat(f.storageDirectory), { code: 'ENOENT' });
});

 test('rejects an incomplete known net48 runtime package', async t => {
  const f = await fixture(t, [exe, runtimeFiles[1]]);
  await assert.rejects(installCompilerArchive(f.archive, { storageDirectory: f.storageDirectory }), /antlr.runtime.dll/);
});

test('authenticated HTTPS redirects preserve same-origin headers and drop credentials at a CDN', async t => {
  const { EventEmitter } = await import('node:events');
  const { PassThrough } = await import('node:stream');
  const httpsModule = require('node:https') as typeof import('node:https');
  const calls: { host: string; headers: Record<string, string> }[] = [];
  const f = await fixture(t, runtimeFiles);
  const fakeGet = ((url: URL, options: import('node:https').RequestOptions, callback: (response: import('node:http').IncomingMessage) => void) => {
    calls.push({ host: url.hostname, headers: options.headers as Record<string, string> });
    const stream = new PassThrough();
    const response = stream as unknown as import('node:http').IncomingMessage;
    response.statusCode = calls.length < 3 ? 302 : 200;
    response.headers = calls.length === 1 ? { location: '/signed.zip' }
      : calls.length === 2 ? { location: 'https://cdn.example.test/compiler.zip?signature=secret' } : {};
    const request = new EventEmitter() as import('node:http').ClientRequest;
    request.setTimeout = () => request;
    process.nextTick(() => callback(response));
    setImmediate(() => stream.end(calls.length >= 3 ? f.bytes : undefined));
    return request;
  }) as typeof httpsModule.get;
  t.mock.method(httpsModule, 'get', fakeGet);
  const result = await installCompiler({
    url: 'https://downloads.example.test/compiler.zip?token=secret', storageDirectory: f.storageDirectory,
    headers: { Authorization: 'Bearer private', 'X-Custom-Secret': 'private' },
  });
  assert.equal(calls.length, 3);
  assert.equal(calls[0].headers.Authorization, 'Bearer private');
  assert.equal(calls[1].headers.Authorization, 'Bearer private');
  assert.equal(calls[2].headers.Authorization, undefined);
  assert.equal(calls[2].headers['X-Custom-Secret'], undefined);
  assert.equal(result.sourceUrl, 'https://downloads.example.test/compiler.zip');
});
