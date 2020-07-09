using System;
using System.IO;
using System.Text;

namespace GloDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            bool extractData = false;
            Console.WriteLine(@"Blast Thru data file decrypter and extracter - version 1.0");
            Console.Write("Arguments: ");
            for (int i = 0; i < args.Length; i++)
            {
                Console.Write(args[i]);
                Console.Write(" ");
            }
            Console.WriteLine("");
            if (args.Length < 2)
            {
                Console.WriteLine("Arguments must be at least 2 parameters");
                return;
            }
            
            if (args.Length >= 3)
            {
                if (args[2] == "extract")
                {
                    extractData = true;
                    if (args.Length == 4)
                    {
                        if (!Directory.Exists(args[3]))
                        {
                            Directory.CreateDirectory(args[3]);
                        }
                        Directory.SetCurrentDirectory(args[3]);
                    }
                }
            }
            FileStream fstrm = new FileStream(args[0],FileMode.Open,FileAccess.Read);
            FileStream fstrm2 = new FileStream(args[1], FileMode.Create, FileAccess.ReadWrite);
            if (fstrm != null && fstrm2 != null)
            {
                int num = (int)fstrm.Length;
                byte[] array = new byte[fstrm.Length];
                byte[] array2 = new byte[fstrm.Length];
                fstrm.Read(array,0, (int)fstrm.Length);
                for (int i = 0; i < fstrm.Length; i++)
                {
                    array2[i] = (byte)(array[i] ^ 0xFF);
                }
                fstrm2.Write(array2, 0, (int)fstrm.Length);
            }
            fstrm2.Dispose();
            fstrm2.Close();
            if (extractData)
            {
                fstrm2 = new FileStream(args[1], FileMode.Open, FileAccess.Read);
                var binreader = new BinaryReader(fstrm2);
                int numOfFiles = binreader.ReadInt32();
                ASCIIEncoding encoding = new ASCIIEncoding();
                for (int i = 0; i < numOfFiles; i++)
                {
                    string filenamestr = encoding.GetString(binreader.ReadBytes(binreader.ReadInt32()));
                    Console.WriteLine(string.Format("Filename: {0}", filenamestr));
                    int filenameOffset = binreader.ReadInt32();
                    int fileSize = binreader.ReadInt32();
                    byte[] fileDataArray = new byte[fileSize];
                    var curFilePos = fstrm2.Position;
                    fstrm2.Seek(filenameOffset,SeekOrigin.Begin);
                    fstrm2.Read(fileDataArray, 0, fileSize);
                    fstrm2.Seek(curFilePos, SeekOrigin.Begin);
                    Console.WriteLine("Extracting file: {0}", filenamestr);
                    filenamestr.Replace('\\', '/');
                    if (filenamestr.Contains('/'))
                    {
                        if (!Directory.Exists(filenamestr))
                        {
                            Directory.CreateDirectory(filenamestr);
                        }
                    }
                    var strmWR = new BinaryWriter(File.Create(filenamestr));
                    strmWR.Write(fileDataArray,0,fileSize);
                    strmWR.Dispose();
                }
            }
        }
    }
}
