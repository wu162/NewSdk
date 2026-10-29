using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using NewModCompiler.Diagnostics;

namespace NewModCompiler.Tests
{
    internal static class PerformanceTests
    {
        static Dictionary<string, object>[] Rows(BuildPerformance value)
        {
            var serializer = new JavaScriptSerializer();
            var report = serializer.Deserialize<Dictionary<string, object>>(serializer.Serialize(value.Snapshot()));
            return ((System.Collections.IEnumerable)report["entries"]).Cast<Dictionary<string, object>>().ToArray();
        }
        static void Work()
        {
            long until = Stopwatch.GetTimestamp() + Stopwatch.Frequency / 1000;
            while (Stopwatch.GetTimestamp() < until) { }
        }
        internal static int Run()
        {
            var off = new BuildPerformance("off");
            if (off.Measure("ignored") != null || off.Measure("ignored", true) != null)
                throw new Exception("Disabled performance collection created a scope.");
            var summary = new BuildPerformance("summary");
            using (summary.Measure("phase")) Work();
            if (summary.Measure("detail", true) != null || Rows(summary).Length != 1)
                throw new Exception("Summary collected detailed scopes.");

            var detailed = new BuildPerformance("detailed");
            detailed.StreamId = "base/Mod";
            using (detailed.Measure("parent")) {
                Work();
                using (detailed.Measure("child", true)) Work();
                try { using (detailed.Measure("child", true)) { Work(); throw new InvalidOperationException(); } }
                catch (InvalidOperationException) { }
                Work();
            }
            var rows = Rows(detailed);
            var parent = rows.Single(r => (string)r["name"] == "parent");
            var child = rows.Single(r => (string)r["name"] == "child");
            double total = Convert.ToDouble(parent["totalMs"]), self = Convert.ToDouble(parent["selfMs"]);
            if (rows.Length != 2 || Convert.ToInt64(child["count"]) != 2 ||
                total < self || self <= 0 || Convert.ToDouble(child["maxMs"]) > Convert.ToDouble(child["totalMs"]) ||
                Math.Abs(total - self - Convert.ToDouble(child["totalMs"])) > 0.00001 ||
                rows.Any(r => (string)r["streamId"] != "base/Mod"))
                throw new Exception("Nested timing/count/exception aggregation differs.");
            // Recursive phase names must aggregate, with direct child time
            // subtracted once; a later stream must have its own row.
            using (detailed.Measure("recursive")) using (detailed.Measure("recursive")) Work();
            detailed.StreamId = "apt/UI";
            using (detailed.Measure("child")) Work();
            rows = Rows(detailed);
            if (rows.Length != 4 || Convert.ToInt64(rows.Single(r => (string)r["name"] == "recursive")["count"]) != 2)
                throw new Exception("Recursive or multi-stream aggregation differs.");
            return 5;
        }
    }
}
