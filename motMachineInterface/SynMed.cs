using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Data;
using NLog;

using Autofac;
using Mot.Client.Sdk;
using Mot.Client.Sdk.Patients;
using Mot.Client.Sdk.Cards;


using Mot.Shared.Model.Cards;
using Mot.Shared.Model.Rxes;
using Mot.Shared.Model.Rxes.RxRegimens;
using Mot.Shared.Model.Patients;

using Mot.Shared.Framework;

using motCommonLib;

namespace motMachineInterface
{
    /// <summary>
    /// SynMed Table Abstractions and Output Generator
    /// </summary>
    public class SynMedHeader
    {
        public string RECORD_TYPE { get; set; } = "H";
        public int RECORD_COUNT { get; set; }
    }
    public class SynMedField
    {
        public string __name { get; set; }
        public bool __mandatory { get; set; } = false;
        public object __data { get; set; }
        public int __length { get; set; }
        public SynMedField(string __name, object __data, bool __mandatory, int __length = 0)
        {
            this.__name = __name;
            this.__data = __data;
            this.__mandatory = __mandatory;
            this.__length = __length;
        }
    }
    public class SynMedRow
    {
        private List<SynMedField> __field_list;

        public SynMedRow()
        {
            __field_list = new List<SynMedField>();

            Type __SynMedFieldData = typeof(SynMedRow);
            PropertyInfo[] __field_data = __SynMedFieldData.GetProperties();
        }
        public string RECORD_TYPE
        {
            get { return (string)__field_list.Find(f => f?.__name == "RECORD_TYPE")?.__data; }
            set { __field_list.Add(new SynMedField("RECORD_TYPE", value?.Trim(), false, 5)); }
        }
        public string ADMINISTRATION_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "ADMINISTRATION_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("ADMINISTRATION_DATE", value?.Trim(), true, 10)); }
        }
        public string ADMINISTRATION_TIME
        {
            get { return (string)__field_list.Find(f => f?.__name == "ADMINISTRATION_TIME")?.__data; }
            set { __field_list.Add(new SynMedField("ADMINISTRATION_TIME", value?.Trim(), true, 11)); }
        }
        public string LOCAL_DRUG_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "LOCAL_DRUG_ID")?.__data; }
            set { __field_list.Add(new SynMedField("LOCAL_DRUG_ID", value?.Trim(), true, 15)); }
        }
        public string DRUG_QUANTITY
        {
            get { return (string)__field_list.Find(f => f?.__name == "DRUG_QUANTITY")?.__data; }
            set { __field_list.Add(new SynMedField("DRUG_QUANTITY", value?.Trim(), true)); }
        }
        public string DRUG_DESCRIPTION
        {
            get { return (string)__field_list.Find(f => f?.__name == "DRUG_DESCRIPTION")?.__data; }
            set { __field_list.Add(new SynMedField("DRUG_DESCRIPTION", value?.Trim(), true, 75)); }
        }
        public string DISPLAY_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "DISPLAY_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("DISPLAY_NAME", value?.Trim(), false, 75)); }
        }
        public string EXTERNAL_DRUG_FLAG
        {
            get { return (string)__field_list.Find(f => f?.__name == "EXTERNAL_DRUG_FLAG")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("EXTERNAL_DRUG_FLAG", value?.Trim(), false, 1)); }
        }
        public string NOT_IN_BLISTER
        {
            get { return (string)__field_list.Find(f => f?.__name == "NOT_IN_BLISTER")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("NOT_IN_BLISTER", value?.Trim(), false, 1)); }
        }
        public string PRESCRIPTION_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PRESCRIPTION_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("PRESCRIPTION_NUMBER", value?.Trim(), true, 15)); }
        }
        public string PATIENT_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_ID")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_ID", value?.Trim(), true, 10)); }
        }
        public string PATIENT_FULL_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_FULL_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_FULL_NAME", value?.Trim(), true, 50)); }
        }
        public string PATIENT_LANGUAGE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_LANGUAGE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_LANGUAGE", value?.Trim(), false, 3)); }
        }
        public string PATIENT_FIRST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_FIRST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_FIRST_NAME", value?.Trim(), false, 25)); }
        }
        public string PATIENT_LAST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_LAST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_LAST_NAME", value?.Trim(), false, 25)); }
        }
        public string PATIENT_MOTHER_LAST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_MOTHER_LAST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_MOTHER_LAST_NAME", value?.Trim(), false, 20)); }
        }
        public string PATIENT_ADDRESS
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_ADDRESS")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_ADDRESS", value?.Trim(), false, 50)); }
        }
        public string PATIENT_CITY
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_CITY")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_CITY", value?.Trim(), false, 50)); }
        }
        public string PATIENT_STATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_STATE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_STATE", value?.Trim(), false, 50)); }
        }
        public string PATIENT_ZIP_CODE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_ZIP_CODE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_ZIP_CODE", value?.Trim(), false, 25)); }
        }
        public string PATIENT_COUNTRY
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_COUNTRY")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_COUNTRY", value?.Trim(), false, 50)); }
        }
        public string PATIENT_BIN_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_BIN_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_BIN_NUMBER", value?.Trim(), false, 10)); }
        }
        public string PATIENT_PHONE_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_PHONE_NUMBER")?.__data; }
            set { __field_list?.Add(new SynMedField("PATIENT_PHONE_NUMBER", value?.Trim(), false, 25)); }
        }
        public string PATIENT_BIRTH_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_BIRTH_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_BIRTH_DATE", value?.Trim(), false, 25)); }
        }
        public string PATIENT_WITH_PRN
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_WITH_PRN")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("PATIENT_WITH_PRN", value?.Trim(), false, 1)); }
        }
        public string QTY_PER_ADMINISTRATION
        {
            get { return (string)__field_list.Find(f => f?.__name == "QTY_PER_ADMINISTRATION")?.__data; }
            set { __field_list.Add(new SynMedField("QTY_PER_ADMINISTRATION", value?.Trim(), false)); }
        }
        public string ADMINISTRATION_PER_DAY
        {
            get { return (string)__field_list.Find(f => f?.__name == "ADMINISTRATION_PER_DAY")?.__data; }
            set { __field_list.Add(new SynMedField("ADMINISTRATION_PER_DAY", value?.Trim(), false)); }
        }
        public string DAY_LAPSE
        {
            get { return (string)__field_list.Find(f => f?.__name == "DAY_LAPSE")?.__data; }
            set { __field_list.Add(new SynMedField("DAY_LAPSE", value?.Trim(), false)); }
        }
        public string PERIOD_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_NAME", value?.Trim(), false, 8)); }
        }
        public string PERIOD_BEGINNING_TIME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_BEGINNING_TIME")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_BEGINNING_TIME", value?.Trim(), false)); }
        }
        public string PERIOD_ENDING_TIME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_ENDING_TIME")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_ENDING_TIME", value?.Trim(), false)); }
        }
        public string PERIOD_ORDER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_ORDER")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_ORDER", value?.Trim(), false)); }
        }
        public string IS_HOUR_DRIVEN
        {
            get { return (string)__field_list.Find(f => f?.__name == "IS_HOUR_DRIVEN")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("IS_HOUR_DRIVEN", value?.Trim(), false, 1)); }
        }
        public string INSTITUTION_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_NAME", value?.Trim(), false, 30)); }
        }
        public string INSTITUTION_UNIT_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_UNIT_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_UNIT_NAME", value?.Trim(), false, 25)); }
        }
        public string INSTITUTION_FLOOR_LEVEL
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_FLOOR_LEVEL")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_FLOOR_LEVEL", value?.Trim(), false, 15)); }
        }
        public string INSTITUTION_ROOM_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_ROOM_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_ROOM_NUMBER", value?.Trim(), false, 15)); }
        }
        public string INSTITUTION_BED_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_BED_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_BED_NUMBER", value?.Trim(), false, 15)); }
        }
        public string PHYSICIAN_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHYSICIAN_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PHYSICIAN_NAME", value?.Trim(), false, 25)); }
        }
        public string PHYSICIAN_LICENCE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHYSICIAN_LICENCE")?.__data; }
            set { __field_list.Add(new SynMedField("PHYSICIAN_LICENCE", value?.Trim(), false, 15)); }
        }
        public string PHARMACIST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHARMACIST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PHARMACIST_NAME", value?.Trim(), false, 30)); }
        }
        public string REFILL_QUANTITY
        {
            get { return (string)__field_list.Find(f => f?.__name == "REFILL_QUANTITY")?.__data; }
            set { __field_list.Add(new SynMedField("REFILL_QUANTITY", value?.Trim(), false)); }
        }
        public string FIRST_REFILL_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "FIRST_REFILL_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("FIRST_REFILL_DATE", value?.Trim(), false)); }
        }
        public string LAST_REFILL_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "LAST_REFILL_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("LAST_REFILL_DATE", value?.Trim(), false)); }
        }
        public string COST
        {
            get { return (string)__field_list.Find(f => f?.__name == "COST")?.__data; }
            set { __field_list.Add(new SynMedField("COST", value?.Trim(), false)); }
        }
        public string PRESCRIPTION_INSTRUCTION
        {
            get { return (string)__field_list.Find(f => f?.__name == "PRESCRIPTION_INSTRUCTION")?.__data; }
            set { __field_list.Add(new SynMedField("PRESCRIPTION_INSTRUCTION", value?.Trim(), false, 90)); }
        }
        public string PRESCRIPTION_COMMENT
        {
            get { return (string)__field_list.Find(f => f?.__name == "PRESCRIPTION_COMMENT")?.__data; }
            set { __field_list.Add(new SynMedField("PRESCRIPTION_COMMENT", value?.Trim(), false, 75)); }
        }
        public string REORDER_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "REORDER_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("REORDER_NUMBER", value?.Trim(), false, 25)); }
        }
        public string INSTRUCTION_REASON
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTRUCTION_REASON")?.__data; }
            set { __field_list.Add(new SynMedField("INSTRUCTION_REASON", value?.Trim(), false, 35)); }
        }
        public string GROUP_TITLE
        {
            get { return (string)__field_list.Find(f => f?.__name == "GROUP_TITLE")?.__data; }
            set { __field_list.Add(new SynMedField("GROUP_TITLE", value?.Trim(), false, 50)); }
        }
        public string CARD_NOTE_01
        {
            get { return (string)__field_list.Find(f => f?.__name == "CARD_NOTE_01")?.__data; }
            set { __field_list.Add(new SynMedField("CARD_NOTE_01", value?.Trim(), false, 35)); }
        }
        public string CARD_NOTE_02
        {
            get { return (string)__field_list.Find(f => f?.__name == "CARD_NOTE_02")?.__data; }
            set { __field_list.Add(new SynMedField("CARD_NOTE_02", value?.Trim(), false, 35)); }
        }
        public string CELL_NOTE
        {
            get { return (string)__field_list.Find(f => f?.__name == "CELL_NOTE")?.__data; }
            set { __field_list.Add(new SynMedField("CELL_NOTE", value?.Trim(), false, 35)); }
        }
        public string PHARMACY_ACCREDITATION_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHARMACY_ACCREDITATION_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("PHARMACY_ACCREDITATION_NUMBER", value?.Trim(), false, 35)); }
        }
        public string ORDER_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "ORDER_ID")?.__data; }
            set { __field_list.Add(new SynMedField("ORDER_ID", value?.Trim(), false, 10)); }
        }
        public string CYCLE_BASE_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "CYCLE_BASE_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("CYCLE_BASE_DATE", value?.Trim(), false)); }
        }
        public string CYCLE_LENGTH
        {
            get { return (string)__field_list.Find(f => f?.__name == "CYCLE_LENGTH")?.__data; }
            set { __field_list.Add(new SynMedField("CYCLE_LENGTH", value?.Trim(), false)); }
        }
        public string CYCLE_FIRST_DAY_FIXED
        {
            get { return (string)__field_list.Find(f => f?.__name == "CYCLE_FIRST_DAY_FIXED")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("CYCLE_FIRST_DAY_FIXED", value?.Trim(), false, 1)); }
        }

        public string ONE_MAR_DOSE_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "ONE_MAR_DOSE_ID")?.__data; }
            set { __field_list.Add(new SynMedField("ONE_MAR_DOSE_ID", value?.Trim(), false, 2)); }
        }
        public string ONE_MAR_WEB_SITE
        {
            get { return (string)__field_list.Find(f => f?.__name == "ONE_MAR_WEB_SITE")?.__data; }
            set { __field_list.Add(new SynMedField("ONE_MAR_WEB_SITE", value?.Trim(), false, 2)); }
        }
        public string PP_FILE_CELL_POSITION
        {
            get { return (string)__field_list.Find(f => f?.__name == "PP_FILE_CELL_POSITION")?.__data; }
            set { __field_list.Add(new SynMedField("PP_FILE_CELL_POSITION", value?.Trim(), false, 2)); }
        }
        public string PP_FILE_CELL_POSITION_X
        {
            get { return (string)__field_list.Find(f => f?.__name == "PP_FILE_CELL_POSITION_X")?.__data; }
            set { __field_list.Add(new SynMedField("PP_FILE_CELL_POSITION_X", value?.Trim(), false, 2)); }
        }
        public string PP_FILE_CELL_POSITION_Y
        {
            get { return (string)__field_list.Find(f => f?.__name == "PP_FILE_CELL_POSITION_Y")?.__data; }
            set { __field_list.Add(new SynMedField("PP_FILE_CELL_POSITION_Y", value?.Trim(), false, 2)); }
        }
        public void write(string __filename, FileMode __mode)
        {
            try
            {
                // Collect the CSV data row
                var __csv_row = string.Empty;
                Type __SynMedFieldData = typeof(SynMedRow);
                PropertyInfo[] __field_data = __SynMedFieldData.GetProperties();

                foreach (var __f in __field_data)
                {
                    //__csv_row += __f.GetValue(this, null) + ";";
                    __csv_row += __f.GetValue(this, null) + "\t";
                }

                __csv_row = __csv_row.Substring(0, __csv_row.Length - 1);

                using (var __fs = new FileStream(__filename, FileMode.Append, FileAccess.Write))
                {
                    using (var __sw = new StreamWriter(__fs))
                    {
                        
                        //while(__csv_row.Contains('\n'))
                        //{
                        //    __csv_row = __csv_row.Remove(__csv_row.IndexOf('\n'), 1);
                        //}

                        __sw.WriteLine(__csv_row);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
    public class SynMedTable
    {
        public static Dictionary<int, string> __state = new Dictionary<int, string>()
        {
            { 0, "AL" }, { 1, "AK" }, { 2, "AZ" },
            { 3, "AR" }, { 4, "CA" }, { 5, "CO" },
            { 6, "CT" }, { 7, "DE" }, { 8, "DC" },
            { 9, "FL" }, { 10, "GA" }, { 11, "HI" },
            { 12, "ID" }, { 13, "IL" }, { 14, "IN" },
            { 15, "IA" }, { 16, "KS" }, { 17, "KY" },
            { 18, "LA" }, { 19, "ME" }, { 20, "MD" },
            { 21, "MA" }, { 22, "MI" }, { 23, "MN" },
            { 24, "MS" }, { 25, "MO" }, { 26, "MT" },
            { 27, "NE" }, { 28, "NV" }, { 29, "NH" },
            { 30, "NJ" }, { 31, "NM" }, { 32, "NY" },
            { 33, "NC" }, { 34, "ND" }, { 35, "OH" },
            { 36, "OK" }, { 37, "OR" }, { 38, "PA" },
            { 39, "RI" }, { 40, "SC" }, { 41, "SD" },
            { 42, "TN" }, { 43, "TX" }, { 44, "UT" },
            { 45, "VT" }, { 46, "VA" }, { 47, "WA" },
            { 48, "WV" }, { 49, "WI" }, { 50, "WY" }
        };

        // Components
        private string __csv_header;
        private List<SynMedRow> __table_rows;

        // Parameters
        public string __patient_name { get; set; }
        public string __patient_last_name { get; set; }
        public string __patient_first_name { get; set; }
        public string __patient_middle_initial { get; set; }
        public DateTime __patient_dob { get; set; }
        public DateTime __cycle_start_date { get; set; }
        public int __cycle_length { get; set; } = 30;
        public string __filename { get; set; } = @".\__synmed_default";

        // Targets
        private motCommonLib.motSocket __socket;
        private FileStream __file;
        private FileMode __mode = FileMode.CreateNew;

        private void __write_to_file()
        {
            if (__table_rows.Count > 0)
            {
                foreach (var __row in __table_rows)
                {
                    __row.write(__filename, __mode);
                }
            }
            else
            {
                File.Delete(__filename);
            }
        }
        private int __rx_dose_time_compare(SynMedRow a, SynMedRow b)
        {
            return string.Compare(a.ADMINISTRATION_DATE, b.ADMINISTRATION_DATE);
        }
        public void WriteByCycleDate(DateTime __cycle_start_date)
        { }
        public void WriteByPatient()
        { }
        public void fillLegacyRow(SynMedRow __new_row, motLegacySynMed.__motsynmed_patient __pat, motLegacySynMed.__motsynmed_rx __rx, motLegacySynMed.__motsynmed_daily_dose __dose, DateTime __base_date, DateTime __current_date)
        {
            __new_row.RECORD_TYPE = "15";
            __new_row.ADMINISTRATION_DATE = string.Format("{0:yyyyMMdd}", __current_date);
            __new_row.ADMINISTRATION_TIME = string.Format("{0}", __dose.__dose_time);
            __new_row.DRUG_QUANTITY = __dose.__qty.ToString();

            __new_row.LOCAL_DRUG_ID = __rx.__NDC;
            __new_row.DRUG_DESCRIPTION = !string.IsNullOrEmpty(__rx.__visual_description) ? __rx.__visual_description : "UNKNOWN";
            __new_row.DISPLAY_NAME = __rx.__cup_name;

            __new_row.EXTERNAL_DRUG_FLAG = "";
            __new_row.NOT_IN_BLISTER = (__rx.__bulk == 1 || __rx.__chart_only == 1) ? "Y" : "N";

            __new_row.PRESCRIPTION_NUMBER = __rx.__rx_num.ToString();
            __new_row.PATIENT_ID = __pat.__i_patient_id.ToString();
            __new_row.PATIENT_FULL_NAME = string.Format("{0} {1} {2}", __pat.__first_name, __pat.__middle_initial, __pat.__last_name);
            __new_row.PATIENT_LANGUAGE = "";
            __new_row.PATIENT_FIRST_NAME = __patient_first_name;
            __new_row.PATIENT_LAST_NAME = __patient_last_name;
            __new_row.PATIENT_MOTHER_LAST_NAME = "";

            __new_row.PATIENT_ADDRESS = __pat.__main_address.__address1 + ", " + __pat.__main_address.__address2;
            __new_row.PATIENT_CITY = __pat.__main_address.__city;
            __new_row.PATIENT_STATE = __pat.__main_address.__state;
            __new_row.PATIENT_ZIP_CODE = __pat.__main_address.__zip;
            __new_row.PATIENT_COUNTRY = "";
            __new_row.PATIENT_BIN_NUMBER = "";
            __new_row.PATIENT_PHONE_NUMBER = !string.IsNullOrEmpty(__pat.__phone) ? __pat.__phone : "No Phone";

            __new_row.PATIENT_BIRTH_DATE = string.Format("{0:yyyyMMdd}", __pat.__dob);

            __new_row.PERIOD_NAME = "";
            __new_row.PERIOD_BEGINNING_TIME = "";   // string.Format("{0:yyyyMMdd}", DateTime.Now);
            __new_row.PERIOD_ENDING_TIME = "";      // string.Format("{0:hh:mm}", DateTime.Now);
            __new_row.PERIOD_ORDER = "";
            __new_row.IS_HOUR_DRIVEN = "";

            __new_row.INSTITUTION_NAME = !string.IsNullOrEmpty(__pat.__facility.__facility_name) ? __pat.__facility.__facility_name : "Independent";
            __new_row.INSTITUTION_UNIT_NAME = "";
            __new_row.INSTITUTION_FLOOR_LEVEL = "";
            __new_row.INSTITUTION_ROOM_NUMBER = !string.IsNullOrEmpty(__pat.__room) ? __pat.__room : string.Empty;
            __new_row.INSTITUTION_BED_NUMBER = "";

            __new_row.PHYSICIAN_NAME = string.Format(" {0} {1} {2}", __rx.__prescriber.__first_name, __rx.__prescriber.__middle_initial, __rx.__prescriber.__last_name);
            __new_row.PHYSICIAN_LICENCE = __rx.__prescriber.__dea;

            __new_row.PHARMACIST_NAME = "";
            __new_row.REFILL_QUANTITY = __rx.__refills.ToString();
            __new_row.FIRST_REFILL_DATE = "";
            __new_row.LAST_REFILL_DATE = "";
            __new_row.COST = "";
            __new_row.PRESCRIPTION_INSTRUCTION = __rx.__sig;

            if (string.IsNullOrEmpty(__new_row.PRESCRIPTION_COMMENT))
            {
                __new_row.PRESCRIPTION_COMMENT = "";
            }

            __new_row.REORDER_NUMBER = "";
            __new_row.INSTRUCTION_REASON = "";
            __new_row.GROUP_TITLE = "";
            __new_row.CARD_NOTE_01 = "";
            __new_row.CARD_NOTE_02 = "";
            __new_row.CELL_NOTE = "";
            __new_row.PHARMACY_ACCREDITATION_NUMBER = __pat.__store.__dea;
            __new_row.ORDER_ID = "";

            __new_row.CYCLE_BASE_DATE = string.Format("{0:yyyyMMdd}", __base_date);
            __new_row.CYCLE_LENGTH = __cycle_length.ToString();
            __new_row.CYCLE_FIRST_DAY_FIXED = "";

            __new_row.ONE_MAR_DOSE_ID = "";
            __new_row.ONE_MAR_WEB_SITE = "";

            __table_rows.Add(__new_row);

        }
        public void fillRow(SynMedRow __new_row, Rx __rx, DateTime __base_date, DateTime __current_date, DoseScheduleItem __dose)
        {
            __new_row.RECORD_TYPE = "15";
            __new_row.ADMINISTRATION_DATE = string.Format("{0:yyyyMMdd}", __current_date);
            __new_row.ADMINISTRATION_TIME = string.Format("{0:00}:{1:00}", __dose.GetTimespan().Hours, __dose.GetTimespan().Minutes);
            __new_row.DRUG_QUANTITY = __dose.Dose.ToString();

            // Strip the '-' out of the NDC
            string NDC = __rx.Drug.NdcNumber;

            while ((bool)NDC?.Contains("-"))
            {
                NDC = NDC.Remove(NDC.IndexOf("-"), 1);
            }

            __new_row.LOCAL_DRUG_ID = NDC;
            __new_row.DRUG_DESCRIPTION = !string.IsNullOrEmpty(__rx.Drug.VisualDescription) ? __rx.Drug.VisualDescription : "UNKNOWN";
            __new_row.DISPLAY_NAME = __rx.Drug.DosageCupName;

            // It's unclear how to do BULK scrips, maybe we just don't send them
            __new_row.EXTERNAL_DRUG_FLAG = "";
            __new_row.NOT_IN_BLISTER = (__rx.IsBulk || __rx.IsChartOnly) ? "Y" : "N";

            __new_row.PRESCRIPTION_NUMBER = __rx.RxSystemId;
            __new_row.PATIENT_ID = __rx.PatientId.ToString();
            __new_row.PATIENT_FULL_NAME = string.Format("{0} {1} {2}", __rx.Patient.FirstName, __rx.Patient.MiddleInitial, __rx.Patient.LastName);
            __new_row.PATIENT_LANGUAGE = "";
            __new_row.PATIENT_FIRST_NAME = __patient_first_name;
            __new_row.PATIENT_LAST_NAME = __patient_last_name;
            __new_row.PATIENT_MOTHER_LAST_NAME = "";
            __new_row.PATIENT_ADDRESS = __rx.Patient.UsePatientInfo ? __rx.Patient.Address.Address1 : __rx.Patient.Facility.Address.Address1;
            __new_row.PATIENT_CITY = __rx.Patient.UsePatientInfo ? __rx.Patient.Address.City : __rx.Patient.Facility.Address.City;

            if (__rx.Patient.UsePatientInfo && __rx.Patient.Address.State == null ||
              !__rx.Patient.UsePatientInfo && __rx.Patient.Facility.Address.State == null)
            {
                throw new ArgumentException("NULL State Value");
            }

            __new_row.PATIENT_STATE = __state[__rx.Patient.UsePatientInfo ? (int)__rx.Patient.Address.State : (int)__rx.Patient.Facility.Address.State];

            __new_row.PATIENT_ZIP_CODE = __rx.Patient.UsePatientInfo ? __rx.Patient.Address.PostalCode : __rx.Patient.Facility.Address.PostalCode;
            __new_row.PATIENT_COUNTRY = "";
            __new_row.PATIENT_BIN_NUMBER = "";
            __new_row.PATIENT_PHONE_NUMBER = !string.IsNullOrEmpty(__rx.Patient.PrimaryPhone) ? __rx.Patient.PrimaryPhone : "No Phone";

            __new_row.PATIENT_BIRTH_DATE = string.Format("{0:yyyyMMdd}", __rx.Patient.DateOfBirth);

            __new_row.PERIOD_NAME = "";
            __new_row.PERIOD_BEGINNING_TIME = "";   // string.Format("{0:yyyyMMdd}", DateTime.Now);
            __new_row.PERIOD_ENDING_TIME = "";      // string.Format("{0:hh:mm}", DateTime.Now);
            __new_row.PERIOD_ORDER = "";
            __new_row.IS_HOUR_DRIVEN = "";

            __new_row.INSTITUTION_NAME = !string.IsNullOrEmpty(__rx.Patient.Facility.Name) ? __rx.Patient.Facility.Name : "Independent";
            __new_row.INSTITUTION_UNIT_NAME = "";
            __new_row.INSTITUTION_FLOOR_LEVEL = "";
            __new_row.INSTITUTION_ROOM_NUMBER = !string.IsNullOrEmpty(__rx.Patient.Room) ? __rx.Patient.Room : string.Empty;
            __new_row.INSTITUTION_BED_NUMBER = "";

            __new_row.PHYSICIAN_NAME = string.Format(" {0} {1} {2}", __rx.Prescriber.FirstName, __rx.Prescriber.MiddleInitial, __rx.Prescriber.LastName);
            __new_row.PHYSICIAN_LICENCE = __rx.Prescriber.Dea;

            __new_row.PHARMACIST_NAME = "";
            __new_row.REFILL_QUANTITY = __rx.Refills.ToString();
            __new_row.FIRST_REFILL_DATE = "";
            __new_row.LAST_REFILL_DATE = "";
            __new_row.COST = "";
            __new_row.PRESCRIPTION_INSTRUCTION = __rx.CardSig;

            if (string.IsNullOrEmpty(__new_row.PRESCRIPTION_COMMENT))
            {
                __new_row.PRESCRIPTION_COMMENT = "";
            }

            __new_row.REORDER_NUMBER = "";
            __new_row.INSTRUCTION_REASON = "";
            __new_row.GROUP_TITLE = "";
            __new_row.CARD_NOTE_01 = "";
            __new_row.CARD_NOTE_02 = "";
            __new_row.CELL_NOTE = "";
            __new_row.PHARMACY_ACCREDITATION_NUMBER = __rx.Store.Dea;
            __new_row.ORDER_ID = "";

            __new_row.CYCLE_BASE_DATE = string.Format("{0:yyyyMMdd}", __base_date);
            __new_row.CYCLE_LENGTH = __cycle_length.ToString();
            __new_row.CYCLE_FIRST_DAY_FIXED = "";

            __new_row.ONE_MAR_DOSE_ID = "";
            __new_row.ONE_MAR_WEB_SITE = "";

            __table_rows.Add(__new_row);
        }
        public void WriteLegacyRxCollection(motLegacySynMed.__motsynmed_patient __patient)
        {
            DateTime __base_date = __cycle_start_date;
            SynMedRow __new_row;

            try
            {
                foreach (var __rx in __patient.__rxes)
                {
                    DateTime __start_date = __rx.__start_date;
                    DateTime __current_date = __start_date;
                    int X = 1;
                    int Y = 1;


                    if (__start_date <= __rx.__start_date && __rx.__expire_date > DateTime.Today)
                    {
                        switch (__rx.__type)
                        {
                            case RxType.Daily:
                                foreach (var __dose in __rx.__dose_schedule)  // The number of dose schedule items should be the number of cards
                                {
                                    for (int __index = 0; __index < __cycle_length; __index++)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Daily Prescription";
                                        __current_date = (DateTime)__base_date.AddDays(__index);

                                        __new_row.PP_FILE_CELL_POSITION = (__index + 1).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_X = (((__index % 7) + 1)).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_Y = (((__index) / 7) + 1).ToString();
                                      
                                        fillLegacyRow(__new_row, __patient, __rx, __dose, __start_date, __current_date);  // Any Daily Schedule (1 line per day)
                                    }
                                }

                                break;

                            case RxType.Prn:

                                motLegacySynMed.__motsynmed_daily_dose __prn_dose = new motLegacySynMed.__motsynmed_daily_dose();
                                __prn_dose.__dose_time = "12:00";
                                __prn_dose.__dose_schedule_name = "PRN";
                                __prn_dose.__isolate = 1;
                                __prn_dose.__qty = __rx.__qty_per_dose;
                                __prn_dose.__card_sn = Convert.ToInt32(__rx.__patient.__card_dose_sn["PRN"]);

                                //for (int __index = 0; __index < __patient.__cycle_days; __index++)
                                //{
                                    __new_row = new SynMedRow();
                                    __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn["PRN"];
                                    __new_row.PRESCRIPTION_COMMENT = "PRN Prescription";
                                    __new_row.PATIENT_WITH_PRN = "Y";
                                    __new_row.QTY_PER_ADMINISTRATION = __rx.__qty_per_dose.ToString();
                                    __new_row.DRUG_QUANTITY = (__rx.__qty_per_dose * __cycle_length).ToString();
                                    __new_row.ADMINISTRATION_PER_DAY = ((int)__rx.__qty_to_dispense / __cycle_length).ToString();
                                    __new_row.DAY_LAPSE = "1";

                                    __new_row.PP_FILE_CELL_POSITION = (0 + 1).ToString();
                                    __new_row.PP_FILE_CELL_POSITION_X = (((1 % 7) + 1)).ToString();
                                    __new_row.PP_FILE_CELL_POSITION_Y = (((1) / 7) + 1).ToString();

                                    fillLegacyRow(__new_row, __patient, __rx, __prn_dose, __start_date, __current_date);
                                //}

                                break;

                            case RxType.Alternating:
                                int __cup_num = 0;
                                 
                                foreach (var __day in __rx.__a_dose_schedule)
                                {
                                    foreach (var __dose in __day.__dose_schedule)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Alternating Prescription - every (" + __rx.__alternate_days + ") days";

                                        __new_row.PP_FILE_CELL_POSITION = (__cup_num + 1).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_X = (((__cup_num % 7) + 1)).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_Y = (((__cup_num) / 7) + 1).ToString();                                       
                                       
                                        fillLegacyRow(__new_row, __patient, __rx, __dose, __start_date, __day.__dose_date);
                                    }

                                    __cup_num += __rx.__alternate_days;
                                }

                                break;

                            case RxType.MonthlyTitrating:
                            case RxType.WeeklyTitrating:
                                __cup_num = 0;

                                if (__rx.__t_dose_schedule != null)
                                {
                                    

                                    foreach (var __day in __rx.__t_dose_schedule)
                                    {
                                        foreach (var __dose in __day.__dose_schedule)
                                        {
                                            __new_row = new SynMedRow();
                                            __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                            __new_row.PRESCRIPTION_COMMENT = "Monthly or Weekly Titrating Prescription";

                                            __new_row.PP_FILE_CELL_POSITION = (__cup_num + 1).ToString();
                                            __new_row.PP_FILE_CELL_POSITION_X = (((__cup_num % 7) + 1)).ToString();
                                            __new_row.PP_FILE_CELL_POSITION_Y = (((__cup_num) / 7) + 1).ToString();
                                        }

                                        __cup_num++;
                                    }
                                }

                                break;

                            case RxType.DayOfMonth:

                                if (__rx.__dom_list == null)
                                {
                                    continue;
                                }
                                __current_date = __start_date;
                                __cup_num = 0;

                                foreach (char __day in __rx.__dom_list)
                                {                                   
                                    if (__day == '1' && __rx.__dose_schedule != null)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__rx.__dose_schedule[0].__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Day Of Month Prescription";

                                        __new_row.PP_FILE_CELL_POSITION = (__cup_num + 1).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_X = (((__cup_num % 7) + 1)).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_Y = (((__cup_num) / 7) + 1).ToString();

                                        fillLegacyRow(__new_row, __patient, __rx, __rx.__dose_schedule[0], __start_date, __current_date);
                                    }

                                    __cup_num++;
                                    __current_date = __current_date.AddDays(1);                                    
                                }

                                break;

                            case RxType.DayOfWeek:
                                __current_date = __start_date;
                                __cup_num = 0;
                                foreach (byte __day in __rx.__dow_list)
                                {
                                    if (__day == '1' && __rx.__dose_schedule != null)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__rx.__dose_schedule[0].__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Day Of Week Prescription";

                                        __new_row.PP_FILE_CELL_POSITION = (__cup_num + 1).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_X = (((__cup_num % 7) + 1)).ToString();
                                        __new_row.PP_FILE_CELL_POSITION_Y = (((__cup_num) / 7) + 1).ToString();

                                        fillLegacyRow(__new_row, __patient, __rx, __rx.__dose_schedule[0], __start_date, __current_date);
                                    }

                                    __cup_num++;
                                    __current_date = __current_date.AddDays(1);
                                   
                                }

                                break;


                            case RxType.Sequential:
                                /*  Not Supported By SynMed 
                                var __sequential_regimen = __rx.osageRegimen as RxSequentialRegimen;

                                foreach (var __sequential_dose in __sequential_regimen.DoseSchedule.DoseScheduleItems)
                                {
                                    __new_row = new SynMedRow();
                                    __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                    __new_row.PRESCRIPTION_COMMENT = "Sequential Prescription";
                                    fillRow(__new_row, __rx, __base_date, __current_date, __sequential_dose);
                                }
                                */

                                break;

                            default:
                                break;
                        }
                    }
                }


                __write_to_file();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add row item: {0}", ex.StackTrace);
            }
        }
        public int __sort_by_time_ndc(SynMedRow a, SynMedRow b)
        {
            if (a.ADMINISTRATION_TIME == b.ADMINISTRATION_TIME && a.LOCAL_DRUG_ID == b.LOCAL_DRUG_ID)
            {
                return 1;
            }

            return 0;
        }
        public async Task WriteRxCollection(IEnumerable<Rx> __rxes, string __file_name, Patient __patient)
        {
            DateTime __base_date = __cycle_start_date;

            if (__file_name != null)
            {
                __filename = __file_name;
            }

            try
            {
                foreach(var r in __rxes)
                {
                    r.Patient = __patient;
                }

                SynMedRow __new_row;

                var scope = motNextSynMed.container;
                var __config = new CardSettings();

                //var __cardsQuery = scope.Resolve<IEntityQuery<Card>>();
                var __card = scope.Resolve<IPopulateCardsCommand>();
                var __sns = scope.Resolve<IManageCardsCommand>();


                IEnumerable<Card> __cards = await __card.PopulateCardForRxes(__rxes, __config);

                List<Guid> __batch = new List<Guid>();

                foreach (var c in __cards)
                {
                    __batch.Add(c.BatchId);
                }

                IEnumerable<KeyValuePair<Guid, int>> __serial_numbers = await __sns.SetCardsSerialNo(__batch);


                foreach (var __full_card in __cards)
                {
                    foreach (var __cup in __full_card.Cups)
                    {
                        foreach (var __drug in __cup.Drugs)
                        {
                            __new_row = new SynMedRow();

                            switch (__full_card.Capacity)
                            {
                                case CardCapacity.High:
                                    __new_row.RECORD_TYPE = "50";
                                    break;

                                case CardCapacity.Standard:
                                    __new_row.RECORD_TYPE = "15";
                                    break;

                                default:
                                    break;
                            }

                            foreach (var s in __serial_numbers)
                            {
                                if (s.Key == __full_card.Id)
                                {
                                    __new_row.GROUP_TITLE = s.Value.ToString();
                                }
                            }


                            __new_row.PP_FILE_CELL_POSITION = (__cup.CupNumber + 1).ToString();
                            __new_row.PP_FILE_CELL_POSITION_X = (((__cup.CupNumber) % 7) + 1).ToString();
                            __new_row.PP_FILE_CELL_POSITION_Y = (((__cup.CupNumber) / 7) + 1).ToString();

                            __new_row.ADMINISTRATION_DATE = string.Format("{0:yyyyMMdd}", __cup.Time);
                            __new_row.ADMINISTRATION_TIME = __cup.Time.TimeOfDay.ToString();
                            __new_row.CYCLE_BASE_DATE = string.Format("{0:yyyyMMdd}", __full_card.DueDate);
                            __new_row.DRUG_QUANTITY = __drug.Quantity.ToString();
                            __new_row.DISPLAY_NAME = __drug.DrugName;
                            __new_row.PATIENT_WITH_PRN = __drug.RxType == RxType.Prn ? "Y" : "N";
                            __new_row.DAY_LAPSE = __drug.RxType == RxType.Prn ? "1" : "";

                            __new_row.PRESCRIPTION_COMMENT = "Prescription type: " + __drug.RxType.ToString();


                            string NDC = string.Empty;

                            foreach (var __rx in __rxes)
                            {
                                if (__rx.Id == __drug.RxId)
                                {
                                    NDC = __rx.Drug.NdcNumber;

                                    while ((bool)NDC?.Contains("-"))
                                    {
                                        NDC = NDC.Remove(NDC.IndexOf("-"), 1);
                                    }

                                    __new_row.LOCAL_DRUG_ID = NDC;                            
                                    __new_row.DRUG_DESCRIPTION = !string.IsNullOrEmpty(__rx.Drug.VisualDescription) ? __rx.Drug.VisualDescription : "UNKNOWN";
                                    __new_row.DISPLAY_NAME = __rx.Drug.DosageCupName;
                                    __new_row.EXTERNAL_DRUG_FLAG = "";
                                    __new_row.NOT_IN_BLISTER = (__rx.IsBulk || __rx.IsChartOnly) ? "Y" : "N";
                                    __new_row.PRESCRIPTION_NUMBER = __rx.RxSystemId;
                                    __new_row.PATIENT_FIRST_NAME = __patient.FirstName;
                                    __new_row.PATIENT_LAST_NAME = __patient.LastName;
                                    __new_row.PATIENT_MOTHER_LAST_NAME = "";
                                    __new_row.PATIENT_ADDRESS = __patient.UsePatientInfo ? __patient.Address.Address1 + __patient.Address.Address2 : __patient.Facility.Address.Address1 + __patient.Facility.Address.Address2;
                                    __new_row.PATIENT_CITY = __patient.UsePatientInfo ? __patient.Address.City : __patient.Facility.Address.City;

                                    if (__patient.UsePatientInfo && __patient.Address.State == null ||
                                        !__patient.UsePatientInfo && __patient.Facility.Address.State == null)
                                    {
                                        throw new ArgumentException("NULL State Value");
                                    }

                                    __new_row.PATIENT_STATE = __state[__patient.UsePatientInfo ? (int)__patient.Address.State : (int)__patient.Facility.Address.State];
                                    __new_row.PATIENT_ZIP_CODE = __patient.UsePatientInfo ? __patient.Address.PostalCode : __patient.Facility.Address.PostalCode;

                                    __new_row.PATIENT_COUNTRY = "";
                                    __new_row.PATIENT_BIN_NUMBER = "";
                                    __new_row.PATIENT_PHONE_NUMBER = !string.IsNullOrEmpty(__patient.PrimaryPhone) ? __patient.PrimaryPhone : "No Phone";

                                    __new_row.PATIENT_BIRTH_DATE = string.Format("{0:yyyyMMdd}", __patient.DateOfBirth);
                                    __new_row.PHYSICIAN_NAME = string.Format(" {0} {1} {2}", __rx.Prescriber.FirstName, __rx.Prescriber.MiddleInitial, __rx.Prescriber.LastName);
                                    __new_row.PHYSICIAN_LICENCE = __rx.Prescriber.Dea;

                                    __new_row.REFILL_QUANTITY = __rx.Refills.ToString();
                                    __new_row.FIRST_REFILL_DATE = "";
                                    __new_row.LAST_REFILL_DATE = "";
                                    __new_row.COST = "";

                                    
                                    // TODO - Need to retain the newline
                                    if (__rx.CardSig.Contains("\r\n"))
                                    {
                                        __rx.CardSig = __rx.CardSig.Remove(__rx.CardSig.IndexOf("\r\n"),2);
                                    }
                                    

                                    __new_row.PRESCRIPTION_INSTRUCTION = __rx.CardSig;

                                    __new_row.PHARMACY_ACCREDITATION_NUMBER = __rx.Store.Dea;
                                }
                            }

                            __new_row.PATIENT_ID = __full_card.PatientId.ToString();
                            __new_row.PATIENT_FULL_NAME = __full_card.CupName;
                            __new_row.PATIENT_LANGUAGE = "";
                            __new_row.PERIOD_NAME = "";
                            __new_row.PERIOD_BEGINNING_TIME = "";   // string.Format("{0:yyyyMMdd}", DateTime.Now);
                            __new_row.PERIOD_ENDING_TIME = "";      // string.Format("{0:hh:mm}", DateTime.Now);
                            __new_row.PERIOD_ORDER = "";
                            __new_row.IS_HOUR_DRIVEN = "";

                            __new_row.INSTITUTION_NAME = !string.IsNullOrEmpty(__full_card.FacilityName) ? __full_card.FacilityName : "Independent";
                            __new_row.INSTITUTION_UNIT_NAME = "";
                            __new_row.INSTITUTION_FLOOR_LEVEL = "";
                            __new_row.INSTITUTION_ROOM_NUMBER = !string.IsNullOrEmpty(__full_card.Room) ? __full_card.Room : string.Empty;
                            __new_row.INSTITUTION_BED_NUMBER = "";

                            __new_row.PHARMACIST_NAME = "";


                            if (string.IsNullOrEmpty(__new_row.PRESCRIPTION_COMMENT))
                            {
                                __new_row.PRESCRIPTION_COMMENT = "";
                            }

                            __new_row.REORDER_NUMBER = "";
                            __new_row.INSTRUCTION_REASON = "";
                            __new_row.GROUP_TITLE = "";
                            __new_row.CARD_NOTE_01 = "";
                            __new_row.CARD_NOTE_02 = "";
                            __new_row.CELL_NOTE = "";

                            __new_row.ORDER_ID = "";
                            
                            __new_row.CYCLE_BASE_DATE = string.Format("{0:yyyyMMdd}", __base_date);

                            //__new_row.CYCLE_LENGTH = __cycle_length.ToString();

                            __new_row.CYCLE_LENGTH = ((int)__patient.CardDays).ToString();
                            
                            __new_row.CYCLE_FIRST_DAY_FIXED = "";

                            __new_row.ONE_MAR_DOSE_ID = "";
                            __new_row.ONE_MAR_WEB_SITE = "";

                            __table_rows.Add(__new_row);

                        }
                    }
                }
               
                __write_to_file();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public SynMedTable(string __patient_last_name, string __patient_first_name, string __patient_middle_initial, DateTime __cycle_start_date, int __cycle_length, string __path = null)
        {
            this.__patient_name = string.Format("{0} {1} {2}", __patient_last_name, __patient_first_name, __patient_middle_initial);
            this.__patient_first_name = __patient_first_name;
            this.__patient_last_name = __patient_last_name;
            this.__patient_middle_initial = __patient_middle_initial;
            this.__cycle_start_date = __cycle_start_date;
            this.__cycle_length = __cycle_length;

            try
            {
                if (__path == null)
                {
                    __path = @"C:\motNext\SynmedFiles";
                }

                __table_rows = new List<SynMedRow>();

                // Collect the CSV header
                var __csv_header = string.Empty;
                Type __SynMedFieldNames = typeof(SynMedRow);
                PropertyInfo[] __fieldnames = __SynMedFieldNames.GetProperties();

                for (int i = 0; i < __fieldnames.Length; i++)
                {
                    __csv_header += __fieldnames[i].Name.ToString();
                    if (i < __fieldnames.Length - 1)
                    {
                        __csv_header += "\t";
                    }
                }

                if (!Directory.Exists(__path))
                {
                    Directory.CreateDirectory(__path);
                }

                __filename = string.Format(@"{0}\{1:yyyyMMdd} - {2} - {3}.csv", __path, DateTime.Today, __patient_name, Path.GetRandomFileName());

                char[] __junk = { '<', '>' };

                while (__filename?.IndexOfAny(__junk) > -1)
                {
                    __filename = __filename.Remove(__filename.IndexOfAny(__junk), 1);
                }

                // Create a new file
                using (var __file = new FileStream(__filename, FileMode.Create))
                {
                    using (StreamWriter __sw = new StreamWriter(__file))
                    {
                        __sw.WriteLine(__csv_header);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:  path = {0}\nFilename = {1}\n{2}", __path, __filename, ex.StackTrace);
            }
        }
        public SynMedTable(motCommonLib.motSocket __socket, string __patient_name, DateTime __patient_dob, DateTime __cycle_start_date, int __cycle_length)
        {
            this.__patient_name = __patient_name;
            this.__patient_dob = __patient_dob;
            this.__cycle_start_date = __cycle_start_date;
            this.__cycle_length = __cycle_length;
            this.__socket = __socket;
        }
    }
}