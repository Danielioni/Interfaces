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
        Logger __logger;

        private static IAuthenticationService authService;
        private string __username;
        private string __password;
        private bool __logged_in = false;
        private string __file_name;

        public async Task<int> GetRxCount(Guid patId, DateTime dt)
        {
            int retval = 0;
            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Rx>>();
                    var rxes = await query1.QueryAsync(new QueryParameters<Rx>(__rx => __rx.PatientId == patId && __rx.Status == RxStatus.Active));

                    foreach(var r in rxes)
                    {
                        if(!r.IsExpiredByDate(dt))
                        {
                            retval++;
                        }
                    }
                }
            }
            catch
            { throw; }

            return retval;
        }

        // Display Support
        public static DataSet ToDataSet<T>(IEnumerable<T> list, string table_name)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, ColType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            ds.DataSetName = table_name;
            ds.Tables[0].TableName = table_name;

            return ds;
        }
        public async Task<DataSet> GetFacilitiesAsDataSet(string __loc = null)
        {
            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Facility>>();

                    if (__loc == null)
                    {
                        var facilities = await query1.QueryAsync(new QueryParameters<Facility>(__facility => !__facility.IsHidden));
                        return ToDataSet<Facility>(facilities, "Facilities");
                    }
                    else
                    {
                        Guid __g = new Guid(__loc);
                        var facilities = await query1.QueryAsync(new QueryParameters<Facility>(__facility => !__facility.IsHidden && __facility.Id == __g));
                        return ToDataSet<Facility>(facilities, "Facilities");
                    }
                }
            }
            catch (Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<DataSet> GetPatientsAsDataSet(string __loc = null)
        {
            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Patient>>();
                                  
                    if (__loc == null)
                    {
                        var patients = await query1.QueryAsync(new QueryParameters<Patient>(__patient => __patient.Status == Status.Active));
                        return ToDataSet<Patient>(patients, "Patients");
                    }
                    else
                    {
                        Guid __g = new Guid(__loc);
                        var patients =  await query1.QueryAsync(new QueryParameters<Patient>(__patient => __patient.Status != Status.Hold && __patient.FacilityId == __g));
                        return ToDataSet<Patient>(patients, "Patients");
                    }
                }
            }
            catch (Exception ex)
            {
                __logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        public async Task<DataSet> GetRxesAsDataSet(Guid patId, DateTime dt)
        {
            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Rx>>();                 
                    var rxes = await query1.QueryAsync(new QueryParameters<Rx>(__rx => __rx.PatientId == patId && __rx.Status == RxStatus.Active));
                    return ToDataSet<Rx>(rxes, "Rxes");
                }
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
            }
            catch
            { throw; }
        }


        public async Task<bool> Login(string __uname, string __pw)
        {
            __username = __uname;
            __password = __pw;

            try
            {
                var auth = new AuthorizeModel();

                auth.GrantAccessNeeded = false;
                auth.UserName = __uname;
                auth.UserPassword = __pw;

                //Authentication service will automatically store access_token and refresh_token and re-issue them when they are about to expire.
                await authService.LoginAsync(auth);

               return  __logged_in = true;               
            }
            catch
            {
                throw;
            }

            return false;
        }
        public async Task<bool> WriteCycle(DateTime __cycle_start, DateTime __end_cycle_range)
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
                                                
                                    if(__rx.Status == RxStatus.Active)
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
                                            new QueryParameters<Rx>(rx => __patient.Id == rx.PatientId && rx.Status == RxStatus.Active,
                                                    r => r.RxDosageRegimen.DoseSchedule,
                                                    r => r.RxDosageRegimen.DoseSchedule.DoseScheduleItems,
                                                    r => r.Drug,
                                                    r => r.Prescriber,
                                                    r => r.Store)
                                        );
                                     
                                                                      
                                    __table = new SynMedTable(__patient.LastName, __patient.FirstName, __patient.MiddleInitial, __patient.DueDate, (int)__patient.CardDays);
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

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

            return false;
        }
        public async Task<bool> WritePatient(Guid __patID, DateTime __from_date)
        {
            if (!__logged_in)
            {
                throw new Exception("Not logged in");
            }

            int __counter = 0;
            try
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Rx>>();

                    var rxes = await query1.QueryAsync(
                            new QueryParameters<Rx>(rx => rx.Status == RxStatus.Active && rx.Id == __patID,
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

                    string __patient_last_name = string.Empty;
                    string __patient_first_name = string.Empty;
                    string __patient_middle_initial = string.Empty;
                    DateTime __due_date = DateTime.Today;
                    int __card_days = 28;
                    Patient __patient = (Patient)null;

                    foreach(var r in rxes)
                    {
                        __patient_first_name = r.Patient.FirstName;
                        __patient_last_name = r.Patient.LastName;
                        __patient_middle_initial = r.Patient.MiddleInitial;
                        __due_date = r.Patient.DueDate;
                        __card_days = (int)r.Patient.CardDays;
                        __patient = r.Patient;

                        __counter++;
                    }

                    if (__counter > 0)
                    {
                        __table = new SynMedTable(__patient_last_name, __patient_first_name, __patient_middle_initial, __due_date, (int)__card_days);
                        await __table.WriteRxCollection(rxes, __file_name, __patient);
                    }
                }

                return true;            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }
        public async Task WriteFacilityCycle(string __facility_name, DateTime __cycle_start_date, int __cycle_length)
        {
            if (!__logged_in)
            {
                throw new Exception("Not logged in");
            }

            try
            {
                var __due_date = new DateTimeOffset(__cycle_start_date);

                using (var scope = container.BeginLifetimeScope())
                {
                    var query1 = scope.Resolve<IEntityQuery<Facility>>();
                    var query2 = scope.Resolve<IEntityQuery<Patient>>();

                    // Find the Facility
                    var __facility = await query1.QueryAsync(
                                new QueryParameters<Facility>(f => f.Name == __facility_name));

                    // Process each patient
                    foreach (var f in __facility)
                    {
                        var __patients = await query2.QueryAsync(new QueryParameters<Patient>(p => p.FacilityId == f.Id && p.DueDate == __due_date));
                        foreach (var p in __patients)
                        {
                            await WritePatient(p.Id, p.DueDate);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
