using System;
using System.IO;

// These two methods are copied verbatim from SDK/defaultscript.cs. The fixture
// preparer rejects drift before this isolated original-script oracle is used.
class AdditionalControl
{
    static int Main(string[] args)
    {
        try {
            if(Directory.Exists(args[0])) CopyFolders(args[0],args[2]);
            if(Directory.Exists(args[1])) CopyFolders(args[1],args[2]);
            return 0;
        } catch(Exception error) {Console.Error.WriteLine(error);return 7;}
    }
        public static void CopyFolders(string source, string destination)
        {
			if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
			
            DirectoryInfo di = new DirectoryInfo(source);
            CopyFiles(source, destination);

            foreach (DirectoryInfo d in di.GetDirectories())
            {
                string newDir = Path.Combine(destination, d.Name);
                CopyFolders(d.FullName, newDir);
            }
        }

        public static void CopyFiles(string source, string destination)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            FileInfo[] files = di.GetFiles();

            foreach (FileInfo f in files)
            {
                string sourceFile = f.FullName;
                string destFile = Path.Combine(destination, f.Name);
                File.Copy(sourceFile, destFile, true);
            }
        }

}
