using System;
using System.Collections;
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

        private void watchDirectory(string dirName, string ip)
        {
            Port pt;
            try
            {
                 pt = new Port(ip, "24042");
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
                            Console.WriteLine($"Sending {__fileName}...");
                            Parser p = new Parser(pt, sr.ReadToEnd());
                            sr.Close();
                            File.Delete(__fileName);
                            Console.WriteLine($"{__fileName} sent.");

                            string result = pt.Read();
                            if (string.IsNullOrEmpty(result))
                            {
                                Console.WriteLine("No result received.");
                            }
                            else if (result.All(b => b < 30))
                            {
                                var bytes = result.Select(c => Convert.ToInt32(c));
                                Console.WriteLine($"Byte(s): {string.Join(",", bytes)}.");
                            }
                            else
                            {
                                Console.WriteLine($"String(s): {string.Join(Environment.NewLine, result.Split('\r'))}");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error: {e.Message}.");
                            sr.Close();
                            if (!File.Exists(__fileName))
                            {
                                continue;
                            }
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
            watchDirectory(@".\", "192.168.0.140");
        }

        public fileSystemWatcher(string dirName, string ip)
        {
            if (!System.IO.Directory.Exists(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }

            watchDirectory(dirName, ip);
        }
    }
}