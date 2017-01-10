create view dbo.vMOTLocation
as
select 

		SEIVTYPE as RxSys_LocID,
		CPK_CLIENT as RxSys_StoreID,
		CLIENTNAME as LocationName,
		ADDR1 as Address1,
		ADDR2 as Address2,
		CITY ,
		st as STATE,
		ZIP,
		PHONE,
		Touchdate,
		Createdon

from client

Go

