use kipadmin
go

declare @adNr as integer; set @adNr=_ADNUMMER_;

select top 1 ki.info
from kipContactInfo ki
where ki.adnr=@adNr and contacttypeid=3
order by GeenMailings
go
