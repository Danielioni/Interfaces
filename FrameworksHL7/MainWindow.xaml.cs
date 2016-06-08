using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NHapi.Base.Parser;
using NHapi.Base.Model;
using NHapi.Model.V251.Message;
using System.Threading;

using motInboundLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameworksHL7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //string message = @"MSH|^~\&|CohieCentral|COHIE|Clinical Data Provider|TCH|20060228155525||QRY^R02^QRY_R02|1|P|2.5.1|QRD|20060228155525|R|I||||10^RD&Records&0126|38923^^^^^^^^&TCH|||";
            //string message = @"MSH|^~\&|3rd Party Interface|SNM|FrameworkLTC|PDC|20090121161123||ZMA|179545|P|2.5||||||ASCII||| ZMA|L|Console14|K200|00182145310|549|1|10|96531|Test Guy";
            string message = @"MSH|^~\&|3rd Party Interface|SNM|FrameworkLTC|PDC|20110214162636||MFN^M15^MFN_M15|179547|P|2.5||||||ASCII||| MFI|INV^Inventory Master File||UPD|||MFE|MUP|||00039006013|CEIIM|00039006013^LASIX TAB 40MG||||^AVENTIS|PHR\F\PDC\F\DEFAULT\F\DEFAULT||-100||||||| MFE|MUP|||00039006013|CEIIM|00039006013^LASIX TAB 40MG||||^AVENTIS|OSS\F\Console14\F\K200||100|||||||";
            try
            {
                //PipeParser parser = new PipeParser();
                //IMessage m = parser.Parse(message);

                //MFN_M15 n = m as MFN_M15;

                //Assert.IsNotNull(qryR02);
                //Assert.AreEqual("38923", qryR02.QRD.GetWhoSubjectFilter(0).IDNumber.Value);
                HL7SocketListener hsl = new HL7SocketListener(5000);

                hsl.start();

            }
            catch(Exception e)
            {
                Console.WriteLine("{0}", e.Message);
            }

            try
            {
                HL7Rest __input = new HL7Rest();

                while (true)
                {
                    switch (__input.waitForTrigger())
                    {
                        case RecordType.Drug:
                            __input.getDrugRecord();
                            break;

                        case RecordType.Location:
                            __input.getLocationRecord();
                            break;

                        case RecordType.Patient:
                            __input.getPatientRecord();
                            break;

                        case RecordType.Prescriber:
                            __input.getPrescriberRecord();
                            break;

                        case RecordType.Prescription:
                            __input.getPrescriptionRecord();
                            break;

                        case RecordType.Store:
                            __input.getStoreRecord();
                            break;

                        case RecordType.TimeQty:
                            __input.getTimeQtyRecord();
                            break;

                        case RecordType.Unkown:
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }

    public enum RecordType
    {
        Drug,
        Location,
        Patient,
        Prescription,
        Prescriber,
        Store,
        TimeQty,
        Unkown
    }

    public class HL7Rest : httpInputSource
    {
        private string __siteRoot = "";

        public RecordType waitForTrigger()
        {
            //
            // Framework will send the following message types
            //  1. ADT
            //  2. MFN
            //  3. RDE (Drug)
            //  4. RDE (Literal)
            //  5. RDS
            //

            Thread.Sleep(5000);
            Console.WriteLine("Ho Hum: {0}", Thread.CurrentThread.Name);
            return RecordType.Unkown;
        }

        public override motDrugRecord getDrugRecord()
        {
            motDrugRecord __drug = new motDrugRecord("Add");
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                __xTable.Add("", "RxSys_DrugID");
                __xTable.Add("", "LblCode");
                __xTable.Add("", "ProdCode");
                __xTable.Add("", "TradeName");
                __xTable.Add("", "Strength");
                __xTable.Add("", "Unit");
                __xTable.Add("", "RxOtc");
                __xTable.Add("", "DoseForm");
                __xTable.Add("", "Route");
                __xTable.Add("", "DrugSchedule");
                __xTable.Add("", "VisualDescription");
                __xTable.Add("", "DrugName");
                __xTable.Add("", "ShortName");
                __xTable.Add("", "NDCNum");
                __xTable.Add("", "SizeFactor");
                __xTable.Add("", "Template");
                __xTable.Add("", "ConsultMsg");
                __xTable.Add("", "GenericFor");


                var __record = getJsonRecord(__siteRoot, @"api/drug/1");

                if (__record != null)
                {
                    PipeParser parser = new PipeParser();
                    IMessage __message = parser.Parse(Convert.ToString(__record));


                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Drug data " + e.Message);
            }

            return base.getDrugRecord();
        }

        public override motLocationRecord getLocationRecord()
        {
            motLocationRecord __location = new motLocationRecord();
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                // Load the translaton table -- Database Column Name to Gateway Tag Name                
                __xTable.Add("", "RxSys_LocID");
                __xTable.Add("", "RxSys_StoreID");
                __xTable.Add("", "LocationName");
                __xTable.Add("", "Address1");
                __xTable.Add("", "Address2");
                __xTable.Add("", "City");
                __xTable.Add("", "State");
                __xTable.Add("", "Zip");
                __xTable.Add("", "Phone");
                __xTable.Add("", "Comments");
                __xTable.Add("", "CycleDays");
                __xTable.Add("", "CycleType");

                var __record = getJsonRecord(__siteRoot, @"api/location/1");

                if (__record != null)
                {
                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Location data " + e.Message);
            }

            return base.getLocationRecord();
        }

        public override motPatientRecord getPatientRecord()
        {
            motPatientRecord __patient = new motPatientRecord();
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                // Load the translaton table -- Database Column Name to Gateway Tag Name  
                __xTable.Add("", "RxSys_PatID");
                __xTable.Add("", "LastName");
                __xTable.Add("", "FirstName");
                __xTable.Add("", "MiddleInitial");
                __xTable.Add("", "Address1");
                __xTable.Add("", "Address2");
                __xTable.Add("", "City");
                __xTable.Add("", "State");
                __xTable.Add("", "Zip");
                __xTable.Add("", "Phone1");
                __xTable.Add("", "Phone2");
                __xTable.Add("", "WorkPhone");
                __xTable.Add("", "RxSys_LocID");
                __xTable.Add("", "Room");
                __xTable.Add("", "Comments");
                __xTable.Add("", "CycleDate");
                __xTable.Add("", "CycleDays");
                __xTable.Add("", "CycleType");
                __xTable.Add("", "Status");
                __xTable.Add("", "RxSys_LastDoc");
                __xTable.Add("", "RxSys_PrimaryDoc");
                __xTable.Add("", "RxSys_AltDoc");
                __xTable.Add("", "SSN");
                __xTable.Add("", "Allergies");
                __xTable.Add("", "Diet");
                __xTable.Add("", "DxNotes");
                __xTable.Add("", "TreatmentNotes");
                __xTable.Add("", "DOB");
                __xTable.Add("", "Height");
                __xTable.Add("", "Weight");
                __xTable.Add("", "ResponsibleName");
                __xTable.Add("", "InsName");
                __xTable.Add("", "InsPNo");
                __xTable.Add("", "AltInsName");
                __xTable.Add("", "AltInsPNo");
                __xTable.Add("", "MCareNum");
                __xTable.Add("", "MCaidNum");
                __xTable.Add("", "AdmitDate");
                __xTable.Add("", "ChartOnly");
                var __record = getJsonRecord(__siteRoot, @"api/Patient/1");

                if (__record != null)
                {
                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Patient data " + e.Message);
            }
            return base.getPatientRecord();
        }

        public override motPrescriberRecord getPrescriberRecord()
        {
            motPrescriberRecord __prescriber = new motPrescriberRecord();
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                __xTable.Add("", "RxSys_DocID");
                __xTable.Add("", "LastName");
                __xTable.Add("", "FirstName");
                __xTable.Add("", "Address1");
                __xTable.Add("", "Address2");
                __xTable.Add("", "City");
                __xTable.Add("", "Zip");
                __xTable.Add("", "Phone");
                __xTable.Add("", "Comments");
                __xTable.Add("", "DEA_ID");
                __xTable.Add("", "TPID");
                __xTable.Add("", "Speciality");

                var __record = getJsonRecord(__siteRoot, @"api/Prescriber/1");

                if (__record != null)
                {
                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Prescriber data " + e.Message);
            }
            return base.getPrescriberRecord();
        }

        public override motPrescriptionRecord getPrescriptionRecord()
        {
            motPrescriptionRecord __scrip = new motPrescriptionRecord();
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                // Load the translaton table -- Database Column Name to Gateway Tag Name                
                __xTable.Add("", "RxSys_RxNum");
                __xTable.Add("", "RxSys_PatID");
                __xTable.Add("", "RxSys_DocID");
                __xTable.Add("", "RxSys_DrugID");
                __xTable.Add("", "Sig");
                __xTable.Add("", "RxStartDate");
                __xTable.Add("", "RxStopDate");
                __xTable.Add("", "DiscontinueDate");
                __xTable.Add("", "DoseScheduleName");
                __xTable.Add("", "Comments");
                __xTable.Add("", "Refills");
                __xTable.Add("", "RxSys_NewRxNum");
                __xTable.Add("", "Isolate");
                __xTable.Add("", "RxType");
                __xTable.Add("", "MDOMStart");
                __xTable.Add("", "MDOMEnd");
                __xTable.Add("", "QtyPerDose");
                __xTable.Add("", "QtyDispensed");
                __xTable.Add("", "Status");
                __xTable.Add("", "DoW");
                __xTable.Add("", "SpecialDoses");
                __xTable.Add("", "DoseTimeQtys");
                __xTable.Add("", "ChartOnly");
                __xTable.Add("", "AnchorDate");

                var __record = getJsonRecord(__siteRoot, @"api/Prescription/1");

                if (__record != null)
                {
                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Prescription data " + e.Message);
            }

            return base.getPrescriptionRecord();
        }

        public override motStoreRecord getStoreRecord()
        {
            motStoreRecord __store = new motStoreRecord();
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                // Load the translaton table -- Database Column Name to Gateway Tag Name                
                __xTable.Add("", "RxSys_StoreID");
                __xTable.Add("", "StoreName");
                __xTable.Add("", "Address1");
                __xTable.Add("", "Address2");
                __xTable.Add("", "City");
                __xTable.Add("", "State");
                __xTable.Add("", "Zip");
                __xTable.Add("", "Phone");
                __xTable.Add("", "Fax");
                __xTable.Add("", "DEANum");

                var __record = getJsonRecord(__siteRoot, @"api/store/1");

                if (__record != null)
                {
                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Store data: " + e.Message);
            }

            return base.getStoreRecord();
        }

        public override motTimeQtysRecord getTimeQtyRecord()
        {
            motTimeQtysRecord __tq = new motTimeQtysRecord();
            Dictionary<string, string> __xTable = new Dictionary<string, string>();

            try
            {
                // Load the translaton table -- Database Column Name to Gateway Tag Name                
                __xTable.Add("", "RxSys_LocID");
                __xTable.Add("", "DoseScheduleName");
                __xTable.Add("", "DoseTimeQtys");

                var __record = getJsonRecord(__siteRoot, @"api/TQ/1");

                if (__record != null)
                {
                    // Got something, now transform it to what we need
                    // SourceRecord s = <SourceRecord> __record;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get Time/Quantity data: " + e.Message);
            }

            
            return base.getTimeQtyRecord();
        }

        public HL7Rest(string __rootPath)
        {
            __siteRoot = __rootPath;
        }

        public HL7Rest() { }
    }
}
