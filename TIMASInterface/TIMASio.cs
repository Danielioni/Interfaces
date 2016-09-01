using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using motCommonLib;

namespace TIMASIntegration
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
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
        public string OutputDirectory { get; set; } = @"C:\MOTNow\Export\TIMAS";


    public TIMASOutput()
{}

 ~TIMASOutput()
{}

    }
}
