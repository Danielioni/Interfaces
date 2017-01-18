// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney and Medicine-On-Time, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using NLog;

namespace motCommonLib
{
    public class UIupdateArgs : EventArgs
    {
        public string __event_message { get; set; }
        public string __msh_in { get; set; }
        public string __msh_out { get; set; }
        public string timestamp { get; set; }
    }

    public delegate void UpdateUIEventHandler(object __sender, UIupdateArgs __args);
    public delegate void UpdateUIErrorHandler(object __sender, UIupdateArgs __args);

    public enum motErrorlLevel
    {
        Off = 0,
        Error,     // Errors Only
        Warning,   // Errors and Warnings
        Info       // Errors, Warning, and Info
    };

    public class HL7Exception : Exception
    {
        public HL7Exception()
        { }

        public HL7Exception(int code, string message) : base(message)
        { }

        public HL7Exception(int code, string message, Exception inner) : base(message, inner)
        { }
     }
    public class  KeyPair
    { 
        System.TypeCode __type;
        System.Type __system_type;

        public string Key { get; set; }
        public object Value
        {
            get
            {
                return (System.TypeCode)Value;   
            }

            set
            {
                __system_type = value.GetType();
                Value = value;
            }
        }

        public KeyPair(string k, object v)
        {
            Key = k;

            __system_type = v.GetType();
            Value = v;
        }

        public KeyPair(string k, Int32 v)
        {
            Key = k;
            Value = v;

            __type = TypeCode.Int32;
            __system_type = v.GetType();

        }

        public KeyPair(string k, string v)
        {
            Key = k;
            Value = v;

            __type = TypeCode.String;
            __system_type = v.GetType();
        }

        public KeyPair(string k, bool v)
        {
            Key = k;
            Value = v;

            __type = TypeCode.Boolean;
            __system_type = v.GetType();
        }

        public System.TypeCode GetValueType()
        {
            return __type;
        }

        public new string ToString()
        {
            switch (__type)
            {
                case TypeCode.String:
                    return string.Format("\"{0}\" : \"{1}\"", Key, Value);
                    
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return string.Format("\"{0}\" : {1}", Key, Value);

                case TypeCode.Boolean:
                    return string.Format("\"{0}\" : {1}", Key, Value.ToString());

                case TypeCode.Object:
                    return string.Format("[]");
                
                default:
                    return string.Empty;
            }
        }
    }
    static public class motUtils
    {
        static public void __write_log(Logger __log, motErrorlLevel __current, motErrorlLevel __this, string __message)
        {
            try
            {
                if (__this >= __current)
                {
                    switch (__this)
                    {
                        case motErrorlLevel.Error:
                            __log.Error(__message);
                            break;

                        case motErrorlLevel.Warning:
                            __log.Warn(__message);
                            break;

                        case motErrorlLevel.Info:
                            __log.Info(__message);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch
            { }
        }

        static public string __normalize_address(string __address)
        {
            IPAddress[] __ip_list = Dns.GetHostAddresses(__address);

            foreach (IPAddress __ip in __ip_list)
            {
                if (__ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    __address = __ip.ToString();
                }
            }

            return __address;
        }

        static public object __get_dict_value(Dictionary<object, object> __map, object __key)
        {
            object __val;

            if (__map.TryGetValue(__key, out __val))
            {
                return __val;
            }

            return null;
        }

        static public string __get_dict_value(Dictionary<string, string> __map, string __key)
        {
            string __val;

            if (__map.TryGetValue(__key, out __val))
            {
                return __val;
            }

            return null;
        }

        static public KeyPair __get_dict_value(Dictionary<string, KeyPair> __map, string __key)
        {
             KeyPair __val;

            if (__map.TryGetValue(__key, out __val))
            {
                return __val;
            }

            return null;
        }


        static public string __get_field_list_value(List<Field> __fields, string __tag)
        {
            foreach (Field __f in __fields)
            {
                if (__f.tagName == __tag)
                {
                    return __f.tagData;
                }
            }

            return null;
        }
    }
}
