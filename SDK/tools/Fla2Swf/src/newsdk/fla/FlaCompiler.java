package newsdk.fla;

import com.jpexs.flash.fla.reader.*;
import com.jpexs.flash.fla.exporter.*;
import com.jpexs.decompiler.flash.SWF;
import com.jpexs.decompiler.flash.SWFOutputStream;
import com.jpexs.decompiler.flash.SWFInputStream;
import com.jpexs.decompiler.flash.types.sound.SoundFormat;
import com.jpexs.decompiler.flash.action.parser.script.ActionScript2Parser;
import com.jpexs.decompiler.flash.tags.*;
import com.jpexs.decompiler.flash.types.*;
import java.awt.image.BufferedImage;
import java.io.*;
import java.util.*;
import javax.imageio.ImageIO;

/** Binary Flash 8 model to SWF 8. Unsupported runtime features fail before output. */
final class FlaCompiler {
    final BinaryFlaDocument document;
    final SWF scriptContext = new SWF();
    private final SwfWriter definitions = new SwfWriter();
    private final ShapeCompiler shapes = new ShapeCompiler(this);
    private final TextCompiler texts = new TextCompiler(this);
    private final MorphCompiler morphs = new MorphCompiler(this);
    private final Map<Long, BinaryFlaDocument.Page> library = new LinkedHashMap<>();
    private final Map<Long, BinaryFlaDocument.Media> media = new LinkedHashMap<>();
    private final Map<Long, Bitmap> bitmaps = new HashMap<>();
    private final Map<Long, Integer> sounds = new HashMap<>();
    private final Map<BinaryFlaDocument.Page, Map<Integer, Integer>> symbols = new IdentityHashMap<>();
    private final Set<BinaryFlaDocument.Page> compiling = Collections.newSetFromMap(new IdentityHashMap<BinaryFlaDocument.Page, Boolean>());
    private final Map<FlaRecord, Integer> shapeIds = new IdentityHashMap<>();
    private final Map<String, byte[]> actionCache = new HashMap<>();
    private final Map<FlaRecord, byte[]> clipCache = new IdentityHashMap<>();
    private final Map<String, Integer> graphics = new HashMap<>();
    private XflExportResult recovered;
    private int lastId;
    int frameCount;

    FlaCompiler(BinaryFlaDocument document) throws IOException {
        this.document = document; scriptContext.version = 8;
        if (document.getContentsRevision() != 0x3f) throw new IOException("Only binary Flash 8 FLA (Contents revision 0x3F) is supported");
        if (!document.getIssues().isEmpty()) throw new IOException("Incomplete FLA parse: " + document.getIssues().get(0));
        for (BinaryFlaDocument.Page p : document.getPages()) if (p.getRole() == BinaryFlaDocument.Page.Role.LIBRARY) {
            if (p.getLibraryId() == null || library.put(p.getLibraryId(), p) != null)
                throw new IOException("Missing or duplicate library ID");
        }
        for (BinaryFlaDocument.Media m : document.getMedia()) {
            long id = n(m.getDescriptor(), "mediaId", -1);
            if (id < 0 || media.put(id, m) != null) throw new IOException("Missing or duplicate media ID");
        }
    }
    int nextId() throws IOException {
        if (lastId == 65535) throw new IOException("SWF character ID limit exceeded");
        return ++lastId;
    }
    void define(int tag, SwfWriter data) { definitions.tag(tag, data); }
    void define(Tag tag) throws IOException {
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        SWFOutputStream stream = new SWFOutputStream(out, 8, "UTF-8");
        tag.getData(stream); stream.flush(); definitions.tag(tag.getId(), out.toByteArray());
        scriptContext.addTag(tag);
    }

    byte[] compile(boolean compressed) throws IOException {
        if (document.getWidthTwips() == null || document.getHeightTwips() == null || document.getFrameRate() == null)
            throw new IOException("Missing stage dimensions or frame rate");
        SwfWriter timeline = new SwfWriter();
        List<Placement> previous = Collections.emptyList();
        for (BinaryFlaDocument.Page p : document.getPages()) if (p.getRole() == BinaryFlaDocument.Page.Role.SCENE) {
            Timeline t = timeline(p);
            for (int f = 0; f < t.length; f++) {
                List<Placement> current = t.frame(f);
                display(timeline, previous, current); t.events(timeline, f);
                timeline.tag(1); previous = current; frameCount++;
                if (frameCount > 65535) throw new IOException("SWF exceeds 65535 frames");
            }
        }
        if (frameCount == 0) throw new IOException("FLA contains no scene frames");
        SwfWriter exports = new SwfWriter(); List<String> names = new ArrayList<>(); List<Integer> ids = new ArrayList<>();
        for (BinaryFlaDocument.Page p : library.values()) {
            FlaLinkage linkage = linkage(p.getDescriptor());
            if (linkage != null && linkage.getMode() != FlaLinkage.Mode.NONE) {
                checkLinkage(p.getDescriptor(), linkage);
                if (linkage.getMode() == FlaLinkage.Mode.EXPORT) {
                    names.add(linkage.getIdentifier()); ids.add(symbol(p));
                }
            }
        }
        for (BinaryFlaDocument.Media m : media.values()) {
            FlaLinkage linkage = linkage(m.getDescriptor());
            if (linkage != null && linkage.getMode() != FlaLinkage.Mode.NONE) {
                checkLinkage(m.getDescriptor(), linkage);
                if (linkage.getMode() == FlaLinkage.Mode.EXPORT) {
                    names.add(linkage.getIdentifier());
                    long id = n(m.getDescriptor(), "mediaId", -1);
                    ids.add(m.getDescriptor().getClassName().equals("CMediaBits") ? bitmap((int) id).id : sound(id));
                }
            }
        }
        if (!ids.isEmpty()) {
            Set<String> unique = new HashSet<>(); exports.u16(ids.size());
            for (int i = 0; i < ids.size(); i++) {
                if (names.get(i) == null || names.get(i).isEmpty() || !unique.add(names.get(i)))
                    throw new IOException("Missing or duplicate ActionScript linkage identifier");
                exports.u16(ids.get(i)); exports.string(names.get(i));
            }
        }
        SwfWriter body = new SwfWriter();
        long width = document.getWidthTwips(), height = document.getHeightTwips();
        if (width <= 0 || height <= 0 || width > Integer.MAX_VALUE || height > Integer.MAX_VALUE)
            throw new IOException("Invalid stage dimensions");
        body.rect(0, (int) width, 0, (int) height);
        double rate = document.getFrameRate();
        if (!Double.isFinite(rate) || rate <= 0 || rate >= 256) throw new IOException("Frame rate outside SWF 8 range");
        body.u16((int) Math.round(rate * 256)); body.u16(frameCount);
        SwfWriter attributes = new SwfWriter(); attributes.u32(0); body.tag(69, attributes);
        long color = n(document.getContents(), "backgroundColorRgba", 0xffffffffL);
        SwfWriter background = new SwfWriter(); background.u8((int) color); background.u8((int) (color >>> 8)); background.u8((int) (color >>> 16));
        body.tag(9, background); body.bytes(definitions.data());
        if (!ids.isEmpty()) body.tag(56, exports);
        body.bytes(timeline.data()); body.tag(0);
        return SwfWriter.movie(body.data(), compressed);
    }

    static FlaLinkage linkage(FlaRecord r) {
        FlaRecord metadata = record(r, "libraryMetadata");
        FlaRecord raw = record(metadata, "linkage");
        if (raw == null) raw = record(r, "linkage");
        return raw == null ? null : raw.getLinkage();
    }
    private void checkLinkage(FlaRecord owner, FlaLinkage link) throws IOException {
        if (link.getMode() == FlaLinkage.Mode.IMPORT || link.getMode() == FlaLinkage.Mode.UNKNOWN)
            throw unsupported(owner, "Runtime shared-library imports are not supported");
        if (link.getClassName() != null && !link.getClassName().isEmpty())
            throw unsupported(owner, "External AS2 class linkage requires a classpath compiler");
    }
    private int shape(FlaRecord r) throws IOException {
        Integer old = shapeIds.get(r); if (old != null) return old;
        int id = shapes.shape(r); shapeIds.put(r, id); return id;
    }
    private int symbol(BinaryFlaDocument.Page page) throws IOException {
        return symbol(page, n(page.getDescriptor(), "symbolType", 2) == 1 ? 1 : 0);
    }
    private int symbol(BinaryFlaDocument.Page page, int kind) throws IOException {
        if (compiling.contains(page)) throw unsupported(page.getDescriptor(), "Recursive symbol graph");
        Map<Integer, Integer> variants = symbols.computeIfAbsent(page, unused -> new HashMap<Integer, Integer>());
        Integer old = variants.get(kind); if (old != null) return old;
        checkLinkageIfPresent(page.getDescriptor());
        if (n(record(page.getDescriptor(), "component"), "componentEnabled", 0) != 0)
            throw unsupported(page.getDescriptor(), "Compiled UI components are not supported");
        int id = nextId(); variants.put(kind, id); compiling.add(page);
        try {
            Timeline t = timeline(page);
            if (kind != 0) button(page, id, t, kind == 2);
            else {
                SwfWriter out = new SwfWriter(); out.u16(id); out.u16(t.length);
                List<Placement> previous = Collections.emptyList();
                for (int f = 0; f < t.length; f++) {
                    List<Placement> current = t.frame(f); display(out, previous, current); t.events(out, f);
                    out.tag(1); previous = current;
                }
                out.tag(0); define(39, out);
            }
            if (n(page.getDescriptor(), "scaleGridEnabled", 0) != 0) {
                FlaBounds b = page.getDescriptor().getBounds("scaleGridBounds");
                if (b == null || !b.isDefined()) throw unsupported(page.getDescriptor(), "Invalid scale-9 grid");
                SwfWriter grid = new SwfWriter(); grid.u16(id);
                grid.rect(b.getXMinTwips(), b.getXMaxTwips(), b.getYMinTwips(), b.getYMaxTwips()); define(78, grid);
            }
            return id;
        } finally { compiling.remove(page); }
    }
    private void checkLinkageIfPresent(FlaRecord r) throws IOException { FlaLinkage l = linkage(r); if (l != null) checkLinkage(r, l); }
    private int graphic(BinaryFlaDocument.Page page, int frame) throws IOException {
        String key = page.getLibraryId() + ":" + frame;
        Integer old = graphics.get(key); if (old != null) return old;
        if (!compiling.add(page)) throw unsupported(page.getDescriptor(), "Recursive graphic symbol");
        try {
            int id = nextId(); graphics.put(key, id);
            SwfWriter out = new SwfWriter(); out.u16(id); out.u16(1);
            Timeline t = timeline(page); display(out, Collections.<Placement>emptyList(), t.frame(frame)); t.events(out, frame);
            out.tag(12, script("stop();", page.getDescriptor())); out.tag(1); out.tag(0); define(39, out); return id;
        } finally { compiling.remove(page); }
    }

    private void objects(List<Placement> out, FlaRecord r, int age) throws IOException {
        switch (r.getClassName()) {
            case "CPicFrame":
            case "CPicShape":
                for (FlaRecord c : r.getChildren()) objects(out, c, age);
                int shapeId = shape(r); if (shapeId != 0) out.add(new Placement(shapeId, r));
                break;
            case "CPicSymbol": case "CPicShapeObj": case "CPicSprite": case "CPicButton":
                long libId = n(r, "libraryId", -1); BinaryFlaDocument.Page page = library.get(libId);
                if (page == null) throw unsupported(r, "Missing library symbol " + libId);
                int symbolId;
                if (r.getClassName().equals("CPicSymbol") || r.getClassName().equals("CPicShapeObj")) {
                    int length = timeline(page).length, first = (int) n(r, "firstFrame", 0), mode = (int) n(r, "loopMode", 0);
                    if (first < 0 || first >= length || mode < 0 || mode > 2) throw unsupported(r, "Invalid graphic playback settings");
                    int f = mode == 2 ? first : mode == 1 ? Math.min(first + age, length - 1) : (first + age) % length;
                    symbolId = graphic(page, f);
                } else symbolId = symbol(page, r.getClassName().equals("CPicButton") ? n(r, "trackAsMenu", 0) != 0 ? 2 : 1 : 0);
                out.add(new Placement(symbolId, r)); break;
            case "CPicBitmap":
                Bitmap bitmap = bitmap((int) n(r, "mediaId", -1));
                if (record(r, "raster") != null) throw unsupported(r, "Inline bitmap payload");
                out.add(new Placement(bitmap.shapeId, r)); break;
            case "CPicText":
                out.add(new Placement(texts.text(r), r)); break;
            default: throw unsupported(r, "Unsupported display object " + r.getClassName());
        }
    }
    private void display(SwfWriter out, List<Placement> before, List<Placement> after) throws IOException {
        Map<Integer, Placement> old = new HashMap<>(); for (Placement p : before) old.put(p.depth, p);
        Set<Integer> remaining = new HashSet<>(); for (Placement p : after) remaining.add(p.depth);
        for (Placement p : before) if (!remaining.contains(p.depth)) { SwfWriter remove = new SwfWriter(); remove.u16(p.depth); out.tag(28, remove); }
        for (Placement p : after) {
            Placement prev = old.get(p.depth); byte[] state = place(p, false, true);
            if (prev == null || !Arrays.equals(state, place(prev, false, true)))
                out.tag(70, place(p, prev != null, prev == null || prev.id != p.id));
        }
    }
    private byte[] place(Placement p, boolean move, boolean character) throws IOException {
        FlaRecord r = p.record;
        byte[] filters = EffectsCompiler.filters(r), clips = r.getClassName().equals("CPicFrame") ? new byte[0] : clipActions(source(record(r, "actions")), r);
        String name = s(r, "instanceName", "");
        SwfWriter color = EffectsCompiler.color(r);
        SwfWriter out = new SwfWriter();
        out.u8((clips.length == 0 ? 0 : 128) | (p.clipDepth == 0 ? 0 : 64) | (name.isEmpty() ? 0 : 32)
                | (color == null ? 0 : 8) | 4 | (character ? 2 : 0) | (move ? 1 : 0));
        int blend = (int) n(r, "blendMode", 1);
        if (blend < 0 || blend > 14) throw unsupported(r, "Unknown blend mode");
        boolean cache = n(r, "cacheAsBitmap", 0) != 0;
        out.u8((cache ? 4 : 0) | (blend > 1 ? 2 : 0) | (filters.length > 0 ? 1 : 0));
        out.u16(p.depth); if (character) out.u16(p.id); out.matrix(p.matrix);
        if (color != null) out.bytes(color.data());
        if (!name.isEmpty()) out.string(name);
        if (p.clipDepth != 0) out.u16(p.clipDepth);
        if (filters.length > 0) out.bytes(filters);
        if (blend > 1) out.u8(blend);
        if (cache) out.u8(1);
        if (clips.length > 0) out.bytes(clips);
        return out.data();
    }
    static final class Placement {
        final int id; FlaRecord record; double[] matrix; int depth, clipDepth;
        Placement(int id, FlaRecord r) {
            this.id = id; this.record = r;
            FlaMatrix m = r.getMatrix(); matrix = m == null ? new double[]{1, 0, 0, 1, 0, 0}
                    : new double[]{m.getA(), m.getB(), m.getC(), m.getD(), m.getTxTwips(), m.getTyTwips()};
        }
    }

    private final Map<BinaryFlaDocument.Page, Timeline> timelines = new IdentityHashMap<>();
    private Timeline timeline(BinaryFlaDocument.Page p) throws IOException {
        Timeline t = timelines.get(p); if (t == null) { t = new Timeline(p); timelines.put(p, t); } return t;
    }
    private final class Timeline {
        final List<FlaRecord> layers; final List<List<FlaRecord>> keys = new ArrayList<>();
        final int length; final int[] bases, capacities;
        Timeline(BinaryFlaDocument.Page p) throws IOException {
            if (p.getRoot() == null) throw unsupported(p.getDescriptor(), "Missing page stream");
            layers = new ArrayList<>(p.getRoot().getChildren()); int max = 1;
            // FLA stores a mask above its children. SWF requires the mask at the
            // lowest depth of the clipped range, while preserving child stacking.
            for (FlaRecord mask : new ArrayList<>(layers)) if (mask.getLayerType() == 4) {
                int first = layers.indexOf(mask);
                for (FlaRecord child : layers) if (child.getLayerParent() == mask) first = Math.min(first, layers.indexOf(child));
                layers.remove(mask); layers.add(first, mask);
            }
            bases = new int[layers.size()]; capacities = new int[layers.size()]; int depth = 1;
            for (int i = 0; i < layers.size(); i++) {
                FlaRecord layer = layers.get(i);
                if (!layer.getClassName().equals("CPicLayer")) throw unsupported(layer, "Unexpected timeline child");
                List<FlaRecord> frames = layer.getChildren(); keys.add(frames); int end = 0, capacity = 1;
                for (FlaRecord f : frames) {
                    int duration = (int) n(f, "duration", 1);
                    if (duration < 1 || end + (long) duration > 65535) throw unsupported(f, "Invalid frame duration");
                    end += duration;
                    capacity = Math.max(capacity, objectCount(f));
                }
                capacities[i] = capacity; bases[i] = depth; depth += capacity; max = Math.max(max, end);
            }
            if (depth > 65535) throw new IOException("SWF depth limit exceeded");
            length = max;
        }
        List<Placement> frame(int index) throws IOException {
            List<Placement> result = new ArrayList<>();
            for (int i = 0; i < layers.size(); i++) {
                FlaRecord layer = layers.get(i); int type = layer.getLayerType();
                if (type == 1 || type == 3) continue;
                if (type != 0 && type != 2 && type != 4 && type != 5) throw unsupported(layer, "Unknown layer type " + type);
                int start = 0;
                List<FlaRecord> frames = keys.get(i);
                for (int k = 0; k < frames.size(); k++) {
                    FlaRecord f = frames.get(k); int duration = (int) n(f, "duration", 1);
                    if (index >= start && index < start + duration) {
                        List<Placement> items = new ArrayList<>();
                        if ((n(f, "keyMode", 0) & 255) == 2 && index > start)
                            items.addAll(morphs.frame(f, (index - start) / (double) duration));
                        else objects(items, f, index - start);
                        if ((n(f, "keyMode", 0) & 255) == 1 && layer.getLayerParent() != null && layer.getLayerParent().getLayerType() == 1)
                            throw unsupported(layer, "Guided motion paths are not supported");
                        if ((n(f, "keyMode", 0) & 255) == 1 && k + 1 < frames.size() && index > start)
                            tween(items, f, frames.get(k + 1), (index - start) / (double) duration);
                        if (items.size() > capacities[i]) throw unsupported(f, "Unexpected display-list expansion");
                        for (int j = 0; j < items.size(); j++) { Placement item = items.get(j); item.depth = bases[i] + j; result.add(item); }
                        break;
                    }
                    start += duration;
                }
            }
            for (int i = 0; i < layers.size(); i++) if (layers.get(i).getLayerType() == 4) {
                int min = Integer.MAX_VALUE, max = 0;
                for (int j = 0; j < layers.size(); j++) if (layers.get(j).getLayerParent() == layers.get(i)) {
                    min = Math.min(min, bases[j]); max = Math.max(max, bases[j] + capacities[j] - 1);
                }
                List<Placement> masks = new ArrayList<>();
                for (Placement item : result) if (item.depth >= bases[i] && item.depth < bases[i] + capacities[i]) masks.add(item);
                if (max > 0 && !masks.isEmpty()) {
                    if (masks.size() != 1 || min <= masks.get(0).depth)
                        throw unsupported(layers.get(i), "Mask requires one shape below a contiguous masked depth range");
                    masks.get(0).clipDepth = max;
                }
            }
            return result;
        }
        void events(SwfWriter out, int index) throws IOException {
            Set<String> labels = new HashSet<>();
            int streaming = 0;
            for (int i = 0; i < layers.size(); i++) {
                int type = layers.get(i).getLayerType(); if (type == 1 || type == 3) continue;
                int start = 0;
                for (FlaRecord f : keys.get(i)) {
                    if (n(f, "soundMediaId", 0) != 0 && n(f, "soundSync", 0) == 3
                            && index >= start && index < start + n(f, "duration", 1)) {
                        if (++streaming > 1) throw unsupported(f, "Concurrent streaming tracks require audio mixing");
                        streamSound(out, f, index - start);
                    }
                    if (start == index) {
                        String label = s(f, "name", "");
                        if (!label.isEmpty() && n(f, "labelType", 0) != 1 && labels.add(label)) {
                            SwfWriter l = new SwfWriter(); l.string(label); if (n(f, "bookmark", 0) != 0) l.u8(1); out.tag(43, l);
                        }
                        String source = source(record(f, "actions")); if (!source.trim().isEmpty()) out.tag(12, script(source, f));
                        if (n(f, "soundMediaId", 0) != 0 && n(f, "soundSync", 0) != 3) startSound(out, f);
                        break;
                    }
                    start += (int) n(f, "duration", 1);
                }
            }
        }
    }
    private int objectCount(FlaRecord r) {
        if (r.getClassName().equals("CPicFrame") || r.getClassName().equals("CPicShape")) {
            int count = records(record(r, "shapeData"), "edges").isEmpty() ? 0 : 1;
            for (FlaRecord c : r.getChildren()) count += objectCount(c);
            return count;
        }
        return 1;
    }
    private void tween(List<Placement> current, FlaRecord frame, FlaRecord next, double t) throws IOException {
        List<Placement> end = new ArrayList<>(); objects(end, next, 0);
        if (end.size() != current.size()) throw unsupported(frame, "Motion tween changes object count");
        for (int i = 0; i < current.size(); i++) {
            TweenCompiler.interpolate(new TweenCompiler.PlacementPair(current.get(i), end.get(i)), frame, t);
        }
    }

    private void button(BinaryFlaDocument.Page page, int id, Timeline t, boolean menu) throws IOException {
        if (t.length > 4) throw unsupported(page.getDescriptor(), "Button timeline exceeds four states");
        for (List<FlaRecord> frames : t.keys) for (FlaRecord frame : frames)
            if (!source(record(frame, "actions")).trim().isEmpty()) throw unsupported(frame, "Button states cannot contain frame scripts");
        SwfWriter out = new SwfWriter(); out.u16(id); out.u8(menu ? 1 : 0); out.u16(0);
        for (int f = 0; f < t.length; f++) {
            for (Placement p : t.frame(f)) {
                if (!source(record(p.record, "actions")).trim().isEmpty() || EffectsCompiler.filters(p.record).length > 0)
                    throw unsupported(p.record, "Button child has scripts or filters");
                out.u8(1 << f); out.u16(p.id); out.u16(p.depth); out.matrix(p.matrix);
                SwfWriter c = EffectsCompiler.color(p.record); if (c == null) { out.bits(0, 6); out.align(); } else out.bytes(c.data());
            }
        }
        out.u8(0); define(34, out);
    }

    byte[] script(String source, FlaRecord context) throws IOException {
        byte[] cached = actionCache.get(source); if (cached != null) return cached;
        try {
            DoActionTag tag = new DoActionTag(scriptContext);
            tag.setActions(new ActionScript2Parser(scriptContext, tag).actionsFromString(source, "UTF-8"));
            byte[] result = tag.getActionBytes().getRangeData(); actionCache.put(source, result); return result;
        } catch (Exception ex) { throw unsupported(context, "ActionScript compilation failed: " + ex.getMessage()); }
    }
    private byte[] clipActions(String source, FlaRecord owner) throws IOException {
        if (source.trim().isEmpty()) return new byte[0];
        byte[] cached = clipCache.get(owner); if (cached != null) return cached;
        try {
            PlaceObject3Tag host = new PlaceObject3Tag(scriptContext);
            CLIPACTIONS clips = new CLIPACTIONS(); clips.clipActionRecords = new ArrayList<>();
            host.clipActions = clips;
            for (String handler : ScriptHandlers.split(source)) {
                CLIPACTIONRECORD action = new CLIPACTIONRECORD(scriptContext, host);
                action.setParentClipActions(clips); clips.clipActionRecords.add(action);
                action.setActions(new ActionScript2Parser(scriptContext, action).actionsFromString(handler, "UTF-8"));
            }
            clips.calculateAllEventFlags();
            ByteArrayOutputStream bytes = new ByteArrayOutputStream(); SWFOutputStream out = new SWFOutputStream(bytes, 8, "UTF-8");
            out.writeCLIPACTIONS(clips); out.flush(); byte[] result = bytes.toByteArray(); clipCache.put(owner, result); return result;
        } catch (Exception ex) { throw unsupported(owner, "Instance event compilation failed: " + ex.getMessage()); }
    }
    static String source(FlaRecord actions) throws IOException {
        if (actions == null) return "";
        if (actions.get("source") instanceof String) return (String) actions.get("source");
        if (!records(actions, "commands").isEmpty() || record(actions, "compiledCommands") != null)
            throw unsupported(actions, "Archived editor-command actions need source reconstruction");
        return "";
    }

    static final class Bitmap { final int id, shapeId; Bitmap(int id, int shapeId) { this.id = id; this.shapeId = shapeId; } }
    Bitmap bitmap(Integer mediaId) throws IOException {
        if (mediaId == null) throw new IOException("Missing bitmap media ID");
        long key = mediaId.longValue(); Bitmap old = bitmaps.get(key); if (old != null) return old;
        BinaryFlaDocument.Media m = media.get(key);
        if (m == null || !m.getDescriptor().getClassName().equals("CMediaBits")) throw new IOException("Missing bitmap " + key);
        if (recovered == null) recovered = new BinaryFlaToXflExporter().export(document);
        int ordinal = 0;
        for (BinaryFlaDocument.Media item : document.getMedia()) {
            if (item.getDescriptor().getClassName().equals("CMediaBits")) ordinal++;
            if (item == m) break;
        }
        BinaryData encoded = recovered.getFiles().get("LIBRARY/bitmap-" + ordinal + ".png");
        if (encoded == null) encoded = recovered.getFiles().get("LIBRARY/bitmap-" + ordinal + ".jpg");
        if (encoded == null) throw unsupported(m.getDescriptor(), "Bitmap recovery failed: " + recovered.getReport());
        BufferedImage image = ImageIO.read(new ByteArrayInputStream(encoded.toByteArray()));
        if (image == null) throw unsupported(m.getDescriptor(), "Unrecognized bitmap data");
        if (image.getWidth() > 65535 || image.getHeight() > 65535) throw unsupported(m.getDescriptor(), "Bitmap dimensions exceed SWF 8 limits");
        int id = nextId(); SwfWriter pixels = new SwfWriter();
        for (int y = 0; y < image.getHeight(); y++) for (int x = 0; x < image.getWidth(); x++) {
            int c = image.getRGB(x, y), a = c >>> 24;
            pixels.u8(a); pixels.u8(((c >>> 16 & 255) * a + 127) / 255);
            pixels.u8(((c >>> 8 & 255) * a + 127) / 255); pixels.u8(((c & 255) * a + 127) / 255);
        }
        SwfWriter imageTag = new SwfWriter(); imageTag.u16(id); imageTag.u8(5); imageTag.u16(image.getWidth()); imageTag.u16(image.getHeight());
        imageTag.bytes(SwfWriter.compress(pixels.data())); define(36, imageTag);
        int shapeId = nextId(); SwfWriter shape = new SwfWriter(); shape.u16(shapeId);
        int w = image.getWidth() * 20, h = image.getHeight() * 20;
        shape.rect(0, w, 0, h); shape.u8(1); shape.u8(n(m.getDescriptor(), "allowSmoothing", 0) != 0 ? 0x41 : 0x43);
        shape.u16(id); shape.matrix(new double[]{20, 0, 0, 20, 0, 0}); shape.u8(0);
        shape.bits(1, 4); shape.bits(0, 4); shape.bits(0, 1); shape.bits(3, 5); shape.bits(1, 5); shape.bits(0, 1); shape.bits(0, 1); shape.bits(1, 1);
        ShapeCompiler.line(shape, w, 0); ShapeCompiler.line(shape, 0, h); ShapeCompiler.line(shape, -w, 0); ShapeCompiler.line(shape, 0, -h);
        shape.bits(0, 6); shape.align(); define(32, shape);
        Bitmap result = new Bitmap(id, shapeId); bitmaps.put(key, result); return result;
    }
    private int sound(long mediaId) throws IOException {
        Integer old = sounds.get(mediaId); if (old != null) return old;
        BinaryFlaDocument.Media m = media.get(mediaId);
        if (m == null || m.getSoundFormat() == null || m.getStream() == null) throw new IOException("Missing sound " + mediaId);
        FlaSoundFormat f = m.getSoundFormat();
        if (f.getUnknownFlags() != 0 || f.getCodecId() != 0 && f.getCodecId() != 2 && f.getCodecId() != 3)
            throw unsupported(m.getDescriptor(), "Unsupported sound codec " + f.getCodecId());
        FlaRecord audio = record(m.getStream().getRoot(), "primaryAudio");
        if (audio == null || Boolean.TRUE.equals(audio.get("shortRead")) || !(audio.get("data") instanceof BinaryData))
            throw unsupported(m.getDescriptor(), "Incomplete sound buffer");
        int id = nextId(); SwfWriter out = new SwfWriter(); out.u16(id);
        out.u8((f.getCodecId() == 0 ? 3 : f.getCodecId()) << 4 | f.getSampleRateCode() << 2
                | (f.getBitsPerSample() == 16 ? 2 : 0) | (f.getChannelCount() == 2 ? 1 : 0));
        out.u32(n(m.getDescriptor(), "sampleCount", 0)); if (f.getCodecId() == 2) out.u16(0);
        out.bytes(((BinaryData) audio.get("data")).toByteArray()); define(14, out); sounds.put(mediaId, id); return id;
    }
    private void startSound(SwfWriter out, FlaRecord f) throws IOException {
        int mode = (int) n(f, "soundSync", 0);
        if (mode < 0 || mode > 2) throw unsupported(f, "Streaming sound is not supported");
        SwfWriter s = new SwfWriter(); s.u16(sound(n(f, "soundMediaId", 0)));
        long in = n(f, "soundInPoint", 0), end = n(f, "soundOutPoint", 0x3fffffff), loops = n(f, "soundLoopCount", 1);
        List<?> envelope = f.get("soundEnvelope") instanceof List ? (List<?>) f.get("soundEnvelope") : Collections.emptyList();
        s.u8((mode == 2 ? 32 : 0) | (mode == 1 ? 16 : 0) | (envelope.isEmpty() ? 0 : 8)
                | (loops == 1 ? 0 : 4) | (end == 0x3fffffff ? 0 : 2) | (in == 0 ? 0 : 1));
        if (in != 0) s.u32(in); if (end != 0x3fffffff) s.u32(end); if (loops != 1) s.u16((int) loops);
        if (!envelope.isEmpty()) {
            if (envelope.size() > 255) throw unsupported(f, "Too many sound envelope points");
            s.u8(envelope.size());
            for (Object o : envelope) { List<?> p = (List<?>) o; s.u32(((Number) p.get(0)).longValue()); s.u16(((Number) p.get(1)).intValue()); s.u16(((Number) p.get(2)).intValue()); }
        }
        out.tag(15, s);
    }

    private final Map<Long, byte[]> pcmCache = new HashMap<>();
    private void streamSound(SwfWriter out, FlaRecord frame, int age) throws IOException {
        long id = n(frame, "soundMediaId", 0); BinaryFlaDocument.Media m = media.get(id);
        if (m == null || m.getSoundFormat() == null || m.getStream() == null) throw unsupported(frame, "Missing streaming sound");
        FlaSoundFormat f = m.getSoundFormat();
        if (f.getUnknownFlags() != 0 || f.getCodecId() != 0 && f.getCodecId() != 2 && f.getCodecId() != 3)
            throw unsupported(frame, "Unsupported streaming sound codec " + f.getCodecId());
        int bits = f.getCodecId() == 2 ? 16 : f.getBitsPerSample(), channels = f.getChannelCount(), stride = channels * bits / 8;
        byte[] pcm = pcmCache.get(id);
        if (pcm == null) {
            FlaRecord audio = record(m.getStream().getRoot(), "primaryAudio");
            if (audio == null || Boolean.TRUE.equals(audio.get("shortRead")) || !(audio.get("data") instanceof BinaryData))
                throw unsupported(frame, "Incomplete streaming audio buffer");
            pcm = ((BinaryData) audio.get("data")).toByteArray();
            if (f.getCodecId() == 2) {
                SwfWriter mp3 = new SwfWriter(); mp3.u16(0); mp3.bytes(pcm);
                SoundFormat format = new SoundFormat(2, f.getSampleRateHz(), channels == 2, true);
                pcm = format.getDecoder().decode(new SWFInputStream(scriptContext, mp3.data()));
            }
            pcmCache.put(id, pcm);
        }
        double samplesPerFrame = f.getTimebaseRateHz() / document.getFrameRate();
        if (samplesPerFrame > 65535) throw unsupported(frame, "Streaming sample count exceeds SWF range");
        if (age == 0) {
            int flags = f.getSampleRateCode() << 2 | (bits == 16 ? 2 : 0) | (channels == 2 ? 1 : 0);
            SwfWriter head = new SwfWriter(); head.u8(flags); head.u8(3 << 4 | flags); head.u16((int) Math.round(samplesPerFrame)); out.tag(45, head);
        }
        long first = Math.round(n(frame, "soundInPoint", 0) * f.getTimebaseRateHz() / 44100.0);
        long end = Math.min(pcm.length / stride, Math.round(n(frame, "soundOutPoint", 0x3fffffff) * f.getTimebaseRateHz() / 44100.0));
        if (end <= first) throw unsupported(frame, "Empty streaming sound interval");
        long length = end - first, begin = Math.round(age * samplesPerFrame), finish = Math.round((age + 1) * samplesPerFrame);
        int loops = (int) n(frame, "soundLoopCount", 1); SwfWriter block = new SwfWriter();
        List<?> envelope = frame.get("soundEnvelope") instanceof List ? (List<?>) frame.get("soundEnvelope") : Collections.emptyList();
        for (long sample = begin; sample < finish; sample++) {
            long offset = (first + sample % length) * stride;
            boolean silent = sample / length >= loops;
            double[] gain = streamGain(envelope, (first + sample % length) * 44100.0 / f.getTimebaseRateHz());
            for (int c = 0; c < channels; c++) {
                double volume = gain[Math.min(c, 1)]; int value;
                if (bits == 8) { value = silent ? 128 : ((pcm[(int) offset + c] & 255) - 128); block.u8(silent ? 128 : (int) Math.max(0, Math.min(255, Math.round(value * volume + 128)))); }
                else {
                    int p = (int) offset + c * 2; value = silent ? 0 : (short) ((pcm[p] & 255) | pcm[p + 1] << 8);
                    block.u16((int) Math.max(-32768, Math.min(32767, Math.round(value * volume))));
                }
            }
        }
        out.tag(19, block);
    }
    private static double[] streamGain(List<?> points, double position) throws IOException {
        if (points.isEmpty()) return new double[]{1, 1};
        List<?> a = (List<?>) points.get(0), b = a;
        for (Object o : points) { List<?> p = (List<?>) o; if (((Number) p.get(0)).doubleValue() > position) { b = p; break; } a = b = p; }
        double x0 = ((Number) a.get(0)).doubleValue(), x1 = ((Number) b.get(0)).doubleValue();
        double t = x1 == x0 ? 0 : Math.max(0, Math.min(1, (position - x0) / (x1 - x0))); double[] result = new double[2];
        for (int i = 0; i < 2; i++) result[i] = (((Number) a.get(i + 1)).doubleValue() * (1 - t) + ((Number) b.get(i + 1)).doubleValue() * t) / 32768.0;
        return result;
    }

    static long n(FlaRecord r, String name, long fallback) { Object v = r == null ? null : r.get(name); return v instanceof Number ? ((Number) v).longValue() : fallback; }
    static double number(FlaRecord r, String name, double fallback) { Object v = r == null ? null : r.get(name); return v instanceof Number ? ((Number) v).doubleValue() : fallback; }
    static String s(FlaRecord r, String name, String fallback) { Object v = r == null ? null : r.get(name); return v instanceof String ? (String) v : fallback; }
    static FlaRecord record(FlaRecord r, String name) { Object v = r == null ? null : r.get(name); return v instanceof FlaRecord ? (FlaRecord) v : null; }
    @SuppressWarnings("unchecked") static List<FlaRecord> records(FlaRecord r, String name) { Object v = r == null ? null : r.get(name); return v instanceof List ? (List<FlaRecord>) v : Collections.<FlaRecord>emptyList(); }
    static IOException unsupported(FlaRecord r, String text) { return new IOException(text + (r == null ? "" : " [" + r.getClassName() + " at 0x" + Integer.toHexString(r.getOffset()) + "]")); }
}
