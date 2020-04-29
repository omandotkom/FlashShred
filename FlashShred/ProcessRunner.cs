using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace FlashShred
{
    class ProcessRunner
    {
        protected ProcessStartInfo processStartInfo;
        public ProcessRunner(string execFilePath) {
            processStartInfo = new ProcessStartInfo(execFilePath);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            
        }
        public void runWithArguments(string args) {
            this.processStartInfo.Arguments = args;
            Process.Start(processStartInfo).WaitForExit();
           
        }
        public void run() {
            Process.Start(processStartInfo).WaitForExit();
        }

    }
}
