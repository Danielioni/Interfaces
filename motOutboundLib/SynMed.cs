using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motOutboundLib
{
    class SynMed
    {
    }

    class SynMedHeader
    {
        public string RECORD_TYPE { get; set; } = "H";
        public int RECORD_COUNT { get; set; }
    }
    class SynMedField
    {
        public string __name { get; set; }
        public bool __mandatory { get; set; } = false;
        public object __data
        {
            set { }
            
            get
            {
                return __out();
            }
        }
        public int __length { get; set; }

        public SynMedField(string __name, object __data, bool __mandatory, int __length = 0)
        {
            this.__name = __name;
            this.__data = __data;
            this.__mandatory = __mandatory;
            this.__length = __length;
        }

        // Normalize field output
        private string __out()
        {      
            if(__data.GetType() ==  typeof(DateTime))
            {
                DateTime __dt;

                if(DateTime.TryParse(__data.ToString(), out __dt))
                {
                    // Output as a properly formatted date
                    if (__name.ToUpper().Contains("DATE"))
                    {
                        // Output as YYYY-MM-DD
                        return __dt.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        // Output as HH:mm
                        return __dt.ToString("HH:mm");
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            
            return __length == 0 ? __data.ToString() : __data.ToString().Substring(0, __length);
        }
    }
    class SynMedRow
    {
        private List<SynMedField> __field_list;

        public SynMedRow()
        {
            __field_list = new List<SynMedField>();
        }

        public string   RECORD_TYPE { set { __field_list.Add(new SynMedField("RECORD_TYPE", value, false, 5)); } }
        public DateTime ADMINISTRATION_DATE { set { __field_list.Add(new SynMedField("ADMINISTRATION_DATE", value, true, 10)); } }
        public DateTime ADMINISTRATION_TIME { set { __field_list.Add(new SynMedField("ADMINISTRATION_TIME", value, true, 11)); } }
        public string   LOCAL_DRUG_ID { set { __field_list.Add(new SynMedField("LOCAL_DRUG_ID", value, true, 15)); } }
        public int      DRUG_QUANTITY { set { __field_list.Add(new SynMedField("DRUG_QUANTITY", value, true)); } }
        public string   DRUG_DESCRIPTION { set { __field_list.Add(new SynMedField("DRUG_DESCRIPTION", value, true, 75)); } }
        public string   DISPLAY_NAME { set { __field_list.Add(new SynMedField("DISPLAY_NAME", value, false, 75)); } }
        public string   EXTERNAL_DRUG_FLAG { set { if(value == "Y" || value =="N") __field_list.Add(new SynMedField("EXTERNAL_DRUG_FLAG", value, false, 1)); } }
        public string   NOT_IN_BLISTER { set { if (value == "Y" || value == "N") __field_list.Add(new SynMedField("NOT_IN_BLISTER", value, false, 1)); } }
        public string   PRESCRIPTION_NUMBER { set { __field_list.Add(new SynMedField("PRESCRIPTION_NUMBER", value, true, 15)); } }
        public string   PATIENT_ID { set { __field_list.Add(new SynMedField("PATIENT_ID", value, true, 10)); } }
        public string   PATIENT_FULL_NAME { set { __field_list.Add(new SynMedField("PATIENT_FULL_NAME", value, true, 50)); } }
        public string   PATIENT_LANGUAGE { set { __field_list.Add(new SynMedField("PATIENT_LANGUAGE", value, false, 3)); } }
        public string   PATIENT_FIRST_NAME { set { __field_list.Add(new SynMedField("PATIENT_FIRST_NAME", value, false, 25)); } }
        public string   PATIENT_LAST_NAME { set { __field_list.Add(new SynMedField("PATIENT_LAST_NAME", value, false, 25)); } }
        public string   PATIENT_MOTHER_LAST_NAME { set { __field_list.Add(new SynMedField("PATIENT_MOTHER_LAST_NAME", value, false, 20)); } }
        public string   PATIENT_ADDRESS { set { __field_list.Add(new SynMedField("PATIENT_ADDRESS", value, false, 50)); } }
        public string   PATIENT_CITY { set { __field_list.Add(new SynMedField("PATIENT_CITY", value, false, 50)); } }
        public string   PATIENT_STATE { set { __field_list.Add(new SynMedField("PATIENT_STATE", value, false, 50)); } }
        public string   PATIENT_ZIP_CODE { set { __field_list.Add(new SynMedField("PATIENT_ZIP_CODE", value, false, 25)); } }
        public string   PATIENT_COUNTRY { set { __field_list.Add(new SynMedField("PATIENT_COUNTRY", value, false, 50)); } }
        public string   PATIENT_BIN_NUMBER { set { __field_list.Add(new SynMedField("PATIENT_BIN_NUMBER", value, false, 10)); } }
        public string   PATIENT_PHONE_NUMBER { set { __field_list.Add(new SynMedField("PATIENT_PHONE_NUMBER", value, false, 25)); } }
        public DateTime PATIENT_BIRTH_DATE { set { __field_list.Add(new SynMedField("PATIENT_BIRTH_DATE", value, false, 25)); } }
        public string   PATIENT_WITH_PRN { set { if (value == "Y" || value == "N") __field_list.Add(new SynMedField("PATIENT_PHONE_NUMBER", value, false, 1)); } }
        public int      QTY_PER_ADMINISTRATION { set { __field_list.Add(new SynMedField("QTY_PER_ADMINISTRATION", value, false)); } }
        public int      ADMINISTRATION_PER_DAY { set { __field_list.Add(new SynMedField("ADMINISTRATION_PER_DAY", value, false)); } }
        public int      DAY_LAPSE { set { __field_list.Add(new SynMedField("DAY_LAPSE", value, false)); } }
        public string   PERIOD_NAME { set { __field_list.Add(new SynMedField("PERIOD_NAME", value, false, 8)); } }
        public DateTime PERIOD_BEGINNING_TIME { set { __field_list.Add(new SynMedField("PERIOD_BEGINNING_TIME", value, false)); } }
        public DateTime PERIOD_ENDING_TIME { set { __field_list.Add(new SynMedField("PERIOD_ENDING_TIME", value, false)); } }
        public int      PERIOD_ORDER { set { __field_list.Add(new SynMedField("PERIOD_ORDER", value, false)); } }
        public string   IS_HOUR_DRIVEN { set { if (value == "Y" || value == "N") __field_list.Add(new SynMedField("IS_HOUR_DRIVEN", value, false, 1)); } }
        public string   INSTITUTION_NAME { set { __field_list.Add(new SynMedField("INSTITUTION_NAME", value, false, 30)); } }
        public string   INSTITUTION_UNIT_NAME { set { __field_list.Add(new SynMedField("INSTITUTION_UNIT_NAME", value, false, 25)); } }
        public string   INSTITUTION_FLOOR_LEVEL { set { __field_list.Add(new SynMedField("INSTITUTION_FLOOR_LEVEL", value, false, 15)); } }
        public string   INSTITUTION_ROOM_NUMBER { set { __field_list.Add(new SynMedField("INSTITUTION_ROOM_NUMBER", value, false, 15)); } }
        public string   INSTITUTION_BED_NUMBER { set { __field_list.Add(new SynMedField("INSTITUTION_BED_NUMBER", value, false, 15)); } }
        public string   PHYSICIAN_NAME { set { __field_list.Add(new SynMedField("PHYSICIAN_NAME", value, false, 25)); } }
        public string   PHYSICIAN_LICENCE { set { __field_list.Add(new SynMedField("PHYSICIAN_LICENCE", value, false, 15)); } }
        public string   PHARMACIST_NAME { set { __field_list.Add(new SynMedField("PHARMACIST_NAME", value, false, 30)); } }
        public int      REFILL_QUANTITY { set { __field_list.Add(new SynMedField("REFILL_QUANTITY", value, false)); } }
        public DateTime FIRST_REFILL_DATE { set { __field_list.Add(new SynMedField("FIRST_REFILL_DATE", value, false)); } }
        public DateTime LAST_REFILL_DATE { set { __field_list.Add(new SynMedField("LAST_REFILL_DATE", value, false)); } }
        public int      COST { set { __field_list.Add(new SynMedField("COST", value, false)); } }
        public string   PRESCRIPTION_INSTRUCTION { set { __field_list.Add(new SynMedField("PRESCRIPTION_INSTRUCTION", value, false, 90)); } }
        public string   PRESCRIPTION_COMMENT { set { __field_list.Add(new SynMedField("PRESCRIPTION_COMMENT", value, false, 75)); } }
        public string   REORDER_NUMBER { set { __field_list.Add(new SynMedField("REORDER_NUMBER", value, false, 25)); } }
        public string   INSTRUCTION_REASON { set { __field_list.Add(new SynMedField("INSTRUCTION_REASON", value, false, 35)); } }
        public string   GROUP_TITLE { set { __field_list.Add(new SynMedField("GROUP_TITLE", value, false, 50)); } }
        public string   CARD_NOTE_01 { set { __field_list.Add(new SynMedField("CARD_NOTE_01", value, false, 35)); } }
        public string   CARD_NOTE_02 { set { __field_list.Add(new SynMedField("CARD_NOTE_02", value, false, 35)); } }
        public string   CELL_NOTE { set { __field_list.Add(new SynMedField("CELL_NOTE", value, false, 35)); } }
        public string   PHARMACY_ACCREDITATION_NUMBER { set { __field_list.Add(new SynMedField("PHARMACY_ACCREDITATION_NUMBER", value, false, 35)); } }
        public string   ORDER_ID { set { __field_list.Add(new SynMedField("ORDER_ID", value, false, 10)); } }
        public DateTime CYCLE_BASE_DATE { set { __field_list.Add(new SynMedField("CYCLE_BASE_DATE", value, false)); } }
        public int      CYCLE_LENGTH { set { __field_list.Add(new SynMedField("CYCLE_LENGTH", value, false)); } }
        public string CYCLE_FIRST_DAY_FIXED { set { if (value == "Y" || value == "N") __field_list.Add(new SynMedField("CYCLE_FIRST_DAY_FIXED", value, false, 1)); } }
        public string PERIOD_NAME_01 { set { __field_list.Add(new SynMedField("PERIOD_NAME_01", value, false, 8)); } }
        public string PERIOD_NAME_02 { set { __field_list.Add(new SynMedField("PERIOD_NAME_02", value, false, 8)); } }
        public string PERIOD_NAME_03 { set { __field_list.Add(new SynMedField("PERIOD_NAME_01", value, false, 8)); } }
        public string PERIOD_NAME_04 { set { __field_list.Add(new SynMedField("PERIOD_NAME_04", value, false, 8)); } }
        public string ONE_MAR_DOSE_ID { set { __field_list.Add(new SynMedField("ONE_MAR_DOSE_ID", value, false, 2)); } }
        public string ONE_MAR_WEB_SITE { set { __field_list.Add(new SynMedField("ONE_MAR_WEB_SITE", value, false, 2)); } }

        public void write_header()
        {
            foreach (SynMedField __field in __field_list)
            {
                // write __field.__name + ","
            }
        }
        public void write()
        {
            foreach(SynMedField __field in __field_list)
            {
                // write __field.__data + ","

                if(__field.__mandatory && string.IsNullOrEmpty(__field.__data.ToString()))
                {
                    throw new ArgumentNullException("Missing required field: " + __field.__name);
                }

            }
        }
    }


    class SynMedTable
    {
        List<SynMedRow> __rows;
    }
}
