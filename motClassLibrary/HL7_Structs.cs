using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace motInboundLib
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
                    __tagname = __tagname + "-" + __minor.ToString();
                    __sub_parse(__tagname, __field, __field[__gotone], __message_data, __minor); // recurse
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

            int __major = 1;
            int __minor = 1;
            int __gotone;

            string[] __field_names = __message.Split(__delims);
            string __tagname = __field_names[0];

            if (__tagname == "MSH")
            {
                __major = 3;
            }

            foreach (string __field in __field_names)
            {
                if ((__field == __tagname) || (__field == @"^~\&"))
                {
                    continue;
                }

                __message_data.Add(__tagname + "-" + __major.ToString(), __field);

                __gotone = __field.IndexOfAny(__local_delimiters);    // Subfield ADT01^RDE^RDE_011
                if (__gotone != -1)
                {
                    __tagname = __tagname + "-" + __major.ToString();

                    // Wait for it so we don't loose the overall seque
                    bool __success = __sub_parse(__tagname, __field, __field[__gotone], __message_data, __minor);
                }

                __major++;
            }
        }

        public HL7MessageParser() { }
    }

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

        public ADT_A01(string __message)
        {
            string[] __segments = __message.Split('\r');

            foreach (string __field in __segments)
            {
                switch (__field.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__field);
                        break;

                    case "PID":
                        __pid = new PID(__field);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__field);
                        break;

                    case "EVN":
                        __evn = new EVN(__field);
                        break;

                    case "TQ1":
                        __obx.Add(new OBX(__field));
                        break;

                    case "AL1":
                        __al1.Add(new AL1(__field));
                        break;

                    case "DG1":
                        __dg1.Add(new DG1(__field));
                        break;

                    default:
                        break;
                }
            }
        }

        public ADT_A01()
        {
        }
    }
    public class ADT_A21    // Delete a Patient
    {
        // A23 - Not in Spec
        public ADT_A21()
        {
            throw new NotImplementedException();
        }

    }
    public class OMP_O09    // Pharmacy Treatment Order Message
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

        public OMP_O09(string __message)
        {
            string[] __segments = __message.Split('\r');

            foreach (string __field in __segments)
            {
                switch (__field.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__field);
                        break;

                    case "PID":
                        __pid = new PID(__field);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__field);
                        break;

                    case "ORC":
                        __orc = new ORC(__field);
                        break;

                    case "TQ1":
                        __tq1.Add(new TQ1(__field));
                        break;

                    case "RXO":
                        __rxo = new RXO(__field);
                        break;

                    case "RXC":
                        __rxc.Add(new RXC(__field));
                        break;

                    case "NTE":
                        __nte.Add(new NTE(__field));
                        break;

                    default:
                        break;
                }
            }
        }
    }
    public class RDE_O11    // Pharmacy/Treatment Encoded Order Message
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


        public RDE_O11()
        {
        }

        public RDE_O11(string __message)
        {
            string[] __segments = __message.Split('\r');

            foreach (string __type in __segments)
            {
                switch (__type.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__type);
                        break;

                    case "PID":
                        __pid = new PID(__type);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__type);
                        break;

                    case "TQ1":
                        __tq1.Add(new TQ1(__type));
                        break;

                    case "RXC":
                        __rxc.Add(new RXC(__type));
                        break;

                    case "RXE":
                        __rxe = new RXE(__type);
                        break;

                    case "RXO":
                        __rxo = new RXO(__type);
                        break;

                    case "RXR":
                        __rxr.Add(new RXR(__type));
                        break;

                    case "ORC":  // Need to parse the order
                        __orc = new ORC(__type);
                        break;

                    case "ZAS":
                        __zas = new ZAS(__type);
                        break;

                    case "ZPI":
                        __zpi = new ZPI(__type);
                        break;

                    case "NTE":
                        __nte.Add(new NTE(__type));
                        break;

                    default:
                        break;
                }
            }

            Console.WriteLine("Done Processing, now what ...");
        }
    }
    public class RDS_O13    // Pharmacy/Treatment Dispense Message
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
        public RDS_O13(string __message)
        {
            string[] __segments = __message.Split('\r');

            foreach (string __field in __segments)
            {
                switch (__field.Substring(0, 3))
                {
                    case "MSH":
                        __msh = new MSH(__field);
                        break;

                    case "PID":
                        __pid = new PID(__field);
                        break;

                    case "PV1":
                        __pv1 = new PV1(__field);
                        break;

                    case "ORC":
                        __orc = new ORC(__field);
                        break;

                    case "RXO":
                        __rxo = new RXO(__field);
                        break;

                    case "RXE":
                        __rxe = new RXE(__field);
                        break;

                    case "RXD":
                        __rxd = new RXD(__field);
                        break;

                    case "NTE":
                        __nte.Add(new NTE(__field));
                        break;

                    case "TQ1":
                        __tq1.Add(new TQ1(__field));
                        break;

                    case "RXR":
                        __rxr.Add(new RXR(__field));
                        break;

                    case "RXC":
                        __rxc.Add(new RXC(__field));
                        break;

                    case "ZPI":
                        __zpi = new ZPI(__field);
                        break;

                    default:
                        break;
                }
            }
        }
        public RDS_O13()
        { }
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
        Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        public void __load()
        {
            __field_names.Add("1", new DataPair(@"Set ID", @"AL1"));
            __field_names.Add("2", new DataPair(@"Allergen Type Code"));
            __field_names.Add("2-1", new DataPair(@"Identifier"));
            __field_names.Add("2-2", new DataPair(@"Text"));
            __field_names.Add("2-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("2-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("2-5", new DataPair(@"Alternate Text"));
            __field_names.Add("2-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("3", new DataPair(@"Allergen Code/Mnemonic/Description"));
            __field_names.Add("3-1", new DataPair(@"Identifier"));
            __field_names.Add("3-2", new DataPair(@"Text"));
            __field_names.Add("3-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("3-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("3-5", new DataPair(@"Alternate Text"));
            __field_names.Add("3-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("4", new DataPair(@"Allergen Severity Code"));
            __field_names.Add("4-1", new DataPair(@"Identifier"));
            __field_names.Add("4-2", new DataPair(@"Text"));
            __field_names.Add("4-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("4-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("4-5", new DataPair(@"Alternate Text"));
            __field_names.Add("4-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("5", new DataPair(@"Allergen Reaction Code"));
            __field_names.Add("6", new DataPair(@"Identification Date"));
        }
        public AL1(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);

            Console.WriteLine("Finished parsing AL1");
        }       
    }
    public class DG1
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            // Dictionary<string, KeyValuePair<string, string>> __field_names = new Dictionary<string, KeyValuePair<string, string>>();
            //KeyValuePair<string, object> __field = new KeyValuePair<string, object>(@"Set ID", @"DG1"));       

            __field_names.Add("1", new DataPair(@"Set ID", @"DG1"));
            __field_names.Add("2", new DataPair(@"Diagnosis Coding Method"));
            __field_names.Add("2-1", new DataPair(@"Identifier"));
            __field_names.Add("2-2", new DataPair(@"Text"));
            __field_names.Add("2-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("2-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("2-5", new DataPair(@"Alternate Text"));
            __field_names.Add("2-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("3", new DataPair(@"Diagnosis Code"));
            __field_names.Add("3-1", new DataPair(@"Identifier"));
            __field_names.Add("3-2", new DataPair(@"Text"));
            __field_names.Add("3-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("3-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("3-5", new DataPair(@"Alternate Text"));
            __field_names.Add("3-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("4", new DataPair(@"Diagnosis Description"));
            __field_names.Add("4-1", new DataPair(@"Time"));
            __field_names.Add("5", new DataPair(@"Diagnosis Date/Time"));
            __field_names.Add("6", new DataPair(@"Diagnosis Type"));
            __field_names.Add("7", new DataPair(@"Diagnostic Major Catagory"));
            __field_names.Add("7-1", new DataPair(@"Identifier"));
            __field_names.Add("7-2", new DataPair(@"Text"));
            __field_names.Add("7-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("7-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("7-5", new DataPair(@"Alternate Text"));
            __field_names.Add("7-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("8", new DataPair(@"Diagnostic Related Group"));
            __field_names.Add("8-1", new DataPair(@"Identifier"));
            __field_names.Add("8-2", new DataPair(@"Text"));
            __field_names.Add("8-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("8-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("8-5", new DataPair(@"Alternate Text"));
            __field_names.Add("8-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("9", new DataPair(@"DRG Approval Indicator"));
            __field_names.Add("10", new DataPair(@"DRG Grouper Review Code"));
            __field_names.Add("11", new DataPair(@"Outlier Type"));
            __field_names.Add("11-1", new DataPair(@"Identifier"));
            __field_names.Add("11-2", new DataPair(@"Text"));
            __field_names.Add("11-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("11-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("11-5", new DataPair(@"Alternate Text"));
            __field_names.Add("11-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("12", new DataPair(@"Outlier Days"));
            __field_names.Add("13", new DataPair(@"Outlier Cost"));
            __field_names.Add("13-1", new DataPair(@"Price"));
            __field_names.Add("13-1-1", new DataPair(@"Quantity"));
            __field_names.Add("13-1-2", new DataPair(@"Denomination"));
            __field_names.Add("13-2", new DataPair(@"Price Type"));
            __field_names.Add("13-3", new DataPair(@"From Value"));
            __field_names.Add("13-4", new DataPair(@"To Value"));
            __field_names.Add("13-5", new DataPair(@"Range Units"));
            __field_names.Add("13-5-1", new DataPair(@"Identifier"));
            __field_names.Add("13-5-2", new DataPair(@"Text"));
            __field_names.Add("13-5-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("13-5-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("13-5-5", new DataPair(@"Alternate Text"));
            __field_names.Add("13-5-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("13-6", new DataPair(@"Range Type"));
            __field_names.Add("14", new DataPair(@"Grouper Version and Type"));
            __field_names.Add("15", new DataPair(@"Diagnosis Priority"));
            __field_names.Add("16", new DataPair(@"Diagnosing Clinician"));
            __field_names.Add("16-1", new DataPair(@"ID Number"));
            __field_names.Add("16-2-2", new DataPair(@"Surname"));
            __field_names.Add("16-2-3", new DataPair(@"Own Surname"));
            __field_names.Add("16-2-4", new DataPair(@"Surname Prefix From Partner/Spouse"));
            __field_names.Add("16-2", new DataPair(@"Family Name"));
            __field_names.Add("16-3", new DataPair(@"Given Name"));
            __field_names.Add("16-4", new DataPair(@"Second and Further Given Names or Initials Thereof"));
            __field_names.Add("16-5", new DataPair(@"Suffix"));
            __field_names.Add("16-6", new DataPair(@"Prefix"));
            __field_names.Add("16-7", new DataPair(@"Source Table"));
            __field_names.Add("16-8", new DataPair(@"Assigning Authority"));
            __field_names.Add("16-8-1", new DataPair(@"Namespace ID"));
            __field_names.Add("16-8-2", new DataPair(@"Universal ID"));
            __field_names.Add("16-8-3", new DataPair(@"Universal ID Type"));
            __field_names.Add("16-9", new DataPair(@"Name Type Code"));
            __field_names.Add("16-10", new DataPair(@"Identifier Check Digit"));
            __field_names.Add("16-11", new DataPair(@"Check Digit Scheme"));
            __field_names.Add("16-12", new DataPair(@"Identifier Type Code"));
            __field_names.Add("16-13", new DataPair(@"Assigning Facility"));
            __field_names.Add("16-13-1", new DataPair(@"Namespace ID"));
            __field_names.Add("16-13-2", new DataPair(@"Universal ID"));
            __field_names.Add("16-13-3", new DataPair(@"Universal ID Type"));
            __field_names.Add("16-14", new DataPair(@"Name Representation Code"));
            __field_names.Add("16-15", new DataPair(@"Name Context"));
            __field_names.Add("16-15-1", new DataPair(@"Identifier"));
            __field_names.Add("16-15-2", new DataPair(@"Text"));
            __field_names.Add("16-15-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("16-15-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("16-15-5", new DataPair(@"Alternate Text"));
            __field_names.Add("16-15-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("16-16", new DataPair(@"Name Assembly Order"));
            __field_names.Add("16-17", new DataPair(@"Effective Date"));
            __field_names.Add("16-18", new DataPair(@"Expiration Date"));
            __field_names.Add("16-19", new DataPair(@"Professional Suffix"));
            __field_names.Add("16-20", new DataPair(@"Assigning Juristiction"));
            __field_names.Add("16-20-1", new DataPair(@"Identifier"));
            __field_names.Add("16-20-2", new DataPair(@"Text"));
            __field_names.Add("16-20-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("16-20-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("16-20-5", new DataPair(@"Alternate Text"));
            __field_names.Add("16-20-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("16-21", new DataPair(@"Assigning Agency or Department"));
            __field_names.Add("16-21-1", new DataPair(@"Identifier"));
            __field_names.Add("16-21-2", new DataPair(@"Text"));
            __field_names.Add("16-21-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("16-21-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("16-21-5", new DataPair(@"Alternate Text"));
            __field_names.Add("16-21-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("17", new DataPair(@"Diagnosis Classification"));
            __field_names.Add("18", new DataPair(@"Confidential Indicator"));
            __field_names.Add("19", new DataPair(@"Attestation Date/Time"));
            __field_names.Add("20", new DataPair(@"Diagnosis Identifier"));
            __field_names.Add("20-1", new DataPair(@"Entity Identifier"));
            __field_names.Add("20-2", new DataPair(@"Namespace ID"));
            __field_names.Add("20-3", new DataPair(@"Universal ID"));
            __field_names.Add("20-4", new DataPair(@"Universal ID Type"));
            __field_names.Add("21", new DataPair(@"Diagnosis Action Code"));

        }

        public void __parse(string __dg1_data)
        {
            string[] __flds = __dg1_data.Split('|');

            //Dictionary<string,DataPair>.Enumerator

            var __enumerator = __field_names.GetEnumerator();

            foreach (string __field in __flds)
            {
                __enumerator.Current.Value.__data = __field;
                __enumerator.MoveNext();
            }
        }

        public DG1()
        {
            __load();
        }

        public DG1(string __message)
        {
            __load();
            __parse(__message);
        }
    }
    public class EVN
    {

        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"Event Type Code"));

            __field_names.Add("2", new DataPair(@"Recorded Date/Time"));
            __field_names.Add("2-1", new DataPair(@"Time"));
            __field_names.Add("2-2", new DataPair(@"Degree of Percision"));
            __field_names.Add("3", new DataPair(@"Date/Time Planned Event"));
            __field_names.Add("4", new DataPair(@"Event Reason Code"));
            __field_names.Add("5", new DataPair(@"Operator ID"));
            __field_names.Add("5-1", new DataPair(@"ID Number"));
            __field_names.Add("5-1-1", new DataPair(@"Surname"));
            __field_names.Add("5-1-2", new DataPair(@"Own Surname Prefix"));
            __field_names.Add("5-1-3", new DataPair(@"Surname Prefix From Partner/Spouse"));
            __field_names.Add("5-1-4", new DataPair(@"Surname From Partner/Spouse"));
            __field_names.Add("5-2", new DataPair(@"Family Name"));
            __field_names.Add("5-3", new DataPair(@"Given Name"));
            __field_names.Add("5-4", new DataPair(@"Second and Further Given Names or Initials Thereof"));
            __field_names.Add("5-5", new DataPair(@"Suffix"));
            __field_names.Add("5-6", new DataPair(@"Prefix"));
            __field_names.Add("5-7", new DataPair(@"Degree"));
            __field_names.Add("5-8", new DataPair(@"Source Table"));
            __field_names.Add("5-9", new DataPair(@"Assigning Authority"));
            __field_names.Add("5-9-1", new DataPair(@"Namespace ID"));
            __field_names.Add("5-9-2", new DataPair(@"Universal ID"));
            __field_names.Add("5-9-3", new DataPair(@"Universal ID Type"));
            __field_names.Add("5-10", new DataPair(@"Name Type Code"));
            __field_names.Add("5-11", new DataPair(@"Identifier Check Digit"));
            __field_names.Add("5-12", new DataPair(@"Check Digit Scheme"));
            __field_names.Add("5-13", new DataPair(@"Identifier Type Code"));
            __field_names.Add("5-14", new DataPair(@"Assigning Facility"));
            __field_names.Add("5-14-1", new DataPair(@"Namespace ID"));
            __field_names.Add("5-14-2", new DataPair(@"Universal ID"));
            __field_names.Add("5-14-3", new DataPair(@"Universal ID Type"));
            __field_names.Add("5-15", new DataPair(@"Name Representation Code"));
            __field_names.Add("5-16", new DataPair(@"Name Context"));
            __field_names.Add("5-16-1", new DataPair(@"Text"));
            __field_names.Add("5-16-2", new DataPair(@"Name of Coding System"));
            __field_names.Add("5-16-3", new DataPair(@"Alternate Identifier"));
            __field_names.Add("5-16-4", new DataPair(@"Alternate Text"));
            __field_names.Add("5-16-5", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("5-17", new DataPair(@"Name Validity Range"));
            __field_names.Add("5-17-1", new DataPair(@"Range Start Date/Time"));
            __field_names.Add("5-17-1-1", new DataPair(@"Time"));
            __field_names.Add("5-17-1-2", new DataPair(@"Degree of Percision"));
            __field_names.Add("5-17-2", new DataPair(@"Range End Date/Time"));
            __field_names.Add("5-17-2-1", new DataPair(@"Time"));
            __field_names.Add("5-17-2-2", new DataPair(@"Degree of Percision"));
            __field_names.Add("5-18", new DataPair(@"Name Assembly Order"));
            __field_names.Add("5-19", new DataPair(@"Effective Date"));
            __field_names.Add("5-19-1", new DataPair(@"Time"));
            __field_names.Add("5-19-2", new DataPair(@"Degree of Percision"));
            __field_names.Add("5-20", new DataPair(@"Expiration Date "));
            __field_names.Add("5-20-1", new DataPair(@"Time"));
            __field_names.Add("5-20-2", new DataPair(@"Degree of Percision"));
            __field_names.Add("5-21", new DataPair(@"Profssional Suffix"));
            __field_names.Add("5-22", new DataPair(@"Assigning Juristiction"));
            __field_names.Add("5-22-1", new DataPair(@"Text"));
            __field_names.Add("5-22-2", new DataPair(@"Name of Coding System"));
            __field_names.Add("5-22-3", new DataPair(@"Alternate Identifier"));
            __field_names.Add("5-22-4", new DataPair(@"Alternate Text"));
            __field_names.Add("5-22-5", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("5-23", new DataPair(@"Assigning Agency or Department"));
            __field_names.Add("5-23-1", new DataPair(@"Text"));
            __field_names.Add("5-23-2", new DataPair(@"Name of Coding System"));
            __field_names.Add("5-23-3", new DataPair(@"Alternate Identifier"));
            __field_names.Add("5-23-4", new DataPair(@"Alternate Text"));
            __field_names.Add("5-23-5", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("6", new DataPair(@"Event Occured"));
            __field_names.Add("6-1", new DataPair(@"Time"));
            __field_names.Add("6-2", new DataPair(@"Degree of Percision"));
            __field_names.Add("7", new DataPair(@"Event Facility"));
            __field_names.Add("7-1", new DataPair(@"Namespace ID"));
            __field_names.Add("7-2", new DataPair(@"Universal ID"));
            __field_names.Add("7-3", new DataPair(@"Universal ID Type"));

        }

      

        public EVN()
        {
            __load();
        }

        public EVN(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class MSH
    {
        public Dictionary<string, string> __field_names = new Dictionary<string, string>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
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


            __field_names.Add("0", @"SegmentName");
            __field_names.Add("1", @"Field Separator");
            __field_names.Add("2", @"Encoding Characters");

            __field_names.Add("3", @"Sending Application");
            __field_names.Add("3-1", @"Namespace ID");
            __field_names.Add("3-2", @"Universal ID");
            __field_names.Add("3-3", @"Universal ID Type;");

            __field_names.Add("4", @"Sending Facility");
            __field_names.Add("4-1", @"Namespace ID");
            __field_names.Add("4-2", @"Universal ID");
            __field_names.Add("4-3", @"Universal ID Type;");

            __field_names.Add("5", @"Receiving Application");
            __field_names.Add("5-1", @"Namespace ID");
            __field_names.Add("5-2", @"Universal ID");
            __field_names.Add("5-3", @"Universal ID Type;");

            __field_names.Add("6", @"Receiving Facility");
            __field_names.Add("6-1", @"Namespace ID");
            __field_names.Add("6-2", @"Universal ID");
            __field_names.Add("6-3", @"Universal ID Type;");

            __field_names.Add("7", @"Date/Time of Message");
            __field_names.Add("7-1", @"Time");
            __field_names.Add("7-2", @"Degree of Percision");

            __field_names.Add("8", @"Security");

            __field_names.Add("9", @"Message Type");
            __field_names.Add("9-1", @"Message Code");
            __field_names.Add("9-2", @"Trigger Event");
            __field_names.Add("9-3", @"Message Structure");

            __field_names.Add("10", @"Message Control ID");

            __field_names.Add("11", @"Processing ID");
            __field_names.Add("11-1", @"Processing ID");
            __field_names.Add("11-2", @"Processing Mode");

            __field_names.Add("12", @"Version ID");
            __field_names.Add("12-1", @"Version ID");
            __field_names.Add("12-2", @"Internationalization Code");
            __field_names.Add("12-2-1", @"Identifier");
            __field_names.Add("12-2-2", @"Text");
            __field_names.Add("12-2-3", @"Name of Coding System");
            __field_names.Add("12-2-4", @"Alternate Identifier");
            __field_names.Add("12-2-5", @"Alternate Text");
            __field_names.Add("12-2-6", @"Name of Alternate Coding System");
            __field_names.Add("12-3", @"International Version ID");
            __field_names.Add("12-3-1", @"Text");
            __field_names.Add("12-3-2", @"Name of Coding System");
            __field_names.Add("12-3-3", @"Alternate Identifier");
            __field_names.Add("12-3-4", @"Alternate Text");
            __field_names.Add("12-3-5", @"Name of Alternate Coding System");

            __field_names.Add("13", @"Sequence Number");
            __field_names.Add("14", @"Continuation Pointer");
            __field_names.Add("15", @"Accept Acknowledgement Type");
            __field_names.Add("16", @"Application Acknowledgement Type");
            __field_names.Add("17", @"Country Code");
            __field_names.Add("18", @"Character Set");

            __field_names.Add("19", @"Principal Language of Message");
            __field_names.Add("19-1", @"Identifier");
            __field_names.Add("19-2", @"Text");
            __field_names.Add("19-3", @"Name of Coding System");
            __field_names.Add("19-4", @"Alternate Identifier");
            __field_names.Add("19-5", @"Alternate Text");
            __field_names.Add("19-6", @"Name of Alternate Coding System");

            __field_names.Add("20", @"Alternate Character Set Handling Scheme");
            __field_names.Add("21", @"Message Profile Identifier");
            __field_names.Add("21-1", @"Entity Identifier");
            __field_names.Add("21-2", @"Namespace Identifier");
            __field_names.Add("21-3", @"Universal ID");
            __field_names.Add("21-4", @"Universal ID Type");
        }

     
        public MSH()
        {
            __load();
        }

        public MSH(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);

            Console.WriteLine("Finished parsing MSH");
        }

    }
    public class NTE
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();

        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"Set ID", "NTE"));
            __field_names.Add("2", new DataPair(@"Source of Comment"));
            __field_names.Add("3", new DataPair(@"Comment"));
            __field_names.Add("4", new DataPair(@"Comment Type"));
            __field_names.Add("4-1", new DataPair(@"Identifier"));
            __field_names.Add("4-2", new DataPair(@"Text"));
            __field_names.Add("4-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("4-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("4-5", new DataPair(@"Alternate Text"));
            __field_names.Add("4-6", new DataPair(@"Name of Alternate Coding System"));
        }



        public NTE(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }

        public NTE()
        {
            __load();
        }
    }
    public class OBX
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"Set ID", "OBX"));
            __field_names.Add("2", new DataPair(@"Value Type"));

            __field_names.Add("3", new DataPair(@"Observation Indicator"));
            __field_names.Add("3-1", new DataPair(@"Identifier"));
            __field_names.Add("3-2", new DataPair(@"Text"));
            __field_names.Add("3-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("3-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("3-5", new DataPair(@"Alternate Text"));
            __field_names.Add("3-6", new DataPair(@"Name of Alternate Coding System"));

            __field_names.Add("4", new DataPair(@"Observation Sub ID"));
            __field_names.Add("5", new DataPair(@"Observation Value"));
            __field_names.Add("6", new DataPair(@"Units"));

            __field_names.Add("6-1", new DataPair(@"Identifier"));
            __field_names.Add("6-2", new DataPair(@"Text"));
            __field_names.Add("6-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("6-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("6-5", new DataPair(@"Alternate Text"));
            __field_names.Add("6-6", new DataPair(@"Name of Alternate Coding System"));

            __field_names.Add("7", new DataPair(@"Reference Range"));
            __field_names.Add("8", new DataPair(@"Abnormal Flags"));
            __field_names.Add("9", new DataPair(@"Probability"));
            __field_names.Add("10", new DataPair(@"Nature of Abnormal Test"));
            __field_names.Add("11", new DataPair(@"Observation Result Status"));

            __field_names.Add("12", new DataPair(@"Effective Date of Reference Range"));
            __field_names.Add("12-1", new DataPair(@"Time"));
            __field_names.Add("12-2", new DataPair(@"Degree of Percision"));

            __field_names.Add("13", new DataPair(@"User Defined Access Checks"));

            __field_names.Add("14", new DataPair(@"Date/Time of the Observation"));
            __field_names.Add("14-1", new DataPair(@"Time"));
            __field_names.Add("14-2", new DataPair(@"Degree of Percision"));

            __field_names.Add("15", new DataPair(@"Producer's ID"));
            __field_names.Add("15-1", new DataPair(@"Identifier"));
            __field_names.Add("15-2", new DataPair(@"Text"));
            __field_names.Add("15-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("15-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("15-5", new DataPair(@"Alternate Text"));
            __field_names.Add("15-6", new DataPair(@"Name of Alternate Coding System"));

            __field_names.Add("16", new DataPair(@"Responsible Observer"));
            // ...

            __field_names.Add("17", new DataPair(@"Observation Method"));
            // ...

            __field_names.Add("18", new DataPair(@"Equipment Instance Identifier"));
            // ...

            __field_names.Add("19", new DataPair(@"Date/Time of the Analysis"));
            // ...
        }

        public OBX()
        {
            __load();
        }

        public OBX(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ORC
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();

        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"ID"));
            __field_names.Add("2", new DataPair(@"Placer Order Number"));
            __field_names.Add("3", new DataPair(@"Filler Order Number"));
            __field_names.Add("4", new DataPair(@"Placer Group Number"));
            __field_names.Add("5", new DataPair(@"Order Status"));
            __field_names.Add("6", new DataPair(@"Response Flag"));
            __field_names.Add("7", new DataPair(@"Quantity/Timing"));
            __field_names.Add("8", new DataPair(@"Parent"));
            __field_names.Add("9", new DataPair(@"Date/Time of Transaction"));
            __field_names.Add("10", new DataPair(@"Entered By"));
            __field_names.Add("11", new DataPair(@"Verified By"));
            __field_names.Add("12", new DataPair(@"Ordering Provider"));
            __field_names.Add("13", new DataPair(@"Enterer's Location"));
            __field_names.Add("14", new DataPair(@"Call Back Phone Number"));
            __field_names.Add("15", new DataPair(@"Order Effective Date/Time"));
            __field_names.Add("16", new DataPair(@"Order Control Reason"));
            __field_names.Add("17", new DataPair(@"Entering Organization"));
            __field_names.Add("18", new DataPair(@"Entering Device"));
            __field_names.Add("19", new DataPair(@"Action By"));
            __field_names.Add("20", new DataPair(@"Advanced Beneficiary Notice Code"));
            __field_names.Add("21", new DataPair(@"Ordering Facility Name"));
            __field_names.Add("22", new DataPair(@"Ordering Facility Address"));
            __field_names.Add("23", new DataPair(@"Ordering Facility Phone Number"));
            __field_names.Add("24", new DataPair(@"Ordering Provider Address"));
            __field_names.Add("25", new DataPair(@"Order Status Modifier"));
            __field_names.Add("26", new DataPair(@"Advanced Benificiary Notice Override Reason"));
            __field_names.Add("27", new DataPair(@"Filler's Expected Availability Date/Time"));
            __field_names.Add("28", new DataPair(@"Confidentiality Code"));
            __field_names.Add("29", new DataPair(@"Order Type"));
            __field_names.Add("30", new DataPair(@"Enterer Authorization Mode"));
        }
        public ORC()
        {
            __load();
        }

        public ORC(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class PID
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"Set ID", "PID"));
            __field_names.Add("2", new DataPair(@"Patient ID"));
            __field_names.Add("3", new DataPair(@"Patient Identifier List"));
            __field_names.Add("4", new DataPair(@"Alternate Pathient ID - PID"));
            __field_names.Add("5", new DataPair(@"Patient Name"));
            __field_names.Add("6", new DataPair(@"Mothers Maiden Name"));
            __field_names.Add("7", new DataPair(@"Date/Time of Birth"));
            __field_names.Add("8", new DataPair(@"Administrative Sex"));
            __field_names.Add("9", new DataPair(@"Patient Alias"));
            __field_names.Add("10", new DataPair(@"Race"));
            __field_names.Add("11", new DataPair(@"Patient Address"));
            __field_names.Add("12", new DataPair(@"County Code"));
            __field_names.Add("13", new DataPair(@"Phone Number - Home"));
            __field_names.Add("14", new DataPair(@"Phone Number - Business"));
            __field_names.Add("15", new DataPair(@"Primary Language"));
            __field_names.Add("16", new DataPair(@"Marital Status"));
            __field_names.Add("17", new DataPair(@"Religion"));
            __field_names.Add("18", new DataPair(@"Patient Account Number"));
            __field_names.Add("19", new DataPair(@"SSN - Patient"));
            __field_names.Add("20", new DataPair(@"Drivers License Number - Patient"));
            __field_names.Add("21", new DataPair(@"Mother's Identifier"));
            __field_names.Add("22", new DataPair(@"Ethnic Group"));
            __field_names.Add("23", new DataPair(@"Birth Place"));
            __field_names.Add("24", new DataPair(@"Multiple Birth Indicator"));
            __field_names.Add("25", new DataPair(@"Birth Order"));
            __field_names.Add("26", new DataPair(@"Citizenship"));
            __field_names.Add("27", new DataPair(@"Veterens Military Status"));
            __field_names.Add("28", new DataPair(@"Nationality"));
            __field_names.Add("29", new DataPair(@"Patient Death Date/Time"));
            __field_names.Add("30", new DataPair(@"Patient Deth Indictor"));
            __field_names.Add("31", new DataPair(@"Identity Unknown Indicator"));
            __field_names.Add("32", new DataPair(@"Identity Reliability Code"));
            __field_names.Add("33", new DataPair(@"Last Update Date/Time"));
            __field_names.Add("34", new DataPair(@"Last Update Facility"));
            __field_names.Add("35", new DataPair(@"Species Code"));
            __field_names.Add("36", new DataPair(@"Breed Code"));
            __field_names.Add("37", new DataPair(@"Strain"));
            __field_names.Add("38", new DataPair(@"Production Class Code"));
            __field_names.Add("39", new DataPair(@"Tribal Citizenship"));
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
    public class PV1
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"Set ID", "OBX"));
            __field_names.Add("2", new DataPair(@"Value Type"));
            __field_names.Add("3", new DataPair(@"Observation Indicator"));
            __field_names.Add("4", new DataPair(@"Observation Sub ID"));
            __field_names.Add("5", new DataPair(@"Observation Value"));
            __field_names.Add("6", new DataPair(@"Units"));
            __field_names.Add("7", new DataPair(@"Reference Range"));
            __field_names.Add("8", new DataPair(@"Abnormal Flags"));
            __field_names.Add("9", new DataPair(@"Probability"));
            __field_names.Add("10", new DataPair(@"Nature of Abnormal Test"));
            __field_names.Add("11", new DataPair(@"Set ID", "OBX"));
            __field_names.Add("12", new DataPair(@"Value Type"));
            __field_names.Add("13", new DataPair(@"Observation Indicator"));
            __field_names.Add("14", new DataPair(@"Observation Sub ID"));
            __field_names.Add("15", new DataPair(@"Observation Value"));
            __field_names.Add("16", new DataPair(@"Units"));
            __field_names.Add("17", new DataPair(@"Reference Range"));
            __field_names.Add("18", new DataPair(@"Abnormal Flags"));
            __field_names.Add("19", new DataPair(@"Probability"));
            __field_names.Add("20", new DataPair(@"Nature of Abnormal Test"));
            __field_names.Add("21", new DataPair(@"Set ID", "OBX"));
            __field_names.Add("22", new DataPair(@"Value Type"));
            __field_names.Add("23", new DataPair(@"Observation Indicator"));
            __field_names.Add("24", new DataPair(@"Observation Sub ID"));
            __field_names.Add("25", new DataPair(@"Observation Value"));
            __field_names.Add("26", new DataPair(@"Units"));
            __field_names.Add("27", new DataPair(@"Reference Range"));
            __field_names.Add("28", new DataPair(@"Abnormal Flags"));
            __field_names.Add("29", new DataPair(@"Probability"));
            __field_names.Add("30", new DataPair(@"Nature of Abnormal Test"));
            __field_names.Add("31", new DataPair(@"Set ID", "OBX"));
            __field_names.Add("32", new DataPair(@"Value Type"));
            __field_names.Add("33", new DataPair(@"Observation Indicator"));
            __field_names.Add("34", new DataPair(@"Observation Sub ID"));
            __field_names.Add("35", new DataPair(@"Observation Value"));
            __field_names.Add("36", new DataPair(@"Units"));
            __field_names.Add("37", new DataPair(@"Reference Range"));
            __field_names.Add("38", new DataPair(@"Abnormal Flags"));
            __field_names.Add("39", new DataPair(@"Probability"));
            __field_names.Add("40", new DataPair(@"Nature of Abnormal Test"));
            __field_names.Add("41", new DataPair(@"Set ID", "OBX"));
            __field_names.Add("42", new DataPair(@"Value Type"));
            __field_names.Add("43", new DataPair(@"Observation Indicator"));
            __field_names.Add("44", new DataPair(@"Observation Sub ID"));
            __field_names.Add("45", new DataPair(@"Observation Value"));
            __field_names.Add("46", new DataPair(@"Units"));
            __field_names.Add("47", new DataPair(@"Reference Range"));
            __field_names.Add("48", new DataPair(@"Abnormal Flags"));
            __field_names.Add("49", new DataPair(@"Probability"));
            __field_names.Add("50", new DataPair(@"Nature of Abnormal Test"));
        }
        public PV1()
        {
            __load();
        }

        public PV1(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXC
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
            __field_names.Add("1", new DataPair(@"RX Component Type"));

            __field_names.Add("2", new DataPair(@"Component Code"));
            __field_names.Add("2-1", new DataPair(@"Identifier"));
            __field_names.Add("2-2", new DataPair(@"Text"));
            __field_names.Add("2-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("2-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("2-5", new DataPair(@"Alternate Text"));
            __field_names.Add("2-6", new DataPair(@"Name of Alternate CodingSystem"));

            __field_names.Add("3", new DataPair(@"Component Amount"));

            __field_names.Add("4", new DataPair(@"Component Units"));
            __field_names.Add("4-1", new DataPair(@"Identifier"));
            __field_names.Add("4-2", new DataPair(@"Text"));
            __field_names.Add("4-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("4-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("4-5", new DataPair(@"Alternate Text"));
            __field_names.Add("4-6", new DataPair(@"NAme of Alternate Coding System"));

            __field_names.Add("5", new DataPair(@"Component Strength"));

            __field_names.Add("6", new DataPair(@"Component Strength Units"));
            __field_names.Add("6-1", new DataPair(@"Identifier"));
            __field_names.Add("6-2", new DataPair(@"Text"));
            __field_names.Add("6-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("6-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("6-5", new DataPair(@"Alternate Text"));
            __field_names.Add("6-6", new DataPair(@"NAme of Alternate Coding System"));

            __field_names.Add("7", new DataPair(@"Suplementary Code"));
            __field_names.Add("7-1", new DataPair(@"Identifier"));
            __field_names.Add("7-2", new DataPair(@"Text"));
            __field_names.Add("7-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("7-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("7-5", new DataPair(@"Alternate Text"));
            __field_names.Add("7-6", new DataPair(@"Name of Alternate Coding System"));

            __field_names.Add("8", new DataPair(@"Component Drug Volume"));

            __field_names.Add("9", new DataPair(@"Component Drug Volume Units"));
            __field_names.Add("9-1", new DataPair(@"Identifier"));
            __field_names.Add("9-2", new DataPair(@"Text"));
            __field_names.Add("9-3", new DataPair(@"Name of Coding System"));
            __field_names.Add("9-4", new DataPair(@"Alternate Identifier"));
            __field_names.Add("9-5", new DataPair(@"Alternate Text"));
            __field_names.Add("9-6", new DataPair(@"Name of Alternate Coding System"));
            __field_names.Add("9-7", new DataPair(@"Coding System Version ID"));
            __field_names.Add("9-8", new DataPair(@"Alternate Coding System Version"));
            __field_names.Add("9-9", new DataPair(@"Original Text"));
        }
        public RXC()
        {
            __load();
        }

        public RXC(string __message)
        {
            __load();
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXD
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXD()
        {
        }

        public RXD(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXE
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXE()
        {
        }

        public RXE(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXO
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser = new HL7MessageParser();

        private void __load()
        {
        }

        public RXO()
        {
        }

        public RXO(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class RXR
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser;


        private void __load()
        {
        }

        public RXR()
        {
        }

        public RXR(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class TQ1
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser;

        private void __load()
        {
        }

        public TQ1()
        {
        }

        public TQ1(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZAS
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser;

        private void __load()
        {
        }

        public ZAS()
        {
        }

        public ZAS(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }
    public class ZPI
    {
        public Dictionary<string, DataPair> __field_names = new Dictionary<string, DataPair>();
        public Dictionary<string, string> __msg_data = new Dictionary<string, string>();
        HL7MessageParser __parser;

        private void __load()
        {
        }

        public ZPI()
        {
        }

        public ZPI(string __message)
        {
            __parser.__parse(__message, __msg_data);
        }
    }

    // static void Main(string[] args)
    // {
    //     Run();
    // }
};
