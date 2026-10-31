package newsdk.fla;

import java.io.IOException;
import java.util.*;
import com.jpexs.flash.fla.reader.FlaRecord;
import com.jpexs.decompiler.flash.types.filters.ColorMatrixConvertor;
import static newsdk.fla.FlaCompiler.*;

final class EffectsCompiler {
    static SwfWriter color(FlaRecord owner) throws IOException {
        FlaRecord r = record(owner, "colorTransform"); if (r == null) return null;
        int type = (int) n(r, "effectType", n(owner, "colorEffectType", 3)); if (type == 0) return null;
        int[] mult = new int[]{256, 256, 256, 256}, add = new int[4];
        String[] channels = {"red", "green", "blue", "alpha"};
        for (int i = 0; i < 4; i++) { mult[i] = (int) n(r, channels[i] + "Multiplier", 256); add[i] = (int) n(r, channels[i] + "Offset", 0); }
        if (type == 1 || type == 2) {
            double amount = n(r, "effectPercent", n(owner, "colorEffectPercent", 0)) / 100.0;
            if (Math.abs(amount) > 1) throw unsupported(r, "Color-effect percentage out of range");
            long tint = n(r, "effectColorRgba", n(owner, "colorEffectRgba", 0));
            for (int i = 0; i < 3; i++) {
                mult[i] = (int) Math.round((1 - Math.abs(amount)) * 256);
                add[i] = (int) Math.round(type == 1 ? Math.max(0, amount) * 255 : (tint >>> (i * 8) & 255) * amount);
            }
        } else if (type == 4) {
            for (int i = 0; i < 3; i++) { mult[i] = 256; add[i] = 0; }
            add[3] = 0;
        }
        boolean hasMult = false, hasAdd = false;
        for (int i = 0; i < 4; i++) { hasMult |= mult[i] != 256; hasAdd |= add[i] != 0; }
        if (!hasMult && !hasAdd) return null;
        int width = Math.max(hasMult ? SwfWriter.signedBits(mult) : 0, hasAdd ? SwfWriter.signedBits(add) : 0);
        if (width > 15) throw unsupported(r, "Color transform exceeds SWF bit range");
        SwfWriter out = new SwfWriter(); out.bits(hasAdd ? 1 : 0, 1); out.bits(hasMult ? 1 : 0, 1); out.bits(width, 4);
        if (hasMult) for (int v : mult) out.bits(v, width);
        if (hasAdd) for (int v : add) out.bits(v, width);
        out.align(); return out;
    }
    static byte[] filters(FlaRecord owner) throws IOException {
        List<FlaRecord> enabled = new ArrayList<>();
        for (FlaRecord f : records(record(owner, "filters"), "entries")) {
            FlaRecord g = record(f, "gradient"); if (g == null) g = f;
            FlaRecord settings = record(g, "settings"); if (settings == null) settings = g;
            FlaRecord state = record(settings, "state"); if (state == null) state = record(f, "state");
            if (n(state, "enabled", 1) != 0) enabled.add(f);
        }
        if (enabled.isEmpty()) return new byte[0];
        if (enabled.size() > 255) throw unsupported(owner, "Too many filters");
        SwfWriter out = new SwfWriter(); out.u8(enabled.size());
        for (FlaRecord f : enabled) {
            int type = (int) n(f, "type", -1);
            FlaRecord g = type == 7 ? record(f, "gradient") : f;
            FlaRecord settings = record(g, "settings"); if (settings == null) settings = g;
            if (settings == null || type < 0 || type > 7 || type == 5)
                throw unsupported(f, "Unsupported filter type " + type);
            out.u8(type);
            if (type == 6) {
                ColorMatrixConvertor c = new ColorMatrixConvertor();
                c.setBrightness((int) Math.round(number(f, "brightness", 0)));
                c.setContrast((int) Math.round(number(f, "contrast", 0)));
                c.setSaturation((int) Math.round(number(f, "saturation", 0)));
                c.setHue((int) Math.round(number(f, "hue", 0)));
                for (float value : c.getMatrix()) out.u32(Float.floatToIntBits(value));
                continue;
            }
            if (type == 4 || type == 7) {
                List<FlaRecord> stops = records(g, "stops");
                if (stops.isEmpty() || stops.size() > 255) throw unsupported(f, "Invalid gradient filter stops");
                out.u8(stops.size()); for (FlaRecord stop : stops) out.rgba(n(stop, "colorRgba", 0));
                for (FlaRecord stop : stops) out.u8((int) n(stop, "ratio", 0));
            } else if (type == 3) {
                out.rgba(n(settings, "colorRgba", 0xff000000L)); out.rgba(n(g, "highlightColorRgba", 0xffffffffL));
            } else if (type != 1) out.rgba(n(settings, "colorRgba", 0xff000000L));
            fixed(out, number(settings, "blurX", 5)); fixed(out, number(settings, "blurY", number(settings, "blurX", 5)));
            if (type != 1 && type != 2) { fixed(out, number(settings, "angleRadians", Math.PI / 4)); fixed(out, number(settings, "distance", 5)); }
            int quality = (int) n(settings, "quality", 1);
            if (quality < 0 || quality > (type == 1 ? 31 : type == 0 || type == 2 ? 31 : 15))
                throw unsupported(f, "Filter quality out of range");
            if (type == 1) out.u8(quality << 3);
            else {
                double strength = number(settings, "strengthPercent", 100) / 100;
                if (strength < 0 || strength >= 256) throw unsupported(f, "Filter strength out of range");
                out.u16((int) Math.round(strength * 256));
                int flags = (n(settings, "inner", 0) != 0 ? 128 : 0) | (n(settings, "knockout", 0) != 0 ? 64 : 0)
                        | (n(settings, "hideObject", 0) == 0 ? 32 : 0);
                if (type == 3 || type == 4 || type == 7) flags |= n(g, "full", 0) != 0 ? 16 : 0;
                out.u8(flags | quality);
            }
        }
        return out.data();
    }
    private static void fixed(SwfWriter out, double v) throws IOException {
        if (!Double.isFinite(v) || v * 65536 < Integer.MIN_VALUE || v * 65536 > Integer.MAX_VALUE)
            throw new IOException("Filter value out of range");
        out.u32(Math.round(v * 65536));
    }
}
