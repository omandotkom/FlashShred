using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FlashShred
{
    class Util
    {
        private static List<string> log = new List<string>();
        public static void write(string message) {
            //Console.WriteLine(time + " - " + message);
            log.Add(DateTime.Now.ToString("h:mm:ss") + " - " + message);
        }
        public static void export() {
            string to = Algorithm.getCurrentDirectory();
            File.WriteAllLines(to + @"\"+DateTime.Now.ToString("hmmss")+"-log.txt", log.ToArray());
        }
    }
}
