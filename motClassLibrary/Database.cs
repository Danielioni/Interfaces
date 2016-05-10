using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Direct Access to a database view.  This one is for McKesson
/// </summary>

namespace motInboundLib
{
    class Database
    {
        string getPrescriptionRecQuery =
    " CREATE VIEW dbo.vRx AS " +
    "    SELECT " +
	"-- NOTES FType                         FLen" +
    "        rx.Patient_ID,             -- not null Integer INDEX not unique" +
    "        rx.Rx_ID,                  -- not null (unique ID)  Integer INDEX UNIQUE" +
    "        rx.External_Rx_ID,         -- nullable (Rx# seen by user)	Integer INDEX not unique" +
	"        rx.Prescriber_ID,          -- not null (match vprescriber) Integer INDEX not unique" +
    "        rx.Dosage_Signa_Code,      -- nullable(T, T2, C, etc) VarChar 7" +
	"        s.Signa_Message AS 'Decoded_Dosage_Signa'," +
	"                                   -- nullable(Take 1 Teaspoonful, etc.) VarChar 255"  +
	"        s.Signa_String,            -- nullable VarChar 80" +
    "        s.Instruction_Signa_Text,  -- nullable VarChar 255" +
    "        rx.Date_Written,           -- not null DateTime stored as mm/dd/yyyy hh:mm:ss" +
    "        rx.Dispense_Date,          -- not null DateTime ditto" +
    "        d.Last_Dispense_Stop_Date, -- not null DateTime ditto storage - INDEX not unique" +
    "        rx.Total_Refills_Authorized, -- not null Integer" +
    "        rx.Total_Refills_Used,     -- not null Integer" + 
    "        rx.Dispensed_Item_ID,      -- not null (match to vitem)Integer if dispensed_item_id" +
    "        rx.Item_Version AS 'Dispensed_Item_Version'," +
	"                                   -- not null Small Int and dispensed_item_version" +
    "        i.NDC_Code,                -- nullable Char	11" +
    "        d.Quantity_Dispensed,      -- nullable Float 2 decimals" +
    "        rx.Written_For_Item_ID,    -- not null	Integer<> written_for_itemID (INDEX not unique)" +
    "        rx.Written_For_Item_Version,-- not null Small Int and writen_for_Itemversion, (INDEX not unique)" +
	"                                   --										" +
    "        then it's a substitute/generic item " +
    "        rx.Script_Status,          -- not null (A[ctive],P[rofile])Char 1" +
	"        rx.Prescription_Expiration_Date, -- not null DateTime stored as mm/dd/yyyy hh:mm:ss" +
    "        rx.Responsible_Prescriber_ID, -- nullable Integer" +
	"                                   -- if not null, match to prescriber_id" +
    "        rx.Discontinue_Date,       -- nullable DateTime ditto stored; INDEX not unique" +
    "        rx.Quantity_Written,       -- not null Float trim to 2 decimals" +
    "        rx.Total_Qty_Used,         -- nullable Float trim to 2 decimals" +
    "        rx.Total_Qty_Authorized,   -- not null Float trim to 2 decimals" +
    "        rx.Days_Supply_Written,    -- not null Small Int" +
    "        rx.Days_Supply_Remaining,  -- not null Small Int" +
    "        rx.Script_Origin_Indicator,-- not null (PHO, WRI) Char	 3" +
	"        rx.Dispense_ID,            --- not null Integer" +
    "        rx.Last_Dispense_Number,   ---- not null Integer" +
    "        rx.Timestamp AS 'MSSQLTS'  -- not null SEE DISCUSSION VPATIENT INDEX UNIQUE or not" +
    "FROM    RxTable," +
    "        DispenseHistoryTable d," +
    "        StringSigLookup s," +
    "        ItemTable i" +
    "WHERE[SigJoin] and [DispenseHistoryTableJoin] and[ItemTableJoin]";

        public class mckessonPatient : motPatientRecord
        {
            /*
            Patient_ID              int         4
            Last_Namevar            char        25
            First_Namevar           char        15z
            Middle_Initial          char        1
            Address_Line_1          varchar     25
            Address_Line_2          varchar     25
            City                    varchar     20
            State_Code              char        2
            Zip_Code                int         4
            Zip_Plus_4              int         4
            Patient_Location_Code   char        2
            Primary_Prescriber_ID   int         4
            SSN                     int         4
            BirthDate               datetime    8
            Deceased_Date           datetime    8
            Sex                     char        1
            MSSQLTS                 timestamp   8
            Area_Code               int         4
            Telephone_Number        int         4
            Extension               int         4
           
            // Allergy Record
            Patient_ID              int         4
            Patient_Allergy_ID      int         4
            Allergy_Class_Code      varchar     3
            Descriptionvar          char        80
            Allergy_Free_Text       varchar     70
            Item_ID                 int         4
            Onset_Date              datetime    8
            


            */
        }
    }
}
