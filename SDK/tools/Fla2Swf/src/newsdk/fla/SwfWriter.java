package newsdk.fla;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.zip.DeflaterOutputStream;
import com.jpexs.flash.fla.reader.FlaMatrix;

/** Small SWF 8 encoder. FLA coordinates are converted before reaching this class. */
final class SwfWriter {
    private final ByteArrayOutputStream out = new ByteArrayOutputStream();
    private int pending, count;

    void bits(long value, int width) throws IOException {
        if (width < 0 || width > 32) throw new IOException("Invalid SWF bit width: " + width);
        for (int i = width - 1; i >= 0; i--) {
            pending = (pending << 1) | (int) ((value >>> i) & 1);
            if (++count == 8) { out.write(pending); count = pending = 0; }
        }
    }
    void align() { if (count != 0) { out.write(pending << (8 - count)); count = pending = 0; } }
    void u8(int v) { align(); out.write(v); }
    void u16(int v) { u8(v); u8(v >>> 8); }
    void u32(long v) { u16((int) v); u16((int) (v >>> 16)); }
    void bytes(byte[] b) { align(); out.write(b, 0, b.length); }
    void string(String s) throws IOException {
        if (s.indexOf('\0') >= 0) throw new IOException("SWF strings cannot contain NUL");
        bytes(s.getBytes(StandardCharsets.UTF_8)); u8(0);
    }
    byte[] data() { align(); return out.toByteArray(); }
    void tag(int id, byte[] payload) {
        if (payload.length < 63) u16(id << 6 | payload.length);
        else { u16(id << 6 | 63); u32(payload.length); }
        bytes(payload);
    }
    void tag(int id, SwfWriter payload) { tag(id, payload.data()); }
    void tag(int id) { tag(id, new byte[0]); }

    static int signedBits(int... values) {
        int n = 1;
        for (int v : values) n = Math.max(n, 33 - Integer.numberOfLeadingZeros(v < 0 ? ~v : v));
        return n;
    }
    static int indexBits(int count) { return count == 0 ? 0 : 32 - Integer.numberOfLeadingZeros(count); }
    void rect(int x0, int x1, int y0, int y1) throws IOException {
        int n = signedBits(x0, x1, y0, y1);
        if (n > 31) throw new IOException("SWF bounds exceed signed 31 bits");
        bits(n, 5); bits(x0, n); bits(x1, n); bits(y0, n); bits(y1, n); align();
    }
    void matrix(FlaMatrix m) throws IOException {
        matrix(m == null ? new double[]{1, 0, 0, 1, 0, 0}
                : new double[]{m.getA(), m.getB(), m.getC(), m.getD(), m.getTxTwips(), m.getTyTwips()});
    }
    void matrix(double[] m) throws IOException {
        int a = fixed(m[0]), b = fixed(m[1]), c = fixed(m[2]), d = fixed(m[3]);
        boolean scale = a != 65536 || d != 65536, skew = b != 0 || c != 0;
        bits(scale ? 1 : 0, 1);
        if (scale) { int n = signedBits(a, d); checkMatrixBits(n); bits(n, 5); bits(a, n); bits(d, n); }
        bits(skew ? 1 : 0, 1);
        if (skew) { int n = signedBits(b, c); checkMatrixBits(n); bits(n, 5); bits(b, n); bits(c, n); }
        int tx = (int) Math.round(m[4]), ty = (int) Math.round(m[5]), n = signedBits(tx, ty);
        checkMatrixBits(n); bits(n, 5); bits(tx, n); bits(ty, n); align();
    }
    private static void checkMatrixBits(int n) throws IOException {
        if (n > 31) throw new IOException("SWF matrix exceeds signed 31 bits");
    }
    private static int fixed(double n) throws IOException {
        double value = Math.rint(n * 65536);
        if (!Double.isFinite(value) || value < Integer.MIN_VALUE || value > Integer.MAX_VALUE)
            throw new IOException("SWF matrix coefficient out of range");
        return (int) value;
    }
    void rgba(long v) { u32(v); }
    static byte[] compress(byte[] bytes) throws IOException {
        ByteArrayOutputStream result = new ByteArrayOutputStream();
        try (DeflaterOutputStream z = new DeflaterOutputStream(result)) { z.write(bytes); }
        return result.toByteArray();
    }
    static byte[] movie(byte[] body, boolean compressed) throws IOException {
        SwfWriter result = new SwfWriter();
        result.bytes((compressed ? "CWS" : "FWS").getBytes(StandardCharsets.US_ASCII));
        result.u8(8); result.u32(body.length + 8L);
        result.bytes(compressed ? compress(body) : body);
        return result.data();
    }
}
