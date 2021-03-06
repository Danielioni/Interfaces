/******  VIEW DEFINITIONS FOR MEDICINE-ON-TIME DATA SHARE INTERFACE *****/
/****** Object:  View dbo.vPrescriber (7 of 12)


--if exists (select * from sysobjects where id = object_id('dbo.vPrescriber') and sysstat & 0xf = 2)
--	drop view dbo.vPrescriber
--GO

/** PROVIDES INFO & Primary Voice Telephone # FOR EACH INDIVIDUAL PRESCRIBER **/
/* MS SQLSERVER numeric values
		signed:
 			INTEGER = +-2,000,000,000
			SMALL INT = +- 32,767
			FLOAT = precision to 53 digits
		unsigned:
			TINY INT = 0-255
 */

--CREATE VIEW dbo.vPrescriber
--AS




SELECT [NO]
      ,[PH_LAST]
      ,[PH_FIRST]
            ,NULL AS Middle_Initial
      ,[PH_ADDRESS]
      ,[PH_ADDR2]
      ,[PH_SPEC]
      ,[PH_ORG]
      ,[PH_CITY]
      ,[PH_STATE]
      ,[PH_ZIP]
      ,NULL AS Zip_Plus_4
      ,NULL AS Area_Code
      ,[PH_PHONE]
      ,[PH_PHONEXT]
      ,[PH_DEA]
      ,NULL AS DEA_Suffix
      ,[PROFDESIG]
      ,DELFLAG
  FROM [CPRPROD].[dbo].[DOCTORS]
  
  
  
  
  
