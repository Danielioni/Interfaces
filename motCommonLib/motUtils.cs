using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace motCommonLib
{
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
    }
}
