using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NewModCompiler.Application
{
    public static class Planner
    {
        public static BuildPlan Resolve(BuildRequest request, IDependencyPlanner dependencyPlanner = null)
        {
            var tasks = new List<BuildTask>(); var skipped = new List<SkippedContent>();
            foreach (string group in request.Content) {
                string[] files;
                if (group == "base") {
                    string file = Path.Combine(request.ModRoot,"Data","Mod.xml");
                    if (!File.Exists(file) && request.AutoDetectContent) { skipped.Add(new SkippedContent { Content = group, Status = "SkippedNoInputs", Reason = "Data/Mod.xml is absent during automatic discovery." }); continue; }
                    if (!File.Exists(file)) throw new BuildFailure(ExitCode.Arguments,"PLAN_BASE_MISSING","Selected base content requires Data/Mod.xml.");
                    files = new [] {file};
                } else {
                    string dir = Path.Combine(request.ModRoot,"Data",group == "apt" ? "AptUI" : "AdditionalMaps");
                    files = Directory.Exists(dir) ? Directory.GetFiles(dir, group == "apt" ? "*.xml" : "MapMetaData_*.xml", SearchOption.TopDirectoryOnly) : new string[0];
                    files = files.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ThenBy(x => x, StringComparer.Ordinal).ToArray();
                    if (files.Length == 0) skipped.Add(new SkippedContent { Content = group, Status = "SkippedNoInputs", Reason = "No matching optional entries in selected content group." });
                }
                foreach (string input in files) {
                    string name = Path.GetFileNameWithoutExtension(input);
                    string id = group + "/" + name;
                    if (tasks.Any(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))) throw new BuildFailure(ExitCode.Arguments,"PLAN_COLLISION","Case-insensitive stream output collision: " + id);
                    tasks.Add(new BuildTask { Id = id, Content = group, InputPath = Path.GetFullPath(input), OutputName = group + "/" + name, Reason = request.AutoDetectContent ? "AutomaticContentDiscovery" : "ExplicitContentSelection", Dependencies = new string[0] });
                }
            }
            if (request.AutoDetectContent && tasks.Count == 0) throw new BuildFailure(ExitCode.Arguments,"PLAN_NO_INPUTS","No valid MOD inputs found. Expected Data/Mod.xml, Data/AptUI/*.xml or Data/AdditionalMaps/MapMetaData_*.xml. Additional files alone are not a compiled MOD input.");
            if (request.FixNeutralAssets && !tasks.Any(x => x.Content == "base")) throw new BuildFailure(ExitCode.Arguments,"PACKAGE_NEUTRAL_CONTENT","Neutral-asset repair requires an actual base input (Data/Mod.xml).");
            return new BuildPlan { Tasks = OrderTasks(dependencyPlanner == null ? tasks : dependencyPlanner.Expand(request,tasks.AsReadOnly())), Skipped = skipped.ToArray(), PlanKind = dependencyPlanner == null ? "StreamDiscovery" : "ExplicitDependencyGraph" };
        }
        public static BuildTask[] OrderTasks(IEnumerable<BuildTask> input)
        {
            if (input == null) throw new BuildFailure(ExitCode.Arguments,"PLAN_DEPENDENCIES","Dependency planner returned no graph.");
            var tasks = new Dictionary<string,BuildTask>(StringComparer.OrdinalIgnoreCase);
            foreach (BuildTask task in input) {
                if (task == null || string.IsNullOrEmpty(task.Id) || string.IsNullOrEmpty(task.InputPath) || string.IsNullOrEmpty(task.OutputName) || task.Dependencies == null)
                    throw new BuildFailure(ExitCode.Arguments,"PLAN_DEPENDENCIES","Incomplete task in dependency graph.");
                if (task.Content != "base" && task.Content != "apt" && task.Content != "maps") throw new BuildFailure(ExitCode.Arguments,"PLAN_CONTENT","Unsupported dependency content group: " + task.Content);
                if (!File.Exists(task.InputPath)) throw new BuildFailure(ExitCode.Arguments,"PLAN_INPUT_MISSING","Dependency input does not exist: " + task.InputPath);
                BuildTask old;
                if (tasks.TryGetValue(task.Id,out old)) {
                    if (!string.Equals(Path.GetFullPath(old.InputPath),Path.GetFullPath(task.InputPath),StringComparison.OrdinalIgnoreCase) || old.Content != task.Content || old.OutputName != task.OutputName || !old.Dependencies.OrderBy(x=>x,StringComparer.OrdinalIgnoreCase).SequenceEqual(task.Dependencies.OrderBy(x=>x,StringComparer.OrdinalIgnoreCase),StringComparer.OrdinalIgnoreCase))
                        throw new BuildFailure(ExitCode.Arguments,"PLAN_COLLISION","Conflicting task contexts share an id: " + task.Id);
                    continue;
                }
                if (tasks.Values.Any(x=>string.Equals(x.OutputName,task.OutputName,StringComparison.OrdinalIgnoreCase) || (x.Content==task.Content && string.Equals(Path.GetFileNameWithoutExtension(x.InputPath),Path.GetFileNameWithoutExtension(task.InputPath),StringComparison.OrdinalIgnoreCase))))
                    throw new BuildFailure(ExitCode.Arguments,"PLAN_COLLISION","Two task contexts would write the same stream workspace: " + task.OutputName);
                tasks.Add(task.Id,task);
            }
            var ordered = new List<BuildTask>(); var pending = new HashSet<string>(tasks.Keys,StringComparer.OrdinalIgnoreCase);
            while (pending.Count > 0) {
                foreach (string id in pending) foreach (string dependency in tasks[id].Dependencies)
                    if (!tasks.ContainsKey(dependency)) throw new BuildFailure(ExitCode.Arguments,"PLAN_DEPENDENCY_MISSING","Unknown dependency " + dependency + " of " + id);
                var ready = pending.Where(id=>tasks[id].Dependencies.All(d=>!pending.Contains(d))).OrderBy(id=>Priority(tasks[id].Content)).ThenBy(id=>id,StringComparer.OrdinalIgnoreCase).ThenBy(id=>id,StringComparer.Ordinal).ToArray();
                if (ready.Length == 0) throw new BuildFailure(ExitCode.Arguments,"PLAN_CYCLE","Cycle in stream dependency graph.");
                foreach (string id in ready) { ordered.Add(tasks[id]); pending.Remove(id); }
            }
            return ordered.ToArray();
        }
        static int Priority(string content) { return content == "base" ? 0 : content == "apt" ? 1 : content == "maps" ? 2 : 3; }
    }
}
