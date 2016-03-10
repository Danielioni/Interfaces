using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace motInboundLib
{
    public class fileSystemWatcher : inputMethod
    {
        public void writeData()
        {
        }

        private void watchDirectory(string dirName)
        {
            Port pt;
            try
            {
                 pt = new Port("192.168.0.140", "24042");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            while (true)
            {
                Thread.Sleep(1024);

                if (Directory.GetFiles(dirName) != null)
                {
                    string[] __fileEntries = Directory.GetFiles(dirName);
                    StreamReader sr = null;

                    foreach (string __fileName in __fileEntries)
                    {
                        if (__fileName.Contains(".FAILED"))
                            continue;
                        try
                        {
                            
                            sr = new StreamReader(__fileName);
                            Parser p = new Parser(pt, sr.ReadToEnd());
                            sr.Close();
                            File.Delete(__fileName);
                        }
                        catch(Exception e)
                        {
                            sr.Close();
                            if (!File.Exists(__fileName + ".FAILED"))
                            {
                                File.Move(__fileName, __fileName + ".FAILED");
                            }
                            else
                            {
                                File.Delete(__fileName);
                            }
                        }
                    }
                }
            }
        }

        public fileSystemWatcher()
        {
            watchDirectory(@".\");
        }

        public fileSystemWatcher(string dirName)
        {
            if (!System.IO.Directory.Exists(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }

            watchDirectory(dirName);
        }
    }
}