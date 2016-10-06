using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using motCommonLib;
using NLog;
using System.Xml;


namespace XMLProxy
{
    public class Execute
    {
        private string __RxSys_Key = "PioneerRxID";
        private XmlDocument __xml_doc;

        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;        

        motSocket __listener;
        Thread __worker;

        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Error;
        public bool __auto_truncate { get; set; } = false;

        motLookupTables __lookup = new motLookupTables();
        Logger __logger = null;
        LogLevel __log_level { get; set; } = LogLevel.Error;

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
        private void __parse_xml(string __data)
        {
            __update_event_ui(string.Format("Received Request from {0}", __listener.remoteEndPoint.ToString()));
            __logger.Info("Data: {0}", __data);

            try
            {
                if (__data.Contains(__RxSys_Key))  // Do this better so we can isolate the sending system
                {
                    __xml_doc.LoadXml(__data);

                    // The interesting parts of PioneerRx XML are:
                    //          Pharmacy
                    //          Prescribers<Prescriber>           
                    //          Patient
                    //          Facility
                    //          Rx

                    XmlNodeList __pharmacy_list = __xml_doc.GetElementsByTagName("Pharmacy");
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
                    
                }
            }
            catch
            {

            }
        }
        public void __start_up(ExecuteArgs __args)
        {
            try
            {
                Console.WriteLine("__start_listener: {0}", Thread.CurrentThread.ManagedThreadId);
              
                int __lp = Convert.ToInt32(__args.__listen_port);
                __listener = new motSocket(__lp, __parse_xml);

                __worker = new Thread(new ThreadStart(__listener.listen));
                __worker.Name = "listener";
                __worker.Start();

                __update_event_ui("Started listening to on port: " + __args.__listen_port);
                __update_event_ui(string.Format("Sending data to: {0}:{1}", __args.__gateway_address, __args.__gateway_port));
            }
            catch (Exception e)
            {
                __update_error_ui(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
                __logger.Log(__log_level, "Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message);
            }
        }

        public void __shut_down()
        {
            __update_event_ui("XML Proxy Shutting down");
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("XMLProxy");
        }

        ~Execute()
        { }
    }
}
