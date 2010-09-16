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
:r ../dataconversie/GebruikersRechtToekennen.sql
GO
:r ../dataconversie/GebruikersRechtOntnemen.sql
GO
:r ../dataconversie/PersonenOpkuisen.sql
GO
:r ../dataconversie/GroepsWerkJaarVerwijderen.sql
GO
:r ./handigheidjes/gapfusie.sql
GO
:r ./handigheidjes/dubbelePersoon.sql
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

create procedure pers.spFixVoorkeursAdres @PersoonID int as
-- DOEL: zorg ervoor dat iedere gelieerde persoon gekoppeld aan de persoon
-- met gegeven PersoonID minstens 1 voorkeursadres heeft. 
-- (aangenomen dat de persoon adressen heeft; anders gebeurt er niets)
update gp1 
set gp1.VoorkeursAdresID = nieuw.PersoonsAdresID
from pers.GelieerdePersoon gp1 join
(
select gp.GelieerdePersoonID, max(pa.PersoonsAdresID) as PersoonsAdresID 
from pers.GelieerdePersoon gp
join pers.PersoonsAdres pa on gp.PersoonID = pa.PersoonID
where gp.persoonID = @PersoonID
	and gp.VoorkeursAdresID is null
group by gp.GelieerdePersoonID
) nieuw on gp1.GelieerdePersoonID = nieuw.GelieerdePersoonID

GO

GRANT EXECUTE ON pers.spFixVoorkeursAdres TO GapRole
GO

CREATE PROCEDURE pers.spAdNummerZetten @PersoonID int, @AdNummer INT AS
-- DOEL: Het AD-Nummer van een gelieerde persoon zetten
UPDATE pers.Persoon SET AdNummer=@AdNummer, AdInAanvraag=0 WHERE PersoonID=@PersoonID
GO

GRANT EXECUTE ON pers.AdNummerZetten TO KipSyncRole
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
