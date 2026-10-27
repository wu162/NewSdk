// Evidence-only decompilation of fixed original native functions.
// @category TextureRecovery
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.Function;
import java.io.*;
import java.nio.charset.StandardCharsets;

public class ExportTextureDxt extends GhidraScript {
    public void run() throws Exception {
        File out = new File(getScriptArgs()[0]); out.mkdirs();
        long[] rvas = {0x57f00,0x57ff0,0x5c790,0x7d500,0x7e8f0,0x8b570,0x8b380};
        String[] names = {"EncodeDXT3","EncodeDXT5","CompressDispatch","QuantizeAlpha","CodeAlphaBlock","FastEncodeDXT5","FastEncodeDXT1AllPixelsSame"};
        DecompInterface decompiler = new DecompInterface();
        try {
            decompiler.openProgram(currentProgram);
            for(int i=0;i<rvas.length;i++) {
                Address address = currentProgram.getImageBase().add(rvas[i]);
                Function f = getFunctionAt(address);
                if(f==null) { disassemble(address); f=createFunction(address,names[i]); }
                if(f==null) throw new IOException("Missing function " + names[i]);
                DecompileResults result=decompiler.decompileFunction(f,90,monitor);
                if(!result.decompileCompleted()) throw new IOException(names[i]+": "+result.getErrorMessage());
                try(Writer writer=new OutputStreamWriter(new FileOutputStream(new File(out,names[i]+".c")),StandardCharsets.UTF_8)) {
                    writer.write("/* Original DLL evidence only; RVA 0x"+Long.toHexString(rvas[i])+"; not recovered business source. */\n");
                    writer.write(result.getDecompiledFunction().getC());
                }
                println("EXPORTED "+names[i]+" "+address);
            }
        } finally { decompiler.dispose(); }
    }
}
