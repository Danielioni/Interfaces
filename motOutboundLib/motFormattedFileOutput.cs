using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace motOutboundLib
{
    public class motFormattedFileOutput
    {
        public motFormattedFileOutput()
        {
        }

        public byte[] WriteDelimitedFile(string __path, List<KeyValuePair<string, string>> __data)
        {
            try
            {
                if(string.IsNullOrEmpty(__path))
                {
                    throw new Exception("Invalid Filename");
                }

                if (__data == null || __data.Count < 1)
                {
                    throw new Exception("Invalid Data List");
                }

                if (__data[0].Value.Length != 2)
                {
                    throw new Exception("Invalid header -- must be a Table Type (P,D,L,A,R,S,T) followed by an Action (A,C,D)");
                }

                using (MemoryStream m = new MemoryStream(4096))
                {
                    foreach (KeyValuePair<string, string> __item in __data)
                    {
                        m.Write(Encoding.ASCII.GetBytes(__item.Value), 0, __item.Value.Length);
                        m.WriteByte(0xEE);
                    }

                    m.WriteByte(0xE2);             // Final Delimiter

                    byte[] b = new byte[m.Length];
                    b = m.ToArray();

                    File.WriteAllBytes(__path + ".drec", b);

                    return b;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delimited file write error {0}", ex.Message);
                throw;
            }
        }

        public string WriteTaggedFile(string __path, List<KeyValuePair<string, string>> __data, string __table_type, string __action)
        {
            StringBuilder __tagged_string = new StringBuilder();

            try
            {
                if (string.IsNullOrEmpty(__path))
                {
                    throw new Exception("Invalid Filename");
                }

                if (__data == null || __data.Count < 1)
                {
                    throw new Exception("Invalid Data List");
                }

                __tagged_string.Append(string.Format("<Record>\n"));
                __tagged_string.Append(string.Format("<Table>{0}</Table>", __table_type));
                __tagged_string.Append(string.Format("<Action>{0}</Action>", __action));

                foreach(KeyValuePair<string,string> __kvp in __data)
                {
                    __tagged_string.Append(string.Format("<{0}>{1}</{0}>", __kvp.Key, __kvp.Value));
                }

                __tagged_string.Append(string.Format("</Record>"));

                File.WriteAllText(__path + ".trec", __tagged_string.ToString());

                return __tagged_string.ToString();

            }
            catch(Exception ex)
            {
                Console.WriteLine("Tagged file write error {0}", ex.Message);
                throw;
            }
        }
    }
}
