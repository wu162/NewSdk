import * as path from 'node:path';

export type CompilerOperation = 'doctor' | 'plan' | 'build';

export interface RunConfiguration {
    compilerPath: string;
    sdkRoot: string;
    modRoot: string;
    outputDirectory: string;
    modVersion: string;
}

export interface DiagnosticPaths {
    events: string;
    report: string;
    log: string;
}

/** Separate plan evidence so checking a plan never replaces the previous build report. */
export function diagnosticPaths(operation: 'plan' | 'build', outputDirectory: string): DiagnosticPaths {
    const paths = /^[a-z]:[\\/]|^\\\\/i.test(outputDirectory) ? path.win32 : path;
    return {
        events: paths.join(outputDirectory, `${operation}.events.jsonl`),
        report: paths.join(outputDirectory, `${operation}-report.json`),
        log: paths.join(outputDirectory, `${operation}.log`),
    };
}

/** Return an argv array; these values are never interpreted by a shell. */
export function compilerArguments(operation: CompilerOperation, configuration: RunConfiguration): string[] {
    if (!configuration.sdkRoot.trim()) throw new Error('请选择 SDK 目录。');
    if (operation === 'doctor') return ['doctor', '--sdk-root', configuration.sdkRoot];
    if (!configuration.modRoot.trim()) throw new Error('请选择 MOD 根目录。');
    if (!configuration.outputDirectory.trim()) throw new Error('请选择输出目录。');
    if (!configuration.modVersion.trim()) throw new Error('请填写 MOD 版本。');
    const paths = diagnosticPaths(operation, configuration.outputDirectory);
    return [
        operation,
        '--mod', configuration.modRoot,
        '--sdk-root', configuration.sdkRoot,
        '--out', configuration.outputDirectory,
        '--mod-version', configuration.modVersion.trim(),
        '--package-content', 'auto',
        '--package-format', 'big',
        '--include-additional=true',
        '--events', paths.events,
        '--report', paths.report,
        '--log', paths.log,
    ];
}

/** Pure lock manager, shared by all runner instances in one extension host. */
export class ProjectRunLocks {
    private readonly held = new Set<string>();

    acquire(identity: string): () => void {
        if (this.held.has(identity)) throw new Error('这个 MOD 已有正在运行的编译器任务。');
        this.held.add(identity);
        let released = false;
        return () => {
            if (!released) {
                released = true;
                this.held.delete(identity);
            }
        };
    }
}
