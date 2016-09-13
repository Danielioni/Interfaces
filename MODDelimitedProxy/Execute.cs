using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using motCommonLib;
using NLog;

namespace motDefaultProxyUI
{
    public class Execute
    {
        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Error;
        public bool __auto_truncate { get; set; } = false;

        motLookupTables __lookup = new motLookupTables();
        Logger __logger = null;

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

        }

        // Do the real work here - call delegates to update UI

        public void __start_up(ExecuteArgs __args)
        {
            try
            {
                __update_event_ui("Default Proxy Starting Up");
                __update_event_ui(string.Format("Listening on: {0}:{1}, Sending to: {2}:{3}", __args.__listen_address, __args.__listen_port, __args.__gateway_address, __args.__gateway_port));
            }
            catch (Exception e)
            {
                __update_error_ui(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
            }
        }

        public void __shut_down()
        { 
            __update_event_ui("Default Proxy Shutting down");
        }

        public Execute()
        {

        }

        ~Execute()
        { }
    }
}
