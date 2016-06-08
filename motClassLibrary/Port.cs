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
using System.Text;
using System.Net.Sockets;
using NLog;

/// <summary>
/// Port is the bas class that talcs to the TCP/IP port that MOT uses.  The system will layer up from there
/// to encompass parsers to transform XML, JSON, MOT Markup and Delimited text to MOT Markup, and then up to 
/// collection methods including the pit, a monitored directory into which one throws files to import, a Web API,
/// a RESTful/JSON interface and anything else that's needed
/// </summary>

namespace motInboundLib
{
    public class Port
    {
        public int TCP_TIMEOUT { get; set; } = 300000;

        public TcpClient tcpSocket = null;
        NetworkStream dataStream;
        private Logger logger;

        bool __open = false;

        public string tcp_address { get; set; }
        public string tcp_port { get; set; }

        public Port()
        {
            logger = LogManager.GetLogger("motInboundLib.Port");
        }

        public Port(string address, string port)
        {
            if(__open)
            {
                return;
            }

            logger = LogManager.GetLogger("motInboundLib.Port");

            tcp_address = address;
            tcp_port = port;

            try
            {
                Open();
                __open = true;
                logger.Info(@"Successfully Opened {0}:{1}", address, port);
            }
            catch (SocketException e)
            {
                logger.Fatal(@"Failed to open socket: " + address + " / " + port + " " + e.Message);
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                logger.Fatal(@"Failed to open socket: " + address + " / " + port + " " + e.Message);
                throw new Exception(@"Failed to open socket: " + address + "/" + port + " " + e.Message);
            }
        }

        ~Port()
        {
            Close();
        }

        public void Open()
        {
            try
            {
                tcpSocket = new TcpClient(tcp_address, Convert.ToInt32(tcp_port));
                dataStream = tcpSocket.GetStream();
                dataStream.ReadTimeout = TCP_TIMEOUT;
                dataStream.WriteTimeout = TCP_TIMEOUT;                        
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Gateway ArgumentNullException: {0}", e);
                throw new Exception("Invalid Argument");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Gateway SocketException: {0}", e);
                throw;
            }
        }

        public void Close()
        {
            if (tcpSocket != null)
            {
                __open = false;
                dataStream.Close();
                tcpSocket.Close();
            }
        }

        public void Flush()
        {
            try
            {
                if (tcpSocket != null)
                {
                    dataStream.Flush();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Port flush failure " + e.Message);
            }
        }



        public bool Write(string __buf, int __len)
        {
            try
            {
                if (tcpSocket != null)
                {
                    dataStream.Write(Encoding.ASCII.GetBytes(__buf), 0, __len);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to port: {0}", e.Message);
            }

            return false;
        }

        public bool Read(ref string __buf)
        {
            if (tcpSocket != null)
            {
                byte[] __readbuf = new byte[256];
                int __retval = 0;

                __retval = dataStream.Read(__readbuf, 0, 256);
                __buf = Encoding.UTF8.GetString(__readbuf);

                return (__retval == 0);
            }

            return false;
        }

        public string Read()
        {
            if (tcpSocket != null)
            {
                try
                {
                    byte[] __readbuf = new byte[4096];

                    int __retval = dataStream.Read(__readbuf, 0, __readbuf.Length);
                    return Encoding.UTF8.GetString(__readbuf);
                }
                catch (Exception e)
                {        
                }
            }

            return null;
        }

        public bool Reset()
        {
            if (tcpSocket != null)
            {
                try
                {
                    Close();
                    Open();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
