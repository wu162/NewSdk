using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.UiCompiler.Fla;

namespace BinaryAssetBuilder.UiCompiler.Swf
{
    internal sealed class FlaCompiler
    {
        private readonly FlaDocument document;
        private readonly SwfWriter definitions = new SwfWriter();
        private readonly ShapeCompiler shapes;
        private readonly Dictionary<long, FlaRecord> library, media;
        private readonly Dictionary<string, int> symbols = new Dictionary<string, int>();
        private readonly HashSet<FlaRecord> compiling = new HashSet<FlaRecord>();
        private readonly Dictionary<FlaRecord, int> shapeIds = new Dictionary<FlaRecord, int>();
        private readonly Dictionary<long, BitmapAsset> bitmaps = new Dictionary<long, BitmapAsset>();
        private readonly Dictionary<long, int> sounds = new Dictionary<long, int>();
        private readonly Dictionary<FlaRecord, Timeline> timelines = new Dictionary<FlaRecord, Timeline>();
        private int lastId;
        internal FlaCompiler(FlaDocument document)
        {
            this.document = document; shapes = new ShapeCompiler(this);
            library = document.Library.Where(d => d.get("streamName") != null).ToDictionary(d => d.Number("libraryId", -1));
            media = document.Media.Where(d => d.get("streamName") != null).ToDictionary(d => d.Number("mediaId", -1));
            if (library.ContainsKey(-1) || media.ContainsKey(-1)) throw new InvalidDataException("Missing library identifier.");
        }
        internal int NextId() { if (lastId == 65535) throw new InvalidDataException("SWF character limit."); return ++lastId; }
        internal void Define(int tag, SwfWriter writer) => definitions.tag(tag, writer);
        internal byte[] Compile(bool compressed = false)
        {
            var timeline = new SwfWriter(); var previous = new List<Placement>(); int count = 0;
            foreach (var scene in document.Scenes)
            {
                var t = GetTimeline(scene);
                for (int f = 0; f < t.Length; f++) { var current = t.Frame(f); Display(timeline, previous, current); t.Events(timeline, f); timeline.tag(1); previous = current; if (++count > 65535) throw new InvalidDataException("Too many frames."); }
            }
            if (count == 0) throw new InvalidDataException("FLA contains no scene.");
            var exports = new List<KeyValuePair<string, int>>();
            foreach (var descriptor in library.Values.Concat(media.Values))
            {
                var link = descriptor.Record("libraryMetadata")?.Record("linkage") ?? descriptor.Record("linkage");
                if (link == null) continue; int flags = (int)link.Number("flags");
                if ((flags & 2) != 0 || link.Text("url").Length != 0) throw new NotSupportedException("Runtime shared-library imports require an external library resolver.");
                if (link.Text("className").Length != 0) throw new NotSupportedException("External AS2 class linkage requires a classpath compiler.");
                if ((flags & 1) != 0) exports.Add(new KeyValuePair<string, int>(link.Text("identifier"), library.ContainsValue(descriptor) ? Symbol(descriptor, -1, descriptor.Number("symbolType") == 1) : descriptor.ClassName == "CMediaBits" ? Bitmap(descriptor.Number("mediaId")).Id : Sound(descriptor.Number("mediaId"))));
            }
            var body = new SwfWriter(); int[] bounds = document.Contents.Words("boundsWords", 4);
            int width = bounds[1] - bounds[0], height = bounds[3] - bounds[2]; double rate = document.Contents.Real("frameRateWord") / 65536;
            if (width <= 0 || height <= 0 || rate <= 0 || rate >= 256) throw new InvalidDataException("Invalid stage dimensions/frame rate.");
            body.rect(0, width, 0, height); body.u16(SwfWriter.Round(rate * 256)); body.u16(count);
            var attrs = new SwfWriter(); attrs.u32(0); body.tag(69, attrs);
            var background = new SwfWriter(); long c = document.Contents.Number("backgroundColorRgba", 0xffffffffL); background.u8((int)c); background.u8((int)(c >> 8)); background.u8((int)(c >> 16)); body.tag(9, background);
            body.bytes(definitions.data());
            if (exports.Count > 0)
            { var e = new SwfWriter(); e.u16(exports.Count); var names = new HashSet<string>(StringComparer.Ordinal); foreach (var item in exports) { if (item.Key.Length == 0 || !names.Add(item.Key)) throw new InvalidDataException("Missing/duplicate linkage identifier."); e.u16(item.Value); e.text(item.Key); } body.tag(56, e); }
            body.bytes(timeline.data()); body.tag(0); return SwfWriter.movie(body.data(), compressed);
        }
        private Timeline GetTimeline(FlaRecord descriptor)
        { if (!timelines.TryGetValue(descriptor, out Timeline result)) { result = new Timeline(this, document.Page(descriptor)); timelines.Add(descriptor, result); } return result; }
        private int Shape(FlaRecord record) { if (!shapeIds.TryGetValue(record, out int id)) shapeIds.Add(record, id = shapes.Shape(record)); return id; }
        private int Symbol(FlaRecord descriptor, int frame = -1, bool button = false)
        {
            string key = descriptor.Number("libraryId") + ":" + frame + ":" + button;
            if (compiling.Contains(descriptor)) throw new NotSupportedException("Recursive symbol graph.");
            if (symbols.TryGetValue(key, out int id)) return id;
            if (descriptor.Record("component")?.Number("componentEnabled") != null && descriptor.Record("component").Number("componentEnabled") != 0) throw new NotSupportedException("Precompiled components require source symbols.");
            compiling.Add(descriptor); id = NextId(); symbols.Add(key, id);
            try
            {
                var t = GetTimeline(descriptor); var w = new SwfWriter(); w.u16(id);
                if (button)
                {
                    if (t.Length > 4) throw new InvalidDataException("Button has more than four states."); w.u8(0); w.u16(0);
                    for (int f = 0; f < t.Length; f++) foreach (var p in t.Frame(f))
                    { RejectVisualEffects(p.Record); if (Source(p.Record).Length != 0) throw new NotSupportedException("Button child actions."); w.u8(1 << f); w.u16(p.Id); w.u16(p.Depth); w.matrix(p.Matrix); WriteColor(w, p.Record, true); }
                    w.u8(0); Define(34, w);
                }
                else
                {
                    int length = frame < 0 ? t.Length : 1; w.u16(length); var previous = new List<Placement>();
                    for (int f = 0; f < length; f++) { var current = t.Frame(frame < 0 ? f : frame); Display(w, previous, current); t.Events(w, frame < 0 ? f : frame); if (frame >= 0) w.tag(12, ActionScriptCompiler.Compile("stop();")); w.tag(1); previous = current; }
                    w.tag(0); Define(39, w);
                }
                if (descriptor.Number("scaleGridEnabled") != 0) { var grid = new SwfWriter(); grid.u16(id); int[] b = descriptor.Words("scaleGridBounds", 4); grid.rect(b[0], b[1], b[2], b[3]); Define(78, grid); }
                return id;
            }
            finally { compiling.Remove(descriptor); }
        }
        private void Objects(List<Placement> output, FlaRecord r, int age)
        {
            int id;
            switch (r.ClassName)
            {
                case "CPicFrame": case "CPicShape": foreach (var child in r.getChildren()) Objects(output, child, age); id = Shape(r); if (id != 0) output.Add(new Placement(id, r)); return;
                case "CPicSymbol": case "CPicShapeObj": case "CPicSprite": case "CPicButton":
                    if (!library.TryGetValue(r.Number("libraryId", -1), out var descriptor)) throw new InvalidDataException("Missing library symbol.");
                    bool graphic = r.ClassName == "CPicSymbol" || r.ClassName == "CPicShapeObj";
                    int frame = -1;
                    if (graphic) { int length = GetTimeline(descriptor).Length, first = (int)r.Number("firstFrame"), mode = (int)r.Number("loopMode"); if (first < 0 || first >= length || mode < 0 || mode > 2) throw new InvalidDataException("Invalid graphic playback settings."); frame = mode == 2 ? first : mode == 1 ? Math.Min(first + age, length - 1) : (first + age) % length; }
                    id = Symbol(descriptor, frame, r.ClassName == "CPicButton"); break;
                case "CPicBitmap": id = Bitmap(r.Number("mediaId", -1)).ShapeId; break;
                case "CPicText": id = new TextCompiler(this).Compile(r); break;
                default: throw new NotSupportedException("Display object " + r.ClassName);
            }
            output.Add(new Placement(id, r));
        }
        internal sealed class Placement
        {
            internal int Id, Depth, ClipDepth; internal FlaRecord Record; internal double[] Matrix;
            internal Placement(int id, FlaRecord record) { Id = id; Record = record; Matrix = record.Matrix(); }
        }
        private void Display(SwfWriter w, List<Placement> before, List<Placement> after)
        {
            var old = before.ToDictionary(p => p.Depth); var depths = new HashSet<int>(after.Select(p => p.Depth));
            foreach (var p in before) if (!depths.Contains(p.Depth)) { var remove = new SwfWriter(); remove.u16(p.Depth); w.tag(28, remove); }
            foreach (var p in after)
            { old.TryGetValue(p.Depth, out var prev); byte[] state = Place(p, false, true); if (prev == null || !state.SequenceEqual(Place(prev, false, true))) w.tag(26, Place(p, prev != null, prev == null || prev.Id != p.Id)); }
        }
        private byte[] Place(Placement p, bool move, bool character)
        {
            RejectVisualEffects(p.Record); string name = p.Record.Text("instanceName"); byte[] clips = ActionScriptCompiler.CompileHandlers(Source(p.Record), p.Record.ClassName == "CPicButton");
            var color = Color(p.Record); var w = new SwfWriter(); w.u8((clips.Length > 0 ? 128 : 0) | (p.ClipDepth != 0 ? 64 : 0) | (name.Length > 0 ? 32 : 0) | (color != null ? 8 : 0) | 4 | (character ? 2 : 0) | (move ? 1 : 0));
            w.u16(p.Depth); if (character) w.u16(p.Id); w.matrix(p.Matrix); if (color != null) w.bytes(color); if (name.Length > 0) w.text(name); if (p.ClipDepth != 0) w.u16(p.ClipDepth); if (clips.Length > 0) w.bytes(clips); return w.data();
        }
        private static void RejectVisualEffects(FlaRecord r)
        { if (r.Number("blendMode", 1) > 1 || (r.Record("filters")?.Records("entries") ?? new List<FlaRecord>()).Any(f => f.Number("enabled", 1) != 0)) throw new NotSupportedException("Flash filters/blend modes are not representable by the RA3 APT runtime."); }
        private static void WriteColor(SwfWriter w, FlaRecord r, bool identity) { var data = Color(r); if (data != null) w.bytes(data); else if (identity) { w.bits(0, 6); w.align(); } }
        private static byte[] Color(FlaRecord owner)
        {
            var r = owner.Record("colorTransform"); if (r == null) return null; int type = (int)r.Number("effectType", owner.Number("colorEffectType", 3)); if (type == 0) return null;
            int[] mult = { 256, 256, 256, 256 }, add = new int[4]; string[] channel = { "red", "green", "blue", "alpha" };
            for (int i = 0; i < 4; i++) { mult[i] = (int)r.Number(channel[i] + "Multiplier", 256); add[i] = (int)r.Number(channel[i] + "Offset"); }
            if (type == 1 || type == 2) { double amount = r.Real("effectPercent", owner.Real("colorEffectPercent")) / 100; if (Math.Abs(amount) > 1) throw new InvalidDataException("Color transform percentage."); long tint = r.Number("effectColorRgba", owner.Number("colorEffectRgba")); for (int i = 0; i < 3; i++) { mult[i] = SwfWriter.Round((1 - Math.Abs(amount)) * 256); add[i] = SwfWriter.Round(type == 1 ? Math.Max(0, amount) * 255 : (tint >> (8 * i) & 255) * amount); } }
            else if (type == 4) { for (int i = 0; i < 3; i++) { mult[i] = 256; add[i] = 0; } add[3] = 0; }
            bool hm = mult.Any(v => v != 256), ha = add.Any(v => v != 0); if (!hm && !ha) return null; int n = Math.Max(hm ? SwfWriter.signedBits(mult) : 0, ha ? SwfWriter.signedBits(add) : 0); if (n > 15) throw new InvalidDataException("Color transform overflow.");
            var w = new SwfWriter(); w.bits(ha ? 1 : 0, 1); w.bits(hm ? 1 : 0, 1); w.bits(n, 4); if (hm) foreach (int v in mult) w.bits(v, n); if (ha) foreach (int v in add) w.bits(v, n); return w.data();
        }
        internal static string Source(FlaRecord r)
        { var actions = r.Record("actions"); if (actions == null) return ""; if (actions.get("source") is string source) return source; if (actions.Records("commands").Count != 0 || actions.Record("compiledCommands") != null) throw new NotSupportedException("Archived editor-command actions need source reconstruction."); return ""; }
        private sealed class Timeline
        {
            private readonly FlaCompiler compiler; private readonly List<FlaRecord> layers; private readonly int[] bases, capacities;
            internal readonly int Length;
            internal Timeline(FlaCompiler compiler, FlaRecord page)
            {
                this.compiler = compiler; layers = page.getChildren().ToList(); int length = 1, depth = 1;
                foreach (var mask in layers.ToArray()) if (mask.getLayerType() == 4) { int first = layers.IndexOf(mask); foreach (var child in layers) if (child.getLayerParent() == mask) first = Math.Min(first, layers.IndexOf(child)); layers.Remove(mask); layers.Insert(first, mask); }
                bases = new int[layers.Count]; capacities = new int[layers.Count];
                for (int i = 0; i < layers.Count; i++)
                { if (layers[i].ClassName != "CPicLayer") throw new InvalidDataException("Invalid timeline layer."); int end = 0, capacity = 1; foreach (var f in layers[i].getChildren()) { int d = (int)f.Number("duration", 1); if (d <= 0 || end + d > 65535) throw new InvalidDataException("Invalid frame duration."); end += d; capacity = Math.Max(capacity, ObjectCount(f)); } bases[i] = depth; capacities[i] = capacity; depth += capacity; length = Math.Max(length, end); }
                if (depth > 65535) throw new InvalidDataException("SWF depth limit."); Length = length;
            }
            private static int ObjectCount(FlaRecord r) => r.ClassName == "CPicFrame" || r.ClassName == "CPicShape" ? r.getChildren().Sum(ObjectCount) + (r.Record("shapeData")?.Records("edges").Count > 0 ? 1 : 0) : 1;
            internal List<Placement> Frame(int index)
            {
                var result = new List<Placement>();
                for (int i = 0; i < layers.Count; i++)
                {
                    int type = layers[i].getLayerType(); if (type == 1 || type == 3) continue; if (type != 0 && type != 2 && type != 4 && type != 5) throw new NotSupportedException("Layer type " + type);
                    int start = 0; var frames = layers[i].getChildren();
                    for (int k = 0; k < frames.Count; k++)
                    {
                        var f = frames[k]; int duration = (int)f.Number("duration", 1);
                        if (index >= start && index < start + duration)
                        {
                            var items = new List<Placement>(); int mode = (int)f.Number("keyMode") & 255;
                            if (mode == 2 && index > start) compiler.Morph(items, f, (index - start) / (double)duration); else compiler.Objects(items, f, index - start);
                            if (mode == 1 && k + 1 < frames.Count && index > start)
                            { if (layers[i].getLayerParent()?.getLayerType() == 1) throw new NotSupportedException("Guided motion paths."); var target = new List<Placement>(); compiler.Objects(target, frames[k + 1], 0); TweenCompiler.Apply(items, target, f, (index - start) / (double)duration); }
                            if (items.Count > capacities[i]) throw new InvalidDataException("Unexpected display-list expansion.");
                            for (int j = 0; j < items.Count; j++) { items[j].Depth = bases[i] + j; result.Add(items[j]); } break;
                        }
                        start += duration;
                    }
                }
                for (int i = 0; i < layers.Count; i++) if (layers[i].getLayerType() == 4)
                { int end = 0; for (int j = 0; j < layers.Count; j++) if (layers[j].getLayerParent() == layers[i]) end = Math.Max(end, bases[j] + capacities[j] - 1); var masks = result.Where(p => p.Depth >= bases[i] && p.Depth < bases[i] + capacities[i]).ToList(); if (end > 0 && masks.Count > 0) { if (masks.Count != 1 || end <= masks[0].Depth) throw new NotSupportedException("Mask requires a contiguous depth range."); masks[0].ClipDepth = end; } }
                return result;
            }
            internal void Events(SwfWriter w, int index)
            {
                var labels = new HashSet<string>();
                foreach (var layer in layers)
                {
                    if (layer.getLayerType() == 1 || layer.getLayerType() == 3) continue; int start = 0;
                    foreach (var f in layer.getChildren())
                    {
                        if (index == start)
                        { string label = f.Text("label"); if (label.Length > 0 && labels.Add(label)) { var l = new SwfWriter(); l.text(label); w.tag(43, l); } string source = Source(f); if (source.Trim().Length > 0) w.tag(12, ActionScriptCompiler.Compile(source)); if (f.Number("soundMediaId") != 0) compiler.SoundEvent(w, f); }
                        start += (int)f.Number("duration", 1);
                    }
                }
            }
        }
        internal sealed class BitmapAsset { internal int Id, ShapeId; internal BitmapAsset(int id, int shape) { Id = id; ShapeId = shape; } }
        internal BitmapAsset Bitmap(long mediaId)
        {
            if (bitmaps.TryGetValue(mediaId, out var old)) return old;
            if (!media.TryGetValue(mediaId, out var descriptor) || descriptor.ClassName != "CMediaBits") throw new InvalidDataException("Missing bitmap " + mediaId);
            byte[] argb = BitmapDecoder.Decode(document.Payload(descriptor), out int width, out int height); int id = NextId(); var image = new SwfWriter(); image.u16(id); image.u8(5); image.u16(width); image.u16(height); image.bytes(SwfWriter.compress(argb)); Define(36, image);
            var edge = new FlaRecord("edge"); var data = new FlaRecord("shapeData"); var shape = new FlaRecord("CPicShape"); var fill = new FlaRecord("fill"); fill.set("flags", 0x42); fill.set("bitmapMediaId", mediaId); fill.set("matrixWords", new List<int> { 20 * 65536, 0, 0, 20 * 65536, 0, 0 });
            var edges = new List<FlaRecord>(); int[] xs = { 0, width * 20, width * 20, 0, 0 }, ys = { 0, 0, height * 20, height * 20, 0 };
            for (int i = 0; i < 4; i++) { var e = new FlaRecord("edge"); e.set("startX", xs[i] * 256); e.set("startY", ys[i] * 256); e.set("endX", xs[i + 1] * 256); e.set("endY", ys[i + 1] * 256); e.set("fillIndex1", 1); edges.Add(e); }
            data.set("edges", edges); data.set("fills", new List<FlaRecord> { fill }); shape.set("shapeData", data);
            // Cache before the rectangle's bitmap fill resolves this same media ID.
            var result = new BitmapAsset(id, 0); bitmaps.Add(mediaId, result); result.ShapeId = shapes.Shape(shape); return result;
        }
        private int Sound(long mediaId)
        {
            if (sounds.TryGetValue(mediaId, out int id)) return id;
            if (!media.TryGetValue(mediaId, out var descriptor) || descriptor.ClassName != "CMediaSound") throw new InvalidDataException("Missing sound.");
            int flags = (int)descriptor.Number("formatFlags"); int codec = flags >> 4;
            if (codec != 0 && codec != 2 && codec != 3) throw new NotSupportedException("Sound codec " + codec);
            var primary = document.Payload(descriptor).Record("primaryAudio"); if (primary == null || !(primary.get("data") is BinaryData bytes)) throw new InvalidDataException("Missing sound data.");
            var w = new SwfWriter(); id = NextId(); w.u16(id); w.u8(flags); w.u32(descriptor.Number("sampleCount")); if (codec == 2) w.u16(0); w.bytes(bytes.toByteArray()); Define(14, w); sounds.Add(mediaId, id); return id;
        }
        private void SoundEvent(SwfWriter w, FlaRecord frame)
        {
            int sync = (int)frame.Number("soundSync"); if (sync == 3) throw new NotSupportedException("Streaming audio requires frame-aligned resampling."); var s = new SwfWriter(); s.u16(Sound(frame.Number("soundMediaId")));
            long first = frame.Number("soundInPoint"), end = frame.Number("soundOutPoint", 0x3fffffff), loops = frame.Number("soundLoopCount", 1); var envelope = frame.get("soundEnvelope") as IList;
            s.u8((sync == 2 ? 32 : sync == 1 ? 16 : 0) | (envelope?.Count > 0 ? 8 : 0) | (loops != 1 ? 4 : 0) | (end != 0x3fffffff ? 2 : 0) | (first != 0 ? 1 : 0)); if (first != 0) s.u32(first); if (end != 0x3fffffff) s.u32(end); if (loops != 1) s.u16((int)loops);
            if (envelope?.Count > 0) { s.u8(envelope.Count); foreach (IList point in envelope) { s.u32(Convert.ToInt64(point[0])); s.u16(Convert.ToInt32(point[1])); s.u16(Convert.ToInt32(point[2])); } } w.tag(15, s);
        }
        private void Morph(List<Placement> output, FlaRecord frame, double t) => MorphCompiler.Apply(this, shapes, output, frame, t);
    }
}
