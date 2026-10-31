package newsdk.fla;

import com.jpexs.flash.fla.reader.*;
import java.io.IOException;
import java.util.*;
import static newsdk.fla.FlaCompiler.*;

/** Uses the FLA's stored shape correspondence, rather than matching unrelated contours. */
final class MorphCompiler {
    private final ShapeCompiler shapes;
    private final Map<FlaRecord, Map<Double, List<FlaCompiler.Placement>>> cache = new IdentityHashMap<>();
    MorphCompiler(FlaCompiler compiler) { shapes = new ShapeCompiler(compiler); }

    List<FlaCompiler.Placement> frame(FlaRecord frame, double t) throws IOException {
        double key = t;
        Map<Double, List<FlaCompiler.Placement>> frames = cache.computeIfAbsent(frame, unused -> new HashMap<Double, List<FlaCompiler.Placement>>());
        List<FlaCompiler.Placement> old = frames.get(t);
        if (old != null) { List<FlaCompiler.Placement> copies = new ArrayList<>(); for (FlaCompiler.Placement p : old) copies.add(new FlaCompiler.Placement(p.id, p.record)); return copies; }
        FlaRecord raw = record(frame, "morphShape"); FlaMorphShape morph = raw == null ? null : raw.getMorphShape();
        if (morph == null || morph.getSegments() == null || morph.getMatrixA() == null || morph.getMatrixB() == null)
            throw unsupported(frame, "Shape tween lacks correspondence data");
        t = TweenCompiler.ease(frame, t, 0);
        List<FlaRecord> palette = records(raw, "fills"), strokes = records(raw, "strokes");
        List<FlaRecord> fills = new ArrayList<>(), lines = new ArrayList<>(), edges = new ArrayList<>();
        for (FlaMorphShape.Segment segment : morph.getSegments()) {
            int fill = 0, line = 0;
            if (segment.getFillIndexA() == null || segment.getFillIndexB() == null || segment.getStrokeIndexA() == null || segment.getStrokeIndexB() == null)
                throw unsupported(raw, "Incomplete morph style indices");
            if (segment.getFillIndexA() >= 0 || segment.getFillIndexB() >= 0) {
                FlaRecord a = get(palette, segment.getFillIndexA()), b = get(palette, segment.getFillIndexB());
                fills.add(paint(a, b, t)); fill = fills.size();
            }
            if (segment.getStrokeIndexA() >= 0 || segment.getStrokeIndexB() >= 0) {
                FlaRecord a = get(strokes, segment.getStrokeIndexA()), b = get(strokes, segment.getStrokeIndexB());
                FlaRecord style = TweenCompiler.copy(a == null ? b : a);
                if (n(a, "styleType", 0) != 0 || n(b, "styleType", 0) != 0) throw unsupported(raw, "Procedural morph stroke");
                style.set("width", Math.round(lerp(n(a, "width", n(b, "width", 0)), n(b, "width", n(a, "width", 0)), t)));
                style.set("colorRgba", TweenCompiler.colorLerp(n(a, "colorRgba", 0), n(b, "colorRgba", 0), t));
                lines.add(style); line = lines.size();
            }
            double[] start = segment.interpolateStartPixels(t); double x = start[0] * 20, y = start[1] * 20;
            for (FlaMorphShape.Curve curve : segment.getCurves()) {
                if (curve.isLine() == null) throw unsupported(raw, "Incomplete morph curve");
                double[] end = curve.interpolateAnchorPixels(t), control = curve.interpolateControlPixels(t);
                FlaRecord edge = FlaRecord.create("edge");
                edge.set("strokeIndex", line); edge.set("fillIndex0", fill); edge.set("fillIndex1", 0);
                edge.set("startX", Math.round(x * 256)); edge.set("startY", Math.round(y * 256));
                edge.set("endX", Math.round(end[0] * 20 * 256)); edge.set("endY", Math.round(end[1] * 20 * 256));
                edge.set("controlX", Math.round(control[0] * 20 * 256)); edge.set("controlY", Math.round(control[1] * 20 * 256));
                edge.set("curve", !curve.isLine()); edges.add(edge); x = end[0] * 20; y = end[1] * 20;
            }
        }
        FlaRecord shape = FlaRecord.create("CPicShape"), data = FlaRecord.create("shapeData");
        shape.set("shapeData", data); data.set("edges", edges); data.set("fills", fills); data.set("strokes", lines);
        int[] a = morph.getMatrixA().getWords(), b = morph.getMatrixB().getWords(); List<Integer> matrix = new ArrayList<>();
        for (int i = 0; i < 6; i++) matrix.add((int) Math.round(lerp(a[i], b[i], t)));
        shape.set("matrixWords", matrix);
        int id = shapes.shape(shape); List<FlaCompiler.Placement> result = new ArrayList<>();
        if (id != 0) result.add(new FlaCompiler.Placement(id, shape)); frames.put(key, result); return result;
    }
    private static FlaRecord get(List<FlaRecord> list, int index) throws IOException {
        if (index < 0) return null; if (index >= list.size()) throw new IOException("Morph palette index out of range"); return list.get(index);
    }
    private static FlaRecord paint(FlaRecord a, FlaRecord b, double t) throws IOException {
        FlaRecord source = a == null ? b : a; if (source == null) throw new IOException("Empty morph paint");
        FlaPaintStyle.Kind ka = a == null ? FlaPaintStyle.Kind.SOLID : a.getPaintStyle().getKind();
        FlaPaintStyle.Kind kb = b == null ? FlaPaintStyle.Kind.SOLID : b.getPaintStyle().getKind();
        if (ka == FlaPaintStyle.Kind.SOLID && kb != ka) { FlaRecord swapped = paint(b, a, 1 - t); return swapped; }
        FlaRecord out = TweenCompiler.copy(source);
        if (ka == FlaPaintStyle.Kind.SOLID && kb == ka) {
            out.set("packedValue", TweenCompiler.colorLerp(n(a, "packedValue", 0), n(b, "packedValue", 0), t)); return out;
        }
        if (ka == FlaPaintStyle.Kind.LINEAR_GRADIENT || ka == FlaPaintStyle.Kind.RADIAL_GRADIENT) {
            if (kb != ka && kb != FlaPaintStyle.Kind.SOLID) throw unsupported(a, "Morph changes gradient type");
            List<FlaRecord> sa = records(a, "stops"), sb = records(b, "stops"), stops = new ArrayList<>();
            if (kb != FlaPaintStyle.Kind.SOLID && sa.size() != sb.size()) throw unsupported(a, "Morph gradients have different stop counts");
            for (int i = 0; i < sa.size(); i++) {
                FlaRecord stop = TweenCompiler.copy(sa.get(i));
                stop.set("colorRgba", TweenCompiler.colorLerp(n(sa.get(i), "colorRgba", 0), kb == FlaPaintStyle.Kind.SOLID ? n(b, "packedValue", 0) : n(sb.get(i), "colorRgba", 0), t));
                if (kb != FlaPaintStyle.Kind.SOLID) stop.set("ratio", Math.round(lerp(n(sa.get(i), "ratio", 0), n(sb.get(i), "ratio", 0), t)));
                stops.add(stop);
            }
            out.set("stops", stops); if (kb == ka) interpolateMatrix(out, a, b, t); return out;
        }
        if (ka == FlaPaintStyle.Kind.BITMAP && kb == ka && n(a, "bitmapMediaId", -1) == n(b, "bitmapMediaId", -2)) {
            interpolateMatrix(out, a, b, t); return out;
        }
        throw unsupported(a, "Unsupported morph paint transition");
    }
    private static void interpolateMatrix(FlaRecord out, FlaRecord a, FlaRecord b, double t) throws IOException {
        if (a.getMatrix() == null || b.getMatrix() == null) throw unsupported(a, "Incomplete morph paint matrix");
        int[] ma = a.getMatrix().getWords(), mb = b.getMatrix().getWords(); List<Integer> words = new ArrayList<>();
        for (int i = 0; i < 6; i++) words.add((int) Math.round(lerp(ma[i], mb[i], t))); out.set("matrixWords", words);
    }
    private static double lerp(double a, double b, double t) { return a + (b - a) * t; }
}
