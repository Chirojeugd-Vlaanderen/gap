
-- ! EERST DE GEGEVENS VOOR UNIT TESTS IMPORTEREN

-- Dit script maakt alle Chiro groepen aan, en geeft de nodige GAV's de 
-- nodige rechten.

-- Test groep voor WatiN tests.

DECLARE @testGroepCode AS VARCHAR(10); SET @testGroepCode = 'WatiN';
DECLARE @testGroepID AS INT;

DECLARE @testGewestID AS INT; SET @testGewestID = (SELECT GroepID FROM grp.Groep WHERE Code='TST/0000');

IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@testGroepCode)
BEGIN
	INSERT INTO grp.Groep(Naam, Code) VALUES('St-WebAutomatedTestIndotNet', 'WatiN');
	SET @testGroepID = scope_identity();

	INSERT INTO grp.ChiroGroep (ChiroGroepID, KaderGroepID) VALUES(@testGroepID, @testGewestID);
	INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID) VALUES ('2009', @testGroepID);
END
ELSE
BEGIN
	SET @testGroepID = (SELECT GroepID FROM grp.Groep WHERE Code=@testGroepCode)
END


-- voeg hier je gebruikersnaam toe als je de watin tests wil runnen
EXEC auth.SpGebruikersRechtToekennen 'watin', 'lap-jve\johan'


PRINT 'public const int WATINGROEPID = ' + CAST(@testGroepID AS VARCHAR(10)) + ';';

GO
	