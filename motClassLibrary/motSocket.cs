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
using System.Net.Mail;

namespace motInboundLib
{

    public class motMailServer
    {
        public void __test()
        {
            string to = "jane@contoso.com";
            string from = "ben@contoso.com";
            string subject = "Using the new SMTP client.";
            string body = @"Using this new feature, you can send an e-mail message from an application very easily.";

            MailMessage message = new MailMessage(from, to, subject, body);
            SmtpClient client = new SmtpClient("smtp.google.com");
            Console.WriteLine("Changing time out from {0} to 100.", client.Timeout);

            client.Timeout = 100;

            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.

            client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
                            ex.ToString());
            }
        }
    }
    public class motSocket
    {
        //private Port __p;
        private static bool __running = false;

        private string __s_iobuffer = "";
        private byte[] __b_iobuffer;
        private int __portnum;

        private static TcpClient __client;
        private static TcpListener __trigger;
        private static NetworkStream __stream;

        private static EndPoint __remoteEndPoint;
        private static EndPoint __localEndPoint;

        private Logger __logger;

        //private Thread __working_thread;
        //private Thread __client_thread;

        public delegate void __void_delegate(string __data);
        public delegate bool __bool_delegate();

        private __void_delegate __callback;

        /// <summary>
        /// motSocket constructor to immediatly thread the Listener function.
        /// </summary>
        /// <param name="__port">The TCP/IP port number to monitor</param>
        /// <param name="__func">The Function to start on the new thread</param>
        public motSocket(int __port, __void_delegate __callback_p)
        {
            try
            {
                __callback = __callback_p;
                __portnum = __port;
                __logger = LogManager.GetLogger("motInboundLib.Socket");
                __trigger = new TcpListener(IPAddress.Any, __port);
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
            try
            {
                __portnum = __port;
                __trigger = new TcpListener(IPAddress.Any, __port);
                __running = true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to connect to remote socket " + e.Message);
            }
        }

        ~motSocket()
        {
            if (__running)
            {
                close();
            }
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
            __trigger.Start();
            __logger.Info("Listening on port {0}", __portnum);

            while (__running)
            {
                try
                {
                    __client = __trigger.AcceptTcpClient();
                    __stream = __client.GetStream();


                    __remoteEndPoint = __client.Client.RemoteEndPoint;
                    __localEndPoint = __client.Client.LocalEndPoint;

                    Console.WriteLine("Accepted connection from remote endpoint {0}", __remoteEndPoint.ToString());
                    __logger.Info("Accepted connection from remote endpoint {0}", __remoteEndPoint.ToString());

                    /*Task.Run(() =>
                     {
                         Thread.CurrentThread.Name = "reader";
                         read();                    
                     });
                     */

                    if (read() > 0)
                    {
                        __callback(__s_iobuffer);
                    }

                    __stream.Close();
                    __client.Close();
                }
                catch (SocketException e)
                {
                    // Probably shutting down
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // throw new Exception("Failed waiting for trigger " + e.Message); 
                }
            }

            // __trigger.Stop();
            return;
        }

        public int read()
        {
            int __inbytes = 0;
            int __total_bytes = 0;

            __b_iobuffer = new byte[8192];
            __s_iobuffer = "";

            try
            {
                __logger.Info("Reading data ...");
                __stream.ReadTimeout = 300;

                do
                {
                    __inbytes = __stream.Read(__b_iobuffer, 0, __b_iobuffer.Length);
                    __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer);
                    __total_bytes += __inbytes;

                } while (__stream.DataAvailable);
            }
            catch (Exception e)
            {
                Console.WriteLine("read() failed: {0}", e.Message);
            }

            return __total_bytes; ;
        }

        public void write(string __data)
        {
            if (__running)
            {
                try
                {
                    __stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                }
                catch (Exception e)
                {
                    throw new Exception("Socket write failure " + e.Message);
                }
            }
        }

        public void write(byte[] __data)
        {
            if (__running)
            {
                try
                {
                    __stream.Write(__data, 0, __data.Length);
                }
                catch (Exception e)
                {
                    throw new Exception("Socket write failure " + e.Message);
                }
            }
        }

        public void flush()
        {
            if (__running)
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
        }

        public void close()
        {
            if (__running)
            {
                try
                {
                    __running = false;
                    __trigger.Stop();
                }
                catch (Exception e)
                {
                    throw new Exception("Socket close failure " + e.Message);
                }
            }
        }
    }
}
