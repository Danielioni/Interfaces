/**********************************************************************

janoman: HL7 to XML-Parser

HL7Dictionary.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Dictionary implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Janoman.Healthcare.HL7
{

    internal sealed class HL7Dictionary : IDictionary
    {
        #region private members

        private bool initialized = false;
        private Dictionary<string, string>hl7Dictionary = null;
        private Dictionary<string,string>hl7Attributes = null;
        private HL7ConverterMessageDelegate callback = null;

        #endregion private members

        #region IDictionary Member

        public string Lookup(string tag)
        {
            Evaluater.ThrowIfFalse(this.initialized, "HL7-Dictionary not initialized", true);

            Evaluater.ThrowIfFalse(this.hl7Dictionary.ContainsKey(tag), 
                string.Format("Not such a key {0} in this dictionary", tag), true);

            return this.hl7Dictionary[tag];
        }

        public KeyValuePair<string, string> LookupAndAlsoAttributes(string tag)
        {
            string attributes = string.Empty;
            if (this.hl7Attributes.ContainsKey(tag))
            {
                attributes = this.hl7Attributes[tag];
            }
            return new KeyValuePair<string, string>(Lookup(tag), attributes);

        }

        #endregion

        public void Init(string dictionary)
        {
            using (var reader = new StringReader(dictionary))
            {

                this.hl7Attributes = new Dictionary<string, string>();
                this.hl7Dictionary = new Dictionary<string, string>();

                while (true)
                {
                    int position = 0;
                    int dPosition = 0;
                    int length = 0;
                    string delimeter = " ";
                    string a = string.Empty, b = string.Empty, c = string.Empty;

                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    dPosition = line.IndexOf(delimeter, position);

                    Evaluater.ThrowIfTrue(-1 == dPosition, "Wrong index", true);

                    // Hier dPosition testen
                    length = dPosition - position;
                    a = line.Substring(position, length);
                    // Position aktualisieren
                    position += length + 1;

                    if (position < line.Length)
                    {
                        dPosition = line.IndexOf(delimeter, position);
                        if (-1 == dPosition)
                        {
                            dPosition = line.Length;
                        }
                        length = dPosition - position;
                        b = line.Substring(position, length);
                        position += length + 1;
                    }

                    this.hl7Dictionary.Add(a, b);

                    if (position < line.Length)
                    {
                        c = line.Substring(position);
                        this.hl7Attributes.Add(a, c);

                    }
                }
                this.initialized = true;
            }
        }


        #region ICallback Member

        public void SetCallback(HL7ConverterMessageDelegate callback)
        {
            this.callback = callback;
        }

        #endregion
    }
}
