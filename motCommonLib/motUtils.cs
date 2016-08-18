using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace motCommonLib
{
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
