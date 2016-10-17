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
/// Port is the bas class that talks to the TCP/IP port that the MOT gateway uses. The system will layer up from there
/// to encompass parsers to transform XML, JSON, MOT Markup and Delimited text to MOT Markup, and then up to 
/// collection methods including the pit, a monitored directory into which one throws files to import, a Web API,
/// a RESTful/JSON interface and anything else that's needed
/// </summary>

namespace motCommonLib
{
    public class motPort : IDisposable
    {
        public int TCP_TIMEOUT { get; set; } = 300000;

        public TcpClient __tcp_socket = null;
        NetworkStream __data_stream;

        private Logger logger;
        public LogLevel __log_level { get; set; } = LogLevel.Error;

        bool __open = false;

        public string __tcp_address { get; set; }
        public int __tcp_port { get; set; }


        private void __open_socket(string __address, int __port)
        {
            __tcp_port = __port;

            IPAddress[] __host = Dns.GetHostAddresses(__address);

            foreach (IPAddress __h in __host)
            {
                if (__h.AddressFamily == AddressFamily.InterNetwork)
                {
                    __tcp_address = __h.ToString();
                }
            }

            logger = LogManager.GetLogger("motInboundLib.Port");

            try
            {
                Open();
                __open = true;
                logger.Log(__log_level, @"Successfully Opened {0}:{1}", __address, __port);
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
        public motPort(string address, int port, bool __stay_open = true)
        {
            __open_socket(address, port);

            // Just tested to see if we could get it,  now free it up and open it when we need it
            if (!__stay_open)
            {
                this.Close();
            }
        }
        public motPort(string address, string port, bool __stay_open = true)
        {
            __open_socket(address, __tcp_port = Convert.ToInt32(port));

            // Just tested to see if we could get it,  now free it up and open it when we need it
            if(!__stay_open)
            {
                this.Close();
            }
        }
        ~motPort()
        {
            Close();
            Dispose();
        }

        public void Open()
        {
            if (__open)
            {
                return;
            }

            try
            {
                __tcp_socket = new TcpClient(__tcp_address, __tcp_port);
                __data_stream = __tcp_socket.GetStream();
                __data_stream.ReadTimeout = TCP_TIMEOUT;
                __data_stream.WriteTimeout = TCP_TIMEOUT;
            }
            catch (ArgumentNullException e)
            {
                string __error = string.Format(@"Gateway ArgumentNullException: {0}", e.Message);

                Console.WriteLine(__error);
                logger.Log(__log_level, __error);

                throw new Exception(@"Invalid Argument");
            }
            catch (SocketException e)
            {
                Console.WriteLine(@"Gateway SocketException: {0}", e.Message);
                logger.Log(__log_level, @"Gateway SocketException: {0}", e.Message);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Open(string __address, int __port)
        {
            __tcp_address = __address;
            __tcp_port = __port;

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
            __tcp_address = __address;
            __tcp_port = Convert.ToInt32(__port);

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
                if (__open && __tcp_socket != null)
                {
                    __open = false;
                    __data_stream.Close();
                    __tcp_socket.Close();
                }
            }
            catch (Exception e)
            {
                string __error = string.Format(@"Error closing port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Log(__log_level, __error);
                throw new Exception(__error);
            }
        }
        public void Flush()
        {
            try
            {
                if (__open && __tcp_socket != null)
                {
                    __data_stream.Flush();
                }
            }
            catch (Exception e)
            {
                string __error = string.Format(@"Error flushing port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Log(__log_level, __error);
                throw new Exception(__error);
            }
        }

        public bool ProcessRetVal()
        {
            byte[] __retval = new byte[64];

            int __retlen = __data_stream.Read(__retval, 0, __retval.Length);

            if (__retval[0] == '\x06')   // MOT Gateway ACK
            {
                return true;
            }

            string __error = string.Format(@"Error writing to port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, __retval);
            Console.WriteLine(__error);
            logger.Log(__log_level, __error);

            return false;
        }

        public bool Write(string __buf, int __len)
        {
            try
            {
                if (__open && __tcp_socket != null)
                {
                    __data_stream.Write(Encoding.ASCII.GetBytes(__buf), 0, __len);
                    return ProcessRetVal();
                }
            }
            catch (Exception e)
            {
                string __error = string.Format(@"Error writing to port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Log(__log_level, __error);
                throw new Exception(__error);
            }

            return false;
        }
        public bool Read(ref string __buf)
        {
            try
            {
                if (__open && __tcp_socket != null)
                {
                    byte[] __readbuf = new byte[1024];
                    int __retval = 0;

                    while (__data_stream.DataAvailable)
                    {
                        __retval = __data_stream.Read(__readbuf, 0, __readbuf.Length);
                        __buf += Encoding.UTF8.GetString(__readbuf, 0, __retval);
                    }

                    return (__retval == 0);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format(@"Error reading from port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, e.Message);
                Console.WriteLine(__error);
                logger.Log(__log_level, __error);
                throw new Exception(__error);
            }

            return false;
        }
        public string Read()
        {
            if (__open && __tcp_socket != null)
            {
                try
                {
                    byte[] __readbuf = new byte[1024];
                    int __retval = 0;
                    string __data = string.Empty;

                    while (__data_stream.DataAvailable)
                    {
                        __retval = __data_stream.Read(__readbuf, 0, __readbuf.Length);
                        __data += Encoding.UTF8.GetString(__readbuf, 0, __retval);
                    }
                }
                catch (Exception e)
                {
                    string __error = string.Format(@"Error reading from port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, e.Message);
                    Console.WriteLine(__error);
                    logger.Log(__log_level, __error);
                    throw new Exception(__error);
                }
            }

            return null;
        }
        public bool Reset()
        {
            if (__open && __tcp_socket != null)
            {
                try
                {
                    Close();
                    Open();
                }
                catch (Exception e)
                {
                    string __error = string.Format(@"Error resetting port [{0}/{1}] : {2}", this.__tcp_address, this.__tcp_port, e.Message);
                    Console.WriteLine(__error);
                    logger.Log(__log_level, __error);
                    throw new Exception(__error);
                }

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            /*
            ((IDisposable)__data_stream).Dispose();
            ((IDisposable)__tcp_socket).Dispose();
            */
        }
    }
}
