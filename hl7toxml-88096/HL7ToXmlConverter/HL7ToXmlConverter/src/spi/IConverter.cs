/**********************************************************************

janoman: HL7 to XML-Parser

IConverter.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief converter interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Janoman.Healthcare.HL7
{

    /// <summary>
    /// Message Callback Delegate. Can be used to map messages to any Logging framework
    /// </summary>
    /// <param name="message">message to log</param>
    /// <param name="level">level to log</param>
    public delegate void HL7ConverterMessageDelegate(string message, LogLevel level);

    /// <summary>
    /// Converter interface
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Initialize Converter
        /// </summary>
        /// <param name="dictionary">distionary content</param>
        /// <param name="messageTypes">List of messagetype
        /// <code>
        /// Dictionary@lt;string, string&gt; eventsMessages = new Dictionary&lt;string, string&gt;();
        /// eventsMessages.Add("ADT_A01", "ADT_A01");
        /// eventsMessages.Add("ADT_A02", "ADT_A02");
        /// eventsMessages.Add("ADT_A03", "ADT_A03");
        /// eventsMessages.Add("ADT_A04", "ADT_A01");
        /// eventsMessages.Add("ADT_A05", "ADT_A01");
        /// eventsMessages.Add("ADT_A12", "ADT_A12");
        /// </code>
        /// </param>
        void Init(string dictionary, Dictionary<string, string> messageTypes);
        /// <summary>
        /// Initialize Converter
        /// </summary>
        /// <param name="dictionary">distionary content</param>
        /// <param name="messageTypes">List of messagetype
        /// <code>
        /// Dictionary&lt;string, string&gt; eventsMessages = new Dictionary&lt;string, string&gt;();
        /// eventsMessages.Add("ADT_A01", "ADT_A01");
        /// eventsMessages.Add("ADT_A02", "ADT_A02");
        /// eventsMessages.Add("ADT_A03", "ADT_A03");
        /// eventsMessages.Add("ADT_A04", "ADT_A01");
        /// eventsMessages.Add("ADT_A05", "ADT_A01");
        /// eventsMessages.Add("ADT_A12", "ADT_A12");
        /// </code>
        /// </param>
        void Init(Stream dictionary, Dictionary<string, string> messageTypes);
        /// <summary>
        /// Initialize Converter
        /// </summary>
        /// <param name="dictionary">distionary content</param>
        void Init(string dictionary);
        /// <summary>
        /// Initialize Converter
        /// </summary>
        /// <param name="dictionary">distionary content</param>
        void Init(Stream dictionary);
        /// <summary>
        /// Properties to configure converter
        /// <code>
        /// IConverter converter = HL72XmlConverterFactory.Create();
        /// converter.Properties.SetBoolProperty("strict-mode", false);
        /// converter.Properties.SetBoolProperty("hl7-namespace", true);
        /// </code>
        /// </summary>
        IProperties Properties {get;}
        /// <summary>
        /// Convert HL7 to HL7 XML Version 2
        /// </summary>
        /// <param name="hl7">HL7-Message</param>
        /// <returns>HL7-Message as XML</returns>
        string Convert(Stream hl7);
        /// <summary>
        /// Convert HL7 to HL7 XML Version 2
        /// </summary>
        /// <param name="hl7">HL7-Message</param>
        /// <returns>HL7-Message as XML</returns>
        string Convert(string hl7);
        /// <summary>
        /// Subcribe callback to receive logging events from converter and internal components of this library.
        /// </summary>
        /// <param name="callback">Callback function</param>
        void SubcsribeConverterMessages(HL7ConverterMessageDelegate callback);
        /// <summary>
        /// deliver a detailed protocol for the transformation
        /// </summary>
        /// <returns></returns>
        string GetProtocol();
    }
}
