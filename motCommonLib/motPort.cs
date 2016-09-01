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
using System.Net;
using NLog;

/// <summary>
/// Port is the bas class that talcs to the TCP/IP port that MOT uses.  The system will layer up from there
/// to encompass parsers to transform XML, JSON, MOT Markup and Delimited text to MOT Markup, and then up to 
/// collection methods including the pit, a monitored directory into which one throws files to import, a Web API,
/// a RESTful/JSON interface and anything else that's needed
/// </summary>

namespace motCommonLib
{
    public class motPort
    {
        public int TCP_TIMEOUT { get; set; } = 300000;

        public TcpClient tcpSocket = null;
        NetworkStream dataStream;
        private Logger logger;

        bool __open = false;

        public string tcp_address { get; set; }
        public int tcp_port { get; set; }


        private void __open_socket(string __address, int __port)
        {
            tcp_port = __port;

            IPAddress[] __host = Dns.GetHostAddresses(__address);

            foreach (IPAddress __h in __host)
            {
                if (__h.AddressFamily == AddressFamily.InterNetwork)
                {
                    tcp_address = __h.ToString();
                }
            }

            logger = LogManager.GetLogger("motInboundLib.Port");

            try
            {
                Open();
                __open = true;
                logger.Info(@"Successfully Opened {0}:{1}", __address, __port);
            }
            catch (SocketException e)
            {
                logger.Fatal(@"Failed to open socket: " + __address + " / " + __port + " " + e.Message);
                throw new Exception(e.Message);
            }
            catch (Exception e)
            {
                logger.Fatal(@"Failed to open socket: " + __address + " / " + __port + " " + e.Message);
                throw new Exception(@"Failed to open socket: " + __address + "/" + __port + " " + e.Message);
            }
        }

        public motPort()
        {
            logger = LogManager.GetLogger("motInboundLib.Port");
        }
        public motPort(string address, int port)
        {
            __open_socket(address, port);
        }
        public motPort(string address, string port)
        {
            __open_socket(address, tcp_port = Convert.ToInt32(port));       
        }
        ~motPort()
        {
            Close();
        }

        public void Open()
        {
            if(__open)
            {
                return;
            }

            try
            {
                tcpSocket = new TcpClient(tcp_address, tcp_port);
                dataStream = tcpSocket.GetStream();
                dataStream.ReadTimeout = TCP_TIMEOUT;
                dataStream.WriteTimeout = TCP_TIMEOUT;                        
            }
            catch (ArgumentNullException e)
            {
                string __error = string.Format(@"Gateway ArgumentNullException: {0}", e.Message);

                Console.WriteLine(__error);
                logger.Error(__error);

                throw new Exception(@"Invalid Argument");
            }
            catch (SocketException e)
            {
                Console.WriteLine(@"Gateway SocketException: {0}", e.Message);
                logger.Error(@"Gateway SocketException: {0}", e.Message);
                throw;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public void Open(string __address, int __port)
        {
            tcp_address = __address;
            tcp_port = __port;

            try
            {
                Open();
            }
            catch
            {
                throw;
            }
        }
        public void Open(string __address, string __port)
        {
            tcp_address = __address;
            tcp_port = Convert.ToInt32(__port);

            try
            {
                Open();
            }
            catch
            {
                throw;
            }
        }
        public void Close()
        {
            try
            {
                if (tcpSocket != null)
                {
                    __open = false;
                    dataStream.Close();
                    tcpSocket.Close();
                }
            }
            catch (Exception e)
            {
                string __error = string.Format(@"Error closing port [{0}/{1}] : {2}", this.tcp_address, this.tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Error(__error);
                throw new Exception(__error);
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
                string __error = string.Format(@"Error flushing port [{0}/{1}] : {2}", this.tcp_address, this.tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Error(__error);
                throw new Exception(__error);
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
                string __error = string.Format(@"Error writing to port [{0}/{1}] : {2}", this.tcp_address, this.tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Error(__error);
                throw new Exception(__error);
            }

            return false;
        }
        public bool Read(ref string __buf)
        {
            try
            {
                if (tcpSocket != null)
                {
                    byte[] __readbuf = new byte[256];
                    int __retval = 0;

                    __retval = dataStream.Read(__readbuf, 0, 256);
                    __buf = Encoding.UTF8.GetString(__readbuf);

                    return (__retval == 0);
                }
            }
            catch(Exception e)
            {
                string __error = string.Format(@"Error reading from port [{0}/{1}] : {2}", this.tcp_address, this.tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Error(__error);
                throw new Exception(__error);
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
                    string __error = string.Format(@"Error reading from port [{0}/{1}] : {2}", this.tcp_address, this.tcp_port, e.Message);
                    Console.WriteLine(__error);
                    logger.Error(__error);
                    throw new Exception(__error);
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
                    string __error = string.Format(@"Error resetting port [{0}/{1}] : {2}", this.tcp_address, this.tcp_port, e.Message);
                    Console.WriteLine(__error);
                    logger.Error(__error);
                    throw new Exception(__error);
                }

                return true;
            }

            return false;
        }
    }
}
