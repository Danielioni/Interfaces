using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    class taggedParser : Parser
    {
        public taggedParser()
        {
        }
        public taggedParser(string inboundData)
        {
        }
        public new void Write(string inboundData)
        {
            try
            {
                Write(inboundData, inboundData.Length);
            }
            catch
            {
                throw;
            }
        }
    }
}
