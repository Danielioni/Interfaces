/******  VIEW DEFINITIONS FOR MEDICINE-ON-TIME DATA SHARE INTERFACE *****/
/****** Object:  View dbo.vPatient (3 of 12)


--if exists (select * from sysobjects where id = object_id('dbo.vPatient') and sysstat & 0xf = 2)
--	drop view dbo.vPatient
--GO
/** PROVIDES INFO FOR EACH INDIVIDUAL PATIENT **/


--CREATE VIEW dbo.vPatient
--AS
SELECT [MRN]
      ,[LAST_NAME]
      ,[FIRST_NAME]
      ,NULL AS Middle_Initial
      ,[ADDRESS]
      ,[ADDRESS2]
      ,[CITY]
      ,[STATE]
      ,[ZIP]
      ,NULL AS Zip_Plus_4
	  ,NULL AS patient_location_code
	  ,[PH_NO]
	  ,[SSN]
	  ,DISC_DATE
	  ,[SEX]
	  ,TIMESTAMP
	  ,NULL AS Area_Code
      ,[PHONE]
  FROM [CPRPROD].[dbo].[HR]
-- GO