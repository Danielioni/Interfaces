using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using motCommonLib;
using NLog;


using Autofac;
using Mot.Client.Sdk;
using Mot.Shared.Framework;
using Mot.Shared.Model;
using Mot.Shared.Model.Rxes;
using Mot.Shared.Model.Patients;

namespace QuickMAR
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

        private string __target_address;
        private int __target_port;
        private string __sending_app,
                       __pharmacy_id,
                       __recieving_facility;

        private long __control_number;

        //----- motNext
        public static IContainer container;
        private static IAuthenticationService authService;
        private IEnumerable<Patient> __patients;
        ICollection<Rx> __Rxs;
        private bool __logged_in = false;

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
        void __parse_data(string __data)
        {

        }

        public async void __start_up(ExecuteArgs __args)
        {
            this.__target_address = __args.__gateway_address;
            this.__target_port = Convert.ToInt32(__args.__gateway_port);
            this.__sending_app = __args.__sending_application;
            this.__pharmacy_id = __args.__pharmacy_id;
            this.__recieving_facility = __args.__receiving_facility;

            this.__control_number = Convert.ToInt64(Properties.Settings.Default.ControlNumber);

            try
            {
                using (var p = new motSocket(__target_address, __target_port))
                {
                    __show_common_event("QuickMAR Proxy Starting Up");
                    __show_common_event(string.Format("Sending to: {0}:{1}", __args.__gateway_address, __args.__gateway_port));
                }
                
               await Login("mot", "mot");
                
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__gateway_address, __args.__gateway_port, e.Message));
                __logger.Error("Failed to start on {0}:{1}, Error: {2}", __args.__gateway_address, __args.__gateway_port, e.Message);
            }
        }

        public void __shut_down()
        {
            __show_error_event("Default Proxy Shutting down");
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("QuickMAR");
        }

        ~Execute()
        { }

        //------------------------------------------------------------------------------------------------------

        public void __send_message(string __message)
        {
            try
            {
                byte[] __retval = new byte[256];

                using (var __target = new motSocket(__target_address, __target_port))
                {
                    __target.write(__message);
                    __target.read(ref __retval, 0, 256);
                    __show_common_event(Encoding.UTF8.GetString(__retval, 0, 256));

                    __control_number++;
                    Properties.Settings.Default.ControlNumber = string.Format("{0,10}", __control_number.ToString("D10"));
                }
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("Message Wite Failed. Error: {0}", ex.Message));
                __logger.Error("Message Wite Failed. Error: {0}", ex.Message);
            }


        }

        public string __build_MSH(string __type)
        {

            return string.Format(@"MSH|^ ~\&|{0}|{1}|QUICKMAR|{3}|{4:yyyyMMddhhmmss}||{5}|{6}|P|2.5|",
                                        __sending_app,
                                        __pharmacy_id,
                                        __recieving_facility,
                                        DateTime.Now,
                                        __type,
                                        string.Format("{0,10}", __control_number.ToString("D10"))
                                );
        }


        public void __build_RDEO01_Set()
        {
            string __message = __build_MSH("RDE^O01") + "\n";

        }

        public void __build_RDEO11_Set()
        {
            string __message = __build_MSH("RDE^O11") + "\n";
        }

        public void __build_RDSO13_Set()
        {
            string __message = __build_MSH("RDS^O13") + "\n";
        }

        public void _build_ADTO01_Set()
        {
            string __message = __build_MSH("ADT^O01") + "\n";

        }

        public void _build_ADTO03_Set()
        {
            string __message = __build_MSH("ADT^O03") + "\n";
        }

        public void _build_ADTO08_Set()
        {
            string __message = __build_MSH("ADT^O08") + "\n";
        }


        //----------------------------------------------------------------------------------
        //   #region Database


        public async Task Login(string __uname, string __pw)
        {
            try
            {
                __logger = LogManager.GetLogger("QuickMAR_motNextDB");

                Simple.OData.Client.V4Adapter.Reference();

                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterModule<FrameworkModule>();
                containerBuilder.RegisterModule<SdkModule>();
                container = containerBuilder.Build();

                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                authService = container.Resolve<IAuthenticationService>();

                var auth = new AuthorizeModel();

                //auth.IsGrantAccesNeed = false;
                auth.GrantAccessNeeded = false;
                auth.UserName = __uname;
                auth.UserPassword = __pw;

                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                await authService.LoginAsync(auth);

                __logged_in = true;
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("Message Wite Failed. Error: {0}", ex.Message));
                __logger.Error("Message Wite Failed. Error: {0}", ex.Message);
            }
        }
    }
}
