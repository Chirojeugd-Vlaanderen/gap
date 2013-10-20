
DECLARE @Import AS TABLE(rownum int IDENTITY (1, 1) Primary key NOT NULL, AdNr int, StamNr VARCHAR(10), Voornaam VARCHAR(MAX), Naam VARCHAR(MAX), Email VARCHAR(MAX), Login VARCHAR(MAX));

INSERT INTO @Import(AdNr, StamNr, Voornaam, Naam, Email, Login)
select distinct ll.adnr, cg.Stamnr, p.VoorNaam, REPLACE(p.Naam, ' ', ''), ci.Info, LEFT(REPLACE(p.Naam, ' ', ''), 5) + LEFT(p.VoorNaam,2) as Login
from KipAdmin.dbo.kipHeeftFunctie hf 
join KipAdmin.lid.lid l on l.ID = hf.LeidKad
join KipAdmin.lid.lid ll on l.AdNr = ll.AdNr
join KipAdmin.grp.Chirogroep cg on ll.GroepID = cg.GroepID
join KipAdmin.dbo.kipPersoon p on p.AdNr = ll.AdNr 
join KipAdmin.dbo.kipContactInfo ci on ci.AdNr = p.Adnr and ContactTypeID = 3 and VolgNr = 1
where (hf.functie=216 or hf.functie=177) and l.werkjaar=2009 and cg.Type = 'L';


declare @Proc nvarchar(50)
declare @RowCnt int
declare @MaxRows int
declare @ExecSql nvarchar(255)

select @RowCnt = 1
select @Proc = 'data.spGroepUitKipadmin'



select @MaxRows=count(*) from @Import

while @RowCnt <= @MaxRows
begin
    select @ExecSql = 'exec ' + @Proc + ' ''' + StamNr + '''' from @Import where rownum = @RowCnt 
    --print @ExecSql
    execute sp_executesql @ExecSql
    Select @RowCnt = @RowCnt + 1
end