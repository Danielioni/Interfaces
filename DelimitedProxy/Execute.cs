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
using motCommonLib;
using NLog;

namespace DelimitedProxy
{
    public class Execute
    {
        private motSocket __socket;
        private Thread __worker;

        string __gateway_address;
        string __gateway_port;

        Logger __logger = null;
        LogLevel __log_level { get; set; } = LogLevel.Info;

        public bool __auto_truncate { get; set; } = false;
        public bool __use_v1 { get; set; } = false;

        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Info; // For debugging, set to Error for production

        //motErrorlLevel __save_error_level = motErrorlLevel.Error;
        motLookupTables __lookup = new motLookupTables();

        public void __update_event_ui(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__message = __message + "\n";
            __event_ui_handler(this, __args);
        }

        public void __update_error_ui(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__message = __message + "\n";
            __error_ui_handler(this, __args);

            __logger.Log(__log_level, "{0}", __message);
        }

        // Do the real work here - call delegates to update UI
        private void __clean_buffer(byte[] __b_iobuffer)
        {
            for (int i = 0; i < __b_iobuffer.Length; i++)
            {
                // Ugly hack to get around UTF8 Normalization failing to convert properly
                // It makes the MOT delimited interface fail

                if (__b_iobuffer[i] == '\xEE')
                {
                    __b_iobuffer[i] = (byte)'~';
                }
                if (__b_iobuffer[i] == '\xE2' || __b_iobuffer[i] == '\x0A')
                {
                    __b_iobuffer[i] = (byte)'^';
                }
            }
        }
        void __parse(string __data)
        {
            Console.WriteLine("Received Request from {0}", __socket.remoteEndPoint.ToString());

            __update_event_ui(string.Format("Received Request from {0}", __socket.remoteEndPoint.ToString()));
            __logger.Debug("Data: {0}", __data);

            try
            {
                var __parser = new motParser();

                __parser.__log_level = __log_level;
                __parser.p = new motSocket(__gateway_address, Convert.ToInt32(__gateway_port), __delimited_protocol_processor);
                __parser.parseDelimited(__data, __use_v1);

            }
            catch (Exception ex)
            {
                __update_error_ui("Failed at __parse: " + ex.Message);
                __logger.Error("Failed at __parse: {0}", ex.Message);
            }
        }

        private bool __delimited_protocol_processor(byte[] __buffer)
        {
            try
            {
                // just pass along the response
                __socket.write_return(__buffer);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //string __test_prescriber = "AP\xEE LastName\xEE FirstName\xEE MiddleInitial\xEE Address1\xEE Address2\xEE City\xEE State\xEE Zip\xEE Phone\xEE Comments\xEE DEA_ID\xEE TPID\xEE Speciality\xEE Fax\xEE PagerInfo\xEERxSysDoc1\xEE 1025143\xE2 AP\xEEpLastName\xEEpFirstName\xEEpMiddleInitial\xEEpAddress1\xEEpAddress2\xEEpCity\xEEpState\xEEpZip\xEEPhone\xEEpComments\xEEpDEA_ID\xEEpTPID\xEEpSpeciality\xEEpFax\xEEpPagerInfo\xEERxSysDoc2\xEE 1972834\xE2";

        public void __start_up(ExecuteArgs __args)
        {
            try
            {
                Console.WriteLine("__start_listener: {0}", Thread.CurrentThread.ManagedThreadId);

                __gateway_address = __args.__gateway_address;
                __gateway_port = __args.__gateway_port;

                int __lp = Convert.ToInt32(__args.__listen_port);
                __socket = new motSocket(__lp, __parse);
                __socket.__b_stream_processor = __clean_buffer;
                __socket.__b_protocol_processor = __delimited_protocol_processor;

                __use_v1 = __args.__use_v1;

                // This will start the listener and call the callback 
                __worker = new Thread(new ThreadStart(__socket.listen));
                __worker.Name = "listener";
                __worker.Start();

                __update_event_ui("Started listening to on port: " + __args.__listen_port);
                __update_event_ui(string.Format("Sending data to: {0}:{1}", __args.__gateway_address, __args.__gateway_port));
            }
            catch (Exception e)
            {
                string __err = string.Format("An error occurred while attempting to start the delimited gateway listener: {0}", e.Message);

                Console.WriteLine(__err);
                __logger.Log(__log_level, __err);
                __update_error_ui(__err);

                throw;
            }
        }

        public void __shut_down()
        {
            __socket.close();
            __worker.Join();
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("motDelimitedProxy");
        }

        ~Execute()
        { }
    }
}
