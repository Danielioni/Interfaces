/**********************************************************************

janoman: HL7 to XML-Parser

HL7Analyzer.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Analyzer implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Janoman.Healthcare.HL7
{

    internal sealed class HL7Analyzer : IContentHandler, IAnalyzer
    {
        #region private contants
        private const string CLASSNAME = "HL7Analyzer";
        private const string INDENT     = "                                                ";
        #endregion private constants
        
        #region private members
        private int                         accept          = 0;
        private int                         level           = 0;
        private bool                        inSegment;
        private bool                        running;
        private bool                        useHL7v2Namespace;

        private IXmlOutput                  xmlOutput       = null;
        private IScanner                    hl7Scanner      = null;
        private IDictionary                 hl7Dictionary   = null;

        private HL7ElementStack             elementStack    = null;
        private IParser                     hl7Parser       = null;

        private StringBuilder               protocolBuilder = null;

        private HL7ConverterMessageDelegate callback        = null;
        #endregion private members

        #region ctors
        public HL7Analyzer(IXmlOutput xmlOutput, IScanner scanner, IDictionary dictionary, bool useNamespace)
        {
            this.xmlOutput = xmlOutput;
            this.hl7Scanner = scanner;
            this.hl7Dictionary = dictionary;
            this.useHL7v2Namespace = useNamespace;
            this.hl7Parser = new HL7Parser();
            this.hl7Parser.ContentHandler = this;
            this.elementStack = new HL7ElementStack();
        }
        #endregion ctors

        #region IAnalyzer Member
        public void Run()
        {
            const string FUNCTION = "::Run ";
            CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");
            
            this.accept = 0;
            this.level = -1;
            this.inSegment = false;

            this.running = true;

            this.protocolBuilder = new StringBuilder();

            try
            {
                string messageType = this.hl7Scanner.GetMessageType();
                
                Evaluater.ThrowAndCallbackIfNullOrIsEmpty(messageType, "Messagetype", true, ref this.callback);

                string content = this.hl7Dictionary.Lookup(messageType);

                Evaluater.ThrowIfNullOrIsEmpty(content,string.Format("No Grammar found for {0}",messageType) , true);

                ParserState state = this.hl7Parser.ParseDictionaryEntry(content, messageType);

                Evaluater.ThrowAndCallbackIfParserError(state, string.Empty, true, ref this.callback);

                string error = string.Empty;
                string token = string.Empty;
                var tokenPair = this.hl7Scanner.GetNextToken();
                if (HL7Token.EOF != tokenPair.Token &&
                    !string.IsNullOrEmpty(tokenPair.Tag))
                {
                    //throw new HL7AnalyzerException();
                }
            }
            catch (Exception e)
            {
                throw new HL7AnalyzerException(e.Message,e);
            }
            finally
            {
                this.running = false;
            }
        }

        public void SetCallback(HL7ConverterMessageDelegate callback)
        {
            this.callback = callback;
        }

        public string GetMessageProtocol()
        {
            const string FUNCTION = "::GetMessageProtocol ";
            CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");

            if (null != this.protocolBuilder)
            {
                return this.protocolBuilder.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region IContentHandler Member

        public ParserState StartElement(HL7Element element)
        {
            Evaluater.ThrowAndCallbackIfFalse(this.running, "Analyzer not running", true, ref this.callback);
            if (this.level >= this.accept)
            {
                return ParserState.NoValue;
            }

            Log(" StartElement     ");
            Log(string.Format(" {0} {1}", this.level, this.accept));
            Log(string.Format(" {0} {1}", element.Level, element.Depth));
            Log(" " + element.Name);
            LogOptionalRepeatable(element);
            Log("\n");

            bool segmentLevel = false;
            ParserState state = ParserState.NoValue;
            string attribute = string.Empty;
            string tag = element.Name;
            var pair = this.hl7Dictionary.LookupAndAlsoAttributes(tag);
            {
                var e = new HL7Element(tag, new Dictionary<string, string>());
                this.elementStack.Add(e);
                Log(string.Format("{0} T: ", INDENT));
                this.elementStack.Dump(ref this.protocolBuilder);
            }

            if (0 == element.Level
                && 0 == this.accept
                && IsSegment(tag))
            {
                segmentLevel = true;
                {
                    Log("******** Start Segment: " + tag + " **************\n");
                    Log("-> expecting ");
                    Log("token \"" + tag + "\"\n");
                }
                var newPair = this.hl7Scanner.GetNextToken();

                Log("next token after Segmentlabel: " + GetTokenName(newPair.Token) + "\n");

                if (!(HL7Token.FieldSeparator == newPair.Token
                    || HL7Token.SegmentTerminator == newPair.Token
                    || HL7Token.EOF == newPair.Token))
                {
                    this.hl7Scanner.Dump();
                    Evaluater.ThrowAndCallback("Unexpected EOF", true, ref this.callback);
                }

                Log("found segment \"" + newPair.Tag + "\" in input stream\n");

                newPair.Tag.Trim();
                if (tag.Equals(newPair.Tag))
                {
                    //alles ok
                    {
                        Log("SEG  [");
                        Log(newPair.Tag);
                        Log("]\n");
                    }
                    this.inSegment = true;
                    state = ParserState.Segment;
                    state |= ParserState.Repeatable;
                }
                else
                {
                    this.accept = 0;
                    {
                        Log("SEG  [");
                        Log(newPair.Tag);
                        Log("] ** ignoring " + tag + " **\n");
                    }
                    this.hl7Scanner.PutBackToken();
                    this.elementStack.RemoveAt(this.elementStack.Count - 1);
                    return ParserState.NoValue;
                }
                this.level++;
                // expect anything after segment has started
                this.accept = 3;
            }

            if (this.level < this.accept
                && this.level >= 0
                && this.inSegment)
            {
                this.accept = 3;
            }

            ParserState state1 = ParserState.UnknownState;
            element.Empty = true;

            if (this.level <= this.accept)
            {
                // Rekursion
                state1 = this.hl7Parser.ParseDictionaryEntry(pair.Key, tag);

                Evaluater.ThrowAndCallbackIfLessThanParserNoValue(state1, 
                    string.Empty, true, ref this.callback);

            }
            state |= state1;

            if ((state & ParserState.Value) == ParserState.Value)
            {
                element.Empty = false;
            }

            if (segmentLevel)
            {
                this.level--;

                Log("********   End Segment: " + tag + " **************\n");
                Log("-> scanning for ");
                Log("token <Segment delimiter>\n");

            }

            EndElement(element);

            if (!element.Empty)
            {
                this.elementStack.Mark();
                this.elementStack.WriteXml(this.xmlOutput);
            }
            Log(string.Format("{0} T: ", INDENT));
            
            this.elementStack.Dump(ref this.protocolBuilder);
            this.elementStack.RemoveAt(this.elementStack.Count - 1);

            this.hl7Scanner.GetCurrentToken();

            return state;
        }

        public ParserState EndElement(HL7Element element)
        {
            Evaluater.ThrowAndCallbackIfFalse(this.running, 
                "Analyzer not running", true, ref this.callback);
            
            ParserState state = ParserState.NoValue;
            HL7TokenPair hl7TokenPair = null;
            string s = string.Empty;

            Log(" EndElement       ");
            Log(string.Format(" {0} {1}", this.level, this.accept));
            Log(string.Format(" {0} {1}", element.Level, element.Depth));
            LogOptionalRepeatable(element);
            if (!element.Empty) Log(" #");
            Log("\n");

            hl7TokenPair = this.hl7Scanner.GetCurrentToken();
            HL7Token hl7Token = hl7TokenPair.Token;

            if (0 > this.level)
            {
                while ((HL7Token.FieldSeparator == hl7Token
                    || HL7Token.ComponentSeparator == hl7Token
                    || HL7Token.SubComponenteSeparator == hl7Token)
                    && 0 != this.accept)
                {
                    hl7TokenPair = this.hl7Scanner.GetNextToken();
                    hl7Token = hl7TokenPair.Token;

                    Log(INDENT + " ");
                    Log(string.Format("WARN: ignoring extra field: {0}\n", hl7TokenPair.Tag));
                    Log(string.Format("{0} next: {1}\n", INDENT, GetTokenName(hl7Token)));
                }
                this.accept = 0;            // expect SEGMENT
                this.inSegment = false;
            }
            else if (0 == this.level)
            {
                if (element.Repeatable)
                {
                    while (HL7Token.ComponentSeparator == hl7Token
                        || HL7Token.SubComponenteSeparator == hl7Token)
                    {
                        hl7TokenPair = this.hl7Scanner.GetNextToken();
                        hl7Token = hl7TokenPair.Token;

                        Log(INDENT + " ");
                        Log(string.Format("WARN: ignoring extra component: {0}\n", hl7TokenPair.Tag));
                        Log(string.Format("{0} next: {1}\n", INDENT, GetTokenName(hl7Token)));
                    }
                }
                else
                {
                    while (HL7Token.ComponentSeparator == hl7Token
                        || HL7Token.SubComponenteSeparator == hl7Token
                        || HL7Token.RepeatSeparator == hl7Token)
                    {
                        hl7TokenPair = this.hl7Scanner.GetNextToken();
                        hl7Token = hl7TokenPair.Token;

                        Log(INDENT + " ");
                        Log(string.Format("WARN: ignoring extra non-repeatable field: {0}\n", hl7TokenPair.Tag));
                        Log(string.Format("{0} next: {1}\n", INDENT, GetTokenName(hl7Token)));
                    }
                }
            }
            else if (1 == this.level)
            {
                while (HL7Token.SubComponenteSeparator == hl7Token)
                {
                    hl7TokenPair = this.hl7Scanner.GetNextToken();
                    hl7Token = hl7TokenPair.Token;

                    Log(INDENT + " ");
                    Log(string.Format("WARN: ignoring extra subcomponent: {0}\n", hl7TokenPair.Tag));
                    Log(string.Format("{0} next: {1}\n", INDENT, GetTokenName(hl7Token)));
                }
            }
            else if (2 == this.level)
            {
                // Subcomponents
            }
            else
            {
                // unknown
            }

            return state;
        }

        public ParserState StartComplexType(HL7Element element)
        {
            try
            {
                Evaluater.ThrowAndCallbackIfFalse(this.running, 
                    "Analyzer not running", true, ref this.callback);

                ParserState state = ParserState.NoValue;

                this.level++;

                Log(" StartComplexType ");
                Log(string.Format(" {0} {1}", this.level, this.accept));
                Log(string.Format(" {0} {1}", element.Level, element.Depth));
                Log(" " + element.Name);
                LogOptionalRepeatable(element);
                Log("\n");


                string tmp = string.Empty;
                string type = "%" + element.Name;

                string definition = this.hl7Dictionary.Lookup(type);
                if ("(#PCDATA)".Equals(definition)
                    || "(#ANY)".Equals(definition)
                    || "(PCDATA)".Equals(definition)
                    || "(ANY)".Equals(definition))
                {
                    {
                        Log(INDENT + " ");
                        switch (element.Level)
                        {
                            case 0:
                                Log("FLD: ");
                                break;
                            case 1:
                                Log("CMP: ");
                                break;
                            case 2:
                                Log("SUB: ");
                                break;
                            default:
                                Log("???: ");
                                break;
                        }
                    }
                    if (this.level <= this.accept)
                    {
                        var hl7TokenPair = this.hl7Scanner.GetNextToken();
                        if (!string.IsNullOrEmpty(hl7TokenPair.Tag))
                        {
                            hl7TokenPair.Tag.Trim();

                            if (!string.IsNullOrEmpty(hl7TokenPair.Tag))
                                Log(string.Format(" [{0}]", hl7TokenPair.Tag));
                            if (!(0 == this.elementStack.Count))
                            {
                                this.elementStack[this.elementStack.Count - 1].Text = hl7TokenPair.Tag;
                            }
                            state = ParserState.Value;
                        }

                        Log("\n");
                        Log(string.Format("{0} next: {1}\n", INDENT, GetTokenName(hl7TokenPair.Token)));

                        this.accept = 0;
                        switch (hl7TokenPair.Token)
                        {
                            case HL7Token.EOF:			// OPTION: EOF is treated as segment terminator
                                this.accept = 0;	    // expect SEGMENT
                                //fout << INDENT << " ** accept SEG" << endl;
                                this.inSegment = false;
                                return state;	        // segment			
                            case HL7Token.SegmentTerminator:
                                // continue with next segment
                                switch (element.Level)
                                {
                                    case 0:			// field level
                                    case 1:			// component level
                                    case 2:			// subcomponent level
                                        break;
                                    default:
                                        Evaluater.ThrowAndCallback("Unexpected Level", 
                                            true, ref this.callback);
                                        return ParserState.Error;
                                }
                                this.accept = 0;	// expect SEGMENT
                                //** accept SEG" << endl;
                                this.inSegment = false;
                                return state;	    // segment			
                            case HL7Token.FieldSeparator:
                                // continue with next field
                                switch (element.Level)
                                {
                                    case 0:			// field level
                                        break;
                                    case 1:			// component level
                                    case 2:			// subcomponent level
                                        break;
                                    default:
                                        Evaluater.ThrowAndCallback("Unexpected Level",
                                            true, ref this.callback);
                                        return ParserState.Error;
                                }
                                this.accept = 1;	// expect FIELD
                                //fout << INDENT << " ** accept FLD(1)" << endl;
                                return state;	    //field
                            case HL7Token.ComponentSeparator:
                                // continue with next component
                                switch (element.Level)
                                {
                                    case 0:			// field level
                                    case 1:			// component level
                                        break;
                                    case 2:			// subcomponent level
                                        break;
                                    default:
                                        Evaluater.ThrowAndCallback("Unexpected Level",
                                            true, ref this.callback);
                                        return ParserState.Error;
                                }
                                this.accept = 2;	// expect COMPONENT
                                //fout << INDENT << " ** accept CMP(2)" << endl;
                                return state;	// component
                            case HL7Token.SubComponenteSeparator:
                                // continue with next subcomponent
                                switch (element.Level)
                                {
                                    case 0:			// field level
                                    case 1:			// component level
                                    case 2:			// subcomponent level
                                        break;
                                    default:
                                        Evaluater.ThrowAndCallback("Unexpected Level",
                                            true, ref this.callback);
                                        return ParserState.Error;
                                }
                                this.accept = 3;	// expect SUBCOMPONENT
                                //fout << INDENT << " ** accept SUB(3)" << endl;
                                return state;	    // subcomponent
                            case HL7Token.RepeatSeparator:
                                // repeat current field
                                switch (element.Level)
                                {
                                    case 0:			// field level
                                        break;
                                    case 1:			// component level
                                    case 2:			// subcomponent level
                                        break;
                                    default:
                                        Evaluater.ThrowAndCallback("Unexpected Level",
                                            true, ref this.callback);
                                        return ParserState.Error;
                                }
                                this.accept = 1;	// expect field
                                //fout << INDENT << " ** accept FLD(1)" << endl;
                                state |= ParserState.Repeatable;		// set repeat-bit
                                return state;							// repeat current field
                            case HL7Token.Error:
                                Evaluater.ThrowAndCallback("Unexpected Error",
                                    true, ref this.callback);
                                return ParserState.Error;
                            default:
                                Evaluater.ThrowAndCallback("Unexpected Error",
                                    true, ref this.callback);
                                return ParserState.Error;
                        }
                    }
                }
                else
                {
                    if (this.level <= this.accept)
                    {
                        state = this.hl7Parser.ParseDictionaryEntry(definition, type);

                        state |= ParserState.Recursion;
                    }
                }
                return state;
            }
            catch
            {
                throw;
            }
        }

        public ParserState EndComplexType(HL7Element element)
        {
            Evaluater.ThrowAndCallbackIfFalse(this.running, 
                "Analyzer not running", true, ref this.callback);

            Log(" EndComplexType   ");
            Log(string.Format(" {0} {1}", this.level, this.accept));
            Log(string.Format(" {0} {1}", element.Level, element.Depth));
            LogOptionalRepeatable(element);
            Log("\n");
          
            ParserState state = ParserState.NoValue;

            this.level--;

            return state;
        }

        public ParserState StartSequence(HL7Sequence sequence)
        {
            Evaluater.ThrowAndCallbackIfFalse(this.running, 
                "Analyzer not running", true, ref this.callback);

            Log(" StartSequence    ");
            Log(string.Format(" {0} {1}",this.level,this.accept)); 
            Log(string.Format(" {0} {1}", sequence.Level, sequence.Depth));
            Log(" " + sequence.Name);
            LogOptionalRepeatable(sequence);
            Log("\n");

            if (1 == sequence.Depth)
            {
                var element = sequence.ToElement();
                if (useHL7v2Namespace)
                {
                    element.AddAttribute("xmlns", "urn:hl7-org:v2xml");
                }
                this.elementStack.Clear();
                this.elementStack.Add(element);
                this.elementStack.Dump(ref this.protocolBuilder);
            }

            return ParserState.NoValue;
        }

        public ParserState EndSequence(HL7Sequence sequence)
        {
            Evaluater.ThrowAndCallbackIfFalse(this.running, 
                "Analyzer not running", true, ref this.callback);
            
            ParserState state = ParserState.NoValue;

            if (1 == sequence.Depth)
            {
                if (!sequence.Empty)
                {
                    this.elementStack.WriteXml(this.xmlOutput);
                }
                this.elementStack.Dump(ref this.protocolBuilder);
                this.elementStack.RemoveAt(this.elementStack.Count - 1);
            }

            Log(" EndSequence      ");
            Log(string.Format(" {0} {1}", this.level, this.accept));
            Log(string.Format(" {0} {1}", sequence.Level, sequence.Depth));
            Log(" " + sequence.Name);
            LogOptionalRepeatable(sequence);
            Log(string.Format("  [{0}]", sequence.Occur));
            if (!sequence.Empty) Log(" #");
            Log("\n");

            return state;
        }

        #endregion

        #region private methods
        private string GetTokenName(HL7Token hl7Token)
        {
            switch (hl7Token)
            {
                case HL7Token.SegmentTerminator:
                    return "segment terminiator";
                case HL7Token.FieldSeparator:
                    return "field separator";
                case HL7Token.ComponentSeparator:
                    return "component separator";
                case HL7Token.SubComponenteSeparator:
                    return "subcomponent separator";
                case HL7Token.RepeatSeparator:
                    return "repeat separator";
                case HL7Token.EOF:
                    return "EOF";
                case HL7Token.Error:
                    return "ERROR";
                default:
                    return "***UNKNOWN TOKEN***";
            }
        }

        private bool IsSegment(string segment)
        {
            if (3 == segment.Length &&
                -1 == segment.IndexOf('.') &&
                -1 == segment.IndexOf('_'))
            {
                return true;
            }
            return false;
        }

        private bool IsXmlUnderStructure(string segment)
        {
            if (3 == segment.IndexOf('_'))
            {
                return true;
            }
            return false;
        }

        private void LogOptionalRepeatable(HL7Element element)
        {
            if (null == this.protocolBuilder)
                return;
            if (element.Optional)
                this.protocolBuilder.Append(" ?");
            if (element.Repeatable)
                this.protocolBuilder.Append(" *");
        }

        private void Log(string message)
        {
            if (null == this.protocolBuilder)
                return;
            this.protocolBuilder.Append(message);
        }
        #endregion private methods
    }
}
