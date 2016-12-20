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
using System.Globalization;

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
    public class motWriteQueue
    {
        
        private List<KeyValuePair<string, string>> __records { get; set; } = null;
        public motWriteQueue()
        {
            __records = new List<KeyValuePair<string, string>>();
        }
        ~motWriteQueue()
        {
            if(__records != null)
            {
                __records.Clear();
            }
        }

        private int compare(KeyValuePair<string,string> a, KeyValuePair<string, string> b)
        {
            return string.Compare(a.Key, b.Key);
        }
        public void Add(string __type, string __record)
        {
            if(string.IsNullOrEmpty(__type) || string.IsNullOrEmpty(__record))
            {
                throw new ArgumentNullException("motQueue.Add NULL argument");
            }

            __records.Add(new KeyValuePair<string,string>(__type, __record));
            __records.Sort(compare); 
        }
        public void Write(motSocket __socket)
        {
            if(__socket == null)
            {
                throw new ArgumentNullException("motQueue Null Socket Argument");
            }

            try
            {
                // Push it to the port
                foreach (KeyValuePair<string,string> __record in __records)
                {
                    __socket.write(__record.Value);
                }

                __socket.write("<EOF/>");

                // Flush
                __records.Clear();
                
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to write queue: " + ex.StackTrace);
            }
        }
        public void Clear()
        {
            __records.Clear();
        }
    }

    /// <summary>
    /// Basic list processing and rules for record creation - base class for all records
    /// </summary>
    public class motRecordBase
    {
        protected string    __dsn;
        protected string    __tableAction;
        protected Logger    __logger = LogManager.GetLogger("motCommonLib.Record");
        protected motSocket __default = null;


        public bool __log_records { get; set; } = false;
        public bool __auto_truncate { get; set; } = false;
        public bool __strong_validation { get; set; } = true;

        // External Ordered Queue
        public bool __queue_writes { get; set; } = false;
        public motWriteQueue __write_queue { get; set; } = null;
        public void AddToQueue(string __type, List<Field> __qualifiedTags)
        {
            if (__write_queue== null)
            {
                __logger.Error("Null Queue");
                throw new ArgumentNullException("Invalid Queue");
            }

            if (__qualifiedTags == null)
            {
                __logger.Error("Null parameters to base.Write()");
                throw new ArgumentNullException("Bad Tag List");
            }

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
                __write_queue.Add(__type, __record);
                
            }
            catch (Exception ex)
            {
                __logger.Error("Add To Queue: {0)\n{1}", ex.Message, __record);
                throw;
            }
        }
        public void WriteQueue(motSocket p)
        {
            try
            {
                if (__write_queue == null)
                {
                    throw new ArgumentNullException("Invalid Queue");
                }

                __write_queue.Write(p);
                __write_queue.Clear();
            }
            catch
            {
                throw;
            }

        }
        public void Commit(motSocket p)
        {
            WriteQueue(p);
        }

        protected string __normalize_date(string __date)
        {
            if(string.IsNullOrEmpty(__date))
            {
                throw new ArgumentNullException("Invalid Date Argument");
            }
            string[] __date_patterns =  // Hope I got them all
                {
                "yyyyMMdd",
                "yyyyMMd",
                "yyyyMdd",
                "yyyyMd",

                "yyyyddMM",
                "yyyyddM",
                "yyyydMM",
                "yyyydM",

                "ddMMyyyy",
                "ddMyyyy",
                "dMMyyyy",
                "dMyyyy",

                "MMddyyyy",
                "MMdyyyy",
                "Mddyyyy",
                "Mdyyyy",

                "dd/MM/yyyy",
                "dd/M/yyyy",
                "d/MM/yyyy",
                "d/M/yyyy",
          
                "MM/dd/yyyy",
                "MM/d/yyyy",
                "M/dd/yyyy",               
                "M/d/yyyy",

                "yyyy-MM-dd",
                "yyyy-M-dd",
                "yyyy-MM-d",
                "yyyy-M-d",

                "yyyy-dd-MM",
                "yyyy-d-MM",
                "yyyy-dd-M",
                "yyyy-d-M"
            };

            DateTime __dt;
            if (DateTime.TryParseExact(__date, __date_patterns, CultureInfo.InvariantCulture, DateTimeStyles.None, out __dt))
            {
                return __dt.ToString("yyyy-MM-dd"); // return MOT Legacy Gateway Format
            }

            return __date;
        }
        protected string __normalize_string(string __val)
        {
            if(string.IsNullOrEmpty(__val))
            {
                return string.Empty;
            }

            char[] __junk = { '-', '.', ',', ' ', ';', ':', '(', ')' };

            while (__val?.IndexOfAny(__junk) > -1)
            {
                __val = __val.Remove(__val.IndexOfAny(__junk), 1);
            }

            return __val;
        }
        public string __validate_dea_number(string __id)
        {
            if(string.IsNullOrEmpty(__id))
            {
                return string.Empty;
            }

            // DEA Number Format is 2 letters, 6 numbers, & 1 check digit (CC-NNNNNNN) 
            // The first letter is a code identifying the type of registrant (see below)
            // The second letter is the first letter of the registrant's last name
            __id = __normalize_string(__id);

            if (__strong_validation == true)
            {
                if(__id.Length < 9)
                {
                    throw new FormatException("REJECTED: Invalid DEA Number, minimum length is 9. Received " + __id.Length + " in " + __id);
                }

                if (__id.Length > 9)
                {
                    throw new FormatException("REJECTED: Invalid DEA Number, maximum length is 9. Received " + __id.Length + " in " + __id);
                }

                if (__id[1] != '9' && !Char.IsLetter(__id[1]))
                {
                    throw new FormatException("REJECTED: Invalid DEA Number, the id " + __id.Substring(0, 2) + " in " + __id + " is incorrect");
                }

                for (int i = 2; i < 7; i++)
                {
                    if (!Char.IsNumber(__id[i]))
                    {
                        throw new FormatException("REJECTED: Invalid DEA Number, the trailing 6 characters must be digits, not " + __id.Substring(2) + " in " + __id);
                    }
                }
            }

            return __id;
        }
        public void checkDependencies(List<Field> __qualifiedTags)
        {
            //
            // Legacy MOT Gateway rules (Required Column)
            //
            //      'K' - Key Field, required for all actions
            //      'A' - Required for all Add actions
            //      'W' - Would like to have but not required
            //      'C' - Required for all Change actions
            //

            Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("action")));

            //  There are rules for fields that are required in add/change/delete.  Test them here
            for (int i = 0; i < __qualifiedTags.Count; i++)
            {
                // required == true, when == 'k', _table action == '*', tagData == empty -> Exception
                // required == true, when == 'a', _table action == 'change'  -> Pass
                // required == true, when == 'a', _table action == 'add', tagData == live data -> Pass
                // required == true, when == 'a', _table action == 'add', tagData == empty -> Exception
                // required == true, when == 'c', _table_action == 'change', tagData == empty -> Exception

                if (__qualifiedTags[i].required && (__qualifiedTags[i].when == f.tagData.ToLower()[0] || __qualifiedTags[i].when == 'k'))  // look for a,c,k
                {
                    if (string.IsNullOrEmpty(__qualifiedTags[i].tagData))
                    {
                        if (!__auto_truncate)
                        {
                            string __err = string.Format("REJECTED: Field {0} empty but required for the {1} operation on a {2} record!", __qualifiedTags[i].tagName, f.tagData, __qualifiedTags[0].tagData);

                            __logger.Error(__err);
                            throw new Exception(__err);
                        }
                        else
                        {
                            string __err = string.Format("Attention: Empty {0}", __qualifiedTags[i].tagName);
                            __logger.Error(__err);
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
        protected bool setField(List<Field> __qualifiedTags, string __val, string __tag)
        {
            string __log_data = string.Empty;

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
                    __log_data = string.Format("Field Overflow at: <{0}>, Data: {1} Maxlen = {2} but got: {3}", __tag, __val, f.maxLen, __val.ToString().Length);
                    __logger.Error(__log_data);
                    throw new Exception(__log_data);
                }

                __log_data = string.Format("Autotruncated Overflowed Field at: <{0}>, Data: {1} Maxlen = {2} but got: {3}", __tag, __val, f.maxLen, __val.ToString().Length);
                __logger.Warn(__log_data);

                __val = __val?.Substring(0, f.maxLen);
            }

            f.tagData = string.IsNullOrEmpty(__val)  ? string.Empty : __val;

            return true;
        }
        protected bool setField(List<Field> __qualifiedTags, string __val, string __tag, bool __truncate)
        {
            string __log_data = string.Empty;

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

            if (!string.IsNullOrEmpty(__val) && __val.ToString().Length > f.maxLen)
            {
                if (!__truncate)
                {
                    __log_data = string.Format("Field Overflow at: <{0}>, Data: {1}. Maxlen = {2} but got: {3}", __tag, __val, f.maxLen, __val.ToString().Length);
                    __logger.Error(__log_data);
                    throw new Exception(__log_data);
                }

                __log_data = string.Format("Autotruncated Oferflowed Field at: <{0}>, Data: {1}. Maxlen = {2} but got: {3}", __tag, __val, f.maxLen, __val.ToString().Length);
                __logger.Warn(__log_data);

                __val = __val?.Substring(0, f.maxLen);
            }

            f.tagData = string.IsNullOrEmpty(__val) ? string.Empty : __val;

            return true;
        }
        protected void Write(motSocket p, List<Field> __qualifiedTags, bool __do_logging)
        {
            string __record = "<Record>";

            if (p == null || __qualifiedTags == null)
            {
                __logger.Error("Null parameters to base.Write()");
                throw new ArgumentNullException("Null Arguments");
            }

            try
            {
                checkDependencies(__qualifiedTags);

                for (int i = 0; i < __qualifiedTags.Count; i++)
                {
                    __record += "<" + __qualifiedTags[i].tagName + ">" +
                                      __qualifiedTags[i].tagData + "</" +
                                      __qualifiedTags[i].tagName + ">";
                }

                __record += "</Record>";

                // Push it to the port
                p.write(__record);
                p.write("<EOF/>");

                __logger.Info(__record);
            }
            catch (Exception ex)
            {
                __logger.Error("{0)\n{1}", ex.Message, __record);
                throw;
            }
        }
        protected void Write(motSocket p, List<Field> __qualifiedTags)
        {
            try
            {
                Write(p, __qualifiedTags, true);
            }
            catch
            {
                throw;
            }
        }
        public void Write(motSocket p, string __text)
        {
            try
            {
                p.write(__text);
            }
            catch (Exception e)
            {
                Console.Write("Failed to write {0} to port.  Error {1}", __text, e.Message);
            }
        }
        private void Write(List<Field> __tags)
        {
            try
            {
                motSocket p = new motSocket("localhost", 24042);
                Write(p, __tags);
            }
            catch
            {
                throw;
            }
        }
        private void Write(string __str)
        {
            try
            {
                motSocket p = new motSocket("localhost", 24042);
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
    ///  Drug Record (Key == E)
    /// </summary>
    public class motDrugRecord : motRecordBase
    {
        public volatile List<Field> __qualifiedTags;

        private void createRecord(string tableAction)
        {
            __tableAction = tableAction.ToLower();

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
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }
        public motDrugRecord(string Action,  bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Drug record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("E", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Drug record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }
        public void Clear()
        {
            base.Clear(__qualifiedTags);
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
        public string Strength
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("strength")));
                return f.tagData;
            }
            
            set
            {
                setField(__qualifiedTags, value, "Strength", false);
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
            }

            set
            {
                if ((value < 2 && value > 7) && value != 99)
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
                if (string.IsNullOrEmpty(value))
                {
                    bool __tmp = __auto_truncate;
                    __auto_truncate = false;

                    setField(__qualifiedTags, "Pharmacist Attention - Missing Drug Name", "DrugName");

                    __auto_truncate = __tmp;

                    return;
                }

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
                setField(__qualifiedTags, __normalize_string(value), "NDCNum", false);
            }
        }
        public int SizeFactor
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("sizefactor")));
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
    }

    /// <summary>
    /// Prescriber Record (Key == C)
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
            catch
            {
                throw;
            }
        }
        public motPrescriberRecord() : base()
        {
        }
        public motPrescriberRecord(string Action) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Prescriber record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }

        public motPrescriberRecord(string Action, bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Prescriber record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("C", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Prescriber record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

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
                if (!string.IsNullOrEmpty(value))
                {
                    setField(__qualifiedTags, value?.ToUpper(), "State");
                }
            }
        }
        public string PostalCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("zip")));
                return f.tagData;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    while ((bool)value?.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
                }

                setField(__qualifiedTags, value, "Zip");
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
                if (!string.IsNullOrEmpty(value))
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
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
                setField(__qualifiedTags, __normalize_string(value), "Phone");
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
                setField(__qualifiedTags, __validate_dea_number(value), "DEA_ID");
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
        public string Email
        {
            set
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                f.tagData += "\nEmail: " + value;
            }
        }
        public string IM
        {
            set
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                f.tagData += "\nIM: " + value;
            }
        }
    }

    /// <summary>
    /// Patient Record (Key == D)
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
            catch
            {
                throw;
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
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }

        public motPatientRecord(string Action, bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Patient record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("D", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Patient record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

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
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("rxsys_primarydoc")));
                if (f == null)
                {
                    return string.Empty;
                }

                return f.tagData;
            }

            set
            {
                setField(__qualifiedTags, value, "RxSys_PrimaryDoc");
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
                setField(__qualifiedTags, __normalize_string(value), "MiddleInitial");
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
                if (!string.IsNullOrEmpty(value))
                {
                    setField(__qualifiedTags, value?.ToUpper(), "State");
                }
            }
        }
        public string PostalCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("zip")));
                return f.tagData;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
                }

                setField(__qualifiedTags, __normalize_string(value), "Zip");
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
                if (!string.IsNullOrEmpty(value))
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
                }

                setField(__qualifiedTags, __normalize_string(value), "Zip");
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
                setField(__qualifiedTags, __normalize_string(value), "Phone1");
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
                setField(__qualifiedTags, __normalize_string(value), "Phone2");
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
                setField(__qualifiedTags, __normalize_string(value), "WorkPhone");
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
                setField(__qualifiedTags, __normalize_date(value), "CycleDate");
            }
        }
        public int CycleDays
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains((("cycledays"))));
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                setField(__qualifiedTags, __normalize_string(value), "SSN");
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
                setField(__qualifiedTags, __normalize_date(value), "DOB");
            }
        }
        public int Height
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("height")));
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                setField(__qualifiedTags, __normalize_date(value), "AdmitDate");
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
                if (!string.IsNullOrEmpty(value))
                {
                    if (value?.ToUpper() != "F" && value?.ToUpper() != "M")
                    {
                        value = "U";
                    }

                    setField(__qualifiedTags, value, "Gender");
                }
            }
        }
        public string Email
        {
            set
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                f.tagData += string.Format("\nEmail: {0}\n", value);
            }
        }
        public string IM
        {
            set
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                f.tagData += string.Format("\nIM: {0}\n", value);
            }
        }
    }

    /// <summary>
    /// Prescription Record  (Key == G)
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
            catch
            {
                throw;
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
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }
        public motPrescriptionRecord(string Action, motErrorlLevel LogLevel) : base()
        {
            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Prescription record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }
        public motPrescriptionRecord(string Action, bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Prescription record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("G", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Prescription record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

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
                setField(__qualifiedTags, __normalize_date(value), "RxStartDate");
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
                setField(__qualifiedTags, __normalize_date(value), "RxStopDate");
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
                setField(__qualifiedTags, __normalize_date(value), "DiscontinueDate");
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
                setField(__qualifiedTags, __normalize_string(value), "DoseScheduleName");
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
                setField(__qualifiedTags, __normalize_date(value), "AnchorDate");
            }
        }
    }

    /// <summary>
    /// Location/Facility Record (Key == B)
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
                __qualifiedTags.Add(new Field("Address2", "", 40, false, 'w'));
                __qualifiedTags.Add(new Field("City", "", 30, true, 'w'));
                __qualifiedTags.Add(new Field("State", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Zip", "", 9, true, 'w'));
                __qualifiedTags.Add(new Field("Phone", "", 10, true, 'w'));
                __qualifiedTags.Add(new Field("Comments", "", 32767, false, 'n'));
                __qualifiedTags.Add(new Field("CycleDays", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("CycleType", "", 2, false, 'n'));
            }
            catch
            {
                throw;
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
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }
        public motLocationRecord(string Action, bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Location record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("B", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Location record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

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
                if (string.IsNullOrEmpty(value))
                {
                    bool __tmp = __auto_truncate;
                    __auto_truncate = false;

                    setField(__qualifiedTags, "Pharmacist Attention - Missing Location Name", "LocationName");

                    __auto_truncate = __tmp;
                    return;
                }

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
                if (!string.IsNullOrEmpty(value))
                {
                    setField(__qualifiedTags, value?.ToUpper(), "State");
                }
            }
        }
        public string PostalCode
        {
            get
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("zip")));
                return f.tagData;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
                }

                setField(__qualifiedTags, __normalize_string(value), "Zip");
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
                if (!string.IsNullOrEmpty(value))
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
                }

                setField(__qualifiedTags, __normalize_string(value), "Zip");
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
                setField(__qualifiedTags, __normalize_string(value), "Phone");

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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
                return !string.IsNullOrEmpty(f.tagData) ? Convert.ToInt32(f.tagData) : 0;
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
    }

    /// <summary>
    /// Store Record (Key == A)
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
            catch
            {
                throw;
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
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }

        public motStoreRecord(string Action,  bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create Store record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("A", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write Store record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

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
                if (!string.IsNullOrEmpty(value))
                {
                    setField(__qualifiedTags, value?.ToUpper(), "State");
                }
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
                if (!string.IsNullOrEmpty(value))
                {
                    while (value.Contains("-"))
                    {
                        value = value.Remove(value.IndexOf("-"), 1);
                    }
                }

                setField(__qualifiedTags, __normalize_string(value), "Zip");
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
                setField(__qualifiedTags, __normalize_string(value), "Phone");
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
                setField(__qualifiedTags, __validate_dea_number(value), "DEANum");
            }
        }
        

        public string WebSite
        {
            set
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                f.tagData += string.Format("\nWebsite: {0}\n", value);
            }
            
        }

        public string Email
        {
            set
            {
                Field f = __qualifiedTags?.Find(x => x.tagName.ToLower().Contains(("comments")));
                f.tagData += string.Format("\nEmail: {0}\n", value);
            }
        }
    }

    /// <summary>
    /// TimeQtys Record  (Key == F)
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
                __qualifiedTags.Add(new Field("DoseTimesQtys", "", 192, true, 'a'));
            }
            catch
            {
                throw;
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
                __logger.Error(__error);
                Console.Write(__error);

                throw;
            }
        }
        public motTimeQtysRecord(string Action, bool AutoTruncate) : base()
        {
            __auto_truncate = AutoTruncate;

            try
            {
                __qualifiedTags = new List<Field>();
                createRecord(Action);
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to create TimeQtys record: {0}", e.Message);
                __logger.Error(__error);
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
        public void AddToQueue(motWriteQueue __queue)
        {
            __write_queue = __queue;
            AddToQueue();
        }
        public void AddToQueue()
        {
            AddToQueue("F", __qualifiedTags);
        }
        public void Write(motSocket p, bool __log_on = false)
        {
            try
            {
                if (__queue_writes)
                {
                    AddToQueue();
                }
                else
                {
                    Write(p, __qualifiedTags, __log_on);
                }
            }
            catch (Exception e)
            {
                string __error = string.Format("Failed to write TQ record: {0}", e.Message);
                __logger.Error(__error);
                Console.Write(__error);

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
    }
}
