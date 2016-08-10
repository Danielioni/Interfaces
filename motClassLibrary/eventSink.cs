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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NLog;

namespace motInboundLib
{
        public delegate void __socket_input_event_handler(Object __sender, EventArgs __args);
        public delegate void __http_input_event_handler(Object __sender, EventArgs __args);
        public delegate void __filesystem_input_event_handler(Object __sender, EventArgs __args);

    public class EventMessageArgs : EventArgs
    {
        public object items { get; set; }
        public DateTime timestamp { get; set; }
    }

    class eventSink
    {
        __socket_input_event_handler        __socket_event;
        __http_input_event_handler          __http_event;
        __filesystem_input_event_handler    __filesystem_event;

        public void __event__listener(string __port, string __uri, string __directory)
        {
            __socket_listener(__port);
            __http_listener(__uri);
            __filesystem_listener(__directory);
        }

        private void __http_listener(string __uri)
        {

            string[] __items = { string.Empty };
            EventMessageArgs __args = null;
            __http_input_event_handler __http_handler = __http_event;    

            // await __level0_http_listener();
                    
            __args.items = __items;
            __args.timestamp = DateTime.Now;
            __http_handler(this, __args);
        }

        public void __socket_listener(string __port)
        {
            string[] __items = { string.Empty };
            EventMessageArgs __args = null;
            __socket_input_event_handler __socket_handler = __socket_event;

            // await __level0_socket_listener();

            __args.items = __items;
            __args.timestamp = DateTime.Now;
            __socket_handler(this, __args);
        }

        public void __filesystem_listener(string __directory)
        {
            string[] __items = { string.Empty };
            EventMessageArgs __args = null;
            __filesystem_input_event_handler __filesystem_handler = __filesystem_event;

            // await __level0_filesystem_listener();

            __args.items = __items;
            __args.timestamp = DateTime.Now;
            __filesystem_handler(this, __args);
        }
    }

    class eventListener
    {
        eventSink __listener = new eventSink();
    }
}


        /*
        public delegate void ADT_A01EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
        public delegate void ADT_A12EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
        public delegate void OMP_O09EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
        public delegate void RDE_O11EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
        public delegate void RDS_013EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);

        public class HL7SocketListener
        {
            const int TCP_TIMEOUT = 300000;

            private Logger __logger;
            public motSocket __socket;
            private Thread __worker;
            private int __listener_port = 0;

            List<Dictionary<string, string>> __message_data = new List<Dictionary<string, string>>();

            public ADT_A01EventReceivedHandler ADT_A01MessageEventReceived;
            public ADT_A12EventReceivedHandler ADT_A12MessageEventReceived;
            public OMP_O09EventReceivedHandler OMP_O09MessageEventReceived;
            public RDE_O11EventReceivedHandler RDE_O11MessageEventReceived;
            public RDS_013EventReceivedHandler RDS_O13MessageEventReceived;

        }

        class __doWork()
        {
            
            void __processEvent(object sender, EventArgs args)
            {
                // __listener = new httpListener();
                // __listener = new fileSystemListener();
                // __listener = new mumbleListener()
                //
                // __listener.EventHandler += __mumbleEvent;



                __listener = new HL7SocketListener(Convert.ToInt32(__source_port));

                __listener.ADT_A01MessageEventReceived += __process_ADT_A01_Event;
                __listener.ADT_A12MessageEventReceived += __process_ADT_A12_Event;

                { ... }
            }
        }
        */
