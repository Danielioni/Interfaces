using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;


namespace motCommonLib
{

    /// <summary>
    /// HL7 - Grammer:
    /// 
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

    /// <summary>
    /// Messsages to support for FrameworkLTE
    /// 
    ///     ADT^A04, ADT_A01
    ///     ADT^A08, ADT_A01
    ///     
    ///     OMP^O09, OMP_O09
    ///     ADT^A23, ADT_A21
    ///     
    ///     RDE^011, RDE_O11
    ///     
    ///         v2.7.1 Spec
    ///         Header:     MSH, [{SFT}], [UAC], [{NTE}], 
    ///         Patient:        [PID, [PD1], [{PRT}],[{NTE}], 
    ///         Visit:              [PV1, [PV2], [{PRT}]],
    ///         Insurance:          [IN1, [IN2], [IN3]],
    ///         Misc:               [GT1], [{AL1}]],
    ///         Order:          [{ORC,
    ///         Timing:             [{TQ1, [{TQ2}]}],
    ///         Detail:             [RXO, [{NTE}], {RXR},
    ///         Component:              [{RXC, [{NTE}]}],
    ///                             [{PRT}], RXE, [{PRT}],[{NTE}],
    ///         Timing Encoded:         [{TQ1, [{TQ2}]}],
    ///                             {RXR}, [{RXC}],
    ///         Observation:            [{OBX, [{PRT}],[{NTE}]}],
    ///                             [{FT1}], [BLG], [{CTI}] }]
    ///         
    ///     FrameworkLTC Spec
    ///     -----------------
    ///     RDE^O11^RDE_O11  - DO(Drug Order)
    ///         Header:     MSH, 
    ///         Patient:        [PID,[PV1]],
    ///         Order:          {ORC,[RXO,{RXR}],RXE,[{NTE},{TQ1},{RXR},[{RXC]},
    ///         Custom:         [ZPI]
    ///
    ///     RDE^O11^RDE_O11  - LO(Literal Order)
    ///         Header:     MSH, 
    ///         Patient:        PID, [PV1],
    ///         Order:          ORC,[TQ1],[RXE],
    ///         Custom:         [ZAS]
    ///        
    ///     -- RDE Special Interpretations
    ///         PID-2:          3rd Party Patient ID [CX]
    ///         PID-3:          FW Patient ID - FacilityID\F\PatientID
    ///         PID-4:          Alternate Patient ID
    /// 
    ///     EPIC Spec
    ///     ----------
    ///     RDE^O11
    ///         Header:     MSH,PID,CON,
    ///         Order:          {ORC,[RXE,RXD,ZLB,PRT,OBX]}
    ///                    
    ///     Response:       ORR,MSH,MSA,[ERR]
    ///     
    ///     --- RDE Special Interpretations
    ///         PID-2:       CX :: Backward Compatability Only
    ///         PID-3:       ID^^^SA^assigning athority-ID2^^^SA2^Assigning Athourity
    ///            
    ///     RDS^O13, RDS_O13
    ///     
    /// 
    /// </summary>

    /// <summary>
    /// Messages to support for EPIC
    /// </summary>



    public class HL7MessageParser
    {

        public bool __sub_parse(string __tagname, string __message, char __delimiter, Dictionary<string, string> __message_data, int __minor)
        {
            char[] __local_delimiters = { '\\', '&', '~', '^' };
            string[] __field_names = __message.Split(__delimiter);

            __minor = 1;

            foreach (string __field in __field_names)
            {
                int __gotone = __field.IndexOfAny(__local_delimiters);

                if (__gotone > 0)
                {
                    string __tmp_tagname = __tagname;

                    __tagname = __tagname + "." + __minor.ToString();
                    __sub_parse(__tagname, __field, __field[__gotone], __message_data, __minor); // recurse

                    __tagname = __tmp_tagname;

                    continue;
                }

                __message_data.Add(__tagname + "." + __minor.ToString(), __field.TrimStart(' ').TrimEnd(' '));
                __minor++;
            }

            return true;
        }
        public void __parse(string __message, Dictionary<string, string> __message_data)
        {
            char[] __delims = { '|' };
            char[] __local_delimiters = { '\\', '&', '~', '^' };

            int __major = 0;
            int __minor = 1;
            int __gotone;

            string[] __field_names = __message.Split(__delims);
            string __tagname = __field_names[0].TrimStart(' ').TrimEnd(' ');
            int i = 0;

            while (i < __field_names.Length)
            {
                // Trim out all the whitespace
                __field_names[i] = __field_names[i].TrimStart(' ').TrimEnd(' ');
                string __field = __field_names[i++];

                // Catch the Root Name  {RXE,RXE}
                if (__major == 0)
                {
                    __message_data.Add(__tagname, __field);
                    __major++;
                    continue;
                }

                // Doah!  MSH requires something special
                if (__tagname == "MSH" && __major < 3)
                {
                    if (__major < 3)
                    {
                        __message_data.Add("MSH.1", "|");
                        __message_data.Add("MSH.2", @"^~\&");
                        __major = 3;
                        continue;
                    }
                }

                __message_data.Add(__tagname + "." + __major.ToString(), __field);

                if (__field != @"^~\&")
                {
                    __gotone = __field.IndexOfAny(__local_delimiters);    // Subfield ADT01^RDE^RDE_011
                    if (__gotone != -1)
                    {
                        if (__field.IndexOf('~') > 0)
                        {
                            char[] __tilde = { '~' };
                            string[] __tilde_split = __field.Split(__tilde);
                            var __tmp_field_name = __field_names[0];

                            foreach (string __split in __tilde_split)
                            {
                                __tagname = __tagname + "." + __major.ToString();
                                __sub_parse(__tagname, __split, __split[__field.IndexOfAny(__local_delimiters)], __message_data, __minor);
                                __tagname = "~" + __tmp_field_name;
                                __tmp_field_name = __tagname;  // Add '~', 1 for each iteration.  We'll decode them later
                            }
                        }
                        else
                        {
                            __tagname = __tagname + "." + __major.ToString();

                            // Wait for it so we don't loose the overall seque
                            bool __success = __sub_parse(__tagname, __field, __field[__gotone], __message_data, __minor);

                            // Reset the tagname to the root level
                            __tagname = __field_names[0];
                        }
                    }
                }

                __major++;
            }
        }

        public HL7MessageParser() { }
    }

    public class HL7_Element_Base
    {
        public Dictionary<string, string> __field_names;
        public Dictionary<string, string> __msg_data;

        protected XDocument __data;

        public string Get(string __key)
        {
            if (string.IsNullOrEmpty(__key))
            {
                return string.Empty;
            }

            string __result;

            __result = (from elem in __data.Descendants(__key) select elem.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(__result))
            {
                if (__key.Substring(__key.Length - 2, 2) == ".1")
                {
                    // Try without the .1 first,  if its empty, try the full key
                    __result = (from elem in __data.Descendants(__key.Substring(0, __key.Length - 2)) select elem.Value).FirstOrDefault();
                    if (!string.IsNullOrEmpty(__result))
                    {
                        return __result;
                    }
                }                
            }

            return __result;
        }

        public List<string> GetList(string __key)
        {
            if (string.IsNullOrEmpty(__key))
            {
                return null;
            }

            var __result = (from elem in __data.Descendants(__key) select elem.Value).ToList();
                      
            return __result;
        }

        protected string[] __clear_newlines(string[] __strings)
        {
            for (int i = 0; i < __strings.Length; i++)
            {
                if (__strings[i].IndexOf("\n") != -1)
                {
                    __strings[i] = __strings[i].Substring(1);
                }
            }

            return __strings;
        }

        public HL7_Element_Base(XElement __xe)
        {
            __data = new XDocument(__xe);
        }

        public HL7_Element_Base()
        {
            __field_names = new Dictionary<string, string>();
            __msg_data = new Dictionary<string, string>();
        }

        ~HL7_Element_Base()
        {
            //__field_names.Clear();
            //__msg_data.Clear();
        }
    }

    //
    // Messages - Specific to Softwriters FrameworkLTE
    //
    public class ADT_A01 : HL7_Element_Base   // Register (A04), Update (A08) a Patient
    {
        // A04 - MSH, EVN, PID, PV1, [{OBX}], [{AL1}], [{DG1}]
        // A08 - MSH, EVN, PID, PV1, [{OBX}], [{AL1}], [{DG1}]

        // Note that IN1 seems to show up with ADT_A04

        public MSH __msh;
        public EVN __evn;
        public PID __pid;
        public PV1 __pv1;
        public PV2 __pv2;
        public PR1 __pr1;
        public UAC __uac;
        public PD1 __pd1;

        public GT1 __gt1;
        public List<OBX> __obx;
        public List<AL1> __al1;
        public List<ROL> __rol;
        public List<DG1> __dg1;
        public List<SFT> __sft;
    
        public List<DB1> __db1;

        public List<Dictionary<string, string>> __message_store;

        public ADT_A01(XDocument __xdoc) : base()
        {
            __obx = new List<OBX>();
            __al1 = new List<AL1>();
            __dg1 = new List<DG1>();
            __rol = new List<ROL>();
            __db1 = new List<DB1>();
            __sft = new List<SFT>();

            foreach (XElement __xe in __xdoc.Root.Elements())
            {
                switch (__xe.Name.ToString())
                {
                    case "AL1":
                        __al1.Add(new AL1(__xe));
                        break;

                    case "SFT":
                        __sft.Add(new SFT(__xe));
                        break;

                    case "UAC":
                        __uac = new UAC(__xe);
                        break;

                    case "EVN":
                        __evn = new EVN(__xe);
                        break;

                    case "DG1":
                        __dg1.Add(new DG1(__xe));
                        break;

                    case "DB1":
                        __db1.Add(new DB1(__xe));
                        break;

                    case "GT1":
                        __gt1 = new GT1(__xe);
                        break;

                    case "MSH":
                        __msh = new MSH(__xe);
                        break;

                    case "OBX":
                        __obx.Add(new OBX(__xe));
                        break;

                    case "PID":
                        __pid = new PID(__xe);
                        break;

                    case "PD1":
                        __pd1 = new PD1(__xe);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__xe);
                        break;

                    case "PV2":
                        __pv2 = new PV2(__xe);
                        break;

                    case "ROL":
                        __rol.Add(new ROL(__xe));
                        break;

                    default:
                        break;
                }
            }
        }

        public ADT_A01() : base()
        {
        }
    }
    public class ADT_A21 : HL7_Element_Base   // Delete a Patient
    {
        public List<Dictionary<string, string>> __message_store;

        // A23 - Not in Spec
        public ADT_A21()
        {
            throw new NotImplementedException();
        }

    }
    public class OMP_O09 : HL7_Element_Base   // Pharmacy Treatment Order Message
    {
        // Control Code NW (New)            MSH, PID, [PV1], { ORC, [{TQ1}], [{RXR}], RXO, [{RXC}] }, [{NTE}]
        // Control Code DC (Discontinue)    MSH, PID, [PV1], { ORC, [{TQ1}], RXO, [{RXC}] }
        // Control Code RF (Refill)         MSH, PID, [PV1], { ORC, [{TQ1}], RXO, [{RXC}] }

        public MSH __msh;
        public List<Order> __orders;
        public Patient __patient;
        public Header __header;

        public OMP_O09(XDocument __xdoc) : base()
        {

            string __last_significant_item = "NONE";

            __header = new Header();
            __orders = new List<Order>();
            __patient = new Patient();
            Order __current_order = new Order();

            __data = __xdoc;

            foreach (XElement __xe in __xdoc.Root.Elements())
            {
                switch (__xe.Name.ToString())
                {
                    case "AL1":
                        __patient.__al1.Add(new AL1(__xe));
                        break;

                    case "MSH":
                        __header.__msh = new MSH(__xe);
                        __last_significant_item = "MSH";
                        break;

                    case "OBX":
                        __current_order.__obx.Add(new OBX(__xe));
                        break;

                    case "PID":
                        __patient.__pid = new PID(__xe);
                        __last_significant_item = "PID";
                        break;

                    case "PV1":
                        __patient.__pv1 = new PV1(__xe);
                        __last_significant_item = "PV1";
                        break;

                    case "PV2":
                        __patient.__pv2 = new PV2(__xe);
                        __last_significant_item = "PV2";
                        break;

                    case "PD1":
                        __patient.__pd1 = new PD1(__xe);
                        break;

                    case "IN1":
                        __patient.__in1 = new IN1(__xe);
                        break;

                    case "IN2":
                        __patient.__in2 = new IN2(__xe);
                        break;

                    case "TQ1":
                        __current_order.__tq1.Add(new TQ1(__xe));
                        break;

                    case "RXC":
                        __current_order.__rxc.Add(new RXC(__xe));
                        break;

                    case "RXE":
                        __current_order.__rxe = new RXE(__xe);
                        __last_significant_item = "RXE";
                        break;

                    case "RXO":
                        __current_order.__rxo = new RXO(__xe);
                        break;

                    case "RXR":
                        __current_order.__rxr.Add(new RXR(__xe));
                        break;

                    case "ORC":  // Need to parse the order       
                        if (!__current_order.Empty()) // Is this a new order
                        {
                            __orders.Add(__current_order);
                            __current_order = null;
                        }

                        __current_order = new Order();
                        __current_order.__orc = new ORC(__xe);
                        __last_significant_item = "ORC";
                        break;

                    case "PRT":

                        if (__last_significant_item == "PID" || __last_significant_item.Contains("PV"))
                        {
                            __patient.__prt.Add(new PRT(__xe));
                        }
                        else
                        {
                            __current_order.__prt.Add(new PRT(__xe));
                        }

                        break;

                    case "ZAS":
                        __header.__zas = new ZAS(__xe);
                        break;

                    case "ZLB":
                        __header.__zlb = new ZLB(__xe);
                        break;

                    case "ZPI":
                        __header.__zpi = new ZPI(__xe);
                        break;

                    case "NTE":
                        if (__last_significant_item == "PID")
                        {
                            __patient.__nte.Add(new NTE(__xe));
                        }
                        else if (__last_significant_item == "MSH")
                        {
                            __header.__nte.Add(new NTE(__xe));
                        }
                        else
                        {
                            __current_order.__nte.Add(new NTE(__xe));
                        }
                        break;

                    case "DG1":
                        __patient.__dg1.Add(new DG1(__xe));
                        break;

                    case "SFT":
                        __header.__sft.Add(new SFT(__xe));
                        break;

                    default:
                        break;
                }
            }


            if (__current_order != null)
            {
                __orders.Add(__current_order);
            }
        }
    }
    /*
        RDE^O11^RDE_O11  - DO(Drug Order)
            MSH, [PID,[PV1],{ORC,[RXO, RXR], RXE, [{NTE}], {TQ1}, RXR, [{RXC]} ]}

        RDE^O11^RDE_O11  - LO(Literal Order)
            MSH, PID, [PV1],ORC,[TQ1],[RXE]
    */

    // RDE_O11 Order - {ORC,[RXO,{RXR}],RXE,[{NTE}],{TQ1},{RXR},[{RXC]}]}
    public class Order // Triggered by a new ORC
    {
        public ORC __orc;
        public RXO __rxo;
        public RXE __rxe;
        public List<RXR> __rxr;
        public List<NTE> __nte;
        public List<TQ1> __tq1;
        public List<RXC> __rxc;
        public List<OBX> __obx;
        public List<PRT> __prt;
        public List<FT1> __ft1;

        public Order()
        {
            try
            {
                //__orc = new ORC();
                //__rxo = new RXO();
                //__rxe = new RXE();
                __rxr = new List<RXR>();
                __nte = new List<NTE>();
                __tq1 = new List<TQ1>();
                __rxc = new List<RXC>();
                __obx = new List<OBX>();
                __prt = new List<PRT>();
                __ft1 = new List<FT1>();
            }
            catch
            { throw; }

        }

        public bool Empty()
        {
            
            if (__orc == null )
            {
                return true;
            }
            
            return false;
        }
    }
    public class Patient
    {
        public PID __pid;
        public PD1 __pd1;
        public List<PRT> __prt;
        public List<NTE> __nte;
        public List<AL1> __al1;
        public List<DG1> __dg1;
        public List<CX1> __cx1;
        public List<OBX> __obx;
        public PV1 __pv1;
        public PV2 __pv2;
        public IN1 __in1;
        public IN2 __in2;

        public Patient()
        {
            try
            {
                __prt = new List<PRT>();
                __nte = new List<NTE>();
                __al1 = new List<AL1>();
                __dg1 = new List<DG1>();
                __cx1 = new List<CX1>();
                __obx = new List<OBX>();
            }
            catch
            { throw; }
        }

        public bool Empty()
        {
            /*
            if (__pid.__msg_data.Count == 0 &&
                __pd1.__msg_data.Count == 0 &&
                __pv1.__msg_data.Count == 0 &&
                __pv2.__msg_data.Count == 0 &&
                __in1.__msg_data.Count == 0 &&
                __in2.__msg_data.Count == 0 &&
                __dg1.Count == 0 &&
                __prt.Count == 0 &&
                __nte.Count == 0 &&
                __al1.Count == 0)
            {
                return true;
            }
            */
            return false;

        }
    }
    public class Header
    {

        public MSH __msh;
        public List<SFT> __sft;
        public UAC __uac;
        public List<NTE> __nte;

        // Hangers on
        public ZAS __zas;
        public ZLB __zlb;
        public ZPI __zpi;

        public Header()
        {
            try
            {
                __sft = new List<SFT>();
                __nte = new List<NTE>();
            }
            catch
            { throw; }
        }
    }
    public class RDE_O11 : HL7_Element_Base  // Pharmacy/Treatment Encoded Order Message
    {
        // FrameworkLTC
        //  Drug Order       MSH, [ PID, [PV1] ], { ORC, [RXO, {RXR}, RXE, [{NTE}], {TQ1}, {RXR}, [{RXC}] }, [ZPI]
        //  Literal Order    MSH, PID, [PV1], ORC, [TQ1], [RXE], [ZAS]

        public MSH __msh;
        public ZAS __zas;           // FrameworksLTC
        public ZLB __zlb;           // Epic
        public ZPI __zpi;           // FrameworksLTC

        public Header __header;
        public List<Order> __orders;
        public Patient __patient;

        public RDE_O11() : base()
        {
        }

        public RDE_O11(XDocument __xdoc)
        {
            string __last_significant_item = "NONE";

            __header = new Header();
            __orders = new List<Order>();
            __patient = new Patient();

            Order __current_order = new Order();

            __data = __xdoc;

            foreach (XElement __xe in __xdoc.Root.Elements())
            {
                switch (__xe.Name.ToString())
                {
                    case "AL1":
                        __patient.__al1.Add(new AL1(__xe));
                        break;

                    case "MSH":
                        __header.__msh = new MSH(__xe);
                        __last_significant_item = "MSH";
                        break;

                    case "OBX":
                        __current_order.__obx.Add(new OBX(__xe));
                        break;

                    case "PID":
                        __patient.__pid = new PID(__xe);
                        __last_significant_item = "PID";
                        break;

                    case "PV1":
                        __patient.__pv1 = new PV1(__xe);
                        __last_significant_item = "PV1";
                        break;

                    case "PV2":
                        __patient.__pv2 = new PV2(__xe);
                        __last_significant_item = "PV2";
                        break;

                    case "PD1":
                        __patient.__pd1 = new PD1(__xe);
                        break;

                    case "PRT":
                        
                        if (__last_significant_item == "PID" || __last_significant_item.Contains("PV"))
                        {
                            __patient.__prt.Add(new PRT(__xe));
                        }
                        else
                        {
                            __current_order.__prt.Add(new PRT(__xe));
                        }
                        
                        break;

                    case "IN1":
                        __patient.__in1 = new IN1(__xe);
                        break;

                    case "IN2":
                        __patient.__in2 = new IN2(__xe);
                        break;

                    case "TQ1":
                        __current_order.__tq1.Add(new TQ1(__xe));
                        break;

                    case "RXC":
                        __current_order.__rxc.Add(new RXC(__xe));
                        break;

                    case "RXE":
                        __current_order.__rxe = new RXE(__xe);
                        __last_significant_item = "RXE";
                        break;

                    case "RXO":
                        __current_order.__rxo = new RXO(__xe);
                        break;

                    case "RXR":
                        __current_order.__rxr.Add(new RXR(__xe));
                        break;

                    case "ORC":  // Need to parse the order       
                        if (!__current_order.Empty()) // Is this a new order
                        {
                            __orders.Add(__current_order);
                            __current_order = null;
                        }

                        __current_order = new Order();
                        __current_order.__orc = new ORC(__xe);
                        __last_significant_item = "ORC";
                        break;

                    case "ZAS":
                        __header.__zas = new ZAS(__xe);
                        break;

                    case "ZLB":
                        __header.__zlb = new ZLB(__xe);
                        break;

                    case "ZPI":
                        __header.__zpi = new ZPI(__xe);
                        break;

                    case "NTE":
                        if (__last_significant_item == "PID")
                        {
                            __patient.__nte.Add(new NTE(__xe));
                        }
                        else if (__last_significant_item == "MSH")
                        {
                            __header.__nte.Add(new NTE(__xe));
                        }
                        else
                        {
                            __current_order.__nte.Add(new NTE(__xe));
                        }
                        break;

                    case "DG1":
                        __patient.__dg1.Add(new DG1(__xe));
                        break;

                    case "SFT":
                        __header.__sft.Add(new SFT(__xe));
                        break;

                    default:
                        break;
                }
            }


            if (__current_order != null)
            {
                __orders.Add(__current_order);
            }
        }
    }
    public class RDS_O13 : HL7_Element_Base   // Pharmacy/Treatment Dispense Message
    {
        // Dispense Msg      MSH, [ PID, [PV1] ], { ORC, [RXO], {RXE}, [{NTE}], {TQ1}, {RXR}, [{RXC}], RXD }, [ZPI] 

        public MSH __msh;
        public ZPI __zpi;
        public List<Order> __orders;
        public Patient __patient;
        public Header __header;

        public RDS_O13(XDocument __xdoc) : base()
        {
            string __last_significant_item = "NONE";

            __orders = new List<Order>();
            __header = new Header();
            __patient = new Patient();

            Order __current_order = new Order();

            foreach (XElement __xe in __xdoc.Root.Elements())
            {
                switch (__xe.Name.ToString())
                {
                    case "AL1":
                        __patient.__al1.Add(new AL1(__xe));
                        break;

                    case "MSH":
                        __header.__msh = new MSH(__xe);
                        __last_significant_item = "MSH";
                        break;

                    case "OBX":
                        __current_order.__obx.Add(new OBX(__xe));
                        break;

                    case "PID":
                        __patient.__pid = new PID(__xe);
                        __last_significant_item = "PID";
                        break;

                    case "PV1":
                        __patient.__pv1 = new PV1(__xe);
                        __last_significant_item = "PV1";
                        break;

                    case "PV2":
                        __patient.__pv2 = new PV2(__xe);
                        __last_significant_item = "PV2";
                        break;

                    case "PD1":
                        __patient.__pd1 = new PD1(__xe);
                        break;

                    case "PRT":
                        
                        if (__last_significant_item == "PID" || __last_significant_item.Contains("PV"))
                        {
                            __patient.__prt.Add(new PRT(__xe));
                        }
                        else
                        {
                            __current_order.__prt.Add(new PRT(__xe));
                        }
                        
                        break;

                    case "IN1":
                        __patient.__in1 = new IN1(__xe);
                        break;

                    case "IN2":
                        __patient.__in2 = new IN2(__xe);
                        break;

                    case "TQ1":
                        __current_order.__tq1.Add(new TQ1(__xe));
                        break;

                    case "RXC":
                        __current_order.__rxc.Add(new RXC(__xe));
                        break;

                    case "RXE":
                        __current_order.__rxe = new RXE(__xe);
                        __last_significant_item = "RXE";
                        break;

                    case "RXO":
                        __current_order.__rxo = new RXO(__xe);
                        break;

                    case "RXR":
                        __current_order.__rxr.Add(new RXR(__xe));
                        break;

                    case "ORC":  // Need to parse the order       
                        if (!__current_order.Empty()) // Is this a new order
                        {
                            __orders.Add(__current_order);
                            __current_order = null;
                        }

                        __current_order = new Order();
                        __current_order.__orc = new ORC(__xe);
                        __last_significant_item = "ORC";
                        break;

                    case "ZAS":
                        __header.__zas = new ZAS(__xe);
                        break;

                    case "ZLB":
                        __header.__zlb = new ZLB(__xe);
                        break;

                    case "ZPI":
                        __header.__zpi = new ZPI(__xe);
                        break;

                    case "NTE":
                        if (__last_significant_item == "PID")
                        {
                            __patient.__nte.Add(new NTE(__xe));
                        }
                        else if (__last_significant_item == "MSH")
                        {
                            __header.__nte.Add(new NTE(__xe));
                        }
                        else
                        {
                            __current_order.__nte.Add(new NTE(__xe));
                        }
                        break;

                    case "DG1":
                        __patient.__dg1.Add(new DG1(__xe));
                        break;

                    case "SFT":
                        __header.__sft.Add(new SFT(__xe));
                        break;

                    default:
                        break;
                }
            }

            if (__current_order != null)
            {
                __orders.Add(__current_order);
            }
        }

        public RDS_O13() : base()
        { }
    }

    //
    //  Segments - These all parse as SEG-#,-#... and stored as a Dictioary<string,string>
    //

    public class FT1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        public void __load()
        {
        }

        public FT1(XElement __xe) : base(__xe)
        {

        }

        public FT1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }

    public class AL1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        public void __load()
        {
        }
        public AL1(XElement __xe) : base(__xe)
        {
        }
        public AL1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class EVN : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public EVN(XElement __xe) : base(__xe)
        {

        }
        public EVN() : base()
        {
            __load();
        }

        public EVN(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class DB1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public DB1() : base()
        {
            __load();
        }
        public DB1(XElement __xe) : base(__xe)
        {
        }
    }
    public class DG1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public DG1() : base()
        {
            __load();
        }
        public DG1(XElement __xe) : base(__xe)
        {
        }
        public DG1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class CX1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public CX1() : base()
        {
            __load();
        }
        public CX1(XElement __xe) : base(__xe)
        {
        }
        public CX1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class GT1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public GT1() : base()
        {
            __load();
        }
        public GT1(XElement __xe) : base(__xe)
        {

        }
        public GT1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class IIM : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public IIM() : base()
        {
            __load();
        }
        public IIM(XElement __xe) : base(__xe)
        {
        }
        public IIM(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class IN1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public IN1() : base()
        {
            __load();
        }
        public IN1(XElement __xe) : base(__xe)
        {

        }
        public IN1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class IN2 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public IN2() : base()
        {
            __load();
        }
        public IN2(XElement __xe) : base(__xe)
        {
        }
        public IN2(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class MSA : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();
    
        private void __load()
        {
        }
        public MSA() : base()
        {
            __load();
        }
        public MSA(XElement __xe) : base(__xe)
        {
        }
      
        public MSA(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class MSH : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();
        bool __parsed_list = false;

        private void __load()
        {
        }
        public MSH()
        {
            __load();
        }
        public new string Get(string __key)
        {
            if (__parsed_list)
            {
                string __tmp = string.Empty;
                __msg_data.TryGetValue(__key, out __tmp);
                return __tmp;
            }
            else
            {
                return base.Get(__key);
            }
        }
        public MSH(XElement __xe) : base(__xe)
        {
            __parsed_list = false;
            //__data = new XDocument(__xe);
        }
        public MSH(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
            __parsed_list = true;

            //
            // Wierdness - In MSH-9 there can be 2 or 3 items for example |ADT^AO1| or |RDE^O11^RDE_O11|. It looks like if there
            //             is no MSH-9-3, 9-1 and 9-2 need to be commbined to make 9-3. So, the strategy should be to combine 1 & 2 into 3 
            //             and if there's a 3, it will overwrite it.
            string __tmp = string.Empty;

            if (!__msg_data.TryGetValue("MSH.9.3", out __tmp))
            {
                __msg_data.Add("MSH.9.3", __msg_data["MSH.9.1"] + "_" + __msg_data["MSH.9.2"]);
            }
        }

        public string __full_message()
        {
            //string __out = @"MSH|^~\&||MOT_HL7Gateway|MOT_HL7Gateway|";
            string __whole_thing = string.Empty;

            var __enumerator = __msg_data.GetEnumerator();

            while (true)
            {
                try
                {
                    __enumerator.MoveNext();
                    __whole_thing += __enumerator.Current.Value + "|";
                }
                catch
                { break; }
            }

            return __whole_thing;
        }

        public string __ack_string()
        {
            return null;
        }

        public string __nak_string()
        {
            return null;
        }

    }
    public class NK1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public NK1() : base()
        {
            __load();
        }
        public NK1(XElement __xe) : base(__xe)
        {

        }
        public NK1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class NTE : HL7_Element_Base
    {
        // public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>()
        //public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public NTE(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
        public NTE(XElement __xe) : base(__xe)
        {

        }
        public NTE() : base()
        {
            __load();
        }
    }
    public class OBX : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public OBX() : base()
        {
            __load();
        }
        public OBX(XElement __xe) : base(__xe)
        {
        }
        public OBX(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ORC : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public ORC() : base()
        {
            __load();
        }
        public ORC(XElement __xe) : base(__xe)
        {
        }
        public ORC(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PID : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public PID()
        {
            __load();
        }

        public PID(XElement __xe) : base(__xe)
        {
        }
        public PID(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PR1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public PR1() : base()
        {
            __load();
        }
        public PR1(XElement __xe) : base(__xe)
        {
        }
        public PR1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PV1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        // TODO:  Finish Field Names
        private void __load()  // TODO:  Finish Patient Visit Fields
        {

        }
        public PV1() : base()
        {
            __load();
        }
        public PV1(XElement __xe) : base(__xe)
        {
        }
        public PV1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PV2 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        // TODO:  Finish Field Names
        private void __load()  // TODO:  Finish Patient Visit Fields
        {

        }
        public PV2() : base()
        {
            __load();
        }
        public PV2(XElement __xe) : base(__xe)
        {
        }
        public PV2(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PRT : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        // TODO:  Finish Field Names
        private void __load()  // TODO:  Finish Patient Visit Fields
        {

        }
        public PRT() : base()
        {
            __load();
        }
        public PRT(XElement __xe) : base(__xe)
        {
        }
        public PRT(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class SFT : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        // TODO:  Finish Field Names
        private void __load()  // TODO:  Finish Patient Visit Fields
        {

        }
        public SFT() : base()
        {
            __load();
        }
        public SFT(XElement __xe) : base(__xe)
        {
        }
        public SFT(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class UAC : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        // TODO:  Finish Field Names
        private void __load()  // TODO:  Finish Patient Visit Fields
        {

        }
        public UAC() : base()
        {
            __load();
        }
        public UAC(XElement __xe) : base(__xe)
        {
        }
        public UAC(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ROL : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public ROL() : base()
        {
            __load();
        }
        public ROL(XElement __xe) : base(__xe)
        {
        }
        public ROL(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PD1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public PD1() : base()
        {
            __load();
        }
        public PD1(XElement __xe) : base(__xe)
        {
        }
        public PD1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXC : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {

        }
        public RXC() : base()
        {
            __load();
        }
        public RXC(XElement __xe) : base(__xe)
        {
        }
        public RXC(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXD : HL7_Element_Base
    {

        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXD() : base()
        {
        }
        public RXD(XElement __xe) : base(__xe)
        {
        }
        public RXD(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXE : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXE() : base()
        {
        }
        public RXE(XElement __xe) : base(__xe)
        {
        }
        public RXE(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXO : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXO() : base()
        {
        }
        public RXO(XElement __xe) : base(__xe)
        {
        }
        public RXO(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXR : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();


        private void __load()
        {
        }
        public RXR(XElement __xe) : base(__xe)
        {
        }
        public RXR() : base()
        {
        }

        public RXR(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class TQ1 : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public TQ1() : base()
        {
        }
        public TQ1(XElement __xe) : base(__xe)
        {
        }
        public TQ1(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZAS : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public ZAS() : base()
        {
        }
        public ZAS(XElement __xe) : base(__xe)
        {
        }
        public ZAS(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZLB : HL7_Element_Base   // Epic Specific
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public ZLB(XElement __xe) : base(__xe)
        {
        }
        public ZLB() : base()
        {
        }

        public ZLB(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZPI : HL7_Element_Base  // FrameworksLTC  Specific
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public ZPI(XElement __xe) : base(__xe)
        {
        }
        public ZPI() : base()
        {
        }

        public ZPI(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }

    public class ZFI : HL7_Element_Base
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }
        public ZFI(XElement __xe) : base(__xe)
        {
        }
        public ZFI() : base()
        {
        }

        public ZFI(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ACK
    {
        public string __ack_string { get; set; } = string.Empty;
        public string __clean_ack_string { get; set; } = string.Empty;
        XDocument __xdoc;

        public void Build(MSH __msh, string __org, string __proc)
        {
            if (string.IsNullOrEmpty(__org))
            {
                __org = "Unknown";
            }

            if (string.IsNullOrEmpty(__proc))
            {
                __proc = "Unknown";
            }

            MSH __tmp_msh = __msh;

            string __time_stamp = DateTime.Now.ToString("yyyyMMddhh");

            __tmp_msh.__msg_data["MSH.5"] = __tmp_msh.__msg_data["MSH.3"];
            __tmp_msh.__msg_data["MSH.6"] = __tmp_msh.__msg_data["MSH.4"];

            __tmp_msh.__msg_data["MSH.3"] = __proc;
            __tmp_msh.__msg_data["MSH.4"] = __org;

            __ack_string = '\x0B' +
                           @"MSH|^~\&|" +
                           __tmp_msh.__msg_data["MSH.3"] + "|" +
                           __tmp_msh.__msg_data["MSH.4"] + "|" +
                           __tmp_msh.__msg_data["MSH.5"] + "|" +
                           __tmp_msh.__msg_data["MSH.6"] + "|" +
                           __time_stamp + "||ACK^" +
                           __tmp_msh.__msg_data["MSH.9.2"] + "|" +
                           __tmp_msh.__msg_data["MSH.10"] + "|" +
                           __tmp_msh.__msg_data["MSH.11"] + "|" +
                           __tmp_msh.__msg_data["MSH.12"] + "|" +
                           "\r" +
                           @"MSA|AA|" +
                           __tmp_msh.__msg_data["MSH.10"] + "|" +
                           '\x1C' +
                           '\x0D';

            __clean_ack_string = @"MSH | ^~\& | " +
                           __tmp_msh.__msg_data["MSH.3"] + " | " +
                           __tmp_msh.__msg_data["MSH.4"] + " | " +
                           __tmp_msh.__msg_data["MSH.5"] + " | " +
                           __tmp_msh.__msg_data["MSH.6"] + " | " +
                           __time_stamp + "|| ACK^" +
                           __tmp_msh.__msg_data["MSH.9.2"] + " | " +
                           __tmp_msh.__msg_data["MSH.10"] + " | " +
                           __tmp_msh.__msg_data["MSH.11"] + " | " +
                           __tmp_msh.__msg_data["MSH.12"] + " | " +
                           "\n\tMSA | AA | " +
                           __tmp_msh.__msg_data["MSH.10"] + "|\n";
        }

        public ACK(MSH __msh)
        {
            Build(__msh, "MOT-II HL7 Proxy", "Medicine-On-Time");
        }
        public ACK(MSH __msh, string __org)
        {
            Build(__msh, __org, "MOT-II HL7 Proxy");
        }
        public ACK(MSH __msh, string __org, string __proc)
        {
            Build(__msh, __org, __proc);
        }
    }
    public class NAK
    {
        public string __nak_string { get; set; } = string.Empty;
        public string __clean_nak_string { get; set; } = string.Empty;

        public void Build(MSH __msh, string __error_code, string __org, string __proc)
        {
            if (string.IsNullOrEmpty(__org))
            {
                __org = "Unknown";
            }

            if (string.IsNullOrEmpty(__proc))
            {
                __proc = "Unknown";
            }

            if (string.IsNullOrEmpty(__error_code))
            {
                __error_code = "AF";
            }

            MSH __tmp_msh = __msh;
            string __time_stamp = DateTime.Now.ToString("yyyyMMddhh");

            __tmp_msh.__msg_data["MSH.5"] = __tmp_msh.__msg_data["MSH.3"];
            __tmp_msh.__msg_data["MSH.6"] = __tmp_msh.__msg_data["MSH.4"];
            __tmp_msh.__msg_data["MSH.3"] = __proc;
            __tmp_msh.__msg_data["MSH.4"] = __org;

            __nak_string = '\x0B' +
                           @"MSH|^~\&|" +
                           __tmp_msh.__msg_data["MSH.3"] + "|" +
                           __tmp_msh.__msg_data["MSH.4"] + "|" +
                           __tmp_msh.__msg_data["MSH.5"] + "|" +
                           __tmp_msh.__msg_data["MSH.6"] + "|" +
                           __time_stamp + "||NAK^" +
                           __tmp_msh.__msg_data["MSH.9.2"] + "|" +
                           __tmp_msh.__msg_data["MSH.10"] + "|" +
                           __tmp_msh.__msg_data["MSH.11"] + "|" +
                           __tmp_msh.__msg_data["MSH.12"] + "|" +
                           "\r" +
                           @"MSA|" +
                           __error_code + "|" +
                           __tmp_msh.__msg_data["MSH.10"] + "|" +
                           '\x1C' +
                           '\x0D';

            __clean_nak_string = @"MSH | ^~\& |" +
                           __tmp_msh.__msg_data["MSH.3"] + " | " +
                           __tmp_msh.__msg_data["MSH.4"] + " | " +
                           __tmp_msh.__msg_data["MSH.5"] + " | " +
                           __tmp_msh.__msg_data["MSH.6"] + " | " +
                           __time_stamp + "| | NAK^" +
                           __tmp_msh.__msg_data["MSH.9.2"] + " | " +
                           __tmp_msh.__msg_data["MSH.10"] + " | " +
                           __tmp_msh.__msg_data["MSH.11"] + " | " +
                           __tmp_msh.__msg_data["MSH.12"] + " | " +
                           "\n\tMSA | " +
                           __error_code + "| " +
                           __tmp_msh.__msg_data["MSH.10"] + " |\n";
        }

        public NAK(MSH __msh, string __error_code)
        {
            Build(__msh, __error_code, "Medicine-On-Time", "MOT-II HL7 Proxy");
        }
        public NAK(MSH __msh, string __error_code, string __org)
        {
            Build(__msh, __error_code, __org, "MOT-II HL7 Proxy");
        }
        public NAK(MSH __msh, string __error_code, string __org, string __proc)
        {
            Build(__msh, __error_code, __org, __proc);
        }
    }
};
