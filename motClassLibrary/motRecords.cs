using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motInboundLib
{
    public class Field
    {
        public string tagName { get; set; }

        public string tagData { get; set; }

        public int maxLen { get; set; }

        bool required { get; set; }

        char when { get; set; }

        public Field(string f, string t, int m, bool r, char w)
        {
            tagName = f;
            tagData = t;
            maxLen = m;
            required = r;
            when = w;
        }
    }

    public class Header
    {
        public Action __action { get; set; }
        public RecordType __type { get; set; }

        public List<Field> __qualifiedTags;

        public Header()
        {
            __qualifiedTags = new List<Field>();
        }
    }


    public class motDrugRecord : Header
    {
        public motDrugRecord()
        {
        }

        Field RxSysDrug
        {
            set
            {
                string val = value;

                Field f = __qualifiedTags.Find(x => x.tagName.Contains("RxSys_DrugID"));
                if (val.Length > f.maxLen)
                {
                    throw new Exception("RxSys_DrugID Field Overflow");
                }

                f.tagData = val;
            }

            get;
        }

        public motDrugRecord(Action a)
        {
            // Load in the data
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

        public void setRxSys_DrugID(string val)
        {
            Field f = __qualifiedTags.Find(x => x.tagName.Contains("RxSys_DrugID"));
            if (val.Length > f.maxLen)
            {
                throw new Exception("RxSys_DrugID Field Overflow");
            }

            f.tagData = val;
        }

        public void setLabelCode(string val)
        {
            Field f = __qualifiedTags.Find(x => x.tagName.Contains("LblCode"));
            if (val.Length > f.maxLen)
            {
                throw new Exception("LabelCode Field Overflow");
            }

            f.tagData = val;
        }

        public void setProductCode(string val)
        {
            Field f = __qualifiedTags.Find(x => x.tagName.Contains("ProdCode"));
            if (val.Length > f.maxLen)
            {
                throw new Exception("ProductCode Field Overflow");
            }

            f.tagData = val;
        }

        public void setTradeName(string val)
        {
            Field f = __qualifiedTags.Find(x => x.tagName.Contains("TradeName"));
            if (val.Length > f.maxLen)
            {
                throw new Exception("TradeName Field Overflow");
            }

            f.tagData = val;
        }

    }


    class motPrescriberRecord
    {

    }

}
