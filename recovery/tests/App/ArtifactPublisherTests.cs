using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using NewModCompiler.Application;

namespace NewModCompiler.Tests
{
    public static class ArtifactPublisherTests
    {
        static int assertions;
        static void Check(bool value, string message) { assertions++; if (!value) throw new Exception("ArtifactPublisher: " + message); }
        static Artifact FileAt(string path, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, content);
            string hash; using (var input = File.OpenRead(path)) using (var sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(input)).Replace("-", "").ToLowerInvariant();
            return new Artifact { Path = path, Kind = "stream.bin", Size = new FileInfo(path).Length, Sha256 = hash };
        }
        static void Reject(Action action, string label)
        {
            bool rejected = false;
            try { action(); } catch (IOException) { rejected = true; } catch (ArgumentException) { rejected = true; }
            Check(rejected, label + " was not rejected");
        }
        static void Junction(string path, string target)
        {
            using (var process = Process.Start(new ProcessStartInfo("cmd.exe", "/d /c mklink /J \"" + path + "\" \"" + target + "\"") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true }))
            {
                string log = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd(); process.WaitForExit(); Check(process.ExitCode == 0, "junction fixture failed: " + log);
            }
        }
        public static int Run(string work)
        {
            assertions = 0;
            string output = Path.Combine(work, "out"), staging = Path.Combine(output, ".build", "one"), target = Path.Combine(output, "release");
            var request = new BuildRequest { OutputRoot = output, ModRoot = Path.Combine(work, "mod"), SdkRoot = Path.Combine(work, "sdk") };
            Directory.CreateDirectory(request.ModRoot); Directory.CreateDirectory(request.SdkRoot);
            Artifact asset = FileAt(Path.Combine(staging, "apt", "例 子", "stream.bin"), "new payload");
            FileAt(Path.Combine(staging, "unlisted-stale.bin"), "must not be published");
            string sentinel = Path.Combine(target, "old.txt"); FileAt(sentinel, "old generation");
            var publisher = new ArtifactPublisher();
            using (ArtifactPublication transaction = publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None))
            {
                Check(transaction.State == "Prepared" && transaction.Result == null, "preparation must not imply publication");
                Check(File.ReadAllText(sentinel) == "old generation", "preparation changed old output");
                Reject(delegate { publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None); }, "concurrent transaction on same destination");
                using (ArtifactPublication independent = publisher.Prepare(request, staging, "independent", new[] { asset }, CancellationToken.None))
                    Check(independent.State == "Prepared", "independent destination was unnecessarily serialized");
                transaction.Commit();
                Check(transaction.State == "CommittedPendingFinalization", "commit must await final output channels");
                Reject(delegate { publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None); }, "destination lock must survive commit until finalization");
                Check(transaction.Result.Status == "CommittedPendingFinalization", "premature publication status");
                Check(File.ReadAllText(transaction.Result.Artifacts[0].Path) == "new payload", "committed payload differs");
                Check(Directory.GetFiles(target, "*", SearchOption.AllDirectories).Length == 1, "unlisted staging file leaked into publication");
                transaction.Rollback();
                Check(transaction.State == "RolledBack" && transaction.Result == null, "rollback state/result differs");
            }
            Check(File.ReadAllText(sentinel) == "old generation", "late-output-failure rollback lost old generation");
            using (ArtifactPublication transaction = publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None))
            {
                transaction.Commit(); // Simulated late report/log failure: Dispose must undo commit.
            }
            Check(File.ReadAllText(sentinel) == "old generation", "dispose did not roll back unfinalized commit");
            using (ArtifactPublication transaction = publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None))
            {
                transaction.Commit(); transaction.Complete();
                Check(transaction.State == "Completed" && transaction.Result.Status == "PublishedDirectory", "final success state differs");
                Check(transaction.Result.Artifacts.Length == 1 && transaction.Result.Artifacts[0].Sha256 == asset.Sha256, "final artifact receipt differs");
                Check(!File.Exists(sentinel), "stale previous output remains mixed into new generation");
            }
            Check(File.Exists(asset.Path), "publisher modified staging input");
            string published=Path.Combine(target,"apt","例 子","stream.bin");
            DateTime stamp=new DateTime(2001,2,3,4,5,6,DateTimeKind.Utc); File.SetLastWriteTimeUtc(published,stamp);
            using (ArtifactPublication same=publisher.Prepare(request,staging,"release",new[] {asset},CancellationToken.None))
            {
                Check(same.Unchanged && Directory.GetDirectories(output,".publish-*").Length==0,"Unchanged output allocated a publication copy");
                same.Commit(); same.Rollback();
                Check(File.ReadAllText(published)=="new payload" && File.GetLastWriteTimeUtc(published)==stamp,"Unchanged rollback touched the existing generation");
            }
            using (ArtifactPublication same=publisher.Prepare(request,staging,"release",new[] {asset},CancellationToken.None))
            {
                same.Commit(); same.Complete();
                Check(same.Unchanged && File.GetLastWriteTimeUtc(published)==stamp,"Unchanged success replaced published output");
            }
            File.WriteAllText(published,"bad payload");
            using (ArtifactPublication repair=publisher.Prepare(request,staging,"release",new[] {asset},CancellationToken.None))
            {
                Check(!repair.Unchanged,"Changed destination was treated as unchanged"); repair.Commit(); repair.Complete();
            }
            Check(File.ReadAllText(published)=="new payload","Changed destination was not repaired");
            using (ArtifactPublication retry = publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None))
                Check(retry.State == "Prepared", "completed transaction did not release destination lock");
            Reject(delegate { publisher.Prepare(request, staging, "../escape", new[] { asset }, CancellationToken.None); }, "destination traversal");
            Artifact outside = FileAt(Path.Combine(work, "outside.bin"), "outside");
            Reject(delegate { publisher.Prepare(request, staging, "release", new[] { outside }, CancellationToken.None); }, "source outside staging");
            Reject(delegate { publisher.Prepare(request, staging, "release", new[] { asset, asset }, CancellationToken.None); }, "duplicate allowlist target");
            string oldHash = asset.Sha256; asset.Sha256 = new string('0', 64);
            Reject(delegate { publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None); }, "source hash mismatch"); asset.Sha256 = oldHash;
            long oldSize = asset.Size; asset.Size++;
            Reject(delegate { publisher.Prepare(request, staging, "release", new[] { asset }, CancellationToken.None); }, "source size mismatch"); asset.Size = oldSize;
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel(); bool rejected = false;
                try { publisher.Prepare(request, staging, "release", new[] { asset }, cancelled.Token); } catch (OperationCanceledException) { rejected = true; }
                Check(rejected, "cancelled preparation was accepted");
            }
            string link = Path.Combine(output, "linked"); Junction(link, request.ModRoot);
            try { Reject(delegate { publisher.Prepare(request, staging, "linked", new[] { asset }, CancellationToken.None); }, "destination junction"); }
            finally { Directory.Delete(link); }
            link = Path.Combine(staging, "link"); Junction(link, request.ModRoot);
            try
            {
                Artifact throughLink = FileAt(Path.Combine(request.ModRoot, "input.bin"), "input sentinel");
                throughLink.Path = Path.Combine(link, "input.bin");
                Reject(delegate { publisher.Prepare(request, staging, "release", new[] { throughLink }, CancellationToken.None); }, "source ancestor junction");
                Check(File.ReadAllText(Path.Combine(request.ModRoot, "input.bin")) == "input sentinel", "junction rejection changed an input");
            }
            finally { Directory.Delete(link); }
            Check(Directory.GetDirectories(output, ".publish-*").Length == 0 && Directory.GetDirectories(output, ".previous-*").Length == 0, "successful/failed transactions leaked owned preparation or backup directories");
            Console.WriteLine("PASS ArtifactPublisher: " + assertions + " filesystem assertions; isolated component, no CLI integration");
            return assertions;
        }
    }
}
