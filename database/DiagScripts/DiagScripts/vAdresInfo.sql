use gap
go
create view adr.vPersoonsAdresInfo as
-- voorkeursadres(sen) van een persoon
-- (kunnen er meerdere zijn als een persoon aan meerdere groepen is gekoppeld)
select  p.adnummer,gp.gelieerdepersoonid,gp.persoonid,pa.persoonsadresid, p.naam, p.voornaam,
case when bua.buitenlandsadresid is null then sn.naam else bua.straat end as straatnaam,
a.huisnr, a.bus, 
isnull(bua.postnummer, sn.postnummer) as postnr,
isnull(bua.postcode, '') as postcode,
case when bua.buitenlandsadresid is null then wp.naam else bua.woonplaats end as woonplaats,
isnull(l.naam,'') as land, pa.versie
 from pers.persoon p
join pers.gelieerdepersoon gp on p.persoonid = gp.persoonid
left outer join pers.persoonsadres pa on gp.voorkeursadresid=pa.persoonsadresid
left outer join adr.adres a on pa.adresid=a.adresid
left outer join adr.belgischadres bea on bea.belgischadresid=a.adresid
left outer join adr.buitenlandsadres bua on bua.buitenlandsadresid=a.adresid
left outer join adr.straatnaam sn on bea.straatnaamid=sn.straatnaamid
left outer join adr.woonplaats wp on bea.woonplaatsid=wp.woonplaatsid
left outer join adr.land l on bua.landid=l.landid