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
    /// 
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
    /// <summary>
    /// Titrating Rx Type - needs to offer varialble dosage over time and variable dos_schedules.
    /// For example:
    ///     2017-03-10  4 Prednisone 10mg @ 08:00
    ///     2017-03-11  3 Prednisone 10mg @ 08:00
    ///     2017-03-12  1 Prednisone 10mg @ 08:00
    ///     2017-03-13  1 Prednisone 10mg @ 08:00
    ///     2017-03-12  1 Prednisone 10mg @ 08:00    
    ///     
    ///     --or-- with a specified daily dose schedule, BID, QD, ...
    ///     
    ///     2017-03-10  4 Prednisone 5mg @ 08:00
    ///     2017-03-10  4 Prednisone 5mg @ 20:00
    ///     2017-03-11  3 Prednisone 5mg @ 08:00
    ///     2017-03-11  3 Prednisone 5mg @ 20:00
    ///     2017-03-12  2 Prednisone 5mg @ 08:00
    ///     2017-03-12  2 Prednisone 5mg @ 20:00
    ///     2017-03-13  1 Prednisone 5mg @ 08:00
    ///     2017-03-10  1 Prednisone 5mg @ 20:00
    ///     
    /// </summary>
    public class __motsynmed_titrating_schedule : IDisposable
    {
        public Guid __g_id;
        public decimal __i_id;

        public DateTime __dose_date;
        public List<__motsynmed_daily_dose> __dose_schedule;
        public __motsynmed_daily_dose __dose_item;

        public __motsynmed_titrating_schedule()
        {
            __dose_schedule = new List<__motsynmed_daily_dose>();
        }
        public void Dispose()
        {
            __dose_schedule.Clear();
        }
    }
    public class __motsynmed_alternating_schedule : IDisposable
    {
        public Guid __g_id;
        public decimal __i_id;

        public DateTime __dose_date;
        public int __alternating_days;
        public List<__motsynmed_daily_dose> __dose_schedule;

        public __motsynmed_alternating_schedule()
        {
            __dose_schedule = new List<__motsynmed_daily_dose>();
        }
        public void Dispose()
        {
            __dose_schedule.Clear();
        }
    }
    public class __motsynmed_daily_dose
    {
        public Guid __g_id;
        public decimal __i_id;

        public string __dose_schedule_name;
        public string __dose_time; // HH:MM
        public decimal __qty;
        public int __card_sn;
        public int __isolate;
        public string __special_instructions;

        public __motsynmed_daily_dose()
        {
            __card_sn = -1;
            __isolate = 0;
        }
    }
    public class __motsynmed_rx
    {
        public Guid __g_presccriber_id;
        public decimal __i_prescriber_id;
        public __motsynmed_prescriber __prescriber;

        public decimal __rx_num;
        public int __mot_rx_num;

        public Guid __g_rxid;
        public long __i_rxid;

        public __motsynmed_patient __patient;

        public DateTime __written_date;
        public DateTime __start_date;
        public DateTime __expire_date;
        public DateTime __dc_date;

        public int __mdomstart;
        public int __mdomend;
        public byte[] __dow_list;
        public string __dom_list;
        public int __alternate_days;       // Flex Days

        public string __NDC;
        public string __visual_description;
        public int __drug_schedule;
        public string __trade_name;
        public string __cup_name;
        public string __unit;
        public string __strength;
        public string __route;
        public string __dose_form;
        public string __otc;
        public string __consult_message;
        public string __generic_for;

        public RxType? __type;
        public string __dose_code;

        public decimal __qty_per_dose;
        public decimal __qty_to_dispense;
        public int __refills;

        public bool __active;

        public string __sig;
        public int __isolate;
        public int __bulk;
        public int __chart_only;

        public List<__motsynmed_daily_dose> __dose_schedule;
        public List<__motsynmed_titrating_schedule> __t_dose_schedule;
        public List<__motsynmed_alternating_schedule> __a_dose_schedule;

        public __motsynmed_rx()
        {
            __dow_list = new byte[7];
            __dose_schedule = new List<__motsynmed_daily_dose>();
            __t_dose_schedule = new List<__motsynmed_titrating_schedule>();
            __a_dose_schedule = new List<__motsynmed_alternating_schedule>();
            __prescriber = new __motsynmed_prescriber();
        }
    }
    public class __motsynmed_facility
    {
        public Guid __g_id;
        public long __i_id;

        public short __store_id;

        public string __facility_name;
        public __motsynmed_address __main_address;
        public string __phone;

        public __motsynmed_facility()
        {
            __main_address = new __motsynmed_address();
        }
    }
    public class __motsynmed_store
    {
        public short __id;

        public string __store_name;
        public __motsynmed_address __main_address;
        public string __phone;
        public string __dea;

        public __motsynmed_store()
        {
            __main_address = new __motsynmed_address();
        }
    }
    public class __motsynmed_patient
    {
        public Guid __g_patient_id;
        public decimal __i_patient_id;

        public string __last_name;
        public string __first_name;
        public string __middle_initial;
        public DateTime __dob;
        public DateTime __cycle_date;

        public int __cycle_days;

        public string __phone;
        public List<__motsynmed_rx> __rxes;

        public bool __use_patient_address;
        public __motsynmed_address __main_address;

        public string __room;
        public string __bed;

        public Guid __g_prescriber_id;
        public long __i_prescriber_id;
        public __motsynmed_prescriber __prescriber;

        public Guid __g_facility_id;
        public long __i_facility_id;
        public __motsynmed_facility __facility;

        public Guid __g_store_id;
        public short __i_store_id;
        public __motsynmed_store __store;

        public Dictionary<string, string> __card_dose_sn;

        public __motsynmed_patient()
        {
            __main_address = new __motsynmed_address();
            __rxes = new List<__motsynmed_rx>();
            __store = new __motsynmed_store();
            __facility = new __motsynmed_facility();
            __prescriber = new __motsynmed_prescriber();
            __card_dose_sn = new Dictionary<string, string>();
        }
    }
    public class __motsynmed_card : IDisposable
    {
        public __motsynmed_patient __patient;
        public __motsynmed_facility __faccility;
        public __motsynmed_prescriber __prescriber;
        public __motsynmed_store __store;

        public int __card_sn;
        public int[] __bubble_num;
        public DateTime __card_duedate;
        public DateTime __card_enddate;
        public DateTime __card_dispensdate;
        public string __card_time;
        public int __card_type;


        public __motsynmed_card()
        {
            __patient = new __motsynmed_patient();
            __faccility = new __motsynmed_facility();
            __prescriber = new __motsynmed_prescriber();
            __store = new __motsynmed_store();

            __bubble_num = new int[31];
        }
        ~__motsynmed_card()
        {
            Dispose();
        }

        public void Dispose()
        {
            __patient.__rxes.Clear();
        }
    }
    /// <summary>
    /// Base interface for driving the Synmed robot
    /// </summary>
    public interface ISynMedRobotDriver
    {
        Task Login(string __uname, string __pw);
        Task WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __cycle_start_date, int __cycle_length);
        Task WriteCycle(DateTime __cycle_start, DateTime __end_cycle_range);
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
        Logger __logger;

        public Dictionary<string, string> __card_dose_time = new Dictionary<string, string>();   // "08:00", "3245"

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
                {"WV", 48 }, {"WI", 49 }, {"WY", 50 },
                {"PR", 51 }, {"ON", 52 }, {"UK", 53 }
            };

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
        public string __get_next_cardsn(__motsynmed_rx rx)
        {
            try
            {
                DataSet __max_cardsn = new DataSet();
                __motsynmed_facility f = new __motsynmed_facility(); ;

                __get_facility(f, rx.__patient.__i_facility_id);

                __db.executeQuery(string.Format("select MAX(cardsn)  from cardserial;"), __max_cardsn, "__max_cardsn");
                DataRow r = __max_cardsn.Tables["__max_cardsn"].Rows[0];
                int __new_card_sn = __get_value<int>(r, "MAX(cardserial.cardsn)");

                if (f.__facility_name.Contains("'"))
                {
                    f.__facility_name = f.__facility_name.Replace("'", "''");
                }

                __db.executeNonQuery(string.Format("INSERT into cardserial (cardsn, patid, patnamelast, patnamefirst, loccode, locname, prtdttm, prtdate) VALUES ({0}, {1}, '{2}', '{3}', {4}, '{5}', '{6:yyyy-MM-dd}', '{7:yyyy-MM-dd}');",
                                    __new_card_sn + 1,
                                    rx.__patient.__i_patient_id,
                                    rx.__patient.__last_name,
                                    rx.__patient.__first_name,
                                    rx.__patient.__i_facility_id,
                                    f.__facility_name,
                                    DateTime.Now,
                                    DateTime.Now));

                return (__new_card_sn + 1).ToString();
            }
            catch
            {
                return "-1";
            }
        }
        private void __convert_dose_schedule(__motsynmed_rx __rx, long __loc_code)
        {
            DataSet __tq_list = new DataSet();
            __motsynmed_daily_dose __tmp_dose;

            if(__rx.__type == RxType.Prn)
            {
                __rx.__patient.__card_dose_sn.Add("PRN", __get_next_cardsn(__rx));
            }

            if (__rx.__dose_code.Contains("CUSTOM"))  // The LinkCode is the mot RX number
            {
                __db.executeQuery(string.Format("SELECT * from ds_times_qtys where dscode = 'CUSTOM' AND linkcode = '{0}';", __rx.__mot_rx_num), __tq_list, "__dose_schedule");
            }
            else  // The LinkCode is the FacilityID
            {
                __db.executeQuery(string.Format("SELECT * from ds_times_qtys where dscode = '{0}' AND linkcode = '{1}';", __rx.__dose_code, __loc_code), __tq_list, "__dose_schedule");
            }

            if (__tq_list.Tables["__dose_schedule"].Rows.Count > 0)
            {
                foreach (DataRow __record in __tq_list.Tables["__dose_schedule"].Rows)
                {
                    __tmp_dose = new __motsynmed_daily_dose();
                    __tmp_dose.__dose_schedule_name = __rx.__dose_code;
                    __tmp_dose.__dose_time = __get_value<TimeSpan>(__record, "dosetime").ToString();

                    if (!string.IsNullOrEmpty(__tmp_dose.__dose_time) && __tmp_dose.__dose_time.Length > 5)
                    {
                        __tmp_dose.__dose_time = __tmp_dose.__dose_time.Substring(0, 5);
                        __tmp_dose.__qty = __get_value<decimal>(__record, "doseqty");
                        __tmp_dose.__special_instructions = __get_value<string>(__record, "textnotes");

                        if (__tmp_dose.__qty > 0)
                        {
                            __rx.__dose_schedule.Add(__tmp_dose);

                            if (!__rx.__patient.__card_dose_sn.ContainsKey(__tmp_dose.__dose_time))
                            {
                                __rx.__patient.__card_dose_sn.Add(__tmp_dose.__dose_time, __get_next_cardsn(__rx));
                            }
                        }
                    }
                }
            }
        }
        private void __convert_titrating_dose_schedule(__motsynmed_rx __rx, long __loc_code, DateTime __start_date)
        {
            DataSet __titration_list = new DataSet();
            DateTime __current_date = __start_date;
            decimal __dose_qty;

            // Iterate through the titration schedule and update the dose schedule and quantity
            __db.executeQuery(string.Format("SELECT * from ds_times_qtys_special where ds_times_qtys_special.motrxnum = '{0}';", __rx.__mot_rx_num), __titration_list, "__titration_schedule");
            if (__titration_list.Tables["__titration_schedule"].Rows.Count > 0)
            {
                __convert_dose_schedule(__rx, __loc_code);

                foreach (DataRow r in __titration_list.Tables["__titration_schedule"].Rows)
                {
                    __motsynmed_titrating_schedule __tmp_ts = new __motsynmed_titrating_schedule();

                    
                    __tmp_ts.__dose_schedule = __rx.__dose_schedule;
                    __tmp_ts.__dose_date = __current_date;
                    __dose_qty = __get_value<decimal>(r, "doseqty");

                    if (__dose_qty > 0)
                    {
                        foreach (var d in __tmp_ts.__dose_schedule)
                        {
                            d.__qty = __dose_qty;
                        }

                        __rx.__t_dose_schedule.Add(__tmp_ts);
                    }

                    __current_date = __start_date.AddDays(__get_value<byte>(r, "bubble_num"));
                }
            }
        }
        private void __convert_alternating_dose_schedule(__motsynmed_rx __rx, long __loc_code, DateTime __start_date)
        {
            DataSet __alternating_list = new DataSet();
            DateTime __current_date = __start_date;

            __db.executeQuery(string.Format("SELECT flexdays from rx where motrxnum = '{0}';", __rx.__mot_rx_num), __alternating_list, "__alternating_schedule");
            if (__alternating_list.Tables["__alternating_schedule"].Rows.Count > 0)  // There should only be a single row
            {
                DataRow r = __alternating_list.Tables["__alternating_schedule"].Rows[0];
                var __repeat_days = __get_value<byte>(r, "flexdays");

                DateTime __last_date = __start_date.AddDays(30);
                __convert_dose_schedule(__rx, __loc_code);

                while (__current_date <= __last_date)
                {
                    __motsynmed_alternating_schedule __tmp_as = new __motsynmed_alternating_schedule();
                    
                    __tmp_as.__dose_date = __current_date;
                    __tmp_as.__dose_schedule = __rx.__dose_schedule;
                    __rx.__a_dose_schedule.Add(__tmp_as);

                    __current_date = __current_date.AddDays(__repeat_days);
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
                    return RxType.DayOfWeek;  // __dow array represents dose days 

                case 7:
                    return RxType.DayOfMonth; // Day of month string represnts map of days

                case 8:
                    return RxType.MonthlyTitrating; // Multiple Dose Lines in __dose_schedule

                case 9:
                    return RxType.WeeklyTitrating;  // Multiple Dose Lines in __dose_schedule

                case 13:
                    return RxType.Sequential;

                case 18:
                    return RxType.Alternating;  // Dose 1 on Start Date, += __alternate_days until card end or End Date

                default:
                    break;
            }

            return null;
        }
        public void __get_facility(__motsynmed_facility f, long __facility_id)
        {
            DataSet __db_faclities = new DataSet();

            try
            {
                __db.executeQuery(string.Format("SELECT * FROM location WHERE loccode = '{0}';", __facility_id), __db_faclities, "Facilities");
                if (__db_faclities.Tables["Facilities"].Rows.Count > 0)
                {
                    DataRow r = __db_faclities.Tables["Facilities"].Rows[0];

                    f.__facility_name = __get_value<string>(r, "locname");
                    f.__main_address.__address1 = __get_value<string>(r, "Address1");
                    f.__main_address.__address2 = __get_value<string>(r, "Address2");
                    f.__main_address.__city = __get_value<string>(r, "City");
                    f.__main_address.__state = (__get_value<string>(r, "State") != null) ? __state_no[__get_value<string>(r, "State")] : 0;
                    f.__main_address.__zip = __get_value<string>(r, "Zip");
                    f.__phone = __get_value<string>(r, "Phone");
                    f.__store_id = __get_value<short>(r, "mot_storenum");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Retrive facility {0} failed\n{1}", __facility_id, ex.StackTrace);
            }
        }
        public void __get_prescriber(__motsynmed_prescriber p, long __prescriber_id)
        {
            DataSet __db_prescribers = new DataSet();

            __db.executeQuery(string.Format("SELECT * FROM prescriber WHERE doccode = '{0}';", __prescriber_id), __db_prescribers, "Prescribers");
            if (__db_prescribers.Tables["Prescribers"].Rows.Count > 0)
            {
                DataRow r = __db_prescribers.Tables["Prescribers"].Rows[0];

                p.__last_name = __get_value<string>(r, "LastName");
                p.__first_name = __get_value<string>(r, "FirstName");
                p.__middle_initial = __get_value<string>(r, "MiddleInitial");
                p.__main_address.__address1 = __get_value<string>(r, "Address1");
                p.__main_address.__address2 = __get_value<string>(r, "Address2");
                p.__main_address.__city = __get_value<string>(r, "City");
                p.__main_address.__state = (__get_value<string>(r, "State") != null) ? __state_no[__get_value<string>(r, "State")] : 0;
                p.__main_address.__zip = __get_value<string>(r, "Zip");
                p.__dea = __get_value<string>(r, "DEA");
                p.__phone = __get_value<string>(r, "Phone");

            }
        }
        public void __get_store(__motsynmed_store s, short __store_id)
        {
            DataSet __db_store = new DataSet();

            __db.executeQuery(string.Format("SELECT * FROM store WHERE mot_storenum = '{0}';", __store_id), __db_store, "Stores");
            if (__db_store.Tables["Stores"].Rows.Count > 0)
            {
                DataRow r = __db_store.Tables["Stores"].Rows[0];
                s.__store_name = __get_value<string>(r, "Name");
                s.__main_address.__address1 = __get_value<string>(r, "Address1");
                s.__main_address.__address2 = __get_value<string>(r, "Address2");
                s.__main_address.__city = __get_value<string>(r, "City");
                s.__main_address.__state = (__get_value<string>(r, "State") != null) ? __state_no[__get_value<string>(r, "State")] : 0;
                s.__main_address.__zip = __get_value<string>(r, "Zip");
                s.__dea = __get_value<string>(r, "DEA");
                s.__phone = __get_value<string>(r, "Phone");

            }
        }
        // Normal Tasks
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
        /// <summary>
        /// WriteCycle Creates a card set based on the cycle date.
        /// 
        ///     Step 1:  Locate all the patients whose cycles begin on a specific date or date range
        ///     Step 2:  For each active patient, locate all the active prescriptions for the patient and add them to a prescription list the patient class contains
        ///     Step 3:  For each active prescription. locate the doctors that wrote them
        ///     Step 4:  For each active patient, locate the Facility they live in
        ///     Step 5:  Locate the store that provides Medicine-On-Time to the patient
        ///     Step 6:  Write the SynMed file using the collected items
        ///     
        /// </summary>
        /// <param name="__cycle_start"></param>
        /// <returns>Task</returns>
        public async Task WriteCycle(DateTime __cycle_start, DateTime __end_cycle_range)
        {
            try
            {
                // Find all the patients
                DataSet __db_patients = new DataSet();
                List<__motsynmed_patient> __patients = new List<__motsynmed_patient>();

                if (__end_cycle_range > __cycle_start)
                {
                    __db.executeQuery(string.Format("SELECT * FROM Patient WHERE cycledate >= date('{0:yyyy-MM-dd}' AND cycledate <= date('{1:yyyy-MM-dd}');", __cycle_start, __end_cycle_range), __db_patients, "Patients");
                }
                else
                {
                    __db.executeQuery(string.Format("SELECT * FROM Patient WHERE cycledate >= date('{0:yyyy-MM-dd}');", __cycle_start), __db_patients, "Patients");
                }

                // Make sure we have some and add them to the collection
                if (__db_patients.Tables["Patients"].Rows.Count > 0)
                {
                    foreach (DataRow p in __db_patients.Tables["Patients"].Rows)
                    {
                        var __patient = new __motsynmed_patient();

                        __patient.__cycle_date = __get_date_value(p, "CycleDate");
                        __patient.__cycle_days = __get_value<byte>(p, "CycleDays");
                        __patient.__last_name = __get_value<string>(p, "LastName");
                        __patient.__first_name = __get_value<string>(p, "FirstName");
                        __patient.__middle_initial = __get_value<string>(p, "MiddleInitial");
                        __patient.__main_address.__address1 = __get_value<string>(p, "Address1");
                        __patient.__main_address.__address2 = __get_value<string>(p, "Address2");
                        __patient.__main_address.__city = __get_value<string>(p, "City");
                        __patient.__main_address.__state = (__get_value<string>(p, "State") != null) ? __state_no[__get_value<string>(p, "State")] : 0;
                        __patient.__main_address.__zip = __get_value<string>(p, "Zip");
                        __patient.__phone = __get_value<string>(p, "Phone");
                        __patient.__dob = __get_date_value(p, "DOB");
                        __patient.__room = __get_value<string>(p, "Room");

                        // Set up the links to other assets
                        __patient.__i_patient_id = __get_value<int>(p, "MotPatID");
                        __patient.__i_prescriber_id = __get_value<int>(p, "PrimaryDoc");
                        __patient.__i_facility_id = __get_value<int>(p, "LocCode");
                        __patient.__i_store_id = __patient.__facility.__store_id;

                        if (__patient.__i_prescriber_id == 0)
                        {
                            __logger.Warn("Patient {0} - {1} does not have a primary prescriber", __patient.__last_name, __patient.__i_patient_id);
                        }

                        __get_prescriber(__patient.__prescriber, __patient.__i_prescriber_id);
                        __get_facility(__patient.__facility, __patient.__i_facility_id);
                        __get_store(__patient.__store, __patient.__facility.__store_id);

                        // Add it to the list
                        __patients.Add(__patient);
                    }
                }

                // Build the Rx List for each patient
                DataSet __db_rxes = new DataSet();

                foreach (var __patient in __patients)
                {
                    __patient.__rxes = new List<__motsynmed_rx>();

                    __db.executeQuery(string.Format("SELECT * FROM rx JOIN drugs ON rx.drugs_seqno = drugs.seq_no WHERE motpatid = '{0}' AND rx.status = 1 AND rx.deleted = 0;", __patient.__i_patient_id), __db_rxes, "Scrips");

                    if (__db_rxes.Tables["Scrips"].Rows.Count > 0)
                    {
                        foreach (DataRow r in __db_rxes.Tables["Scrips"].Rows)
                        {
                            var __p_id = __get_value<int>(r, "doccode");
                            if (__p_id == 0)
                            {
                                __logger.Error("Ignoring RX with no prescriber ({0})", __get_value<string>(r, "rxsys_rxnum"));
                                continue;
                            }

                            var __exp_date = __get_date_value(r, "expiration_date");
                            if (__exp_date < DateTime.Today || __exp_date < __cycle_start)
                            {
                                __logger.Info("Expired RX");
                                continue;
                            }

                            RxType? __type = __convert_rx(__get_value<byte>(r, "rxtype"));
                            if (__type == null)
                            {
                                __logger.Error("Ignoring RX with no type ({0})", __get_value<decimal>(r, "rxsys_rxnum"));
                                continue;   // Ignore bogus RX types
                            }

                            var __rx = new __motsynmed_rx();

                            // Rx info
                            __rx.__patient = __patient;

                            __rx.__type = __convert_rx(__get_value<byte>(r, "rxtype"));
                            __rx.__expire_date = __exp_date;

                            __rx.__start_date = __cycle_start < __get_date_value(r, "written_date") ? __get_date_value(r, "written_date") : __cycle_start;

                            //__rx.__written_date = __get_date_value(r, "written_date");
                            __rx.__rx_num = __get_value<decimal>(r, "rxsys_rxnum");
                            __rx.__mot_rx_num = __get_value<int>(r, "motrxnum");
                            __rx.__dose_code = __get_value<string>(r, "dscode");
                            __rx.__qty_to_dispense = __get_value<decimal>(r, "qty2disp");
                            __rx.__qty_per_dose = __get_value<decimal>(r, "qty_per_dose");
                            __rx.__sig = __get_value<string>(r, "sig2print");

                            __rx.__mdomstart = __get_value<byte>(r, "mdomstart");
                            __rx.__mdomend = __get_value<byte>(r, "mdomend");
                            __rx.__alternate_days = __get_value<byte>(r, "flexdays");

                            __rx.__dow_list[0] = __get_value<byte>(r, "su");
                            __rx.__dow_list[1] = __get_value<byte>(r, "mo");
                            __rx.__dow_list[2] = __get_value<byte>(r, "tu");
                            __rx.__dow_list[3] = __get_value<byte>(r, "we");
                            __rx.__dow_list[4] = __get_value<byte>(r, "th");
                            __rx.__dow_list[5] = __get_value<byte>(r, "fr");
                            __rx.__dow_list[6] = __get_value<byte>(r, "sa");

                            __rx.__dom_list = __get_value<string>(r, "mdomstring");


                            // Drug Info
                            __rx.__NDC = __get_value<string>(r, "NDCNum");
                            __rx.__drug_schedule = __get_value<byte>(r, "drug_sched");
                            __rx.__trade_name = __get_value<string>(r, "tradename");
                            __rx.__cup_name = __get_value<string>(r, "short_name");
                            __rx.__strength = __get_value<string>(r, "strength");
                            __rx.__unit = __get_value<string>(r, "unit");
                            __rx.__route = __get_value<string>(r, "route");
                            __rx.__dose_form = __get_value<string>(r, "dose_form");
                            __rx.__visual_description = __get_value<string>(r, "visual_descript");
                            __rx.__otc = __get_value<string>(r, "rx_otc");
                            __rx.__generic_for = __get_value<string>(r, "genericfor");

                            // Packaging
                            __rx.__chart_only = __get_value<byte>(r, "chart_only");
                            __rx.__isolate = __get_value<byte>(r, "default_isolate");

                            // ID's                         
                            __rx.__i_prescriber_id = __get_value<int>(r, "doccode");
                            __get_prescriber(__rx.__prescriber, (long)__rx.__i_prescriber_id);

                            switch (__rx.__type)
                            {
                                case RxType.Daily:
                                case RxType.DayOfMonth:
                                case RxType.DayOfWeek:
                                    __convert_dose_schedule(__rx, __patient.__i_facility_id);
                                    break;

                                case RxType.MonthlyTitrating:
                                case RxType.WeeklyTitrating:
                                    __convert_titrating_dose_schedule(__rx, __patient.__i_facility_id, __rx.__start_date);
                                    break;

                                case RxType.Alternating:
                                    __convert_alternating_dose_schedule(__rx, __patient.__i_facility_id, __rx.__start_date);
                                    break;

                                case RxType.Prn:
                                    __convert_dose_schedule(__rx, __patient.__i_facility_id);
                                    foreach (var e in __rx.__dose_schedule)
                                    {
                                        e.__isolate = 1;
                                    }
                                    break;

                                default:
                                    break;
                            }

                            __patient.__rxes.Add(__rx);
                        }
                    }
                }
                // The patient list --might-- contain a bunch of patients with 0 rxes ...
                //var done = -1;

                foreach (var p in __patients)
                {
                    if (p.__rxes.Count > 0)
                    {
                        if(p.__cycle_days == 0)
                        {
                            continue;
                        }

                        __table = new SynMedTable(p.__last_name, p.__first_name, p.__middle_initial, p.__cycle_date, p.__cycle_days);

                        // Definitly need to have a notion of a card here.  There should be a unique SN for each card and each card contains a single dose time

                        __table.WriteLegacyRxCollection(p);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Build Patient Record Failed: {0}", ex.StackTrace);
                __logger.Error("Build Patient Record Failed: {0}", ex.StackTrace);
            }

        }
        public motLegacySynMed(string __path)
        {
            __logger = LogManager.GetLogger("mot_machineInterface");
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
                __logger = LogManager.GetLogger("mot_machineInterface");

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
        public async Task WriteCycle(DateTime __cycle_start, DateTime __end_cycle_range)
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
            foreach (var __row in __table_rows)
            {
                __row.write(__filename, __mode);
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
        public void fillLegacyRow(SynMedRow __new_row, __motsynmed_patient __pat, __motsynmed_rx __rx, __motsynmed_daily_dose __dose, DateTime __base_date, DateTime __current_date)
        {
            __new_row.RECORD_TYPE = "15";
            __new_row.ADMINISTRATION_DATE = string.Format("{0:yyyyMMdd}", __current_date);

            if (__rx.__type != RxType.Prn)
            {
                __new_row.ADMINISTRATION_TIME = string.Format("{0}", __dose.__dose_time);
            }
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
            __new_row.PATIENT_STATE = __state[__pat.__main_address.__state];
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
        public void WriteLegacyRxCollection(__motsynmed_patient __patient)
        {
            DateTime __base_date = __cycle_start_date;
            SynMedRow __new_row;

            try
            {
                foreach (var __rx in __patient.__rxes)
                {
                    DateTime __start_date = __rx.__start_date;
                    DateTime __current_date = __start_date;

                    if (__start_date <= __rx.__start_date && __rx.__expire_date > DateTime.Today)
                    {
                        switch (__rx.__type)
                        {
                            case RxType.Daily:
                                foreach (var __dose in __rx.__dose_schedule)  // The number of dose schedule items should be the number of cards
                                {
                                    for (double __index = 0; __index < __cycle_length; __index++)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Daily Prescription";
                                        __current_date = (DateTime)__base_date.AddDays(__index);
                                        fillLegacyRow(__new_row, __patient, __rx, __dose, __start_date, __current_date);  // Any Daily Schedule (1 line per day)
                                    }
                                }

                                break;

                            case RxType.Prn:

                                __motsynmed_daily_dose __prn_dose = new __motsynmed_daily_dose();
                                __prn_dose.__dose_time = "PRN";
                                __prn_dose.__dose_schedule_name = "PRN";
                                __prn_dose.__isolate = 1;
                                __prn_dose.__qty = __rx.__qty_per_dose;
                                __prn_dose.__card_sn = Convert.ToInt32(__rx.__patient.__card_dose_sn["PRN"]);

                                for (int i = 0; i < __patient.__cycle_days; i++)
                                {
                                    __new_row = new SynMedRow();
                                    __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn["PRN"];
                                    __new_row.PRESCRIPTION_COMMENT = "PRN Prescription";
                                    __new_row.PATIENT_WITH_PRN = "Y";
                                    __new_row.QTY_PER_ADMINISTRATION = __rx.__qty_per_dose.ToString();
                                    __new_row.DRUG_QUANTITY = (__rx.__qty_per_dose * __cycle_length).ToString();
                                    __new_row.ADMINISTRATION_PER_DAY = (__rx.__qty_to_dispense / __cycle_length).ToString();
                                    __new_row.DAY_LAPSE = "1";

                                    fillLegacyRow(__new_row, __patient, __rx, __prn_dose, __start_date, __current_date);
                                }

                                break;

                            case RxType.Alternating:

                                foreach (var __day in __rx.__a_dose_schedule)
                                {
                                    foreach (var __dose in __day.__dose_schedule)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Alternating Prescription - every (" + __rx.__alternate_days + ") days";
                                        fillLegacyRow(__new_row, __patient, __rx, __dose, __start_date, __day.__dose_date);
                                    }
                                }

                                break;

                            case RxType.MonthlyTitrating:
                            case RxType.WeeklyTitrating:

                                if (__rx.__t_dose_schedule != null)
                                {
                                    foreach (var __day in __rx.__t_dose_schedule)
                                    {
                                        foreach (var __dose in __day.__dose_schedule)
                                        {
                                            __new_row = new SynMedRow();
                                            __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__dose.__dose_time];
                                            __new_row.PRESCRIPTION_COMMENT = "Monthly or Weekly Titrating Prescription";
                                            fillLegacyRow(__new_row, __patient, __rx, __dose, __start_date, __day.__dose_date);
                                        }
                                    }
                                }

                                break;

                            case RxType.DayOfMonth:

                                if (__rx.__dom_list == null)
                                {
                                    continue;
                                }
                                __current_date = __start_date;

                                foreach (char __day in __rx.__dom_list)
                                {
                                    if (__day == '1' && __rx.__dose_schedule != null)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__rx.__dose_schedule[0].__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Day Of Month Prescription";
                                        fillLegacyRow(__new_row, __patient, __rx, __rx.__dose_schedule[0], __start_date, __current_date);
                                    }

                                    __current_date = __current_date.AddDays(1);
                                }

                                break;

                            case RxType.DayOfWeek:
                                __current_date = __start_date;

                                foreach (byte __day in __rx.__dow_list)
                                {
                                    if (__day == '1' && __rx.__dose_schedule != null)
                                    {
                                        __new_row = new SynMedRow();
                                        __new_row.GROUP_TITLE = __rx.__patient.__card_dose_sn[__rx.__dose_schedule[0].__dose_time];
                                        __new_row.PRESCRIPTION_COMMENT = "Day Of Week Prescription";
                                        fillLegacyRow(__new_row, __patient, __rx, __rx.__dose_schedule[0], __start_date, __current_date);
                                    }

                                    __current_date = __current_date.AddDays(1);
                                }

                                break;


                            case RxType.Sequential:
                                /*  Not Supported By SynMed
                                var __sequential_regimen = __rx.RxDosageRegimen as RxSequentialRegimen;

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
            catch(Exception ex)
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
                        __csv_header += ";";
                    }
                }

                if (!Directory.Exists(__path))
                {
                    Directory.CreateDirectory(__path);
                }

                __filename = string.Format(@"{0}\{1:yyyyMMdd} - {2} - {3}.card", __path, DateTime.Today, __patient_name, Path.GetRandomFileName());

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