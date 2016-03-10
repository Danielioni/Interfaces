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

        public Field(string f, string t, int m, bool r, char w)
        {
            tagName = f;
            tagData = t;
            maxLen = m;
            required = r;
            when = w;
        }
    }

    /// <summary>
    /// Basic list processing and rules for record creation - base class for all records
    /// </summary>
    public class motRecordBase
    {
        public void setField(List<Field> __qualifiedTags, string __val, string __tag)
        {
            Field f = __qualifiedTags.Find(x => x.tagName.Contains(__tag));
            if (__val.ToString().Length > f.maxLen)
            {
                throw new Exception(__tag + "Field Overflow");
            }

            f.tagData = __val;
        }

        public void Write(Port p, List<Field> __qualifiedTags)
        {
            string __record = "<Record>";

            try
            {
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
                throw;
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
            try
            {
                __qualifiedTags.Add(new Field("Table", "Drug", 10, true, 'a'));
                __qualifiedTags.Add(new Field("Action", tableAction, 10, true, 'a'));
                __qualifiedTags.Add(new Field("RxSys_DrugID", "", 11, true, 'k'));
                __qualifiedTags.Add(new Field("LblCode", "", 6, false, 'n'));
                __qualifiedTags.Add(new Field("ProdCode", "", 4, false, 'n'));
                __qualifiedTags.Add(new Field("TradeName", "", 100, false, 'n'));
                __qualifiedTags.Add(new Field("Strength", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("Unit", "", 10, false, 'n'));
                __qualifiedTags.Add(new Field("RxOTC", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("DoseForm", "", 11, false, 'n'));
                __qualifiedTags.Add(new Field("Route", "", 9, false, 'n'));
                __qualifiedTags.Add(new Field("DrugSchedule", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("VisualDescription", "", 12, false, 'n'));
                __qualifiedTags.Add(new Field("DrugName", "", 40, true, 'a'));
                __qualifiedTags.Add(new Field("ShortName", "", 16, false, 'n'));
                __qualifiedTags.Add(new Field("NDCNum", "", 11, true, 'w'));
                __qualifiedTags.Add(new Field("SizeFactor", "", 2, false, 'n'));
                __qualifiedTags.Add(new Field("Template", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("DefaultIsolate", "", 1, false, 'n'));
                __qualifiedTags.Add(new Field("ConsultMsg", "", 45, false, 'n'));
                __qualifiedTags.Add(new Field("GenericFor", "", 40, false, 'n'));
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setDrugSchedule(string val)
        {
            try
            {
                if(Convert.ToInt32(val) < 2 && Convert.ToInt32(val) > 7)
                {
                    throw new Exception("Drug Schedule must be 2-7");
                }

                setField(__qualifiedTags, val, "DrugSchedule");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setNDCNum(string val)
        {
            try
            {
                val.Remove('-');

                setField(__qualifiedTags, val, "NDCNum");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
    }

    /// <summary>
    /// Prescriber Record - Practitioners who are licensed to write scrips with field level processing rules where appropriate
    /// </summary>

    class motPrescriberRecord : motRecordBase
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setState(string val)
        {
            try
            {
                setField(__qualifiedTags, val.ToUpper(),, "State");
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public void setPostalCode(string val)
        {
            try
            {
                val.Remove('-');  // Sometimes folks pass formatted Zip +4 codes

                setField(__qualifiedTags, val, "Zip");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
    }

    /// <summary>
    /// Patient Record - The folks getting the meds with field level processing rules where appropriate
    /// </summary>
    class motPatientRecord : motRecordBase
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setMiddleInitial(string val)
        {
            try
            {
                val.Remove('.');  // Middle Initial shouldn't have a '.'

                setField(__qualifiedTags, val, "MiddleInitial");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setPostalCode(string val)
        {
            try
            {
                val.Remove('-');  // Sometimes folks pass formatted Zip +4 codes

                setField(__qualifiedTags, val, "Zip");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setRoom(string val)
        {
            try
            {
                val.Remove('.');  // Middle Initial shouldn't have a '.'

                setField(__qualifiedTags, val, "Room");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setCycleDays(string val)
        {
            try
            {
                if(Convert.ToInt32(val) > 35 || Convert.ToInt32(val) < 0)
                {
                    throw new Exception("CycleDays must be (0-35)");
                }

                setField(__qualifiedTags, val, "CycleDays");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
            }
        }
        public void setStatus(string val)
        {
            try
            {
                // Actual error - it would be wrong to convert it to a default value
                if(Convert.ToInt32(val) != 0 && Convert.ToInt32(val) != 1)
                {
                    throw new Exception("Status must be '0 - Hold' or '1 - for Active'");
                }

                setField(__qualifiedTags, val, "Status");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setSSN(string val)
        {
            try
            {
                val.Remove('-');  // SSN shouldn't have a '-'

                setField(__qualifiedTags, val, "SSN");
            }
            catch (Exception e)
            {
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public void setGender(string val)
        {
            try
            {
                if(val.ToUpper() != "F" && val.ToUpper() != "M")
                {
                    throw new Exception("Gender  M or F'");
                }

                setField(__qualifiedTags, val, "Gender");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
