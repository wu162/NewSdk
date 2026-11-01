import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import * as os from 'node:os';
import { runTests, runVSCodeCommand } from '@vscode/test-electron';

async function main(): Promise<void> {
    if (process.platform !== 'win32') throw new Error('VS Code P0 扩展宿主验收仅在 Windows 本地运行。');
    const extensionDevelopmentPath = path.resolve(__dirname, '../..');
    const root = process.env.NEWSDK_TEST_ROOT ?? await fs.mkdtemp(path.join(os.tmpdir(), 'newsdk-host-'));
    await fs.mkdir(root, { recursive: true });
    const workspace = path.join(root, 'MockBasic');
    await fs.mkdir(workspace, { recursive: true });
    const evidence = process.env.NEWSDK_TEST_EVIDENCE ?? path.join(extensionDevelopmentPath, 'test-results');
    await fs.mkdir(evidence, { recursive: true });
    const installation = await runVSCodeCommand(['--install-extension', path.join(extensionDevelopmentPath, 'NewSdk-Mod-Tools-P0.vsix'),
        '--force', '--user-data-dir', path.join(root, 'install-user-data'), '--extensions-dir', path.join(root, 'install-extensions')]);
    await fs.writeFile(path.join(evidence, 'vsix-install.log'), `${installation.stdout}\n${installation.stderr}`);
    const common = {
        extensionDevelopmentPath,
        extensionTestsPath: path.join(__dirname, 'host', 'index'),
        launchArgs: [workspace, '--disable-extensions', '--disable-workspace-trust', '--skip-welcome', '--skip-release-notes',
            '--user-data-dir', path.join(root, 'user-data'), '--extensions-dir', path.join(root, 'extensions')],
    };
    // Reopen the same isolated profile to verify extension-storage persistence.
    for (const phase of ['exercise', 'reopen']) {
        await runTests({ ...common, extensionTestsEnv: { NEWSDK_HOST_PHASE: phase, NEWSDK_TEST_ROOT: root,
            NEWSDK_TEST_EVIDENCE: evidence, NEWSDK_REPOSITORY: path.dirname(extensionDevelopmentPath),
            NEWSDK_COMPILER_URL: process.env.NEWSDK_COMPILER_URL ?? '',
            NEWSDK_COMPILER_SHA256: process.env.NEWSDK_COMPILER_SHA256 ?? '',
            NEWSDK_COMPILER_AUTHORIZATION: process.env.NEWSDK_COMPILER_AUTHORIZATION ?? '',
            NEWSDK_ACCEPTANCE: process.env.NEWSDK_ACCEPTANCE ?? 'smoke' } });
    }
}

main().catch(error => { console.error(error); process.exitCode = 1; });
