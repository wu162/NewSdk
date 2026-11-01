import * as assert from 'node:assert/strict';
import * as fs from 'node:fs/promises';
import * as path from 'node:path';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
import { BuildProtocolState, JsonlTail, readBuildReport } from '../src/buildProtocol';
import { compilerArguments, diagnosticPaths, RunConfiguration } from '../src/compilerArguments';

export interface NormalCancellationOptions extends RunConfiguration { evidenceDirectory: string }
export interface NormalCancellationEvidence {
    status: 'passed'; buildId: string; processExitCode: 6;
    signal: 'CTRL_BREAK_EVENT'; signalSent: true; streamStartedObserved: true; elapsedMs: number;
}
interface ControllerResult {
    succeeded?: boolean; signalSent?: boolean; streamStartedObserved?: boolean;
    processExitCode?: number; buildId?: string; elapsedMs?: number; error?: string;
}
const execute = promisify(execFile);

/** Test the release EXE with real Ctrl+Break; only the test controller is compiled. */
export async function runNormalCancellation(options: NormalCancellationOptions): Promise<NormalCancellationEvidence> {
    assert.equal(process.platform, 'win32', 'Normal Ctrl+Break acceptance requires Windows');
    await fs.mkdir(options.evidenceDirectory, { recursive: true });
    await fs.mkdir(options.outputDirectory, { recursive: true });
    const controller = path.join(options.evidenceDirectory, 'CancellationHost.exe');
    const csc = path.join(process.env.WINDIR ?? 'C:\\Windows', 'Microsoft.NET', 'Framework', 'v4.0.30319', 'csc.exe');
    await execute(csc, ['/nologo', '/target:exe', '/reference:System.Web.Extensions.dll', `/out:${controller}`,
        path.resolve(__dirname, '../../test/host/cancellationHost.cs')], { timeout: 30000 });
    const paths = diagnosticPaths('build', options.outputDirectory);
    const tail = await JsonlTail.capture(paths.events);
    const requestPath = path.join(options.evidenceDirectory, 'cancellation-request.json');
    const resultPath = path.join(options.evidenceDirectory, 'cancellation-controller.json');
    await fs.writeFile(requestPath, JSON.stringify({
        compilerPath: options.compilerPath, arguments: compilerArguments('build', options), cwd: options.modRoot,
        eventsPath: paths.events,
        stdoutPath: path.join(options.evidenceDirectory, 'cancellation.stdout.log'),
        stderrPath: path.join(options.evidenceDirectory, 'cancellation.stderr.log'), timeoutSeconds: 120,
    }, null, 2));
    let controllerFailure: unknown;
    try { await execute(controller, [requestPath, resultPath], { timeout: 150000, windowsHide: true }); }
    catch (error) { controllerFailure = error; }
    const controllerResult = JSON.parse(await fs.readFile(resultPath, 'utf8')) as ControllerResult;
    // Retain all available protocol evidence even when cancellation fails.
    for (const [kind, file] of Object.entries(paths)) {
        const name = kind === 'events' ? 'cancelled.events.jsonl' : kind === 'report' ? 'cancelled.report.json' : 'cancelled.build.log';
        try { await fs.copyFile(file, path.join(options.evidenceDirectory, name)); }
        catch (error) { if (!(error instanceof Error && 'code' in error && error.code === 'ENOENT')) throw error; }
    }
    assert.equal(controllerResult.succeeded, true,
        controllerResult.error ?? (controllerFailure instanceof Error ? controllerFailure.message : 'Cancellation controller failed'));
    assert.equal(controllerResult.signalSent, true);
    assert.equal(controllerResult.streamStartedObserved, true);
    assert.equal(controllerResult.processExitCode, 6);
    const protocol = new BuildProtocolState();
    protocol.accept(await tail.poll());
    const report = await readBuildReport(paths.report);
    assert.deepEqual(tail.errors, []);
    assert.equal(tail.hasPartialLine, false);
    const result = protocol.finish({ exitCode: controllerResult.processExitCode, report });
    assert.equal(result.status, 'cancelled', result.reason);
    assert.equal(result.buildId, controllerResult.buildId, 'Controller, events and report must identify the same build');
    assert.ok(result.buildId);
    assert.equal(report?.status, 'cancelled');
    assert.equal(report?.exitCode, 6);
    assert.deepEqual(result.artifacts, [], 'Cancelled builds must not advertise current output artifacts');
    const evidence: NormalCancellationEvidence = {
        status: 'passed', buildId: result.buildId, processExitCode: 6, signal: 'CTRL_BREAK_EVENT',
        signalSent: true, streamStartedObserved: true, elapsedMs: controllerResult.elapsedMs ?? 0,
    };
    await fs.writeFile(path.join(options.evidenceDirectory, 'normal-cancellation.json'), JSON.stringify(evidence, null, 2));
    return evidence;
}
