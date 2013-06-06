use gap_dev;
go
create view diag.vVerlorenBivakken as
select cg.StamNr, g.naam as GroepsNaam, cg.Gemeente, u.naam as BivakNaam, u.UitstapID
from biv.uitstap u
join grp.groepswerkjaar gwj on u.groepswerkjaarid=gwj.groepswerkjaarid
join grp.groep g on gwj.groepid=g.groepid
join kipadmin_dev.dbo.huidigWerkJaar wj on gwj.WerkJaar = wj.WerkJaar
join kipadmin_dev.grp.chirogroep cg on cg.stamnr=g.code collate SQL_Latin1_General_CP1_CI_AS
left outer join kipadmin_dev.biv.bivakoverzicht bo on cg.groepID = bo.groepID and bo.werkjaar=gwj.werkjaar
-- bivakoverzicht bevat normaal alle bivakken
-- bivakaangifte is wat de delphi client toont.
where u.IsBivak=1 and bo.werkjaar is null

