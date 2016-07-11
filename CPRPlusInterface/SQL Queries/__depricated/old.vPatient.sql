/******  VIEW DEFINITIONS FOR MEDICINE-ON-TIME DATA SHARE INTERFACE *****/
/****** Object:  View dbo.vPatient (3 of 12)
    Documentation Date: 02/20/03 8:37:59 AM ******/

if exists (select * from sysobjects where id = object_id('dbo.vPatient') and sysstat & 0xf = 2)
	drop view dbo.vPatient
GO
/** PROVIDES INFO FOR EACH INDIVIDUAL PATIENT **/
/* MS SQLSERVER numeric values
		signed:
 			INTEGER = +-2,000,000,000
			SMALL INT = +- 32,767
			FLOAT = precision to 53 digits
		unsigned:
			TINY INT = 0-255
 */

CREATE VIEW dbo.vPatient
AS
SELECT
	--                 NOTES											FType					FLen
	p.Patient_ID, -- not null (unique ID)					Integer 				  					INDEX UNIQUE
	p.Last_Name, -- not null											VarChar					25					INDEX not unique
	p.First_Name, -- not null											VarChar					15
	p.Middle_Initial, -- nullable									Char						 1
	p.Address_Line_1, -- nullable									VarChar					25
	p.Address_Line_2, -- nullable									VarChar					25
	p.City, -- nullable														VarChar					20
	p.State_Code, -- nullable											Char						 2
	p.Zip_Code, -- nullable												Integer
	p.Zip_Plus_4, -- nullable											Integer
	p.patient_location_code -- nullable										Integer											If your SW supports facility or location designators, otherwise 0 or null consistently
	p.Primary_Prescriber_ID, -- nullable					Integer 				  		   	  INDEX not unique
	 -- if not null, matches vprescriber.prescriber_id
	p.SSN, -- nullable														Integer
	p.BirthDate, -- nullable											DateTime								 		stored as mm/dd/yyyy hh:mm:ss
	p.Deceased_Date,-- nullable										DateTime										ditto
	p.Sex, -- not null														Char						 1
	p.Timestamp as 'MSSQLTS', -- not null					Char						18					INDEX unique or not
	 -- Microsoft SQLServer does not provide a
	 -- *true* timestamp -- this is their
	 -- implementation of a timestamp. ie., 0x0000000100220EB3
 	p.Area_Code, -- nullable											Integer
	p.Telephone_Number, -- nullable								Integer
	p.Extension -- nullable												Integer
FROM	PatientTable p

	GO