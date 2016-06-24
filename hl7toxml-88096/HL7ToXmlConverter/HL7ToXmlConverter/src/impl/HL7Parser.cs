/**********************************************************************

janoman: HL7 to XML-Parser

HL7Parser.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Parser implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    internal sealed class HL7Parser : IParser
    {

        private int position = 0;

        private int bracketNesting = 0;
        private int recursionLevel = 0;
        private IContentHandler contentHandler = null;

        private string grammarEntry = string.Empty;
        private string grammarRootName = string.Empty;

        private HL7ConverterMessageDelegate callback = null;

        #region IParser Member

        /// <summary>
        /// parse is called when starting with a new declaration (after "lookup")
        /// </summary>
        /// <param name="grammarEntry">Grammatikeintrag im Dictionary</param>
        /// <param name="grammarRootName">Name des Grammatik-Items</param>
        /// <returns>Status des Parser</returns>
        public ParserState ParseDictionaryEntry(string grammarEntry, string grammarRootName)
        {
            this.grammarEntry = grammarEntry;
            this.grammarRootName = grammarRootName;
            this.position = 0;


            ParserState parserState = ParserState.NoValue;
            ParserState ret = ParserState.NoValue;

            ParseDictionaryEntryState state = ParseDictionaryEntryState.Begin;
            string type = string.Empty;
            string debug = string.Empty;

            while (ParseDictionaryEntryState.End != state
                && ParseDictionaryEntryState.Error != state)
            {
                debug = this.grammarEntry.Substring(this.position);
                char c = '\0';
                if (!string.IsNullOrEmpty(debug))
                {
                    c = Convert.ToChar(this.grammarEntry.Substring(this.position, 1));
                }

                switch (state)
                {
                    case ParseDictionaryEntryState.Begin:
                        state = ParseDictionaryEntryState.TrimLeft;
                        break;
                    case ParseDictionaryEntryState.TrimLeft:
                        switch (c)
                        {
                            case ' ':
                                break;
                            case '\t':
                                this.position++;
                                break;
                            case '%':
                                this.position++;
                                state = ParseDictionaryEntryState.ReadType;
                                break;
                            case '(':
                                this.position++;
                                state = ParseDictionaryEntryState.ReadSequence;
                                break;
                            default:
                                ret = ParserState.Error1;
                                state = ParseDictionaryEntryState.Error;
                                DumpError(ret, parserState, position, grammarEntry);
                                parserState = ParserState.Error;
                                break;
                        }
                        break;
                    case ParseDictionaryEntryState.ReadType:
                        if (IsIdChar(c))
                        {
                            type += Convert.ToString(c);
                            this.position++;
                        }
                        else
                        {
                            switch (c)
                            {
                                case ' ':
                                case '\0':
                                    var element = new HL7Element();
                                    element.Name = type;
                                    element.Depth = this.bracketNesting;
                                    element.Level = this.recursionLevel;
                                    this.recursionLevel++;
                                    // start analyzing component/subcomponent of a certain type	%XYZ
                                    // if type is contains primitive, we might read a value
                                    ret = this.contentHandler.StartComplexType(element);
                                    this.contentHandler.EndComplexType(element);
                                    this.recursionLevel--;
                                    if (ParserState.NoValue > ret)
                                    {
                                        DumpError(ret, parserState, position, grammarEntry);
                                        state = ParseDictionaryEntryState.Error;
                                    }
                                    else
                                    {
                                        state = ParseDictionaryEntryState.End;
                                    }
                                    break;
                                default:
                                    ret = ParserState.IllegalChar;
                                    DumpError(ret, parserState, position, grammarEntry);
                                    state = ParseDictionaryEntryState.Error;
                                    parserState = ParserState.IllegalChar;
                                    break;
                            }
                        }
                        break;
                    case ParseDictionaryEntryState.ReadSequence:
                        {
                            this.bracketNesting++;
                            var sequence = new HL7Sequence();
                            sequence.Name = this.grammarRootName;

                            ret = ParseBracket(sequence, true);
                            this.bracketNesting--;
                            if (ParserState.NoValue > ret)
                            {
                                state = ParseDictionaryEntryState.Error;
                                DumpError(ret, parserState, position, grammarEntry);
                                parserState = ParserState.Parse2Failed;
                            }
                            else
                            {
                                state = ParseDictionaryEntryState.End;
                            }
                        }
                        break;
                    case ParseDictionaryEntryState.End:
                        return ret;
                    case ParseDictionaryEntryState.Error:
                        throw new HL7ParserException();
                    default:
                        ret = ParserState.UnknownState;
                        parserState = ParserState.Error;
                        DumpError(ret, parserState, position, grammarEntry);
                        state = ParseDictionaryEntryState.Error;
                        break;

                }
            }

            return ret;
        }

        public IContentHandler ContentHandler
        {
            set
            {
                Evaluater.ThrowIfNull(value, "ContentHandler", true);

                contentHandler = value;
            }
        }

        #endregion

        #region private methods
        private ParserState SequenceHandler(HL7Sequence sequence)
        {
            Evaluater.ThrowIfNull(sequence, "Sequence", true);

            ParserState state = ParserState.NoValue;
            
            this.contentHandler.StartSequence(sequence);
            
            bool empty = false;

            while (!empty)
            {
                empty = true;
                foreach (HL7Element element in sequence.Elements)
                {
                    ParserState state1 = ParserState.NoValue;
                    if (!(element is HL7Sequence))
                    {
                        ParserState state2;
                        while (true)
                        {
                            state2 = this.contentHandler.StartElement(element);

                            Evaluater.ThrowIfLessThanParserNoValue(state2, string.Empty, true);
                            
                            if ((ParserState.Value & state2) == ParserState.Value) 
                                element.Empty = false;
                            
                            // collect bits
                            state1 |= state2;
                            
                            if (!element.Repeatable 
                                || !((state2 & ParserState.Repeatable) == ParserState.Repeatable))
                                break;
                            //reset REP-Flag if repitition is done
                            if (element.Repeatable)
                                state2 = state2 & (~ParserState.Repeatable);
                        }
                    }
                    else
                    {
                        var childSequence = (HL7Sequence)element;
                        childSequence.Name = "@@@UNNAME@@@";
                        //Recursion
                        state1 = SequenceHandler(childSequence);
                        if ((ParserState.Value & state1) == ParserState.Value)
                            childSequence.Empty = false;
                    }

                    Evaluater.ThrowIfLessThanParserNoValue(state1, string.Empty, true);

                    if((ParserState.Value & state1) == ParserState.Value)
                        empty = false;
                    //collect bits
                    state |= state1;
                }
                if(!empty)
                {
                    sequence.Empty = false;
                    sequence.Occur++;
                }
                if (!sequence.Repeatable || !((ParserState.Repeatable & state) == 0))
                {
                    //sequence.Repeatable = true;
                    break;
                }
                //reset REP-Flag if repitition is done
                if (sequence.Repeatable)
                    state = state & (~ParserState.Repeatable);
            }

            this.contentHandler.EndSequence(sequence);

            return state;
        }

        private ParserState ParseBracket(HL7Sequence sequence, bool first)
        {
            ParserState parserState = ParserState.Begin;
            ParserState ret = ParserState.NoValue;

            TypeOccur elementOccur = TypeOccur.Required;
            TypeOccur sequenceOccur = TypeOccur.Required;
            int start = 0;

            Evaluater.ThrowIfNull(sequence, "Sequence", true);

            var parentSequence = sequence/*.Clone()*/;
            
            var newSequence = new HL7Sequence();
            parentSequence.Elements.Add(newSequence);

            string elementName = string.Empty;
            string debug = string.Empty;

            while (ParserState.End != parserState && ParserState.Error != parserState)
            {
                debug = this.grammarEntry.Substring(this.position);
                char c = '\0';
                if(!string.IsNullOrEmpty(debug))
                {
                    c = Convert.ToChar(this.grammarEntry.Substring(this.position, 1));
                }
                switch (parserState)
                {
                    case ParserState.Begin:
                        parserState = ParserState.TrimLeft;
                        break;
                    case ParserState.TrimLeft:
                        if (IsIdChar(c) || '#'.Equals(c))
                        {
                            parserState = ParserState.ReadElement;
                            elementName = string.Empty;
                            elementOccur = TypeOccur.Required;
                            start = this.position;
                            if ('#'.Equals(c))
                            {
                                this.position++;
                            }
                        }
                        else
                        {
                            switch (c)
                            {
                                case ' ':
                                case '\t':
                                    this.position++;
                                    break;
                                case '(':
                                    this.position++;
                                    parserState = ParserState.ReadSequence;
                                    break;
                                default:
                                    parserState = ParserState.Error;
                                    break;
                            }
                        }
                        break;
                    case ParserState.ReadElement:
                        if (IsIdChar(c))
                        {
                            elementName += Convert.ToString(c);
                            this.position++;
                        }
                        else
                        {
                            switch (c)
                            {
                                case '*':
                                    // elt is optional and repeatable
                                    elementOccur |= TypeOccur.Optional;
                                    elementOccur |= TypeOccur.Repeatable;
                                    this.position++;
                                    parserState = ParserState.TrimRight;
                                    break;
                                case '+':
                                    // elt is repeatable
                                    elementOccur |= TypeOccur.Repeatable;
                                    this.position++;
                                    parserState = ParserState.TrimRight;
                                    break;
                                case '?':
                                    // elt is optional
                                    elementOccur = TypeOccur.Optional;
                                    this.position++;
                                    parserState = ParserState.TrimRight;
                                    break;
                                case ' ':
                                case ')':
                                case ',':
                                    parserState = ParserState.TrimRight;
                                    break;
                                default:
                                    ret = ParserState.IllegalChar;
                                    DumpError(ret, parserState, position, this.grammarEntry);
                                    parserState = ParserState.Error;
                                    break;
                            }
                        }
                        break;
                    case ParserState.TrimRight:
                        {
                            var element = new HL7Element();
                            element.Name = elementName;
                            element.Optional = (elementOccur & TypeOccur.Optional) == TypeOccur.Optional;
                            // inherit from seq
                            element.Repeatable = sequence.Repeatable 
                                || (elementOccur & TypeOccur.Repeatable) == TypeOccur.Repeatable;
                            element.Depth = this.bracketNesting;
                            element.Level = this.recursionLevel;
                            // collect elements
                            newSequence.Elements.Add(element);
                        }
                        switch (c)
                        {
                            case ' ':
                            case '\t':
                                this.position++;
                                break;
                            case ',':
                            case ')':
                                parserState = ParserState.ReadDelimeter;
                                break;
                            default:
                                ret = ParserState.IllegalChar;
                                DumpError(ret, parserState, position, this.grammarEntry);
                                parserState = ParserState.Error;
                                break;
                        }
                        break;
                    case ParserState.ReadSequence:
                        this.bracketNesting++;
                        // parse inner bracket expression
                        // ParseBracket called recursivly
                        ret = ParseBracket(newSequence, false);
                        this.bracketNesting--;

                        if (ParserState.NoValue > ret)
                        {
                            DumpError(ret, parserState, position, this.grammarEntry);
                            parserState = ParserState.Error;
                        }
                        else
                        {
                            parserState = ParserState.ReadDelimeter;
                        }

                        break;
                    case ParserState.ReadDelimeter:
                        switch (c)
                        {
                            case ')':
                                this.position++;
                                parserState = ParserState.SequenceOccur;
                                sequenceOccur = TypeOccur.Required;
                                break;
                            case ',':
                                this.position++;
                                parserState = ParserState.TrimLeft;
                                break;
                            default:
                                ret = ParserState.IllegalChar;
                                DumpError(ret, parserState, position, this.grammarEntry);
                                parserState = ParserState.Error;
                                break;
                        }
                        break;
                    case ParserState.SequenceOccur:
                        switch (c)
                        {
                            case '*':
                                this.position++;
                                // seq is optional and repeatable
                                sequenceOccur = TypeOccur.Optional | TypeOccur.Repeatable;
                                parserState = ParserState.SequenceTrim;
                                break;
                            case '?':
                                this.position++;
                                // seq is optional
                                sequenceOccur = TypeOccur.Optional;
                                parserState = ParserState.SequenceTrim;
                                break;
                            case '+':
                                this.position++;
                                // seq is repeatable
                                sequenceOccur = TypeOccur.Repeatable;
                                parserState = ParserState.SequenceTrim;
                                break;
                            default:
                                parserState = ParserState.SequenceTrim;
                                break;
                        }
                        break;
                    case ParserState.SequenceTrim:
                        newSequence.Name = parentSequence.Name;
                        newSequence.Optional = (sequenceOccur & TypeOccur.Optional) > TypeOccur.Required;
                        newSequence.Repeatable = (sequenceOccur & TypeOccur.Repeatable) > TypeOccur.Required;
                        newSequence.Depth = this.bracketNesting;
                        newSequence.Level = this.recursionLevel;
                        if (first)
                        {
                            ret = SequenceHandler(newSequence);
                        }
                        switch (c)
                        {
                            case ' ':
                            case '\t':
                                this.position++;
                                break;
                            case '\0':
                            case ',':
                            case ')':
                                return ret;
                            default:
                                ret = ParserState.IllegalChar;
                                DumpError(ret, parserState, position, this.grammarEntry);
                                parserState = ParserState.Error;
                                break;
                        }
                        break;
                    case ParserState.End:
                        return ret;
                    case ParserState.Error:
                        return ret;
                    default:
                        ret = ParserState.UnknownState;
                        parserState = ParserState.Error;
                        DumpError(ret, parserState, position, this.grammarEntry);
                        break;
                }
            }

            return ret;
        }

        private bool IsIdChar(char c)
        {
            bool ret = false;
            if (!'\0'.Equals(c))
            {
                if (0 <= "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-".IndexOf(Convert.ToString(c)))
                {
                    ret = true;
                }
            }
            return ret;
        }

        private void DumpError(ParserState ret, ParserState parserState, int position, string str)
        {
            DumpError(ret, parserState, position, str, null);
        }


        private void DumpError(ParserState ret, ParserState parserState, int position, string str, string text)
        {
            string stateText = parserState.ToString();
            string error = string.Format("{0}: error {1}, state {2}, position {3}", text, ret, parserState, position);
            if (position >= 0 && position < str.Length)
            {
                string left = str.Substring(0, position);
                string right = str.Substring(position);
                error = "\n" + left + "<----\n--->" + right;
            }
            if (null != this.callback)
                callback(error, LogLevel.Error);
        }
        #endregion private methods

        #region ICallback Member

        public void SetCallback(HL7ConverterMessageDelegate callback)
        {
            this.callback = callback;
        }

        #endregion
    }
}
