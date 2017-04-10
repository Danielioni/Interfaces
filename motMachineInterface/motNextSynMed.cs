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

using Mot.Shared.Model;
using Mot.Shared.Model.Cards;
using Mot.Shared.Model.Rxes;
using Mot.Shared.Model.Rxes.RxRegimens;
using Mot.Shared.Model.Patients;

using Mot.Shared.Framework;

using motCommonLib;

namespace motMachineInterface
{
    public class motNextSynMed : synMedBase
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

        // Display Support
        public DataSet GetFacilitiesAsDataSet(string __loc = null)
        {
            DataSet __db_facilitiess = new DataSet();

            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Facility>>();

                    if (__loc == null)
                    {
                        var facilities = query1.QueryAsync(new QueryParameters<Facility>(__facility => __facility.Id != null, f => f.Name));
                    }
                    else
                    {
                        Guid __g = new Guid(__loc);
                        var facilities = query1.QueryAsync(new QueryParameters<Facility>(__facility => __facility.Id == __g, f => f.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return __db_facilitiess;

        }
        public DataSet GetPatientsAsDataSet(string __loc = null)
        {
            DataSet __db_patients = new DataSet();

            try
            {
                if (__loc == null)
                {
                    //__db.executeQuery(string.Format("SELECT * FROM Patient;"), __db_patients, "Patients");
                }
                else
                {
                    //__db.executeQuery(string.Format("SELECT * FROM Patient WHERE LocCode = '{0}';", __loc), __db_patients, "Patients");
                }

                return __db_patients;
            }
            catch (Exception ex)
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

                //__db.executeQuery(string.Format("SELECT rx.rxsys_rxnum, rx.dscode, rx.qty_per_dose, rx.isolate, drugs.Tradename, rx.sig2print, rx.expiration_date  " +
                //                                               "FROM drugs, rx " +
                //                                               "WHERE rx.motpatid = {0} AND drugs.Seq_No = rx.drugs_seqno AND rx.status = 1 AND date('{1:yyyy-MM-dd}') < expiration_date;",
                //                                               patId, dt), __ds_rxes, "Rxes");
                return __ds_rxes;
            }
            catch (Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return null;
        }

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
                __logger = LogManager.GetLogger("motNext_machineInterface");

                Simple.OData.Client.V4Adapter.Reference();

                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterModule<FrameworkModule>();
                containerBuilder.RegisterModule<SdkModule>();
                container = containerBuilder.Build();


                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                authService = container.Resolve<IAuthenticationService>();

               // if (__pathname == null)
               // {
               //     setup(__pathname);
               //}

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
                var auth = new AuthorizeModel();

                auth.IsGrantAccesNeed = false;
                auth.UserName = __uname;
                auth.UserPassword = __pw;

                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                await authService.LoginAsync(auth);

                __logged_in = true;

                /* Build the initial models
                Patients = new PatientModel();
                Patients.Items = __get_patients();

                RXes = new RxModel();
                RXes.Items = __get_rxes((int)Patients.Items[0].__i_patient_id);

                Facilities = new FacilityModel();
                Facilities.Items = __get_faclities();
                */
            }
            catch
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
                                                                                         p => p.Address,
                                                                                         p => p.Facility,
                                                                                         p => p.Facility.Address
                                                                                         ));

                    foreach (var __patient in __patients)
                    {
                        int counter = 0;

                        try
                        {
                            if (__patient.Rxes.Count > 0)
                            {
                                if (__patient.DateOfBirth == null)
                                {
                                    __patient.DateOfBirth = DateTime.Today;  // Lots of bad data in lagacy import
                                }

                                foreach(var __rx in __patient.Rxes)
                                {
                                    //if(__rx.StartDate <= DateTime.Today && __rx.StopDate > DateTime.Today)
                                    //{
                                    //    counter++;
                                    //}

                                    if(__rx.IsActive)
                                    {
                                        counter++;
                                    }
                                }

                                if (counter > 0)
                                {
                                    TimeSpan __ts = __patient.CurrentCycleEndDate - __patient.CurrentCycleStartDate;
                                    var __stop_date = new DateTimeOffset(DateTime.Today);

                                   
                                                             
                                    var query2 = scope.Resolve<IEntityQuery<Rx>>();
                                    var rxes = await query2.QueryAsync(
                                            new QueryParameters<Rx>(rx => __patient.Id == rx.PatientId && rx.Status != RxStatus.Discountinue && rx.StopDate > __stop_date,
                                                    r => r.RxDosageRegimen.DoseSchedule,
                                                    r => r.RxDosageRegimen.DoseSchedule.DoseScheduleItems,
                                                    r => r.Drug,
                                                    r => r.Prescriber,
                                                    r => r.Store)
                                        );
                                     
                                                                      
                                    __table = new SynMedTable(__patient.LastName, __patient.FirstName, __patient.MiddleInitial, __patient.DueDate, 30);
                                    await __table.WriteRxCollection(rxes, __file_name, __patient);

                                    Console.WriteLine("Wrote: {0}, {1} {2} Rxcount: {3}", __patient.LastName, __patient.FirstName, __patient.MiddleInitial, counter);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
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
        public async Task WritePatient(Patient __patient, string __last_name, string __first_name, string __middle_initial, DateTime __cycle_start_date, int __cycle_length)
        {
            if (!__logged_in)
            {
                throw new Exception("Not logged in");
            }

            try
            {
                
                var __stop_date = new DateTimeOffset(DateTime.Today);

                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Rx>>();

                    if (!string.IsNullOrEmpty(__middle_initial))
                    {

                        var rxes = await query1.QueryAsync(
                            new QueryParameters<Rx>(rx => rx.Status != RxStatus.Discountinue && rx.Patient.LastName == __last_name && rx.Patient.FirstName == __first_name && rx.Patient.MiddleInitial == __middle_initial && rx.StopDate > __stop_date,
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
                        await __table.WriteRxCollection(rxes, __file_name, __patient);
                        
                    }
                    else
                    {

                        var rxes = await query1.QueryAsync(
                            new QueryParameters<Rx>(rx => rx.Status != RxStatus.Discountinue && rx.Patient.LastName == __last_name && rx.Patient.FirstName == __first_name && rx.StopDate > __stop_date,
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
                        await __table.WriteRxCollection(rxes, __file_name, __patient);
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
}
