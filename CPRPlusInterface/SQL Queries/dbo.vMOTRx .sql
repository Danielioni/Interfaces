CREATE VIEW dbo.vMOTRx 
AS 

select 

		SCRIPTEXT as RxSys_RxNum,
		LINK as RxSys_PatID,
		DOCNO as RxSys_DocID,
		NO as RxSys_DrugID,
		POSIG1 + ' ' + POSIG2 + ' ' + POSIG3 as SIG,
		ORIG_RX as RxStartDate,
		END_RX as RxStopDate,
		NOTES as Comments,
		QTYALLOWED as Refills,
		BAGSDISP as QtyPerDose,
		BAGSDISP as QtyDIspensed
---	The dose days and times will not be populdated in CPR+ as of 7/7/16
---		DAY_1, DAY_2, DAY_3, DAY_4, DAY_5, DAY_6, DAY_7
---		DOSETIME as DoseTimesQtys

from labels
where labels.delflag = 0  

go