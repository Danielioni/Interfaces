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

namespace motMatchineInterface
{
    /// <summary>
    /// Generalized classes for mapping motLegacy and motNext data into a common model 
    /// </summary>
    public class __motsynmed_address
    {
        public string __address1;
        public string __address2;
        public string __city;
        public int __state;
        public string __zip;
        public string __phone_number;
    }
    public class __motsynmed_prescriber
    {
        public Guid __g_id;
        public long __i_id;

        public string __last_name;
        public string __first_name;
        public string __middle_initial;
        public __motsynmed_address __main_address;
        public string __phone;
        public string __dea;

        public __motsynmed_prescriber()
        {
            __main_address = new __motsynmed_address();
        }
    }
    public class __motsynmed_dose
    {
        public Guid __g_id;
        public long __i_id;

        public string __dose_schedule_name;
        public string __dose_time; // HH:MM
        public double __qty;
        public string __special_instructions;
    }
    public class __motsynmed_rx
    {
        public Guid __g_presccriber_id;
        public long __i_prescriber_id;

        public long __rx_num;

        public Guid __g_rxid;
        public long __i_rxid;

        public DateTime __written_date;
        public DateTime __start_date;
        public DateTime __expire_date;
        public DateTime __dc_date;
        public int __refills;

        public string __NDC;
        public string __visual_description;
        public int __drug_schedule;
        public string __trade_name;
        public string __cup_name;
        public string __unit;
        public string __strength;
        public string __route;
        public string __dose_form;
        public string __consult_message;
        public string __generic_for;

        public RxType? __rx_type;
        public string __rx_dose_code;

        public string __sig;

        public List<__motsynmed_dose> __dose_schedule;

        public __motsynmed_rx()
        {
            __dose_schedule = new List<__motsynmed_dose>();
        }
    }
    public class __motsynmed_facility
    {
        public Guid __g_id;
        public long __i_id;

        public string __facility_name;
        public __motsynmed_address __main_address;
        public string __phone;

        public __motsynmed_facility()
        {
            __main_address = new __motsynmed_address();
        }
    }
    public class __motsynmed_patient
    {
        public Guid __g_patient_id;
        public long __i_patient_id;

        public string __last_name;
        public string __first_name;
        public string __middle_initial;
        public DateTime __dob;
        public DateTime __cycle_date;
        public string __phone;
        public List<__motsynmed_rx> __rxes;
        public __motsynmed_address __main_address;

        public string __room;
        public string __bed;

        public Guid __g_prescriber_id;
        public long __i_prescriber_id;

        public Guid __g_facility_id;
        public long __i_facility_id;

        public __motsynmed_patient()
        {
            __main_address = new __motsynmed_address();
            __rxes = new List<__motsynmed_rx>();
        }

    }
    public class __motsynmed_card : IDisposable
    {
        public __motsynmed_patient __pat;
        public __motsynmed_facility __fac;
        public __motsynmed_prescriber __doc;

        public int __card_sn;
        public int[] __bubble_num;
        public DateTime __card_duedate;
        public DateTime __card_enddate;
        public DateTime __card_dispensdate;
        public string __card_time;
        public int __card_type;


        public __motsynmed_card()
        {
            __pat = new __motsynmed_patient();
            __fac = new __motsynmed_facility();
            __doc = new __motsynmed_prescriber();

            __bubble_num = new int[31];
        }
        ~__motsynmed_card()
        {
            Dispose();
        }

        public void Dispose()
        {
            __pat.__rxes.Clear();
        }
    }
    /// <summary>
    /// Base interface for driving the Synmed robot
    /// </summary>
    public interface ISynMedRobotDriver
    {
        Task Login(string __uname, string __pw);
        Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __cycle_start_date, int __cycle_length);
        Task WriteCycle(DateTime __cycle_start);
        Task WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length);
    }
    /// <summary>
    /// Implementation for motLegacy drawing data from SQLAnywhere
    /// </summary>
    public class motLegacySynMed : ISynMedRobotDriver
    {
        motODBCServer __db;
        private string __file_name;
        private SynMedTable __table;


        // Transforms
        public DateTime __get_date_value(DataRow __row, string __index)
        {
            var __test_val = __row[__index];

            if (__test_val != DBNull.Value)
            {
                return DateTime.Parse(__row[__index].ToString());
            }

            return DateTime.Now;
        }
        public T __get_value<T>(DataRow __row, string __index)
        {
            var __test_val = __row[__index];

            try
            {
                if (__test_val != DBNull.Value)
                {
                    return (T)__row[__index];
                }
            }
            catch (InvalidCastException ex)
            {
                // No idea what to do here
                Console.Write("Invalid Cast Exception {0}", ex.StackTrace);
            }

            return default(T);
        }
        private void __convert_dose_schedule(string __ds_name, List<__motsynmed_dose> __tq, long __loc_code)
        {
            DataSet __tq_list = new DataSet();
            __motsynmed_dose __tmp_dose;

            __db.executeQuery(string.Format("SELECT * from ds_times_qtys where dscode = '{0}' AND linkcode = '{1}';", __ds_name, __loc_code), __tq_list);

            if (__tq_list.Tables["__table"].Rows.Count > 0)
            {
                foreach (DataRow __record in __tq_list.Tables["__table"].Rows)
                {
                    __tmp_dose = new __motsynmed_dose();
                    __tmp_dose.__dose_schedule_name = __ds_name;

                    __tmp_dose.__dose_time = __get_value<TimeSpan>(__record, "dosetime").ToString();

                    if (!string.IsNullOrEmpty(__tmp_dose.__dose_time))
                    {
                        __tmp_dose.__dose_time = __tmp_dose.__dose_time.Substring(0, 5);
                        __tmp_dose.__qty = (double)__get_value<decimal>(__record, "doseqty");
                        __tmp_dose.__special_instructions = __get_value<string>(__record, "textnotes");

                        if (__tmp_dose.__qty > 0)
                        {
                            __tq.Add(__tmp_dose);
                        }
                    }
                }
            }
        }
        private RxType? __convert_rx(int __legacy_rx)
        {
            switch (__legacy_rx)
            {
                case 0:
                    return RxType.Daily;

                case 2:
                    return RxType.Prn;

                case 5:
                    return RxType.DayOfWeek;

                case 7:
                    return RxType.DayOfMonth;

                case 8:
                    return RxType.MonthlyTitrating;

                case 9:
                    return RxType.WeeklyTitrating;

                case 13:
                    return RxType.Sequential;

                case 18:
                    return RxType.Alternating;

                default:
                    break;
            }

            return null;
        }


        public async Task Login(string __uname, string __pw)
        {
            try
            {
                __db = new motODBCServer("dsn=MOT8;UID=" + __uname + ";PWD=" + __pw);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length)
        { }
        public async Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __cycle_start_date, int __cycle_length)
        { }
        public async Task WriteCycle(DateTime __cycle_start)
        {
            List<__motsynmed_card> __card_list = new List<__motsynmed_card>();

            DataSet __data = new DataSet();
            DataSet __scrip = new DataSet();
            DataSet __pat = new DataSet();
            DataSet __loc = new DataSet();
            DataSet __doc = new DataSet();
            DataSet __patloc = new DataSet();

            Dictionary<string, int> __state_no = new Dictionary<string, int>()
            {
                {"",-1 }, { "AL", 0 }, {"AK", 1 }, {"AZ", 2 },
                {"AR", 3 }, {"CA", 4 }, {"CO", 5 },
                {"CT", 6 }, {"DE", 7 }, {"DC", 8 },
                {"FL", 9 }, {"GA", 10 }, {"HI", 11 },
                {"ID", 12 }, {"IL", 13 }, {"IN", 14 },
                {"IA", 15 }, {"KS", 16 }, {"KY", 17 },
                {"LA", 18 }, {"ME", 19 }, {"MD", 20 },
                {"MA", 21 }, {"MI", 22 }, {"MN", 23 },
                {"MS", 24 }, {"MO", 25 }, {"MT", 26 },
                {"NE", 27 }, {"NV", 28 }, {"NH", 29 },
                {"NJ", 30 }, {"NM", 31 }, {"NY", 32 },
                {"NC", 33 }, {"ND", 34 }, {"OH", 35 },
                {"OK", 36 }, {"OR", 37 }, {"PA", 38 },
                {"RI", 39 }, {"SC", 40 }, {"SD", 41 },
                {"TN", 42 }, {"TX", 43 }, {"UT", 44 },
                {"VT", 45 }, {"VA", 46 }, {"WA", 47 },
                {"WV", 48 }, {"WI", 49 }, {"WY", 50 }
            };

            __db.executeQuery(string.Format("SELECT * FROM cardserial LEFT OUTER JOIN cardserial_bubble ON cardserial.cardsn = cardserial_bubble.cardsn " +
                                                    "where cardserial.card_duedate >= date('{0:yyyy-MM-dd}');", __cycle_start), __data);

            if (__data.Tables["__table"].Rows.Count > 0)
            {
                int i = 0;

                foreach (DataRow __record in __data.Tables["__table"].Rows)
                {
                    i++;
                    __motsynmed_card __cl = new __motsynmed_card();

                    __cl.__card_sn = __get_value<int>(__record, "cardsn");
                   // __cl.__bubble_num[i++] = (int)__get_value<byte>(__record, "bubblenum");
                    __cl.__card_dispensdate = __get_date_value(__record, "dispense_date");
                    __cl.__card_duedate = __get_date_value(__record, "card_duedate");
                    __cl.__card_type = (int)__get_value<byte>(__record, "card_type");

                    // Populate the Patient Object
                    __db.executeQuery(string.Format("Select * from Patient LEFT OUTER JOIN Location ON Patient.LocCode = Location.LocCode where Patient.MotPatId = '{0}';", __record["patid"]), __patloc, "__patloc");

                    if (__patloc.Tables["__patloc"].Rows.Count > 0)
                    {
                        DataRow __row0 = __patloc.Tables["__patloc"].Rows[0];

                        __cl.__pat.__last_name = __get_value<string>(__row0, "LastName");
                        __cl.__pat.__first_name = __get_value<string>(__row0, "FirstName");
                        __cl.__pat.__middle_initial = __get_value<string>(__row0, "MiddleInitial");
                        __cl.__pat.__main_address.__address1 = __get_value<string>(__row0, "Address1");
                        __cl.__pat.__main_address.__address2 = __get_value<string>(__row0, "Address2");
                        __cl.__pat.__main_address.__city = __get_value<string>(__row0, "City");
                        __cl.__pat.__main_address.__state = (__get_value<string>(__row0, "State") != null) ? __state_no[__get_value<string>(__row0, "State")] : 0;
                        __cl.__pat.__main_address.__zip = __get_value<string>(__row0, "Zip");
                        __cl.__pat.__phone = __get_value<string>(__row0, "Phone");
                        __cl.__pat.__dob = __get_date_value(__row0, "DOB");
                        __cl.__pat.__room = __get_value<string>(__row0, "Room"); ;
                        __cl.__fac.__i_id = __get_value<int>(__row0, "LocCode");
                        __cl.__doc.__i_id = __get_value<int>(__row0, "PrimaryDoc");

                    }

                    __db.executeQuery(string.Format("Select * from Location where LocCode = '{0}';", __cl.__fac.__i_id), __loc, "__location");
                    if (__loc.Tables["__location"].Rows.Count > 0)
                    {
                        DataRow __row0 = __loc.Tables["__location"].Rows[0];

                        __cl.__fac.__facility_name = __get_value<string>(__row0, "locname");
                        __cl.__fac.__main_address.__address1 = __get_value<string>(__row0, "Address1");
                        __cl.__fac.__main_address.__address2 = __get_value<string>(__row0, "Address2");
                        __cl.__fac.__main_address.__city = __get_value<string>(__row0, "City");
                        __cl.__fac.__main_address.__state = (__get_value<string>(__row0, "State") != null) ? __state_no[__get_value<string>(__row0, "State")] : 0;
                        __cl.__fac.__main_address.__zip = __get_value<string>(__row0, "Zip");
                        __cl.__fac.__phone = __get_value<string>(__row0, "Phone");
                    }

                    __db.executeQuery(string.Format("Select * from Prescriber where DocCode = '{0}';", __cl.__doc.__i_id), __doc, "__doctor");
                    if (__doc.Tables["__doctor"].Rows.Count > 0)
                    {
                        DataRow __row0 = __doc.Tables["__doctor"].Rows[0];

                        __cl.__doc.__last_name = __get_value<string>(__row0, "LastName");
                        __cl.__doc.__first_name = __get_value<string>(__row0, "FirstName");
                        __cl.__doc.__middle_initial = __get_value<string>(__row0, "MiddleInitial");
                        __cl.__doc.__main_address.__address1 = __get_value<string>(__row0, "Address1");
                        __cl.__doc.__main_address.__address2 = __get_value<string>(__row0, "Address2");
                        __cl.__doc.__main_address.__city = __get_value<string>(__row0, "City");
                        __cl.__doc.__main_address.__state = (__get_value<string>(__row0, "State") != null) ? __state_no[__get_value<string>(__row0, "State")] : 0;
                        __cl.__doc.__main_address.__zip = __get_value<string>(__row0, "Zip");
                        __cl.__doc.__dea = __get_value<string>(__row0, "DEA");
                        __cl.__doc.__phone = __get_value<string>(__row0, "Phone");
                    }

                    if (__get_value<decimal>(__record, "rxnum") > 0)
                    {
                        __db.executeQuery(string.Format("select * from rx LEFT OUTER JOIN drugs ON rx.drugs_seqno = drugs.Seq_no WHERE motrxnum = '{0}'", __record["rxnum"]), __scrip, "__rxes");

                        if (__scrip.Tables["__rxes"].Rows.Count > 0)
                        {
                            foreach (DataRow __rec in __scrip.Tables["__rxes"].Rows)
                            {
                                // Make sure the scrip isn't DC'd
                                if (!string.IsNullOrEmpty(__rec["discontinue_date"]?.ToString()))
                                {
                                    continue;
                                }

                                __motsynmed_rx __tmp_rx = new __motsynmed_rx();

                                __tmp_rx.__i_rxid = (long)__get_value<decimal>(__rec, "rxsys_rxnum");

                                __tmp_rx.__start_date = __get_date_value(__rec, "rxstartdate");
                                __tmp_rx.__expire_date = __get_date_value(__rec, "rxstopdate");
                                __tmp_rx.__written_date = __get_date_value(__rec, "written_date");

                                var __tmp_dose = new __motsynmed_dose();

                                __tmp_dose.__qty = (double)__get_value<decimal>(__rec, "qty_written");
                                __tmp_dose.__dose_schedule_name = __get_value<string>(__rec, "dscode");

                                __convert_dose_schedule(__tmp_dose.__dose_schedule_name, __tmp_rx.__dose_schedule, __cl.__fac.__i_id);
                                __tmp_rx.__rx_type = __convert_rx(__get_value<byte>(__rec, "RxType"));

                                __tmp_rx.__refills = __get_value<byte>(__rec, "refills");
                                __tmp_rx.__sig = __get_value<string>(__rec, "sig2print");
                                __tmp_rx.__NDC = __get_value<string>(__rec, "NDCNum");
                                __tmp_rx.__visual_description = __get_value<string>(__rec, "Visual_Descript");
                                __tmp_rx.__drug_schedule = __get_value<byte>(__rec, "Drug_Sched");
                                __tmp_rx.__trade_name = __get_value<string>(__rec, "Tradename");
                                __tmp_rx.__cup_name = __get_value<string>(__rec, "Short_Name");
                                __tmp_rx.__unit = __get_value<string>(__rec, "Unit");
                                __tmp_rx.__strength = __get_value<string>(__rec, "Strength");
                                __tmp_rx.__route = __get_value<string>(__rec, "Route");
                                __tmp_rx.__dose_form = __get_value<string>(__rec, "Dose_Form");
                                __tmp_rx.__consult_message += string.Format(" \n{0}", __get_value<string>(__rec, "consult_msg"));
                                __tmp_rx.__generic_for = __get_value<string>(__rec, "GenericFor");

                                __cl.__doc.__i_id = __get_value<int>(__rec, "doccode");

                                __tmp_rx.__dose_schedule.Add(__tmp_dose);
                                __cl.__pat.__rxes.Add(__tmp_rx);

                            }
                        }
                    }

                    __card_list.Add(__cl);

                    Console.WriteLine("Added record for - {0} at {1}", __cl.__pat.__last_name, __cl.__fac.__facility_name);
                }

                Console.WriteLine("Done, added {0} records", i);
            }
        }


        public motLegacySynMed(string __path)
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
    }
    /// <summary>
    /// Implmentation for motNext drawing data from motNext SDK
    /// </summary>
    public class motNextSynMed : ISynMedRobotDriver
    {
        public static IContainer container;
        private SynMedTable __table;
        private IEnumerable<Patient> __patients;
        ICollection<Rx> __Rxs;
        Logger __logger;

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
        public motNextSynMed(string __pathname = null)
        {
            try
            {
                __logger = LogManager.GetLogger("mot machineInterface");

                Simple.OData.Client.V4Adapter.Reference();

                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterModule<FrameworkModule>();
                containerBuilder.RegisterModule<SdkModule>();
                container = containerBuilder.Build();


                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                authService = container.Resolve<IAuthenticationService>();

                if (__pathname != null)
                {
                    setup(__pathname);
                }

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
                    var __due_date = new DateTimeOffset((DateTime)__cycle_start);

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
                                if (__patient.DateOfBirth == null)
                                {
                                    __patient.DateOfBirth = DateTime.Today;  // Lots of bad data in lagacy import
                                }

                                TimeSpan __ts = __patient.CurrentCycleEndDate - __patient.CurrentCycleStartDate;
                                await WritePatient(__patient.LastName, __patient.FirstName, __patient.MiddleInitial, __cycle_start, __ts.Days + 1);

                                Console.WriteLine("Wrote: {0}, {1} {2} Rxcount: {3}", __patient.LastName, __patient.FirstName, __patient.MiddleInitial, __patient.Rxes.Count);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __cycle_start_date, int __cycle_length)
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

                        __table = new SynMedTable(__last_name, __first_name, __middle_initial, __cycle_start_date, __cycle_length);
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

                        __table = new SynMedTable(__last_name, __first_name, __middle_initial, __cycle_start_date, __cycle_length);
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
        public async Task BuildPatientByFacility(string __facility)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var query1 = scope.Resolve<IEntityQuery<Patient>>();


                var patients = await query1.QueryAsync(
                                        new QueryParameters<Patient>(__patient => __patient.Status == Status.Active && __patient.Facility.Name == __facility,
                                                                        p => p.Address,
                                                                        p => p.Rxes,
                                                                        p => p.Facility,
                                                                        p => p.Facility.Address,
                                                                        p => p.PatientPrescribers,
                                                                        p => p.Phones)
                                                        );


            }
        }
        public async Task BuildPatientList(int n)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var query1 = scope.Resolve<IEntityQuery<Patient>>();


                var patients = await query1.QueryAsync(
                                        new QueryParameters<Patient>(__patient => __patient.Status == Status.Active,
                                                                        p => p.Address,
                                                                        p => p.Rxes,
                                                                        p => p.Facility,
                                                                        p => p.PatientPrescribers,
                                                                        p => p.Phones).Top(n)
                                                        );


            }
        }
        public async Task BuildFacilityList(int n)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var query1 = scope.Resolve<IEntityQuery<Facility>>();


                var patients = await query1.QueryAsync(
                                        new QueryParameters<Facility>(__facility => __facility.StoreId != null,
                                                                        p => p.Address,
                                                                        p => p.FacilityContacts,
                                                                        p => p.Phones).Top(n)
                                                        );


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

                __csv_row = __csv_row.Substring(0, __csv_row.Length - 1);

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
        { }

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
            __new_row.DRUG_DESCRIPTION = !string.IsNullOrEmpty(__rx.Drug.VisualDescription) ? __rx.Drug.VisualDescription : "UNKOWN";
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


        public async void WriteRxCollection(IEnumerable<Rx> __rxes, string __file_name, Patient __patient = null)
        {
            DateTime __base_date = __cycle_start_date;
            __filename = __file_name;
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

                    if (__rx.StartDate <= DateTime.Today && __rx.StopDate > DateTime.Today)
                    {
                        /*
                        if (!__rx.RxDosageRegimen.IsCycleType || __rx.IsIsolate)
                        {
                            using (var scope = motNextSynMed.container)
                            {
                                //CardSettings __config = new CardSettings();
                                //__config.DueDate = __rx.Patient.DueDate;
                                //__config.CycleEndDate = __rx.Patient.CycleEndDate;
                                //__config.PopulateDate = DateTime.Today;
                                //__config.CardTimesFirstDose = 480;

                                //var __card = scope.Resolve<IPopulateCardsCommand>();
                                //var __sns = scope.Resolve<IManageCardsCommand>();

                                //IEnumerable<Card> __cards = await __card.PopulateCardForRxes(__rxes, __config);

                                //IEnumerable<Card> __card_sn = await __card.PopulateCardsForRx(__rx.Patient.Id, __rx);

                                //IEnumerable<Guid> __batch = new List<Guid>();
                                //__batch.ToList().Add(__card_sn.First().BatchId);

                                //IEnumerable<KeyValuePair<Guid, int>> __serial_numbers = await __sns.SetCardsSerialNo(__batch);
                            }
                        }
                        */

                        if (__rx.RxDosageRegimen.RxType == RxType.Prn)
                        {
                            var __prn_regimen = __rx.RxDosageRegimen as RxPrnRegimen;
                            IOrderedEnumerable<DoseScheduleItem> __dose_schedule_items;
                            __dose_schedule_items = __prn_regimen.GetScheduleItems();

                            if (__dose_schedule_items != null)
                            {
                                foreach (var __dose_item in __dose_schedule_items)
                                {
                                    __new_row = new SynMedRow();
                                    __new_row.GROUP_TITLE = __rx.RxSystemId;
                                    __new_row.PRESCRIPTION_COMMENT = "PRN Prescription";
                                    __new_row.PATIENT_WITH_PRN = "Y";
                                    __new_row.QTY_PER_ADMINISTRATION = __dose_item.Dose.ToString();
                                    __new_row.DRUG_QUANTITY = (__dose_item.Dose * __cycle_length).ToString();
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
                                    for (double __index = 0; __index < __cycle_length; __index++)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.PRESCRIPTION_COMMENT = "Daily Prescription";
                                        __current_date = (DateTime)__base_date.AddDays(__index);
                                        fillRow(__new_row, __rx, __base_date, __current_date, __dose);  // Any Daily Schedule (1 line per dsy)
                                    }

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
                                                __new_row.GROUP_TITLE = __rx.RxSystemId;
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
                                    /*  Not Supported By SynMed
                                    var __sequential_regimen = __rx.RxDosageRegimen as RxSequentialRegimen;

                                    foreach (var __sequential_dose in __sequential_regimen.DoseSchedule.DoseScheduleItems)
                                    {
                                        __new_row = new SynMedRow();
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
                }

                __write_to_file();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public SynMedTable(string __patient_last_name, string __patient_first_name, string __patient_middle_initial, DateTime __cycle_start_date, int __cycle_length)
        {
            this.__patient_name = string.Format("{0} {1} {2}", __patient_last_name, __patient_first_name, __patient_middle_initial);
            this.__patient_first_name = __patient_first_name;
            this.__patient_last_name = __patient_last_name;
            this.__patient_middle_initial = __patient_middle_initial;
            //this.__patient_dob = __patient_dob;
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