CREATE PROCEDURE data.spGroepVerwijderen @stamNr VARCHAR(10) AS
-- Verwijdert alle sporen van een groep
-- en doet daarna ook opkuis in de tabelentries die nergens aan een groep
-- gekoppeld zijn.

DECLARE @groepID AS INT;

SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@StamNr)

-- verwijder eerst groep en rechtstreekse koppelingen

DELETE leia
FROM grp.GroepsWerkJaar gwj 
JOIN lid.Lid l on gwj.GroepsWerkJaarID = l.GroepsWerkJaarID
JOIN lid.Leiding lei on l.LidID = lei.LeidingID
JOIN lid.LeidingInAfdelingsJaar leia on lei.LeidingID = leia.LeidingID
WHERE gwj.GroepID = @groepID

DELETE lei 
FROM grp.GroepsWerkJaar gwj 
JOIN lid.Lid l on gwj.GroepsWerkJaarID = l.GroepsWerkJaarID
JOIN lid.Leiding lei on l.LidID = lei.LeidingID
WHERE gwj.GroepID = @groepID

DELETE kin
FROM grp.GroepsWerkJaar gwj 
JOIN lid.Lid l on gwj.GroepsWerkJaarID = l.GroepsWerkJaarID
JOIN lid.Kind kin on l.LidID = kin.KindID
WHERE gwj.GroepID = @groepID

DELETE lf 
FROM grp.GroepsWerkJaar gwj 
JOIN lid.Lid l on gwj.GroepsWerkJaarID = l.GroepsWerkJaarID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gwj.GroepID = @groepID

DELETE l FROM grp.GroepsWerkJaar gwj JOIN lid.Lid l on gwj.GroepsWerkJaarID = l.GroepsWerkJaarID
WHERE gwj.GroepID = @groepID

DELETE aj
FROM grp.GroepsWerkJaar gwj
JOIN lid.AfdelingsJaar aj on gwj.GroepsWerkJaarID = aj.GroepsWerkJaarID
WHERE gwj.GroepID = @groepID

DELETE cv
FROM pers.GelieerdePersoon gp
JOIN pers.CommunicatieVorm cv ON gp.GelieerdePersoonID = cv.GelieerdePersoonID
WHERE gp.GroepID = @groepID

DELETE pc
FROM pers.GelieerdePersoon gp
JOIN pers.PersoonsCategorie pc ON gp.GelieerdePersoonID = pc.GelieerdePersoonID
WHERE gp.GroepID = @groepID

DELETE FROM pers.GelieerdePersoon WHERE GroepID = @groepID

DELETE FROM grp.GroepsWerkJaar WHERE GroepID = @groepID

DELETE FROM core.Categorie WHERE GroepID = @groepID

DELETE FROM grp.ChiroGroep WHERE ChiroGroepID = @groepID

DELETE FROM lid.Afdeling WHERE GroepID = @groepID

DELETE FROM auth.GebruikersRecht WHERE GroepID = @groepID

DELETE FROM grp.Groep WHERE GroepID = @groepID


-- verwijder personen die aan geen enkele groep meer gekoppeld zijn

DELETE pa FROM pers.PersoonsAdres pa WHERE NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon gp WHERE gp.PersoonID = pa.PersoonID)
DELETE p FROM pers.Persoon p WHERE NOT EXISTS (SELECT 1 FROM pers.GelieerdePersoon gp WHERE gp.PersoonID = p.PersoonID)

-- verwijder adressen die aan geen enkele persoon meer gekoppeld zijn

DELETE a FROM adr.Adres a WHERE NOT EXISTS (SELECT 1 FROM pers.PersoonsAdres pa WHERE pa.AdresID = a.AdresID)


