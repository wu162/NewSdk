using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

// External read-only test executable. Never included in the product source set.
class CompilerDeployment
{
    static void Require(bool value,string message){if(!value)throw new Exception(message);}
    static int Main(string[] args)
    {
        try {
            string exe=Path.GetFullPath(args[0]), bin=Path.GetDirectoryName(exe);
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve+=(sender,e)=>{
                string name=new AssemblyName(e.Name).Name;
                if(name=="antlr.runtime")return Assembly.ReflectionOnlyLoadFrom(Path.Combine(bin,"antlr.runtime.dll"));
                Require(name=="mscorlib" || name=="System" || name.StartsWith("System."),"Unexpected assembly resolution "+name);
                return Assembly.ReflectionOnlyLoad(e.Name);
            };
            var own=Assembly.ReflectionOnlyLoadFrom(exe);
            Require(own.GetModules().Length==1,"Product still depends on netmodule files");
            Require(own.EntryPoint.DeclaringType.FullName=="NewModCompiler.Hosting.CompilerEntry","Production entry did not use the CLI host");
            foreach(string name in File.ReadAllLines(args[1]))Require(own.GetType(name,true).Assembly==own,"Business type ownership differs: "+name);
            foreach(string name in new[]{"SingleExeProbe","TextureProbe","CompilerDeployment","CompilerRoutes","NewModCompiler.Validation.ValidationEntry","NewSdk.BuildValidation.ManagedFoundationValidation","NewModCompiler.Tests.AppTests"})Require(own.GetType(name)==null,"Test/probe class entered product: "+name);
            foreach(Type type in own.GetTypes()) {
                Require(type.Assembly==own,"Type is owned by an external module: "+type.FullName);
                Require(type.Namespace!="NewModCompiler.Tests" && type.Namespace!="NewSdk.BuildValidation" && type.Namespace!="NewModCompiler.Validation","Test namespace entered product: "+type.FullName);
            }
            var json=new JavaScriptSerializer();
            var resources=(object[])json.DeserializeObject(File.ReadAllText(args[2]));
            Require(own.GetManifestResourceNames().Length==resources.Length,"Resource count differs");
            foreach(Dictionary<string,object> resource in resources) {
                string name=(string)resource["logicalName"], expected=(string)resource["sha256"];
                using(Stream input=own.GetManifestResourceStream(name))using(SHA256 sha=SHA256.Create()) {
                    Require(input!=null,"Missing "+name);
                    Require(BitConverter.ToString(sha.ComputeHash(input)).Replace("-","").ToLowerInvariant()==expected,"Resource bytes differ: "+name);
                }
            }
            foreach(AssemblyName reference in own.GetReferencedAssemblies())Require(reference.Name=="antlr.runtime" || reference.Name=="mscorlib" || reference.Name=="System" || reference.Name.StartsWith("System."),"Unexpected product AssemblyRef "+reference.FullName);
            Console.WriteLine("PASS product: exact Foundation type inventory belongs to EXE; one physical module; five exact resources; CLI hosting entry; no probes/tests; declared AssemblyRefs only");return 0;
        } catch(Exception error){Console.Error.WriteLine(error);return 1;}
    }
}


