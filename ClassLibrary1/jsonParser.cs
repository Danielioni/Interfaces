using System.Xml;
using Newtonsoft.Json;

namespace motInboundLib
{
    class jsonParser : Parser
    {


        public jsonParser(string inputData)
        {

            taggedParser helper = new taggedParser();
            string __work = inputData;

            // Look for JSON
            try
            {
                XmlDocument __xmldoc = JsonConvert.DeserializeXmlNode(__work, "Record");
                if (__xmldoc != null)
                {
                    try
                    {
                        helper.Write(__xmldoc.InnerXml);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch (JsonReaderException e)
            {
                throw new System.Exception("[MOT Parser] JSON Reader error " + e.Message);
            }
            catch (JsonSerializationException e)
            {
                throw new System.Exception("[MOT Parser] JSON Serialization error " + e.Message);
            }
        }
    }
}