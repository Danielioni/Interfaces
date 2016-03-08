using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    class delimitedParser : Parser
    {
        public delimitedParser(string inboundData)
        {
            // Turn it into a tagged structure and write it

            try
            {
                // The system uses 0xEE as a delimiter which many will find inconvenient, preferring .CSV instead
                Write(inboundData.Replace(',', '\xEE'));
            }
            catch(System.Exception e)
            {
                throw;
            }
        }
    }
}
