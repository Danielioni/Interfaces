CREATE VIEW dbo.vMOTDrug 
AS 
select

		NO as RxSys_DrugID,
		STRENGTH as Strength,
		STRENGTHU as Unit,
		ROUTE as Route,
		SCHEDULE as DrugeSchedule,
		meddata.SHAPEDESCRIPTION as VisualDescription,
		NAME_ as Drugname,
		NDCMEDI as NDCNum,
		MCRCMNTEXT as GenericFor


from parts

left outer join meddata on 
parts.ndcmedi = meddata.ncd 
where parts.delflag = 0 
and meddata.delflag = 0 
and parttype = 'D'

Go