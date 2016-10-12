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

    /// <summary>
    /// Calling UI From Sub Modules,Libraries, etcetera.
    /// 
    /// Put this into any modules that need to call up to the UI:
    /// 
    /// public class UIupdateArgs : EventArgs
    /// {
    ///     public string __event_message { get; set; }
    ///     public string __data_one { get; set; }
    ///     public string __data_nnn { get; set; }
    ///     public string timestamp { get; set; }
    /// }
    /// 
    /// public delegate void UpdateUIEventHandler(Object __sender, UIupdateArgs __args);
    /// public delegate void UpdateUIErrorHandler(Object __sender, UIupdateArgs __args);
    /// 
    /// Call it like this:
    ///      __ui_args.__ms__data = __some_data;
    ///      __ui_args.__event_message = "REJECTED";
    ///
    ///       UpdateErrorUI(this, __ui_args);
    ///       
    /// </summary>
    /// 
    public class Execute
    {
        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Error;
        public bool __auto_truncate { get; set; } = false;

        motLookupTables __lookup = new motLookupTables();

        Logger __logger = null;
        LogLevel __log_level { get; set; } = LogLevel.Error;

        public void __update_event_ui(Object __sender, UIupdateArgs __args)
        {
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message += "\n";

            __event_ui_handler(this, __args);
        }

        public void __update_error_ui(Object __sender, UIupdateArgs __args)
        {
            
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message += "\n";

            __error_ui_handler(this, __args);
        }

        public void __show_common_event(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message = __message + "\n";

            __event_ui_handler(this, __args);
        }

        public void __show_error_event(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message = __message + "\n";

            __error_ui_handler(this, __args);
        }

        // Do the real work here - call delegates to update UI

        public void __start_up(ExecuteArgs __args)
        {
            Task.Run(() =>
            {
                try
                {
                    __show_common_event("Default Proxy Starting Up");
                    __show_common_event(string.Format("Listening on: {0}:{1}, Sending to: {2}:{3}", __args.__listen_address, __args.__listen_port, __args.__gateway_address, __args.__gateway_port));
                }
                catch (Exception e)
                {
                    __show_error_event(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
                    __logger.Log(__log_level, "Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message);
                }
            });
        }

        public void __shut_down()
        {
            __show_error_event("Default Proxy Shutting down");
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("motDefaultProxyUI");
        }

        ~Execute()
        { }
    }
}
