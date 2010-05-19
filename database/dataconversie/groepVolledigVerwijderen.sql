CREATE PROCEDURE data.spGroepVerwijderen @stamNr VARCHAR(10) AS
-- Verwijdert alle sporen van een groep
-- en doet daarna ook opkuis in de tabelentries die nergens aan een groep
-- gekoppeld zijn.

DECLARE @groepID AS INT;

SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@StamNr)

exec data.spGroepLeegMaken @stamNr

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


