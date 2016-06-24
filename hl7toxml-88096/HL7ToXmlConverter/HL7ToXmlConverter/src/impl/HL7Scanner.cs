/**********************************************************************

janoman: HL7 to XML-Parser

HL7Scanner.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Scanner implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    internal sealed class HL7Scanner : IScanner
    {

        #region private contants
        private const string CLASSNAME = "HL7Scanner";
        #endregion private contants

        #region ctor
        public HL7Scanner()
        {
        }
        #endregion ctor

        #region IScanner Member

        public string GetMessageType()
        {
            const string FUNCTION = "::GetMessageType ";
            //CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");

            int position = 0, position1 = 0, position2 = 0;
            int field = 0;
            int length = this.message.Length;
            string tmpMsg = string.Empty;
            string msgEvent = string.Empty;
            string msgType = string.Empty;

            while (position < length)
            {
                if (this.message.Substring(position, 1).Equals(Convert.ToString(this.sepField)))
                {
                    field++;
                }
                else
                {
                    Evaluater.ThrowAndCallbackIfStringEqual(this.message.Substring(position, 1),
                        Convert.ToString(this.termSegment), string.Empty, true, ref this.callback);
                }

                position++;
                // MSH.9
                if (9 > field)
                {
                    continue;
                }

                position1 = position;

                while (position < length)
                {
                    if (this.message.Substring(position, 1).Equals(Convert.ToString(this.sepComponent)))
                    {
                        // Start der nächsten Komponente
                        // Nachrichtentyp lesen
                        tmpMsg = this.message.Substring(position1, position - position1);
                        position++;

                        position2 = position;
                        while (position < length)
                        {
                            if (this.message.Substring(position, 1).Equals(Convert.ToString(this.sepField))
                                || this.message.Substring(position, 1).Equals(Convert.ToString(this.sepComponent))
                                || this.message.Substring(position, 1).Equals(Convert.ToString(this.sepSubComponent))
                                || this.message.Substring(position, 1).Equals(Convert.ToString(this.repeat))
                                || this.message.Substring(position, 1).Equals(Convert.ToString(this.escape))
                                || this.message.Substring(position, 1).Equals(Convert.ToString(this.termSegment)))
                            {
                                msgEvent = this.message.Substring(position2, position - position2);
                                msgType = tmpMsg + "_" + msgEvent;
                                if (this.hl7EventsAndMessages.ContainsKey(msgType))
                                {
                                    msgType = this.hl7EventsAndMessages[msgType];
                                }
                                CallbackHelper.Info(ref this.callback, CLASSNAME + FUNCTION + "Messagetype=" + msgType);
                                return msgType;
                            }
                            position++;
                        }
                        break;
                    }
                    else if (this.message.Substring(position, 1).Equals(Convert.ToString(this.sepField)))
                    {
                        tmpMsg = this.message.Substring(position1, position - position1);

                        position++;
                        msgType = tmpMsg;

                        if (this.hl7EventsAndMessages.ContainsKey(msgType))
                        {
                            msgType = this.hl7EventsAndMessages[msgType];
                        }
                        CallbackHelper.Info(ref this.callback, CLASSNAME + FUNCTION + "Messagetype=" + msgType);
                        return msgType;
                    }
                    else
                    {
                        Evaluater.ThrowIfStringEqual(this.message.Substring(position, 1),
                            Convert.ToString(this.termSegment), string.Empty, true);
                    }
                    position++;
                }
                break;
            }
            return Evaluater.ThrowAndCallback("No messagetype evaluated within this message", true, ref this.callback).ToString();
        }

        private void GetNextTokenFromPosition(HL7TokenPair pair, char c, ref string akku)
        {
            if (this.termSegment.Equals(c))
            {
                pair.Tag = DeEscape(akku);
                this.position++;
                // OPTION: if CR is followed by LF: ignore silently
                if (this.position < this.length &&
                    this.cLF.Equals(Convert.ToChar(this.message.Substring(this.position, 1))))
                {
                    if (true == this.segmentTerminator ||
                        true == this.strictMode)
                    {
                        CallbackHelper.Error(ref this.callback,
                            "HL7Scanner:GetNextToken Segment Terminator is not conform to the HL7 standard [LINE FEED]");
                        pair.Token = HL7Token.Error;
                        pair.Readed = true;
                        return;
                    }
                    // just ignore it
                    this.position++;
                }
                pair.Token = HL7Token.SegmentTerminator;
                pair.Readed = true;
                this.nextToken = pair;
                return;
            }
            if (this.cLF.Equals(c))
            {
                if (true == this.segmentTerminator ||
                    true == this.strictMode)
                {
                    CallbackHelper.Error(ref this.callback,
                        "HL7Scanner:GetNextToken Segment Terminator is not conform to the HL7 standard [LINE FEED]");
                    pair.Readed = true;
                    pair.Token = HL7Token.Error;
                    return;
                }

                pair.Tag = DeEscape(akku);
                this.position++;
                pair.Token = HL7Token.SegmentTerminator;
                pair.Readed = true;
                this.nextToken = pair;
                return;
            }
            if (this.sepField.Equals(c))
            {
                pair.Tag = DeEscape(akku);
                this.position++;
                pair.Token = HL7Token.FieldSeparator;
                pair.Readed = true;
                this.nextToken = pair;
                return;
            }
            if (this.sepComponent.Equals(c))
            {
                pair.Tag = DeEscape(akku);
                this.position++;
                pair.Token = HL7Token.ComponentSeparator;
                pair.Readed = true;
                this.nextToken = pair;
                return;
            }
            if (this.sepSubComponent.Equals(c))
            {
                pair.Tag = DeEscape(akku);
                this.position++;
                pair.Token = HL7Token.SubComponenteSeparator;
                pair.Readed = true;
                this.nextToken = pair;
                return;
            }
            if (this.repeat.Equals(c))
            {
                pair.Tag = DeEscape(akku);
                this.position++;
                pair.Token = HL7Token.RepeatSeparator;
                pair.Readed = true;
                this.nextToken = pair;
                return;
            }
            pair.Readed = false;
            Evaluater.ThrowAndCallbackIfCharLessThan(c, '\x20', string.Empty, true, ref this.callback);
            akku += Convert.ToString(c);
            this.position++;
        }

        public HL7TokenPair GetNextToken()
        {
            var pair = new HL7TokenPair();
            string akku = string.Empty;
            string tmp = string.Empty;
            char c = '\0';
            //store previous position
            this.previous = this.position;

            while (this.position < this.length)
            {

                c = Convert.ToChar(this.message.Substring(this.position, 1));

                if (c == '\0')
                {
                    this.length = this.position;
                    continue;
                }

                GetNextTokenFromPosition(pair, c, ref akku);

                if (pair.Readed == true)
                    return pair;

                //if (this.termSegment.Equals(c))
                //{
                //    pair.Tag = DeEscape(akku);
                //    this.position++;
                //    // OPTION: if CR is followed by LF: ignore silently
                //    if (this.position < this.length)
                //    {
                //        if (this.cLF.Equals(Convert.ToChar(this.message.Substring(this.position, 1))))
                //        {
                //            if (true == this.segmentTerminator ||
                //                true == this.strictMode)
                //            {
                //                if (null != this.callback)
                //                    callback("HL7Scanner:GetNextToken Segment Terminator is not conform to the HL7 standard [LINE FEED]", LogLevel.Error);
                //                pair.Token = HL7Token.Error;
                //                return pair;
                //            }
                //            // just ignore it
                //            this.position++;
                //        }
                //    }
                //    pair.Token = HL7Token.SegmentTerminator;
                //    this.nextToken = pair;
                //    return pair;
                //}
                //if (this.cLF.Equals(c))
                //{
                //    if (true == this.segmentTerminator ||
                //        true == this.strictMode)
                //    {
                //        if (null != this.callback)
                //            callback("HL7Scanner:GetNextToken Segment Terminator is not conform to the HL7 standard [LINE FEED]", LogLevel.Error);
                //        pair.Token = HL7Token.Error;
                //        return pair;
                //    }

                //    pair.Tag = DeEscape(akku);
                //    this.position++;
                //    pair.Token = HL7Token.SegmentTerminator;
                //    this.nextToken = pair;
                //    return pair;
                //}
                //if (this.sepField.Equals(c))
                //{
                //    pair.Tag = DeEscape(akku);
                //    this.position++;
                //    pair.Token = HL7Token.FieldSeparator;
                //    this.nextToken = pair;
                //    return pair;
                //}
                //if (this.sepComponent.Equals(c))
                //{
                //    pair.Tag = DeEscape(akku);
                //    this.position++;
                //    pair.Token = HL7Token.ComponentSeparator;
                //    this.nextToken = pair;
                //    return pair;
                //}
                //if (this.sepSubComponent.Equals(c))
                //{
                //    pair.Tag = DeEscape(akku);
                //    this.position++;
                //    pair.Token = HL7Token.SubComponenteSeparator;
                //    this.nextToken = pair;
                //    return pair;
                //}
                //if (this.repeat.Equals(c))
                //{
                //    pair.Tag = DeEscape(akku);
                //    this.position++;
                //    pair.Token = HL7Token.RepeatSeparator;
                //    this.nextToken = pair;
                //    return pair;
                //}

                //Evaluater.ThrowAndCallbackIfCharLessThan(c, '\x20', string.Empty, true, ref this.callback);

                //akku += Convert.ToString(c);
                //this.position++;
            }
            pair.Token = HL7Token.EOF;
            this.nextToken = pair;
            return pair;
        }

        public HL7TokenPair GetCurrentToken()
        {
            return this.nextToken;
        }

        public void PutBackToken()
        {
            // restore previous position
            this.position = this.previous;
        }

        public void Dump()
        {
            Evaluater.ThrowAndCallbackIfFalse(this.initialized, "HL7Scanner not initialized.", true, ref this.callback);

            string error = string.Empty;
            error = "\nHL7-MEssage:\n";
            error += this.message.Substring(0, this.previous);
            error += "<---\n";
            error += "Current Token: \"";
            error += this.message.Substring(this.previous, this.position - this.previous);
            error += "\"";
        }

        public int GetPosition()
        {
            Evaluater.ThrowAndCallbackIfFalse(this.initialized, "HL7Scanner not initialized.", true, ref this.callback);

            return this.position;
        }

        public string GetMessage()
        {
            const string FUNCTION = "::GetMessage ";
            CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");

            return this.message;
        }

        #endregion

        #region public methods
        public void Init(Dictionary<string, string> hl7EventsAndMessages)
        {
            const string FUNCTION = "::Init ";
            CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");

            Evaluater.ThrowAndCallbackIfNull(hl7EventsAndMessages, "HL7 Events and Messages List", true, ref this.callback);

            this.hl7EventsAndMessages = hl7EventsAndMessages;
            this.initialized = true;
        }

        public void LoadMessage(string message)
        {
            const string FUNCTION = "::LoadMessage ";

            Evaluater.ThrowAndCallbackIfNullOrIsEmpty(message, "HL7 Message", true, ref this.callback);

            this.message = string.Empty;
            this.position = 0;
            this.previous = 0;
            this.length = 0;

            this.message = message;

            GetDelimiters();

            this.length = this.message.Length;

            CallbackHelper.Debug(ref this.callback,
                CLASSNAME + FUNCTION + string.Format("Message length={0}", this.length));
        }
        #endregion public methods

        #region public members

        private bool segmentTerminator = false;

        public bool SegmentTerminator
        {
            get { return segmentTerminator; }
            set { segmentTerminator = value; }
        }

        private bool strictMode = false;

        public bool StrictMode
        {
            get { return strictMode; }
            set
            {
                strictMode = value;
                const string FUNCTION = "::StrictMode ";
                CallbackHelper.Debug(ref callback,
                    CLASSNAME + FUNCTION + string.Format("StrictMode {0}", this.strictMode));
            }
        }

        private bool hl7v2Namesspace = false;

        public bool Hl7v2Namesspace
        {
            get { return hl7v2Namesspace; }
            set
            {
                hl7v2Namesspace = value;
                const string FUNCTION = "::StrictMode ";
                CallbackHelper.Debug(ref callback,
                    CLASSNAME + FUNCTION + string.Format("Hl7v2 namespace {0}", this.strictMode));
            }
        }

        #endregion members

        #region private methods

        private bool ReadFile(string path)
        {
            const string FUNCTION = "::ReadFile ";
            CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");
            throw new NotImplementedException();
        }

        private void GetDelimiters()
        {
            const string FUNCTION = "::GetDelimiters ";
            CallbackHelper.Debug(ref callback, CLASSNAME + FUNCTION + "Entry");

            Evaluater.ThrowAndCallbackIfStringNotEqual(this.message.Substring(0, 3),
                "MSH", "This Message is not valid. Message doesn't start with <MSH>",
                true, ref this.callback);

            this.sepField = Convert.ToChar(this.message.Substring(3, 1));
            this.sepComponent = Convert.ToChar(this.message.Substring(4, 1));
            this.repeat = Convert.ToChar(this.message.Substring(5, 1));
            this.escape = Convert.ToChar(this.message.Substring(6, 1));
            this.sepSubComponent = Convert.ToChar(this.message.Substring(7, 1));

            // hack to fool generalized parsing routines:
            // rebuild first two "fields" as if they were separated like ordinary fields,
            // escape sequences are used instead of the original contents
            string help = Convert.ToString(this.sepField);
            this.message = this.message.Substring(8);

            if (!'\0'.Equals(this.escape))
            {

                Evaluater.ThrowAndCallbackIfFalse(IsValidCharacterSet(), string.Empty, true, ref this.callback);

                if (!'\0'.Equals(this.sepSubComponent))
                {
                    this.message = "MSH" + help + "\\F\\" + help + "\\S\\\\R\\\\E\\\\T\\" + this.message;
                }
                else
                {
                    this.message = "MSH" + help + "\\F\\" + help + "\\S\\\\R\\\\E\\" + this.message;
                }
            }
            else
            {
                this.message = "MSH" + help + "" + help + "" + this.message;
            }
        }

        private bool IsValidCharacterSet()
        {
            const string FUNCTION = "::IsValidCharacterSet ";

            // strict mode off?
            if (!this.strictMode) return true;

            int pos = 0;
            int pos1 = 0;
            int field = 0;
            int len = this.message.Length;
            string msg = string.Empty;
            string characterSet = string.Empty;

            while (pos < len)
            {
                char c = Convert.ToChar(this.message.Substring(pos, 1));
                if (this.sepField.Equals(c))
                {
                    field++;
                }
                else if (this.termSegment.Equals(c)
                    || '\n'.Equals(c))
                {
                    // MSH-18 not Found but Segment terminator  
                    CallbackHelper.Warn(ref callback, CLASSNAME + FUNCTION +
                        "strict-mode=on, MSH-18 not found, Parser uses ISO8859/1");
                    return true;
                }
                pos++;

                // we want MSH.18
                // Message starts on MSH-2
                if (17 > field)
                {
                    continue;
                }

                pos1 = pos;
                while (pos < len)
                {
                    if (this.sepField.Equals(Convert.ToChar(this.message.Substring(pos, 1)))
                        || this.termSegment.Equals(Convert.ToChar(this.message.Substring(pos, 1))))
                    {
                        characterSet = this.message.Substring(pos1, pos - pos1);
                        if (!"ASCII".Equals(characterSet)
                            && !"8859/1".Equals(characterSet))
                        {
                            CallbackHelper.Warn(ref callback, CLASSNAME + FUNCTION +
                                "strict-mode=on, this Character set is not supported " + characterSet);
                            return false;
                        }
                        return true;
                    }
                    pos++;
                }
            }

            return true;
        }

        private string DeEscape(string deEscape)
        {
            Evaluater.ThrowAndCallbackIfFalse(this.initialized,
                "HL7Scanner not initialized.", true, ref this.callback);


            //End current output line and skip <number> vertical spaces. <number> is a positive
            //integer or absent. If <number> is absent, skip one space. The horizontal character
            //position remains unchanged. Note that for purposes of compatibility with previous
            //versions of HL7, “^\.sp\” is equivalent to “\.br\.”            
            deEscape = deEscape.Replace("\\.sp\\", "\\.br\\");
            //Begin new output line. Set the horizontal position to the current left margin and
            //increment the vertical position by 1
            //           deEscape = deEscape.Replace("\\.br\\","\r");
            //Begin word wrap or fill mode. This is the default state. It can be changed to a nowrap
            //mode using the .nf command.
            deEscape = deEscape.Replace("\\.fi\\", string.Empty);
            //Begin no-wrap mode
            deEscape = deEscape.Replace("\\.nf\\", string.Empty);
            //End current output line and center the next line.            
            deEscape = deEscape.Replace("\\.ce\\", string.Empty);

            string a = deEscape;
            string b = string.Empty;
            string e = string.Empty;

            int pos = 0;
            int max = a.Length;
            int open = a.IndexOf(this.escape);
            int close;
            int len;

            while (0 <= open)
            {
                close = a.IndexOf(this.escape, open + 1);

                Evaluater.ThrowIfTrue(0 > close, string.Empty, true);

                len = open - pos;
                b += a.Substring(pos, len);

                open++;
                len = close - open;

                e = a.Substring(open, len);

                if ("F".Equals(e))
                    b += Convert.ToString(this.sepField);
                else if ("S".Equals(e))
                    b += Convert.ToString(this.sepComponent);
                else if ("R".Equals(e))
                    b += Convert.ToString(this.repeat);
                else if ("E".Equals(e))
                    b += Convert.ToString(this.escape);
                else if ("T".Equals(e))
                    b += Convert.ToString(this.sepSubComponent);
                else if ("H".Equals(e)) // start highlighting ignored 
                    b += string.Empty;
                else if ("N".Equals(e)) // stop highlighting ignored 
                    b += string.Empty;
                else if ("X".Equals(e.Substring(0, 1))) // hex data
                {
                    e = "0x" + e.Substring(1);
                    b += e;
                }
                else if ("Z".Equals(e.Substring(0, 1))) // hex data
                    b += string.Empty;
                else if (".".Equals(e.Substring(0, 1))) // hex data
                {
                    if (".br".Equals(e))
                        b += "\r";
                    else if (".sp".Equals(e))
                        b += "\r";
                    else
                        b += string.Empty;
                }
                else
                    b = "\\" + e + "\\";

                pos = close + 1;

                if (pos >= max)
                {
                    break;
                }
                open = a.IndexOf(this.escape, pos);
            }
            if (pos <= max)
            {
                len = max - pos;
                b += a.Substring(pos, len);
            }

            return b;
        }

        #endregion private methos

        #region private members

        private char termSegment = '\x0D';
        private char sepField = '|';
        private char sepComponent = '^';
        private char sepSubComponent = '\0';
        private char repeat = '~';
        private char escape = '\0';
        private char cLF = '\x0A';

        private string message = string.Empty;
        private int position = 0;
        private int length = 0;
        private int previous = 0;
        private HL7TokenPair nextToken = null;
        private bool initialized = false;

        private Dictionary<string, string> hl7EventsAndMessages = null;

        private HL7ConverterMessageDelegate callback = null;

        #endregion private members

        #region ICallback Member

        public void SetCallback(HL7ConverterMessageDelegate callback)
        {
            this.callback = callback;
        }

        #endregion
    }
}
