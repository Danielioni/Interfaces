/**********************************************************************

janoman: HL7 to XML-Parser

IProperties.cs

Copyright (c) 2010 Jan Schuster

This program is free software and comes with no warranty; for more
information, see the file lgpl.txt or visit
http://hl72xml.codeplex.com/

@brief Properties interface.

**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Janoman.Healthcare.HL7
{
    /// <summary>
    /// Interface for properties
    /// </summary>
    public interface IProperties
    {
        /// <summary>
        /// Find string property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>if property exist <code>true</code> else <code>false</code></returns>
        bool FindStringProperty(string key);
        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>value of property</returns>
        string GetStringProperty(string key);
        /// <summary>
        /// configure converter with this property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <param name="value">value of property</param>
        void SetStringProperty(string key, string value);
        /// <summary>
        /// Find boolean property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>if property exist <code>true</code> else <code>false</code></returns>
        bool FindBoolProperty(string key);
        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>value of property</returns>
        bool GetBoolProperty(string key);
        /// <summary>
        /// configure converter with this property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <param name="value">value of property</param>
        void SetBoolProperty(string key, bool value);
        /// <summary>
        /// Find integer property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>if property exist <code>true</code> else <code>false</code></returns>
        bool FindIntProperty(string key);
        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>value of property</returns>
        int GetIntProperty(string key);
        /// <summary>
        /// configure converter with this property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <param name="value">value of property</param>
        void SetIntProperty(string key, int value);
        /// <summary>
        /// Find long property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>if property exist <code>true</code> else <code>false</code></returns>
        bool FindLongProperty(string key);
        /// <summary>
        /// Get property value
        /// </summary>
        /// <param name="key">name of property</param>
        /// <returns>value of property</returns>
        long GetLongProperty(string key);
        /// <summary>
        /// configure converter with this property
        /// </summary>
        /// <param name="key">name of property</param>
        /// <param name="value">value of property</param>
        void SetLongProperty(string key, long value);
    }
}
