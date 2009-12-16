
-- Dit script maakt alle Chiro groepen aan, en geeft de nodige GAV's de 
-- nodige rechten.

-- Test groep voor WatiN tests.
DECLARE @testGroepID AS INT;
DECLARE @GavID AS INT;

INSERT INTO grp.Groep(Naam, Code) VALUES('St-WebAutomatedTestIndotNet', 'WatiN');
SET @testGroepID = scope_identity();

INSERT INTO grp.ChiroGroep (ChiroGroepID) VALUES(@testGroepID);
INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID) VALUES ('2009', @testGroepID);


-- Hier moeten all test accounts staan.
SET @GavID = (SELECT GavID FROM auth.Gav  WHERE Login='KIPDORP\meersko');
INSERT INTO auth.GebruikersRecht(GavID, GroepID) VALUES(@GavID, @testGroepID);

GO
	