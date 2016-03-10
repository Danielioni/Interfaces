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
            catch(Exception e)
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
            setField(__qualifiedTags, val, "RxSys_DrugID");
        }
        public void setLabelCode(string val)
        {
            setField(__qualifiedTags, val, "LblCode");
        }
        public void setProductCode(string val)
        {
            setField(__qualifiedTags, val, "ProdCode");
        }
        public void setTradeName(string val)
        {
            setField(__qualifiedTags, val, "TradeName");
        }
        public void setStrength(string val)
        {
            setField(__qualifiedTags, val, "Strength");
        }
        public void setUnit(string val)
        {
            setField(__qualifiedTags, val, "Unit");
        }
        public void setRxOTC(string val)
        {
            setField(__qualifiedTags, val, "RxOTC");
        }
        public void setDoseForm(string val)
        {
            setField(__qualifiedTags, val, "DoseForm");
        }
        public void setRoute(string val)
        {
            setField(__qualifiedTags, val, "Route");
        }
        public void setDrugSchedule(string val)
        {
            setField(__qualifiedTags, val, "DrugSchedule");
        }
        public void setVisualDescription(string val)
        {
            setField(__qualifiedTags, val, "VisualDescription");
        }
        public void setDrugName(string val)
        {
            setField(__qualifiedTags, val, "DrugName");
        }
        public void setShortName(string val)
        {
            setField(__qualifiedTags, val, "ShortName");
        }
        public void setNDCNum(string val)
        {
            setField(__qualifiedTags, val, "NDCNum");
        }
        public void setSizeFactor(string val)
        {
            setField(__qualifiedTags, val, "SizeFactor");
        }
        public void setTemplate(string val)
        {
            setField(__qualifiedTags, val, "Template");
        }
        public void setDefaultIsolate(string val)
        {
            setField(__qualifiedTags, val, "DefaultIsolate");
        }
        public void setConsultMsg(string val)
        {
            setField(__qualifiedTags, val, "ConsultMsg");
        }
        public void setGenericFor(string val)
        {
            setField(__qualifiedTags, val, "GenericFor");
        }

        public void Write(Port p)
        {
            try
            {
                Write(p, __qualifiedTags);
            }
            catch(Exception e)
            {
                throw;
            }
        }
    }


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
                __qualifiedTags.Add(new Field("DEA_ID", "",10, true, 'w'));
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
            setField(__qualifiedTags, val, "RxSys_DocID");
        }
    }

}
