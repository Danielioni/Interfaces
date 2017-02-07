using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;


using Autofac;
using Mot.Client.Sdk;
using Mot.Shared.Framework;
using Mot.Shared.Model.Patients;
using Mot.Shared.Model.Rxes;
using Mot.Shared.Model.Rxes.RxRegimens;

//using motCommonLib;

/// <summary>
/// The Primary SynMed interface
///     
///     It supports both Legacy and Next with each filling the appropriate database interfaces and sharing a common output class
///     
/// </summary>

namespace motOutboundLib
{
    public interface ISynMedRobotDriver
    {
        Task Login(string __uname, string __pw);
        Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __patient_dob, DateTime __cycle_start_date, int __cycle_length);
        Task WriteCycle(DateTime __cycle_start);
        Task WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length);
    }

   public class motLegacySynMed : ISynMedRobotDriver
    {
        public async Task Login(string __uname, string __pw)
        { }
        public async Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __patient_dob, DateTime __cycle_start_date, int __cycle_length)
        { }
        public async Task WriteCycle(DateTime __cycle_start)
        { }
        public async Task WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length)
        { }
    }
    public class motNextSynMed : ISynMedRobotDriver
    {
        public static IContainer container;
        private SynMedTable __table;
        private IEnumerable<Patient> __patients;
        //public Patient __patient;
        ICollection<Rx> __Rxs;

        private static IAuthenticationService authService;
        private string __username;
        private string __password;
        private bool __logged_in = false;
        private string __file_name;
        private void setup(string __path)
        {
            try
            {
                // Collect the CSV header
                var __csv_header = string.Empty;
                Type __SynMedFieldNames = typeof(SynMedRow);
                PropertyInfo[] __fieldnames = __SynMedFieldNames.GetProperties();

                for (int i = 0; i < __fieldnames.Length; i++)
                {
                    __csv_header += __fieldnames[i].Name.ToString();
                    if (i < __fieldnames.Length - 1)
                    {
                        __csv_header += ";";
                    }
                }

                if (!Directory.Exists(__path))
                {
                    Directory.CreateDirectory(__path);
                }

                __file_name = string.Format(@"{0}\{1:yyyyMMdd-hhmmss}-{2}.csv", __path, DateTime.Now, Path.GetRandomFileName());

                // Create a new file
                using (var __file = new FileStream(__file_name, FileMode.Create))
                {
                    using (StreamWriter __sw = new StreamWriter(__file))
                    {
                        __sw.WriteLine(__csv_header);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public motNextSynMed(string __pathname)
        {
            try
            {
                Simple.OData.Client.V4Adapter.Reference();

                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterModule<FrameworkModule>();
                containerBuilder.RegisterModule<SdkModule>();
                container = containerBuilder.Build();


                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                authService = container.Resolve<IAuthenticationService>();

                setup(__pathname);

            }
            catch
            { throw; }
        }
        public async Task Login(string __uname, string __pw)
        {
            __username = __uname;
            __password = __pw;

            try
            {
                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                await authService.LoginAsync(__username, __password);

                __logged_in = true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task WriteCycle(DateTime __cycle_start)
        {
            if (!__logged_in)
            {
                throw new Exception("Not logged in");
            }

            try
            {
                using (var scope = container.BeginLifetimeScope())
                {

                    var query = scope.Resolve<IEntityQuery<Patient>>();
                    var __due_date  = new DateTimeOffset(__cycle_start);

                    var __patients = await query.QueryAsync(new QueryParameters<Patient>(pt => pt.DueDate == __due_date && !pt.ChartOnly && !pt.IsHidden,
                                                                                         p => p.Rxes,
                                                                                         p => p.Phones,
                                                                                         p => p.Facility,
                                                                                         p => p.PatientPrescribers
                                                                                         ));

                    foreach (var __patient in __patients)
                    {
                        try
                        {
                            if (__patient.Rxes.Count > 0)
                            {
                                TimeSpan __ts = __patient.CurrentCycleEndDate - __patient.CurrentCycleStartDate;

                                //await Write(__patient.LastName, __patient.FirstName, __patient.MiddleInitial, (DateTime)__patient.DateOfBirth, __cycle_start, __ts.Days + 1);

                                if (__patient.DateOfBirth != null)
                                {
                                    __table = new SynMedTable(__patient.LastName, __patient.FirstName, __patient.MiddleInitial, (DateTime)__patient.DateOfBirth, __cycle_start, __ts.Days + 1);
                                    __table.WriteRxCollection(__patient.Rxes, __file_name, __patient);

                                    Console.WriteLine("Wrote: {0}, {1} {2} Rxcount: {3}", __patient.LastName, __patient.FirstName, __patient.MiddleInitial, __patient.Rxes.Count);
                                }
                            }
                        }
                        catch
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __patient_dob, DateTime __cycle_start_date, int __cycle_length)
        {
            if (!__logged_in)
            {
                throw new Exception("Not logged in");
            }

            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Rx>>();

                    if (!string.IsNullOrEmpty(__middle_initial))
                    {

                        var rxes = await query1.QueryAsync(
                            new QueryParameters<Rx>(rx => rx.Status != RxStatus.Discountinue && rx.Patient.LastName == __last_name && rx.Patient.FirstName == __first_name && rx.Patient.MiddleInitial == __middle_initial,
                                                    r => r.RxDosageRegimen.DoseSchedule,
                                                    r => r.RxDosageRegimen.DoseSchedule.DoseScheduleItems,
                                                    r => r.Drug,
                                                    r => r.Patient.Address,
                                                    r => r.Patient.Facility.Address,
                                                    r => r.Patient.Phones,
                                                    r => r.Prescriber,
                                                    r => r.Store,
                                                    r => r.Patient.Facility)
                                        );

                        __table = new SynMedTable(__last_name, __first_name, __middle_initial, __patient_dob, __cycle_start_date, __cycle_length);
                        __table.WriteRxCollection(rxes, __file_name);
                    }
                    else
                    {

                        var rxes = await query1.QueryAsync(
                            new QueryParameters<Rx>(rx => rx.Status != RxStatus.Discountinue && rx.Patient.LastName == __last_name && rx.Patient.FirstName == __first_name,
                                                    r => r.RxDosageRegimen.DoseSchedule,
                                                    r => r.RxDosageRegimen.DoseSchedule.DoseScheduleItems,
                                                    r => r.Drug,
                                                    r => r.Patient.Address,
                                                    r => r.Patient.Facility.Address,
                                                    r => r.Patient.Phones,
                                                    r => r.Prescriber,
                                                    r => r.Store,
                                                    r => r.Patient.Facility)
                                        );

                        __table = new SynMedTable(__last_name, __first_name, __middle_initial, __patient_dob, __cycle_start_date, __cycle_length);
                        __table.WriteRxCollection(rxes, __file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public async Task WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length)
        {
            if (!__logged_in)
            {
                throw new Exception("Not logged in");
            }
        }

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
    class SynMedRow
    {
        private List<SynMedField> __field_list;

        public SynMedRow()
        {
            __field_list = new List<SynMedField>();

            Type __SynMedFieldData = typeof(SynMedRow);
            PropertyInfo[] __field_data = __SynMedFieldData.GetProperties();

            //foreach (var __f in __field_data)
            //{
            //    __f.SetValue(this,"");
            //}
        }
        public string RECORD_TYPE
        {
            get { return (string)__field_list.Find(f => f?.__name == "RECORD_TYPE")?.__data; }
            set { __field_list.Add(new SynMedField("RECORD_TYPE", value, false, 5)); }
        }
        public string ADMINISTRATION_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "ADMINISTRATION_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("ADMINISTRATION_DATE", value, true, 10)); }
        }
        public string ADMINISTRATION_TIME
        {
            get { return (string)__field_list.Find(f => f?.__name == "ADMINISTRATION_TIME")?.__data; }
            set { __field_list.Add(new SynMedField("ADMINISTRATION_TIME", value, true, 11)); }
        }
        public string LOCAL_DRUG_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "LOCAL_DRUG_ID")?.__data; }
            set { __field_list.Add(new SynMedField("LOCAL_DRUG_ID", value, true, 15)); }
        }
        public string DRUG_QUANTITY
        {
            get { return (string)__field_list.Find(f => f?.__name == "DRUG_QUANTITY")?.__data; }
            set { __field_list.Add(new SynMedField("DRUG_QUANTITY", value, true)); }
        }
        public string DRUG_DESCRIPTION
        {
            get { return (string)__field_list.Find(f => f?.__name == "DRUG_DESCRIPTION")?.__data; }
            set { __field_list.Add(new SynMedField("DRUG_DESCRIPTION", value, true, 75)); }
        }
        public string DISPLAY_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "DISPLAY_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("DISPLAY_NAME", value, false, 75)); }
        }
        public string EXTERNAL_DRUG_FLAG
        {
            get { return (string)__field_list.Find(f => f?.__name == "EXTERNAL_DRUG_FLAG")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("EXTERNAL_DRUG_FLAG", value, false, 1)); }
        }
        public string NOT_IN_BLISTER
        {
            get { return (string)__field_list.Find(f => f?.__name == "NOT_IN_BLISTER")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("NOT_IN_BLISTER", value, false, 1)); }
        }
        public string PRESCRIPTION_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PRESCRIPTION_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("PRESCRIPTION_NUMBER", value, true, 15)); }
        }
        public string PATIENT_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_ID")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_ID", value, true, 10)); }
        }
        public string PATIENT_FULL_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_FULL_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_FULL_NAME", value, true, 50)); }
        }
        public string PATIENT_LANGUAGE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_LANGUAGE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_LANGUAGE", value, false, 3)); }
        }
        public string PATIENT_FIRST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_FIRST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_FIRST_NAME", value, false, 25)); }
        }
        public string PATIENT_LAST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_LAST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_LAST_NAME", value, false, 25)); }
        }
        public string PATIENT_MOTHER_LAST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_MOTHER_LAST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_MOTHER_LAST_NAME", value, false, 20)); }
        }
        public string PATIENT_ADDRESS
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_ADDRESS")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_ADDRESS", value, false, 50)); }
        }
        public string PATIENT_CITY
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_CITY")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_CITY", value, false, 50)); }
        }
        public string PATIENT_STATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_STATE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_STATE", value, false, 50)); }
        }
        public string PATIENT_ZIP_CODE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_ZIP_CODE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_ZIP_CODE", value, false, 25)); }
        }
        public string PATIENT_COUNTRY
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_COUNTRY")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_COUNTRY", value, false, 50)); }
        }
        public string PATIENT_BIN_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_BIN_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_BIN_NUMBER", value, false, 10)); }
        }
        public string PATIENT_PHONE_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_PHONE_NUMBER")?.__data; }
            set { __field_list?.Add(new SynMedField("PATIENT_PHONE_NUMBER", value, false, 25)); }
        }
        public string PATIENT_BIRTH_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_BIRTH_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("PATIENT_BIRTH_DATE", value, false, 25)); }
        }
        public string PATIENT_WITH_PRN
        {
            get { return (string)__field_list.Find(f => f?.__name == "PATIENT_WITH_PRN")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("PATIENT_WITH_PRN", value, false, 1)); }
        }
        public string QTY_PER_ADMINISTRATION
        {
            get { return (string)__field_list.Find(f => f?.__name == "QTY_PER_ADMINISTRATION")?.__data; }
            set { __field_list.Add(new SynMedField("QTY_PER_ADMINISTRATION", value, false)); }
        }
        public string ADMINISTRATION_PER_DAY
        {
            get { return (string)__field_list.Find(f => f?.__name == "ADMINISTRATION_PER_DAY")?.__data; }
            set { __field_list.Add(new SynMedField("ADMINISTRATION_PER_DAY", value, false)); }
        }
        public string DAY_LAPSE
        {
            get { return (string)__field_list.Find(f => f?.__name == "DAY_LAPSE")?.__data; }
            set { __field_list.Add(new SynMedField("DAY_LAPSE", value, false)); }
        }
        public string PERIOD_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_NAME", value, false, 8)); }
        }
        public string PERIOD_BEGINNING_TIME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_BEGINNING_TIME")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_BEGINNING_TIME", value, false)); }
        }
        public string PERIOD_ENDING_TIME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_ENDING_TIME")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_ENDING_TIME", value, false)); }
        }
        public string PERIOD_ORDER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PERIOD_ORDER")?.__data; }
            set { __field_list.Add(new SynMedField("PERIOD_ORDER", value, false)); }
        }
        public string IS_HOUR_DRIVEN
        {
            get { return (string)__field_list.Find(f => f?.__name == "IS_HOUR_DRIVEN")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("IS_HOUR_DRIVEN", value, false, 1)); }
        }
        public string INSTITUTION_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_NAME", value, false, 30)); }
        }
        public string INSTITUTION_UNIT_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_UNIT_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_UNIT_NAME", value, false, 25)); }
        }
        public string INSTITUTION_FLOOR_LEVEL
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_FLOOR_LEVEL")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_FLOOR_LEVEL", value, false, 15)); }
        }
        public string INSTITUTION_ROOM_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_ROOM_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_ROOM_NUMBER", value, false, 15)); }
        }
        public string INSTITUTION_BED_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTITUTION_BED_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("INSTITUTION_BED_NUMBER", value, false, 15)); }
        }
        public string PHYSICIAN_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHYSICIAN_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PHYSICIAN_NAME", value, false, 25)); }
        }
        public string PHYSICIAN_LICENCE
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHYSICIAN_LICENCE")?.__data; }
            set { __field_list.Add(new SynMedField("PHYSICIAN_LICENCE", value, false, 15)); }
        }
        public string PHARMACIST_NAME
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHARMACIST_NAME")?.__data; }
            set { __field_list.Add(new SynMedField("PHARMACIST_NAME", value, false, 30)); }
        }
        public string REFILL_QUANTITY
        {
            get { return (string)__field_list.Find(f => f?.__name == "REFILL_QUANTITY")?.__data; }
            set { __field_list.Add(new SynMedField("REFILL_QUANTITY", value, false)); }
        }
        public string FIRST_REFILL_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "FIRST_REFILL_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("FIRST_REFILL_DATE", value, false)); }
        }
        public string LAST_REFILL_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "LAST_REFILL_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("LAST_REFILL_DATE", value, false)); }
        }
        public string COST
        {
            get { return (string)__field_list.Find(f => f?.__name == "COST")?.__data; }
            set { __field_list.Add(new SynMedField("COST", value, false)); }
        }
        public string PRESCRIPTION_INSTRUCTION
        {
            get { return (string)__field_list.Find(f => f?.__name == "PRESCRIPTION_INSTRUCTION")?.__data; }
            set { __field_list.Add(new SynMedField("PRESCRIPTION_INSTRUCTION", value, false, 90)); }
        }
        public string PRESCRIPTION_COMMENT
        {
            get { return (string)__field_list.Find(f => f?.__name == "PRESCRIPTION_COMMENT")?.__data; }
            set { __field_list.Add(new SynMedField("PRESCRIPTION_COMMENT", value, false, 75)); }
        }
        public string REORDER_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "REORDER_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("REORDER_NUMBER", value, false, 25)); }
        }
        public string INSTRUCTION_REASON
        {
            get { return (string)__field_list.Find(f => f?.__name == "INSTRUCTION_REASON")?.__data; }
            set { __field_list.Add(new SynMedField("INSTRUCTION_REASON", value, false, 35)); }
        }
        public string GROUP_TITLE
        {
            get { return (string)__field_list.Find(f => f?.__name == "GROUP_TITLE")?.__data; }
            set { __field_list.Add(new SynMedField("GROUP_TITLE", value, false, 50)); }
        }
        public string CARD_NOTE_01
        {
            get { return (string)__field_list.Find(f => f?.__name == "CARD_NOTE_01")?.__data; }
            set { __field_list.Add(new SynMedField("CARD_NOTE_01", value, false, 35)); }
        }
        public string CARD_NOTE_02
        {
            get { return (string)__field_list.Find(f => f?.__name == "CARD_NOTE_02")?.__data; }
            set { __field_list.Add(new SynMedField("CARD_NOTE_02", value, false, 35)); }
        }
        public string CELL_NOTE
        {
            get { return (string)__field_list.Find(f => f?.__name == "CELL_NOTE")?.__data; }
            set { __field_list.Add(new SynMedField("CELL_NOTE", value, false, 35)); }
        }
        public string PHARMACY_ACCREDITATION_NUMBER
        {
            get { return (string)__field_list.Find(f => f?.__name == "PHARMACY_ACCREDITATION_NUMBER")?.__data; }
            set { __field_list.Add(new SynMedField("PHARMACY_ACCREDITATION_NUMBER", value, false, 35)); }
        }
        public string ORDER_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "ORDER_ID")?.__data; }
            set { __field_list.Add(new SynMedField("ORDER_ID", value, false, 10)); }
        }
        public string CYCLE_BASE_DATE
        {
            get { return (string)__field_list.Find(f => f?.__name == "CYCLE_BASE_DATE")?.__data; }
            set { __field_list.Add(new SynMedField("CYCLE_BASE_DATE", value, false)); }
        }
        public string CYCLE_LENGTH
        {
            get { return (string)__field_list.Find(f => f?.__name == "CYCLE_LENGTH")?.__data; }
            set { __field_list.Add(new SynMedField("CYCLE_LENGTH", value, false)); }
        }
        public string CYCLE_FIRST_DAY_FIXED
        {
            get { return (string)__field_list.Find(f => f?.__name == "CYCLE_FIRST_DAY_FIXED")?.__data; }
            set { /*if (value == "Y" || value == "N")*/ __field_list.Add(new SynMedField("CYCLE_FIRST_DAY_FIXED", value, false, 1)); }
        }
        /*      public string PERIOD_NAME_01
                {
                    get { return (string)__field_list.Find(f => f?.__name == "PERIOD_NAME_01")?.__data; }
                    set { __field_list.Add(new SynMedField("PERIOD_NAME_01", value, false, 8)); }
                }
                public string PERIOD_NAME_02
                {
                    get { return (string)__field_list.Find(f => f?.__name == "PERIOD_NAME_02")?.__data; }
                    set { __field_list.Add(new SynMedField("PERIOD_NAME_02", value, false, 8)); }
                }
                public string PERIOD_NAME_03
                {
                    get { return (string)__field_list.Find(f => f?.__name == "PERIOD_NAME_01")?.__data; }
                    set { __field_list.Add(new SynMedField("PERIOD_NAME_01", value, false, 8)); }
                }
                public string PERIOD_NAME_04
                {
                    get { return (string)__field_list.Find(f => f?.__name == "PERIOD_NAME_04")?.__data; }
                    set { __field_list.Add(new SynMedField("PERIOD_NAME_04", value, false, 8)); }
                }
        */
        public string ONE_MAR_DOSE_ID
        {
            get { return (string)__field_list.Find(f => f?.__name == "ONE_MAR_DOSE_ID")?.__data; }
            set { __field_list.Add(new SynMedField("ONE_MAR_DOSE_ID", value, false, 2)); }
        }
        public string ONE_MAR_WEB_SITE
        {
            get { return (string)__field_list.Find(f => f?.__name == "ONE_MAR_WEB_SITE")?.__data; }
            set { __field_list.Add(new SynMedField("ONE_MAR_WEB_SITE", value, false, 2)); }
        }

        public void write(string __filename, FileMode __mode)
        {
            //foreach (SynMedField __field in __field_list)
            //{
            try
            {
                // Collect the CSV data row
                var __csv_row = string.Empty;
                Type __SynMedFieldData = typeof(SynMedRow);
                PropertyInfo[] __field_data = __SynMedFieldData.GetProperties();

                foreach (var __f in __field_data)
                {
                    __csv_row += __f.GetValue(this, null) + ";";
                }

                __csv_row = __csv_row.Substring(0, __csv_row.Length - 2);

                using (var __fs = new FileStream(__filename, FileMode.Append, FileAccess.Write))
                {
                    using (var __sw = new StreamWriter(__fs))
                    {
                        __sw.WriteLine(__csv_row);
                    }
                }
            }
            catch
            {
                throw;
            }
            //}

        }
    }
    class SynMedTable
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
            foreach (var __row in __table_rows)
            {
                __row.write(__filename, __mode);
            }
        }

        public void WriteByCycleDate(DateTime __cycle_start_date)
        {

        }

        public void WriteByPatient()
        { }

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
            __new_row.DRUG_DESCRIPTION = __rx.Drug.VisualDescription;
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
            __new_row.PATIENT_STATE = __state[__rx.Patient.UsePatientInfo ? (int)__rx.Patient.Address.State : (int)__rx.Patient.Facility.Address.State];
            __new_row.PATIENT_ZIP_CODE = __rx.Patient.UsePatientInfo ? __rx.Patient.Address.PostalCode : __rx.Patient.Facility.Address.PostalCode;
            __new_row.PATIENT_COUNTRY = "";
            __new_row.PATIENT_BIN_NUMBER = "";
            __new_row.PATIENT_PHONE_NUMBER = __rx.Patient.Phones?.FirstOrDefault().Number;
            __new_row.PATIENT_BIRTH_DATE = string.Format("{0:yyyyMMdd}", __rx.Patient.DateOfBirth);

            __new_row.PERIOD_NAME = "";
            __new_row.PERIOD_BEGINNING_TIME = "";   // string.Format("{0:yyyyMMdd}", DateTime.Now);
            __new_row.PERIOD_ENDING_TIME = "";      // string.Format("{0:hh:mm}", DateTime.Now);
            __new_row.PERIOD_ORDER = "";
            __new_row.IS_HOUR_DRIVEN = "";

            __new_row.INSTITUTION_NAME = __rx.Patient.Facility.Name;
            __new_row.INSTITUTION_UNIT_NAME = "";
            __new_row.INSTITUTION_FLOOR_LEVEL = "";
            __new_row.INSTITUTION_ROOM_NUMBER = __rx.Patient.Room;
            __new_row.INSTITUTION_BED_NUMBER = "";

            __new_row.PHYSICIAN_NAME = string.Format(" {0} {1} {2}", __rx.Prescriber.FirstName, __rx.Prescriber.MiddleInitial, __rx.Prescriber.LastName);
            __new_row.PHYSICIAN_LICENCE = __rx.Prescriber.Dea;

            __new_row.PHARMACIST_NAME = "";
            __new_row.REFILL_QUANTITY = __rx.Refills.ToString();
            __new_row.FIRST_REFILL_DATE = "";
            __new_row.LAST_REFILL_DATE = "";
            __new_row.COST = "";
            __new_row.PRESCRIPTION_INSTRUCTION = __rx.CardSig;

            if(string.IsNullOrEmpty(__new_row.PRESCRIPTION_COMMENT))
                __new_row.PRESCRIPTION_COMMENT = "";

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

        public async void WriteRxCollection(IEnumerable<Rx> __rxes, string __file_name, Patient __patient = null)
        {
            DateTime __base_date = __cycle_start_date;
            __filename = __file_name;
            //int __save_cycle_length = 0;
            SynMedRow __new_row;

            try
            {
                foreach (var __rx in __rxes)
                {
                    if (__patient != null)
                    {
                        __rx.Patient = __patient;
                    }

                    DateTime __start_date = __rx.Patient.DueDate;
                    DateTime __current_date = __start_date;

                    if (__rx.IsActive)
                    {
                        if(__rx.RxDosageRegimen.RxType == RxType.Prn)
                        {
                            var __prn_regimen = __rx.RxDosageRegimen as RxPrnRegimen;
                            IOrderedEnumerable<DoseScheduleItem> __dose_schedule_items;
                            __dose_schedule_items = __prn_regimen.GetScheduleItems();

                            if (__dose_schedule_items != null)
                            {
                                foreach (var __dose_item in __dose_schedule_items)
                                {
                                    __new_row = new SynMedRow();
                                    __new_row.PRESCRIPTION_COMMENT = "PRN Prescription";
                                    __new_row.PATIENT_WITH_PRN = "Y";
                                    __new_row.QTY_PER_ADMINISTRATION = __dose_item.Dose.ToString();
                                    __new_row.DRUG_QUANTITY = __rx.QuantityWritten.ToString();
                                    __new_row.ADMINISTRATION_PER_DAY = (__rx.QuantityWritten / __cycle_length).ToString();
                                    __new_row.ADMINISTRATION_TIME = string.Format("{0:00}:{1:00}", __dose_item.GetTimespan().Hours, __dose_item.GetTimespan().Minutes);
                                    __new_row.DAY_LAPSE = "1";

                                    fillRow(__new_row, __rx, __base_date, __current_date, __dose_item);
                                }
                            }

                            continue;
                        }

                        foreach (DoseScheduleItem __dose in __rx.RxDosageRegimen.DoseSchedule.DoseScheduleItems)
                        {
                            switch (__rx.RxDosageRegimen.RxType)
                            {
                                case RxType.Daily:
                                    __new_row = new SynMedRow();
                                    __new_row.PRESCRIPTION_COMMENT = "Daily Prescription";
                                    fillRow(__new_row, __rx, __base_date, __current_date, __dose);  // Any Daily Schedule

                                    break;

                                case RxType.Prn:                                   
                                    break;

                                case RxType.Alternating:
                                    var __alternating_regimen = __rx.RxDosageRegimen as RxAlternatingRegimen;

                                    for (int __day = 0; __day < __cycle_length; __day += __alternating_regimen.RepeatDays)
                                    {
                                        __current_date = __base_date.AddDays(__day);

                                        foreach (var __item in __alternating_regimen.DoseSchedule.DoseScheduleItems)
                                        {
                                            __new_row = new SynMedRow();
                                            __new_row.PRESCRIPTION_COMMENT = "Alternating Prescription - every (" + __alternating_regimen.RepeatDays + ") days";
                                            fillRow(__new_row, __rx, __base_date, __current_date, __item);
                                        }
                                    }

                                    break;

                                case RxType.MonthlyTitrating:
                                case RxType.WeeklyTitrating:
                                    var __titrating_regimen = __rx.RxDosageRegimen as IAlternatingItemsContainer;

                                    // Get the Alternating DoseRegamin
                                    using (var scope = motNextSynMed.container.BeginLifetimeScope())
                                    {
                                        var itemsQuery = scope.Resolve<IEntityQuery<RxAlternatingItem>>();

                                        if (__titrating_regimen != null && __titrating_regimen.AlternatingItems == null)
                                        {
                                            __titrating_regimen.AlternatingItems = (await itemsQuery.QueryAsync(new QueryParameters<RxAlternatingItem>(
                                                item => item.RxDosageRegimenId == __titrating_regimen.Id))).ToList();
                                        }
                                    }

                                    foreach (var __item in __titrating_regimen.AlternatingItems)
                                    {
                                        for (int __day = 0; __day < __cycle_length; __day++)
                                        {
                                            if (__item.Doses[__day] > 0)
                                            {
                                                __new_row = new SynMedRow();
                                                __new_row.DRUG_QUANTITY = __item.Doses[__day].ToString();
                                                __new_row.PRESCRIPTION_COMMENT = "Monthly or Weekly Titrating Prescription";
                                                fillRow(__new_row, __rx, __base_date, __current_date, __dose);
                                                __current_date = __base_date.AddDays(__day + 1);
                                            }
                                        }
                                    }

                                    break;

                                case RxType.DayOfMonth:
                                    var __day_of_month_regimen = __rx.RxDosageRegimen as RxDayOfMonthRegimen;

                                    for (var __day = 0; __day < __cycle_length; __day++)
                                    {
                                        __current_date = __base_date.AddDays(__day);

                                        foreach (int __specific_day in __day_of_month_regimen.DayOfMonth)
                                        {
                                            if (__current_date.Day == __specific_day)
                                            {
                                                foreach (var __item in __day_of_month_regimen.DoseSchedule.DoseScheduleItems)
                                                {
                                                    __new_row = new SynMedRow();
                                                    __new_row.PRESCRIPTION_COMMENT = "Day Of Month Prescription";
                                                    fillRow(__new_row, __rx, __base_date, __current_date, __item);
                                                }
                                            }
                                        }
                                    }

                                    break;

                                case RxType.DayOfWeek:
                                    var __day_of_week_regimen = __rx.RxDosageRegimen as RxDayOfWeekRegimen;

                                    for (int __day = 0; __day < __cycle_length; __day++)
                                    {
                                        __current_date = __base_date.AddDays(__day);

                                        foreach (DayOfWeek __specific_day in __day_of_week_regimen.DaysOfWeek)
                                        {
                                            if (__current_date.DayOfWeek == __specific_day)
                                            {
                                                foreach (var __item in __day_of_week_regimen.DoseSchedule.DoseScheduleItems)
                                                {
                                                    __new_row = new SynMedRow();
                                                    __new_row.PRESCRIPTION_COMMENT = "Day Of Week Prescription";
                                                    fillRow(__new_row, __rx, __base_date, __current_date, __item);
                                                }
                                            }
                                        }
                                    }

                                    break;


                                case RxType.Sequential:
                                    var __sequential_regimen = __rx.RxDosageRegimen as RxSequentialRegimen;

                                    foreach (var __sequential_dose in __sequential_regimen.DoseSchedule.DoseScheduleItems)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.PRESCRIPTION_COMMENT = "Sequential Prescription";
                                        fillRow(__new_row, __rx, __base_date, __current_date, __sequential_dose);
                                    }

                                    break;

                                default:
                                    break;
                            }
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

        /*
        private void setup()
        {
            try
            {
                // Collect the CSV header
                __csv_header = string.Empty;
                Type __SynMedFieldNames = typeof(SynMedRow);
                PropertyInfo[] __fieldnames = __SynMedFieldNames.GetProperties();

                for (int i = 0; i < __fieldnames.Length; i++)
                {
                    __csv_header += __fieldnames[i].Name.ToString();
                    if (i < __fieldnames.Length - 1)
                    {
                        __csv_header += ",";
                    }
                }

                using (__file = new FileStream(__filename, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter __sw = new StreamWriter(__file))
                    {
                        __sw.WriteLine(__csv_header);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        */

        public SynMedTable(string __patient_last_name, string __patient_first_name, string __patient_middle_initial, DateTime __patient_dob, DateTime __cycle_start_date, int __cycle_length)
        {
            this.__patient_name = string.Format("{0} {1} {2}", __patient_last_name, __patient_first_name, __patient_middle_initial);
            this.__patient_first_name = __patient_first_name;
            this.__patient_last_name = __patient_last_name;
            this.__patient_middle_initial = __patient_middle_initial;
            this.__patient_dob = __patient_dob;
            this.__cycle_start_date = __cycle_start_date;
            this.__cycle_length = __cycle_length;

            try
            {
                __table_rows = new List<SynMedRow>();

                /*
                if (!File.Exists(__filename))
                {
                    setup();
                }
                */
            }
            catch
            { throw; }
        }

        public SynMedTable(motCommonLib.motSocket __socket, string __patient_name, DateTime __patient_dob, DateTime __cycle_start_date, int __cycle_length)
        {
            this.__patient_name = __patient_name;
            this.__patient_dob = __patient_dob;
            this.__cycle_start_date = __cycle_start_date;
            this.__cycle_length = __cycle_length;
            this.__socket = __socket;

            /*
            if (!File.Exists(__filename))
            {
                setup();
            }
            */
        }
    }
}
