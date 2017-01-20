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
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
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
        public bool __use_ssl { get; set; } = false;
        public int __port { get; set; } = 0;

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

        public int TCP_TIMEOUT { get; set; } = 5000;

        private TcpClient __client;
        private TcpListener __trigger;

        private EndPoint __remoteEndPoint;
        private EndPoint __localEndPoint;

        private Logger __logger;

        public delegate void __void_string_delegate(string __data);
        public delegate void __void_byte_delegate(byte[] __data);

        public delegate bool __bool_string_delegate(string __data);
        public delegate bool __bool_byte_delegate(byte[] __data);

        public __void_string_delegate __s_callback { get; set; } = null;
        public __void_byte_delegate __b_stream_processor { get; set; } = null;
        public __bool_string_delegate __s_protocol_processor { get; set; } = null;
        public __bool_byte_delegate __b_protocol_processor { get; set; } = __default_protocol_processor;

        bool __open_for_listening = false;

        private NetworkStream __stream;
        private SslStream __ssl_stream;

        public static Mutex __socket_mutex = null;

        public motSocket()
        {
            __logger = LogManager.GetLogger("motCommonLib.Socket");
            __logger.Info("Constructed with no parameters");

            if (__socket_mutex == null)
            {
                __socket_mutex = new Mutex();
            }
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

            if (__socket_mutex == null)
            {
                __socket_mutex = new Mutex();
            }

            try
            {
                this.__s_callback = __s_callback;
                this.__port = __port;

                open_as_server();

                __logger.Info("Listening on port {0}", __port);
                __running = true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect to remote socket " + e.Message);
            }
        }
        public motSocket(int __port)
        {
            __logger = LogManager.GetLogger("motCommonLib.Socket");
            __open_for_listening = true;

            if (__socket_mutex == null)
            {
                __socket_mutex = new Mutex();
            }

            try
            {
                this.__port = __port;

                open_as_server();

                __logger.Info("Listening on port {0}", __port);
                __running = true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect to remote socket " + e.Message);
            }
        }
        public static bool __validate_server_certificate(object __sender, X509Certificate __server_cert, X509Chain __cert_chain, SslPolicyErrors __ssl_policy_errors)
        {
            if (__ssl_policy_errors == SslPolicyErrors.None)
            {
                return true;
            }

            //__logger.Error("Server certificate validation errors: {0}", __ssl_policy_errors);

            return false;   // burn the connection
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

            if (__socket_mutex == null)
            {
                __socket_mutex = new Mutex();
            }

            __logger = LogManager.GetLogger("motCommonLib.Socket");

            try
            {
                open_as_client();
            }
            catch
            {
                throw;
            }
        }
        public motSocket(string __address, int __port, bool __secure_connection, __bool_byte_delegate __byte_protocol_processor = null)
        {
            this.__port = __port;
            this.__address = __address;
            this.__b_protocol_processor = __byte_protocol_processor == null ? __default_protocol_processor : __byte_protocol_processor;
            this.__use_ssl = __secure_connection;
            this.__open_for_listening = false;

            __logger = LogManager.GetLogger("motCommonLib.Socket");

            if (__socket_mutex == null)
            {
                __socket_mutex = new Mutex();
            }

            try
            {
                open_as_client();
            }
            catch
            {
                throw;
            }
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

            set
            {
                this.remoteEndPoint = value;
            }
        }
        public EndPoint localEndPoint
        {
            get
            {
                return __localEndPoint;
            }
        }
        public void async_handler(IAsyncResult __ar)
        {            
            Thread.CurrentThread.Name = "*** IMPATIENT FRED ***";
            //Console.WriteLine("async_handler on Thread: {0}", Thread.CurrentThread.Name);

            try
            {
                TcpListener __l = (TcpListener)__ar.AsyncState;

                var __client = __l.EndAcceptTcpClient(__ar);
                var __lstream = __client.GetStream();

                __logger.Debug("Received connection from {0}", __client.Client.RemoteEndPoint);
                __remoteEndPoint = __client.Client.RemoteEndPoint;
                __localEndPoint = __client.Client.LocalEndPoint;

                int __inbytes = 0;
                int __total_bytes = 0;
                int __count = 0;
                
                try
                {
                    //Thread.Sleep(1);

                    __b_iobuffer = new byte[1024];
                    __s_iobuffer = "";

                    while (!__lstream.DataAvailable)
                    {
                        if(__count > 99)
                        {
                            throw new Exception("No data in stream ...");
                        }

                        //Console.WriteLine("Waiting for data ... {0}", __count++);
                        Thread.Sleep(1);
                    }

                    while (__lstream.DataAvailable)
                    {
                        __inbytes = __lstream.Read(__b_iobuffer, 0, __b_iobuffer.Length);
                        __b_stream_processor?.Invoke(__b_iobuffer);
                        __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer, 0, __inbytes);
                        __total_bytes += __inbytes;
                    }
                }
                catch (IOException)
                {
                    // timeout
                }
                catch (Exception ex)
                {
                    __logger.Error("async read() failed: {0}", ex.Message);
                    throw new Exception("async read() failed: " + ex.Message);
                }

                if (__total_bytes > 0)
                {
                    // Assign the local stream to the global and process
                    __stream = __lstream;

                    Task t = Task.Run(() =>
                    {
                        Thread.CurrentThread.Name = "*** SON OF IMPATIENT FRED ***";
                        __s_callback?.Invoke(__s_iobuffer);
                        //Console.WriteLine("Done with asyc_handler Task");
                    });              
                }

                //Console.WriteLine("Triggering mutex");
                tcpClientConnected.Set();
            }
            catch
            {
                tcpClientConnected.Set();
                return;
            }
        }

        private X509Certificate __x_509_cert;

        public void secure_async_handler(IAsyncResult __ar)
        {
            try
            {
                TcpListener __l = (TcpListener)__ar.AsyncState;

                var __client = __l.EndAcceptTcpClient(__ar);
                var __lstream = new SslStream(__client.GetStream(), false);
                __ssl_stream.AuthenticateAsServer(__x_509_cert, false, SslProtocols.Tls, true);

                int __inbytes = 0;
                int __total_bytes = 0;

                try
                {
                    Thread.Sleep(1);

                    __b_iobuffer = new byte[1024];
                    __s_iobuffer = "";

                    do
                    {
                        __inbytes = __ssl_stream.Read(__b_iobuffer, 0, __b_iobuffer.Length);
                        __b_stream_processor?.Invoke(__b_iobuffer);
                        __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer, 0, __inbytes);
                        __total_bytes += __inbytes;

                    }
                    while (__inbytes > 0);
                }
                catch (IOException)
                {
                    // timeout
                }
                catch (Exception ex)
                {
                    __logger.Error("read() failed: {0}", ex.Message);
                    throw new Exception("read() failed: " + ex.Message);
                }

                // Assuming we can stop blocking the port, probably wrong thinking
                tcpClientConnected.Set();

                if (__total_bytes > 0)
                {
                    __ssl_stream = __lstream;
                    __s_callback?.Invoke(__s_iobuffer);
                }

                __lstream.Close();
                __client.Close();
            }
            catch
            {
                tcpClientConnected.Set();
                return;
            }
        }
        public ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public void listen_async()
        {
            while (__running)
            {
                tcpClientConnected.Reset();
                __trigger.BeginAcceptTcpClient(new AsyncCallback(async_handler), __trigger);

                //Console.WriteLine("BeginAcceptTcpClient");

                tcpClientConnected.WaitOne();
            }
        }

        public void secure_listen_async(X509Certificate __x_509_cert)
        {
            if (__x_509_cert == null)
            {
                throw new ArgumentNullException("Missing X509 Certificate");
            }

            this.__x_509_cert = __x_509_cert;

            while(__running)
            {
                tcpClientConnected.Reset();
                __trigger.BeginAcceptTcpClient(new AsyncCallback(secure_async_handler), __trigger);
                tcpClientConnected.WaitOne();
            }
        }
        public void secure_listen(X509Certificate __x_509_cert)
        {
            if (__x_509_cert == null)
            {
                throw new ArgumentNullException("Missing X509 Certificate");
            }

            __running = true;

            while (__running)
            {
                try
                {
                    using (__client = __trigger.AcceptTcpClient())
                    {
                        __ssl_stream = new SslStream(__client.GetStream(), false);
                        __ssl_stream.ReadTimeout = TCP_TIMEOUT;
                        __ssl_stream.WriteTimeout = TCP_TIMEOUT;

                        __remoteEndPoint = __client.Client.RemoteEndPoint;
                        __localEndPoint = __client.Client.LocalEndPoint;

                        __ssl_stream.AuthenticateAsServer(__x_509_cert, false, SslProtocols.Tls, true);

                        __logger.Info("Accepted and Authenticated TLS connection from remote endpoint {0}", __remoteEndPoint.ToString());


                        if (read() > 0)
                        {
                            __s_callback?.Invoke(__s_iobuffer);
                        }

                        __ssl_stream.Close();
                        __client.Close();
                    }
                }
                catch (IOException ex)
                {
                    __logger.Error("Create secure stream I/O error: " + ex.StackTrace);
                }
                catch (AuthenticationException ex)
                {
                    __logger.Error("Create secure stream Authentication error: " + ex.StackTrace);
                }
                catch (InvalidOperationException ex)
                {
                    // probably shutting the thread down
                    __running = false;
                    Console.WriteLine(ex.Message);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void listen()
        {
            //int __counter = 0;

            __running = true;

            while (__running)
            {
                try
                {
                    using (var __client = __trigger.AcceptTcpClient())
                    {
                        using (__stream = __client.GetStream())
                        {
                            __stream.ReadTimeout = TCP_TIMEOUT;
                            __stream.WriteTimeout = TCP_TIMEOUT;

                            __remoteEndPoint = __client.Client.RemoteEndPoint;
                            __localEndPoint = __client.Client.LocalEndPoint;

                            __logger.Debug("Accepted connection from remote endpoint {0}", __remoteEndPoint.ToString());

                            //__stream = __new_stream;

                            if (read() > 0)
                            {
                                __s_callback?.Invoke(__s_iobuffer);
                            }

                            __stream.Close();
                            __client.Close();
                        }
                    }
                }
                catch (IOException ex)
                {
                    // maybe a timeout
                }
                catch (InvalidOperationException ex)
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

                //__counter++;
            }

            return;
        }
        public int read()
        {
            int __inbytes = 0;
            int __total_bytes = 0;

            try
            {
                Thread.Sleep(1);

                __b_iobuffer = new byte[1024];
                __s_iobuffer = "";

                if (__use_ssl)
                {
                    do
                    {
                        __inbytes = __ssl_stream.Read(__b_iobuffer, 0, __b_iobuffer.Length);

                        __b_stream_processor?.Invoke(__b_iobuffer);

                        __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer, 0, __inbytes);
                        __total_bytes += __inbytes;

                    }
                    while (__inbytes > 0);
                }
                else
                {
                    while (__stream.DataAvailable)
                    {
                        __inbytes = __stream.Read(__b_iobuffer, 0, __b_iobuffer.Length);

                        __b_stream_processor?.Invoke(__b_iobuffer);

                        __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer, 0, __inbytes);
                        __total_bytes += __inbytes;

                        //Thread.Sleep(1);
                    }
                }
            }
            catch (IOException)
            {
                // timeout
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
                int __r_count = 0;

                if (__use_ssl)
                {
                    __r_count = __ssl_stream.Read(__b_buffer, __index, __count);
                    __b_stream_processor?.Invoke(__b_iobuffer);
                }
                else
                {
                    __r_count = __stream.Read(__b_buffer, __index, __count);
                    __b_stream_processor?.Invoke(__b_iobuffer);
                }

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
            __socket_mutex.WaitOne();

            try
            {
                if (__use_ssl)
                {
                    __ssl_stream.Write(__data, 0, __data.Length);
                }
                else
                {
                    __stream.Write(__data, 0, __data.Length);
                }
            }
            catch
            { }

            __socket_mutex.ReleaseMutex();
        }
        public void write_return(string __data)
        {
            __socket_mutex.WaitOne();

            try
            {
                if (__use_ssl)
                {
                    __ssl_stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                }
                else
                {
                    __stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                }
            }
            catch { }

            __socket_mutex.ReleaseMutex();
        }
        public bool write(string __data)
        {
            __socket_mutex.WaitOne();

            try
            {
                byte[] __buffer = new byte[256];

                if (__use_ssl)
                {
                    __ssl_stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                    __ssl_stream.Read(__buffer, 0, __buffer.Length);
                }
                else
                {
                    __stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                    __stream.Read(__buffer, 0, __buffer.Length);
                }

                __socket_mutex.ReleaseMutex();

                return (bool)__b_protocol_processor?.Invoke(__buffer);
            }
            catch (IOException ex)
            {
                __socket_mutex.ReleaseMutex();

                __logger.Error("write() failed: {0}", ex.Message);
                throw new Exception("write() failed: " + ex.Message);

                // timeout
                //return false;
            }
            catch (Exception ex)
            {
                __socket_mutex.ReleaseMutex();

                __logger.Error("write() failed: {0}", ex.Message);
                throw new Exception("write() failed: " + ex.Message);
            }
        }
        public bool write(byte[] __data)
        {
            __socket_mutex.WaitOne();

            try
            {
                byte[] __buffer = new byte[256];

                if (__use_ssl)
                {
                    __ssl_stream.Write(__data, 0, __data.Length);
                    __ssl_stream.Read(__buffer, 0, __buffer.Length);
                }
                else
                {
                    __stream.Write(__data, 0, __data.Length);
                    __stream.Read(__buffer, 0, __buffer.Length);
                }

                __socket_mutex.ReleaseMutex();

                return (bool)__b_protocol_processor?.Invoke(__buffer);
            }
            catch (IOException)
            {
                __socket_mutex.ReleaseMutex();

                // timeout
                return false;
            }
            catch (Exception ex)
            {
                __socket_mutex.ReleaseMutex();

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
                if (__use_ssl)
                {
                    __ssl_stream.Flush();
                }
                else
                {
                    __stream.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Stream flush failure " + ex.Message);
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
        public void open_as_server()
        {
            try
            {
                __trigger = new TcpListener(IPAddress.Any, __port);
                __trigger.Start();

                __open_for_listening = true;
                __running = true;
            }
            catch (Exception ex)
            {
                __logger.Error("Failed to start server on: {0} : {1}", __port, ex.Message);
                throw;
            }
        }
        public void open_as_client()
        {
            try
            {
                __client = new TcpClient(this.__address, __port);

                if (__use_ssl)
                {
                    // Resolve the machine name for the certificate
                    var __hostname = Dns.GetHostEntry(__address).HostName;
                    if (string.IsNullOrEmpty(__hostname))
                    {
                        __hostname = __address;
                    }

                    __ssl_stream = new SslStream(__client.GetStream(),
                                                    false,
                                                    new RemoteCertificateValidationCallback(__validate_server_certificate),
                                                    null
                                                 );

                    try
                    {
                        __ssl_stream.AuthenticateAsClient(__hostname);
                    }
                    catch (AuthenticationException ex)
                    {
                        __logger.Error("[Authentication] Failed to connect securely to {0}:{1}. Error: {2}", __address, __port, ex.StackTrace);
                        __client.Close();
                        throw ex;
                    }
                    catch (IOException ex)
                    {
                        __logger.Error("[SystemIO] Failed to connect securely to {0}:{1}. Error: {2}", __address, __port, ex.StackTrace);
                        __client.Close();
                        throw;
                    }

                    __ssl_stream.ReadTimeout = TCP_TIMEOUT;
                    __ssl_stream.WriteTimeout = TCP_TIMEOUT;
                }
                else
                {
                    __stream = __client.GetStream();
                    __stream.ReadTimeout = TCP_TIMEOUT;
                    __stream.WriteTimeout = TCP_TIMEOUT;
                }

                __remoteEndPoint = __client.Client.RemoteEndPoint;
                __localEndPoint = __client.Client.LocalEndPoint;

                __open_for_listening = false;
            }
            catch
            {
                throw;
            }

        }
        /*    public void open(X509Certificate2 __x_509_cert = null)
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

                        if (__use_ssl)
                        {
                            __ssl_stream = new SslStream(__client.GetStream(), false);
                            __ssl_stream.ReadTimeout = TCP_TIMEOUT;
                            __ssl_stream.WriteTimeout = TCP_TIMEOUT;

                            __remoteEndPoint = __client.Client.RemoteEndPoint;
                            __localEndPoint = __client.Client.LocalEndPoint;

                            __ssl_stream.AuthenticateAsServer(__x_509_cert, false, SslProtocols.Tls, true);
                        }
                        else
                        {
                            __stream = __client.GetStream();
                        }

                        __stream.ReadTimeout = TCP_TIMEOUT;
                        __stream.WriteTimeout = TCP_TIMEOUT;

                        __remoteEndPoint = __client.Client.RemoteEndPoint;
                        __localEndPoint = __client.Client.LocalEndPoint;
                    }

                    __running = true;
                }
                catch (AuthenticationException ex)
                {
                    __logger.Fatal(@"Failed to authenticate socket: " + __address + " / " + __port + " " + ex.Message);
                    throw new Exception(@"Failed to authenticate socket: " + __address + " / " + __port + ex.Message);
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
            }*/
    }
}
