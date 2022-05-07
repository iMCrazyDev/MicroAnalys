using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroAnalys
{
    public class PythonExecutor
    {
        public struct PythonExecuteParams
        {
            public int Exp;
            public int Scale;
            public string FilePath;
        }

        private static string ExecutePython(string args)
        {
            ProcessStartInfo pycheck = new ProcessStartInfo()
            {
                FileName = @"python.exe",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(pycheck))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static bool CheckInstallation()
        {
            return ExecutePython("--version").StartsWith("Python");
        }
    }
}
