using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
namespace FlashShred
{
    class Verifier
    {
        private byte[] memorybyte;
        private byte[] filebyte;
        public Verifier(byte[] memory, byte[] file) {
            this.memorybyte = memory;
            this.filebyte = file;
        }
        private  string GetHash(HashAlgorithm hashAlgorithm, byte[] input = null, Stream fileStream = null)
        {
            byte[] data = null;
            // Convert the input string to a byte array and compute the hash.
            if (input != null)
            {
                data = hashAlgorithm.ComputeHash(input);
            }
            if (fileStream != null)
            {
                fileStream.Position = 0;
                data = hashAlgorithm.ComputeHash(fileStream);
            }

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            if (fileStream != null)
            {
                fileStream.Close();
            }
            return sBuilder.ToString();
        }
        public bool VerifyHash()
        {
            SHA256 sha256 = SHA256.Create();
            string m = GetHash(sha256, memorybyte);
            //string f = GetHash(sha256, null,new FileStream(file, FileMode.Open));
            string f = GetHash(sha256, filebyte);
            Console.WriteLine("====== file " + f);
            Console.WriteLine("====== mem " + m);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (comparer.Compare(m,f) != 0)
            {
                return true;
            }
            return false;
           // return comparer.Compare(m, f) == 0;

        }
    }

    
}
