﻿// 
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
                    __val = __val.Substring(0, f.maxLen-1);
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
        public void setRxSys_DrugID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_DrugID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setLabelCode(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "LblCode");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setProductCode(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "ProdCode");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setTradeName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "TradeName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setStrength(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Strength");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setUnit(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Unit");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxOTC(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxOTC");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDoseForm(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DoseForm");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRoute(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Route");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDrugSchedule(string val)
        {
            try
            {
                if (Convert.ToInt32(val) < 2 && Convert.ToInt32(val) > 7)
                {
                    throw new Exception("Drug Schedule must be 2-7");
                }

                setField(__qualifiedTags, val, "DrugSchedule");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setVisualDescription(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "VisualDescription");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDrugName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DrugName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setShortName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "ShortName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setNDCNum(string val)
        {
            try
            {
                if (val.Contains("-"))
                {
                    val.Remove('-');
                }

                setField(__qualifiedTags, val, "NDCNum");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setSizeFactor(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "SizeFactor");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setTemplate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Template");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDefaultIsolate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DefaultIsolate");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setConsultMsg(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "ConsultMsg");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setGenericFor(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "GenericFor");
            }
            catch (Exception e)
            {
                throw e;
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
        public void setRxSys_DocID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_DocID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setLastName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "LastName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setFirstName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "FirstNameID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setMiddleInitial(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "MiddleInitial");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress1(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address1");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress2(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address2");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCity(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "City");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setState(string val)
        {
            try
            {
                setField(__qualifiedTags, val.ToUpper(), "State");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPostalCode(string val)
        {
            try
            {
                if (val.Contains("-"))
                {
                    val.Remove('-');  // Sometimes folks pass formatted Zip +4 codes
                }

                setField(__qualifiedTags, val, "Zip");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPhone(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Phone");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setComments(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Comments");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDEA_ID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DEA_ID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setTPID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "TPID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setSpecialty(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Specialty");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setFax(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Fax");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPagerInfo(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "PagerInfo");
            }
            catch (Exception e)
            {
                throw e;
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
        public void setRxSys_DocID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_DocID");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public void setLastName(string val)
        {

            try
            {
                setField(__qualifiedTags, val, "LastName");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public void setFirstName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "FirstNameID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setMiddleInitial(string val)
        {
            try
            {
                if (val.Contains("."))
                {
                    val.Remove('.');  // Middle Initial shouldn't have a '.'
                }

                setField(__qualifiedTags, val, "MiddleInitial");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress1(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address1");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress2(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address2");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCity(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "City");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setState(string val)
        {
            try
            {
                setField(__qualifiedTags, val.ToUpper(), "State");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPostalCode(string val)
        {
            try
            {
                if (val.Contains("-"))
                {
                    val.Remove('-');  // Sometimes folks pass formatted Zip +4 codes
                }

                setField(__qualifiedTags, val, "Zip");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPhone1(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Phone1");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPhone2(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Phone2");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setWorkPhone(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "WorkPhone");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxSys_LocID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_LocID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRoom(string val)
        {
            try
            {
                if (val.Contains("."))
                {
                    val.Remove('.');  // Middle Initial shouldn't have a '.'
                }

                setField(__qualifiedTags, val, "Room");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setComments(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Comments");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCycleDate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "CycleDate");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCycleDays(string val)
        {
            try
            {
                if (Convert.ToInt32(val) > 35 || Convert.ToInt32(val) < 0)
                {
                    throw new Exception("CycleDays must be (0-35)");
                }

                setField(__qualifiedTags, val, "CycleDays");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCycleType(string val)
        {
            try
            {
                // Actual error - it would be wrong to convert it to a default value
                if (Convert.ToInt32(val) != 0 && Convert.ToInt32(val) != 1)
                {
                    throw new Exception("CycleType must be '0 - Monthly' or '1 - Weekly'");
                }

                setField(__qualifiedTags, val, "CycleType");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setStatus(string val)
        {
            try
            {
                // Actual error - it would be wrong to convert it to a default value
                if (Convert.ToInt32(val) != 0 && Convert.ToInt32(val) != 1)
                {
                    throw new Exception("Status must be '0 - Hold' or '1 - for Active'");
                }

                setField(__qualifiedTags, val, "Status");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxSys_LastDoc(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_LastDoc");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxSys_PrimaryDoc(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_PrimaryDoc");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxSys_AltDoc(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_AltDoc");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setSSN(string val)
        {
            try
            {
                if (val.Contains("-"))
                {
                    val.Remove('-');  // SSN shouldn't have a '-'
                }

                setField(__qualifiedTags, val, "SSN");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAllergies(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Allergies");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDiet(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Diet");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDxNotes(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DxNotes");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setTreatmentNotes(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "TreatmentNotes");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDOB(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DOB");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setHeight(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Height");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setWeight(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Weight");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setResponisbleName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "ResponsibleName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setInsName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "InsName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setInsPNo(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "InsPNo");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAltInsName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "AltInsName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAltInsPNo(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "AltInsPNo");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setMedicareNum(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "MCareNum");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setMedicaidNum(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "MCaidNum");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAdmitDate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "AdmitDate");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setChartOnly(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "ChartOnly");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setGender(string val)
        {
            try
            {
                if (val.ToUpper() != "F" && val.ToUpper() != "M")
                {
                    throw new Exception("Gender  M or F'");
                }

                setField(__qualifiedTags, val, "Gender");
            }
            catch (Exception e)
            {
                throw e;
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
        public void setRxSys_RxNum(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_RxNum");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public void setRxSys_PatID(string val)
        {

            try
            {
                setField(__qualifiedTags, val, "RxSys_PatID");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public void setRxSys_DocID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_DrugID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setSig(string val)
        {
            try
            {
                if (val.Contains("."))
                {
                    val.Remove('.');  // Middle Initial shouldn't have a '.'
                }

                setField(__qualifiedTags, val, "Sig");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxStartDate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxStartDate");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxStopDate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxStopDate");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDiscontinueDate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DiscontinueDate");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDoseScheduleName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DoseScheduleName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setComments(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Comments");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRefils(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Refills");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setMDOMStart(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "MDOMStart");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setMDOMStop(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "MDOMStop");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setQtyPerDoese(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "QtyPerDose");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setQtyDispensed(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "QtyDispensed");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setStatus(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Status");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDoW(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DoW");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setSpecialDoses(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "SpecialDoses");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDoseTimesQtys(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DoseTimesQtys");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setChartOnly(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "ChartOnly");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAnchorDate(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "AnchorDate");
            }
            catch (Exception e)
            {
                throw e;
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
        public void setRxSys_LocID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_LocID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setRxSys_StoreID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_StoreID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setLocationName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "LocationName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress1(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address1");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress2(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address2");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCity(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "City");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setState(string val)
        {
            try
            {
                setField(__qualifiedTags, val.ToUpper(), "State");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPostalCode(string val)
        {
            try
            {
                if (val.Contains("-"))
                {
                    val.Remove('-');  // Sometimes folks pass formatted Zip +4 codes
                }

                setField(__qualifiedTags, val, "Zip");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPhone(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Phone");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setComments(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Comments");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCycleDays(string val)
        {
            try
            {
                if (Convert.ToInt32(val) > 35 || Convert.ToInt32(val) < 0)
                {
                    throw new Exception("CycleDays must be (0-35)");
                }

                setField(__qualifiedTags, val, "CycleDays");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCycleType(string val)
        {
            try
            {
                // Actual error - it would be wrong to convert it to a default value
                if (Convert.ToInt32(val) != 0 && Convert.ToInt32(val) != 1)
                {
                    throw new Exception("CycleType must be '0 - Monthly' or '1 - Weekly'");
                }

                setField(__qualifiedTags, val, "CycleType");
            }
            catch (Exception e)
            {
                throw e;
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
        public void setRxSys_StoreID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_StoreID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setStoreName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "StoreName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress1(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address1");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setAddress2(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Address2");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setCity(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "City");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setState(string val)
        {
            try
            {
                setField(__qualifiedTags, val.ToUpper(), "State");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPostalCode(string val)
        {
            try
            {
                if (val.Contains("-"))
                {
                    val.Remove('-');  // Sometimes folks pass formatted Zip +4 codes
                }

                setField(__qualifiedTags, val, "Zip");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setPhone(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Phone");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setFax(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "Fax");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDEANum(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DEANum");
            }
            catch (Exception e)
            {
                throw e;
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
        public void setRxSys_LocID(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "RxSys_LocID");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDoseScheduleName(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DoseScheduleName");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void setDoseTimeQtys(List<string> tqlist)
        {
            string val = "";

            try
            {
                foreach (string s in tqlist)
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
        public void setDoseTimeQtys(string val)
        {
            try
            {
                setField(__qualifiedTags, val, "DoseTimeQtys");
            }
            catch (Exception e)
            {
                throw e;
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
                throw e;
            }
        }
    }
}
