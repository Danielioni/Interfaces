/******  VIEW DEFINITIONS FOR MEDICINE-ON-TIME DATA SHARE INTERFACE *****/
/****** Object:  View dbo.vPrescriber (7 of 12)
    Documentation Date: 02/20/03 8:37:59 AM ******/

if exists (select * from sysobjects where id = object_id('dbo.vPrescriber') and sysstat & 0xf = 2)
	drop view dbo.vPrescriber
GO

/** PROVIDES INFO & Primary Voice Telephone # FOR EACH INDIVIDUAL PRESCRIBER **/
/* MS SQLSERVER numeric values
		signed:
 			INTEGER = +-2,000,000,000
			SMALL INT = +- 32,767
			FLOAT = precision to 53 digits
		unsigned:
			TINY INT = 0-255
 */

CREATE VIEW dbo.vPrescriber
AS

SELECT
	--                 NOTES												FType					FLen
	p.Prescriber_ID,-- not null (unique ID)					Integer 				  				INDEX unique
	p.Last_Name, -- not null												VarChar					25				INDEX not unique
	p.First_Name, --not null												VarChar					15
	p.Middle_Initial, -- nullable										Char						 1
	p.Address_Line_1, -- nullable										VarChar					25
	p.Address_Line_2, -- nullable										VarChar					25
	p.City, -- nullable															VarChar					20
	p.State_Code, -- nullable												Char						 2
	p.Zip_Code, -- nullable													Integer
	p.Zip_Plus_4, -- nullable												Integer
	pt.Area_Code, -- nullable												Integer
	pt.Telephone_Number, -- nullable 								Integer
	pt.Extension, -- nullable												Integer
	p.DEA_Number, -- nullable												Char						 9
	p.DEA_Suffix, -- nullable												Char						 6
	p.Prescriber_Type, -- not null (DDS, MD, etc)		VarChar					 4
	p.Active_Flag -- not null	(Y,N)									Char						 1
	FROM	Prescriber p
		[JOIN] prescriber_telephone pt
	 -- provide only the primary voice phone #)
GO