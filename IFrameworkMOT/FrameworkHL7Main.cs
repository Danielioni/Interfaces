using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using motInboundLib;
using NHapi.Model.V25.Segment;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Group;
using NHapi.Base;

namespace IFrameworkMOT
{
    public partial class FrameworkHL7Main : Form
    {
        private HL7Run __hl7_proc;

        public FrameworkHL7Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // This will start the listener and call the callback 
            Thread __worker = new Thread(new ThreadStart(__start));
            __worker.Name = "waiter";
            __worker.Start();

            textBox1.Text = "Started New thread";

        }

        public void __update_ui(string __msg)
        {
            textBox1.Text = __msg;
        }

        private void __start()
        {
            __hl7_proc = new HL7Run();
        }

        // MOT Override Functions
        public class HL7Run : HL7Message
        {
            private HL7Message __message;

            public HL7Run()
            {
                __message = new HL7Message();
            }

            public override void __process_drug_record(MFN_M15 m)
            {
                // Drug schedules are sometimes stored in Roman Numerals
                Dictionary<string, int> __roman = new Dictionary<string, int>();
                __roman.Add("I", 1);
                __roman.Add("II", 2);
                __roman.Add("III", 3);
                __roman.Add("IV", 4);
                __roman.Add("V", 5);
                __roman.Add("VI", 6);
                __roman.Add("VII", 7);
                __roman.Add("VIII", 8);
                __roman.Add("IX", 9);
                __roman.Add("X", 10);
            }

            private string __parse_repeat_pattern(string __pattern)
            {
                //  Val(D) (Daily), Code == QJ#
                //      1 = Mon, ...
                //      Example:    MWF QJ135
                //  Val(E) (Every x Days), Code = Q#D
                //      Example: Every 2nd Day Q2D
                //  Val(M) (Monthly), Code = QL#,#,...
                //      Example: 1st and the 15th of the month  QL1,15

                if (__pattern[0] != 'q' && __pattern[0] != 'Q')
                {
                    return null;
                }

                string __ret_pattern = "";
                int __index = 2;

                switch (__pattern[1])
                {
                    case 'j':
                    case 'J':
                        __ret_pattern = "J,";
                        for (__index = 2; __pattern[__index] != '\0'; __index++)   // Make J,1,2,3
                        {
                            __ret_pattern += (__pattern[__index] + ',');
                        }

                        break;

                    case 'l':
                    case 'L':
                        string[] __tok = __pattern.Split(',');

                        __ret_pattern = "L ";

                        while (__tok[__index] != null)                  // Make L,1,15
                        {
                            __ret_pattern += (__tok[__index] + ',');
                        }

                        break;

                    default:            // Assume Q#D
                        __ret_pattern = "D,";

                        __ret_pattern = __pattern[1].ToString(); // Make D,3
                        break;
                }

                return __ret_pattern;

            }

            public  void __process_prescription_record(RDE_O11 __rde_o_11)
            {
                // This record has a little of everything in it including facilities, prescribers, drugs, ...
                // The only required segments are MSH, ORC and OBX but it appears that RXE is heavily used

                //  '[Optional]'
                //  '{Repeatable}'
                // FrameworrkLTC defines MSH, [PID, [PV1]], 
                //                              {
                //                                  ORC,            -- Required
                //                                  [RXO, {RXR}], 
                //                                  RXE,            -- Required
                //                                  [{NTE}], 
                //                                  {TQ1},          -- Required
                //                                  {RXR},          -- Required
                //                                  [{RXC}]
                //                              }, 
                //                              [ZPI]

                try
                {
                    int __index = 0;
                    Dictionary<string, string> __order_type = new Dictionary<string, string>();
                    __order_type.Add("NW", "Add");
                    __order_type.Add("DC", "Change");
                    __order_type.Add("RF", "Add");
                    __order_type.Add("XO", "Change");
                    __order_type.Add("CA", "Delete");
                    __order_type.Add("RE", "Change");
/*
                    for (int i = 0; i < __rde_o_11.ORDERRepetitionsUsed; i++)
                    {
                        // Create record
                        RDE_O11_ORDER __order = __rde_o_11.GetORDER(i);


                        motPrescriptionRecord __scrip = new motPrescriptionRecord(__order_type[__order.ORC.OrderControl.Value]);

                        // Rx Number
                        __scrip.RxSys_RxNum = __order.RXE.PrescriptionNumber.Value;

                        // Patient ID (PID.2.1)
                        __scrip.RxSys_PatID = __rde_o_11.PATIENT.PID.PatientID.IDNumber.Value;

                        // Doctor ID (RXE.13.1) - Note, there can be multiples -- Concat + '\n';
                        {
                            XCN[] __tmpXCN = __order.RXE.GetOrderingProviderSDEANumber();
                            string __tmpS = null;

                            if (__order.RXE.OrderingProviderSDEANumberRepetitionsUsed > 1)
                            {
                                for (__index = 0; __index < __order.RXE.OrderingProviderSDEANumberRepetitionsUsed; __index++)
                                {
                                    __tmpS += __tmpXCN[__index].IDNumber.Value;
                                    __tmpS += '\n';
                                }

                                __scrip.RxSys_DocID = __tmpS;
                            }
                            else
                            {
                                __scrip.RxSys_DocID = __tmpXCN[0].IDNumber.Value;
                            }
                        }

                        // Drug ID (RXE.2.1)  -- Can be other than the NDC Number
                        __scrip.RxSys_DrugID = __order.RXE.GiveCode.Identifier.Value;


                        // Sig (RXE.7.2) - Note, there can be multiples -- Concat + '\n';
                        {
                            CE[] __tmpCE = __order.RXE.GetProviderSAdministrationInstructions();
                            string __tmpS = null;

                            if (__order.RXE.ProviderSAdministrationInstructionsRepetitionsUsed > 1)
                            {
                                for (__index = 0; __index < __order.RXE.ProviderSAdministrationInstructionsRepetitionsUsed; __index++)
                                {
                                    __tmpS += __tmpCE[__index].Text.Value;
                                    __tmpS += '\n';
                                }

                                __scrip.Sig = __tmpS;
                            }
                            else
                            {
                                __scrip.Sig = __tmpCE[0].Text.Value;
                            }
                        }

                        // Start & End Date (ORC.7.4, RXE.1.4)
                        {
                            RDE_O11_TIMING_ENCODED __tmpTQ1 = __order.GetTIMING_ENCODED();

                            // TQ1 Dates have trailing zeros for time and all we want is the date in YYYYMMDD
                            __scrip.RxStartDate = __tmpTQ1.TQ1.StartDateTime.Time.Value.Substring(0, 8);

                            // End Dates have an odd habit of being NULL
                            if (!string.IsNullOrEmpty(__tmpTQ1.TQ1.EndDateTime.Time.Value))
                                __scrip.RxStopDate = __tmpTQ1.TQ1.EndDateTime.Time.Value.Substring(0, 8);
                        }


                        // DC Date (RXE.1.5)
                        if (__order.ORC.OrderControl.Value == "DC")
                        {
                            __scrip.DiscontinueDate = __order.RXE.QuantityTiming.EndDateTime.Time.Value.Substring(0, 8); ;
                        }



                        // Comments (NTE.3)
                        {
                            NTE __tmpNTE;
                            string __tmpS = null;

                            if (__order.NTERepetitionsUsed > 1)
                            {
                                for (__index = 0; __index < __order.NTERepetitionsUsed; __index++)
                                {
                                    __tmpNTE = __order.GetNTE(__index);
                                    __tmpS += __tmpNTE.GetComment(__index).Value;
                                    __tmpS += '\n';
                                }

                                __scrip.Comments = __tmpS;
                            }
                            else
                            {
                                __tmpNTE = __order.GetNTE(0);
                                __scrip.Comments = __tmpNTE.GetComment(0).Value;
                            }
                        }

                        // Refills (RXE.12)
                        __scrip.Refills = __order.RXE.NumberOfRefills.Value;

                        // New Rx Number
                        // Isolate


                        // Rx Type (TQ1.3)
                        //
                        //  Val(D) (Daily), Code == QJ#
                        //      1 = Mon, ...
                        //      Example:    MWF QJ135
                        //  Val(E) (Every x Days), Code = Q#D
                        //      Example: Every 2nd Day Q2D
                        //  Val(M) (Monthly), Code = QL#,#,...
                        //      Example: 1st and the 15th of the month  QL1,15
                        //
                        //
                        //  Titration is managed by repeating TQ1's with different dosages and Daily Codes
                        //
                        //
                        // 
                        // TODO: RxType needs Conversion to MOT --  
                        //  MOT(0)  == Daily [Isolate vals - 0/1]           HL7(TQ1.3)  Code = QJ1
                        //  MOT(18) == Alternating [Isolate vals - 0/1]     HL7(TQ1.3)  Code = Q#D
                        //          MDOMStart is number of alternating days or 1 id empty
                        //  MOT(5)  == Day of Week  [Isolate vals - 0/1]    HL7(TQ1.3)  Code = QJ#######
                        //          DoW is number of alternating days or 1 id empty
                        //  MOT(8)  == Monthly Titrating [Isolate vals - 1]
                        //          MDOMStart is number of alternating days or 1 id empty, MDOMEnd is valid
                        //  MOT(9)  == Weekly Titrating [Isolate vals - 1]
                        //  MOT(2)  == PRN [Isolate vals - 1]
                        //  MOT(13) == Sequential [Isolate vals - 1]    
                        //  MOT(20) == Monthly TCustom [Isolate vals - 1]
                        //  MOT(21) == Weekly TCustom [Isolate vals -  1]

                        //  For Titrating (8 & 9) the __scrip.Special Doses field is set to List<string>(1..35) where each entry is formatted 0.00
                        //  where each entry describes a dose for the day e.g. {5.00,4.00,3.00,2.00,1.00,1.00,1.00}.  Dose time is set using {...}
                        //  
                        //  
                       

                        // MDOMStart - Set for Alternating, Day of Week, & Day of Month
                        // MDOMStop  - Set for Day of Month

                        // Dose Schedule Name (TQ1.3)
                        //
                        // Titration could be represented by  
                        //   TQ1.1 == "Set Name char(4)"
                        //   TQ1.7 == <YYYYMMDDHHMM>    -- Start Date
                        //     
                        //      {
                        //          TQ1.3 == <QJ#>
                        //          TQ1.2 == <DoseQty, Unit>
                        //          TQ1.4 == <HHMMSS>
                        //      }
                        //
                        //      Example:
                        //          {
                        //              {"QJ3", 5, "MG", "13:00"}   -- Starts Wednesday
                        //              {"QJ4", 4, "MG", "13:00"}
                        //              {"QJ5", 2, "MG", "13:00"}
                        //              {"QJ6", 2, "MG", "13:00"}
                        //              {"QJ7", 1, "MG", "13:00"}
                        //              {"QJ1", 1, "MG", "13:00"}
                        //              {"QJ2", 1, "MG", "13:00"}
                        //          }

                        {

                            int __index1 = 0;
                            int __index2 = 0;
                            int __scrip_type = 0;
                      
                            List<motTimeQtysRecord> __motTQ_list = new List<motTimeQtysRecord>();
                            motTimeQtysRecord __motTQ = new motTimeQtysRecord();


                            for (__index1 = 0; __index1 < __order.TIMING_ENCODEDRepetitionsUsed; __index1++)  // Number of TQ1's
                            {
                                RDE_O11_TIMING_ENCODED __TQ1 = __order.GetTIMING_ENCODED(__index1);
                                RPT[] __RPT = __TQ1.TQ1.GetRepeatPattern();

                                if (__RPT != null && (__RPT.Length > 0))
                                {
                                    foreach (RPT __rpt in __RPT)
                                    {
                                        __motTQ.DoseScheduleName = __rpt.RepeatPatternCode.Identifier.Value;

                                        string _s = __parse_repeat_pattern(__rpt.RepeatPatternCode.Identifier.Value);
                                        string[] _t = _s.Split(',');

                                        switch (_t[0][0])
                                        {
                                            case 'J':
                                                __scrip_type = 0;
                                                break;

                                            case 'L':
                                                __scrip_type = 20;
                                                break;

                                            case 'D':
                                                __scrip_type = 18;

                                                continue;

                                            default:
                                                break;
                                        }

                                        foreach (string _e in _t)
                                        {

                                        }
                                    }
                                }
                                else  // No Repeat Pattern defined
                                {
                                    string __times = "";
                                    string __dose = "";
                                    string __unit = "";
                                    string __start_date = "";
                                    string __end_date = "";
                                    string __message = "";

                                    string __mot_dosetime = "";

                                    for (int t = 0; t < __TQ1.TQ1.ExplicitTimeRepetitionsUsed; t++)
                                    {
                                        __times += __TQ1.TQ1.GetExplicitTime(t) + ",";
                                        __mot_dosetime += string.Format("{0:D4}{1:D2}", __TQ1.TQ1.GetExplicitTime(t), __TQ1.TQ1.Quantity.Quantity.Value);
                                    }

                                    __dose = __TQ1.TQ1.Quantity.Quantity.Value;
                                    __unit = __TQ1.TQ1.Quantity.Units.Identifier.Value;

                                    if(!string.IsNullOrEmpty(__TQ1.TQ1.StartDateTime.Time.Value))
                                        __start_date = __TQ1.TQ1.StartDateTime.Time.Value.Substring(0, 8);
                                    
                                    if (!string.IsNullOrEmpty(__TQ1.TQ1.EndDateTime.Time.Value))
                                        __end_date = __TQ1.TQ1.EndDateTime.Time.Value.Substring(0, 8);

                                    if (!string.IsNullOrEmpty(__TQ1.TQ1.TextInstruction.Value))
                                        __message = __TQ1.TQ1.TextInstruction.Value;



                                }                                                                                                                   
                            }
                        }

                        // Qty Per Dose (TQ1 2)
                        {
                            RDE_O11_TIMING_ENCODED __tmpQT1 = __order.GetTIMING_ENCODED();
                            __scrip.QtyPerDose = __tmpQT1.TQ1.Quantity.Quantity.Value;
                        }

                        // Qty Dispensed (RXE 10)
                        __scrip.QtyDispensed = __order.RXE.DispenseAmount.Value;

                        // Status
                        // DoW
                        // Special Doses
                        // Dose Time Qtys
                        // Chart Only
                        // Anchor Date

                    }*/
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed processing prescription record {0}", e.Message);
                }
            }

            public override void __process_prescriber_record()
            { }

            public override void __process_facility_record()
            { }

            public override void __process_store_record()
            { }

            public override void __process_time_qty_record()
            { }

            //public override void __process_patient_record(ADT_A01 __adt_a01)
            //{ }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MSH __msh = new MSH();
            ORC __orc = new ORC();
        }
    }
}
