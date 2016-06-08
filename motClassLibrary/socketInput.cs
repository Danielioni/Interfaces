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

namespace motInboundLib
{
   
    public class motSocket
    {
        Port __p;
        private static bool __running = false;

        private string              __s_iobuffer = "";
        private byte[]              __b_iobuffer;
        private     int             __portnum;

        private  static TcpClient       __client;
        private  static TcpListener     __trigger;
        private  static NetworkStream   __stream;

        private  static EndPoint        __remoteEndPoint;
        private  static EndPoint        __localEndPoint;

        private     Logger          __logger;

        private     Thread          __working_thread;
        private     Thread          __client_thread;

        public      delegate void   __void_delegate(string __data);
        public      delegate bool   __bool_delegate();

        private     __void_delegate __callback;

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
            catch(Exception e)
            {
                throw new Exception("Failed to connect to remote socket " +  e.Message);
            }
        }

        ~motSocket()
        {
            if(__running)
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
            while(__running)
            {
                try
                {
                    __trigger.Start();
                    __client = __trigger.AcceptTcpClient();
                    __stream = __client.GetStream();

                    __remoteEndPoint = __client.Client.RemoteEndPoint;
                    __localEndPoint = __client.Client.LocalEndPoint;

                    __client_thread = new Thread(new ThreadStart(read));
                    __client_thread.Name = "reader";
                    __client_thread.Start();
                }
                catch (SocketException e)
                {
                    // Probabbly shutting down
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                   // throw new Exception("Failed waiting for trigger " + e.Message); 
                }
              
            }

           // __trigger.Stop();
            return;
        }
        
        public void read()
        {
            int __inbytes = 0;
            int __total_bytes = 0;

            __b_iobuffer = new byte[8192];
            __s_iobuffer = ""; 

            try
            {
                while (true)
                {
                    __inbytes = __stream.Read(__b_iobuffer, 0, __b_iobuffer.Length);
                    if (__inbytes > 0)
                    {
                        __s_iobuffer += Encoding.UTF8.GetString(__b_iobuffer);
                        __total_bytes += __inbytes;
                    }
                    else
                    {
                        break;
                    }
                }

                if(__total_bytes > 0)
                {
                    __callback(__s_iobuffer);
                    __stream.Close();

                    // stop the thread we're on, don't need it any more
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("read() failed: {0}", e.Message);
            }
        }

        public void write(string __data)
        {
            if(__running)
            {
                try
                {
                    __stream.Write(Encoding.UTF8.GetBytes(__data), 0, __data.Length);
                }
                catch(Exception e)
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
