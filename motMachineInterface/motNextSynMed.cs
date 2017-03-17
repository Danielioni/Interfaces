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
    public class motNextSynMed
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

                /* Build the initial models
                Patients = new PatientModel();
                Patients.Items = __get_patients();

                RXes = new RxModel();
                RXes.Items = __get_rxes((int)Patients.Items[0].__i_patient_id);

                Facilities = new FacilityModel();
                Facilities.Items = __get_faclities();
                */
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
}
