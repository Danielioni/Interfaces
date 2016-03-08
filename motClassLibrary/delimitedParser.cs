using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Supports the default delimited format for the gateway.  Delimited with \`xEE`
/// </summary>
namespace motInboundLib
{
    class delimitedParser : Parser
    {
        public delimitedParser(string inboundData)
        {
            // It would make sense to vaalidate the checksum here
            Write(inboundData, inboundData.Length);
        }
    }
}
