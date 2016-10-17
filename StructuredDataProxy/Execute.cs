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
using NLog;

namespace StructuredDataProxy
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
        motSocket   __listener;
        motPort     __gateway;
        Thread      __worker;
        XmlDocument __xml_doc;
        ExecuteArgs __args;

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

        void __parsePioneerRxRecord(XmlDocument __xdoc)
        {
            XmlNodeList __node_list;
            motPort __gateway = new motPort();

            // The interesting parts of PioneerRx XML are:
            //          Pharmacy
            //          Prescribers<Prescriber>           
            //          Patient
            //          Facility
            //          Rx


            /*
                  <Pharmacy>
                     <Identification>
                       <PioneerRxID></PioneerRxID>
                       <NCPDP></NCPDP>
                       <NPI></NPI>
                       <DEA></DEA>
                       <DPS></DPS>
                       <FederalTaxID></FederalTaxID>
                       <StateLicenseNumber></StateLicenseNumber>
                     </Identification>
                     <PharmacyName></PharmacyName>
                     <StoreNumber></StoreNumber>
                     <Addresses>
                       <Address>
                         <SequenceNumber></SequenceNumber>
                         <AddressLine1></AddressLine1>
                         <AddressLine2></AddressLine2>
                         <City></City>
                         <StateCode></StateCode>
                         <ZipCode></ZipCode>
                         <Type></Type>
                       </Address>
                     </Addresses>
                     <PhoneNumbers>
                       <PhoneNumber>
                         <SequenceNumber></SequenceNumber>
                         <AreaCode></AreaCode>
                         <Number></Number>
                         <Extension></Extension>
                         <Type></Type>
                       </PhoneNumber>
                     </PhoneNumbers>
                     <PrimaryPhoneSequenceNumber></PrimaryPhoneSequenceNumber>
                     <PrimaryFaxSequenceNumber></PrimaryFaxSequenceNumber>
                     <Email></Email>
                     <WebAddress></WebAddress>
                     <PharmacistInChargeEmployeeID></PharmacistInChargeEmployeeID>
                   </Pharmacy>
               */
            motStoreRecord __store = new motStoreRecord("Add", __args.__error_level, __args.__auto_truncate);
           
            __store.StoreName       = __xml_doc.GetElementsByTagName("PharmacyName").Item(0).InnerText;
            __store.RxSys_StoreID   = __xml_doc?.GetElementsByTagName("PioneerRxID").Item(0).InnerText;
            __store.DEANum          = __xml_doc?.GetElementsByTagName("DEA").Item(0).InnerText;
            __store.Address1        = __xml_doc?.GetElementsByTagName("AddressLine1").Item(0).InnerText;
            __store.Address2        = __xml_doc?.GetElementsByTagName("AddressLine2").Item(0).InnerText;
            __store.City            = __xml_doc?.GetElementsByTagName("City").Item(0).InnerText;
            __store.State           = __xml_doc?.GetElementsByTagName("StateCode").Item(0).InnerText;
            __store.Zip             = __xml_doc?.GetElementsByTagName("ZipCode").Item(0).InnerText;

            __node_list = __xml_doc?.GetElementsByTagName("PhoneNumbers");
            IEnumerator __ienum = __node_list.GetEnumerator();

            while (__ienum.MoveNext())
            {
                XmlNode __node = (XmlNode)__ienum.Current;

                if(__node.ChildNodes[4].InnerText.ToLower() == "fax")
                {
                    __store.Fax = __node.ChildNodes[0].InnerText + __node.ChildNodes[1].InnerText;
                }
                else
                {
                    __store.Phone = __node.ChildNodes[0].InnerText + __node.ChildNodes[1].InnerText;
                }
            }

            __gateway.Open();
            //__store.Write(__gateway);
            __gateway.Close();
           
        }

        void __parse_data(string __data)
        {
            __show_common_event(string.Format("Received Request from {0}", __listener.remoteEndPoint.ToString()));
            __logger.Info("Data: {0}", __data);

            try
            {
                // Determine the data type

                // Valid XML
                if(__data.Contains("<?xml version="))
                {
                    __xml_doc = new XmlDocument();

                    // Pretty sure its real XML
                    __data = __data.Remove(0, __data.IndexOf("\n<") +1 );
                    __xml_doc.LoadXml(__data);

                    // Let's try and figure out whose data it is. 
                    if (__data.Contains("PioneerRxID"))
                    {
                        // Pretty obvious
                        __show_common_event("PioneerRx Formatted Data");
                        __parsePioneerRxRecord(__xml_doc);
                    }
                   
                }

                if (Regex.IsMatch(__data, @"?<=\{)\s *[^{]*? (?=[\},])(?<=\{)\s *[^{]*? (?=[\},]"))
                {
                    // Pretty sure its real JSON
                    // It's easier to work on it in XML I think

                }

                // Valid Other ...
            }
            catch(Exception ex)
            {
                __logger.Error("Parse Pioneer Data: {0}", ex.Message);
                __show_error_event("Parse Pioneer Data Failure: " + ex.Message);
            }
        }

        public void __start_up(ExecuteArgs __args)
        {
            this.__args = __args;

            // Test cross thread communication
            Task.Run(() =>
            {
                try
                {
                    __show_common_event("Strucured Data Proxy Starting Up");
                }
                catch (Exception e)
                {
                    __show_error_event(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
                    __logger.Log(__log_level, "Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message);
                }
            });

            // Generic listener setup
            try
            {
                Console.WriteLine("__start_listener: {0}", Thread.CurrentThread.ManagedThreadId);

                int __lp = Convert.ToInt32(__args.__listen_port);
                __listener = new motSocket(__lp, __parse_data);

                __worker = new Thread(new ThreadStart(__listener.listen));
                __worker.Name = "listener";
                __worker.Start();

                __show_common_event("Started listening to on port: " + __args.__listen_port);

                __gateway = new motPort(__args.__gateway_address, __args.__gateway_port, false);
                __show_common_event(string.Format("Sending data to: {0}:{1}", __args.__gateway_address, __args.__gateway_port));               
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
                __logger.Log(__log_level, "Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message);
            }
        }

        public void __shut_down()
        {
            __listener.close();
            __show_error_event("Structured Data Proxy Shutting down");
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("StructuredDataProxy");
        }

        ~Execute()
        { }
    }
}
