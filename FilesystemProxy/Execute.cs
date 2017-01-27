using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.Text.RegularExpressions;
using motCommonLib;
using motInboundLib;
using NLog;

namespace FilesystemProxy
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
        //motSocket __listener;
        //motSocket __gateway;
        Thread __worker;
        ExecuteArgs __args;

        motFileSystemListener __fsl;

        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        public bool __auto_truncate { get; set; } = false;
        public bool __debug_mode { get; set; } = false;
        public bool __send_eof { get; set; } = false;

        motLookupTables __lookup = new motLookupTables();

        Logger __logger = null;
        LogLevel __log_level { get; set; } = LogLevel.Error;


        void __update_ui_event(object __sender, UIupdateArgs __args)
        {
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message += "\n";

            __event_ui_handler(this, __args);
        }

        void __update_ui_error(object __sender, UIupdateArgs __args)
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

        public void __parse_data(string data)
        { }

        
        public void __start_up(ExecuteArgs __args)
        {
            this.__args = __args;

            try
            {
                __send_eof = __args.__send_eof;
                __debug_mode = __args.__debug_mode;
                __auto_truncate = __args.__auto_truncate;

                __fsl = new motFileSystemListener(__args.__directory, __args.__gateway_address, __args.__gateway_port, __args.__file_type, __args.__auto_truncate, __args.__send_eof, __args.__debug_mode);

                __fsl.UpdateEventUI += __update_ui_event;
                __fsl.UpdateErrorUI += __update_ui_error;

                __worker = new Thread(() => __fsl.watchDirectory(__args.__directory, __args.__gateway_address, __args.__gateway_port));
                __worker.Name = "filesystem listener";
                __worker.Start();

                __show_common_event("Started listening to directory: " + __args.__directory + " and sending to gateway at: " + __args.__gateway_address + "/" + __args.__gateway_port);
            }
            catch(Exception ex)
            {
                __show_error_event(ex.Message);
            }        
        }

        public void __shut_down()
        {
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("FileSystemProxy");
        }

        ~Execute()
        { }
    }
}
