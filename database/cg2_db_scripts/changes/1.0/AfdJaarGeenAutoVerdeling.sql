ALTER TABLE lid.AfdelingsJaar ADD
	GeenAutoVerdeling bit NOT NULL CONSTRAINT DF_AfdelingsJaar_GeenAutoVerdeling DEFAULT 0
GO

