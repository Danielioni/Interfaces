using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    public class Parser : Port
    {
        InputStucture   __type { get; set; }
        xmlParser       x;
        jsonParser      j;
        taggedParser    t;
        delimitedParser d;

        public void Write(string inboundData)
        {
            if (!Write(inboundData, inboundData.Length))
            {
                // Need to do better than this, need to retrieve the error code at least        
                throw new System.Exception("[MOT Parser] Failed to write to gateway");
            }
        }

        public Parser()
        {
            ;
        }

        public Parser(string inputStream)
        {
            try
            {
                //
                // Figure out what the input type is and set up the right parser
                //
                if (inputStream.Contains("<?") && inputStream.ToLower().Contains("xml"))
                {
                    // Pretty sure its a live XML file
                    x = new xmlParser(inputStream);
                    return;
                }

                if (inputStream.Contains("{") && inputStream.Contains(":"))
                {
                    // Pretty sure its a live JSON file
                    j = new jsonParser(inputStream);
                    return;
                }

                if (inputStream.ToLower().Contains("<record>") && inputStream.ToLower().Contains("<table>"))
                {
                    // Pretty sure its a MOT tagged file
                    t = new taggedParser(inputStream);
                    return;
                }

                throw new Exception("[MOT Parser] Unidentified file type");
            }
            catch(Exception e)
            {
                Console.WriteLine("[MOT Gateway] Parse failure: {0}", e.Message);
                throw;
            }
        }

      
        public Parser(string inputStream, InputStucture __type)
        {
            switch(__type)
            {
                case InputStucture.__inputXML:
                    x = new xmlParser(inputStream);
                    break;

                case InputStucture.__inputJSON:
                    j = new jsonParser(inputStream);
                    break;

                case InputStucture.__itDelimted:
                    d = new delimitedParser(inputStream);
                    break;

                case InputStucture.__inputTagged:
                    t = new taggedParser(inputStream);
                    break;

                case InputStucture.__inputUndefined:
                default:
                    break;

            }
        }
    }
}
