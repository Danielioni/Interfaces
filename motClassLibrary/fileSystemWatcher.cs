// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney and Medicine-On-Time, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

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
                throw e;
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
                throw e;
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
                throw e;
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