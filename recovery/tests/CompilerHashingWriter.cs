using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using BinaryAssetBuilder.Core;

// The exact same operation corpus is compiled into an original-only executable
// and the recovered validation host. Raw UTF-16 inputs are generated in-process,
// so JSON/UTF-8 transport cannot replace unpaired surrogates in the test input.
public static class CompilerHashingWriter
{
    const string Disable = "NewSdk.Compiler.DisableBufferedHashingWriter";
    static readonly JavaScriptSerializer Json = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
    static readonly Type WriterType = typeof(HashProvider).Assembly.GetType("BinaryAssetBuilder.Core.HashingWriter", true);
    static readonly MethodInfo FinalMethod = WriterType.GetMethod("GetFinalHash", BindingFlags.Public | BindingFlags.Instance);
    static int checks;
    static readonly List<string> tests = new List<string>();
    static readonly List<object> evidence = new List<object>();
    static void Require(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
    static TextWriter Writer(uint seed) { return (TextWriter)Activator.CreateInstance(WriterType, new object[] { seed }); }
    static uint Final(TextWriter writer) { return (uint)FinalMethod.Invoke(writer, new object[0]); }
    static string Error(Exception error)
    {
        while (error is TargetInvocationException) error = error.InnerException;
        var argument = error as ArgumentException;
        return error.GetType().FullName + ":" + (argument == null ? "" : argument.ParamName) + ":" + error.Message;
    }
    static string Pattern(int count)
    {
        // Keep unpaired surrogates separate here. Multiple ANSI-contracted
        // pairs inside one legacy chunk can expose uninitialized native bytes;
        // such bytes cannot be a stable independent-process hash oracle.
        const string pattern = "aZ09 \t\r\néß中\0\uD800x\uDC00";
        var result = new char[count];
        for (int index = 0; index < count; index++) result[index] = pattern[index % pattern.Length];
        return new string(result);
    }
    static void Step(List<string[]> rows, string name, int ordinal, TextWriter writer, Action action)
    {
        string outcome = "OK"; try { action(); } catch (Exception error) { outcome = Error(error); }
        uint first = Final(writer), second = Final(writer);
        rows.Add(new[] { name, ordinal.ToString(CultureInfo.InvariantCulture), outcome,
            first.ToString(CultureInfo.InvariantCulture), second.ToString(CultureInfo.InvariantCulture), writer.Encoding.CodePage.ToString(CultureInfo.InvariantCulture) });
    }
    static List<string[]> Corpus()
    {
        var rows = new List<string[]>();
        foreach (uint seed in new[] { 0u, 1u, 0x87654321u, UInt32.MaxValue })
        foreach (int count in new[] { 0, 1, 31, 511, 512, 513, 1023, 1024, 1025, 4097, 65537 })
        foreach (bool array in new[] { false, true })
        {
            string name = "boundary-" + seed.ToString(CultureInfo.InvariantCulture) + "-" + count + "-" + array;
            var writer = Writer(seed); string input = Pattern(count); char[] padded = ("prefix" + input + "suffix").ToCharArray();
            Step(rows, name, 0, writer, () => { if (array) writer.Write(padded, 6, count); else writer.Write(input); });
            Step(rows, name, 1, writer, writer.Flush);
            Step(rows, name, 2, writer, () => writer.Write('X'));
            Step(rows, name, 3, writer, () => writer.Write(Pattern(513)));
            Step(rows, name, 4, writer, () => writer.Write((string)null));
            Step(rows, name, 5, writer, () => writer.WriteLine("tail"));
            Step(rows, name, 6, writer, () => { writer.NewLine = "\n"; writer.WriteLine(); });
            Step(rows, name, 7, writer, writer.Dispose);
            Step(rows, name, 8, writer, () => writer.Write("after-dispose"));
        }
        foreach (int prefix in new[] { 0, 1, 511, 512, 513, 1024 })
        {
            var writer = Writer(17); string name = "arguments-" + prefix; int ordinal = 0;
            Step(rows, name, ordinal++, writer, () => writer.Write(Pattern(prefix)));
            Step(rows, name, ordinal++, writer, () => writer.Write((string)null));
            Step(rows, name, ordinal++, writer, () => writer.Write((char[])null));
            Step(rows, name, ordinal++, writer, () => writer.Write((char[])null, 0, 0));
            Step(rows, name, ordinal++, writer, () => writer.Write(new char[3], -1, 1));
            Step(rows, name, ordinal++, writer, () => writer.Write(new char[3], 0, -1));
            Step(rows, name, ordinal++, writer, () => writer.Write(new char[3], 4, 0));
            Step(rows, name, ordinal++, writer, () => writer.Write(new char[3], Int32.MaxValue, Int32.MaxValue));
            Step(rows, name, ordinal++, writer, () => writer.Write(new char[0], 0, 0));
            Step(rows, name, ordinal++, writer, () => writer.Write((object)null));
            Step(rows, name, ordinal++, writer, () => writer.Write('Y'));
            Step(rows, name, ordinal++, writer, () => writer.Write((string)null));
            Step(rows, name, ordinal++, writer, () => writer.Write(Pattern(777).ToCharArray(), 7, 769));
        }
        var fragments = Writer(0x13579BDF); string stream = Pattern(4097); int position = 0, step = 0;
        while (position < stream.Length)
        {
            int take = Math.Min((step * 37) % 601 + 1, stream.Length - position), offset = position;
            if (step % 3 == 0 && take > 1) take = 1;
            int length = take;
            Step(rows, "mixed-fragments", step, fragments, () => {
                if (length == 1) fragments.Write(stream[offset]);
                else if (offset % 2 == 0) fragments.Write(stream.ToCharArray(), offset, length);
                else fragments.Write(stream.Substring(offset, length));
            });
            position += take; step++;
        }
        var oneChar = Writer(123); string chars = Pattern(1025);
        for (int index = 0; index < chars.Length; index++) oneChar.Write(chars[index]);
        Step(rows, "1025-character-overload-fragments", 0, oneChar, oneChar.Flush);
        var pair = Writer(91);
        string splitPair = new string('A', 511) + "\uD83D\uDE80" + new string('B', 511);
        Step(rows, "surrogate-pair-split-exactly-between-512-chunks", 0, pair, () => pair.Write(splitPair));
        Step(rows, "surrogate-pair-split-exactly-between-512-chunks", 1, pair, () => pair.Write("after"));
        var singlePair = Writer(19);
        Step(rows, "single-surrogate-pair-with-native-NUL-boundary", 0, singlePair, () => singlePair.Write("\uD83D\uDE80"));
        // Public production entry serializes actual XML through XmlWriter into
        // the internal writer; include escaping, metadata and large flushes.
        foreach (uint seed in new[] { 0u, UInt32.MaxValue })
        foreach (int length in new[] { 0, 1, 127, 511, 512, 513, 8191, 32769 })
        {
            XmlNode node = Xml(length); string outcome = "OK"; uint hash = 0, again = 0;
            try { hash = HashProvider.GetXmlHash(seed, ref node); again = HashProvider.GetXmlHash(seed, ref node); }
            catch (Exception error) { outcome = Error(error); }
            rows.Add(new[] { "public-XML-" + seed.ToString(CultureInfo.InvariantCulture) + "-" + length, "0", outcome,
                hash.ToString(CultureInfo.InvariantCulture), again.ToString(CultureInfo.InvariantCulture), Encoding.Default.CodePage.ToString(CultureInfo.InvariantCulture) });
        }
        XmlNode pairXml = Xml(1); pairXml.AppendChild(pairXml.OwnerDocument.CreateTextNode("\uD83D\uDE80"));
        uint pairHash = HashProvider.GetXmlHash(23, ref pairXml), pairAgain = HashProvider.GetXmlHash(23, ref pairXml);
        rows.Add(new[] { "public-XML-single-surrogate-pair", "0", "OK", pairHash.ToString(CultureInfo.InvariantCulture), pairAgain.ToString(CultureInfo.InvariantCulture), Encoding.Default.CodePage.ToString(CultureInfo.InvariantCulture) });
        return rows;
    }
    static XmlNode Xml(int length)
    {
        const string valid = "abc&<>\"'é中\t\r\n";
        var document = new XmlDocument(); document.LoadXml("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><ArmorTemplate id='HashProbe'/></AssetDeclaration>");
        var body = new StringBuilder(); while (body.Length < length) body.Append(valid);
        // Public XML uses legal characters; direct writer cases above preserve
        // raw unpaired characters and a pair split at a 512-character boundary.
        string value = body.ToString();
        document.DocumentElement.FirstChild.Attributes.Append(document.CreateAttribute("Value")).Value = value;
        document.DocumentElement.FirstChild.AppendChild(document.CreateTextNode(value));
        document.DocumentElement.AppendChild(document.CreateComment("comment"));
        document.DocumentElement.AppendChild(document.CreateCDataSection("raw<>&é中"));
        return document.DocumentElement;
    }
#if HASHING_ORIGINAL
    public static int Main(string[] args)
    {
        try
        {
            var rows = Corpus(); string assemblySha;
            using (var sha = SHA256.Create()) assemblySha = Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(typeof(HashProvider).Assembly.Location)));
            File.WriteAllText(args[0], Json.Serialize(new { succeeded = true, sourceCommit = args[1], runId = args[2], assemblySha256Base64 = assemblySha, rows }));
            Console.WriteLine("PASS HashingWriterOriginal: " + rows.Count); return 0;
        }
        catch (Exception error) { Console.Error.WriteLine(error); return 1; }
    }
#else
    static void Differential(string path, string source, string run)
    {
        var original = Json.Deserialize<Dictionary<string, object>>(File.ReadAllText(path));
        Require((bool)original["succeeded"] && (string)original["sourceCommit"] == source && (string)original["runId"] == run, "Original hashing transcript identity mismatch");
        var expected = Json.Deserialize<List<string[]>>(Json.Serialize(original["rows"]));
        foreach (bool disabled in new[] { false, true })
        {
            AppContext.SetSwitch(Disable, disabled); var actual = Corpus();
            Require(expected.Count == actual.Count && actual.Count > 850, "Original hashing corpus size/identity mismatch");
            for (int index = 0; index < actual.Count; index++)
                Require(actual[index].SequenceEqual(expected[index]), "Original/" + (disabled ? "legacy" : "buffered") + " hashing mismatch row " + index + ": " + Json.Serialize(actual[index]) + " != " + Json.Serialize(expected[index]));
            Require(actual.All(row => row[3] == row[4]), "Repeated GetFinalHash mutated pending/running state");
            evidence.Add(new { id = "isolated-original-DLL-hashing-corpus", disabled, rows = actual.Count, encoding = Encoding.Default.CodePage });
        }
        AppContext.SetSwitch(Disable, false);
        tests.Add("isolated-original-DLL-vs-default-and-legacy-exact-512-UTF16-ANSI-unicode-overload-error-repeated-final-flush-continue-and-public-XML-differential");
    }
    static void BoundedScratch()
    {
        FieldInfo pending = WriterType.GetField("m_pending", BindingFlags.Instance | BindingFlags.NonPublic);
        AppContext.SetSwitch(Disable, false); var writer = Writer(0); writer.Write("small");
        Require(pending.GetValue(writer) == null, "A single small string allocated a 512-character scratch");
        var arrayWriter = Writer(0); arrayWriter.Write("small".ToCharArray(), 0, 5);
        Require(pending.GetValue(arrayWriter) == null, "A single small char-array write allocated scratch");
        writer.Write("second"); var buffer = (char[])pending.GetValue(writer);
        Require(buffer != null && buffer.Length == 512, "Fragmented pending text did not lazily allocate bounded scratch");
        for (int index = 0; index < 2048; index++) writer.Write('a');
        Require(Object.ReferenceEquals(buffer, pending.GetValue(writer)), "A writer did not reuse its own bounded scratch");
        AppContext.SetSwitch(Disable, true); var legacy = Writer(0); legacy.Write("small"); legacy.Write("second");
        Require(pending.GetValue(legacy) == null, "Legacy diagnostic path allocated buffered scratch");
        AppContext.SetSwitch(Disable, false);
        tests.Add("single-small-write-keeps-original-string-path-fragmented-pending-lazily-owns-one-512-char-buffer-and-legacy-never-allocates-it");
    }
    static void Measurement()
    {
        foreach (int size in new[] { 16, 512, 32768 })
        {
            XmlNode node = Xml(size); uint expected = HashProvider.GetXmlHash(27, ref node); int iterations = size > 1000 ? 32 : 512;
            foreach (bool disabled in new[] { false, true }) { AppContext.SetSwitch(Disable, disabled); for (int index = 0; index < 4; index++) Require(HashProvider.GetXmlHash(27, ref node) == expected, "XML hashing warmup mismatch"); }
            var samples = new List<object>(); var buffered = new List<long>(); var legacy = new List<long>();
            for (int round = 0; round < 7; round++)
            foreach (bool disabled in (round % 2 == 0 ? new[] { false, true } : new[] { true, false }))
            {
                AppContext.SetSwitch(Disable, disabled); uint value = 0; long start = Stopwatch.GetTimestamp();
                for (int index = 0; index < iterations; index++) value ^= HashProvider.GetXmlHash(27, ref node);
                long elapsed = Stopwatch.GetTimestamp() - start;
                Require(value == (iterations % 2 == 0 ? 0 : expected), "Timed public XML hash result changed");
                (disabled ? legacy : buffered).Add(elapsed);
                samples.Add(new { round, disabled, elapsedMs = elapsed * 1000.0 / Stopwatch.Frequency });
            }
            buffered.Sort(); legacy.Sort();
            evidence.Add(new { id = "public-XML-hash-component-measurement", requestedContentChars = size, actualSerializedChars = node.OuterXml.Length, iterations, rounds = 7,
                bufferedMedianMs = buffered[3] * 1000.0 / Stopwatch.Frequency, legacyMedianMs = legacy[3] * 1000.0 / Stopwatch.Frequency, samples = samples.ToArray(),
                scope = "Actual public HashProvider.GetXmlHash including XmlWriter serialization and legacy ANSI native hash calls; excludes DOM creation/reflection/identity checks. No forced GC, speed assertion, allocation-byte or full-CLI speedup claim" });
        }
        AppContext.SetSwitch(Disable, false); tests.Add("bounded-small-medium-large-public-XML-hashing-seven-round-component-samples-with-no-speed-threshold");
    }
    public static int Main(string[] args)
    {
        object result; int code = 0; bool previous; AppContext.TryGetSwitch(Disable, out previous);
        try
        {
            Require(args.Length == 6 && IntPtr.Size == 4 && Path.DirectorySeparatorChar == '\\', "Expected Windows x86 work/result/source/run/SDK/originalHashingTranscript");
            Differential(args[5], args[2], args[3]); BoundedScratch(); Measurement();
            result = new { succeeded = true, sourceCommit = args[2], runId = args[3], checks, tests = tests.ToArray(), evidence = evidence.ToArray(),
                scope = "Original-only Core hashing transcript versus buffered/legacy TextWriter and public GetXmlHash; exact legacy 512 UTF-16 chunks and native ANSI behavior retained" };
            Console.WriteLine("PASS HashingWriter: " + tests.Count + " groups; " + checks + " checks");
        }
        catch (Exception error) { code = 1; Console.Error.WriteLine(error); result = new { succeeded = false, sourceCommit = args.Length > 2 ? args[2] : null, runId = args.Length > 3 ? args[3] : null, checks, error = error.ToString() }; }
        finally { AppContext.SetSwitch(Disable, previous); }
        if (args.Length > 1) File.WriteAllText(args[1], Json.Serialize(result)); return code;
    }
#endif
}
