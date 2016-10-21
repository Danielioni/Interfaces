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
        motSocket __listener;
        motPort __gateway;
        Thread __worker;
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
            //__gateway = new motPort();  // Clever way to turn off gateway for testing

            // The interesting parts of PioneerRx XML are:
            //          Pharmacy
            //          Prescribers<Prescriber>           
            //          Patient
            //          Facility
            //          Rx

            try
            {
                XmlNode __pharmacy = __xdoc.SelectSingleNode("//Pharmacy");


                motStoreRecord __store = new motStoreRecord("Add", __args.__error_level, __args.__auto_truncate);

                __store.StoreName = __pharmacy.SelectSingleNode("PharmacyName").InnerText;
                __store.RxSys_StoreID = __pharmacy.SelectSingleNode("Identification/PioneerRxID").InnerText;
                __store.DEANum = __pharmacy.SelectSingleNode("Identification/DEA").InnerText;
                __store.Address1 = __pharmacy.SelectSingleNode("//Address/AddressLine1").InnerText;
                __store.Address2 = __pharmacy.SelectSingleNode("//Address/AddressLine2").InnerText;
                __store.City = __pharmacy.SelectSingleNode("//Address/City").InnerText;
                __store.State = __pharmacy.SelectSingleNode("//Address/StateCode").InnerText;
                __store.Zip = __pharmacy.SelectSingleNode("//Address/ZipCode").InnerText;

                XmlNodeList __phone_node_list = __pharmacy.SelectNodes("PhoneNumbers/PhoneNumber");
                foreach (XmlNode __num in __phone_node_list)
                {
                    if (__num["Type"].InnerText.ToLower() == "fax")
                    {
                        __store.Fax = __num["AreaCode"].InnerText + __num["Number"].InnerText;
                    }
                    else
                    {
                        __store.Phone = __num["AreaCode"].InnerText + __num["Number"].InnerText;
                    }
                }

                __store.Write(__gateway);
                __show_common_event("Added Store: " + __store.StoreName);
            }
            catch (Exception ex)
            {
                __show_error_event("Failed writing Store Record: " + ex.Message);
            }

            try
            {
                XmlNodeList __prescribers = __xdoc.SelectNodes("//Prescribers/Prescriber");

                foreach (XmlNode __prescriber in __prescribers)
                {
                    var __doc = new motPrescriberRecord("Add", __args.__error_level, __args.__auto_truncate);

                    // its odd.  The PrescriberID is writen like this:
                    //      <PresccriberID>
                    //          <GUID/>40DE86C9-FD69-44D7-9DA4-4D2A3194DE49
                    //      </PrescriberID>
                    // and I can't find a way to get at the XPath lets me get the node but no value 
                    //__doc.RxSys_DocID = __prescriber.SelectSingleNode("Identification/PrescriberID").ChildNodes[1].InnerText;

                    __doc.RxSys_DocID = __prescriber.SelectSingleNode("Identification/PrescriberID/GUID").InnerText;
                    __doc.DEA_ID = __prescriber.SelectSingleNode("Identification/DEA").InnerText;
                    __doc.FirstName = __prescriber.SelectSingleNode("Name/FirstName").InnerText;
                    __doc.LastName = __prescriber.SelectSingleNode("Name/LastName").InnerText;
                    __doc.MiddleInitial = __prescriber.SelectSingleNode("Name/MiddleName").InnerText;

                    // We can only take a single address, so just take the first one
                    __doc.Address1 = __prescriber.SelectSingleNode("//Address/AddressLine1").InnerText;
                    __doc.Address2 = __prescriber.SelectSingleNode("//Address/AddressLine2").InnerText;
                    __doc.City = __prescriber.SelectSingleNode("//Address/City").InnerText;
                    __doc.State = __prescriber.SelectSingleNode("//Address/StateCode").InnerText;
                    __doc.PostalCode = __prescriber.SelectSingleNode("//Address/City").InnerText;
                    __doc.Specialty = string.IsNullOrEmpty(__prescriber["PrimarySpecializationID"].InnerText) ? 0 : Convert.ToInt32(__prescriber["PrimarySpecializationID"].InnerText);

                    XmlNodeList __phone_node_list = __prescriber.SelectNodes("PhoneNumbers/PhoneNumber");
                    foreach (XmlNode __num in __phone_node_list)
                    {
                        if (__num["Type"].InnerText.ToLower() == "fax")
                        {
                            __doc.Fax = __num["AreaCode"].InnerText + __num["Number"].InnerText;
                        }
                        else
                        {
                            __doc.Phone = __num["AreaCode"].InnerText + __num["Number"].InnerText;
                        }
                    }

                    __doc.Email = __prescriber.SelectSingleNode("Email").InnerText;
                    __doc.Comments += __prescriber.SelectSingleNode("Comments/Informational").InnerText + "\n";
                    __doc.Comments += __prescriber.SelectSingleNode("Comments/Critical").InnerText + "\n";
                    __doc.Comments += __prescriber.SelectSingleNode("Comments/PointOfSale").InnerText + "\n";

                    __doc.Write(__gateway);

                    __show_common_event("Added Prescriber: " + __doc.FirstName + " " + __doc.LastName);
                }
            }
            catch (Exception ex)
            {
                __show_error_event("Failed Writing Records: " + ex.Message);
                throw;
            }


            try
            {
                var __patient = __xdoc.SelectSingleNode("//Patient");
                var __pat = new motPatientRecord("Add", __args.__error_level, __args.__auto_truncate);
                List<motPrescriptionRecord> __rx_list = new List<motPrescriptionRecord>();

                __pat.RxSys_PatID = __patient?.SelectSingleNode("//Identification/PatientID/Guid").InnerText;
                __pat.SSN = __patient?.SelectSingleNode("//Identification/SSN").InnerText;
                __pat.DOB = __patient?.SelectSingleNode("DateOfBirth/Date").InnerText;
                __pat.FirstName = __patient?.SelectSingleNode("Name/FirstName").InnerText;
                __pat.LastName = __patient?.SelectSingleNode("Name/LastName").InnerText;
                __pat.MiddleInitial = __patient?.SelectSingleNode("Name/MiddleName").InnerText;
                __pat.Gender = __patient?.SelectSingleNode("Gender").InnerText;
                __pat.Height = string.IsNullOrEmpty(__patient?.SelectSingleNode("HeightInches").InnerText) ? 0 : Convert.ToInt32(__patient?.SelectSingleNode("HeightInches").InnerText);
                __pat.Weight = string.IsNullOrEmpty(__patient?.SelectSingleNode("WeightOz").InnerText) ? 0 : Convert.ToInt32(__patient?.SelectSingleNode("WeightOz").InnerText);

                // We can only take a single address, so just take the first one
                __pat.Address1 = __patient?.SelectSingleNode("//Address/AddressLine1").InnerText;
                __pat.Address2 = __patient?.SelectSingleNode("//Address/AddressLine2").InnerText;
                __pat.City = __patient?.SelectSingleNode("//Address/City").InnerText;
                __pat.State = __patient?.SelectSingleNode("//Address/StateCode").InnerText;
                __pat.Zip = __patient?.SelectSingleNode("//Address/City").InnerText;

                __pat.Email = __patient?.SelectSingleNode("Email").InnerText;

                __pat.Comments += __patient?.SelectSingleNode("Comments/Informational").InnerText + "\n";
                __pat.Comments += __patient?.SelectSingleNode("Comments/Critical").InnerText + "\n";
                __pat.Comments += __patient?.SelectSingleNode("Comments/PointOfSale").InnerText + "\n";
                __pat.Comments += __patient?.SelectSingleNode("Comments/LastMTMComment").InnerText + "\n";

                XmlNodeList __phone_node_list = __patient?.SelectNodes("PhoneNumbers/PhoneNumber");               
                foreach (XmlNode __num in __phone_node_list)
                {
                    if (__num["SequenceNumber"].InnerText == "1")
                    {
                        __pat.Phone1 = __num["AreaCode"].InnerText + __num["Number"].InnerText;
                    }
                    else if (__num["SequenceNumber"].InnerText == "2")
                    {
                        __pat.Phone2 = __num["AreaCode"].InnerText + __num["Number"].InnerText;
                    }                
                }

                __pat.RxSys_DocID = __patient?.SelectSingleNode("PatientPrimaryPrescriberID").InnerText;

                XmlNodeList __allergy_node_list = __patient?.SelectNodes("Allergies/Allergy");
                int i = 1; 
                foreach(XmlNode __allergy in __allergy_node_list)
                {
                    __pat.Allergies += string.Format("{0}) {1}, MedID: {2}, IDType: {3}\n", i++, __allergy["Description"].InnerText, __allergy["MedicationID"].InnerText,__allergy["MedicationIDType"].InnerText);
                }

                XmlNodeList __scrip_node_list = __patient?.SelectNodes("OtherMedications/Medication");
                foreach(XmlNode __scrip in __scrip_node_list)
                {
                    var __new_scrip = new motPrescriptionRecord("Add", __args.__error_level, __args.__auto_truncate);

                    __new_scrip.RxSys_RxNum = __scrip["RxNumber"].InnerText;
                    __new_scrip.RxSys_DocID = __scrip["Prescriber"].InnerText;
                    __new_scrip.RxSys_DrugID = __scrip["DrugNDC"].InnerText;
                    __new_scrip.QtyDispensed = __scrip["DaysSupply"].InnerText;
                    __new_scrip.RxStartDate = __scrip["LastDateFilled"].InnerText;
                    __new_scrip.Refills = __scrip["RefillsRemaining"].InnerText;

                    __new_scrip.RxSys_PatID = __pat.RxSys_PatID;
                    __new_scrip.RxType = "0";

                    __rx_list.Add(__new_scrip);
                }

                __pat.Write(__gateway);
                __show_common_event("Added Patient: " + __pat.FirstName + " " + __pat.LastName);
                foreach(motPrescriptionRecord __scp in __rx_list)
                {
                    __scp.Write(__gateway);
                    __show_common_event("Added Scrip: " + __scp.RxSys_RxNum + " for " + __pat.FirstName + " " + __pat.LastName);
                }

            }
            catch(Exception ex)
            {
                __show_error_event("Failed Writing Records: " + ex.Message);
                throw;
            }

            // Facility
            // RX

        }

        void __parse_data(string __data)
        {
            __show_common_event(string.Format("Received Request from {0}", __listener.remoteEndPoint.ToString()));
            __logger.Info("Data: {0}", __data);

            try
            {
                // Determine the data type

                // Valid XML
                if (__data.Contains("<?xml version="))
                {
                    __xml_doc = new XmlDocument();

                    // Pretty sure its real XML
                    __data = __data.Remove(0, __data.IndexOf("\n<") + 1);
                    __xml_doc.LoadXml(__data);

                    // Let's try and figure out whose data it is. 
                    if (__data.Contains("PioneerRxID"))
                    {
                        // Pretty obvious
                        __show_common_event("PioneerRx Formatted Data");
                        __parsePioneerRxRecord(__xml_doc);
                    }

                }
                /*
                if (Regex.IsMatch(__data, @"?<=\{)\s *[^{]*? (?=[\},])(?<=\{)\s *[^{]*? (?=[\},]"))
                {
                    // Pretty sure its real JSON
                    // It's easier to work on it in XML I think

                }

                // Valid Other ...
                */
            }
            catch (Exception ex)
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

                __gateway = new motPort(__args.__gateway_address, __args.__gateway_port);
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
            if (__listener != null)
            {
                __listener.close();
                __show_common_event("Structured Data Proxy Shutting down");
            }
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("StructuredDataProxy");
        }

        ~Execute()
        { }
    }
}
