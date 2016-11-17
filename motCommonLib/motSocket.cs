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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace motCommonLib
{
    public class motSocket
    {
        [ThreadStatic]
        private bool __running = false;

        private string __s_iobuffer { get; set; } = string.Empty;
        private byte[] __b_iobuffer { get; set; }

        public int __port;
        private string __internal_address = string.Empty;
        public string __address
        {
            get
            {
                return __internal_address;
            }

            set
            {
                IPAddress[] __host_list = Dns.GetHostAddresses(value);

                foreach (IPAddress __host in __host_list)
                {
                    if (__host.AddressFamily == AddressFamily.InterNetwork)
                    {
                        __internal_address = __host.ToString();
                    }
                }

                __open_for_listening = false;
            }
        }

        public int TCP_TIMEOUT { get; set; } = 300000;

        private TcpClient __client;
        private TcpListener __trigger;
        private NetworkStream __stream;
        private EndPoint __remoteEndPoint;
        private EndPoint __localEndPoint;

        private Logger __logger;

        //private Thread __working_thread;
        //private Thread __client_thread;

        public delegate void __void_string_delegate(string __data);
        public delegate void __void_byte_delegate(byte[] __data);

        public delegate bool __bool_string_delegate(string __data);
        public delegate bool __bool_byte_delegate(byte[] __data);

        public __void_string_delegate __s_callback { get; set; } = null;
        public __void_byte_delegate __b_stream_processor { get; set; } = null;
        public __bool_string_delegate __s_protocol_processor { get; set; } = null;
        public __bool_byte_delegate __b_protocol_processor { get; set; } = __default_protocol_processor;

        bool __open_for_listening = false;

        public motSocket()
        {
            __logger = LogManager.GetLogger("motCommonLib.Socket");
            __logger.Info("Constructed with no parameters");
        }
        /// <summary>
        /// motSocket for listening (localhost:port) with data handler
        /// </summary>
        /// <param name="__port">The TCP/IP port number to monitor</param>
        /// <param name="__func">The Function to start on the new thread</param>
        public motSocket(int __port, __void_string_delegate __s_callback = null)
        {
            __open_for_listening = true;
            __logger = LogManager.GetLogger("motCommonLib.Socket");

            try
            {
                this.__s_callback = __s_callback;
                this.__port = __port;

                open();

                __logger.Info("Listening on port {0}", __port);
                __running = true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect to remote socket " + e.Message);
            }
        }

        //
        // Create the socket listener class.  Needs to be started next, which happens at the first listen() call
        //
        public motSocket(int __port)
        {
            __logger = LogManager.GetLogger("motCommonLib.Socket");
            __open_for_listening = true;

            try
            {
                this.__port = __port;

                open();

                __logger.Info("Listening on port {0}", __port);
                __running = true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect to remote socket " + e.Message);
            }
        }



        /// <summary>
        /// motSocket for writing
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="__port"></param>
        /// <param name="__protocol_processor"></param>
        public motSocket(string __address, int __port, __bool_byte_delegate __byte_protocol_processor = null)
        {
            this.__port = __port;
            this.__address = __address;
            this.__b_protocol_processor = __byte_protocol_processor == null ? __default_protocol_processor : __byte_protocol_processor;
            this.__open_for_listening = false;

            __logger = LogManager.GetLogger("motCommonLib.Socket");

            try
            {
                open();
            }
            catch
            { throw; }
        }

        ~motSocket()
        {
        }

        public EndPoint remoteEndPoint
        {
            get
            {
                return __remoteEndPoint;
            }
        }

        public EndPoint localEndPoint
        {
            get
            {
                return __localEndPoint;
            }
        }

        public void listen()
        {
            int __counter = 0;

            __running = true;

            while (__running)
            {
                try
                {
                    using (__client = __trigger.AcceptTcpClient())
                    {                      
                        __stream = __client.GetStream();
                        __stream.ReadTimeout = 60;
                        __stream.WriteTimeout = 60;

                        __remoteEndPoint = __client.Client.RemoteEndPoint;
                        __localEndPoint = __client.Client.LocalEndPoint;
                        __logger.Info("Accepted connection from remote endpoint {0}", __remoteEndPoint.ToString());
                        
                        if (read() > 0)
                        {
                            __s_callback?.Invoke(__s_iobuffer);
                        }
                    }
                }
                catch (System.InvalidOperationException ex)
                {
                    // probably shutting the thread down
                    __running = false;
                    Console.WriteLine(ex.Message);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                __counter++;
            }

            return;
        }

        public int read()
        {
            int __inbytes = 0;
            int __total_bytes = 0;

            try
            {
                __b_iobuffer = new byte[1024];
                __s_iobuffer = "";

                while (__stream.DataAvailable)
                {
                    __inbytes = __stream.Read(__b_iobuffer, 0, __b_iobuffer.Length);

                    __b_stream_processor?.Invoke(__b_iobuffer);

                    __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer, 0, __inbytes);
                    __total_bytes += __inbytes;
                }
            }
            catch (Exception ex)
            {
                __logger.Error("read() failed: {0}", ex.Message);
                throw new Exception("read() failed: " + ex.Message);
            }

            return __total_bytes;
        }

        public int read(ref byte[] __b_buffer, int __index, int __count)
        {
            try
            {
                int __r_count = __stream.Read(__b_buffer, __index, __count);
                __b_stream_processor?.Invoke(__b_iobuffer);
                return __r_count;
            }
            catch
            {
                return 0;
            }
        }

        private static bool __default_protocol_processor(byte[] __buffer)
        {           
            return __buffer.Length > 0;
        }
        public void write_return(byte[] __data)
        {
            __stream.Write(__data, 0, __data.Length);
        }
        public void write_return(string __data)
        {
            __stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
        }
        public bool write(string __data)
        {
            try
            {
                byte[] __buffer = new byte[256];

                __stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);

                // Get a return value
                if (__stream.Read(__buffer, 0, __buffer.Length) > 0)
                {
                    return (bool)__b_protocol_processor?.Invoke(__buffer);
                }

                return true;
            }
            catch (Exception ex)
            {
                __logger.Error("write() failed: {0}", ex.Message);
                throw new Exception("write() failed: " + ex.Message);
            }
        }

        public bool write(byte[] __data)
        {
            try
            {
                byte[] __buffer = new byte[256];

                __stream.Write(__data, 0, __data.Length);

                // Get a return value
                __stream.Read(__buffer, 0, __buffer.Length);
                return (bool)__b_protocol_processor?.Invoke(__buffer);
            }
            catch (Exception ex)
            {
                __logger.Error("write() failed: {0}", ex.Message);
                throw new Exception("write() failed: " + ex.Message);
            }
        }

        public bool send(byte[] __data)
        {
            return write(__data);
        }

        public void flush()
        {

            try
            {
                __stream.Flush();
            }
            catch (Exception e)
            {
                throw new Exception("Socket flush failure " + e.Message);
            }

        }

        public void close()
        {
            if (__running)
            {
                try
                {
                    __running = false;

                    if (__open_for_listening)
                    {
                        __trigger.Stop();
                    }
                    if (__client != null)
                    {
                        __client.Close();
                    }
                    if (__stream != null)
                    {
                        __stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Socket close failure " + ex.Message);
                }
            }
        }

        public void open()
        {
            try
            {
                if (__open_for_listening)
                {
                    __trigger = new TcpListener(IPAddress.Any, __port);
                    __trigger.Start();
                }
                else
                {
                    __client = new TcpClient(this.__address, __port);
                    __stream = __client.GetStream();

                    __stream.ReadTimeout = TCP_TIMEOUT;
                    __stream.WriteTimeout = TCP_TIMEOUT;

                    __remoteEndPoint = __client.Client.RemoteEndPoint;
                    __localEndPoint = __client.Client.LocalEndPoint;
                }

                __running = true;
            }
            catch (SocketException ex)
            {
                __logger.Fatal(@"Failed to open socket: " + __address + " / " + __port + " " + ex.Message);
                throw new Exception(@"Failed to open socket: " + __address + " / " + __port + ex.Message);
            }
            catch (Exception ex)
            {
                __logger.Fatal(@"Failed to open socket: " + __address + " / " + __port + " " + ex.Message);
                throw new Exception(@"Failed to open socket: " + __address + "/" + __port + " " + ex.Message);
            }

            __running = true;
            __logger.Info(@"Successfully Opened {0}:{1} for {2}", __address, __port, __open_for_listening ? "Listening" : "Writing");
        }
    }
}
