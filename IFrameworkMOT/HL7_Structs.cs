using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IFrameworkMOT
{
    /// <summary>
    /// Messsages to support for FrameworkLTE
    /// 
    ///     ADT^A04, ADT_A01
    ///     ADT^A08, ADT_A01
    ///     OMP^O09, OMP_O09
    ///     ADT^A23, ADT_A21
    ///     RDE^011, RDE_O11
    ///     RDS^O13, RDS_O13
    /// 
    ///     Grammer:
    ///         <CR> - Segment Terminator
    ///         |    - Field Separator
    ///         ^    - Component Separator
    ///         &    - Subcompuonent Separator
    ///         ~    - Repetition Seperator
    ///         \    - Escape Character
    ///         
    ///         []   - Optional  )
    ///         {}   - 1 or more repetitions
    ///         
    ///         OPT (Optionality Keys)
    ///             R - Required
    ///             O - Optional
    ///             C - Conditional on Trigger
    ///             X - Not used with this trigger
    ///             B - Left in for backward compatability
    ///             W - Withdrawn
    ///             
    /// </summary>

    public class ADT_A01    // Register (A04), Update (A08) a Patient
    {
        // A04 - MSH, EVN, PID, PV1, [{OBX}], [{AL1}], [{DG1}]
        // A08 - MSH, EVN, PID, PV1, [{OBX}], [{AL1}], [{DG1}]
        public MSH __msh;
        public EVN __evn;
        public PID __pid;
        public PV1 __pv1;
        public List<OBX> __obx;
        public List<AL1> __al1;
        public List<DG1> __dg1;

        public ADT_A01()
        {
            try
            {
                __msh = new MSH();
                __evn = new EVN();
                __pid = new PID();
                __pv1 = new PV1();
                __obx = new List<OBX>();
                __al1 = new List<AL1>();
                __dg1 = new List<DG1>();
            }
            catch
            { throw; }
        }
    }
    public class ADT_A21    // Delete a Patient
    {
        // A23 - Not in Spec
    }
    public class OMP_O09    // Pharmacy Treatment Order Message
    {
        // Control Code NW (New)            MSH, PID, [PV1], { ORC, [{TQ1}], [{RXR}], RXO, [{RXC}] }, [{NTE}]
        // Control Code DC (Discontinue)    MSH, PID, [PV1], { ORC, [{TQ1}], RXO, [{RXC}] }
        // Control Code RF (Refill)         MSH, PID, [PV1], { ORC, [{TQ1}], RXO, [{RXC}] }

        public MSH __msh;
        public PID __pid;
        public PV1 __pv1;

        public class Order
        {
            public ORC __orc;
            public List<TQ1> __tq1;
            public RXO __rxo;
            public List<RXC> __rxc;

            public Order()
            {
                try
                {
                    __orc = new ORC();
                    __tq1 = new List<TQ1>();
                    __rxo = new RXO();
                    __rxc = new List<RXC>();
                }
                catch
                { throw; }
            }
        }

        public List<NTE> __nte;
    }
    public class RDE_O11    // Pharmacy/Treatment Encoded Order Message
    {
        // Drug Order       MSH, [ PID, [PV1] ], { ORC, [RXO, {RXR}, RXE, [{NTE}], {TQ1}, {RXR}, [{RXC}] }, [ZPI]
        // Literal Order    MSH, PID, [PV1], ORC, [TQ1], [RXE], [ZAS]

        public MSH __msh;
        public PID __pid;
        public PV1 __pv1;
        public ORC __orc;
        public List<TQ1> __tq1;
        public List<RXE> __rxe;

        public class Order
        {
            public ORC __orc;
            public RXO __rxo;
            public List<RXR> __rxr;
            public RXE __rxe;
            public List<NTE> __nte;
            public List<TQ1> __tq1;
            public List<RXC> __rxc;

            public Order()
            {
                try
                {
                    __orc = new ORC();
                    __rxo = new RXO();
                    __rxr = new List<RXR>();
                    __rxe = new RXE();
                    __nte = new List<NTE>();
                    __tq1 = new List<TQ1>();
                    __rxc = new List<RXC>();
                }
                catch
                { throw; }
            }
        }

        public Order __order;
        public ZAS __zas;
        public ZPI __zpi;

        public RDE_O11()
        {
            try
            {
                __msh = new MSH();
                __pid = new PID();
                __pv1 = new PV1();
                __tq1 = new List<TQ1>();
                __rxe = new List<RXE>();
                __order = new Order();
                __zas = new ZAS();
                __zpi = new ZPI();
            }
            catch
            { throw; }
        }
    }
    public class RDS_O13    // Pharmacy/Treatment Dispense Message
    {
        // Dispense Msg      MSH, [ PID, [PV1] ], { ORC, [RXO], {RXE}, [{NTE}], {TQ1}, {RXR}, [{RXC}], RXD }, [ZPI] 

        public MSH __msh;
        public PID __pid;
        public PV1 __pv1;

        public class Order
        {
            public ORC __orc;
            public RXO __rxo;
            public RXE __rxe;
            public RXD __rxd;
            public List<NTE> __nte;
            public List<TQ1> __tq1;
            public List<RXR> __rxr;
            public List<RXC> __rxc;


            public Order()
            {
                try
                {
                    __orc = new ORC();
                    __rxo = new RXO();
                    __rxe = new RXE();
                    __nte = new List<NTE>();
                    __tq1 = new List<TQ1>();
                    __rxr = new List<RXR>();
                    __rxc = new List<RXC>();
                    __rxd = new RXD();
                }
                catch
                { throw; }
            }
        }

        public Order __order;
        public ZPI __zpi;

        public RDS_O13()
        {
            try
            {
                __msh = new MSH();
                __pid = new PID();
                __pv1 = new PV1();
                __order = new Order();
                __zpi = new ZPI();
            }
            catch
            { throw; }
        }
    }
    public class DataPair
    {
        public string __tag;
        public object __data;

        public DataPair(string __a)
        {
            __tag = __a;
        }

        public DataPair(string __a, object __d)
        {
            __tag = __a;
            __data = __d;
        }

        public DataPair(string __a, string __d)
        {
            __tag = __a;
            __data = __d;
        }

        public DataPair()
        {
            __tag = string.Empty;
            __data = null;
        }
    }
    public class AL1
    {

        public DataPair __1 = new DataPair(@"Set ID", @"AL1");
        public DataPair __2 = new DataPair(@"Allergen Type Code" );
        public DataPair __3 = new DataPair(@"Allergen Code/Mnemonic/Description" );
        public DataPair __4 = new DataPair(@"Allergen Severity Code" );
        public DataPair __5 = new DataPair(@"Allergen Reaction Code" );
        public DataPair __6 = new DataPair(@"Identification Date" );

        public AL1()
        { }
    }
    public class DG1
    {
        public DataPair __1 = new DataPair(@"Set ID", @"DG1" );
        public DataPair __2 = new DataPair(@"Diagnosis Coding Method");
        public DataPair __3 = new DataPair(@"Diagnosis Code");
        public DataPair __4 = new DataPair(@"Diagnosis Description");

        public DataPair __5 = new DataPair(@"Diagnosis Date/Time");
        public DataPair __6 = new DataPair(@"Diagnosis Type");
        public DataPair __7 = new DataPair(@"Diagnostic Major Catagory");
        public DataPair __8 = new DataPair(@"Diagnostic Related Group");
        public DataPair __9 = new DataPair(@"DRG Approval Indicator");
        public DataPair __10 = new DataPair(@"DRG Grouper Review Code");
        public DataPair __11 = new DataPair(@"Outlier Type");
        public DataPair __12 = new DataPair(@"Outlier Days");
        public DataPair __13 = new DataPair(@"Outlier Cost");
        public DataPair __14 = new DataPair(@"Grouper Version and Type");
        public DataPair __15 = new DataPair(@"Diagnosis Priority");
        public DataPair __16 = new DataPair(@"Diagnosing Clinician");
        public DataPair __17 = new DataPair(@"Diagnosis Classification");
        public DataPair __18 = new DataPair(@"Confidential Indicator");
        public DataPair __19 = new DataPair(@"Attestation Date/Time");
        public DataPair __20 = new DataPair(@"Diagnosis Identifier");
        public DataPair __21 = new DataPair(@"Diagnosis Action Code");

        public DG1()
        { }
    }
    public class EVN
    {
        public DataPair __1 = new DataPair(@"Event Type Code");
        public DataPair __2 = new DataPair(@"Recorded Date/Time");
        public DataPair __3 = new DataPair(@"Date/Time Planned Event");
        public DataPair __4 = new DataPair(@"Event Reason Code");
        public DataPair __5 = new DataPair(@"Operator ID");
        public DataPair __6 = new DataPair(@"Event Occured");
        public DataPair __7 = new DataPair(@"Event Facility");

        public EVN()
        { }
    }
    public class MSH
    {
        public DataPair __1 = new DataPair(@"Field Separator");
        public DataPair __2 = new DataPair(@"Encoding Characters");

        public DataPair __3 = new DataPair(@"Sending Application");
        public DataPair __3_1 = new DataPair(@"Namesppace ID");
        public DataPair __3_2 = new DataPair(@"Universal ID");
        public DataPair __3_3 = new DataPair(@"Universal ID Type;");

        public DataPair __4 = new DataPair(@"Sending Facility");
        public DataPair __4_1 = new DataPair(@"Namesppace ID");
        public DataPair __4_2 = new DataPair(@"Universal ID");
        public DataPair __4_3 = new DataPair(@"Universal ID Type;");

        public DataPair __5 = new DataPair(@"Receiving Application");
        public DataPair __5_1 = new DataPair(@"Namesppace ID");
        public DataPair __5_2 = new DataPair(@"Universal ID");
        public DataPair __5_3 = new DataPair(@"Universal ID Type;");

        public DataPair __6 = new DataPair(@"Receiving Facility");
        public DataPair __6_1 = new DataPair(@"Namesppace ID");
        public DataPair __6_2 = new DataPair(@"Universal ID");
        public DataPair __6_3 = new DataPair(@"Universal ID Type;");

        public DataPair __7 = new DataPair(@"Date/Time of Message");
        public DataPair __7_1 = new DataPair(@"Time");
        public DataPair __7_2 = new DataPair(@"Degree of Percision");

        public DataPair __8 = new DataPair(@"Security");

        public DataPair __9 = new DataPair(@"Message Type");
        public DataPair __9_1 = new DataPair(@"Message Code");
        public DataPair __9_2 = new DataPair(@"Trigger Event");
        public DataPair __9_3 = new DataPair(@"Message Structure");

        public DataPair __10 = new DataPair(@"Message Control ID");

        public DataPair __11 = new DataPair(@"Processing ID");
        public DataPair __11_1 = new DataPair(@"Processing ID");
        public DataPair __11_2 = new DataPair(@"Processing Mode");

        public DataPair __12 = new DataPair(@"Version ID");
        public DataPair __12_1 = new DataPair(@"Version ID");
        public DataPair __12_2 = new DataPair(@"Internationalization Code");
        public DataPair __12_2_1 = new DataPair(@"Identifier");
        public DataPair __12_2_2 = new DataPair(@"Text");
        public DataPair __12_2_3 = new DataPair(@"Name of Coding System");
        public DataPair __12_2_4 = new DataPair(@"Alternate Identifier");
        public DataPair __12_2_5 = new DataPair(@"Alternate Text");
        public DataPair __12_2_6 = new DataPair(@"NAme of Alternate Coding System");
        public DataPair __12_3 = new DataPair(@"International Version ID");
        public DataPair __12_3_1 = new DataPair(@"Text");
        public DataPair __12_3_2 = new DataPair(@"Name of Coding System");
        public DataPair __12_3_3 = new DataPair(@"Alternate Identifier");
        public DataPair __12_3_4 = new DataPair(@"Alternate Text");
        public DataPair __12_3_5 = new DataPair(@"Name of Alternate Coding System");

        public DataPair __13 = new DataPair(@"Sequence Number");
        public DataPair __14 = new DataPair(@"Continuation Pointer");
        public DataPair __15 = new DataPair(@"Accept Acknowledgement Type");
        public DataPair __16 = new DataPair(@"Application Acknowledgement Type");
        public DataPair __17 = new DataPair(@"Country Code");
        public DataPair __18 = new DataPair(@"Character Set");

        public DataPair __19 = new DataPair(@"Principal Language of Message");
        public DataPair __19_1 = new DataPair(@"Identifier");
        public DataPair __19_2 = new DataPair(@"Text");
        public DataPair __19_3 = new DataPair(@"Name of Coding System");
        public DataPair __19_4 = new DataPair(@"Alternate Identifier");
        public DataPair __19_5 = new DataPair(@"Alternate Text");
        public DataPair __19_6 = new DataPair(@"Name of Alternate Coding System");

        public DataPair __20 = new DataPair(@"Alternate Character Set Handling Scheme");
        public DataPair __21 = new DataPair(@"Message Profile Identifier");
        public DataPair __21_1 = new DataPair(@"Entity Identifier");
        public DataPair __21_2 = new DataPair(@"Namespace Identifier");
        public DataPair __21_3 = new DataPair(@"Universal ID");
        public DataPair __21_4 = new DataPair(@"Universal ID Type");

        public MSH()
        { }
    }
    public class NTE
    {
        public DataPair __1 = new DataPair(@"Set ID", "NTE");
        public DataPair __2 = new DataPair(@"Source of Comment");
        public DataPair __3 = new DataPair(@"Comment");
        public DataPair __4 = new DataPair(@"Comment Type");

        public NTE()
        { }
    }
    public class OBX
    {
        public DataPair __1 = new DataPair(@"Set ID", "OBX");
        public DataPair __2 = new DataPair(@"Value Type");

        public DataPair __3 = new DataPair(@"Observation Indicator");
        public DataPair __3_1 = new DataPair(@"Identifier");
        public DataPair __3_2 = new DataPair(@"Text");
        public DataPair __3_3 = new DataPair(@"Name of Coding System");
        public DataPair __3_4 = new DataPair(@"Alternate Identifier");
        public DataPair __3_5 = new DataPair(@"Alternate Text");
        public DataPair __3_6 = new DataPair(@"Name of Alternate Coding System");

        public DataPair __4 = new DataPair(@"Observation Sub ID");
        public DataPair __5 = new DataPair(@"Observation Value");
        public DataPair __6 = new DataPair(@"Units");

        public DataPair __6_1 = new DataPair(@"Identifier");
        public DataPair __6_2 = new DataPair(@"Text");
        public DataPair __6_3 = new DataPair(@"Name of Coding System");
        public DataPair __6_4 = new DataPair(@"Alternate Identifier");
        public DataPair __6_5 = new DataPair(@"Alternate Text");
        public DataPair __6_6 = new DataPair(@"Name of Alternate Coding System");

        public DataPair __7 = new DataPair(@"Reference Range");
        public DataPair __8 = new DataPair(@"Abnormal Flags");
        public DataPair __9 = new DataPair(@"Probability");
        public DataPair __10 = new DataPair(@"Nature of Abnormal Test");
        public DataPair __11 = new DataPair(@"Observation Result Status");

        public DataPair __12 = new DataPair(@"Effective Date of Reference Range");
        public DataPair __12_1 = new DataPair(@"Time");
        public DataPair __12_2 = new DataPair(@"Degree of Percision");
        
        public DataPair __13 = new DataPair(@"User Defined Access Checks");

        public DataPair __14 = new DataPair(@"Date/Time of the Observation");
        public DataPair __14_1 = new DataPair(@"Time");
        public DataPair __14_2 = new DataPair(@"Degree of Percision");

        public DataPair __15 = new DataPair(@"Producer's ID");
        public DataPair __15_1 = new DataPair(@"Identifier");
        public DataPair __15_2 = new DataPair(@"Text");
        public DataPair __15_3 = new DataPair(@"Name of Coding System");
        public DataPair __15_4 = new DataPair(@"Alternate Identifier");
        public DataPair __15_5 = new DataPair(@"Alternate Text");
        public DataPair __15_6 = new DataPair(@"Name of Alternate Coding System");

        public DataPair __16 = new DataPair(@"Responsible Observer");
        // ...

        public DataPair __17 = new DataPair(@"Observation Method");
        // ...

        public DataPair __18 = new DataPair(@"Equipment Instance Identifier");
        // ...

        public DataPair __19 = new DataPair(@"Date/Time of the Analysis");
        // ...


        public OBX()
        { }
    }
    public class ORC
    {
        public DataPair __1 = new DataPair(@"ID");
        public DataPair __2 = new DataPair(@"Placer Order Number");
        public DataPair __3 = new DataPair(@"Filler Order Number");
        public DataPair __4 = new DataPair(@"Placer Group Number");
        public DataPair __5 = new DataPair(@"Order Status");
        public DataPair __6 = new DataPair(@"Response Flag");
        public DataPair __7 = new DataPair(@"Quantity/Timing");
        public DataPair __8 = new DataPair(@"Parent");
        public DataPair __9 = new DataPair(@"Date/Time of Transaction");
        public DataPair __10 = new DataPair(@"Entered By");
        public DataPair __11 = new DataPair(@"Verified By");
        public DataPair __12 = new DataPair(@"Ordering Provider");
        public DataPair __13 = new DataPair(@"Enterer's Location");
        public DataPair __14 = new DataPair(@"Call Back Phone Number");
        public DataPair __15 = new DataPair(@"Order Effective Date/Time");
        public DataPair __16 = new DataPair(@"Order Control Reason");
        public DataPair __17 = new DataPair(@"Entering Organization");
        public DataPair __18 = new DataPair(@"Entering Device");
        public DataPair __19 = new DataPair(@"Action By");
        public DataPair __20 = new DataPair(@"Advanced Beneficiary Notice Code");
        public DataPair __21 = new DataPair(@"Ordering Facility Name");
        public DataPair __22 = new DataPair(@"Ordering Facility Address");
        public DataPair __23 = new DataPair(@"Ordering Facility Phone Number");
        public DataPair __24 = new DataPair(@"Ordering Provider Address");
        public DataPair __25 = new DataPair(@"Order Status Modifier");
        public DataPair __26 = new DataPair(@"Advanced Benificiary Notice Override Reason");
        public DataPair __27 = new DataPair(@"Filler's Expected Availability Date/Time");
        public DataPair __28 = new DataPair(@"Confidentiality Code");
        public DataPair __29 = new DataPair(@"Order Type");
        public DataPair __30 = new DataPair(@"Enterer Authorization Mode");

        public ORC()
        { }
    }
    public class PID
    {
        public DataPair __1 = new DataPair(@"Set ID", "PID");
        public DataPair __2 = new DataPair(@"Patient ID");
        public DataPair __3 = new DataPair(@"Patient Identifier List");
        public DataPair __4 = new DataPair(@"Alternate Pathient ID - PID");
        public DataPair __5 = new DataPair(@"Patient Name");
        public DataPair __6 = new DataPair(@"Mothers Maiden Name");
        public DataPair __7 = new DataPair(@"Date/Time of Birth");
        public DataPair __8 = new DataPair(@"Administrative Sex");
        public DataPair __9 = new DataPair(@"Patient Alias");
        public DataPair __10 = new DataPair(@"Race");
        public DataPair __11 = new DataPair(@"Patient Address");
        public DataPair __12 = new DataPair(@"County Code");
        public DataPair __13 = new DataPair(@"Phone Number - Home");
        public DataPair __14 = new DataPair(@"Phone Number - Business");
        public DataPair __15 = new DataPair(@"Primary Language");
        public DataPair __16 = new DataPair(@"Marital Status");
        public DataPair __17 = new DataPair(@"Religion");
        public DataPair __18 = new DataPair(@"Patient Account Number");
        public DataPair __19 = new DataPair(@"SSN - Patient");
        public DataPair __20 = new DataPair(@"Drivers License Number - Patient");
        public DataPair __21 = new DataPair(@"Mother's Identifier");
        public DataPair __22 = new DataPair(@"Ethnic Group");
        public DataPair __23 = new DataPair(@"Birth Place");
        public DataPair __24 = new DataPair(@"Multiple Birth Indicator");
        public DataPair __25 = new DataPair(@"Birth Order");
        public DataPair __26 = new DataPair(@"Citizenship");
        public DataPair __27 = new DataPair(@"Veterens Military Status");
        public DataPair __28 = new DataPair(@"Nationality");
        public DataPair __29 = new DataPair(@"Patient Death Date/Time");
        public DataPair __30 = new DataPair(@"Patient Deth Indictor");
        public DataPair __31 = new DataPair(@"Identity Unknown Indicator");
        public DataPair __32 = new DataPair(@"Identity Reliability Code");
        public DataPair __33 = new DataPair(@"Last Update Date/Time");
        public DataPair __34 = new DataPair(@"Last Update Facility");
        public DataPair __35 = new DataPair(@"Species Code");
        public DataPair __36 = new DataPair(@"Breed Code");
        public DataPair __37 = new DataPair(@"Strain");
        public DataPair __38 = new DataPair(@"Production Class Code");
        public DataPair __39 = new DataPair(@"Tribal Citizenship");

        public PID()
        { }
    }
    public class PV1
    {
        public DataPair __1 = new DataPair(@"Set ID", "OBX");
        public DataPair __2 = new DataPair(@"Value Type");
        public DataPair __3 = new DataPair(@"Observation Indicator");
        public DataPair __4 = new DataPair(@"Observation Sub ID");
        public DataPair __5 = new DataPair(@"Observation Value");
        public DataPair __6 = new DataPair(@"Units");
        public DataPair __7 = new DataPair(@"Reference Range");
        public DataPair __8 = new DataPair(@"Abnormal Flags");
        public DataPair __9 = new DataPair(@"Probability");
        public DataPair __10 = new DataPair(@"Nature of Abnormal Test");
        public DataPair __11 = new DataPair(@"Set ID", "OBX");
        public DataPair __12 = new DataPair(@"Value Type");
        public DataPair __13 = new DataPair(@"Observation Indicator");
        public DataPair __14 = new DataPair(@"Observation Sub ID");
        public DataPair __15 = new DataPair(@"Observation Value");
        public DataPair __16 = new DataPair(@"Units");
        public DataPair __17 = new DataPair(@"Reference Range");
        public DataPair __18 = new DataPair(@"Abnormal Flags");
        public DataPair __19 = new DataPair(@"Probability");
        public DataPair __20 = new DataPair(@"Nature of Abnormal Test");
        public DataPair __21 = new DataPair(@"Set ID", "OBX");
        public DataPair __22 = new DataPair(@"Value Type");
        public DataPair __23 = new DataPair(@"Observation Indicator");
        public DataPair __24 = new DataPair(@"Observation Sub ID");
        public DataPair __25 = new DataPair(@"Observation Value");
        public DataPair __26 = new DataPair(@"Units");
        public DataPair __27 = new DataPair(@"Reference Range");
        public DataPair __28 = new DataPair(@"Abnormal Flags");
        public DataPair __29 = new DataPair(@"Probability");
        public DataPair __30 = new DataPair(@"Nature of Abnormal Test");
        public DataPair __31 = new DataPair(@"Set ID", "OBX");
        public DataPair __32 = new DataPair(@"Value Type");
        public DataPair __33 = new DataPair(@"Observation Indicator");
        public DataPair __34 = new DataPair(@"Observation Sub ID");
        public DataPair __35 = new DataPair(@"Observation Value");
        public DataPair __36 = new DataPair(@"Units");
        public DataPair __37 = new DataPair(@"Reference Range");
        public DataPair __38 = new DataPair(@"Abnormal Flags");
        public DataPair __39 = new DataPair(@"Probability");
        public DataPair __40 = new DataPair(@"Nature of Abnormal Test");
        public DataPair __41 = new DataPair(@"Set ID", "OBX");
        public DataPair __42 = new DataPair(@"Value Type");
        public DataPair __43 = new DataPair(@"Observation Indicator");
        public DataPair __44 = new DataPair(@"Observation Sub ID");
        public DataPair __45 = new DataPair(@"Observation Value");
        public DataPair __46 = new DataPair(@"Units");
        public DataPair __47 = new DataPair(@"Reference Range");
        public DataPair __48 = new DataPair(@"Abnormal Flags");
        public DataPair __49 = new DataPair(@"Probability");
        public DataPair __50 = new DataPair(@"Nature of Abnormal Test");

        public PV1()
        { }
    }
    public class RXC
    {
        public DataPair __1 = new DataPair(@"RX Component Type");

        public DataPair __2 = new DataPair(@"Component Code");
        public DataPair __2_1 = new DataPair(@"Identifier");
        public DataPair __2_2 = new DataPair(@"Text");
        public DataPair __2_3 = new DataPair(@"Name of Coding System");
        public DataPair __2_4 = new DataPair(@"Alternate Identifier");
        public DataPair __2_5 = new DataPair(@"Alternate Text");
        public DataPair __2_6 = new DataPair(@"Name of Alternate CodingSystem");

        public DataPair __3 = new DataPair(@"Component Amount");

        public DataPair __4 = new DataPair(@"Component Units");
        public DataPair __4_1 = new DataPair(@"Identifier");
        public DataPair __4_2 = new DataPair(@"Text");
        public DataPair __4_3 = new DataPair(@"Name of Coding System");
        public DataPair __4_4 = new DataPair(@"Alternate Identifier");
        public DataPair __4_5 = new DataPair(@"Alternate Text");
        public DataPair __4_6 = new DataPair(@"NAme of Alternate Coding System");

        public DataPair __5 = new DataPair(@"Component Strength");

        public DataPair __6 = new DataPair(@"Component Strength Units");
        public DataPair __6_1 = new DataPair(@"Identifier");
        public DataPair __6_2 = new DataPair(@"Text");
        public DataPair __6_3 = new DataPair(@"Name of Coding System");
        public DataPair __6_4 = new DataPair(@"Alternate Identifier");
        public DataPair __6_5 = new DataPair(@"Alternate Text");
        public DataPair __6_6 = new DataPair(@"NAme of Alternate Coding System");

        public DataPair __7 = new DataPair(@"Suplementary Code");
        public DataPair __7_1 = new DataPair(@"Identifier");
        public DataPair __7_2 = new DataPair(@"Text");
        public DataPair __7_3 = new DataPair(@"Name of Coding System");
        public DataPair __7_4 = new DataPair(@"Alternate Identifier");
        public DataPair __7_5 = new DataPair(@"Alternate Text");
        public DataPair __7_6 = new DataPair(@"Name of Alternate Coding System");

        public DataPair __8 = new DataPair(@"Component Drug Volume");

        public DataPair __9 = new DataPair(@"Component Drug Volume Units");
        public DataPair __9_1 = new DataPair(@"Identifier");
        public DataPair __9_2 = new DataPair(@"Text");
        public DataPair __9_3 = new DataPair(@"Name of Coding System");
        public DataPair __9_4 = new DataPair(@"Alternate Identifier");
        public DataPair __9_5 = new DataPair(@"Alternate Text");
        public DataPair __9_6 = new DataPair(@"Name of Alternate Coding System");
        public DataPair __9_7 = new DataPair(@"Coding System Version ID");
        public DataPair __9_8 = new DataPair(@"Alternate Coding System Version");
        public DataPair __9_9 = new DataPair(@"Original Text");

        public RXC()
        { }
    }
    public class RXD { }
    public class RXE { }
    public class RXO { }
    public class RXR { }
    public class TQ1 { }
    public class ZAS { }
    public class ZPI { }

}
;

namespace IFrameworkMOT
{
    class HL7_Structs
    {

    }
}
