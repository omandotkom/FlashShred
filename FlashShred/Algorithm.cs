using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
namespace FlashShred
{
    
    class Algorithm
    {
        protected String[] fileNames;
        protected DriveInfo drive;
        protected ProcessStartInfo processStartInfo;
        public Algorithm(String[] filenames) {
           
            this.fileNames = filenames;
           //assumed array alwyas has value, validate in frontend!
            drive = new DriveInfo((string)fileNames.GetValue(0));
            string fsutil = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName + @"\fsutil.exe";
            
            processStartInfo = new ProcessStartInfo(fsutil);
            string startInfoArg = "file createnew " + drive.Name + "temp_file " + drive.AvailableFreeSpace.ToString();
            processStartInfo.Arguments = startInfoArg;
            Console.WriteLine(drive.Name + " : " + startInfoArg);
            Process.Start(processStartInfo);
        }
        
        private int cnt() {
            return fileNames.Length;
        }
    }
}
