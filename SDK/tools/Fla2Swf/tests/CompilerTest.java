package newsdk.fla;

import com.jpexs.flash.fla.reader.*;
import com.jpexs.flash.fla.converter.*;
import com.jpexs.decompiler.flash.SWF;
import com.jpexs.decompiler.flash.tags.*;
import com.jpexs.decompiler.flash.types.*;
import com.jpexs.decompiler.flash.timeline.DepthState;
import com.jpexs.decompiler.flash.exporters.commonshape.Matrix;
import com.jpexs.helpers.SerializableImage;
import java.awt.Color;
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.*;
import java.util.*;
import static newsdk.fla.FlaCompiler.*;

/** No test framework: real CFB round trips, independent SWF parsing and rendered pixels. */
public final class CompilerTest {
    private static Path fixture, work;
    private static int checks;
    public static void main(String[] args) {
        try { tests(args); System.exit(0); }
        catch (Throwable ex) { ex.printStackTrace(); System.exit(1); }
    }
    private static void tests(String[] args) throws Exception {
        fixture = Paths.get(args[0], "flash8-empty.fla");
        work = Files.createTempDirectory(Paths.get(args[1]), "compiler-test-");
        try {
            headersAndVersion(); cfbRoundTripAndPixels(); holeAndLayers(); mask(); nestedMovieAndTween();
            labelsAndScenes(); scriptsAndErrors(); textAndFonts(); streamingSound(); customEase(); morph(); buttonVariant(); safeOutput();
            System.out.println("PASS: " + checks + " structural, pixel and failure assertions");
        } finally {
            try (java.util.stream.Stream<Path> paths = Files.walk(work)) {
                for (Path p : (Iterable<Path>) paths.sorted(Comparator.reverseOrder())::iterator) Files.delete(p);
            }
        }
    }
    private static BinaryFlaDocument seed() throws Exception { return new BinaryFlaReader().read(fixture.toFile()); }
    private static FlaRecord root(BinaryFlaDocument d) { return d.getPages().get(0).getRoot(); }
    private static FlaRecord layer(BinaryFlaDocument d) { return root(d).getChildren().get(0); }
    private static FlaRecord frame(BinaryFlaDocument d) { return layer(d).getChildren().get(0); }
    private static void ok(boolean value, String message) { if (!value) throw new AssertionError(message); checks++; }
    private static SWF compile(BinaryFlaDocument d) throws Exception {
        FlaCompiler compiler = new FlaCompiler(d); return Main.validate(compiler.compile(false), compiler.frameCount);
    }
    private static SerializableImage render(SWF swf, int frame) {
        return SWF.frameToImageGet(swf.getTimeline(), frame, frame, null, 0, swf.displayRect, new Matrix(), null, Color.WHITE, 1, true, 1);
    }
    private static FlaRecord shape(int x, int y, int w, int h, long rgba) {
        FlaRecord r = FlaRecord.create("CPicShape"), data = FlaRecord.create("shapeData"), fill = FlaRecord.create("fills");
        fill.set("flags", 0); fill.set("secondaryFlags", 0); fill.set("packedValue", rgba);
        data.set("fills", Arrays.asList(fill)); data.set("strokes", Collections.emptyList());
        List<FlaRecord> edges = new ArrayList<>(); square(edges, x, y, w, h, false); data.set("edges", edges);
        r.set("shapeData", data); r.set("matrixWords", Arrays.asList(65536, 0, 0, 65536, 0, 0)); return r;
    }
    private static void square(List<FlaRecord> edges, int x, int y, int w, int h, boolean hole) {
        int[][] p = {{x, y}, {x+w, y}, {x+w, y+h}, {x, y+h}, {x, y}};
        for (int i = 0; i < 4; i++) {
            FlaRecord edge = FlaRecord.create("edge"); edge.set("strokeIndex", 0); edge.set("fillIndex0", hole ? 0 : 1); edge.set("fillIndex1", hole ? 1 : 0);
            edge.set("startX", p[i][0] * 5120); edge.set("startY", p[i][1] * 5120);
            edge.set("endX", p[i+1][0] * 5120); edge.set("endY", p[i+1][1] * 5120); edge.set("curve", false); edges.add(edge);
        }
    }
    private static void children(FlaRecord r, FlaRecord... children) { r.set("children", Arrays.asList(children)); }
    private static void headersAndVersion() throws Exception {
        BinaryFlaDocument d = seed(); byte[] fws = new FlaCompiler(d).compile(false), cws = new FlaCompiler(d).compile(true);
        ok(new String(fws, 0, 3, StandardCharsets.US_ASCII).equals("FWS") && fws[3] == 8, "FWS version 8");
        ok(new String(cws, 0, 3, StandardCharsets.US_ASCII).equals("CWS") && cws[3] == 8, "CWS version 8");
        ok(Main.validate(cws, 1).frameCount == 1, "Compressed body round trips");
        for (FlaFormatVersion version : Arrays.asList(FlaFormatVersion.MX2004, FlaFormatVersion.CS3, FlaFormatVersion.CS4)) {
            Path input = work.resolve(version + ".fla"), output = work.resolve(version + ".swf");
            new BinaryFlaConverter(version).convert(d).writeToFile(input.toFile());
            expectFailure(() -> Main.run(new String[]{input.toString(), "-o", output.toString()}), "Flash 8");
            ok(!Files.exists(output), "Rejected input has no SWF");
        }
    }
    private static void cfbRoundTripAndPixels() throws Exception {
        BinaryFlaDocument d = seed(); children(frame(d), shape(10, 10, 60, 40, 0xff0000ffL));
        Path input = work.resolve("中文 square.fla"), output = work.resolve("中文 square.swf");
        new BinaryFlaConverter(FlaFormatVersion.F8).convert(d).writeToFile(input.toFile());
        Main.run(new String[]{input.toString(), "-o", output.toString(), "--no-compress"});
        SWF swf = Main.validate(Files.readAllBytes(output), 1); SerializableImage image = render(swf, 0);
        ok(image.getRGB(30, 25) == 0xffff0000, "Red square survives binary FLA to rendered SWF");
        ok(image.getRGB(5, 5) == 0xffffffff, "Background remains white");
    }
    private static void holeAndLayers() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord ring = shape(10, 10, 80, 80, 0xff0000ffL);
        FlaRecord data = record(ring, "shapeData"); List<FlaRecord> edges = new ArrayList<>(records(data, "edges"));
        square(edges, 30, 30, 40, 40, true); data.set("edges", edges); children(frame(d), ring);
        SerializableImage image = render(compile(d), 0);
        ok(image.getRGB(20, 20) == 0xffff0000 && image.getRGB(50, 50) == 0xffffffff, "Two-sided contour preserves a hole");
        FlaRecord upper = TweenCompiler.copy(layer(d)), f = TweenCompiler.copy(frame(d));
        children(f, shape(15, 15, 10, 10, 0xffff0000L)); children(upper, f); children(root(d), layer(d), upper);
        image = render(compile(d), 0); ok(image.getRGB(20, 20) == 0xff0000ff, "Layer creation order preserves stacking");
    }
    private static void mask() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord content = layer(d), mask = TweenCompiler.copy(content);
        FlaRecord maskFrame = TweenCompiler.copy(frame(d)); children(frame(d), shape(0, 0, 100, 100, 0xff0000ffL));
        children(maskFrame, shape(20, 20, 30, 30, 0xff000000L)); children(mask, maskFrame);
        mask.set("type", 4); mask.set("legacyMask", 1); content.set("legacyRelatedLayer", mask); content.set("relatedLayer", mask);
        children(root(d), content, mask); SWF swf = compile(d); SerializableImage image = render(swf, 0);
        ok(image.getRGB(30, 30) == 0xffff0000 && image.getRGB(10, 10) == 0xffffffff, "Mask clips content at the intended depth");
        List<PlaceObject3Tag> places = places(swf);
        ok(places.get(0).clipDepth > places.get(0).depth, "Mask precedes masked depths in SWF");
    }
    private static BinaryFlaDocument movieDocument() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord symbolRoot = TweenCompiler.copy(root(d));
        FlaRecord symbolLayer = TweenCompiler.copy(layer(d)), symbolFrame = TweenCompiler.copy(frame(d));
        children(symbolFrame, shape(0, 0, 20, 20, 0xff0000ffL)); children(symbolLayer, symbolFrame); children(symbolRoot, symbolLayer);
        FlaRecord descriptor = TweenCompiler.copy(d.getSceneDescriptors().get(0)); descriptor.set("streamName", "Symbol 1"); descriptor.set("libraryId", 1L); descriptor.set("symbolType", 2L);
        FlaRecord clip = FlaRecord.create("CPicSprite"); clip.set("libraryId", 1L); clip.set("instanceName", "moving");
        clip.set("matrixWords", Arrays.asList(65536, 0, 0, 65536, 0, 0));
        FlaRecord handler = FlaRecord.create("actions"); handler.set("source", "onClipEvent (load) { var s = '}'; }\n// {\nonClipEvent (enterFrame) { this._alpha = 80; }"); clip.set("actions", handler);
        FlaRecord first = frame(d), second = TweenCompiler.copy(first), clip2 = TweenCompiler.copy(clip);
        first.set("duration", 2L); first.set("keyMode", 1537L); children(first, clip);
        clip2.set("matrixWords", Arrays.asList(65536, 0, 0, 65536, 800, 0)); children(second, clip2); children(layer(d), first, second);
        d.getContents().set("library", Arrays.asList(descriptor));
        return new FlaDocumentBuilder(d.getAnsiCharset()).stream("Contents", FlaStream.Kind.CONTENTS, d.getContents())
                .stream(d.getSceneDescriptors().get(0).get("streamName").toString(), FlaStream.Kind.PAGE, root(d))
                .stream("Symbol 1", FlaStream.Kind.PAGE, symbolRoot).build();
    }
    private static void nestedMovieAndTween() throws Exception {
        SWF swf = compile(movieDocument()); List<PlaceObject3Tag> places = places(swf);
        ok(swf.frameCount == 3 && places.size() == 3, "Nested movie has three parent frames");
        ok(places.get(0).clipActions.clipActionRecords.size() == 2, "Both clip-event handlers compile");
        ok(places.get(1).matrix.translateX == 400, "Classic tween midpoint is 20 pixels");
        ok(places.get(1).placeFlagMove && !places.get(1).placeFlagHasCharacter, "Moving a movie clip retains its instance");
        DepthState a = swf.getTimeline().getDepthState(0, 1), b = swf.getTimeline().getDepthState(1, 1);
        ok(a.instanceId == b.instanceId, "Independent SWF parser retains movie-clip identity across tween");
        ok(render(swf, 1).getRGB(25, 10) == 0xffff0000, "Nested graphic renders at tweened position");
    }
    private static void labelsAndScenes() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord first = frame(d), second = TweenCompiler.copy(first);
        first.set("name", "start"); first.set("duration", 2L); second.set("name", "finish"); children(layer(d), first, second);
        SWF swf = compile(d);
        ok(swf.getTimeline().getFrameWithLabel("start") == 0 && swf.getTimeline().getFrameWithLabel("finish") == 2, "Frame labels retain authored positions");
        FlaRecord descriptor = TweenCompiler.copy(d.getSceneDescriptors().get(0)); descriptor.set("streamName", "Page 2"); descriptor.set("libraryId", 2L);
        d.getContents().set("scenes", Arrays.asList(descriptor, d.getSceneDescriptors().get(0)));
        FlaRecord nextRoot = TweenCompiler.copy(root(d)), nextLayer = TweenCompiler.copy(layer(d)), nextFrame = TweenCompiler.copy(second);
        nextFrame.set("name", "otherScene"); children(nextLayer, nextFrame); children(nextRoot, nextLayer);
        BinaryFlaDocument two = new FlaDocumentBuilder(d.getAnsiCharset()).stream("Contents", FlaStream.Kind.CONTENTS, d.getContents())
                .stream("Page 2", FlaStream.Kind.PAGE, nextRoot).stream(d.getSceneDescriptors().get(1).get("streamName").toString(), FlaStream.Kind.PAGE, root(d)).build();
        swf = compile(two); ok(swf.frameCount == 4 && swf.getTimeline().getFrameWithLabel("otherScene") == 0, "Scene order follows Contents descriptors");
    }
    private static void scriptsAndErrors() throws Exception {
        BinaryFlaDocument d = seed(); children(frame(d), shape(0, 0, 20, 20, 0xff0000ffL));
        FlaRecord actions = TweenCompiler.copy(record(frame(d), "actions")); actions.set("source", "stop(); var message = '中文'; trace(message);"); frame(d).set("actions", actions);
        SWF swf = compile(d); DoActionTag action = null; for (Tag t : swf.getTags()) if (t instanceof DoActionTag) action = (DoActionTag) t;
        ok(action != null && action.getActions().stream().anyMatch(a -> a.getClass().getSimpleName().equals("ActionStop")), "Frame script compiles to AVM1 stop opcode");
        actions.set("source", "var = ;"); expectFailure(() -> compile(d), "ActionScript compilation failed");
        children(frame(d), FlaRecord.create("CPicVideoStream")); expectFailure(() -> compile(d), "Unsupported display object");
        ok(ScriptHandlers.split("on (release) { trace('}'); /* } */ } on (press) {trace('{');}").size() == 2, "Event parser ignores braces in comments and strings");
    }
    private static void textAndFonts() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord text = FlaRecord.create("CPicText"), run = FlaRecord.create("textRun"), style = FlaRecord.create("style");
        text.set("bounds", Arrays.asList(0, 2400, 0, 800)); text.set("runs", Arrays.asList(run));
        run.set("text", "Hello"); run.set("style", style); style.set("fontFamily", "_sans"); style.set("fontSize", 400); style.set("colorRgba", 0xff000000L);
        children(frame(d), text); SWF swf = compile(d); SerializableImage image = render(swf, 0); int dark = 0;
        for (int y = 0; y < 35; y++) for (int x = 0; x < 110; x++) if ((image.getRGB(x, y) & 0xffffff) < 0x808080) dark++;
        ok(dark > 50, "Static text embeds real font outlines and renders");
        text.set("textFlags", 3L); text.set("variableName", "score"); swf = compile(d); DefineEditTextTag edit = null;
        for (Tag t : swf.getTags()) if (t instanceof DefineEditTextTag) edit = (DefineEditTextTag) t;
        ok(edit != null && edit.readOnly && edit.variableName.equals("score") && edit.initialText.equals("Hello"), "Dynamic text retains variable binding and initial value");
    }
    private static void safeOutput() throws Exception {
        Path bad = work.resolve("bad.fla"), output = work.resolve("old.swf"); Files.write(bad, new byte[]{'P', 'K'}); Files.write(output, new byte[]{1, 2, 3});
        expectFailure(() -> Main.run(new String[]{bad.toString(), "-o", output.toString()}), "already exists");
        expectFailure(() -> Main.run(new String[]{bad.toString(), "-o", output.toString(), "--overwrite"}), "OLE/CFB");
        ok(Arrays.equals(Files.readAllBytes(output), new byte[]{1, 2, 3}), "Failed conversion preserves previous output");
        try (java.util.stream.Stream<Path> files = Files.list(work)) { ok(files.noneMatch(p -> p.getFileName().toString().startsWith(".fla2swf-")), "No partial output file remains"); }
    }
    private static void streamingSound() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord f = frame(d); f.set("duration", 2L); f.set("soundMediaId", 1L); f.set("soundSync", 3L);
        FlaRecord descriptor = FlaRecord.create("CMediaSound"); descriptor.set("revision", 6L); descriptor.set("mediaId", 1L);
        descriptor.set("formatFlags", 4L); descriptor.set("sampleCount", 1000L); descriptor.set("streamName", "Media 1");
        FlaRecord payload = FlaRecord.create("soundPayload"), audio = FlaRecord.create("primaryAudio"); byte[] pcm = new byte[1000]; Arrays.fill(pcm, (byte) 0x90);
        audio.set("data", new BinaryData(pcm)); payload.set("primaryAudio", audio); d.getContents().set("media", Arrays.asList(descriptor));
        BinaryFlaDocument sound = new FlaDocumentBuilder(d.getAnsiCharset()).stream("Contents", FlaStream.Kind.CONTENTS, d.getContents())
                .stream(d.getSceneDescriptors().get(0).get("streamName").toString(), FlaStream.Kind.PAGE, root(d))
                .stream("Media 1", FlaStream.Kind.MEDIA, payload).build();
        SWF swf = compile(sound); List<SoundStreamBlockTag> blocks = new ArrayList<>();
        for (Tag t : swf.getTags()) if (t instanceof SoundStreamBlockTag) blocks.add((SoundStreamBlockTag) t);
        ok(blocks.size() == 2, "Streaming sound has a block per timeline frame");
        int total = blocks.get(0).getData().length + blocks.get(1).getData().length;
        ok(total == 919 && blocks.get(0).getData()[0] == (byte) 0x90, "PCM timing carries fractional samples across frames");
    }
    private static void customEase() throws Exception {
        FlaRecord f = FlaRecord.create("CPicFrame"); f.set("customEasePresent", 1L); f.set("useSingleEaseCurve", 1L);
        List<List<List<Double>>> curves = new ArrayList<>(); for (int i = 0; i < 5; i++) curves.add(Collections.emptyList());
        curves.add(Arrays.asList(Arrays.asList(0d, 0d, 0d, 0d, 1d/3, 0d), Arrays.asList(2d/3, 0d, 1d, 1d, 1d, 1d)));
        f.set("customEaseCurves", curves);
        ok(Math.abs(TweenCompiler.ease(f, 0.5, 0) - 0.125) < 1e-7, "Custom cubic ease inverts time before evaluating progress");
    }
    private static void morph() throws Exception {
        BinaryFlaDocument d = seed(); FlaRecord first = frame(d), next = TweenCompiler.copy(first);
        FlaRecord a = shape(10, 10, 20, 20, 0xff0000ffL), b = shape(50, 10, 20, 20, 0xffff0000L);
        children(first, a); children(next, b); first.set("duration", 2L); first.set("keyMode", 1538L); children(layer(d), first, next);
        FlaRecord morph = FlaRecord.create("CPicMorphShape"), segment = FlaRecord.create("CMorphSegment");
        morph.set("matrixWordsA", Arrays.asList(65536, 0, 0, 65536, 0, 0)); morph.set("matrixWordsB", morph.get("matrixWordsA"));
        morph.set("fills", Arrays.asList(records(record(a, "shapeData"), "fills").get(0), records(record(b, "shapeData"), "fills").get(0)));
        morph.set("strokes", Collections.emptyList()); segment.set("fillIndex1", 0); segment.set("fillIndex2", 1);
        segment.set("strokeIndex1", -1); segment.set("strokeIndex2", -1);
        segment.set("startPointA", Arrays.asList(200, 200)); segment.set("startPointB", Arrays.asList(1000, 200));
        int[][] endpoints = {{600, 200}, {600, 600}, {200, 600}, {200, 200}}; List<FlaRecord> curves = new ArrayList<>();
        for (int[] point : endpoints) {
            FlaRecord curve = FlaRecord.create("CMorphCurve"); curve.set("lineFlags", 1L);
            curve.set("anchorPointA", Arrays.asList(point[0], point[1])); curve.set("anchorPointB", Arrays.asList(point[0] + 800, point[1]));
            curve.set("controlPointA", curve.get("anchorPointA")); curve.set("controlPointB", curve.get("anchorPointB")); curves.add(curve);
        }
        segment.set("curves", curves); morph.set("segments", Arrays.asList(segment)); first.set("morphShape", morph);
        SWF swf = compile(d); SerializableImage image = render(swf, 1);
        ok(image.getRGB(40, 20) == 0xff800080, "Stored morph correspondence interpolates geometry and color");
        ok(image.getRGB(15, 20) == 0xffffffff, "Shape tween replaces endpoint geometry on intermediate frames");
    }
    private static void buttonVariant() throws Exception {
        BinaryFlaDocument d = movieDocument(); FlaRecord f = frame(d), clip = f.getChildren().get(0);
        FlaRecord button = FlaRecord.create("CPicButton");
        for (Map.Entry<String, Object> entry : clip.getFields().entrySet()) button.set(entry.getKey(), entry.getValue());
        button.set("actions", null); button.set("trackAsMenu", 1L); children(f, button); f.set("keyMode", 1536L); f.set("duration", 1L); children(layer(d), f);
        SWF swf = compile(d); DefineButton2Tag tag = null;
        for (Tag t : swf.getTags()) if (t instanceof DefineButton2Tag) tag = (DefineButton2Tag) t;
        ok(tag != null && tag.trackAsMenu, "Instance type can override a movie-clip library item as a menu button");
        ok(render(swf, 0).getRGB(10, 10) == 0xffff0000, "Button up-state retains the source symbol artwork");
    }
    private static List<PlaceObject3Tag> places(SWF swf) { List<PlaceObject3Tag> result = new ArrayList<>(); for (Tag t : swf.getTags()) if (t instanceof PlaceObject3Tag) result.add((PlaceObject3Tag) t); return result; }
    private interface Checked { void run() throws Exception; }
    private static void expectFailure(Checked command, String text) throws Exception {
        try { command.run(); } catch (IOException ex) { ok(ex.getMessage().contains(text), "Error contains " + text + ": " + ex.getMessage()); return; }
        throw new AssertionError("Expected failure containing " + text);
    }
}
