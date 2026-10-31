package newsdk.fla;

import com.jpexs.flash.fla.reader.*;
import java.io.IOException;
import java.util.*;
import static newsdk.fla.FlaCompiler.*;

/** Preserves Flash's two-sided fill topology, including holes and shared contours. */
final class ShapeCompiler {
    private final FlaCompiler compiler;
    ShapeCompiler(FlaCompiler compiler) { this.compiler = compiler; }

    int shape(FlaRecord owner) throws IOException {
        FlaRecord data = record(owner, "shapeData");
        List<FlaRecord> edges = records(data, "edges");
        if (edges.isEmpty()) return 0;
        if (!data.isComplete()) throw unsupported(owner, "Incomplete shape");
        List<FlaRecord> fills = records(data, "fills"), strokes = records(data, "strokes");
        int minX = Integer.MAX_VALUE, minY = minX, maxX = Integer.MIN_VALUE, maxY = maxX, extent = 0;
        for (FlaRecord e : edges) {
            for (String p : Boolean.TRUE.equals(e.get("curve"))
                    ? new String[]{"start", "control", "end"} : new String[]{"start", "end"}) {
                int x = coordinate(e, p + "X"), y = coordinate(e, p + "Y");
                minX = Math.min(minX, x); maxX = Math.max(maxX, x);
                minY = Math.min(minY, y); maxY = Math.max(maxY, y);
            }
        }
        for (FlaRecord s : strokes) extent = Math.max(extent, (int) Math.ceil(n(s, "width", 0) / 2.0
                * Math.max(1, n(s, "joinStyle", 1) == 0 ? n(s, "miterLimitWord", 256) / 256.0 : 1)));
        int id = compiler.nextId();
        SwfWriter out = new SwfWriter(); out.u16(id);
        out.rect(minX - extent, maxX + extent, minY - extent, maxY + extent);
        boolean nonScaling = false, scaling = false;
        for (FlaRecord stroke : strokes) {
            nonScaling |= n(stroke, "scaleMode", 0) != 0;
            scaling |= n(stroke, "scaleMode", 0) != 3;
        }
        out.rect(minX, maxX, minY, maxY); out.u8((nonScaling ? 2 : 0) | (scaling ? 1 : 0));
        count(out, fills.size());
        for (FlaRecord f : fills) fill(out, f);
        count(out, strokes.size());
        for (FlaRecord s : strokes) stroke(out, s);
        int fillBits = SwfWriter.indexBits(fills.size()), lineBits = SwfWriter.indexBits(strokes.size());
        if (fillBits > 15 || lineBits > 15) throw unsupported(owner, "Too many shape styles");
        out.bits(fillBits, 4); out.bits(lineBits, 4);
        for (FlaRecord e : edges) {
            int x = coordinate(e, "startX"), y = coordinate(e, "startY");
            int moveBits = SwfWriter.signedBits(x, y);
            if (moveBits > 31) throw unsupported(owner, "Shape coordinate out of range");
            out.bits(0, 1); out.bits(15, 5);
            out.bits(moveBits, 5); out.bits(x, moveBits); out.bits(y, moveBits);
            int f0 = (int) n(e, "fillIndex0", 0) & 0x7fff, f1 = (int) n(e, "fillIndex1", 0) & 0x7fff;
            int line = (int) n(e, "strokeIndex", 0) & 0x7fff;
            if (f0 > fills.size() || f1 > fills.size() || line > strokes.size())
                throw unsupported(e, "Missing shape style");
            out.bits(f0, fillBits); out.bits(f1, fillBits); out.bits(line, lineBits);
            int ex = coordinate(e, "endX"), ey = coordinate(e, "endY");
            if (Boolean.TRUE.equals(e.get("curve")))
                curve(out, x, y, coordinate(e, "controlX"), coordinate(e, "controlY"), ex, ey, 0);
            else line(out, ex - x, ey - y);
        }
        out.bits(0, 6); out.align(); compiler.define(83, out);
        return id;
    }
    private static int coordinate(FlaRecord r, String key) { return (int) Math.round(n(r, key, 0) / 256.0); }
    private static void count(SwfWriter out, int n) throws IOException {
        if (n > 65535) throw new IOException("Too many SWF styles");
        out.u8(Math.min(n, 255)); if (n >= 255) out.u16(n);
    }
    private void fill(SwfWriter out, FlaRecord r) throws IOException {
        FlaPaintStyle s = r.getPaintStyle();
        if (s == null) throw unsupported(r, "Incomplete paint style");
        switch (s.getKind()) {
            case SOLID: out.u8(0); out.rgba(s.getRgbaWord()); break;
            case LINEAR_GRADIENT:
            case RADIAL_GRADIENT:
                boolean focal = s.getKind() == FlaPaintStyle.Kind.RADIAL_GRADIENT
                        && s.getFocalRatio() != null && s.getFocalRatio() != 0;
                out.u8(s.getKind() == FlaPaintStyle.Kind.LINEAR_GRADIENT ? 0x10 : focal ? 0x13 : 0x12);
                if (s.getMatrix() == null || s.getStops() == null || s.getStops().isEmpty())
                    throw unsupported(r, "Incomplete gradient");
                out.matrix(s.getMatrix());
                List<FlaPaintStyle.Stop> stops = s.getStops();
                if (stops.size() > 15) throw unsupported(r, "Gradient exceeds 15 stops");
                out.u8((s.getSpread() == FlaPaintStyle.Spread.REFLECT ? 64
                        : s.getSpread() == FlaPaintStyle.Spread.REPEAT ? 128 : 0)
                        | (Boolean.TRUE.equals(s.isLinearRgb()) ? 16 : 0) | stops.size());
                for (FlaPaintStyle.Stop stop : stops) { out.u8(stop.getRatioByte()); out.rgba(stop.getRgbaWord()); }
                if (focal) {
                    if (Math.abs(s.getFocalRatio()) > 1) throw unsupported(r, "Focal gradient ratio out of range");
                    out.u16((int) Math.round(s.getFocalRatio() * 256));
                }
                break;
            case BITMAP:
                FlaCompiler.Bitmap bitmap = compiler.bitmap(s.getBitmapMediaId());
                out.u8((Boolean.TRUE.equals(s.isBitmapSmoothed()) ? 0x40 : 0x42)
                        + (Boolean.TRUE.equals(s.isBitmapClipped()) ? 1 : 0));
                out.u16(bitmap.id); out.matrix(s.getMatrix()); break;
            default: throw unsupported(r, "Unsupported fill: " + s.getKind());
        }
    }
    private void stroke(SwfWriter out, FlaRecord r) throws IOException {
        FlaStrokeStyle s = r.getStrokeStyle();
        if (s == null || s.getKind() != FlaStrokeStyle.Kind.SOLID)
            throw unsupported(r, "Procedural strokes require outline expansion");
        out.u16(s.getWidthTwips());
        int cap = (int) n(r, "capStyle", 1), join = (int) n(r, "joinStyle", 1), scale = (int) n(r, "scaleMode", 0);
        if (cap < 0 || cap > 2 || join < 0 || join > 2 || scale < 0 || scale > 3)
            throw unsupported(r, "Unknown stroke options");
        int swfCap = cap == 0 ? 1 : cap == 1 ? 0 : 2, swfJoin = join == 0 ? 2 : join == 1 ? 0 : 1;
        FlaRecord paint = record(r, "paint");
        out.bits(swfCap, 2); out.bits(swfJoin, 2); out.bits(paint == null ? 0 : 1, 1);
        out.bits(scale == 2 || scale == 3 ? 1 : 0, 1); out.bits(scale == 1 || scale == 3 ? 1 : 0, 1);
        out.bits(n(r, "pixelHinting", 0) != 0 ? 1 : 0, 1); out.bits(0, 5); out.bits(0, 1); out.bits(swfCap, 2);
        if (swfJoin == 2) out.u16((int) n(r, "miterLimitWord", 768));
        if (paint == null) out.rgba(n(r, "colorRgba", 0xff000000L)); else fill(out, paint);
    }
    static void line(SwfWriter out, int dx, int dy) throws IOException {
        int n = Math.max(2, SwfWriter.signedBits(dx, dy));
        if (n > 17) { int ax = dx / 2, ay = dy / 2; line(out, ax, ay); line(out, dx - ax, dy - ay); return; }
        out.bits(1, 1); out.bits(1, 1); out.bits(n - 2, 4); out.bits(1, 1); out.bits(dx, n); out.bits(dy, n);
    }
    private static void curve(SwfWriter out, int x, int y, int cx, int cy, int ex, int ey, int depth) throws IOException {
        int n = Math.max(2, SwfWriter.signedBits(cx - x, cy - y, ex - cx, ey - cy));
        if (n > 17) {
            if (depth > 24) throw new IOException("Cannot subdivide large quadratic curve");
            int a = (int) Math.round((x + (double) cx) / 2), b = (int) Math.round((y + (double) cy) / 2);
            int c = (int) Math.round((cx + (double) ex) / 2), d = (int) Math.round((cy + (double) ey) / 2);
            int mx = (int) Math.round((a + (double) c) / 2), my = (int) Math.round((b + (double) d) / 2);
            curve(out, x, y, a, b, mx, my, depth + 1); curve(out, mx, my, c, d, ex, ey, depth + 1); return;
        }
        out.bits(1, 1); out.bits(0, 1); out.bits(n - 2, 4);
        out.bits(cx - x, n); out.bits(cy - y, n); out.bits(ex - cx, n); out.bits(ey - cy, n);
    }
}
