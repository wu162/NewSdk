import java.lang.reflect.Field;
import com.jpexs.decompiler.flash.xfl.FLAVersion;

/**
 * Expose FFDec's MX2004 XFL stage without enabling modern motion-object tweens.
 *
 * FFDec normally immediately serializes an MX2004 export to binary FLA. We
 * retain the same MX2004 timeline conversion and temporarily disable only that
 * final serialization, so resolved shared symbols can be made local first.
 * This change is confined to this conversion process; vendor JARs stay intact.
 */
public final class LegacyXflExport {
    public static void main(String[] args) throws Exception {
        Field cfb = FLAVersion.class.getDeclaredField("cfbFlaVersion");
        cfb.setAccessible(true);
        cfb.set(FLAVersion.MX2004, null);
        Field version = FLAVersion.class.getDeclaredField("xflVersion");
        version.setAccessible(true);
        version.set(FLAVersion.MX2004, "2.0");
        com.jpexs.decompiler.flash.gui.Main.main(args);
    }
}
