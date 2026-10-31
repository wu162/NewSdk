using System;
using System.IO;
using BinaryAssetBuilder.UiCompiler.Fla;

namespace BinaryAssetBuilder.UiCompiler.Swf
{
    internal sealed class ShapeCompiler
    {
        private readonly FlaCompiler compiler;
        internal ShapeCompiler(FlaCompiler compiler) { this.compiler = compiler; }
        internal int Shape(FlaRecord owner)
        {
            var data = owner.Record("shapeData"); if (data == null || data.Records("edges").Count == 0) return 0;
            data = StrokeExpander.Expand(data);
            var edges = data.Records("edges"); var fills = data.Records("fills"); var strokes = data.Records("strokes");
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue, extent = 0;
            foreach (var e in edges) foreach (string p in Equals(e.get("curve"), true) ? new[] { "start", "control", "end" } : new[] { "start", "end" })
            { int x = Coordinate(e, p + "X"), y = Coordinate(e, p + "Y"); minX = Math.Min(minX, x); maxX = Math.Max(maxX, x); minY = Math.Min(minY, y); maxY = Math.Max(maxY, y); }
            foreach (var s in strokes) extent = Math.Max(extent, checked((int)Math.Ceiling(s.Real("width") / 2 * Math.Max(1, s.Number("joinStyle", 1) == 0 ? s.Real("miterLimitWord", 256) / 256 : 1))));
            int id = compiler.NextId(); var w = new SwfWriter(); w.u16(id); w.rect(minX - extent, maxX + extent, minY - extent, maxY + extent);
            // DefineShape3 is understood by the original EA converter. Flash 8's
            // cap/join/scale extensions are validated rather than silently dropped.
            Count(w, fills.Count); foreach (var fill in fills) Fill(w, fill, new double[] { minX, maxX, minY, maxY });
            Count(w, strokes.Count); foreach (var stroke in strokes)
            {
                if (stroke.Number("styleType") != 0 || (stroke.Record("paint")?.Number("flags") ?? 0) != 0) throw new NotSupportedException("Procedural/gradient strokes require outline expansion.");
                w.u16((int)stroke.Number("width")); w.u32(stroke.Record("paint")?.Number("packedValue") ?? stroke.Number("colorRgba", 0xff000000L));
            }
            int fb = SwfWriter.indexBits(fills.Count), lb = SwfWriter.indexBits(strokes.Count);
            if (fb > 15 || lb > 15) throw new InvalidDataException("Too many shape styles."); w.bits(fb, 4); w.bits(lb, 4);
            foreach (var e in edges)
            {
                int x = Coordinate(e, "startX"), y = Coordinate(e, "startY"), n = SwfWriter.signedBits(x, y);
                if (n > 31) throw new InvalidDataException("Shape move overflow.");
                int f0 = (int)e.Number("fillIndex0") & 0x7fff, f1 = (int)e.Number("fillIndex1") & 0x7fff, line = (int)e.Number("strokeIndex") & 0x7fff;
                if (f0 > fills.Count || f1 > fills.Count || line > strokes.Count) throw new InvalidDataException("Missing shape style.");
                w.bits(0, 1); w.bits(15, 5); w.bits(n, 5); w.bits(x, n); w.bits(y, n); w.bits(f0, fb); w.bits(f1, fb); w.bits(line, lb);
                if (Equals(e.get("curve"), true)) Curve(w, x, y, Coordinate(e, "controlX"), Coordinate(e, "controlY"), Coordinate(e, "endX"), Coordinate(e, "endY"));
                else Line(w, Coordinate(e, "endX") - x, Coordinate(e, "endY") - y);
            }
            w.bits(0, 6); w.align(); compiler.Define(32, w); return id;
        }
        private static int Coordinate(FlaRecord r, string key) => SwfWriter.Round(r.Real(key) / 256);
        private static void Count(SwfWriter w, int count) { if (count > 65535) throw new InvalidDataException("Too many styles."); w.u8(Math.Min(count, 255)); if (count >= 255) w.u16(count); }
        private void Fill(SwfWriter w, FlaRecord r, double[] bounds)
        {
            int flags = (int)r.Number("flags");
            if ((flags & 0x40) != 0)
            { var bitmap = compiler.Bitmap((int)r.Number("bitmapMediaId")); w.u8(0x40 | (flags & 3)); w.u16(bitmap.Id); w.matrix(r.Matrix()); }
            else if ((flags & 0x10) != 0)
            {
                int bitmap = GradientRasterizer.Compile(compiler, r, bounds, out var matrix); w.u8(0x41); w.u16(bitmap); w.matrix(matrix);
            }
            else if ((flags & 0x20) != 0) throw new NotSupportedException("Extended fill style.");
            else { w.u8(0); w.u32(r.Number("packedValue")); }
        }
        internal static void Line(SwfWriter w, int dx, int dy)
        {
            int n = Math.Max(2, SwfWriter.signedBits(dx, dy));
            if (n > 17) { int x = dx / 2, y = dy / 2; Line(w, x, y); Line(w, dx - x, dy - y); return; }
            w.bits(1, 1); w.bits(1, 1); w.bits(n - 2, 4); w.bits(1, 1); w.bits(dx, n); w.bits(dy, n);
        }
        private static void Curve(SwfWriter w, int x, int y, int cx, int cy, int ex, int ey, int depth = 0)
        {
            int n = Math.Max(2, SwfWriter.signedBits(cx - x, cy - y, ex - cx, ey - cy));
            if (n > 17)
            { if (depth > 24) throw new InvalidDataException("Curve overflow."); int a = SwfWriter.Round((x + (double)cx) / 2), b = SwfWriter.Round((y + (double)cy) / 2), c = SwfWriter.Round((cx + (double)ex) / 2), d = SwfWriter.Round((cy + (double)ey) / 2), mx = SwfWriter.Round((a + (double)c) / 2), my = SwfWriter.Round((b + (double)d) / 2); Curve(w, x, y, a, b, mx, my, depth + 1); Curve(w, mx, my, c, d, ex, ey, depth + 1); return; }
            w.bits(1, 1); w.bits(0, 1); w.bits(n - 2, 4); w.bits(cx - x, n); w.bits(cy - y, n); w.bits(ex - cx, n); w.bits(ey - cy, n);
        }
    }
}
