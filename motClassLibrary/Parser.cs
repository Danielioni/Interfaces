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
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using Newtonsoft.Json;

namespace motInboundLib
{
    public class Parser
    {
        InputStucture __type { get; set; }
        protected Port p;
        
        private void parseTagged(string inboundData)
        {
            try
            {
                Write(inboundData);
            }
            catch (Exception e)
            {
                throw;
            } 
        }

        private void parseDelimited(string inboundData)
        {
        }

        private void parseJSON(string inboundData)
        {       
            // Look for JSON
            try
            {
                XmlDocument __xmldoc = JsonConvert.DeserializeXmlNode(inboundData, "Record");
                if (__xmldoc != null)
                {
                    try
                    {
                        Write(__xmldoc.InnerXml);
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

        private void parseXML(string inputData)
        {
            XmlDocument __xmldoc = new XmlDocument();

            try
            {
                // Check if it's actual XML or not. If so, strip headers up to <Record>
                if (inputData.Contains("<?xml") == false)
                {
                    throw new ArgumentException("[MOT XML Parser] Malformed XML");
                }

                __xmldoc.LoadXml(inputData);

                //
                // Clear out all the comments
                //
                XmlNodeList list = __xmldoc.SelectNodes("//comment()");
                foreach (XmlNode node in list)
                {
                    node.ParentNode.RemoveChild(node);
                }

                //
                // Validate all required fields have content
                // 
                list = __xmldoc.SelectNodes("//*[@required]");
                foreach (XmlNode node in list)
                {
                    if (node.Attributes[0].Value.ToLower() == "true" && node.NodeType == XmlNodeType.Element)
                    {
                        if (node.InnerText.Length == 0)
                        {
                            throw new ArgumentException("[MOT XML Parser] Missing Required Element Content at {0}", node.Name);
                        }

                        node.Attributes.RemoveNamedItem("required");
                    }
                }

                //
                // Validate field lengths and remove attributes
                //
                list = __xmldoc.SelectNodes("//*[@size]");
                foreach (XmlNode node in list)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if (node.InnerText.Length > Convert.ToUInt32(node.Attributes[0].Value))
                        {
                            throw new ArgumentException("[MOT XML Parser] Element Size Overflow at {0}", node.Name);
                        }

                        node.Attributes.RemoveNamedItem("size");
                    }
                }

                //
                // Validate for numeric overflow
                //
                list = __xmldoc.SelectNodes("//*[@maxvalue]");
                foreach (XmlNode node in list)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if (Convert.ToDouble(node.InnerText) > Convert.ToDouble(node.Attributes[0].Value))
                        {
                            throw new ArgumentException("[MOT XML Parser] Element MaxValue Overflow at {0}", node.Name);
                        }

                        node.Attributes.RemoveNamedItem("maxvalue");
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                throw new System.Exception("[MOT XML Parser] Parse Failure " + e.Message);
            }
            catch (System.FormatException e)
            {
                throw new System.Exception("[MOT XML Parser] Parse Error " + e.Message);
            }

            //
            // Finally, clear out the namespace attributes.
            //
            string xmlnsPattern = "\\s+xmlns\\s*(:\\w)?\\s*=\\s*\\\"(?<url>[^\\\"]*)\\\"";
            MatchCollection matchCol = Regex.Matches(__xmldoc.InnerXml, xmlnsPattern);

            foreach (Match m in matchCol)
            {
                __xmldoc.InnerXml = __xmldoc.InnerXml.Replace(m.ToString(), "");
            }

            // Finally, get the <?xml line and be done with it.
            __xmldoc.InnerXml = __xmldoc.InnerXml.Substring(__xmldoc.InnerXml.IndexOf(">") + 1);

            try
            {
                Write(__xmldoc.InnerXml);
            }
            catch
            {
                throw;
            }
        }
   

        public void Write(string inboundData)
        {
            if (!p.Write(inboundData, inboundData.Length))
            {
                // Need to do better than this, need to retrieve the error code at least        
                throw new Exception("[MOT Parser] Failed to write to gateway");
            }
        }

        public Parser()
        {
            ;
        }

        public Parser(Port _p, string inputStream)
        {
            
            p = _p;

            try
            {
                //
                // Figure out what the input type is and set up the right parser
                //
                if (inputStream.Contains("<?") && inputStream.ToLower().Contains("xml"))
                {
                    // Pretty sure its a live XML file
                    parseXML(inputStream);     
                    return;
                }

                if (inputStream.Contains("{") && inputStream.Contains(":"))
                {
                    // Pretty sure its a live JSON file
                    parseJSON(inputStream);
                    return;
                }

                if (inputStream.ToLower().Contains("<record>") && inputStream.ToLower().Contains("<table>"))
                {
                    // Pretty sure its a MOT tagged file
                    parseTagged(inputStream);
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

      
        public Parser(Port _p, string inputStream, InputStucture __type)
        {
            p = _p;

            try
            {
                switch (__type)
                {
                    case InputStucture.__inputXML:
                        parseXML(inputStream);
                        break;

                    case InputStucture.__inputJSON:
                        parseJSON(inputStream);
                        break;

                    case InputStucture.__inputDelimted:
                        parseDelimited(inputStream);
                        break;

                    case InputStucture.__inputTagged:
                        parseTagged(inputStream);
                        break;

                    case InputStucture.__inputUndefined:
                    default:
                        break;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
