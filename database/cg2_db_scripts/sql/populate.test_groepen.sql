
-- Dit script maakt alle Chiro groepen aan, en geeft de nodige GAV's de 
-- nodige rechten.

-- Test groep voor WatiN tests.
DECLARE @testGroepID AS INT;

INSERT INTO grp.Groep(Naam, Code) VALUES('St-WebAutomatedTestIndotNet', 'WatiN');
SET @testGroepID = scope_identity();

INSERT INTO grp.ChiroGroep (ChiroGroepID) VALUES(@testGroepID);
INSERT INTO grp.GroepsWerkJaar(WerkJaar, GroepID) VALUES ('2009', @testGroepID);


-- Alle gekende accounts moeten de WatiN test kunnen runnen, 
-- Maar 'Yvonne', 'Yvette' en 'nietgebruiken' mag ik niet toevoegen.

DECLARE Cursor_Accounts CURSOR FOR SELECT Login FROM auth.Gav;
OPEN Cursor_Accounts;
DECLARE @Read_Login VARCHAR(40);
DECLARE @GavID AS INT;
FETCH NEXT FROM Cursor_Accounts into @Read_Login;
WHILE @@FETCH_STATUS = 0
BEGIN 	  
  IF NOT (@Read_Login = 'Yvonne' or @Read_Login = 'Yvette' or @Read_Login = 'nietgebruiken')
  BEGIN
    SET @GavID = (SELECT GavID FROM auth.Gav  WHERE Login=@Read_Login);
    INSERT INTO auth.GebruikersRecht(GavID, GroepID) VALUES(@GavID, @testGroepID);  
  END
  FETCH NEXT FROM Cursor_Accounts into @Read_Login;
END
CLOSE Cursor_Accounts;
DEALLOCATE Cursor_Accounts;

GO
	