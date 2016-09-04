using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using motCommonLib;

namespace TIMASIntegration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class TIMASOutput
    {

            public string LASTNAME { get; set; } = string.Empty;
            public string FIRSTNAME { get; set; } = string.Empty;
            public string DOB { get; set; } = string.Empty;             // MM/DD/YYYY   - Required
            public string SSN { get; set; } = string.Empty;             // 999999999  - Required
            public string ClientID { get; set; } = string.Empty;
            public string Medicine { get; set; } = string.Empty;        // - Required
            public string Generic { get; set; } = string.Empty;
            public string Start { get; set; } = string.Empty;           // MM/DD/YYYY - Required
            public string Stop { get; set; } = string.Empty;            // MM/DD/YYYY - Required
            public string PRN { get; set; } = string.Empty;             // Y/N
            public string Frequency { get; set; } = string.Empty;       // Dose Schedule - Required
            public string ReasonMedGiven { get; set; } = string.Empty;
            public string Dosage { get; set; } = string.Empty;          // N Thing (Tab, ...) - Required
            public string Strength { get; set; } = string.Empty;        // N Units - Required
            public string DrugType { get; set; } = string.Empty;
            public string Routes { get; set; } = string.Empty;          // - Required
            public string Psychotropic { get; set; } = string.Empty;
            public string SpecialInstructions { get; set; } = string.Empty;
            public string RXNumber { get; set; } = string.Empty;        // - Required
            public string HOATime { get; set; } = string.Empty;
            public string PharmacyNotes { get; set; } = string.Empty;
            public string NDC { get; set; } = string.Empty;

        // End of data
        private FileStream __file;
        private CsvFileWriter __csv_manager;
        private motDatabase __db;
        private string __dsn = @"server=127.0.0.1;port=5432;userid=mot;password=mot!cool;database=Mot";

        public string OutputDirectory { get; set; } = @"C:\MOTNow\Export\TIMAS";

        public void Write(DateTime __all_from_start, DateTime __to_end_date)
        { }
        public void WriteHeader()
        {
            List<string> __header_row = new List<string>
            {
                "LASTNAME",
                "FIRSTNAME",
                "DOB",
                "SSN",
                "ClientID",
                "Medicine",
                "Generic",
                "Start",
                "Stop",
                "PRN",
                "Frequency",
                "ReasonMedsGiven",
                "Dosage",
                "Strength",
                "DrugType",
                "Routes",
                "Psychotropic",
                "SpecialInstructions",
                "RXNumber",
                "HOATime",
                "PharmacyNotes",
                "NDC"
            };

            __csv_manager.WriteRow(__header_row);
        }

        public void Write(List<string> __patient_ids)
        {
            // Do Query Here

        }

        private void __open(bool __overwrite)
        {
            try
            {
                bool __exists = false;

                if (File.Exists(OutputDirectory))
                {
                    __exists = true;
                }

                if (__overwrite)
                {
                    __file = new FileStream(OutputDirectory, FileMode.CreateNew | FileMode.Append);
                }
                else
                {
                    __file = new FileStream(OutputDirectory, FileMode.OpenOrCreate | FileMode.Append);
                }


                if (!__exists)
                {
                    WriteHeader();
                }

                __csv_manager = new CsvFileWriter(__file);
                __db = new motDatabase(__dsn, dbType.NPGServer);

            }
            catch
            {
                throw;
            }
        }
        public TIMASOutput(bool __overwrite)
        {
            __open(__overwrite);
        }
        public TIMASOutput()
        {
            __open(true); 
        }
        ~TIMASOutput()
        {
            __file.Close();
        }
    }
}
