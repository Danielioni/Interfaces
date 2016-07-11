CREATE VIEW dbo.vMOTStore 
AS 
Select


SEIVTYPE as RxSys_StoreID,
CLIENTNAME as StoreName,
ADDR1 as Address1,
ADDR2 as Address2,
CITY,
ST as State,
ZIP,
PHONE,
FAX,
DEANUMBER as DEANum

from client

Go