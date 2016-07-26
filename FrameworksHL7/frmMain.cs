using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using motInboundLib;
using NHapi.Model.V25.Segment;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;

namespace FrameworksHL7
{
    public partial class HL7MainWindow : Form
    {
        public HL7MainWindow()
        {
            InitializeComponent();
        }

        private void HL7MainWindow_Load(object sender, EventArgs e)
        {
            // This will start the listener and call the callback 
            Thread __worker = new Thread(new ThreadStart(__start));
            __worker.Name = "waiter";
            __worker.Start();

            // Now get on with your life ...
         


        }

        private void __start()
        {
            HL7Run __run = new HL7Run();
        }
    
        // MOT Override Functions

        private void __process_drug_record()
        { }

        public class HL7Run : HL7Message
        {
            private HL7Message __message;

            public HL7Run()
            {
                __message = new HL7Message();
            }

            /// <summary>
            /// RXE Information
            ///     An RXE segment can appear immediately after an RXO segment, in which case the medication is attached to the prescription. 
            ///     If the RXE segment appears independently, it is used as a historical record of medications and is attached to the patient.
            ///     
            /// TQ1 Information
            ///     
            /// </summary>
            /// <param name="__rde_o_11"></param>
            public override void __process_prescription_record(RDE_O11 __rde_o_11)
            {
                // This record has a little of everything in it including facilities, prescribers, drugs, ...
                // The only required segments are MSH, ORC and OBX but it appears that RXE is heavily used

                try
                {
                    int __index = 0;

                    for (int i = 0; i < __rde_o_11.ORDERRepetitionsUsed; i++)
                    {
                        motPrescriptionRecord __scrip = new motPrescriptionRecord("Add");
                        var __order = __rde_o_11.GetORDER(i);

                        ///Rx Number
                        __scrip.RxSys_RxNum = __order.RXE.PrescriptionNumber.Value;

                        // Patient ID (PID 2-1)
                        __scrip.RxSys_PatID = __rde_o_11.PATIENT.PID.PatientID.IDNumber.Value;

                        // Doctor ID (RXE 13-1) - Note, there can be multiples -- Concat + '\n';
                        {
                            XCN[] __tmpXCN = __order.RXE.GetOrderingProviderSDEANumber();
                            string __tmpS = null;

                            for (__index = 0; __index < __order.RXE.OrderingProviderSDEANumberRepetitionsUsed; __index++)
                            {
                                __tmpS += __tmpXCN[__index].IDNumber.Value;
                                __tmpS += '\n';
                            }

                            __scrip.RxSys_DocID = __tmpS;
                        }

                        // Drug ID (RXE 2-1)
                        __scrip.RxSys_DrugID = __order.RXE.GiveCode.Identifier.Value;


                        // Sig (RXE 7.2) - Note, there can be multiples -- Concat + '\n';
                        {
                            CE[] __tmpCE = __order.RXE.GetProviderSAdministrationInstructions();
                            string __tmpS = null;

                            for (__index = 0; __index < __order.RXE.ProviderSAdministrationInstructionsRepetitionsUsed; __index++)
                            {
                                __tmpS += __tmpCE[__index].Text;
                                __tmpS += '\n';
                            }

                            __scrip.Sig = __tmpS;
                        }

                        // Start Date (ORC 7-4, RXE 1-4)
                        __scrip.RxStartDate = __order.RXE.QuantityTiming.StartDateTime.ToString();

                        // Stop Date (ORC 7-5, RXE 1-5)
                        __scrip.RxStopDate = __order.RXE.QuantityTiming.EndDateTime.ToString();

                        // DC Date
                        // Dose Schedule Name
                        // Comments

                        // Refills (RXE )
                        __scrip.Refills = __order.RXE.NumberOfRefills.Value;

                        // New Rx Number
                        // Isolate

                        // Rx Type (RXE 44) - MOT values are quite different and describe daily, DoW, DoM, Sequential, Titrating, ...   

                        // RXE -44 describes:
                        //      M	Medication
                        //      S IV Large Volume Solutions
                        //      O   Other solution as medication orders

                        __scrip.RxType = __order.RXE.PharmacyOrderType.Value;

                        // MDOMStart
                        // MDOMStop


                        // Qty Per Dose (RXO 23, RXE 1, TQ1 2)
                        __scrip.QtyPerDose = __order.RXE.QuantityTiming.Quantity.Quantity.Value;

                        // Qty Dispensed (RXE 10)
                        __scrip.QtyDispensed = __order.RXE.DispenseAmount.Value;

                        // Status
                        // DoW
                        // Special Doses
                        // Dose Time Qtys
                        // Chart Only
                        // Anchor Date
                    }
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

            public override void __process_patient_record(ADT_A01 __adt_a01)
            { }
        }

    }
}
