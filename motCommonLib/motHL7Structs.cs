﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace motCommonLib
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
    /// 
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

                    __tagname = __tagname + "-" + __minor.ToString();
                    __sub_parse(__tagname, __field, __field[__gotone], __message_data, __minor); // recurse

                    __tagname = __tmp_tagname;

                    continue;
                }

                __message_data.Add(__tagname + "-" + __minor.ToString(), __field);
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
            string __tagname = __field_names[0];

            foreach (string __field in __field_names)
            {
                // Catch the Root Name  {RXE,RXE}
                if(__major == 0)
                {
                    __message_data.Add(__tagname, __field);
                    __major++;
                    continue;
                }

                // Doah!  MSH requires something special
                if(__tagname == "MSH" && __major == 1)
                {
                    __message_data.Add(__tagname + "-" + __major.ToString(), "|");
                    __major++;
                }

                __message_data.Add(__tagname + "-" + __major.ToString(), __field);

                if (__field != @"^~\&")
                {
                    __gotone = __field.IndexOfAny(__local_delimiters);    // Subfield ADT01^RDE^RDE_011
                    if (__gotone != -1)
                    {
                        __tagname = __tagname + "-" + __major.ToString();

                        // Wait for it so we don't loose the overall seque
                        bool __success = __sub_parse(__tagname, __field, __field[__gotone], __message_data, __minor);

                        // Reset the tagname to the root level
                        __tagname = __field_names[0];
                    }
                }

                __major++;
            }
        }

        public HL7MessageParser() { }
    }

    public class HL7_Message_dictionary
    {
        public Dictionary<string, string> __field_names;
        public Dictionary<string, string> __msg_data;

        //public event EventHandler __newMessage;
        //public ML7toMOT __handlers = new ML7toMOT();

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

        public HL7_Message_dictionary()
        {                
            __field_names = new Dictionary<string, string>();
            __msg_data = new Dictionary<string, string>();
        }

        ~HL7_Message_dictionary()
        {
            __field_names.Clear();
            __msg_data.Clear();
        }
    }

    //
    // Messages - Specific to Softwriters FrameworkLTE
    //
    public class ADT_A01 : HL7_Message_dictionary   // Register (A04), Update (A08) a Patient
    {
        // A04 - MSH, EVN, PID, PV1, [{OBX}], [{AL1}], [{DG1}]
        // A08 - MSH, EVN, PID, PV1, [{OBX}], [{AL1}], [{DG1}]
        
        // Note that IN1 seems to show up with ADT_A04

        public MSH __msh;
        public EVN __evn;
        public PID __pid;
        public PV1 __pv1;
        public PR1 __pr1;

        public GT1 __gt1;
        public List<OBX> __obx;
        public List<AL1> __al1;
        public List<ROL> __rol;
        public List<DG1> __dg1;
        public List<IN1> __in1;
        public List<IN2> __in2;
        public List<NK1> __nk1;

        public List<Dictionary<string, string>> __message_store;

        public ADT_A01(string __message) : base()
        {

            string[] __segments = __message.Split('\r');

            __segments = __clear_newlines(__segments);


            __obx = new List<OBX>();
            __al1 = new List<AL1>();
            __dg1 = new List<DG1>();
            __in1 = new List<IN1>();
            __in2 = new List<IN2>();
            __nk1 = new List<NK1>();
            __rol = new List<ROL>();

            __message_store = new List<Dictionary<string, string>>();

            foreach (string __field in __segments)
            {
                switch (__field.Substring(0, 3))
                {
                    case "AL1":
                        AL1 __tmp_al1 = new AL1(__field);
                        __al1.Add(__tmp_al1);
                        __message_store.Add(__tmp_al1.__msg_data);
                        break;

                    case "EVN":
                        __evn = new EVN(__field);
                        __message_store.Add(__evn.__msg_data);
                        break;

                    case "DG1":
                        DG1 __tmp_dg1 = new DG1(__field);
                        __dg1.Add(__tmp_dg1);
                        __message_store.Add(__tmp_dg1.__msg_data);
                        break;

                    case "GT1":
                        __gt1 = new GT1(__field);
                        __message_store.Add(__gt1.__msg_data);
                        break;

                    case "IN1":
                        IN1 __tmp_in1 = new IN1(__field);
                        __in1.Add(__tmp_in1);
                        __message_store.Add(__tmp_in1.__msg_data);
                        break;

                    case "IN2":
                        IN2 __tmp_in2 = new IN2(__field);
                        __in2.Add(__tmp_in2);
                        __message_store.Add(__tmp_in2.__msg_data);
                        break;

                    case "MSH":
                        __msh = new MSH(__field);
                        __message_store.Add(__msh.__msg_data);
                        break;

                    case "NK1":
                        NK1 __tmp_nk1 = new NK1(__field);
                        __nk1.Add(__tmp_nk1);
                        __message_store.Add(__tmp_nk1.__msg_data);
                        break;

                    case "OBX":
                        OBX __tmp_obx = new OBX(__field);
                        __obx.Add(__tmp_obx);
                        __message_store.Add(__tmp_obx.__msg_data);
                        break;

                    case "PID":
                        __pid = new PID(__field);
                        __message_store.Add(__pid.__msg_data);
                        break;

                    case "PR1":
                        __pr1 = new PR1(__field);
                        __message_store.Add(__pr1.__msg_data);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__field);
                        __message_store.Add(__pv1.__msg_data);
                        break;

                    case "ROL":
                        ROL __tmp_rol = new ROL(__field);
                        __rol.Add(__tmp_rol);
                        __message_store.Add(__tmp_rol.__msg_data);
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
    public class ADT_A21 : HL7_Message_dictionary   // Delete a Patient
    {
        public List<Dictionary<string, string>> __message_store;

        // A23 - Not in Spec
        public ADT_A21() 
        {
            throw new NotImplementedException();
        }

    }
    public class OMP_O09 : HL7_Message_dictionary   // Pharmacy Treatment Order Message
    {
        // Control Code NW (New)            MSH, PID, [PV1], { ORC, [{TQ1}], [{RXR}], RXO, [{RXC}] }, [{NTE}]
        // Control Code DC (Discontinue)    MSH, PID, [PV1], { ORC, [{TQ1}], RXO, [{RXC}] }
        // Control Code RF (Refill)         MSH, PID, [PV1], { ORC, [{TQ1}], RXO, [{RXC}] }

        public MSH __msh;
        public PID __pid;
        public PV1 __pv1;
        public ORC __orc;
        public List<TQ1> __tq1;
        public RXO __rxo;
        public List<RXC> __rxc;
        public List<NTE> __nte;

        public List<Dictionary<string, string>> __message_store;

        /*
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
         */

        public OMP_O09(string __message) : base()
        {
            string[] __segments = __message.Split('\r');

            __segments = __clear_newlines(__segments);

            __tq1 = new List<TQ1>();
            __rxc = new List<RXC>();
            __nte = new List<NTE>();

            __message_store = new List<Dictionary<string, string>>();

            foreach (string __field in __segments)
            {

                switch (__field.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__field);
                        __message_store.Add(__msh.__msg_data);
                        break;

                    case "PID":
                        __pid = new PID(__field);
                        __message_store.Add(__pid.__msg_data);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__field);
                        __message_store.Add(__pv1.__msg_data);
                        break;

                    case "ORC":
                        __orc = new ORC(__field);
                        __message_store.Add(__orc.__msg_data);
                        break;

                    case "TQ1":
                        TQ1 __tmp_tq1 = new TQ1(__field);
                        __tq1.Add(__tmp_tq1);
                        __message_store.Add(__tmp_tq1.__msg_data);
                        break;

                    case "RXO":
                        __rxo = new RXO(__field);
                        __message_store.Add(__rxo.__msg_data);
                        break;

                    case "RXC":
                        RXC __tmp_rxc = new RXC(__field);
                        __rxc.Add(__tmp_rxc);
                        __message_store.Add(__tmp_rxc.__msg_data);
                        break;

                    case "NTE":
                        NTE __tmp_nte = new NTE(__field);
                        __nte.Add(__tmp_nte);
                        __message_store.Add(__tmp_nte.__msg_data);
                        break;

                    default:
                        break;
                }
            }

            // Raise __newMessage Event
            Console.WriteLine("Done Processing OMP_O09, now what ...");
        }
    }


    public class RDE_O11 : HL7_Message_dictionary  // Pharmacy/Treatment Encoded Order Message
    {
        // Drug Order       MSH, [ PID, [PV1] ], { ORC, [RXO, {RXR}, RXE, [{NTE}], {TQ1}, {RXR}, [{RXC}] }, [ZPI]
        // Literal Order    MSH, PID, [PV1], ORC, [TQ1], [RXE], [ZAS]

        public MSH __msh;
        public PID __pid;
        public PV1 __pv1;
        public ORC __orc;
        public RXE __rxe;
        public RXO __rxo;
        public List<RXR> __rxr;
        public List<RXC> __rxc;
        public List<NTE> __nte;
        public List<TQ1> __tq1;
        public ZAS __zas;
        public ZPI __zpi;

        public List<Dictionary<string, string>> __message_store;

        /*
                public class Order
                {
                    public ORC __orc;
                    public RXO __rxo;

                    public RXE __rxe;
                    public List<NTE> __nte;
                    public List<TQ1> __tq1;
                    public List<RXC> __rxc;
                    public List<RXR> __rxr;

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
        */


        public RDE_O11() : base()
        {
        }

        public RDE_O11(string __message) : base()
        {
            string[] __segments = __message.Split('\r');

            __segments = __clear_newlines(__segments);
         

            __rxr = new List<RXR>();
            __rxc = new List<RXC>();
            __nte = new List<NTE>();
            __tq1 = new List<TQ1>();

            __message_store = new List<Dictionary<string, string>>();
            
            foreach (string __type in __segments)
            {
                switch (__type.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__type);
                        __message_store.Add(__msh.__msg_data);
                        break;

                    case "PID":
                        __pid = new PID(__type);
                        __message_store.Add(__pid.__msg_data);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__type);
                        __message_store.Add(__pv1.__msg_data);
                        break;

                    case "TQ1":
                        TQ1 __tmp_tq1 = new TQ1(__type);
                        __tq1.Add(__tmp_tq1);
                        __message_store.Add(__tmp_tq1.__msg_data);
                        break;

                    case "RXC":
                        RXC __tmp_rxc = new RXC(__type);
                        __rxc.Add(__tmp_rxc);
                        __message_store.Add(__tmp_rxc.__msg_data);
                        break;

                    case "RXE":
                        __rxe = new RXE(__type);
                        __message_store.Add(__rxe.__msg_data);
                        break;

                    case "RXO":
                        __rxo = new RXO(__type);
                        __message_store.Add(__rxo.__msg_data);
                        break;

                    case "RXR":
                        RXR __tmp_rxr = new RXR(__type);
                        __rxr.Add(__tmp_rxr);
                        __message_store.Add(__tmp_rxr.__msg_data);
                        break;

                    case "ORC":  // Need to parse the order
                        __orc = new ORC(__type);
                        __message_store.Add(__orc.__msg_data);
                        break;

                    case "ZAS":
                        __zas = new ZAS(__type);
                        __message_store.Add(__zas.__msg_data);
                        break;

                    case "ZPI":
                        __zpi = new ZPI(__type);
                        __message_store.Add(__zpi.__msg_data);
                        break;

                    case "NTE":
                        NTE __tmp_nte = new NTE(__type);
                        __nte.Add(__tmp_nte);
                        __message_store.Add(__tmp_nte.__msg_data);
                        break;

                    default:
                        break;
                }
            }

            // Raise __newMessage Event
            Console.WriteLine("Done Processing RDE_O11, now what ...");
        }
    }
    public class RDS_O13 : HL7_Message_dictionary   // Pharmacy/Treatment Dispense Message
    {
        // Dispense Msg      MSH, [ PID, [PV1] ], { ORC, [RXO], {RXE}, [{NTE}], {TQ1}, {RXR}, [{RXC}], RXD }, [ZPI] 

        public MSH __msh;
        public PID __pid;
        public PV1 __pv1;
        public ORC __orc;
        public RXO __rxo;
        public RXE __rxe;
        public RXD __rxd;
        public List<NTE> __nte;
        public List<TQ1> __tq1;
        public List<RXR> __rxr;
        public List<RXC> __rxc;
        public ZPI __zpi;

        public List<Dictionary<string, string>> __message_store;

        /*
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
        */
        public RDS_O13(string __message) : base()
        {
            string[] __segments = __message.Split('\r');

            __segments = __clear_newlines(__segments);

            __nte = new List<NTE>();
            __tq1 = new List<TQ1>();
            __rxr = new List<RXR>();
            __rxc = new List<RXC>();

            __message_store = new List<Dictionary<string, string>>();

            foreach (string __field in __segments)
            {

                switch (__field.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__field);
                        __message_store.Add(__msh.__msg_data);
                        break;

                    case "PID":
                        __pid = new PID(__field);
                        __message_store.Add(__pid.__msg_data);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__field);
                        __message_store.Add(__pv1.__msg_data);
                        break;

                    case "ORC":
                        __orc = new ORC(__field);
                        __message_store.Add(__orc.__msg_data);
                        break;

                    case "RXO":
                        __rxo = new RXO(__field);
                        __message_store.Add(__rxo.__msg_data);
                        break;

                    case "RXE":
                        __rxe = new RXE(__field);
                        __message_store.Add(__rxe.__msg_data);
                        break;

                    case "RXD":
                        __rxd = new RXD(__field);
                        __message_store.Add(__rxd.__msg_data);
                        break;

                    case "NTE":
                        NTE __tmp_nte = new NTE(__field);
                        __nte.Add(__tmp_nte);
                        __message_store.Add(__tmp_nte.__msg_data);
                        break;

                    case "TQ1":
                        TQ1 __tmp_tq1 = new TQ1(__field);
                        __tq1.Add(__tmp_tq1);
                        __message_store.Add(__tmp_tq1.__msg_data);
                        break;

                    case "RXR":
                        RXR __tmp_rxr = new RXR(__field);
                        __rxr.Add(__tmp_rxr);
                        __message_store.Add(__tmp_rxr.__msg_data);
                        break;

                    case "RXC":
                        //__rxc.Add(new RXC(__field));
                        RXC __tmp_rxc = new RXC(__field);
                        __rxc.Add(__tmp_rxc);
                        __message_store.Add(__tmp_rxc.__msg_data);
                        break;

                    case "ZPI":
                        __zpi = new ZPI(__field);
                        __message_store.Add(__zpi.__msg_data);
                        break;

                    default:
                        break;
                }
            }

            // Raise __newMessage Event
            Console.WriteLine("Done Processing RDS_O13, now what ...");

        }
        public RDS_O13() : base()
        { }
    }


    //
    //  Segments - These all parse as SEG-#,-#... and stored as a Dictioary<string,string>
    //

    public class AL1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        public void __load()
        {
            __field_names.Add("AL1-1", "Set ID");
            __field_names.Add("AL1-2", @"Allergen Type Code");
            __field_names.Add("AL1-2-1", @"Identifier");
            __field_names.Add("AL1-2-2", @"Text");
            __field_names.Add("AL1-2-3", @"Name of Coding System");
            __field_names.Add("AL1-2-4", @"Alternate Identifier");
            __field_names.Add("AL1-2-5", @"Alternate Text");
            __field_names.Add("AL1-2-6", @"Name of Alternate Coding System");
            __field_names.Add("AL1-3", @"Allergen Code/Mnemonic/Description");
            __field_names.Add("AL1-3-1", @"Identifier");
            __field_names.Add("AL1-3-2", @"Text");
            __field_names.Add("AL1-3-3", @"Name of Coding System");
            __field_names.Add("AL1-3-4", @"Alternate Identifier");
            __field_names.Add("AL1-3-5", @"Alternate Text");
            __field_names.Add("AL1-3-6", @"Name of Alternate Coding System");
            __field_names.Add("AL1-4", @"Allergen Severity Code");
            __field_names.Add("AL1-4-1", @"Identifier");
            __field_names.Add("AL1-4-2", @"Text");
            __field_names.Add("AL1-4-3", @"Name of Coding System");
            __field_names.Add("AL1-4-4", @"Alternate Identifier");
            __field_names.Add("AL1-4-5", @"Alternate Text");
            __field_names.Add("AL1-4-6", @"Name of Alternate Coding System");
            __field_names.Add("AL1-5", @"Allergen Reaction Code");
            __field_names.Add("AL1-6", @"Identification Date");
        }
        public AL1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);

            Console.WriteLine("Finished parsing AL1");
        }
    }
    public class EVN : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", @"Event Type Code");

            __field_names.Add("2", @"Recorded Date/Time");
            __field_names.Add("2-1", @"Time");
            __field_names.Add("2-2", @"Degree of Percision");
            __field_names.Add("3", @"Date/Time Planned Event");
            __field_names.Add("4", @"Event Reason Code");
            __field_names.Add("5", @"Operator ID");
            __field_names.Add("5-1", @"ID Number");
            __field_names.Add("5-1-1", @"Surname");
            __field_names.Add("5-1-2", @"Own Surname Prefix");
            __field_names.Add("5-1-3", @"Surname Prefix From Partner/Spouse");
            __field_names.Add("5-1-4", @"Surname From Partner/Spouse");
            __field_names.Add("5-2", @"Family Name");
            __field_names.Add("5-3", @"Given Name");
            __field_names.Add("5-4", @"Second and Further Given Names or Initials Thereof");
            __field_names.Add("5-5", @"Suffix");
            __field_names.Add("5-6", @"Prefix");
            __field_names.Add("5-7", @"Degree");
            __field_names.Add("5-8", @"Source Table");
            __field_names.Add("5-9", @"Assigning Authority");
            __field_names.Add("5-9-1", @"Namespace ID");
            __field_names.Add("5-9-2", @"Universal ID");
            __field_names.Add("5-9-3", @"Universal ID Type");
            __field_names.Add("5-10", @"Name Type Code");
            __field_names.Add("5-11", @"Identifier Check Digit");
            __field_names.Add("5-12", @"Check Digit Scheme");
            __field_names.Add("5-13", @"Identifier Type Code");
            __field_names.Add("5-14", @"Assigning Facility");
            __field_names.Add("5-14-1", @"Namespace ID");
            __field_names.Add("5-14-2", @"Universal ID");
            __field_names.Add("5-14-3", @"Universal ID Type");
            __field_names.Add("5-15", @"Name Representation Code");
            __field_names.Add("5-16", @"Name Context");
            __field_names.Add("5-16-1", @"Text");
            __field_names.Add("5-16-2", @"Name of Coding System");
            __field_names.Add("5-16-3", @"Alternate Identifier");
            __field_names.Add("5-16-4", @"Alternate Text");
            __field_names.Add("5-16-5", @"Name of Alternate Coding System");
            __field_names.Add("5-17", @"Name Validity Range");
            __field_names.Add("5-17-1", @"Range Start Date/Time");
            __field_names.Add("5-17-1-1", @"Time");
            __field_names.Add("5-17-1-2", @"Degree of Percision");
            __field_names.Add("5-17-2", @"Range End Date/Time");
            __field_names.Add("5-17-2-1", @"Time");
            __field_names.Add("5-17-2-2", @"Degree of Percision");
            __field_names.Add("5-18", @"Name Assembly Order");
            __field_names.Add("5-19", @"Effective Date");
            __field_names.Add("5-19-1", @"Time");
            __field_names.Add("5-19-2", @"Degree of Percision");
            __field_names.Add("5-20", @"Expiration Date ");
            __field_names.Add("5-20-1", @"Time");
            __field_names.Add("5-20-2", @"Degree of Percision");
            __field_names.Add("5-21", @"Profssional Suffix");
            __field_names.Add("5-22", @"Assigning Juristiction");
            __field_names.Add("5-22-1", @"Text");
            __field_names.Add("5-22-2", @"Name of Coding System");
            __field_names.Add("5-22-3", @"Alternate Identifier");
            __field_names.Add("5-22-4", @"Alternate Text");
            __field_names.Add("5-22-5", @"Name of Alternate Coding System");
            __field_names.Add("5-23", @"Assigning Agency or Department");
            __field_names.Add("5-23-1", @"Text");
            __field_names.Add("5-23-2", @"Name of Coding System");
            __field_names.Add("5-23-3", @"Alternate Identifier");
            __field_names.Add("5-23-4", @"Alternate Text");
            __field_names.Add("5-23-5", @"Name of Alternate Coding System");
            __field_names.Add("6", @"Event Occured");
            __field_names.Add("6-1", @"Time");
            __field_names.Add("6-2", @"Degree of Percision");
            __field_names.Add("7", @"Event Facility");
            __field_names.Add("7-1", @"Namespace ID");
            __field_names.Add("7-2", @"Universal ID");
            __field_names.Add("7-3", @"Universal ID Type");

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
    public class DG1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {   
            __field_names.Add("DG1-1", @"Set ID");
            __field_names.Add("DG1-2", @"Diagnosis Coding Method");
            __field_names.Add("DG1-2-1", @"Identifier");
            __field_names.Add("DG1-2-2", @"Text");
            __field_names.Add("DG1-2-3", @"Name of Coding System");
            __field_names.Add("DG1-2-4", @"Alternate Identifier");
            __field_names.Add("DG1-2-5", @"Alternate Text");
            __field_names.Add("DG1-2-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-3", @"Diagnosis Code");
            __field_names.Add("DG1-3-1", @"Identifier");
            __field_names.Add("DG1-3-2", @"Text");
            __field_names.Add("DG1-3-3", @"Name of Coding System");
            __field_names.Add("DG1-3-4", @"Alternate Identifier");
            __field_names.Add("DG1-3-5", @"Alternate Text");
            __field_names.Add("DG1-3-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-4", @"Diagnosis Description");
            __field_names.Add("DG1-4-1", @"Time");
            __field_names.Add("DG1-5", @"Diagnosis Date/Time");
            __field_names.Add("DG1-6", @"Diagnosis Type");
            __field_names.Add("DG1-7", @"Diagnostic Major Catagory");
            __field_names.Add("DG1-7-1", @"Identifier");
            __field_names.Add("DG1-7-2", @"Text");
            __field_names.Add("DG1-7-3", @"Name of Coding System");
            __field_names.Add("DG1-7-4", @"Alternate Identifier");
            __field_names.Add("DG1-7-5", @"Alternate Text");
            __field_names.Add("DG1-7-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-8", @"Diagnostic Related Group");
            __field_names.Add("DG1-8-1", @"Identifier");
            __field_names.Add("DG1-8-2", @"Text");
            __field_names.Add("DG1-8-3", @"Name of Coding System");
            __field_names.Add("DG1-8-4", @"Alternate Identifier");
            __field_names.Add("DG1-8-5", @"Alternate Text");
            __field_names.Add("DG1-8-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-9", @"DRG Approval Indicator");
            __field_names.Add("DG1-10", @"DRG Grouper Review Code");
            __field_names.Add("DG1-11", @"Outlier Type");
            __field_names.Add("DG1-11-1", @"Identifier");
            __field_names.Add("DG1-11-2", @"Text");
            __field_names.Add("DG1-11-3", @"Name of Coding System");
            __field_names.Add("DG1-11-4", @"Alternate Identifier");
            __field_names.Add("DG1-11-5", @"Alternate Text");
            __field_names.Add("DG1-11-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-12", @"Outlier Days");
            __field_names.Add("DG1-13", @"Outlier Cost");
            __field_names.Add("DG1-13-1", @"Price");
            __field_names.Add("DG1-13-1-1", @"Quantity");
            __field_names.Add("DG1-13-1-2", @"Denomination");
            __field_names.Add("DG1-13-2", @"Price Type");
            __field_names.Add("DG1-13-3", @"From Value");
            __field_names.Add("DG1-13-4", @"To Value");
            __field_names.Add("DG1-13-5", @"Range Units");
            __field_names.Add("DG1-13-5-1", @"Identifier");
            __field_names.Add("DG1-13-5-2", @"Text");
            __field_names.Add("DG1-13-5-3", @"Name of Coding System");
            __field_names.Add("DG1-13-5-4", @"Alternate Identifier");
            __field_names.Add("DG1-13-5-5", @"Alternate Text");
            __field_names.Add("DG1-13-5-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-13-6", @"Range Type");
            __field_names.Add("DG1-14", @"Grouper Version and Type");
            __field_names.Add("DG1-15", @"Diagnosis Priority");
            __field_names.Add("DG1-16", @"Diagnosing Clinician");
            __field_names.Add("DG1-16-1", @"ID Number");
            __field_names.Add("DG1-16-2-2", @"Surname");
            __field_names.Add("DG1-16-2-3", @"Own Surname");
            __field_names.Add("DG1-16-2-4", @"Surname Prefix From Partner/Spouse");
            __field_names.Add("DG1-16-2", @"Family Name");
            __field_names.Add("DG1-16-3", @"Given Name");
            __field_names.Add("DG1-16-4", @"Second and Further Given Names or Initials Thereof");
            __field_names.Add("DG1-16-5", @"Suffix");
            __field_names.Add("DG1-16-6", @"Prefix");
            __field_names.Add("DG1-16-7", @"Source Table");
            __field_names.Add("DG1-16-8", @"Assigning Authority");
            __field_names.Add("DG1-16-8-1", @"Namespace ID");
            __field_names.Add("DG1-16-8-2", @"Universal ID");
            __field_names.Add("DG1-16-8-3", @"Universal ID Type");
            __field_names.Add("DG1-16-9", @"Name Type Code");
            __field_names.Add("DG1-16-10", @"Identifier Check Digit");
            __field_names.Add("DG1-16-11", @"Check Digit Scheme");
            __field_names.Add("DG1-16-12", @"Identifier Type Code");
            __field_names.Add("DG1-16-13", @"Assigning Facility");
            __field_names.Add("DG1-16-13-1", @"Namespace ID");
            __field_names.Add("DG1-16-13-2", @"Universal ID");
            __field_names.Add("DG1-16-13-3", @"Universal ID Type");
            __field_names.Add("DG1-16-14", @"Name Representation Code");
            __field_names.Add("DG1-16-15", @"Name Context");
            __field_names.Add("DG1-16-15-1", @"Identifier");
            __field_names.Add("DG1-16-15-2", @"Text");
            __field_names.Add("DG1-16-15-3", @"Name of Coding System");
            __field_names.Add("DG1-16-15-4", @"Alternate Identifier");
            __field_names.Add("DG1-16-15-5", @"Alternate Text");
            __field_names.Add("DG1-16-15-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-16-16", @"Name Assembly Order");
            __field_names.Add("DG1-16-17", @"Effective Date");
            __field_names.Add("DG1-16-18", @"Expiration Date");
            __field_names.Add("DG1-16-19", @"Professional Suffix");
            __field_names.Add("DG1-16-20", @"Assigning Juristiction");
            __field_names.Add("DG1-16-20-1", @"Identifier");
            __field_names.Add("DG1-16-20-2", @"Text");
            __field_names.Add("DG1-16-20-3", @"Name of Coding System");
            __field_names.Add("DG1-16-20-4", @"Alternate Identifier");
            __field_names.Add("DG1-16-20-5", @"Alternate Text");
            __field_names.Add("DG1-16-20-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-16-21", @"Assigning Agency or Department");
            __field_names.Add("DG1-16-21-1", @"Identifier");
            __field_names.Add("DG1-16-21-2", @"Text");
            __field_names.Add("DG1-16-21-3", @"Name of Coding System");
            __field_names.Add("DG1-16-21-4", @"Alternate Identifier");
            __field_names.Add("DG1-16-21-5", @"Alternate Text");
            __field_names.Add("DG1-16-21-6", @"Name of Alternate Coding System");
            __field_names.Add("DG1-17", @"Diagnosis Classification");
            __field_names.Add("DG1-18", @"Confidential Indicator");
            __field_names.Add("DG1-19", @"Attestation Date/Time");
            __field_names.Add("DG1-20", @"Diagnosis Identifier");
            __field_names.Add("DG1-20-1", @"Entity Identifier");
            __field_names.Add("DG1-20-2", @"Namespace ID");
            __field_names.Add("DG1-20-3", @"Universal ID");
            __field_names.Add("DG1-20-4", @"Universal ID Type");
            __field_names.Add("DG1-21", @"Diagnosis Action Code");

        }

        public DG1() : base()
        {
            __load();
        }

        public DG1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class GT1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public GT1() : base()
        {
            __load();
        }
        public GT1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class IIM : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public IIM() : base()
        {
            __load();
        }
        public IIM(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class IN1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public IN1() : base()
        {
            __load();
        }
        public IN1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class IN2 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public IN2() : base()
        {
            __load();
        }
        public IN2(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class MSA : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("MSA-1", @"Acknowledgment Code");     // AA - Application Accept
                                                                    // AE - Application Error
                                                                    // AR - Application Reject

            __field_names.Add("MSA-2", @"Message Control ID");      // = MSH-10
            __field_names.Add("MSA-3", @"Text Message");            // FrameworkLTE -ZPS
            __field_names.Add("MSA-4", @"Expected Sequence Number");
            __field_names.Add("MSA-5", @"Delayed Acknowledment Time");
            __field_names.Add("MSA-6", @"Error Condition");
            __field_names.Add("MSA-6-1", @"Identifier");
            __field_names.Add("MSA-6-2", @"Text");
            __field_names.Add("MSA-6-3", @"Name of Coding System");
            __field_names.Add("MSA-6-4", @"Alternate Identifier");
            __field_names.Add("MSA-6-5", @"Text");
            __field_names.Add("MSA-6-6", @"Name of Alternate Coding System");
        }
        public MSA() : base()
        {
            __load();
        }
        public MSA(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class MSH : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            // if a field has ^ ^ ^ in it, that defines the .n items, e.g. |Fred^Flintstone^Bedrock|  the item would parse as follows:
            //      
            //  BDR |Fred^Flintstone^Bedrock|
            //  BDR-1 Fred
            //  BDR-2 Flintstone
            //  BDR-3 Bedrock
            //  So the build rule should be:
            //
            //      If an item is clear text, add it as the top level.  The Tag.1 is implicit


            __field_names.Add("MSH-0", @"SegmentName");
            __field_names.Add("MSH-1", @"Field Separator");
            __field_names.Add("MSH-2", @"Encoding Characters");

            __field_names.Add("MSH-3", @"Sending Application");
            __field_names.Add("MSH-3-1", @"Namespace ID");
            __field_names.Add("MSH-3-2", @"Universal ID");
            __field_names.Add("MSH-3-3", @"Universal ID Type;");

            __field_names.Add("MSH-4", @"Sending Facility");
            __field_names.Add("MSH-4-1", @"Namespace ID");
            __field_names.Add("MSH-4-2", @"Universal ID");
            __field_names.Add("MSH-4-3", @"Universal ID Type;");

            __field_names.Add("MSH-5", @"Receiving Application");
            __field_names.Add("MSH-5-1", @"Namespace ID");
            __field_names.Add("MSH-5-2", @"Universal ID");
            __field_names.Add("MSH-5-3", @"Universal ID Type;");

            __field_names.Add("MSH-6", @"Receiving Facility");
            __field_names.Add("MSH-6-1", @"Namespace ID");
            __field_names.Add("MSH-6-2", @"Universal ID");
            __field_names.Add("MSH-6-3", @"Universal ID Type;");

            __field_names.Add("MSH-7", @"Date/Time of Message");
            __field_names.Add("MSH-7-1", @"Time");
            __field_names.Add("MSH-7-2", @"Degree of Percision");

            __field_names.Add("MSH-8", @"Security");

            __field_names.Add("MSH-9", @"Message Type");
            __field_names.Add("MSH-9-1", @"Message Code");
            __field_names.Add("MSH-9-2", @"Trigger Event");
            __field_names.Add("MSH-9-3", @"Message Structure");

            __field_names.Add("MSH-10", @"Message Control ID");

            __field_names.Add("MSH-11", @"Processing ID");
            __field_names.Add("MSH-11-1", @"Processing ID");
            __field_names.Add("MSH-11-2", @"Processing Mode");

            __field_names.Add("MSH-12", @"Version ID");
            __field_names.Add("MSH-12-1", @"Version ID");
            __field_names.Add("MSH-12-2", @"Internationalization Code");
            __field_names.Add("MSH-12-2-1", @"Identifier");
            __field_names.Add("MSH-12-2-2", @"Text");
            __field_names.Add("MSH-12-2-3", @"Name of Coding System");
            __field_names.Add("MSH-12-2-4", @"Alternate Identifier");
            __field_names.Add("MSH-12-2-5", @"Alternate Text");
            __field_names.Add("MSH-12-2-6", @"Name of Alternate Coding System");
            __field_names.Add("MSH-12-3", @"International Version ID");
            __field_names.Add("MSH-12-3-1", @"Text");
            __field_names.Add("MSH-12-3-2", @"Name of Coding System");
            __field_names.Add("MSH-12-3-3", @"Alternate Identifier");
            __field_names.Add("MSH-12-3-4", @"Alternate Text");
            __field_names.Add("MSH-12-3-5", @"Name of Alternate Coding System");

            __field_names.Add("MSH-13", @"Sequence Number");
            __field_names.Add("MSH-14", @"Continuation Pointer");
            __field_names.Add("MSH-15", @"Accept Acknowledgement Type");
            __field_names.Add("MSH-16", @"Application Acknowledgement Type");
            __field_names.Add("MSH-17", @"Country Code");
            __field_names.Add("MSH-18", @"Character Set");

            __field_names.Add("MSH-19", @"Principal Language of Message");
            __field_names.Add("MSH-19-1", @"Identifier");
            __field_names.Add("MSH-19-2", @"Text");
            __field_names.Add("MSH-19-3", @"Name of Coding System");
            __field_names.Add("MSH-19-4", @"Alternate Identifier");
            __field_names.Add("MSH-19-5", @"Alternate Text");
            __field_names.Add("MSH-19-6", @"Name of Alternate Coding System");

            __field_names.Add("MSH-20", @"Alternate Character Set Handling Scheme");
            __field_names.Add("MSH-21", @"Message Profile Identifier");
            __field_names.Add("MSH-21-1", @"Entity Identifier");
            __field_names.Add("MSH-21-2", @"Namespace Identifier");
            __field_names.Add("MSH-21-3", @"Universal ID");
            __field_names.Add("MSH-21-4", @"Universal ID Type");
        }


        public MSH()
        {
            __load();
        }

        public MSH(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);

            //
            // Wierdness - In MSH-9 there can be 2 or 3 items for example |ADT^AO1| or |RDE^O11^RDE_O11|. It looks like if there
            //             is no MSH-9-3, 9-1 and 9-2 need to be commbined to make 9-3. So, the strategy should be to combine 1 & 2 into 3 
            //             and if there's a 3, it will overwrite it.
            string __tmp = string.Empty;

            if(!__msg_data.TryGetValue("MSH-9-3", out __tmp))
            {
                __msg_data.Add("MSH-9-3", __msg_data["MSH-9-1"] + "_" + __msg_data["MSH-9-2"]);
            }

            Console.WriteLine("Finished parsing MSH");
        }

        public string __full_message()
        {
            string __out = @"MSH|^~\&||MOT_HL7Gateway|MOT_HL7Gateway|";
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
    public class NK1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public NK1() : base()
        {
            __load();
        }
        public NK1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class NTE : HL7_Message_dictionary
    {
        // public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>()
        //public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", @"Set ID");
            __field_names.Add("2", @"Source of Comment");
            __field_names.Add("3", @"Comment");
            __field_names.Add("4", @"Comment Type");
            __field_names.Add("4-1", @"Identifier");
            __field_names.Add("4-2", @"Text");
            __field_names.Add("4-3", @"Name of Coding System");
            __field_names.Add("4-4", @"Alternate Identifier");
            __field_names.Add("4-5", @"Alternate Text");
            __field_names.Add("4-6", @"Name of Alternate Coding System");
        }



        public NTE(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }

        public NTE() : base()
        {
            __load();
        }
    }
    public class OBX : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", @"Set ID");
            __field_names.Add("2", @"Value Type");

            __field_names.Add("3", @"Observation Indicator");
            __field_names.Add("3-1", @"Identifier");
            __field_names.Add("3-2", @"Text");
            __field_names.Add("3-3", @"Name of Coding System");
            __field_names.Add("3-4", @"Alternate Identifier");
            __field_names.Add("3-5", @"Alternate Text");
            __field_names.Add("3-6", @"Name of Alternate Coding System");

            __field_names.Add("4", @"Observation Sub ID");
            __field_names.Add("5", @"Observation Value");
            __field_names.Add("6", @"Units");

            __field_names.Add("6-1", @"Identifier");
            __field_names.Add("6-2", @"Text");
            __field_names.Add("6-3", @"Name of Coding System");
            __field_names.Add("6-4", @"Alternate Identifier");
            __field_names.Add("6-5", @"Alternate Text");
            __field_names.Add("6-6", @"Name of Alternate Coding System");

            __field_names.Add("7", @"Reference Range");
            __field_names.Add("8", @"Abnormal Flags");
            __field_names.Add("9", @"Probability");
            __field_names.Add("10", @"Nature of Abnormal Test");
            __field_names.Add("11", @"Observation Result Status");

            __field_names.Add("12", @"Effective Date of Reference Range");
            __field_names.Add("12-1", @"Time");
            __field_names.Add("12-2", @"Degree of Percision");

            __field_names.Add("13", @"User Defined Access Checks");

            __field_names.Add("14", @"Date/Time of the Observation");
            __field_names.Add("14-1", @"Time");
            __field_names.Add("14-2", @"Degree of Percision");

            __field_names.Add("15", @"Producer's ID");
            __field_names.Add("15-1", @"Identifier");
            __field_names.Add("15-2", @"Text");
            __field_names.Add("15-3", @"Name of Coding System");
            __field_names.Add("15-4", @"Alternate Identifier");
            __field_names.Add("15-5", @"Alternate Text");
            __field_names.Add("15-6", @"Name of Alternate Coding System");

            __field_names.Add("16", @"Responsible Observer");
            // ...

            __field_names.Add("17", @"Observation Method");
            // ...

            __field_names.Add("18", @"Equipment Instance Identifier");
            // ...

            __field_names.Add("19", @"Date/Time of the Analysis");
            // ...
        }

        public OBX() : base()
        {
            __load();
        }

        public OBX(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ORC : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", @"ID");
            __field_names.Add("2", @"Placer Order Number");
            __field_names.Add("3", @"Filler Order Number");
            __field_names.Add("4", @"Placer Group Number");
            __field_names.Add("5", @"Order Status");
            __field_names.Add("6", @"Response Flag");
            __field_names.Add("7", @"Quantity/Timing");
            __field_names.Add("8", @"Parent");
            __field_names.Add("9", @"Date/Time of Transaction");
            __field_names.Add("10", @"Entered By");
            __field_names.Add("11", @"Verified By");
            __field_names.Add("12", @"Ordering Provider");
            __field_names.Add("13", @"Enterer's Location");
            __field_names.Add("14", @"Call Back Phone Number");
            __field_names.Add("15", @"Order Effective Date/Time");
            __field_names.Add("16", @"Order Control Reason");
            __field_names.Add("17", @"Entering Organization");
            __field_names.Add("18", @"Entering Device");
            __field_names.Add("19", @"Action By");
            __field_names.Add("20", @"Advanced Beneficiary Notice Code");
            __field_names.Add("21", @"Ordering Facility Name");
            __field_names.Add("22", @"Ordering Facility Address");
            __field_names.Add("23", @"Ordering Facility Phone Number");
            __field_names.Add("24", @"Ordering Provider Address");
            __field_names.Add("25", @"Order Status Modifier");
            __field_names.Add("26", @"Advanced Benificiary Notice Override Reason");
            __field_names.Add("27", @"Filler's Expected Availability Date/Time");
            __field_names.Add("28", @"Confidentiality Code");
            __field_names.Add("29", @"Order Type");
            __field_names.Add("30", @"Enterer Authorization Mode");
        }
        public ORC() : base()
        {
            __load();
        }

        public ORC(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PID : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", @"Set ID");
            __field_names.Add("2", @"Patient ID");
            __field_names.Add("3", @"Patient Identifier List");
            __field_names.Add("4", @"Alternate Pathient ID - PID");
            __field_names.Add("5", @"Patient Name");
            __field_names.Add("6", @"Mothers Maiden Name");
            __field_names.Add("7", @"Date/Time of Birth");
            __field_names.Add("8", @"Administrative Sex");
            __field_names.Add("9", @"Patient Alias");
            __field_names.Add("10", @"Race");
            __field_names.Add("11", @"Patient Address");
            __field_names.Add("12", @"County Code");
            __field_names.Add("13", @"Phone Number - Home");
            __field_names.Add("14", @"Phone Number - Business");
            __field_names.Add("15", @"Primary Language");
            __field_names.Add("16", @"Marital Status");
            __field_names.Add("17", @"Religion");
            __field_names.Add("18", @"Patient Account Number");
            __field_names.Add("19", @"SSN - Patient");
            __field_names.Add("20", @"Drivers License Number - Patient");
            __field_names.Add("21", @"Mother's Identifier");
            __field_names.Add("22", @"Ethnic Group");
            __field_names.Add("23", @"Birth Place");
            __field_names.Add("24", @"Multiple Birth Indicator");
            __field_names.Add("25", @"Birth Order");
            __field_names.Add("26", @"Citizenship");
            __field_names.Add("27", @"Veterens Military Status");
            __field_names.Add("28", @"Nationality");
            __field_names.Add("29", @"Patient Death Date/Time");
            __field_names.Add("30", @"Patient Deth Indictor");
            __field_names.Add("31", @"Identity Unknown Indicator");
            __field_names.Add("32", @"Identity Reliability Code");
            __field_names.Add("33", @"Last Update Date/Time");
            __field_names.Add("34", @"Last Update Facility");
            __field_names.Add("35", @"Species Code");
            __field_names.Add("36", @"Breed Code");
            __field_names.Add("37", @"Strain");
            __field_names.Add("38", @"Production Class Code");
            __field_names.Add("39", @"Tribal Citizenship");
        }
        public PID()
        {
            __load();
        }

        public PID(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PR1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public PR1() : base()
        {
            __load();
        }
        public PR1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PV1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        // TODO:  Finish Field Names
        private void __load()  // TODO:  Finish Patient Visit Fields
        {
            __field_names.Add("PV1-1", @"Set ID");
            __field_names.Add("PV1-2", @"Patient Class");
            __field_names.Add("PV1-3", @"Observation Indicator");
            __field_names.Add("PV1-4", @"Observation Sub ID");
            __field_names.Add("PV1-5", @"Observation Value");
            __field_names.Add("PV1-6", @"Units");
            __field_names.Add("PV1-7", @"Reference Range");
            __field_names.Add("PV1-8", @"Abnormal Flags");
            __field_names.Add("PV1-9", @"Probability");
            __field_names.Add("PV1-10", @"Nature of Abnormal Test");
            __field_names.Add("PV1-11", @"Set ID");
            __field_names.Add("PV1-12", @"Value Type");
            __field_names.Add("PV1-13", @"Observation Indicator");
            __field_names.Add("PV1-14", @"Observation Sub ID");
            __field_names.Add("PV1-15", @"Observation Value");
            __field_names.Add("PV1-16", @"Units");
            __field_names.Add("PV1-17", @"Reference Range");
            __field_names.Add("PV1-18", @"Abnormal Flags");
            __field_names.Add("PV1-19", @"Probability");
            __field_names.Add("PV1-20", @"Nature of Abnormal Test");
            __field_names.Add("PV1-21", @"Set ID");
            __field_names.Add("PV1-22", @"Value Type");
            __field_names.Add("PV1-23", @"Observation Indicator");
            __field_names.Add("PV1-24", @"Observation Sub ID");
            __field_names.Add("PV1-25", @"Observation Value");
            __field_names.Add("PV1-26", @"Units");
            __field_names.Add("PV1-27", @"Reference Range");
            __field_names.Add("PV1-28", @"Abnormal Flags");
            __field_names.Add("PV1-29", @"Probability");
            __field_names.Add("PV1-30", @"Nature of Abnormal Test");
            __field_names.Add("PV1-31", @"Set ID");
            __field_names.Add("PV1-32", @"Value Type");
            __field_names.Add("PV1-33", @"Observation Indicator");
            __field_names.Add("PV1-34", @"Observation Sub ID");
            __field_names.Add("PV1-35", @"Observation Value");
            __field_names.Add("PV1-36", @"Units");
            __field_names.Add("PV1-37", @"Reference Range");
            __field_names.Add("PV1-38", @"Abnormal Flags");
            __field_names.Add("PV1-39", @"Probability");
            __field_names.Add("PV1-40", @"Nature of Abnormal Test");
            __field_names.Add("PV1-41", @"Set ID");
            __field_names.Add("PV1-42", @"Value Type");
            __field_names.Add("PV1-43", @"Observation Indicator");
            __field_names.Add("PV1-44", @"Observation Sub ID");
            __field_names.Add("PV1-45", @"Observation Value");
            __field_names.Add("PV1-46", @"Units");
            __field_names.Add("PV1-47", @"Reference Range");
            __field_names.Add("PV1-48", @"Abnormal Flags");
            __field_names.Add("PV1-49", @"Probability");
            __field_names.Add("PV1-50", @"Nature of Abnormal Test");
        }
        public PV1() : base()
        {
            __load();
        }

        public PV1(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ROL : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        { }
        public ROL() : base()
        {
            __load();
        }
        public ROL(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXC : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("RXC-1", @"RX Component Type");

            __field_names.Add("RXC-2", @"Component Code");
            __field_names.Add("RXC-2-1", @"Identifier");
            __field_names.Add("RXC-2-2", @"Text");
            __field_names.Add("RXC-2-3", @"Name of Coding System");
            __field_names.Add("RXC-2-4", @"Alternate Identifier");
            __field_names.Add("RXC-2-5", @"Alternate Text");
            __field_names.Add("RXC-2-6", @"Name of Alternate CodingSystem");

            __field_names.Add("RXC-3", @"Component Amount");

            __field_names.Add("RXC-4", @"Component Units");
            __field_names.Add("RXC-4-1", @"Identifier");
            __field_names.Add("RXC-4-2", @"Text");
            __field_names.Add("RXC-4-3", @"Name of Coding System");
            __field_names.Add("RXC-4-4", @"Alternate Identifier");
            __field_names.Add("RXC-4-5", @"Alternate Text");
            __field_names.Add("RXC-4-6", @"NAme of Alternate Coding System");

            __field_names.Add("RXC-5", @"Component Strength");

            __field_names.Add("RXC-6", @"Component Strength Units");
            __field_names.Add("RXC-6-1", @"Identifier");
            __field_names.Add("RXC-6-2", @"Text");
            __field_names.Add("RXC-6-3", @"Name of Coding System");
            __field_names.Add("RXC-6-4", @"Alternate Identifier");
            __field_names.Add("RXC-6-5", @"Alternate Text");
            __field_names.Add("RXC-6-6", @"NAme of Alternate Coding System");

            __field_names.Add("RXC-7", @"Suplementary Code");
            __field_names.Add("RXC-7-1", @"Identifier");
            __field_names.Add("RXC-7-2", @"Text");
            __field_names.Add("RXC-7-3", @"Name of Coding System");
            __field_names.Add("RXC-7-4", @"Alternate Identifier");
            __field_names.Add("RXC-7-5", @"Alternate Text");
            __field_names.Add("RXC-7-6", @"Name of Alternate Coding System");

            __field_names.Add("RXC-8", @"Component Drug Volume");

            __field_names.Add("RXC-9", @"Component Drug Volume Units");
            __field_names.Add("RXC-9-1", @"Identifier");
            __field_names.Add("RXC-9-2", @"Text");
            __field_names.Add("RXC-9-3", @"Name of Coding System");
            __field_names.Add("RXC-9-4", @"Alternate Identifier");
            __field_names.Add("RXC-9-5", @"Alternate Text");
            __field_names.Add("RXC-9-6", @"Name of Alternate Coding System");
            __field_names.Add("RXC-9-7", @"Coding System Version ID");
            __field_names.Add("RXC-9-8", @"Alternate Coding System Version");
            __field_names.Add("RXC-9-9", @"Original Text");
        }
        public RXC() : base()
        {
            __load();
        }

        public RXC(string __message) : base()
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXD : HL7_Message_dictionary
    {
      
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXD() : base()
        {
        }

        public RXD(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXE : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXE() : base()
        {
        }

        public RXE(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXO : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXO() : base()
        {
        }

        public RXO(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXR : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();


        private void __load()
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
    public class TQ1 : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public TQ1() : base()
        {
        }

        public TQ1(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZAS : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public ZAS() :  base()
        {
        }

        public ZAS(string __message) : base()
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZPI : HL7_Message_dictionary
    {
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
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


    public class ACK
    {
        public string __ack_string { get; set; } = string.Empty;

        public ACK(MSH __msh)
        {
            MSH __tmp_msh = __msh;

            string __time_stamp = DateTime.Now.Year.ToString() +
                                  DateTime.Now.Month.ToString() +
                                  DateTime.Now.Day.ToString() +
                                  DateTime.Now.Hour.ToString() +
                                  DateTime.Now.Minute.ToString();

            __tmp_msh.__msg_data["MSH-5"] = __tmp_msh.__msg_data["MSH-3"];
            __tmp_msh.__msg_data["MSH-6"] = __tmp_msh.__msg_data["MSH-4"];

            __tmp_msh.__msg_data["MSH-3"] = "MOT_HL7Gateway";
            __tmp_msh.__msg_data["MSH-4"] = "Medicine-On-Time";

            __ack_string = '\x0B' +
                           @"MSH|^~\&|" +
                           __tmp_msh.__msg_data["MSH-3"] + "|" +
                           __tmp_msh.__msg_data["MSH-4"] + "|" +
                           __tmp_msh.__msg_data["MSH-5"] + "|" +
                           __tmp_msh.__msg_data["MSH-6"] + "|" +
                           __time_stamp + "||ACK^" +
                           __tmp_msh.__msg_data["MSH-9-2"] + "|" +
                           __tmp_msh.__msg_data["MSH-10"] + "|" +
                           __tmp_msh.__msg_data["MSH-11"] + "|" +
                           __tmp_msh.__msg_data["MSH-12"] + "|" +
                           @"\r" +
                           @"MSA|AA|" +
                           __tmp_msh.__msg_data["MSH-10"] + "|" +
                           '\x1C' +
                           '\x0D';
        }
    }
    public class NAK
    {
        public string __nak_string { get; set; } = string.Empty;

        public NAK(MSH __msh, string __error_code)
        {
            MSH __tmp_msh = __msh;

            string __time_stamp = DateTime.Now.Year.ToString() +
                                  DateTime.Now.Month.ToString() +
                                  DateTime.Now.Day.ToString() +
                                  DateTime.Now.Hour.ToString() +
                                  DateTime.Now.Minute.ToString();

            if (string.IsNullOrEmpty(__error_code))
            {
                __error_code = "AF";
            }

            __tmp_msh.__msg_data["MSH-5"] = __tmp_msh.__msg_data["MSH-3"];
            __tmp_msh.__msg_data["MSH-6"] = __tmp_msh.__msg_data["MSH-4"];

            __tmp_msh.__msg_data["MSH-3"] = "MOT_HL7Gateway";
            __tmp_msh.__msg_data["MSH-4"] = "Medicine-On-Time";

            __nak_string = '\x0B' +
                           @"MSH|^~\&|" +
                           __tmp_msh.__msg_data["MSH-3"] + "|" +
                           __tmp_msh.__msg_data["MSH-4"] + "|" +
                           __tmp_msh.__msg_data["MSH-5"] + "|" +
                           __tmp_msh.__msg_data["MSH-6"] + "|" +
                           __time_stamp + "||NAK^" +
                           __tmp_msh.__msg_data["MSH-9-2"] + "|" +
                           __tmp_msh.__msg_data["MSH-10"] + "|" +
                           __tmp_msh.__msg_data["MSH-11"] + "|" +
                           __tmp_msh.__msg_data["MSH-12"] + "|" +
                           @"\r" +
                           @"MSA|" +
                           __error_code + "|" +
                           __tmp_msh.__msg_data["MSH-10"] + "|" +
                           '\x1C' +
                           '\x0D';
        }
    }
};