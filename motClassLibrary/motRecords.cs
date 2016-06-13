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
using NLog;
/// <summary>
/// motRecords - Abstractions for all the record types that the Medicine-On-Time Legacy interface supports.  Classes are constructed 
///              and can be populated by calling individual methods, one for each field, or by a generic setField() call. All set methods
///              length check the arguments against the supported Legacy Interface values and throws an exception on overflow.  This can 
///              be overriden using setField with a bool third argument where "true" maeans Override Length Checking.  No additional data
///              processing is done by the generic setField functions where the direct methods may do content processing to remove illegal
///              characters etc.
///              
///             Example:
///             
///                     motTimeQtysRecord m = new motTimeQtysRecord();
///                     
///                    (1) m.setField("DoseScheduleName", "<<dsname>>");       // Set the Dose Schedule Name with Length Checking Enabled
///                    (2) m.setField("DoseScheduleName", "<<dsname>>", true); // Set the Dose Schedule Name with Length Checking Overridden
///                    (3) m.setDoseScheduleName("<<dsname>>");                // Set the Dose Schedule Name directly 
///                    
///                     m.Write(__port);                                       // Write the completed record to the MOT database monitoring __port
///                     
/// </summary>

namespace motInboundLib
{
    public class Field
    {
        public string tagName { get; set; }
        public string tagData { get; set; }
        public int maxLen { get; set; }
        public bool required { get; set; }
        public char when { get; set; }
        public bool autoTruncate { get; set; }
        public virtual void __rules() { }

        public Field(string f, string t, int m, bool r, char w)
        {
            tagName = f;
            tagData = t;
            maxLen = m;
            required = r;
            when = w;
            autoTruncate = false;
        }

        public Field(string f, string t, int m, bool r, char w, bool a)
        {
            tagName = f;
            tagData = t;
            maxLen = m;
            required = r;
            when = w;
            autoTruncate = a;
        }
    }

    /// <summary>
    /// Basic list processing and rules for record creation - base class for all records
    /// </summary>
    public class motRecordBase
    {
        protected string _tableAction;
        protected Logger logger = LogManager.GetLogger("motInboundLib.Record");

        public void checkDependencies(List<Field> __qualifiedTags)
        {
            //  There are rules for fields that are required in add/change/delete.  Test them here
            for (int i = 0; i < __qualifiedTags.Count; i++)
            {
                // required== true, when == 'a', _table action == 'change'  -> Pass
                // required== true, when == 'a', _table action == 'add', tagData == live data -> Pass
                // required== true, when == 'a', _table action == 'add', tagData == empty -> Exception

                if (__qualifiedTags[i].required && __qualifiedTags[i].when == this._tableAction[0])
                {
                    if (__qualifiedTags[i].tagData.Length == 0)
                    {
                        throw new Exception(__qualifiedTags[i].tagData + "empty but required for " + this._tableAction + " operation!");
                    }
                }
            }
        }
        public void setField(List<Field> __qualifiedTags, string __val, string __tag)
        {
            if (__qualifiedTags == null || __tag == null || __val == null)
            {
                return;
            }

            Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains(__tag.ToLower()));

            f.__rules();

            if (__val.ToString().Length > f.maxLen)
            {
                if (f.autoTruncate == false)
                {
                    throw new Exception("Field Overflow at: <" + __tag + ">. Maxlen = " + f.maxLen + " but got: " + __val.ToString().Length);
                }
                else
                {
                    // Pass through to gateway 
                }
            }

            f.tagData = __val;
        }
        public void setField(List<Field> __qualifiedTags, string __val, string __tag, bool __override)
        {
            if (__qualifiedTags == null || __tag == null || __val == null)
            {
                return;
            }

            Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains(__tag.ToLower()));

            f.__rules();

            if (__val.ToString().Length > f.maxLen)
            {
                if (__override == false)
                {
                    throw new Exception("Field Overflow at: <" + __tag + ">. Maxlen = " + f.maxLen + " but got: " + __val.ToString().Length);
                }
                else
                {
                    // Truncate and pass through to gateway 
                    __val = __val.Substring(0, f.maxLen - 1);
                }
            }

            f.tagData = __val;
        }
        public void Write(Port p, List<Field> __qualifiedTags)
        {
            string __record = "<Record>";

            try
            {
                checkDependencies(__qualifiedTags);

                for (int i = 0; i < __qualifiedTags.Count; i++)
                {
                    // Qualify field requirement
                    // if required and when == action && is_blank -> throw

                    __record += "<" + __qualifiedTags[i].tagName + ">" +
                                      __qualifiedTags[i].tagData + "</" +
                                      __qualifiedTags[i].tagName + ">";
                }

                __record += "</Record>";

                // Push it to the port
                p.Write(__record, __record.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Write(Port p, string __text)
        {
            try
            {
                p.Write(__text, __text.Length);
            }
            catch (Exception e)
            {
                Console.Write("Failed to write {0} to port.  Error {1}", __text, e.Message);
            }
        }
        public motRecordBase()
        {
        }
    }

    /// <summary>
    ///  Drug Record - Drug info with processing rules in a few places
    /// </summary>
    public class motDrugRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            _tableAction = tableAction.ToLower();

            try
            {
                __qualifiedTags.Add(new Field("Table", "Drug", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DrugID", "", 11, true, 'k'));
                __qualifiedTags.Add(new Field("LblCode", "", 6, false, 'n', true));
                __qualifiedTags.Add(new Field("ProdCode", "", 4, false, 'n'));
                __qualifiedTags.Add(new Field("TradeName", "", 100, false, 'n'));
                __qualifiedTags.Add(new Field("Strength", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("Unit", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("RxOTC", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("DoseForm", "", 11, false, 'n'));
                __qualifiedTags.Add(new Field("Route", "", 9, false, 'n'));
                __qualifiedTags.Add(new Field("DrugSchedule", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("VisualDescription", "", 12, false, 'n', true));
                __qualifiedTags.Add(new Field("DrugName", "", 40, true, 'a'));
                __qualifiedTags.Add(new Field("ShortName", "", 16, false, 'n'));
                __qualifiedTags.Add(new Field("NDCNum", "", 11, true, 'w'));
                __qualifiedTags.Add(new Field("SizeFactor", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Template", "", 1, false, 'n', true));
                __qualifiedTags.Add(new Field("DefaultIsolate", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("ConsultMsg", "", 45, false, 'n'));
                __qualifiedTags.Add(new Field("GenericFor", "", 40, false, 'n'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ~motDrugRecord()
        {
        }

        public motDrugRecord()
        {
        }
        public motDrugRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public string RxSys_DrugID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_drugid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_DrugID", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string LabelCode
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("lblcode"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "LblCode", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string ProductCode
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("prodcode"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "ProdCode", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string TradeName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("tradename"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "TradeName", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int Strength
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("strength"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, Convert.ToString(value), "Strength", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Unit
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("unit"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Unit", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxOTC
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxotc"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxOTC", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DoseForm
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("doseform"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DoseForm", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Route
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("route"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Route", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int DrugSchedule
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("drugschedule"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    if (value < 2 && value > 7)
                    {
                        throw new Exception("Drug Schedule must be 2-7");
                    }

                    setField(__qualifiedTags, Convert.ToString(value), "DrugSchedule", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string VisualDescription
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("visualdescription"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "VisualDescription", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DrugName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("drugname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DrugName", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string ShortName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("shortname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "ShortName", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string NDCNum
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("ndcnum"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }

                    setField(__qualifiedTags, value, "NDCNum", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int SizeFactor
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("sizefactor"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, Convert.ToString(value), "SizeFactor", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Template
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("template"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Template", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int DefaultIsolate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("defaultisolate"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, Convert.ToString(value), "DefaultIsolate", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string ConsultMsg
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("consultmsg"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "ConsultMsg", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string GenericFor
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("genericfor"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "GenericFor", false);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }

        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
                //Write(p, "<EOF/>");
            }
            catch (Exception e)
            {
                logger.Error(@"DrugRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }

    /// <summary>
    /// Prescriber Record - Practitioners who are licensed to write scrips with field level processing rules where appropriate
    /// </summary>

    public class motPrescriberRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            try
            {
                __qualifiedTags.Add(new Field("Table", "Prescriber", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DocID", "", 10, true, 'k'));
                __qualifiedTags.Add(new Field("LastName", "", 30, true, 'a'));
                __qualifiedTags.Add(new Field("FirstName", "", 20, true, 'a'));
                __qualifiedTags.Add(new Field("MiddleInitial", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Address1", "", 40, true, 'w'));
                __qualifiedTags.Add(new Field("Address2", "", 40, true, 'w'));
                __qualifiedTags.Add(new Field("City", "", 30, true, 'w'));
                __qualifiedTags.Add(new Field("State", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("Zip", "", 9, true, 'w'));
                __qualifiedTags.Add(new Field("Phone", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Comments", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("DEA_ID", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("TPID", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("Specialty", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Fax", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("PagerInfo", "", 40, false, 'n'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public motPrescriberRecord()
        {
        }
        public motPrescriberRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }

        public string RxSys_DocID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_docid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
                {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_DocID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string LastName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("lastname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "LastName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string FirstName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("firstname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "FirstNameID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string MiddleInitial
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("middleinitial"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "MiddleInitial");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address1
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address1"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address1");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address2
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address2"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address2");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string City
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("city"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "City");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string State
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("state"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value.ToUpper(), "State");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string PostalCode
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("postalcode"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }

                    setField(__qualifiedTags, value, "Zip");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Phone
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("phone"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Phone");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Comments
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("comments"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Comments");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DEA_ID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("dea_id"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DEA_ID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string TPID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("tpid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "TPID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int Specialty
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("speciality"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, Convert.ToString(value), "Specialty");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Fax
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("fax"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Fax");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string PagerInfo
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("pageringfo"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }

            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "PagerInfo");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                logger.Error(@"PrescriberRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }

    /// <summary>
    /// Patient Record - The folks getting the meds with field level processing rules where appropriate
    /// </summary>
    public class motPatientRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            try
            {
                __qualifiedTags.Add(new Field("Table", "Patient", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_PatID", "", 10, true, 'k'));
                __qualifiedTags.Add(new Field("LastName", "", 30, true, 'a'));
                __qualifiedTags.Add(new Field("FirstName", "", 20, true, 'a'));
                __qualifiedTags.Add(new Field("MiddleInitial", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Address1", "", 40, true, 'w'));
                __qualifiedTags.Add(new Field("Address2", "", 40, true, 'w'));
                __qualifiedTags.Add(new Field("City", "", 30, true, 'w'));
                __qualifiedTags.Add(new Field("State", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("Zip", "", 9, true, 'w'));
                __qualifiedTags.Add(new Field("Phone1", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Phone2", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("WorkPhone", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("RxSys_LocID", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Room", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Comments", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("CycleDate", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("CycleDays", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("CycleType", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Status", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("RxSys_LastDoc", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("RxSys_PrimaryDoc", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("RxSys_AltDoc", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("SSN", "", 9, true, 'w'));
                __qualifiedTags.Add(new Field("Allergies", "", 32767, true, 'w'));
                __qualifiedTags.Add(new Field("Diet", "", 32767, true, 'w'));
                __qualifiedTags.Add(new Field("DxNotes", "", 32767, true, 'w'));
                __qualifiedTags.Add(new Field("TreatmentNotes", "", 32767, true, 'w'));
                __qualifiedTags.Add(new Field("DOB", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Height", "", 4, false, 'n'));
                __qualifiedTags.Add(new Field("Weight", "", 4, false, 'n'));
                __qualifiedTags.Add(new Field("ResponsibleName", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("InsName", "", 80, false, 'n'));
                __qualifiedTags.Add(new Field("InsPNo", "", 20, false, 'n'));
                __qualifiedTags.Add(new Field("AltInsName", "", 80, false, 'n'));
                __qualifiedTags.Add(new Field("AltInsPNo", "", 20, false, 'n'));
                __qualifiedTags.Add(new Field("MCareNum", "", 20, false, 'n'));
                __qualifiedTags.Add(new Field("MCaidNum", "", 20, false, 'n'));
                __qualifiedTags.Add(new Field("AdmitDate", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("ChartOnly", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Gender", "", 2, false, 'n'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public motPatientRecord()
        {
        }
        public motPatientRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }


        public string RxSys_DocID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_docid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_DocID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string LastName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("lastname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "LastName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string FirstName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("firstname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "FirstNameID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string MiddleInitial
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("middleinitial"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    while (value.Contains("."))
                    {
                        value = value.Remove(value.IndexOf("."), 1);
                    }

                    setField(__qualifiedTags, value, "MiddleInitial");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address1
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address1"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address1");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address2
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address2"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address2");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string City
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("city"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "City");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string State
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("state"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value.ToUpper(), "State");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string PostalCode
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("postalcode"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }

                    setField(__qualifiedTags, value, "Zip");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Phone1
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("phone1"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Phone1");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Phone2
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("phone2"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Phone2");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string WorkPhone
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("workphone"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "WorkPhone");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_LocID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_licid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_LocID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Room
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("room"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Room");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Comments
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("comments"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Comments");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string CycleDate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("cycledate"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "CycleDate");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int CycleDays
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("cycledays"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    if (value > 35 || value < 0)
                    {
                        throw new Exception("CycleDays must be (0-35)");
                    }

                    setField(__qualifiedTags, Convert.ToString(value), "CycleDays");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int CycleType
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("cycletype"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }


            set
            {
                try
                {
                    // Actual error - it would be wrong to convert it to a default value
                    if (value != 0 && value != 1)
                    {
                        throw new Exception("CycleType must be '0 - Monthly' or '1 - Weekly'");
                    }

                    setField(__qualifiedTags, Convert.ToString(value), "CycleType");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int Status
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("status"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    // Actual error - it would be wrong to convert it to a default value
                    if (value != 0 && value != 1)
                    {
                        throw new Exception("Status must be '0 - Hold' or '1 - for Active'");
                    }

                    setField(__qualifiedTags, Convert.ToString(value), "Status");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_LastDoc
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_lastdoc"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_LastDoc");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_PrimaryDoc
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_primarydoc"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_PrimaryDoc");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_AltDoc
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_altdoc"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_AltDoc");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string SSN
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("ssn"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }

                    setField(__qualifiedTags, value, "SSN");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Allergies
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("allergies"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Allergies");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Diet
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("diet"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Diet");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DxNotes
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("dxnotes"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DxNotes");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string TreatmentNotes
        {

            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("treatmentnotes"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "TreatmentNotes");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DOB
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("dob"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DOB");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int Height
        {
get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("height"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, Convert.ToString(value), "Height");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int Weight
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("weight"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, Convert.ToString(value), "Weight");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string ResponisbleName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("responsiblename"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "ResponsibleName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string InsName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("insname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "InsName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string InsPNo
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("inspno"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "InsPNo");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string AltInsName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("altinsnum"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "AltInsName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string AltInsPNo
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("altinspno"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "AltInsPNo");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string MedicareNum
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("medicarenum"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "MCareNum");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string MedicaidNum
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("medicaidnum"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "MCaidNum");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string AdmitDate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("admitdate"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "AdmitDate");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string ChartOnly
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("chartonly"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "ChartOnly");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Gender
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("gender"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    if (value.ToUpper() != "F" && value.ToUpper() != "M")
                    {
                        throw new Exception("Gender  M or F'");
                    }

                    setField(__qualifiedTags, value, "Gender");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                logger.Error(@"PatientRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }

    /// <summary>
    /// Prescription Record - The actual Scrip
    /// </summary>
    public class motPrescriptionRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            try
            {
                __qualifiedTags.Add(new Field("Table", "Rx", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_RxNum", "", 10, true, 'k'));
                __qualifiedTags.Add(new Field("RxSys_PatID", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DocID", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DrugID", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Sig", "", 32767, true, 'a'));
                __qualifiedTags.Add(new Field("RxStartDate", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("RxStopDate", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("DiscontinueDate", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("DoseScheduleName", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("Comments", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("Refills", "", 4, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_NewRxNum", "", 10, false, 'w'));
                __qualifiedTags.Add(new Field("Isolate", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("RxType", "", 0, true, 'w'));
                __qualifiedTags.Add(new Field("MDOMStart", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("MDOMEnd", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("QtyPerDose", "", 6, true, 'w'));
                __qualifiedTags.Add(new Field("QtyDispensed", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Status", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("DoW", "", 7, true, 'w'));
                __qualifiedTags.Add(new Field("SpecialDoses", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("DoseTimeQtys", "", 32767, true, 'w'));
                __qualifiedTags.Add(new Field("ChartOnly", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("AnchorDate", "", 10, true, 'w'));

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public motPrescriptionRecord()
        {
        }
        public motPrescriptionRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public string RxSys_RxNum
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_rxnum"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_RxNum");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_PatID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_patid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_PatID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_DocID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_docid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_DocID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Sig
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("sig"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Sig");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxStartDate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxstartdate"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxStartDate");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxStopDate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxstopdate"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxStopDate");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DiscontinueDate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("discontinuedate"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DiscontinueDate");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DoseScheduleName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("doseschedulename"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DoseScheduleName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Comments
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("comments"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Comments");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Refils
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("refills"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Refills");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string MDOMStart
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("mdomstart"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "MDOMStart");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string MDOMStop
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("mdomstop"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "MDOMStop");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string QtyPerDoese
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("qtyperdose"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "QtyPerDose");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string QtyDispensed
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("qtydispensed"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "QtyDispensed");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Status
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("status"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Status");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DoW
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("dow"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DoW");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string SpecialDoses
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("specialdoses"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "SpecialDoses");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DoseTimesQtys
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("dosetimeqtys"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DoseTimesQtys");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string ChartOnly
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("chartonly"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "ChartOnly");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string AnchorDate
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("anchordate"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "AnchorDate");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                logger.Error(@"PrescriptionRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }

    /// <summary>
    /// Location/Facility Record - Not to be confused with the Store record
    /// </summary>
    public class motLocationRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            try
            {
                __qualifiedTags.Add(new Field("Table", "Location", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_LocID", "", 11, true, 'k'));
                __qualifiedTags.Add(new Field("RxSys_StoreID", "", 11, false, 'n'));
                __qualifiedTags.Add(new Field("LocationName", "", 60, true, 'a'));
                __qualifiedTags.Add(new Field("Address1", "", 40, true, 'w'));
                __qualifiedTags.Add(new Field("Address2", "", 40, true, 'w'));
                __qualifiedTags.Add(new Field("City", "", 30, true, 'w'));
                __qualifiedTags.Add(new Field("State", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Zip", "", 9, true, 'w'));
                __qualifiedTags.Add(new Field("Phone", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Comments", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("CycleDays", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("CycleType", "", 2, false, 'n'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public motLocationRecord()
        {
        }
        public motLocationRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public string RxSys_LocID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_locid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_LocID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string RxSys_StoreID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_storeid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_StoreID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string LocationName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("locationname"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "LocationName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address1
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address1"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address1");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address2
        {

            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address2"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address2");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string City
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("city"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "City");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string State
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("state"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }
            set
            {
                try
                {
                    setField(__qualifiedTags, value.ToUpper(), "State");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string PostalCode
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("postalcode"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }

                    setField(__qualifiedTags, value, "Zip");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Phone
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("phone"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Phone");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Comments
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("comments"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Comments");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int CycleDays
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("cycledays"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    if (value > 35 || value < 0)
                    {
                        throw new Exception("CycleDays must be (0-35)");
                    }

                    setField(__qualifiedTags, Convert.ToString(value), "CycleDays");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public int CycleType
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("cycletype"));
                    return Convert.ToInt32(f.tagData);
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    // Actual error - it would be wrong to convert it to a default value
                    if (value != 0 && value != 1)
                    {
                        throw new Exception("CycleType must be '0 - Monthly' or '1 - Weekly'");
                    }

                    setField(__qualifiedTags, Convert.ToString(value), "CycleType");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }

        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                logger.Error(@"LocationRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }

    /// <summary>
    /// Store Record
    /// </summary>
    public class motStoreRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            try
            {
                __qualifiedTags.Add(new Field("Table", "Store", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_StoreID", "", 10, true, 'k'));
                __qualifiedTags.Add(new Field("StoreName", "", 60, true, 'a'));
                __qualifiedTags.Add(new Field("Address1", "", 40, false, 'n'));
                __qualifiedTags.Add(new Field("Address2", "", 40, false, 'n'));
                __qualifiedTags.Add(new Field("City", "", 30, false, 'n'));
                __qualifiedTags.Add(new Field("State", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Zip", "", 9, true, 'w'));
                __qualifiedTags.Add(new Field("Phone", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Fax", "", 10, false, 'a'));
                __qualifiedTags.Add(new Field("DEANum", "", 10, true, 'a'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public motStoreRecord()
        {
        }
        public motStoreRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public string RxSys_StoreID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_storeid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_StoreID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string StoreName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("storename"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "StoreName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address1
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address1"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address1");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Address2
        {

            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("address2"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Address2");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string City
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("city"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "City");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string State
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("state"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }
            set
            {
                try
                {
                    setField(__qualifiedTags, value.ToUpper(), "State");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string PostalCode
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("postalcode"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    if (value.Contains("-"))
                    {
                        value = value.Remove('-');  // Sometimes folks pass formatted Zip +4 codes
                    }

                    setField(__qualifiedTags, value, "Zip");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Phone
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("phone"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Phone");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string Fax
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("fax"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "Fax");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DEANum
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("deanum"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DEANum");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                logger.Error(@"StoreRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }

    /// <summary>
    /// TimeQtys Record
    /// </summary>
    public class motTimeQtysRecord : motRecordBase
    {
        public List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            try
            {
                __qualifiedTags.Add(new Field("Table", "TimesQtys", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_LocID", "", 10, true, 'k'));
                __qualifiedTags.Add(new Field("DoseScheduleName", "", 10, true, 'k'));
                __qualifiedTags.Add(new Field("DoseTimeQtys", "", 192, true, 'a'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public motTimeQtysRecord()
        {
        }

        public motTimeQtysRecord(string Action)
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw e;
            }
        }

        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to insert field. " + e);
            }
        }
        public string RxSys_LocID
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("rxsys_locid"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "RxSys_LocID");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public string DoseScheduleName
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("doseschedulename"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DoseScheduleName");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /*
        public List<string> DoseTimeQtys
        {
            string val;

            try
            {
                foreach (string s in value)
                {
                    val += s;
                }

                setField(__qualifiedTags, val, "DoseTimeQtys");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
*/
        public string DoseTimeQtys
        {
            get
            {
                try
                {
                    Field f = __qualifiedTags.Find(x => x.tagName.ToLower().Contains("dosetimequtys"));
                    return f.tagData;
                }
                catch
                {
                    throw new Exception("Illegal Acess");
                }
            }

            set
            {
                try
                {
                    setField(__qualifiedTags, value, "DoseTimeQtys");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                logger.Error(@"TimeQtysRecord Write Failure: {0}", e.Message);
                throw e;
            }
        }
    }
}
