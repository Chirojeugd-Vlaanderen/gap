use Gap;

BEGIN TRANSACTION
GO
ALTER TABLE lid.Afdeling DROP CONSTRAINT FK_Afdeling_Groep
GO
EXECUTE sp_rename N'lid.Afdeling.GroepID', N'ChiroGroepID', 'COLUMN' 
GO
ALTER TABLE lid.Afdeling ADD CONSTRAINT
	FK_Afdeling_ChiroGroep FOREIGN KEY (ChiroGroepID) REFERENCES grp.ChiroGroep(ChiroGroepID)
GO

COMMIT
