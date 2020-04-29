using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Windows.Forms;
namespace FlashShred
{

    class Algorithm
    {
        public delegate void OnOverwritten(string message);
        public delegate void OnPerformStep(string message);
        public event OnOverwritten onOverwrittenEvent;
        public event OnPerformStep onPerformStepEvent;
        protected String[] fileNames;
        protected DriveInfo drive;
        protected List<string> junkFile;
        protected List<string> newFiles;
        private ProcessRunner fsutil, sync;
        private string startInfoArg;
        private int total = 0;
        private const bool IS_SHRINK = true;
        private Stopwatch watch = new System.Diagnostics.Stopwatch();
        private static int totalfiles;
        public static string getCurrentDirectory() {
            return new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName;
        }
        public void Algorithm_onOverwrittenEventq(string message)
        {
            total = total + 1;
            UInt16 tempIter = 0;
            Util.write("Done, overwrittten " + message + ", current total" + total);
            if (total == fileNames.Length)
            {//this can be async
                onPerformStepEvent.Invoke("Menghapus file... ");
                Util.write("Begin removing file...");
                newFiles.ForEach(delegate (string f) {
                    //Console.WriteLine("Removing " + f);
                    if (f != null)
                    {
                        Util.write("Removing " + f);
                        File.Delete(f);
                    }
                });
                onPerformStepEvent.Invoke("Membuat junk file... ");
                while (drive.AvailableFreeSpace > 0 && IS_SHRINK)
                {
                    string jfilename = drive.Name + "junk_file_" + tempIter;
                    startInfoArg = "file createnew " + jfilename + " " + drive.AvailableFreeSpace.ToString();
                    junkFile.Add(jfilename);
                    Util.write("creating " + jfilename);
                    fsutil.runWithArguments(startInfoArg);
                    tempIter++;
                }
                Util.write("Sync Phase 2 ...");
                sync.run();
                onPerformStepEvent.Invoke("Selesai membuat junk file... ");
                // onPerformStepEvent.Invoke("Membuat file sampah");
                Util.write("Starting to remove files, current free space is " + drive.AvailableFreeSpace);
                Util.write("Begin remove shrink file...");
                onPerformStepEvent.Invoke("Menghapus junk file... ");
                junkFile.ForEach(delegate (string jf)
                {
                    if (jf != null && File.Exists(jf))
                    {
                        //Console.WriteLine("Removing " + jf);
                        Util.write("removing junkfile " + jf);
                        File.Delete(jf);
                    }

                });
                //onPerformStepEvent.Invoke("Membuat file temp");
                watch.Stop();
                Util.write("Elapsed time is : " + watch.ElapsedMilliseconds + " ms");
                Util.export();
                onPerformStepEvent.Invoke("Selesai, waktu : " + watch.ElapsedMilliseconds + " ms");

            }
            else
            {
                //onPerformStepEvent.Invoke("Membuat file temp");
            }
        }
        public string[] FileNames
        {
            get { return this.fileNames; }
            set
            {
                this.fileNames = value;
            }
        }
        public Algorithm() {
            //assumed array alwyas has value, validate in frontend!
            onOverwrittenEvent += Algorithm_onOverwrittenEventq;
            
            junkFile = new List<string>();
            newFiles = new List<string>();
            fsutil = new ProcessRunner(getCurrentDirectory() + @"\fsutil.exe");
            sync = new ProcessRunner(getCurrentDirectory() + @"\sync.exe");
           
        }   

        public Boolean CheckValidity() {
            Util.write("Begin checking validity...");
            //checks if files is from the same drive
            foreach (string s in fileNames) {
                FileInfo f = new FileInfo(s);
                if (!f.Exists) {
                    throw new System.Exception(f.FullName + " is not exists, aborting action...");
                    //one of the files doesn't exist
                }
                if (f.IsReadOnly) {
                    throw new System.Exception(f.FullName + " is used by another program, please close all the coresponding processes.");
                    //one of the files is read only
                }
           
        }
            drive = new DriveInfo((string)fileNames.GetValue(0));
            //checks if there's available free space
            if (drive.AvailableFreeSpace == 0)
            {
                throw new System.Exception("Not enough disk space, it's only 0 byte left");
            }
            Util.write("Done, everything is valid.");
            return true;
        }
        public async Task <int> shred() {
            totalfiles = fileNames.Length;
            if (fsutil is null) {
                throw new NullReferenceException();
            }
            watch.Start();
            UInt16 tempIter = 0;
            //the drive variabel maybe not public because the app needs realtime freespace tho
            onPerformStepEvent.Invoke("Membuat file temporary...");
            while (drive.AvailableFreeSpace > 0 && IS_SHRINK) 
            {
                string tfilename = drive.Name + "temp_file_" + tempIter;
                startInfoArg = "file createnew " + tfilename +" " + drive.AvailableFreeSpace.ToString();
                Util.write("creating " + tfilename);
                fsutil.runWithArguments(startInfoArg);
                junkFile.Add(tfilename);
                tempIter++;
            }
            Util.write("Sync Phase 1 ...");
            sync.run();
            onPerformStepEvent.Invoke("Selesai Membuat file temporary");
            foreach (string file in fileNames)
            {
                ThreadPool.QueueUserWorkItem((x) => OverwriteFile((string) x), file);
                             
            }
            
            
            
            //.export(getCurrentDirectory());
            return 1;
        }
        private void OverwriteFile(string filex){

            byte[] oldFileBytes = File.ReadAllBytes(filex);
            //FileInfo file = new FileInfo(filex);
            //FileInfo file = new FileInfo(filex);
            Util.write("Processing " + filex);
            onPerformStepEvent.Invoke("Memulai shred file " + filex);
            
            const int MAX_BUFFER_SIZE = 4096;
            using (var stream = new FileStream(filex, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = null;
                buffer = new byte[MAX_BUFFER_SIZE];
                /*for (int i = 0; i < MAX_BUFFER_SIZE; i++)
                {

                 //   buffer[i] = (byte)_random.Next(0, 255);
                   buffer[i] = (byte)0;
                }*/
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(buffer);
                
                for (var length = oldFileBytes.Length; length > 0; length -= MAX_BUFFER_SIZE)
                {
                    int bytesToWrite = (length > MAX_BUFFER_SIZE) ? MAX_BUFFER_SIZE : (int)length;

                     stream.Write(buffer, 0, bytesToWrite);
                         stream.Flush();
                    //file.Delete();                   
                }
                
            }
            var directoryName = Path.GetDirectoryName(filex);

            
            var newPath = Path.Combine(directoryName, Path.GetRandomFileName());
            Util.write("Renaming " + filex + " -> " + newPath);
            //sementara
            new FileInfo(filex).MoveTo(newPath);
            
            newFiles.Add(newPath);
            //File.Delete(newPath);
            //_onOverwritten(nm);
            byte[] newFileBytes = File.ReadAllBytes(newPath);
            Util.write(filex + " verification result : " + new Verifier(oldFileBytes, newFileBytes).VerifyHash());
            onPerformStepEvent.Invoke("Selesai Shred file " + filex);
            onOverwrittenEvent.Invoke(filex);
        }
      
    }
}
