using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using motCommonLib;
using motInboundLib;
using HL7toXDocumentParser;

namespace motHL7Tests
{
    [TestClass]
    public class hl7Segmenttests
    {
        [TestMethod]
        public void testMSH()
        {
            var __msh = new MSH(@"MSH |^ ~\&| ADT1 | MCM | LABADT | MCM | 198808181126 | SECURITY | ADT ^ A01 | MSG00001 | P | 2.4");
            var __ack = new ACK(__msh);
            Console.WriteLine("ACK: {0}", __ack.__ack_string);

            var __nak = new NAK(__msh, "AF");
            Console.WriteLine("NAK: {0}", __nak.__nak_string);

            __msh = new MSH(@"MSH|^~\&|FrameworkLTC|DELPHI|100|MAYQ|20110802115657||RDE^O11^RDE_O11|36652|P|2.5||||||ASCII|||");
            __ack = new ACK(__msh);
            Console.WriteLine("ACK: {0}", __ack.__ack_string);

            __nak = new NAK(__msh, "AF");
            Console.WriteLine("NAK: {0}", __nak.__nak_string);

            __msh = new MSH(@"MSH|^~\&|FrameworkLTC|DELPHI|100|MAYQ|20110802115657||RDE ^O11^ RDE_O11|36652|P|2.5||||||ASCII|||");
            __ack = new ACK(__msh);
            Console.WriteLine("ACK: {0}", __ack.__ack_string);

            __nak = new NAK(__msh, "AF");
            Console.WriteLine("NAK: {0}", __nak.__nak_string);
        }

        [TestMethod]
        public void testPID()
        {
            /*
            var __pid = new PID(@"PID|1|JS567002|MQNH\F\567675||Smith^Jeremy^A^^^^D||19860103000000|F|||100 Some Street^Apt #10^Pittsburgh^PA^15215||(412)781-0001|||||MR0000004|111-22-3333|||||||||||N|||||||||");
            __pid = new PID(@"PID|||PATID1234^5^M11||JONES^WILLIAM^A^III||19610615|M-||2106-3|1200 N ELM STREET^^GREENSBORO^NC^27401-1020|GL|(919)379-1212|(919)271-3434~(919)277-3114||S||PATID12345001^2^M10|123456789|9-87654^NC");
            */
        }

        [TestMethod]
        public void testNK1()
        {
            var __nk1 = new NK1(@"NK1 | 1 | JONES ^ BARBARA ^ K | SPO ||||| 20011105");
            __nk1 = new NK1(@"NK1 | 1 | JONES ^ MICHAEL ^ A | FTH");
        }

        [TestMethod]
        public void testPV1()
        {
            var __pv1 = new PV1(@"PV1|1|I|2000^2012^01||||004777^LEBAUER^SIDNEY^J.|||SUR||-||1|A0-"); 
        }

        [TestMethod]
        public void testAL1()
        {
            /*
            var __HL7xml = new HL7toXDocumentParser.Parser(); 
            var xDoc = __HL7xml.Parse(@"AL1|1||^PENICILLIN||PRODUCES HIVES~RASH");

            var xElement = xDoc.Root;
            var __al1 = new AL1(xElement);

            // var __al1 = new AL1(@"AL1|1||^PENICILLIN||PRODUCES HIVES~RASH");

            xDoc = __HL7xml.Parse(@"AL1|2||^CAT DANDER~FLEAS~HAIR~FUR||Produces SNEEZING~RASH~EYE WATERING~EAR PAIN");
            xElement = xDoc.Root;
            __al1 = new AL1(xElement);

            //__al1 = new AL1(@"AL1|2||^CAT DANDER~FLEAS~HAIR~FUR||Produces SNEEZING~RASH~EYE WATERING~EAR PAIN");
            */
        }
    }
}
