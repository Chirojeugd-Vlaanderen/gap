USE Chirogroep_Vervljo
go

DECLARE @stamNr AS VARCHAR(10)
SET @stamNr = 'MG /0111'

DECLARE @groepID AS INT;

SET @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@StamNr)

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
