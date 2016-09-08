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
        Logger __logger;

        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        

        public void __update_event_ui(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__message = __message;
            __event_ui_handler(this, __args);
        }

        public void __update_error_ui(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__message = __message;
            __error_ui_handler(this, __args);

        }

        // Do the real work here - call delegates to update UI
        void __parse(string __data)
        {

            
        }

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
                string __err = string.Format("An error occurred while attempting to start the HL7 listener: {0}", e.Message);
                Console.WriteLine(__err);
                __logger.Error(__err);

                __update_error_ui("Failed to start:" + e.Message);

                throw;
            }
            
        }

        public void __shut_down()
        {

        }

        public Execute()
        {
            __logger = LogManager.GetLogger("motDelimitedProxy");        
        }

        ~Execute()
        { }
    }
}
