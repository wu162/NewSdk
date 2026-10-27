// Exact native colour function boundaries and call graph; evidence only.
// @category TextureRecovery
import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.*;
import com.google.gson.GsonBuilder;
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.util.*;

public class ExportTextureDxtGraph extends GhidraScript {
    public void run() throws Exception {
        File out=new File(getScriptArgs()[0]);out.mkdirs();
        Address base=currentProgram.getImageBase();
        ArrayDeque<Function> queue=new ArrayDeque<>();
        for(long rva:new long[]{0x7c420,0x83cd0}) {
            Function f=getFunctionAt(base.add(rva));
            if(f==null){disassemble(base.add(rva));f=createFunction(base.add(rva),null);}
            if(f==null)throw new IOException("Missing colour root");queue.add(f);
        }
        Set<String> seen=new HashSet<>();List<Object> functions=new ArrayList<>();
        try(PrintWriter listing=new PrintWriter(new OutputStreamWriter(new FileOutputStream(new File(out,"graph.asm")),StandardCharsets.UTF_8))) {
            while(!queue.isEmpty()) {
                monitor.checkCancelled();Function f=queue.remove();
                String key=f.getEntryPoint().toString();if(!seen.add(key))continue;
                if(seen.size()>400)throw new IOException("Unexpected colour graph expansion");
                Map<String,Object> row=new LinkedHashMap<>();
                row.put("entry",key);row.put("name",f.getName());row.put("external",f.isExternal());
                row.put("thunk",f.isThunk());row.put("signature",f.getSignature().toString());
                List<Object> instructions=new ArrayList<>();List<String> callees=new ArrayList<>();
                listing.println("; FUNCTION "+key+" "+f.getName());
                if(!f.isExternal()) {
                    row.put("rva",f.getEntryPoint().subtract(base));
                    row.put("bodyBytes",f.getBody().getNumAddresses());
                    InstructionIterator iterator=currentProgram.getListing().getInstructions(f.getBody(),true);
                    while(iterator.hasNext()) {
                        Instruction ins=iterator.next();Map<String,Object> item=new LinkedHashMap<>();
                        item.put("rva",ins.getAddress().subtract(base));item.put("size",ins.getLength());
                        item.put("text",ins.toString());item.put("flow",ins.getFlowType().toString());
                        List<String> targets=new ArrayList<>();
                        for(Address target:ins.getFlows())targets.add(target.toString());
                        item.put("targets",targets);
                        List<Object> refs=new ArrayList<>();
                        for(Reference ref:ins.getReferencesFrom()) {
                            if(ref.getToAddress().isMemoryAddress()) {
                                Map<String,Object> reference=new LinkedHashMap<>();reference.put("operand",ref.getOperandIndex());
                                reference.put("target",ref.getToAddress().toString());reference.put("type",ref.getReferenceType().toString());refs.add(reference);
                            }
                        }
                        item.put("references",refs);instructions.add(item);
                        listing.printf("%08X\t%s%n",ins.getAddress().subtract(base),ins.toString());
                    }
                }
                for(Function called:f.getCalledFunctions(monitor)) {callees.add(called.getEntryPoint().toString());queue.add(called);}
                row.put("callees",callees);row.put("instructions",instructions);functions.add(row);
            }
        }
        Map<String,Object> result=new LinkedHashMap<>();result.put("evidenceOnly",true);result.put("probeOnly",true);
        result.put("scope","Original PC colour graph and exact instruction boundaries; no recovery acceptance");
        result.put("imageBase",base.toString());result.put("functions",functions);
        try(Writer writer=new OutputStreamWriter(new FileOutputStream(new File(out,"graph.json")),StandardCharsets.UTF_8)) {
            new GsonBuilder().setPrettyPrinting().create().toJson(result,writer);
        }
        println("EXPORTED colour functions="+functions.size());
    }
}
