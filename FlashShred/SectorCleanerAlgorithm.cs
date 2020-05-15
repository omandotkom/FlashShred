using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FlashShred
{

    public delegate void CleanerStarted(string letter);
    public delegate void CleanerProgress(string letter);
    public delegate void CleanerCleaning(string letter);
    public delegate void CleanerFinished(string letter);
    class SectorCleanerAlgorithm
    {//const long FILE_SIZE_IN_BYTE = 2097152; // 2MB https://www.unitconverters.net/data-storage-converter.html
     //const long FILE_SIZE_IN_BYTE = 1073741824; // 1gb
        private const long FILE_SIZE_IN_BYTE = 524288000; // 500mb
        public event CleanerStarted OnCleanerStarted;
        public event CleanerProgress OnCleanerProgress;
        public event CleanerCleaning OnCleanerCleaning;
        public event CleanerFinished OnCleanerFinished;
        public void TheCleanerStarted(string m)
        {
            OnCleanerStarted?.Invoke(m);
        }
        public void TheCleanerProgress(string m)
        {
            OnCleanerProgress?.Invoke(m);
        }
        public void TheCleanerCleaning(string m)
        {
            OnCleanerCleaning?.Invoke(m);
        }
        public void TheCleanerFinished(string m)
        {
            OnCleanerFinished?.Invoke(m);
        }
        public static DriveInfo[] GetDrives()
        {
            return DriveInfo.GetDrives();
        }
        private string drive_letter;
        public void SetDriveLetter(string lett) {
            this.drive_letter = lett;
        }
        public Int64 Steps()
        {
            long totalFreeBytes = new DriveInfo(drive_letter).TotalFreeSpace;
            return (totalFreeBytes / FILE_SIZE_IN_BYTE) + 1;

        }
        public void clean()
        {
            DriveInfo drive = new DriveInfo(drive_letter);
            long total_space = drive.TotalFreeSpace;
            long junk_size = 0;
            long num = 1;
            TheCleanerStarted("Sector Cleaning " + drive_letter + " " + total_space + " bytes in total.");
            List<string> q_file_list = new List<string>();

            while (total_space > 0)
            {
                if (total_space <= FILE_SIZE_IN_BYTE)
                {

                    junk_size = total_space;
                    total_space -=  - total_space;
                }
                else
                {
                    total_space -= - FILE_SIZE_IN_BYTE;
                    junk_size = FILE_SIZE_IN_BYTE;
                }
                //convert to KB
                junk_size /= 1024;
                //Console.WriteLine(junk_size);
                string cmd = drive.Name + "q_" + num + " " + junk_size;
                q_file_list.Add(drive.Name + "q_" + num);
                //ThreadPool.QueueUserWorkItem((x) =>new Program().OverwriteFile((string)x), cmd);
                TheCleanerProgress("Please wait, creation in progress " + drive.Name + "q_" + num);
                OverwriteFile(cmd);
                //Console.WriteLine(cmd + " remaining space logically " + total_space);

                num++;
            }

            TheCleanerCleaning("Removing junk...");
            
            q_file_list.ForEach(delegate (String filename)
            {
                //                ThreadPool.QueueUserWorkItem((x) => new Program().remove((string)x), filename);
                remove(filename);
            });
            TheCleanerFinished("Done, " + drive_letter + " successfully cleaned.");
        }

        void remove(String file)
        {
             if (File.Exists(file)) File.Delete(file);

            Console.WriteLine("removing file" + file);
        }

        public void OverwriteFile(string command)
        {
            ProcessRunner process = new ProcessRunner(@"C:\Users\Khalid Abdurrahman\Google Drive\TA1\Penting\softwares\rdfc\creatfil.exe");
            process.runWithArguments(command);
        }
    }
}
