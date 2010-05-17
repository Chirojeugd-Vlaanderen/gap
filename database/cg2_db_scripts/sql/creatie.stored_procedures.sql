:r ../dataconversie/EnkelCijfers.sql
GO
:r ../dataconversie/UcFirst.sql
GO
:r ../dataconversie/NieuweGroepUitKipadmin.sql
GO
:r 	../dataconversie/GroepsPersonenUitKipadmin.sql
GO
:r ../dataconversie/groepLeegMaken.sql
GO
:r ../dataconversie/groepVolledigVerwijderen.sql
GO
:r ../dataconversie/GenereerGebruikersNaam.sql
GO
:r ../dataconversie/VerwijderAccenten.sql
GO


CREATE FUNCTION core.ufnSoundEx
(
  @tekst VARCHAR(MAX)
)
RETURNS VARCHAR(8) AS
-- Doel: UDF die enkel soundex uitvoert, zodat we deze kunnen gebruiken in
-- entity framework
BEGIN
  RETURN
  (
	SELECT SOUNDEX(@tekst)
  )
END;
GO

GRANT EXECUTE ON core.ufnSoundEx TO GapRole
GO

--------------------------------------------
-- nog een view

CREATE VIEW [auth].[vGebruikers] AS
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
GO