use gap_dev;
go
create view diag.vVerlorenBivakken as
select cg.StamNr, g.naam as GroepsNaam, cg.Gemeente, u.naam as BivakNaam, u.UitstapID
from biv.uitstap u
join grp.groepswerkjaar gwj on u.groepswerkjaarid=gwj.groepswerkjaarid
join grp.groep g on gwj.groepid=g.groepid
join kipadmin_dev.dbo.huidigWerkJaar wj on gwj.WerkJaar = wj.WerkJaar
join kipadmin_dev.grp.chirogroep cg on cg.stamnr=g.code collate SQL_Latin1_General_CP1_CI_AS
left outer join kipadmin_dev.biv.bivakaangifte bo on cg.groepID = bo.groepID and bo.werkjaar=gwj.werkjaar
-- bivakoverzicht is wat de delphi client toont (-> enkel aangiften met adres & contactpersoon)
-- bivakaangifte zou moeten overeenkomen met uitstap in GAP.
where u.IsBivak=1 and bo.werkjaar is null and u.DatumVan >= kipadmin_dev.dbo.Date(gwj.werkjaar, 9, 1)
										  and u.DatumVan <= kipadmin_dev.dbo.Date(gwj.werkjaar + 1, 9, 1)

