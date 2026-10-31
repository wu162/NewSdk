package newsdk.fla;

import com.jpexs.flash.fla.reader.FlaRecord;
import java.io.IOException;
import java.util.*;
import static newsdk.fla.FlaCompiler.*;

/** Bakes classic motion tweens into SWF display-list updates. */
final class TweenCompiler {
    static void interpolate(PlacementPair pair, FlaRecord frame, double fraction) throws IOException {
        FlaCompiler.Placement a = pair.a, b = pair.b;
        if (a.id != b.id && n(a.record, "libraryId", -1) != n(b.record, "libraryId", -2))
            throw unsupported(frame, "Motion tween changes symbol identity");
        double position = ease(frame, fraction, 0), rotation = ease(frame, fraction, 1), scale = ease(frame, fraction, 2);
        double[] start = a.matrix.clone(), end = b.matrix;
        double[] da = decompose(start), db = decompose(end);
        double angle = db[0] - da[0];
        while (angle > Math.PI) angle -= 2 * Math.PI;
        while (angle < -Math.PI) angle += 2 * Math.PI;
        long mode = n(frame, "rotationMode", 0), turns = n(frame, "rotationCount", 0);
        if (mode == 1) { if (angle < 0) angle += Math.PI * 2; angle += turns * Math.PI * 2; }
        else if (mode == 2) { if (angle > 0) angle -= Math.PI * 2; angle -= turns * Math.PI * 2; }
        else if ((n(frame, "keyMode", 0) & 0x400) == 0) angle = 0;
        double theta = da[0] + angle * rotation;
        double sx = lerp(da[1], db[1], scale), sy = lerp(da[2], db[2], scale), skew = lerp(da[3], db[3], scale);
        if ((n(frame, "keyMode", 0) & 0x200) == 0) { sx = da[1]; sy = da[2]; skew = da[3]; }
        a.matrix = new double[]{Math.cos(theta) * sx, Math.sin(theta) * sx,
                Math.cos(theta) * skew - Math.sin(theta) * sy, Math.sin(theta) * skew + Math.cos(theta) * sy,
                lerp(start[4], end[4], position), lerp(start[5], end[5], position)};
        FlaRecord owner = copy(a.record);
        int[] ca = colorValues(a.record), cb = colorValues(b.record);
        FlaRecord color = FlaRecord.create("colorTransform"); color.set("effectType", 3L);
        String[] names = {"red", "green", "blue", "alpha"};
        for (int i = 0; i < 4; i++) {
            color.set(names[i] + "Multiplier", Math.round(lerp(ca[i], cb[i], ease(frame, fraction, 3))));
            color.set(names[i] + "Offset", Math.round(lerp(ca[i + 4], cb[i + 4], ease(frame, fraction, 3))));
        }
        owner.set("colorTransform", color);
        List<FlaRecord> fa = records(record(a.record, "filters"), "entries"), fb = records(record(b.record, "filters"), "entries");
        if (!fa.isEmpty() || !fb.isEmpty()) {
            if (fa.size() != fb.size()) throw unsupported(frame, "Tween changes filter count");
            FlaRecord filters = FlaRecord.create("filters"); List<FlaRecord> entries = new ArrayList<>();
            for (int i = 0; i < fa.size(); i++) {
                if (n(fa.get(i), "type", -1) != n(fb.get(i), "type", -1)) throw unsupported(frame, "Tween changes filter type");
                entries.add(interpolateFilter(fa.get(i), fb.get(i), ease(frame, fraction, 4)));
            }
            filters.set("entries", entries); owner.set("filters", filters);
        }
        a.record = owner;
    }
    static final class PlacementPair {
        final FlaCompiler.Placement a, b;
        PlacementPair(FlaCompiler.Placement a, FlaCompiler.Placement b) { this.a = a; this.b = b; }
    }
    static FlaRecord copy(FlaRecord r) { FlaRecord copy = FlaRecord.create(r.getClassName()); for (Map.Entry<String, Object> e : r.getFields().entrySet()) copy.set(e.getKey(), e.getValue()); return copy; }
    private static FlaRecord interpolateFilter(FlaRecord a, FlaRecord b, double t) throws IOException {
        FlaRecord out = copy(a);
        for (String key : a.getFields().keySet()) {
            Object av = a.get(key), bv = b.get(key);
            if (av instanceof FlaRecord && bv instanceof FlaRecord) out.set(key, interpolateFilter((FlaRecord) av, (FlaRecord) bv, t));
            else if (av instanceof Number && bv instanceof Number && !Arrays.asList("revision", "type", "quality", "inner", "knockout", "enabled", "full", "hideObject").contains(key))
                out.set(key, key.endsWith("Rgba") ? colorLerp(((Number) av).longValue(), ((Number) bv).longValue(), t)
                        : lerp(((Number) av).doubleValue(), ((Number) bv).doubleValue(), t));
            else if (av instanceof List && !Objects.equals(av, bv)) throw unsupported(a, "Gradient filter tween requires stop correspondence");
        }
        return out;
    }
    private static int[] colorValues(FlaRecord r) throws IOException {
        SwfWriter encoded = EffectsCompiler.color(r);
        // Normalize the archived mode into multipliers/offsets without parsing SWF.
        int[] v = {256, 256, 256, 256, 0, 0, 0, 0}; if (encoded == null) return v;
        FlaRecord c = record(r, "colorTransform"); int type = (int) n(c, "effectType", n(r, "colorEffectType", 3));
        String[] names = {"red", "green", "blue", "alpha"};
        for (int i = 0; i < 4; i++) { v[i] = (int) n(c, names[i] + "Multiplier", 256); v[i + 4] = (int) n(c, names[i] + "Offset", 0); }
        if (type == 1 || type == 2) {
            double amount = n(c, "effectPercent", n(r, "colorEffectPercent", 0)) / 100.0; long tint = n(c, "effectColorRgba", n(r, "colorEffectRgba", 0));
            for (int i = 0; i < 3; i++) { v[i] = (int) Math.round((1 - Math.abs(amount)) * 256); v[i + 4] = (int) Math.round(type == 1 ? Math.max(0, amount) * 255 : (tint >>> (8 * i) & 255) * amount); }
        } else if (type == 4) for (int i = 0; i < 3; i++) { v[i] = 256; v[i + 4] = 0; }
        return v;
    }
    static long colorLerp(long a, long b, double t) {
        long out = 0; for (int i = 0; i < 4; i++) out |= Math.max(0, Math.min(255, Math.round(lerp(a >>> (i * 8) & 255, b >>> (i * 8) & 255, t)))) << (i * 8); return out;
    }
    private static double[] decompose(double[] m) throws IOException {
        double sx = Math.hypot(m[0], m[1]); if (sx < 1e-9) throw new IOException("Cannot interpolate singular tween matrix");
        return new double[]{Math.atan2(m[1], m[0]), sx, (m[0] * m[3] - m[1] * m[2]) / sx, (m[0] * m[2] + m[1] * m[3]) / sx};
    }
    private static double lerp(double a, double b, double t) { return a + (b - a) * t; }
    @SuppressWarnings("unchecked") static double ease(FlaRecord frame, double t, int target) throws IOException {
        if (n(frame, "customEasePresent", 0) == 0) {
            double amount = (short) n(frame, "acceleration", 0) / 100.0;
            return t + Math.abs(amount) * ((amount >= 0 ? t * (2 - t) : t * t) - t);
        }
        Object value = frame.get("customEaseCurves");
        if (!(value instanceof List)) throw unsupported(frame, "Missing custom easing curve");
        List<List<List<Double>>> curves = (List<List<List<Double>>>) value;
        int index = n(frame, "useSingleEaseCurve", 1) != 0 ? 5 : target;
        if (index >= curves.size() || curves.get(index).isEmpty()) return t;
        List<List<Double>> points = curves.get(index);
        if (points.size() < 2) throw unsupported(frame, "Invalid custom easing curve");
        double x0 = points.get(0).get(2), x1 = points.get(points.size() - 1).get(2);
        double y0 = points.get(0).get(3), y1 = points.get(points.size() - 1).get(3), x = x0 + t * (x1 - x0);
        if (x1 <= x0 || y1 == y0) throw unsupported(frame, "Degenerate custom easing curve");
        for (int i = 0; i + 1 < points.size(); i++) {
            List<Double> a = points.get(i), b = points.get(i + 1);
            if (x > b.get(2)) continue;
            double low = 0, high = 1;
            if (!monotone(a.get(2), a.get(4), b.get(0), b.get(2)))
                throw unsupported(frame, "Nonmonotonic custom easing curve");
            for (int j = 0; j < 36; j++) { double mid = (low + high) / 2; if (cubic(a.get(2), a.get(4), b.get(0), b.get(2), mid) < x) low = mid; else high = mid; }
            double result = (cubic(a.get(3), a.get(5), b.get(1), b.get(3), (low + high) / 2) - y0) / (y1 - y0);
            if (!Double.isFinite(result)) throw unsupported(frame, "Nonfinite custom easing value");
            return result;
        }
        return 1;
    }
    private static double cubic(double a, double b, double c, double d, double t) { double u = 1 - t; return u * u * u * a + 3 * u * u * t * b + 3 * u * t * t * c + t * t * t * d; }
    private static boolean monotone(double a, double b, double c, double d) {
        double qa = -a + 3 * b - 3 * c + d, qb = 2 * (a - 2 * b + c), qc = b - a;
        double min = Math.min(qc, qa + qb + qc), vertex = qa == 0 ? -1 : -qb / (2 * qa);
        if (qa > 0 && vertex > 0 && vertex < 1) min = Math.min(min, qa * vertex * vertex + qb * vertex + qc);
        return min >= -1e-12;
    }
}
