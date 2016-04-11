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
        Port pt;

        public void writeData()
        {
        }

        private void openPort(string address, string port)
        {
            try
            {
                pt = new Port(address, port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void watchDirectory(string dirName, string address, string port)
        {
            try
            {
                openPort(address, port);
            }
            catch (Exception e)
            {
                throw;
            }

            watchDirectory(dirName);
        }

        private void watchDirectory(string dirName)
        {

            try
            {
                openPort("127.0.0.1", "24042");
            }
            catch (Exception e)
            {
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
            watchDirectory(System.IO.Directory.GetCurrentDirectory());
        }

        public fileSystemWatcher(string dirName)
        {
            if (!System.IO.Directory.Exists(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }

            watchDirectory(dirName);
        }

        public fileSystemWatcher(string dirName, string address, string port)
        {
            if (!System.IO.Directory.Exists(dirName))
            {
                System.IO.Directory.CreateDirectory(dirName);
            }

            watchDirectory(dirName, address, port);
        }
    }
}