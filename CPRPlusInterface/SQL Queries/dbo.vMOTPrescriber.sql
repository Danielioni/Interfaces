CREATE VIEW dbo.vMOTPrescriber 
AS 
SELECT
	no as RxSys_DcocID,
	ph_last as LasName,
	ph_first as FirstName,
	PH_ADDRESS as Address1,
	PH_ADDR2 as Address2,
	PH_CITY as city,
	ph_state as state,
	ph_zip as zip,
	ph_phone as phone,
	notes as comments,
	ph_dea as dea_id,
	ph_fax as fax
	

FROM doctors       
                                            
GO