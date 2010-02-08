---- procedure om te bekijken of we van iedereen een groep hebben
--
--declare @adnrs table(AdNr int);
--declare @testers table(AdNr int, GroepID int, StamNr varchar(10));
--
--insert into @adnrs
--select distinct l.adnr
--from kipHeeftFunctie hf 
--join lid.lid l on l.ID = hf.LeidKad
--where (hf.functie=216 or hf.functie=177) and l.werkjaar=2009;
--
--insert into @testers
--select distinct ll.adnr, ll.groepID, cg.Stamnr
--from kipHeeftFunctie hf 
--join lid.lid l on l.ID = hf.LeidKad
--join lid.lid ll on l.AdNr = ll.AdNr
--join grp.Chirogroep cg on ll.GroepID = cg.GroepID
--where (hf.functie=216 or hf.functie=177) and l.werkjaar=2009 and cg.Type = 'L';
--
--select * from @adnrs n where not exists
--(select * from @testers t where t.adnr=n.adnr)

CREATE VIEW auth.vGebruikers AS
SELECT DISTINCT ll.Adnr, p.Voornaam, p.Naam, ci.Info AS MailAdres, auth.ufnGenereerGebruikersNaam(p.Voornaam, p.Naam) as Login, cg.StamNr
, CASE hf.functie WHEN 177 THEN 'KIPDORP' ELSE 'CHIROPUBLIC' END AS Domain
FROM Kipadmin.dbo.kipHeeftFunctie hf 
JOIN Kipadmin.lid.lid l ON l.ID = hf.LeidKad
JOIN Kipadmin.lid.lid ll ON l.AdNr = ll.AdNr
JOIN Kipadmin.grp.Chirogroep cg ON ll.GroepID = cg.GroepID
JOIN Kipadmin.dbo.kipPersoon p ON p.AdNr = ll.AdNr 
JOIN Kipadmin.dbo.kipContactInfo ci ON ci.AdNr = p.Adnr AND ContactTypeID = 3 AND VolgNr = 1
JOIN Kipadmin.dbo.HuidigWerkJaar wj ON l.werkJaar = wj.werkJaar
WHERE (hf.functie=216 OR hf.functie=177) AND cg.Type = 'L';
