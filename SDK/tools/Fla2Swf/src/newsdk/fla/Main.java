package newsdk.fla;

import com.jpexs.flash.fla.reader.*;
import com.jpexs.decompiler.flash.SWF;
import com.jpexs.decompiler.flash.tags.*;
import java.awt.Font;
import java.awt.GraphicsEnvironment;
import java.io.*;
import java.nio.file.*;
import java.util.*;

/** Command-line entry point. No Flash installation, JSFL, COM or desktop session. */
public final class Main {
    private Main() { }
    public static void main(String[] args) {
        try { run(args); System.exit(0); }
        catch (Exception ex) { System.err.println("Fla2Swf: " + ex.getMessage()); System.exit(1); }
    }
    static void run(String[] args) throws Exception {
        Path input = null, output = null; boolean overwrite = false, compressed = true;
        List<Path> fonts = new ArrayList<>();
        for (int i = 0; i < args.length; i++) {
            String arg = args[i];
            if (arg.equals("--help") || arg.equals("-h")) { usage(); return; }
            if (arg.equals("--version")) { System.out.println("NewSdk Fla2Swf 1.0 (Flash 8 only)"); return; }
            if (arg.equals("--overwrite")) overwrite = true;
            else if (arg.equals("--no-compress")) compressed = false;
            else if (arg.equals("-o") || arg.equals("--output") || arg.equals("--font")) {
                if (++i == args.length) throw new IOException("Missing value for " + arg);
                if (arg.equals("--font")) fonts.add(Paths.get(args[i])); else output = Paths.get(args[i]);
            } else if (arg.startsWith("-")) throw new IOException("Unknown option " + arg);
            else if (input == null) input = Paths.get(arg);
            else throw new IOException("Expected one input FLA; unexpected argument " + arg);
        }
        if (input == null) { usage(); throw new IOException("An input FLA is required"); }
        input = input.toAbsolutePath().normalize();
        if (!input.getFileName().toString().toLowerCase(Locale.ROOT).endsWith(".fla"))
            throw new IOException("Input must be a binary Flash 8 .fla file");
        if (!Files.isRegularFile(input)) throw new IOException("Input file does not exist: " + input);
        if (output == null) {
            String filename = input.getFileName().toString();
            output = input.resolveSibling(filename.substring(0, filename.length() - 4) + ".swf");
        }
        output = output.toAbsolutePath().normalize();
        if (!output.getFileName().toString().toLowerCase(Locale.ROOT).endsWith(".swf")) throw new IOException("Output must have the .swf extension");
        if (input.equals(output) || Files.exists(output) && Files.isSameFile(input, output)) throw new IOException("Output cannot overwrite the input FLA");
        if (Files.exists(output) && !overwrite) throw new IOException("Output already exists; use --overwrite: " + output);
        try (InputStream in = Files.newInputStream(input)) {
            byte[] magic = new byte[8];
            if (in.read(magic) != 8 || !Arrays.equals(magic, new byte[]{(byte) 0xd0, (byte) 0xcf, 0x11, (byte) 0xe0, (byte) 0xa1, (byte) 0xb1, 0x1a, (byte) 0xe1}))
                throw new IOException("Expected OLE/CFB Flash 8 FLA; ZIP/XFL and renamed SWF files are not supported");
        }
        for (Path font : fonts) {
            try (InputStream in = Files.newInputStream(font)) {
                GraphicsEnvironment.getLocalGraphicsEnvironment().registerFont(Font.createFont(Font.TRUETYPE_FONT, in));
            }
        }
        BinaryFlaDocument document = new BinaryFlaReader().read(input.toFile());
        FlaCompiler compiler = new FlaCompiler(document); byte[] bytes = compiler.compile(compressed);
        validate(bytes, compiler.frameCount);
        Files.createDirectories(output.getParent());
        Path temporary = Files.createTempFile(output.getParent(), ".fla2swf-", ".tmp");
        try {
            Files.write(temporary, bytes);
            if (overwrite) {
                try { Files.move(temporary, output, StandardCopyOption.ATOMIC_MOVE, StandardCopyOption.REPLACE_EXISTING); }
                catch (AtomicMoveNotSupportedException ex) { Files.move(temporary, output, StandardCopyOption.REPLACE_EXISTING); }
            } else Files.move(temporary, output);
        } finally { Files.deleteIfExists(temporary); }
        System.out.println("SWF 8: " + output + " (" + compiler.frameCount + " frames, " + bytes.length + " bytes)");
    }
    static SWF validate(byte[] data, int expectedFrames) throws Exception {
        if (data.length < 8 || data[3] != 8) throw new IOException("Generated output is not SWF 8");
        SWF swf = new SWF(new ByteArrayInputStream(data), false);
        if (swf.version != 8 || swf.isAS3() || swf.frameCount != expectedFrames)
            throw new IOException("Generated SWF failed version or frame-count validation");
        validateTags(swf.getTags()); return swf;
    }
    private static void validateTags(Iterable<Tag> tags) throws IOException {
        for (Tag t : tags) {
            if (t.getVersion() > 8 || t.getId() == 72 || t.getId() == 82 || t.getId() > 83)
                throw new IOException("Generated tag exceeds Flash 8: " + t.getTagName());
            if (t.remainingData != null && t.remainingData.getLength() != 0)
                throw new IOException("Generated tag has unparsed data: " + t.getTagName());
            if (t instanceof DefineSpriteTag) validateTags(((DefineSpriteTag) t).getTags());
        }
    }
    private static void usage() {
        System.out.println("Usage: Fla2Swf input.fla [-o output.swf] [--overwrite] [--no-compress] [--font file.ttf]\n"
                + "Standalone binary Flash 8 FLA to SWF 8. Requires Java 8+, no Flash installation.\n"
                + "Unsupported features fail with a record offset; no partial SWF is published.");
    }
}
