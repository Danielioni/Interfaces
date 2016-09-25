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
        motSocket __listener;
        motPort __gateway;
        Thread __worker;

        Logger __logger = null;
        LogLevel __log_level { get; set; } = LogLevel.Error;

        public bool __auto_truncate { get; set; } = false;

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
        void __parse(string __data)
        {
            __update_event_ui(string.Format("Received Request from {0}", __listener.remoteEndPoint.ToString()));
            __logger.Info("Data: {0}", __data);

            var __parser = new motParser();

            __parser.__log_level = __log_level;
            __parser.p = __gateway;
            __parser.parseDelimited(__data);
            

        }

        //string __test_prescriber = "AP\xEE LastName\xEE FirstName\xEE MiddleInitial\xEE Address1\xEE Address2\xEE City\xEE State\xEE Zip\xEE Phone\xEE Comments\xEE DEA_ID\xEE TPID\xEE Speciality\xEE Fax\xEE PagerInfo\xEERxSysDoc1\xEE 1025143\xE2 AP\xEEpLastName\xEEpFirstName\xEEpMiddleInitial\xEEpAddress1\xEEpAddress2\xEEpCity\xEEpState\xEEpZip\xEEPhone\xEEpComments\xEEpDEA_ID\xEEpTPID\xEEpSpeciality\xEEpFax\xEEpPagerInfo\xEERxSysDoc2\xEE 1972834\xE2";

        public void __start_up(ExecuteArgs __args)
        {
            try
            {
                Console.WriteLine("__start_listener: {0}", Thread.CurrentThread.ManagedThreadId);

                int __lp = Convert.ToInt32(__args.__listen_port);
                __listener = new motSocket(__lp, __parse);
                __gateway = new motPort(__args.__gateway_address, __args.__gateway_port);

                // This will start the listener and call the callback 
                __worker = new Thread(new ThreadStart(__listener.listen));
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
            __listener.close();
            __gateway.Close();
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
