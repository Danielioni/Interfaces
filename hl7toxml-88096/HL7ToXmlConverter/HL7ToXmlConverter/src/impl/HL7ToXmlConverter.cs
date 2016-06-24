/**********************************************************************

janoman: HL7 to XML-Parser

HL7ToXmlConverter.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Converter implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    using System.IO;

    /// <summary>
    /// Implementation for Converter to converte HL7 to XL7Xml Version 2
    /// </summary>
    public sealed class HL7ToXmlConverter : IConverter
    {
        #region ctors
        internal HL7ToXmlConverter()
        {
            this.properties = new HL7ConverterProperties();
        }
        #endregion ctors

        #region private members
        private IProperties properties = null;
        private HL7Dictionary hl7Dictionary = null;
        private bool initialized = false;
        private bool strictMode = false;
        private bool useHL7v2Namespace = false;
        private string initString = string.Empty;
        private Dictionary<string, string> messageTypes = null;
        private HL7ConverterMessageDelegate callback = null;
        private string protocol = string.Empty;
        private string encoding = string.Empty;
        #endregion private members



        #region IConverter Member

        /// <summary>
        /// Init converter with grammar
        /// </summary>
        /// <param name="dictionary">Dictionary</param>
        public void Init(Stream dictionary)
        {
            string dict = HL7ToXmlConverterHelper.GetStringFromStream(dictionary); ;
            Init(dict, new Dictionary<string, string>());
        }

        /// <summary>
        /// Init converter with grammar an HL7 Event and Types
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="messageTypes"></param>
        public void Init(Stream dictionary, Dictionary<string, string> messageTypes)
        {
            string dict = HL7ToXmlConverterHelper.GetStringFromStream(dictionary);
            Init(dict, messageTypes);
        }

        /// <summary>
        /// Init converter with grammar
        /// </summary>
        /// <param name="dictionary">Dictionary</param>
        public void Init(string dictionary)
        {
            Init(dictionary, new Dictionary<string,string>());
        }

        /// <summary>
        /// Init converter with grammar an HL7 Event and Types
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="messageTypes"></param>
        public void Init(string dictionary, Dictionary<string, string> messageTypes)
        {
            CallbackHelper.Info(ref this.callback, "Hl72XmlConverter::Init Entry Function");
            Evaluater.ThrowIfTrue(this.initialized, "Converter always initialized", true);

            try
            {
                Evaluater.ThrowIfNullOrIsEmpty(dictionary, "HL7-Grammar", true);

                InitializeWithProperties();

                this.hl7Dictionary = new HL7Dictionary();
                this.hl7Dictionary.Init(dictionary);

                this.messageTypes = messageTypes;

                this.initialized = true;
            }
            catch (Exception e)
            {
                throw new HL7ToXmlConverterException(e.Message, e);
            }
            finally
            {
                CallbackHelper.Info(ref this.callback, "Hl72XmlConverter::Init Exit Function");
            }
        }

        /// <summary>
        /// Convert HL7 Data stream to HL7Xml Version 2 Message as string
        /// </summary>
        /// <param name="hl7">HL7 Message</param>
        /// <returns>HL7Xml Version 2 Message</returns>
        public string Convert(Stream hl7)
        {
            string message = HL7ToXmlConverterHelper.GetStringFromStream(hl7);
            return Convert(message);
        }

        /// <summary>
        /// Convert HL7 Data string to HL7Xml Version 2 Message as string
        /// </summary>
        /// <param name="hl7">HL7 Message</param>
        /// <returns>HL7Xml Version 2 Message</returns>
        public string Convert(string hl7)
        {
            CallbackHelper.Info(ref this.callback, "Hl72XmlConverter::Convert Entry Function");
            Evaluater.ThrowIfFalse(this.initialized, "Converter is not initialized", true);
            string output = string.Empty;

            this.protocol = string.Empty;

            var scanner = new HL7Scanner();
            scanner.SetCallback(this.callback);
            scanner.Init(this.messageTypes);
            scanner.StrictMode = this.strictMode;
            scanner.Hl7v2Namesspace = this.useHL7v2Namespace;
            scanner.LoadMessage(hl7);

            var xmlOutput = new HL7XmlOutput(this.encoding);

            IAnalyzer analyzer = new HL7Analyzer(xmlOutput, (IScanner)scanner, this.hl7Dictionary, useHL7v2Namespace);
            analyzer.SetCallback(this.callback);

            try
            {
                analyzer.Run();
            }
            catch
            {
                throw;
            }
                finally
            {
                this.protocol = analyzer.GetMessageProtocol();
                CallbackHelper.Info(ref this.callback, "Hl72XmlConverter::Convert Exit Function");
            }

            return xmlOutput.GetXml();

        }

        /// <summary>
        /// Retrieve configuration Properties
        /// </summary>
        public IProperties Properties
        {
            get 
            { 
                return this.properties; 
            }
        }

        /// <summary>
        /// Registrate callback to log messages to logging framework
        /// </summary>
        /// <param name="callback">delegate to handle</param>
        public void SubcsribeConverterMessages(HL7ConverterMessageDelegate callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Retrieve the Protocol for convertion
        /// </summary>
        /// <returns>Protocol</returns>
        public string GetProtocol()
        {
            return this.protocol;
        }

        #endregion

        #region private methods
        private void InitializeWithProperties()
        {
            if (this.properties.FindBoolProperty("strict-mode"))
                this.strictMode = this.properties.GetBoolProperty("strict-mode");

            if (this.properties.FindBoolProperty("hl7-namespace"))
                this.useHL7v2Namespace = this.properties.GetBoolProperty("hl7-namespace");

            if (this.properties.FindStringProperty("encoding"))
                this.encoding = this.properties.GetStringProperty("encoding");
        }
        #endregion private methods


    }
}
