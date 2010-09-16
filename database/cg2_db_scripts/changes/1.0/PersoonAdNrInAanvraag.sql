ALTER TABLE pers.Persoon ADD
	AdInAanvraag bit NOT NULL CONSTRAINT DF_Persoon_AdInAanvraag DEFAULT 0
GO