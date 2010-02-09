
-- Dit script maakt alle Chiro groepen aan, en geeft de nodige GAV's de 
-- nodige rechten.

-- Test groep voor WatiN tests.

DECLARE @testGroepCode AS VARCHAR(10); SET @testGroepCode = 'WatiN';
DECLARE @testGroepID AS INT;


IF NOT EXISTS (SELECT 1 FROM grp.Groep WHERE Code=@testGroepCode)
BEGIN
	INSERT INTO grp.Groep(Naam, Code) VALUES('St-WebAutomatedTestIndotNet', 'WatiN');
	SET @testGroepID = scope_identity();

	INSERT INTO grp.ChiroGroep (ChiroGroepID) VALUES(@testGroepID);
	INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID) VALUES ('2009', @testGroepID);
END
ELSE
BEGIN
	SET @testGroepID = (SELECT GroepID FROM grp.Groep WHERE Code=@testGroepCode)
END

-- Alle gekende accounts moeten de WatiN test kunnen runnen, 
-- Maar 'Yvonne', 'Yvette' en 'nietgebruiken' mag ik niet toevoegen.

INSERT INTO auth.GebruikersRecht (GavID, GroepID)
SELECT g1.GavID, @testGroepID FROM auth.Gav g1
WHERE g1.Login NOT IN ('Yvonne', 'Yvette', 'nietgebruiken')
AND NOT EXISTS (SELECT 1 FROM auth.GebruikersRecht g2 WHERE g2.GavID = g1.GavID AND g2.GroepID = @testGroepID)

PRINT 'public const int WATINGROEPID = ' + CAST(@testGroepID AS VARCHAR(10)) + ';';

GO
	
