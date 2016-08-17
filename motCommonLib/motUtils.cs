using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace motCommonLib
{
    public class KeyPair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyPair(string k, string v)
        {
            Key = k;
            Value = v;
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
