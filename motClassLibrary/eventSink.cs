using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    class eventSink
    {
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

        }
    }
