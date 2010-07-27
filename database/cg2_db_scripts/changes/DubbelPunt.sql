
ALTER TABLE pers.Persoon ADD
	DubbelPuntAbonnement bit NOT NULL CONSTRAINT DF_Persoon_DubbelPuntAbonnement DEFAULT 0
GO

ALTER TABLE lid.Leiding
	DROP COLUMN DubbelPuntAbonnement
GO