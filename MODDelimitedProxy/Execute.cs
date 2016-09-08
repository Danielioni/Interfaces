using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace motDefaultProxyUI
{
    public class Execute
    {
       
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

        public void __start_up(ExecuteArgs __args)
        {
            Thread.Sleep(2048);
            __update_event_ui("Starting up new system");

            Thread.Sleep(2048);
            __update_error_ui("Sending an error over");
        }

        public void __shut_down()
        {

        }

        public Execute()
        {
            
        }

        ~Execute()
        { }
    }
}
