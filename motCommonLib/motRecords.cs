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
using System.Data;
using System.Data.SqlTypes;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using motCommonLib;

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

namespace motCommonLib
{
    public class Field
    {
        public string tagName { get; set; }
        public string tagData { get; set; }
        public int maxLen { get; set; }
        public bool required { get; set; }
        public char when { get; set; }
        public bool autoTruncate { get; set; }
        public bool __new { get; set; }
        public virtual void __rules() { }

        public Field(string f, string t, int m, bool r, char w)
        {
            tagName = f;
            tagData = t;
            maxLen = m;
            required = r;
            when = w;
            autoTruncate = __new = false;
        }
        public Field(string f, string t, int m, bool r, char w, bool a, bool n)
        {
            tagName = f;
            tagData = t;
            maxLen = m;
            required = r;
            when = w;
            autoTruncate = true;
            __new = true;
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
    public class Query
    {
        public string __table { set; get; }
        public Dictionary<string, string> __field;

        public Query()
        {
            __field = new Dictionary<string, string>();
        }

        public void Clear()
        {
            __table = string.Empty;
            __field.Clear();
        }
    }

   
    /// <summary>
    /// Basic list processing and rules for record creation - base class for all records
    /// </summary>
    public class motRecordBase
    {
        protected string __dsn;

        protected string _tableAction;
        protected Logger logger = LogManager.GetLogger("motInboundLib.Record");
        protected motPort __default;

        public bool __log_records { get; set; } = false;
        public motErrorlLevel __log_level { get; set; } = motErrorlLevel.Error;  // 0 = off, 1 - Errors Only, 2 - Errors and Warnings,  3 - Errors, Warnings, and Info
        public bool __auto_truncate { get; set; } = false;

        public void checkDependencies(List<Field> __qualifiedTags)
        {
            Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("action")));

            //  There are rules for fields that are required in add/change/delete.  Test them here
            for (int i = 0; i < __qualifiedTags.Count; i++)
            {
                // required== true, when == 'a', _table action == 'change'  -> Pass
                // required== true, when == 'a', _table action == 'add', tagData == live data -> Pass
                // required== true, when == 'a', _table action == 'add', tagData == empty -> Exception

                if (__qualifiedTags[i].required && __qualifiedTags[i].when == f.tagData.ToLower()[0])
                {
                    if (__qualifiedTags[i].tagData != null)
                    {
                        if (__qualifiedTags[i].tagData.Length == 0)
                        {
                            string __err = string.Format("Field {0} empty but required for the {1} operation on a {2} record!", __qualifiedTags[i].tagName, f.tagData, __qualifiedTags[0].tagData);

                            Console.WriteLine(__err);
                            throw new Exception(__err);
                        }
                    }
                }
            }
        }
        public void Clear(List<Field> __qualifiedTags)
        {
            string __type = __qualifiedTags[0].tagData;
            string __action = __qualifiedTags[1].tagData;

            foreach (Field __field in __qualifiedTags)
            {
                __field.tagData = string.Empty;
            }

            __qualifiedTags[0].tagData = __type;
            __qualifiedTags[1].tagData = __action;

        }

        protected void __write_log(string __data,  motErrorlLevel __el)
        {
            if(!__log_records || motErrorlLevel.Off == __log_level)
            {
                return;
            }

            switch(__el)
            {
                case motErrorlLevel.Error:
                    if(__log_level >= motErrorlLevel.Error)
                    {
                        logger.Error(__data);
                    }
                    break;

                case motErrorlLevel.Warning:
                    if( __log_level >= motErrorlLevel.Warning)
                    {
                        logger.Warn(__data);
                    }
                    break;

                case motErrorlLevel.Info:
                    if (__log_level > motErrorlLevel.Warning)
                    {
                        logger.Info(__data);
                    }
                    break;

                default:
                    break;
            }
        }

        protected bool setField(List<Field> __qualifiedTags, string __val, string __tag)
        {
            if (__qualifiedTags == null || __tag == null)
            {
                return false;
            }

            Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((__tag.ToLower())));

            if (f == null)
            {
                __qualifiedTags.Add(new Field(__tag, __val, -1, false, 'n', false, true));
                return false;   // Field doesn't exist
            }

            if (__val != null && __val.ToString().Length > f.maxLen)
            {
                if (!__auto_truncate)
                {
                    string __log_data = string.Format("Field Overflow at: <{0}>, Data: {1}. Maxlen = {2} but got: {3}", __tag, __val, f.maxLen, __val.ToString().Length);
                    __write_log(__log_data, motErrorlLevel.Error);
                    throw new Exception("Field Overflow at: <" + __tag + ">. Maxlen = " + f.maxLen + " but got: " + __val.ToString().Length);
                }

                __val = __val?.Substring(0, f.maxLen - 1);
            }

            f.tagData = __val;

            return true;
        }
        protected bool setField(List<Field> __qualifiedTags, string __val, string __tag, bool __truncate)
        {
            if (__qualifiedTags == null || __tag == null)
            {
                return false;
            }

            Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((__tag.ToLower())));

            if (f == null)
            {
                __qualifiedTags.Add(new Field(__tag, __val, -1, false, 'n', false, true));
                return false;   // Field doesn't exist
            }

            if (__val != null && __val.ToString().Length > f.maxLen)
            {
                if (!__truncate)
                {
                    string __log_data = string.Format("Field Overflow at: <{0}>, Data: {1}. Maxlen = {2} but got: {3}", __tag, __val, f.maxLen,  __val.ToString().Length);

                    __write_log(__log_data, motErrorlLevel.Error);
                    throw new Exception(__log_data);
                }

                __val = __val?.Substring(0, f.maxLen - 1);
            }

            f.tagData = __val;
            return true;
        }
        protected void Write(motPort p, List<Field> __qualifiedTags, bool __do_logging)
        {
            string __record = "<Record>";
            bool __tmp = __log_records;
            __log_records = __do_logging;

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

                if (__log_records == true)
                {
                    __write_log(__record, motErrorlLevel.Info);
                }

                __log_records = __tmp;
            }
            catch(Exception e)
            {
                __write_log(e.Message, motErrorlLevel.Error);
                __log_records = __tmp;
                throw;
            }
        }
        public void Write(motPort p, List<Field> __qualifiedTags)
        {
            try
            {
                Write(p, __qualifiedTags, __log_records);
            }
            catch
            {
                throw;
            }
        }
        public void Write(motPort p, string __text)
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
        public void Write(List<Field> __tags)
        {
            try
            {
                motPort p = new motPort("127.0.0.1", "24042");
                Write(p, __tags);
            }
            catch
            {
                throw;
            }
        }
        public void Write(string __str)
        {
            try
            {
                motPort p = new motPort("127.0.0.1", "24042");
                Write(p, __str);
            }
            catch
            {
                throw;
            }
        }
        public object writeREST_JSON(string __uri, string __account, string __password, List<Field> __tags, Dictionary<string, string> __map)
        {
            // Build the JSON structure 
            //Dictionary<string, string>.Enumerator __cursor = __map.GetEnumerator();
            //__cursor.MoveNext();

            var __json_request = "{\n";

            foreach (Field __tag in __tags)
            {
                string __temp_tag = motUtils.__get_dict_value(__map, __tag.tagName);
                if (__temp_tag != null && !string.IsNullOrEmpty(__tag.tagData))
                {
                    __json_request += string.Format("{0} : {1}\n", __tag.tagName, __tag.tagData);
                }
            }

            __json_request += "};\n";


            var restClient = new RestClient(__uri)
            {
                Authenticator = new HttpBasicAuthenticator(__account, __password)
            };
            RestRequest request = new RestRequest();
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");



            //var myContentJson = JsonConvert.SerializeObject(obj, settings);

            request.AddParameter("application/json", __json_request, ParameterType.RequestBody);

            var response = restClient.Execute(request);

            //To make the call async
            //var cancellationTokenSource = new CancellationTokenSource();
            //var response2 = restClient.ExecuteTaskAsync<T>(request, cancellationTokenSource.Token);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var browserStackException = new ApplicationException(message, response.ErrorException);
                throw browserStackException;
            }

            return response.Content;

        }
        public void readDatabaseRecord(motDatabase __db, Query __query, List<Field> __qualifiedTags)
        {

            Dictionary<string, string>.Enumerator __cursor = __query.__field.GetEnumerator();

            var __base_query =
                string.Format("SELECT * FROM public.\"Facilities\", public.\"Addresses\", public.\"Patients\" WHERE \"Patients\".\"AddressId\" = \"Addresses\".\"Id\" AND \"Patients\".\"FacilityId\" = \"Facilities\".\"Id\"");


            while (__cursor.MoveNext())
            {
                __base_query += string.Format("AND \"{0}\" = '{1}' ", __cursor.Current.Key, __cursor.Current.Value);
            }

            __base_query += ";";

            if (__db.executeQuery(__base_query))
            {
                if (__db.__recordSet.Tables["__table"].Rows.Count > 0)
                {
                    DataRow __record = __db.__recordSet.Tables["__table"].Rows[0];

                    // Print the DataType of each column in the table. 
                    foreach (DataColumn column in __record.Table.Columns)
                    {
                        setField(__qualifiedTags, __record[column.ColumnName].ToString(), column.ColumnName, true);
                    }
                }
            }
        }
        public motRecordBase()
        {
            __dsn = string.Format("server={0};port={1};userid={2};password={3};database={4}",
                                  Properties.Settings.Default.DB_Address,
                                  Properties.Settings.Default.DB_Port,
                                  Properties.Settings.Default.DB_UserName,
                                  Properties.Settings.Default.DB_Password,
                                  Properties.Settings.Default.DB_DatabaseName);
        }
    }

    /// <summary>
    ///  Drug Record - Drug info with processing rules in a few places
    /// </summary>
    public class motDrugRecord : motRecordBase
    {
        public volatile List<Field> __qualifiedTags;

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
            catch
            {
                throw;
            }
        }
        ~motDrugRecord()
        {
        }
        public motDrugRecord() : base()
        {
        }
        public motDrugRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Drug record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }

        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }

        public string RxSys_DrugID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_drugid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_DrugID", false);
            }
        }
        public string LabelCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("lblcode")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "LblCode", false);
            }
        }
        public string ProductCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("prodcode")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "ProdCode", false);
            }
        }
        public string TradeName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("tradename")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "TradeName", false);
            }
        }
        public int Strength
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("strength")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                setField(__qualifiedTags, Convert.ToString(value), "Strength", false);
            }
        }
        public string Unit
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("unit")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Unit", false);
            }
        }
        public string RxOTC
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxotc")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxOTC", false);
            }
        }
        public string DoseForm
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("doseform")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DoseForm", false);
            }
        }
        public string Route
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("route")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Route", false);
            }
        }
        public int DrugSchedule
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("drugschedule")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                if (value < 2 && value > 7)
                {
                    throw new Exception("Drug Schedule must be 2-7");
                }

                setField(__qualifiedTags, Convert.ToString(value), "DrugSchedule", false);
            }
        }
        public string VisualDescription
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("visualdescription")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "VisualDescription", false);
            }
        }
        public string DrugName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("drugname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DrugName", false);
            }
        }
        public string ShortName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("shortname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "ShortName", false);
            }
        }
        public string NDCNum
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("ndcnum")));
                return f.tagData;
            }

            set
            {
                while (value.Contains("-"))
                {
                    value = value.Remove(value.IndexOf("-"), 1);
                }

                setField(__qualifiedTags, value, "NDCNum", false);
            }
        }
        public int SizeFactor
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("sizefactor")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                setField(__qualifiedTags, Convert.ToString(value), "SizeFactor", false);
            }
        }
        public string Template
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("template")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Template", false);
            }
        }
        public int DefaultIsolate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("defaultisolate")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                setField(__qualifiedTags, Convert.ToString(value), "DefaultIsolate", false);
            }
        }
        public string ConsultMsg
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("consultmsg")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "ConsultMsg", false);
            }
        }
        public string GenericFor
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("genericfor")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "GenericFor", false);
            }
        }

        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }

        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Drug record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }

        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Drug record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
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
                string __error = string.Format("Failed to create Prescriber record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }
        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }
        public string RxSys_DocID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_docid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_DocID");
            }
        }
        public string LastName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("lastname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "LastName");
            }
        }
        public string FirstName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("firstname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "FirstName");
            }
        }
        public string MiddleInitial
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("middleinitial")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "MiddleInitial");
            }
        }
        public string Address1
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address1")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address1");
            }
        }
        public string Address2
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address2")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address2");
            }
        }
        public string City
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("city")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "City");
            }
        }
        public string State
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("state")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value.ToUpper(), "State");
            }
        }
        public string PostalCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("postalcode")));
                return f.tagData;
            }

            set
            {
                while (value.Contains("-"))
                {
                    value = value.Remove(value.IndexOf("-"), 1);
                }

                setField(__qualifiedTags, value, "Zip");
            }
        }
        public string Phone
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("phone")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { '(', ')', '-', '.', ',', ' ' };

                while (value.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "Phone");
            }
        }
        public string Comments
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Comments");
            }
        }
        public string DEA_ID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("dea_id")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DEA_ID");
            }
        }
        public string TPID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("tpid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "TPID");
            }
        }
        public int Specialty
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("speciality")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                setField(__qualifiedTags, Convert.ToString(value), "Specialty");
            }
        }
        public string Fax
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("fax")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Fax");
            }
        }
        public string PagerInfo
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("pageringfo")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "PagerInfo");
            }
        }
        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Prescriber record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Prescriber record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
    }

    /// <summary>
    /// Patient Record - The folks getting the meds with field level processing rules where appropriate
    /// </summary>
    public class motPatientRecord : motRecordBase
    {
        public volatile List<Field> __qualifiedTags;

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
        public motPatientRecord() : base()
        {
        }
        public motPatientRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Patient record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }
        public void Clear()
        {
            base.Clear(__qualifiedTags);
        }

        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }

        public string RxSys_PatID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_patid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_PatID");
            }
        }
        public string RxSys_DocID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_docid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_DocID");
            }
        }
        public string LastName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("lastname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "LastName");
            }
        }
        public string FirstName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("firstname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "FirstName");
            }
        }
        public string MiddleInitial
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("middleinitial")));
                return f.tagData;
            }

            set
            {
                while (value.Contains("."))
                {
                    value = value.Remove(value.IndexOf("."), 1);
                }

                setField(__qualifiedTags, value, "MiddleInitial");
            }
        }
        public string Address1
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address1")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address1");
            }
        }
        public string Address2
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address2")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address2");
            }
        }
        public string City
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("city")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "City");
            }
        }
        public string State
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("state")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value.ToUpper(), "State");
            }
        }
        public string PostalCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("postalcode")));
                return f.tagData;
            }

            set
            {
                while (value.Contains("-"))
                {
                    value = value.Remove(value.IndexOf("-"), 1);
                }

                setField(__qualifiedTags, value, "Zip");
            }
        }
        public string Phone1
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("phone1")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { '(', ')', '-', '.', ',', ' ' };

                while (value.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "Phone1");
            }
        }
        public string Phone2
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("phone2")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { '(', ')', '-', '.', ',', ' ' };

                while (value.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "Phone2");
            }
        }
        public string WorkPhone
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("workphone")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { '(', ')', '-', '.', ',', ' ' };

                while (value.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "WorkPhone");
            }
        }
        public string RxSys_LocID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_licid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_LocID");
            }
        }
        public string Room
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("room")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Room");
            }
        }
        public string Comments
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((("comments"))));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Comments");
            }
        }
        public string CycleDate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((("cycledate"))));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "CycleDate");
            }
        }
        public int CycleDays
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((("cycledays"))));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                if (value > 35 || value < 0)
                {
                    throw new Exception("CycleDays must be (0-35)");
                }

                setField(__qualifiedTags, Convert.ToString(value), "CycleDays");
            }
        }
        public int CycleType
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((("cycletype"))));
                return Convert.ToInt32(f.tagData);
            }


            set
            {
                // Actual error - default to 0
                if (value != 0 && value != 1)
                {
                    //throw new Exception("CycleType must be '0 - Monthly' or '1 - Weekly'");
                    value = 0;
                }

                setField(__qualifiedTags, Convert.ToString(value), "CycleType");
            }
        }
        public int Status
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((("status"))));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                // Actual error - Dedfault to Hold
                if (value != 0 && value != 1)
                {
                    //throw new Exception("Status must be '0 - Hold' or '1 - for Active'");
                    value = 0;
                }

                setField(__qualifiedTags, Convert.ToString(value), "Status");
            }
        }
        public string RxSys_LastDoc
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_lastdoc")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_LastDoc");
            }
        }
        public string RxSys_PrimaryDoc
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_primarydoc")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_PrimaryDoc");
            }
        }
        public string RxSys_AltDoc
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_altdoc")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_AltDoc");
            }
        }
        public string SSN
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("ssn")));
                return f.tagData;
            }

            set
            {
                while (value.Contains("-"))
                {
                    value = value.Remove(value.IndexOf("-"), 1);
                }

                setField(__qualifiedTags, value, "SSN");
            }
        }
        public string Allergies
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("allergies")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Allergies");
            }
        }
        public string Diet
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("diet")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Diet");
            }
        }
        public string DxNotes
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("dxnotes")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DxNotes");
            }
        }
        public string TreatmentNotes
        {

            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("treatmentnotes")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "TreatmentNotes");
            }
        }
        public string DOB
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("dob")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DOB");
            }
        }
        public int Height
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("height")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                setField(__qualifiedTags, Convert.ToString(value), "Height");
            }
        }
        public int Weight
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("weight")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                setField(__qualifiedTags, Convert.ToString(value), "Weight");
            }
        }
        public string ResponisbleName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("responsiblename")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "ResponsibleName");
            }
        }
        public string InsName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("insname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "InsName");
            }
        }
        public string InsPNo
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("inspno")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "InsPNo");
            }
        }
        public string AltInsName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("altinsnum")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "AltInsName");
            }
        }
        public string AltInsPNo
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("altinspno")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "AltInsPNo");
            }
        }
        public string MedicareNum
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("medicarenum")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "MCareNum");
            }
        }
        public string MedicaidNum
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("medicaidnum")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "MCaidNum");
            }
        }
        public string AdmitDate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("admitdate")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "AdmitDate");
            }
        }
        public string ChartOnly
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("chartonly")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "ChartOnly");
            }
        }
        public string Gender
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("gender")));
                return f.tagData;
            }

            set
            {
                if (value.ToUpper() != "F" && value.ToUpper() != "M")
                {
                    throw new Exception("Gender  M or F'");
                }

                setField(__qualifiedTags, value, "Gender");
            }
        }
        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Patient record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Patient record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write()
        {
            base.Write(__qualifiedTags);
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
                __qualifiedTags.Add(new Field("RxSys_RxNum", "", 12, true, 'k'));
                __qualifiedTags.Add(new Field("RxSys_PatID", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DocID", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DrugID", "", 11, true, 'a'));
                __qualifiedTags.Add(new Field("Sig", "", 32767, true, 'a'));
                __qualifiedTags.Add(new Field("RxStartDate", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("RxStopDate", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("DiscontinueDate", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("DoseScheduleName", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("Comments", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("Refills", "", 4, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_NewRxNum", "", 10, false, 'w'));
                __qualifiedTags.Add(new Field("Isolate", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("RxType", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("MDOMStart", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("MDOMEnd", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("QtyPerDose", "", 6, true, 'w'));
                __qualifiedTags.Add(new Field("QtyDispensed", "", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Status", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("DoW", "", 7, true, 'w'));
                __qualifiedTags.Add(new Field("SpecialDoses", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("DoseTimesQtys", "", 32767, true, 'w'));
                __qualifiedTags.Add(new Field("ChartOnly", "", 2, true, 'w'));
                __qualifiedTags.Add(new Field("AnchorDate", "", 10, true, 'w'));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public motPrescriptionRecord() : base()
        {
        }
        public motPrescriptionRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Prescription record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }
        public string RxSys_RxNum
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_rxnum")));
                return f.tagData;
            }

            set
            {

                setField(__qualifiedTags, value, "RxSys_RxNum");
            }
        }
        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }
        public string RxSys_PatID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_patid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_PatID");
            }
        }
        public string RxSys_DocID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_docid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_DocID");
            }
        }
        public string RxSys_DrugID
        {
            get
            {

                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_drugid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_DrugID");
            }
        }
        public string Sig
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("sig")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Sig");
            }
        }
        public string RxStartDate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxstartdate")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxStartDate");
            }
        }
        public string RxStopDate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxstopdate")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxStopDate");
            }
        }
        public string DiscontinueDate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("discontinuedate")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DiscontinueDate");
            }
        }
        public string DoseScheduleName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("doseschedulename")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { ' ', ';', ':' };

                while (value.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "DoseScheduleName");
            }
        }
        public string Comments
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Comments");
            }
        }
        public string Refills
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("refills")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Refills");
            }
        }
        public string RxSys_NewRxNum
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_newrxnum")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_NewRxNum");
            }
        }
        public string Isolate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("isolate")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Isolate");
            }
        }
        public string RxType
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxtype")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxType");
            }
        }
        public string MDOMStart
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("mdomstart")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "MDOMStart");
            }
        }
        public string MDOMEnd
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("mdomend")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "MDOMEnd");
            }
        }
        public string QtyPerDose
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("qtyperdose")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "QtyPerDose");
            }
        }
        public string QtyDispensed
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("qtydispensed")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "QtyDispensed");
            }
        }
        public string Status
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("status")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Status");
            }
        }
        public string DoW
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("dow")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DoW");
            }
        }
        public string SpecialDoses
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("specialdoses")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "SpecialDoses");
            }
        }
        public string DoseTimesQtys
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("dosetimesqtys")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DoseTimesQtys");
            }
        }
        public string ChartOnly
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("chartonly")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "ChartOnly");
            }
        }
        public string AnchorDate
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("anchordate")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "AnchorDate");
            }
        }
        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Prescription record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Prescription record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write()
        {
            base.Write(__qualifiedTags);
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
        public motLocationRecord() : base()
        {
        }
        public motLocationRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Location record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }
        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }
        public string RxSys_LocID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_locid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_LocID");
            }
        }
        public string RxSys_StoreID
        {
            get
            {

                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_storeid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_StoreID");
            }
        }
        public string LocationName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("locationname")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "LocationName");
            }
        }
        public string Address1
        {
            get
            {

                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address1")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address1");
            }
        }
        public string Address2
        {

            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address2")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address2");
            }
        }
        public string City
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("city")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "City");
            }
        }
        public string State
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("state")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value.ToUpper(), "State");
            }
        }
        public string PostalCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("postalcode")));
                return f.tagData;
            }

            set
            {
                while (value.Contains("-"))
                {
                    value = value.Remove(value.IndexOf("-"), 1);
                }

                setField(__qualifiedTags, value, "Zip");
            }
        }
        public string Phone
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("phone")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { '(', ')', '-', '.', ',', ' ' };


                while (value.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "Phone");

            }
        }
        public string Comments
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Comments");
            }
        }
        public int CycleDays
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("cycledays")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                if (value > 35 || value < 0)
                {
                    throw new Exception("CycleDays must be (0-35)");
                }

                setField(__qualifiedTags, Convert.ToString(value), "CycleDays");
            }
        }
        public int CycleType
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("cycletype")));
                return Convert.ToInt32(f.tagData);
            }

            set
            {
                // Actual error - it would be wrong to convert it to a default value
                if (value != 0 && value != 1)
                {
                    throw new Exception("CycleType must be '0 - Monthly' or '1 - Weekly'");
                }

                setField(__qualifiedTags, Convert.ToString(value), "CycleType");
            }

        }
        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Location record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Location record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write()
        {
            base.Write(__qualifiedTags);
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
        public motStoreRecord() : base()
        {
        }
        public motStoreRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Store record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }
        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }
        public string RxSys_StoreID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_storeid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_StoreID");
            }
        }
        public string StoreName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("storename")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "StoreName");
            }
        }
        public string Address1
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address1")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address1");
            }
        }
        public string Address2
        {

            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("address2")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Address2");
            }
        }
        public string City
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("city")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "City");
            }
        }
        public string State
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("state")));
                return f.tagData;
            }
            set
            {
                setField(__qualifiedTags, value?.ToUpper(), "State");
            }
        }
        public string Zip
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("zip")));
                return f.tagData;
            }

            set
            {
                if (value != null && value.Contains("-"))
                {
                    value = value.Remove('-');  // Sometimes folks pass formatted Zip +4 codes
                }

                setField(__qualifiedTags, value, "Zip");
            }
        }
        public string Phone
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("phone")));
                return f.tagData;
            }

            set
            {
                char[] __junk = { '(', ')', '-', '.', ',', ' ' };

                while (value?.IndexOfAny(__junk) > -1)
                {
                    value = value.Remove(value.IndexOfAny(__junk), 1);
                }

                setField(__qualifiedTags, value, "Phone");
            }
        }
        public string Fax
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("fax")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "Fax");
            }
        }
        public string DEANum
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("deanum")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DEANum");
            }
        }
        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Store record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to creawritete Store record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write()
        {
            base.Write(__qualifiedTags);
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

        public motTimeQtysRecord() : base()
        {
        }

        public motTimeQtysRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create TimeQtys record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }

        public void setField(string __fieldname, string __val)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname);
            }
            catch
            {
                throw;
            }
        }
        public void setField(string __fieldname, string __val, bool __override)
        {
            try
            {
                base.setField(__qualifiedTags, __val, __fieldname, __override);
            }
            catch
            {
                throw;
            }
        }
        public void readDatabaseRecord(motDatabase __db, Query __query)
        {
            if (__db == null || __query == null)
            {
                throw new ArgumentNullException("Null value passed as argument");
            }
            try
            {
                readDatabaseRecord(__db, __query, __qualifiedTags);
            }
            catch
            {
                throw;
            }
        }
        public string RxSys_LocID
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_locid")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_LocID");
            }
        }
        public string DoseScheduleName
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("doseschedulename")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DoseScheduleName");
            }
        }
        public string DoseTimeQtys
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("dosetimequtys")));
                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "DoseTimeQtys");
            }
        }
        public void Write(motPort p, bool __log_on)
        {
            try
            {
                Write(p, __qualifiedTags, __log_on);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write TimeQtys record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write(motPort p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write TimeQtys record: {0}", e.Message);
                __write_log(__error, motErrorlLevel.Error);
                Console.Write(__error);

                throw;
            }
        }
        public void Write()
        {
            base.Write(__qualifiedTags);
        }
    }
}
