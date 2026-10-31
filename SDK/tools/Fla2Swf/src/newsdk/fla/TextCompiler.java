package newsdk.fla;

import com.jpexs.flash.fla.reader.*;
import com.jpexs.decompiler.flash.tags.*;
import com.jpexs.decompiler.flash.types.*;
import java.awt.Font;
import java.awt.GraphicsEnvironment;
import java.awt.font.FontRenderContext;
import java.awt.font.GlyphVector;
import java.awt.font.LineMetrics;
import java.io.IOException;
import java.util.*;
import static newsdk.fla.FlaCompiler.*;

final class TextCompiler {
    private final FlaCompiler compiler;
    private final Map<FlaRecord, Integer> cache = new IdentityHashMap<>();
    private final FontRenderContext context = new FontRenderContext(null, true, true);
    TextCompiler(FlaCompiler compiler) { this.compiler = compiler; }

    int text(FlaRecord r) throws IOException {
        Integer previous = cache.get(r); if (previous != null) return previous;
        FlaBounds bounds = r.getBounds();
        if (bounds == null || !bounds.isDefined()) throw unsupported(r, "Missing text bounds");
        List<FlaRecord> runs = records(r, "runs");
        if (runs.isEmpty()) throw unsupported(r, "Text without a complete style run");
        int flags = (int) n(r, "textFlags", 0), id = compiler.nextId();
        RECT rect = new RECT(bounds.getXMinTwips(), bounds.getXMaxTwips(), bounds.getYMinTwips(), bounds.getYMaxTwips());
        if ((flags & 1) != 0) {
            FlaRecord style = record(runs.get(0), "style");
            if (runs.size() > 1) throw unsupported(r, "Rich dynamic/input text requires HTML layout");
            DefineFont2Tag font = font(style, s(runs.get(0), "text", ""), false);
            DefineEditTextTag tag = new DefineEditTextTag(compiler.scriptContext);
            tag.characterID = id; tag.bounds = rect; tag.hasText = true; tag.initialText = s(runs.get(0), "text", "");
            tag.hasFont = true; tag.fontId = font.fontID; tag.fontHeight = (int) n(style, "fontSize", 240);
            tag.hasTextColor = true; tag.textColor = rgba(n(style, "colorRgba", 0xff000000L));
            tag.readOnly = (flags & 2) != 0; tag.password = (flags & 4) != 0;
            tag.multiline = (flags & 16) != 0; tag.wordWrap = tag.multiline && (flags & 8) == 0;
            tag.noSelect = (flags & 128) != 0; tag.border = (flags & 64) != 0;
            tag.html = (flags & 0x8000) != 0; tag.useOutlines = false; tag.autoSize = n(r, "autoExpand", 0) != 0;
            tag.maxLength = (int) n(r, "maxCharacters", 0); tag.hasMaxLength = tag.maxLength != 0;
            tag.variableName = s(r, "variableName", ""); tag.hasLayout = true; tag.align = (int) n(style, "alignment", 0);
            tag.leftMargin = (int) n(style, "leftMargin", 0); tag.rightMargin = (int) n(style, "rightMargin", 0);
            tag.indent = (int) n(style, "indent", 0); tag.leading = (int) n(style, "lineSpacing", 0);
            compiler.define(tag);
        } else {
            List<TEXTRECORD> records = new ArrayList<>();
            double x = bounds.getXMinTwips(), y = bounds.getYMinTwips();
            int alignment = (int) n(record(runs.get(0), "style"), "alignment", 0);
            if (alignment < 0 || alignment > 2) throw unsupported(r, "Justified text layout is not supported");
            if (runs.size() > 1 && alignment != 0) throw unsupported(r, "Aligned mixed-style text requires paragraph layout");
            for (FlaRecord run : runs) {
                FlaRecord style = record(run, "style"); String value = s(run, "text", "");
                if (n(style, "vertical", 0) != 0 || n(style, "rotation", 0) != 0 || n(style, "characterPosition", 0) != 0
                        || !s(style, "url", "").isEmpty() || !s(style, "urlOverride", "").isEmpty())
                    throw unsupported(r, "Vertical, superscript or linked static text is not supported");
                DefineFont2Tag embedded = font(style, value, true);
                float size = n(style, "fontSize", 240) / 20f; Font awt = systemFont(style).deriveFont(size);
                LineMetrics metrics = awt.getLineMetrics("Mg", context);
                String[] lines = value.replace("\r\n", "\n").replace('\r', '\n').split("\n", -1);
                for (int line = 0; line < lines.length; line++) {
                    String str = lines[line]; GlyphVector glyphs = awt.createGlyphVector(context, str);
                    int spacing = (int) n(style, "letterSpacing", 0);
                    double width = glyphs.getGlyphPosition(glyphs.getNumGlyphs()).getX() * 20 + spacing * Math.max(0, str.length() - 1);
                    double start = line == 0 ? x : bounds.getXMinTwips();
                    if (alignment == 1) start = bounds.getXMaxTwips() - width;
                    if (alignment == 2) start = bounds.getXMinTwips() + (bounds.getWidthTwips() - width) / 2;
                    for (int pos = 0; pos < str.length(); pos += 255) {
                        TEXTRECORD t = new TEXTRECORD(); t.styleFlagsHasFont = t.styleFlagsHasColor = true;
                        t.styleFlagsHasXOffset = t.styleFlagsHasYOffset = true; t.fontId = embedded.fontID;
                        t.textHeight = (int) n(style, "fontSize", 240); t.textColorA = rgba(n(style, "colorRgba", 0xff000000L));
                        t.xOffset = (int) Math.round(start + glyphs.getGlyphPosition(pos).getX() * 20 + spacing * pos);
                        t.yOffset = (int) Math.round(y + metrics.getAscent() * 20); t.glyphEntries = new ArrayList<>();
                        for (int i = pos; i < Math.min(str.length(), pos + 255); i++) {
                            GLYPHENTRY entry = new GLYPHENTRY(); entry.glyphIndex = embedded.charToGlyph(str.charAt(i));
                            entry.glyphAdvance = (int) Math.round((glyphs.getGlyphPosition(i + 1).getX() - glyphs.getGlyphPosition(i).getX()) * 20) + spacing;
                            t.glyphEntries.add(entry);
                        }
                        records.add(t);
                    }
                    x = start + width;
                    if (line + 1 < lines.length) y += metrics.getHeight() * 20 + n(style, "lineSpacing", 0);
                }
            }
            compiler.define(new DefineText2Tag(compiler.scriptContext, id, rect, new MATRIX(), records));
        }
        cache.put(r, id); return id;
    }
    private DefineFont2Tag font(FlaRecord style, String text, boolean embed) throws IOException {
        DefineFont2Tag tag = new DefineFont2Tag(compiler.scriptContext); tag.fontID = compiler.nextId();
        tag.fontName = s(style, "fontFamily", "_serif"); tag.fontFlagsBold = n(style, "bold", 0) != 0;
        tag.fontFlagsItalic = n(style, "italic", 0) != 0; tag.fontFlagsWideCodes = true;
        if (embed) {
            Font font = systemFont(style);
            Set<Character> chars = new TreeSet<>();
            for (char c : text.toCharArray()) if (c != '\r' && c != '\n') {
                if (Character.isSurrogate(c) || !font.canDisplay(c)) throw unsupported(style, "Font lacks character U+" + Integer.toHexString(c));
                chars.add(c);
            }
            for (char c : chars) if (!tag.addCharacter(c, font)) throw unsupported(style, "Failed to embed font glyph");
        }
        compiler.define(tag); return tag;
    }
    private Font systemFont(FlaRecord style) throws IOException {
        String face = s(style, "fontFace", ""), family = s(style, "fontFamily", "_serif");
        int weight = (n(style, "bold", 0) != 0 ? Font.BOLD : 0) | (n(style, "italic", 0) != 0 ? Font.ITALIC : 0);
        if (family.equals("_serif") || family.equals("_sans") || family.equals("_typewriter"))
            return new Font(family.equals("_sans") ? Font.SANS_SERIF : family.equals("_typewriter") ? Font.MONOSPACED : Font.SERIF, weight, 1024);
        for (Font f : GraphicsEnvironment.getLocalGraphicsEnvironment().getAllFonts()) {
            if (!face.isEmpty() && (f.getPSName().equalsIgnoreCase(face) || f.getFontName().equalsIgnoreCase(face))) return f.deriveFont(weight, 1024f);
            if (f.getFamily(Locale.ENGLISH).equalsIgnoreCase(family)) return f.deriveFont(weight, 1024f);
        }
        throw unsupported(style, "Missing font " + family + "; supply the original TTF/OTF with --font");
    }
    private static RGBA rgba(long c) { return new RGBA((int) c & 255, (int) (c >>> 8) & 255, (int) (c >>> 16) & 255, (int) (c >>> 24) & 255); }
}
