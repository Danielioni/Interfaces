using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.IO;

using System.Data;
using NLog;


using Mot.Shared.Model.Rxes;


using motCommonLib;

namespace motMachineInterface
{
    public abstract class synMedBase
    {

    }
    /// <summary>
    /// Implementation for motLegacy drawing data from SQLAnywhere
    /// </summary>
    public class motLegacySynMed : synMedBase
    {
        public motODBCServer __db { get; set; }
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


        // Models
        //public BindingList<__motsynmed_patient> Patients { get; set; }
        //public BindingList<__motsynmed_rx> Rxes { get; set; }
        //public BindingList<__motsynmed_facility> Facilities { get; set; }

        public DataSet motFacilities { get; set; }
        public DataSet motPatients { get; set; }

        /// <summary>
        /// Generalized classes for mapping motLegacy and motNext data into a common model 
        /// </summary>
        public class __motsynmed_address
        {
            public string __address1 { get; set; }
            public string __address2 { get; set; }
            public string __city { get; set; }
            public string __state { get; set; }
            public string __zip { get; set; }
            public string __phone_number { get; set; }
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
        /*
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
         */
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

            public DateTime __written_date { get; set; }
            public DateTime __start_date { get; set; }
            public DateTime __expire_date { get; set; }
            public DateTime __dc_date;

            public int __mdomstart;
            public int __mdomend;
            public byte[] __dow_list;
            public string __dom_list;
            public int __alternate_days;       // Flex Days

            public string __NDC { get; set; }
            public string __visual_description { get; set; }
            public int __drug_schedule { get; set; }
            public string __trade_name;
            public string __cup_name { get; set; }
            public string __unit { get; set; }
            public string __strength { get; set; }
            public string __route { get; set; }
            public string __dose_form { get; set; } 
            public string __otc;
            public string __consult_message;
            public string __generic_for;

            public RxType? __type { get; set; }
            public string __dose_code;

            public decimal __qty_per_dose { get; set; }
            public decimal __qty_to_dispense;
            public int __refills;

            public bool __active;

            public string __sig { get; set; }
            public int __isolate { get; set; }
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
            public long __i_id { get; set; }

            public short __store_id;

            public string __facility_name { get; set; }
            public __motsynmed_address __main_address { get; set; }
            public string __phone { get; set; }

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

            public string __last_name { get; set; }
            public string __first_name { get; set; }
            public string __middle_initial { get; set; }
            public DateTime __dob { get; set; }
            public DateTime __cycle_date { get; set; }

            public int __cycle_days { get; set; }

            public string __phone { get; set; }
            public List<__motsynmed_rx> __rxes;

            public bool __use_patient_address;
            public __motsynmed_address __main_address { get; set; }

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

        // Display Support
        public DataSet GetFacilitiesAsDataSet(string __loc = null)
        {
            DataSet __db_facilitiess = new DataSet();

            try
            {
                if (__loc == null)
                {
                    __db.executeQuery(string.Format("SELECT * FROM Location;"), __db_facilitiess, "Facilities");
                }
                else
                {
                    __db.executeQuery(string.Format("SELECT * FROM Location WHERE LocCode = '{0}';", __loc), __db_facilitiess, "Facilities");
                }

                return __db_facilitiess;
            }
            catch (Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        public DataSet GetPatientsAsDataSet(string __loc = null)
        {
            DataSet __db_patients = new DataSet();

            try
            {
                if (__loc == null)
                {
                    __db.executeQuery(string.Format("SELECT * FROM Patient;"), __db_patients, "Patients");
                }
                else
                {
                    __db.executeQuery(string.Format("SELECT * FROM Patient WHERE LocCode = '{0}';", __loc), __db_patients, "Patients");
                }

                return __db_patients;
            }
            catch(Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        public DataSet GetRxesAsDataSet(int patId, DateTime dt)
        {
            try
            {
                DataSet __ds_rxes = new DataSet();

                __db.executeQuery(string.Format("SELECT rx.rxsys_rxnum, rx.dscode, rx.qty_per_dose, rx.isolate, drugs.Tradename, rx.sig2print, rx.expiration_date  " +
                                                               "FROM drugs, rx " +
                                                               "WHERE rx.motpatid = {0} AND drugs.Seq_No = rx.drugs_seqno AND rx.status = 1 AND date('{1:yyyy-MM-dd}') < expiration_date;",
                                                               patId, dt), __ds_rxes, "Rxes");
                return __ds_rxes;
            }
            catch(Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public BindingList<__motsynmed_rx> __get_rxes(int id)
        {
            DataSet __db_rxes = new DataSet();
            BindingList<__motsynmed_rx> RXList = new BindingList<__motsynmed_rx>();

            __db.executeQuery(string.Format("SELECT * FROM rx JOIN drugs ON rx.drugs_seqno = drugs.seq_no WHERE motpatid = '{0}' AND rx.status = 1 AND rx.deleted = 0;", id), __db_rxes, "Scrips");

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

                    RxType? __type = __convert_rx(__get_value<byte>(r, "rxtype"));
                    if (__type == null)
                    {
                        __logger.Error("Ignoring RX with no type ({0})", __get_value<decimal>(r, "rxsys_rxnum"));
                        continue;   // Ignore bogus RX types
                    }

                    var __rx = new __motsynmed_rx();

                    // Rx info                    
                    __rx.__type = __convert_rx(__get_value<byte>(r, "rxtype"));
                    __rx.__expire_date = __exp_date;
                    __rx.__start_date = __get_date_value(r, "written_date");
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
                            __convert_dose_schedule(__rx, __rx.__patient.__i_facility_id);
                            break;

                        case RxType.MonthlyTitrating:
                        case RxType.WeeklyTitrating:
                            __convert_titrating_dose_schedule(__rx, __rx.__patient.__i_facility_id, __rx.__start_date);
                            break;

                        case RxType.Alternating:
                            __convert_alternating_dose_schedule(__rx, __rx.__patient.__i_facility_id, __rx.__start_date);
                            break;

                        case RxType.Prn:
                            __convert_dose_schedule(__rx, __rx.__patient.__i_facility_id);
                            foreach (var e in __rx.__dose_schedule)
                            {
                                e.__isolate = 1;
                            }
                            break;

                        default:
                            break;
                    }

                    RXList.Add(__rx);
                }

                return RXList;
            }

            return null;
        }
        public BindingList<__motsynmed_patient> __get_patients(string __loc = null)
        {
            DataSet __db_patients = new DataSet();
            BindingList<__motsynmed_patient> __patients = new BindingList<__motsynmed_patient>();

            try
            {
                if (__loc == null)
                {
                    __db.executeQuery(string.Format("SELECT * FROM Patient;"), __db_patients, "Patients");
                }
                else
                {
                    __db.executeQuery(string.Format("SELECT * FROM Patient WHERE LocCode = '{0}';", __loc), __db_patients, "Patients");
                }
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
                        __patient.__main_address.__state = __get_value<string>(p, "State");
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

                    return __patients;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to retrive patients: {0}", ex.Message);
            }

            return null;
        }
        public BindingList<__motsynmed_facility> __get_faclities()
        {
            DataSet __db_facilities = new DataSet();
            BindingList<__motsynmed_facility> __facilities = new BindingList<__motsynmed_facility>();

            __db.executeQuery(string.Format("SELECT * FROM location;"), __db_facilities, "Facilities");
            if (__db_facilities.Tables["Facilities"].Rows.Count > 0)
            {
                foreach (DataRow r in __db_facilities.Tables["Facilities"].Rows)
                {
                    var f = new __motsynmed_facility();

                    f.__facility_name = __get_value<string>(r, "locname");
                    f.__i_id = __get_value<int>(r, "loccode");
                    f.__main_address.__address1 = __get_value<string>(r, "Address1");
                    f.__main_address.__address2 = __get_value<string>(r, "Address2");
                    f.__main_address.__city = __get_value<string>(r, "City");
                    f.__main_address.__state = __get_value<string>(r, "State");
                    f.__main_address.__zip = __get_value<string>(r, "Zip");
                    f.__phone = __get_value<string>(r, "Phone");
                    f.__store_id = __get_value<short>(r, "mot_storenum");

                    __facilities.Add(f);
                }

                return __facilities;
            }

            return null;
        }
        public int __rx_count(int patId, DateTime dt)
        {
            var x = new DataSet();

            __db.executeQuery(string.Format("SELECT * FROM rx WHERE rx.motpatid = {0} AND rx.status = 1 AND date('{1:yyyy-MM-dd}') < expiration_date", patId, dt), x, "Counter");
            return x.Tables["Counter"].Rows.Count;
        }

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

            if (__rx.__type == RxType.Prn)
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
                    f.__main_address.__state = __get_value<string>(r, "State");
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
                p.__main_address.__state = __get_value<string>(r, "State");
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
                s.__main_address.__state = __get_value<string>(r, "State");
                s.__main_address.__zip = __get_value<string>(r, "Zip");
                s.__dea = __get_value<string>(r, "DEA");
                s.__phone = __get_value<string>(r, "Phone");

            }
        }

        // Normal Tasks
        public void Login(string __uname, string __pw)
        {
            try
            {
                __db = new motODBCServer("dsn=MOT8;UID=" + __uname + ";PWD=" + __pw);

                // Build the initial models
                //Patients = __get_patients();
                //Facilities = __get_faclities();

                //motPatients = GetPatientsAsDataSet();
                //motFacilities = GetFacilitiesAsDataSet();

            }
            catch (Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
        public void WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length)
        { }

        public void WritePatient(int __patID, DateTime __from_date)
        {
            try
            {
                DataSet __db_patients = new DataSet();
                DataSet __db_rxes = new DataSet();

                __db.executeQuery(string.Format("SELECT * FROM patient where motpatID = {0}; ", __patID), __db_patients, "Patients");
                if (__db_patients.Tables["Patients"].Rows.Count == 0)
                {
                    return;
                }

                var p = __db_patients.Tables["Patients"].Rows[0];
                var __patient = new __motsynmed_patient();

                __patient.__cycle_date = __get_date_value(p, "CycleDate");
                __patient.__cycle_days = __get_value<byte>(p, "CycleDays");
                __patient.__last_name = __get_value<string>(p, "LastName");
                __patient.__first_name = __get_value<string>(p, "FirstName");
                __patient.__middle_initial = __get_value<string>(p, "MiddleInitial");
                __patient.__main_address.__address1 = __get_value<string>(p, "Address1");
                __patient.__main_address.__address2 = __get_value<string>(p, "Address2");
                __patient.__main_address.__city = __get_value<string>(p, "City");
                __patient.__main_address.__state = __get_value<string>(p, "State");
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


                __db.executeQuery(string.Format("SELECT *  " +
                                                "FROM drugs, rx " +
                                                "WHERE rx.motpatid = {0} AND drugs.Seq_No = rx.drugs_seqno AND rx.status = 1 AND date('{1:yyyy-MM-dd}') < expiration_date;",
                                                 __patID, __from_date), __db_rxes, "Rxes");

                if (__db_rxes.Tables["Rxes"].Rows.Count > 0)
                {
                    foreach (DataRow r in __db_rxes.Tables["Rxes"].Rows)
                    {
                        var __rx = new __motsynmed_rx();

                        // Rx info
                        __rx.__patient = __patient;

                        __rx.__type = __convert_rx(__get_value<byte>(r, "rxtype"));
                        __rx.__expire_date = __get_date_value(r, "expiration_date");

                        __rx.__start_date = __from_date;

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

                    __table = new SynMedTable(__patient.__last_name, __patient.__first_name, __patient.__middle_initial, __patient.__cycle_date, __patient.__cycle_days);
                    __table.WriteLegacyRxCollection(__patient);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Build Patient Record Failed: {0}", ex.StackTrace);
                __logger.Error("Build Patient Record Failed: {0}", ex.StackTrace);
            }
        }
        public void WritePatient(string __last_name, string __first_name, string __middle_initial, DateTime __cycle_start_date, int __cycle_length)
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
        public void WriteCycle(DateTime __cycle_start, DateTime __end_cycle_range)
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
                        __patient.__main_address.__state = __get_value<string>(p, "State");
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
                        if (p.__cycle_days == 0)
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
        public motLegacySynMed(string __path = null)
        {
            __logger = LogManager.GetLogger("motLegacy_machineInterface");
        }
    }
}
