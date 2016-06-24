/**********************************************************************

janoman: HL7 to XML-Parser

HL7ConverterProperties.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Properties implementation.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{

    internal class HL7ConverterProperties : IProperties
    {

        #region private members
        private Dictionary<string,string> stringProperties = null;
        private Dictionary<string,int> intProperties = null;
        private Dictionary<string,long> longProperties = null;
        private Dictionary<string,bool> boolProperties = null;
        #endregion private members

        #region ctors
        public HL7ConverterProperties()
        {
            this.stringProperties = new Dictionary<string, string>();
            this.intProperties = new Dictionary<string, int>();
            this.longProperties = new Dictionary<string, long>();
            this.boolProperties = new Dictionary<string, bool>();
        }
        #endregion ctors


        #region IProperties Member

        public string GetStringProperty(string key)
        {
            if (FindStringProperty(key))
            {
                return this.stringProperties[key.ToUpper()];
            }
            throw new HL7PropertyException(string.Format("Property {0} not configured", key));
        }

        public void SetStringProperty(string key, string value)
        {
            if (FindStringProperty(key))
            {
                this.stringProperties.Remove(key.ToUpper());
            }
            this.stringProperties.Add(key.ToUpper(), value);
        }

        public bool GetBoolProperty(string key)
        {
            if (FindBoolProperty(key))
            {
                return this.boolProperties[key.ToUpper()];
            }
            throw new HL7PropertyException(string.Format("Property {0} not configured", key));
        }

        public void SetBoolProperty(string key, bool value)
        {
            if (FindBoolProperty(key))
            {
                this.boolProperties.Remove(key.ToUpper());
            }
            this.boolProperties.Add(key.ToUpper(), value);
        }

        public int GetIntProperty(string key)
        {
            if (FindIntProperty(key))
            {
                return this.intProperties[key.ToUpper()];
            }
            throw new HL7PropertyException(string.Format("Property {0} not configured", key));
        }

        public void SetIntProperty(string key, int value)
        {
            if (FindIntProperty(key))
            {
                this.intProperties.Remove(key.ToUpper());
            }
            this.intProperties.Add(key.ToUpper(), value);
        }

        public long GetLongProperty(string key)
        {
            if (FindLongProperty(key))
            {
                return this.longProperties[key.ToUpper()];
            }
            throw new HL7PropertyException(string.Format("Property {0} not configured", key));
        }

        public void SetLongProperty(string key, long value)
        {
            if (FindLongProperty(key))
            {
                this.longProperties.Remove(key.ToUpper());
            }
            this.longProperties.Add(key.ToUpper(), value);
        }

        public bool FindStringProperty(string key)
        {
            if(this.stringProperties.ContainsKey(key.ToUpper()))
            {
                return true;
            }
            return false;
        }

        public bool FindBoolProperty(string key)
        {
            if (this.boolProperties.ContainsKey(key.ToUpper()))
            {
                return true;
            }
            return false;
        }

        public bool FindIntProperty(string key)
        {
            if (this.intProperties.ContainsKey(key.ToUpper()))
            {
                return true;
            }
            return false;
        }

        public bool FindLongProperty(string key)
        {
            if (this.longProperties.ContainsKey(key.ToUpper()))
            {
                return true;
            }
            return false;
        }

        #endregion

    }
}
