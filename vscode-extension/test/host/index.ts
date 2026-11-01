import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { createHash } from 'node:crypto';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
import * as vscode from 'vscode';
import { installCompiler } from '../../src/compilerInstaller';

interface Result { status: string; reason?: string; artifacts?: Array<{ path: string }>; diagnostics?: Array<{ code: string }> }
interface State { status: string; progress?: { buildId?: string }; result?: Result }
interface ExtensionApi {
    getState(): State;
    getProject(): unknown;
    selectProject(root: string): Promise<void>;
    configure(values: { compilerPath: string; sdkRoot: string; outputDirectory: string; modVersion: string }): Promise<void>;
    build(): Promise<Result | undefined>;
    plan(): Promise<Result | undefined>;
    stop(): void;
    showWorkbench(): void;
}

const execute = promisify(execFile);
const xml = '<?xml version="1.0" encoding="utf-8"?><AssetDeclaration xmlns="uri:ea.com:eala:asset"><ArmorTemplate id="NewSdkP0Probe" Default="100" /></AssetDeclaration>';

async function inventory(root: string): Promise<Record<string, string>> {
    const result: Record<string, string> = {};
    async function visit(directory: string): Promise<void> {
        for (const entry of await fs.readdir(directory, { withFileTypes: true })) {
            const file = path.join(directory, entry.name);
            if (entry.isDirectory()) await visit(file);
            else if (entry.isFile()) result[path.relative(root, file)] = createHash('sha256').update(await fs.readFile(file)).digest('hex');
        }
    }
    await visit(root);
    return result;
}

async function waitFor(predicate: () => boolean, timeout = 15000): Promise<void> {
    const start = Date.now();
    while (!predicate()) {
        if (Date.now() - start > timeout) throw new Error('等待 VS Code 编译状态超时。');
        await new Promise(resolve => setTimeout(resolve, 100));
    }
}

async function timed<T>(promise: Promise<T>, milliseconds = 90000): Promise<T> {
    let timer: ReturnType<typeof setTimeout> | undefined;
    try {
        return await Promise.race([promise, new Promise<T>((_, reject) => { timer = setTimeout(() => reject(new Error('编译任务验收超时。')), milliseconds); })]);
    } finally { if (timer) clearTimeout(timer); }
}

export async function run(): Promise<void> {
    const root = process.env.NEWSDK_TEST_ROOT!;
    const evidence = process.env.NEWSDK_TEST_EVIDENCE!;
    try {
        await runSuite();
    } finally {
        // Preserve partial protocol files even if an assertion or a task fails.
        for (const directory of ['outputs', 'real-outputs']) {
            try {
                await fs.cp(path.join(root, directory), path.join(evidence, directory), { recursive: true });
            } catch (error) {
                if ((error as NodeJS.ErrnoException).code !== 'ENOENT') console.error(`Cannot retain ${directory} evidence`, error);
            }
        }
    }
}

async function runSuite(): Promise<void> {
    assert.equal(process.platform, 'win32');
    const root = process.env.NEWSDK_TEST_ROOT!;
    const evidence = process.env.NEWSDK_TEST_EVIDENCE!;
    const extension = vscode.extensions.getExtension<ExtensionApi>('wu162.newsdk-mod-tools');
    assert.ok(extension, 'VSIX development extension must be discoverable');
    const api = await extension.activate();
    const commandIds = await vscode.commands.getCommands(true);
    for (const command of ['newsdk.build', 'newsdk.stop', 'newsdk.openWorkbench', 'newsdk.plan', 'newsdk.doctor']) {
        assert.ok(commandIds.includes(command), `Missing command ${command}`);
    }
    api.showWorkbench();
    if (process.env.NEWSDK_HOST_PHASE === 'reopen') {
        // No configuration calls here: the previous process must have persisted it.
        const result = await timed(api.build());
        assert.equal((result ?? api.getState().result)?.status, 'succeeded');
        await fs.writeFile(path.join(evidence, 'persistence.json'), JSON.stringify({ succeeded: true, scope: 'Second VS Code process using the same isolated profile' }, null, 2));
        return;
    }
    const compilerDirectory = path.join(root, 'fixture-compiler');
    await fs.mkdir(compilerDirectory, { recursive: true });
    const executable = path.join(compilerDirectory, 'NewModCompiler.exe');
    const csc = path.join(process.env.WINDIR ?? 'C:\\Windows', 'Microsoft.NET', 'Framework', 'v4.0.30319', 'csc.exe');
    await execute(csc, ['/nologo', '/target:exe', '/reference:System.Web.Extensions.dll', `/out:${executable}`,
        path.resolve(__dirname, '../../../test/host/compilerFixture.cs')], { timeout: 30000 });
    await fs.writeFile(`${executable}.config`, '<?xml version="1.0"?><configuration><startup><supportedRuntime version="v4.0" /></startup></configuration>');
    await fs.writeFile(path.join(compilerDirectory, 'antlr.runtime.dll'), 'Unused protocol fixture sidecar');
    const sdk = path.join(root, 'fixture-sdk');
    await fs.mkdir(sdk, { recursive: true });
    const cases: Array<{ name: string; status: string }> = [];
    async function select(name: string, compilerPath = executable, sdkRoot = sdk): Promise<string> {
        const mod = path.join(root, name);
        await fs.mkdir(path.join(mod, 'Data'), { recursive: true });
        await fs.writeFile(path.join(mod, 'Data', 'Mod.xml'), xml);
        await api.selectProject(mod);
        await api.configure({ compilerPath, sdkRoot, outputDirectory: path.join(root, 'outputs', name), modVersion: '1.0' });
        return mod;
    }
    const basic = await select('MockBasic');
    const source = await inventory(basic);
    let result = await timed(api.plan());
    assert.equal((result ?? api.getState().result)?.status, 'succeeded');
    result = await timed(api.build());
    assert.equal((result ?? api.getState().result)?.status, 'succeeded');
    assert.ok((result ?? api.getState().result)?.artifacts?.length);
    assert.deepEqual(await inventory(basic), source, 'Extension and task must leave MOD source bytes unchanged');
    cases.push({ name: 'plan-build-artifacts-source-readonly', status: 'passed' });
    await select('MockFailure');
    result = await timed(api.build());
    assert.equal((result ?? api.getState().result)?.status, 'failed');
    assert.ok((result ?? api.getState().result)?.diagnostics?.some(item => item.code === 'FIXTURE_ERROR'));
    assert.equal(vscode.languages.getDiagnostics().filter(([uri]) => uri.fsPath.startsWith(root)).length, 0,
        'Unlocated compiler diagnostics must not create invented Problems file locations');
    cases.push({ name: 'compiler-error-no-fake-location', status: 'passed' });
    await select('MockMissing');
    result = await timed(api.build());
    assert.notEqual((result ?? api.getState().result)?.status, 'succeeded');
    cases.push({ name: 'missing-report-never-success', status: 'passed' });
    await select('MockSlow');
    const stopped = timed(api.build());
    await waitFor(() => Boolean(api.getState().progress?.buildId));
    api.stop();
    result = await stopped;
    assert.equal((result ?? api.getState().result)?.status, 'interrupted');
    cases.push({ name: 'stop-button-forced-interruption', status: 'passed' });
    await select('MockBasic');
    result = await timed(api.build());
    assert.equal((result ?? api.getState().result)?.status, 'succeeded');
    cases.push({ name: 'retry-after-stop', status: 'passed' });
    await fs.writeFile(path.join(evidence, 'extension-host.json'), JSON.stringify({ succeeded: true, vscodeVersion: vscode.version,
        scope: 'Real VS Code extension host and Windows ProcessExecution with a synthetic protocol fixture; not real compiler acceptance', cases }, null, 2));

    if (process.env.NEWSDK_COMPILER_URL) {
        const installed = await installCompiler({ url: process.env.NEWSDK_COMPILER_URL,
            expectedSha256: process.env.NEWSDK_COMPILER_SHA256 || undefined,
            headers: process.env.NEWSDK_COMPILER_AUTHORIZATION ? { Authorization: process.env.NEWSDK_COMPILER_AUTHORIZATION } : undefined,
            storageDirectory: path.join(root, 'provided-compiler') });
        await fs.writeFile(path.join(evidence, 'compiler-source.json'), JSON.stringify({ ...installed,
            executableSha256: createHash('sha256').update(await fs.readFile(installed.executablePath)).digest('hex') }, null, 2));
        await realCompilerCases(api, root, evidence, installed.executablePath);
    } else {
        await fs.writeFile(path.join(evidence, 'real-compiler.json'), JSON.stringify({ status: 'skipped', reason: 'NEWSDK_COMPILER_URL 尚未提供，未运行真实编译器。' }, null, 2));
    }
    // Leave the synthetic working project selected for the reopen persistence check.
    await select('MockBasic');
}

async function realCompilerCases(api: ExtensionApi, root: string, evidence: string, compilerPath: string): Promise<void> {
    const sdkRoot = path.join(process.env.NEWSDK_REPOSITORY!, 'SDK');
    const full = process.env.NEWSDK_ACCEPTANCE === 'full';
    const layouts: Array<{ name: string; files: string[] }> = [
        { name: 'RealBase', files: ['Data/Mod.xml'] },
        ...(full ? [{ name: 'RealAptRouting', files: ['Data/AptUI/Entry.xml'] },
            { name: 'RealGlobalRouting', files: ['Data/AdditionalMaps/MapMetaData_Probe.xml'] },
            { name: 'RealAllRouting', files: ['Data/Mod.xml', 'Data/AptUI/Entry.xml', 'Data/AdditionalMaps/MapMetaData_Probe.xml'] }] : []),
    ];
    const results: Array<{ name: string; status: string; artifacts: number }> = [];
    for (const layout of layouts) {
        const mod = path.join(root, layout.name);
        for (const relative of layout.files) {
            await fs.mkdir(path.dirname(path.join(mod, relative)), { recursive: true });
            await fs.writeFile(path.join(mod, relative), xml);
        }
        if (layout.name === 'RealAllRouting') {
            await fs.mkdir(path.join(mod, 'Additional'), { recursive: true });
            await fs.writeFile(path.join(mod, 'Additional', 'readme.txt'), 'Additional packaging fixture');
        }
        const before = await inventory(mod);
        const outputDirectory = path.join(root, 'real-outputs', layout.name);
        await api.selectProject(mod);
        await api.configure({ compilerPath, sdkRoot, outputDirectory, modVersion: '1.0' });
        const planned = await timed(api.plan(), 180000);
        assert.equal((planned ?? api.getState().result)?.status, 'succeeded');
        const built = await timed(api.build(), 180000);
        const result = built ?? api.getState().result;
        assert.equal(result?.status, 'succeeded', result?.reason);
        assert.ok(result.artifacts?.length, 'Real compiler must report actual output artifacts');
        for (const artifact of result.artifacts) await fs.access(artifact.path);
        assert.deepEqual(await inventory(mod), before, 'Real build must leave input MOD unchanged');
        const caseEvidence = path.join(evidence, layout.name);
        await fs.mkdir(caseEvidence, { recursive: true });
        for (const file of ['build.events.jsonl', 'build-report.json', 'build.log', 'plan.events.jsonl', 'plan-report.json', 'plan.log']) {
            await fs.copyFile(path.join(outputDirectory, file), path.join(caseEvidence, file));
        }
        results.push({ name: layout.name, status: result.status, artifacts: result.artifacts.length });
    }
    await fs.writeFile(path.join(evidence, 'real-compiler.json'), JSON.stringify({ status: 'passed', cases: results,
        scope: 'Packaged real compiler, doctor through build preflight, small Armor stream, BIG output and read-only input. Optional full mode adds stream-discovery combinations; Armor XML in AptUI does not establish APT business compiler acceptance.',
        pending: ['Real APT business fixture', 'Normal Ctrl+Break cancellation', 'Complete P0 acceptance with the supplied release package'] }, null, 2));
}
