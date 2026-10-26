using System;
using System.IO;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.AudioCompiler
{
    // Original 0x0600010B-0x0600010E. Cleanup is deterministic IDisposable work;
    // the original C++/CLI class does not declare a managed finalizer.
    internal class AutoCleanUpTempFiles : IDisposable
    {
        private string m_baseTempFileName;

        public AutoCleanUpTempFiles(string baseTempFileName)
        {
            m_baseTempFileName = baseTempFileName;
        }

        private void CleanUp()
        {
            Tracer tracer = Tracer.GetTracer("AudioCompiler", "Provides Audio processing functionality");
            string[] files = Directory.GetFiles(Path.GetDirectoryName(m_baseTempFileName),
                Path.GetFileName(m_baseTempFileName) + "*");
            foreach (string file in files)
            {
                tracer.TraceNote("Cleaning up stale temporary file {0}", file);
                try
                {
                    File.Delete(file);
                }
                catch (SystemException exception)
                {
                    tracer.TraceWarning("Could not delete stale temporary file {0}: {1}", file, exception);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) CleanUp();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
