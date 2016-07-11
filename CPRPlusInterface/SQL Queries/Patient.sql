CREATE VIEW dbo.vMOTPatient 
AS 

with cte_PatIns1
as 
(select mrn, rank, Insurance, policy from patins where rank = 1  and delflag = 0 and Status = 'Active')
, 
cte_PatIns2 
as
(select mrn, rank, Insurance, policy from patins where rank = 2  and delflag = 0 and Status = 'Active')
,
cte_Medicare
as
(select mrn, rank, Insurance, policy from patins where delflag = 0 and Status = 'Active' and Payor = 'MEDICARE')
---When there are patients with multiple medicare policies.  This will create duplicate records in the view.
,
cte_MEDICAID
as
(select mrn, rank, Insurance, policy from patins where delflag = 0 and Status = 'Active' and Payor = 'MEDICAID')


select 
		
		hr.mrn as rxSys_PatID,
		LAST_NAME as LastName,
		FIRST_NAME as FirstName,
		ADDRESS as Address1,
		CITY,
		STATE,
		ZIP ,
		PHONE as Phone1,
		WORKPHONE as WorkPhone,
		PAT_TYPE as RxSys_LocID,
		pat_stat as status,
		ssn,
		Allergies,
		Diet,
		DOB,
		Height,
		Weight,
		MailName as ResponsibleName,
		cte_patins1.Insurance as InsName,
		cte_patins1.policy as InsPNo,
		cte_patins2.Insurance as AltInsName,
		cte_patins2.policy as AltInsPNo,
		cte_Medicare.policy as MCareNum,
		cte_Medicaid.policy as McCaidNum,
		sex as Gender

from HR 
---where delflag = 0 
left outer join cte_patins1 on
hr.mrn = cte_patins1.mrn
left outer join cte_patins2 on
hr.mrn = cte_patins2.mrn
left outer join cte_Medicare on
hr.mrn = cte_Medicare.mrn
left outer join cte_MEDICAID on
hr.mrn = cte_MEDICAID.mrn
where hr.delflag = 0 

GO