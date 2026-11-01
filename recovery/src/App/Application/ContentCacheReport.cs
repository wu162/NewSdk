using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NewModCompiler.Application
{
    // Owner reports are immutable by the time the coordinator calls Merge.
    // Keep the former build-level counters available and retain every group's
    // original report so paths and resource samples always have an owner.
    public static class ContentCacheReport
    {
        public static object Merge(IEnumerable<KeyValuePair<string, object>> reports)
        {
            if (reports == null) throw new ArgumentNullException("reports");
            var groups = new Dictionary<string, object>(StringComparer.Ordinal);
            var values = new List<Dictionary<string, object>>();
            string sessionRoot = null;
            foreach (var item in reports) {
                if (item.Value == null) continue;
                groups.Add(item.Key, item.Value);
                var report = Properties(item.Value); values.Add(report);
                object directory;
                if (report.TryGetValue("sessionCacheDirectory", out directory) && directory is string) {
                    string path = (string)directory;
                    string root = string.Equals(Path.GetFileName(path.TrimEnd('\\', '/')), item.Key, StringComparison.OrdinalIgnoreCase)
                        ? Path.GetDirectoryName(path) : path;
                    if (sessionRoot == null) sessionRoot = root;
                    else if (!string.Equals(sessionRoot, root, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Content caches must share the configured session root.");
                }
            }
            if (values.Count == 0) return null;
            var result = MergeProperties(values);
            result["groups"] = groups;
            result["sessionCacheDirectory"] = sessionRoot;
            return result;
        }

        static Dictionary<string, object> Properties(object value)
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);
            var dictionary = value as IDictionary;
            if (dictionary != null) {
                foreach (DictionaryEntry item in dictionary) result.Add((string)item.Key, item.Value);
            } else {
                foreach (PropertyInfo property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    if (property.GetIndexParameters().Length == 0) result.Add(property.Name, property.GetValue(value, null));
            }
            return result;
        }

        static Dictionary<string, object> MergeProperties(IEnumerable<Dictionary<string, object>> reports)
        {
            var columns = new Dictionary<string, List<object>>(StringComparer.Ordinal);
            foreach (var report in reports)
                foreach (var pair in report) {
                    List<object> values;
                    if (!columns.TryGetValue(pair.Key, out values)) columns.Add(pair.Key, values = new List<object>());
                    if (pair.Value != null) values.Add(pair.Value);
                }
            var merged = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var pair in columns) merged.Add(pair.Key, MergeValue(pair.Key, pair.Value));
            return merged;
        }

        static object MergeValue(string name, IList<object> values)
        {
            if (values.Count == 0) return null;
            object first = values[0];
            if (first is bool || first is string)
                return values.All(value => object.Equals(first, value)) ? first : null;
            if (first is double || first is float || first is decimal) {
                var numbers = values.Select(Convert.ToDouble);
                return IsPeak(name) ? numbers.Max() : numbers.Sum();
            }
            if (first is int || first is long || first is uint || first is ulong || first is short || first is byte) {
                var numbers = values.Select(Convert.ToInt64);
                if (name == "clockFrequency" || name == "intervalSampleLimit" || IsPeak(name)) return numbers.Max();
                if (name == "minimumEffectiveJobs") return numbers.Min();
                return numbers.Sum();
            }
            var sequence = first as IEnumerable;
            if (sequence != null && !(first is IDictionary)) {
                var combined = new List<object>();
                foreach (IEnumerable value in values) foreach (object item in value) combined.Add(item);
                return combined.ToArray();
            }
            return MergeProperties(values.Select(Properties));
        }

        static bool IsPeak(string name)
        {
            return name.StartsWith("peak", StringComparison.Ordinal) || name.StartsWith("max", StringComparison.Ordinal) ||
                name == "sampledPeakPrivateBytes" || name == "effectiveJobs" || name == "ioCapacity";
        }
    }
}
