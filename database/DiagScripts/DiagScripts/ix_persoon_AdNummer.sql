USE gap
GO

CREATE INDEX IX_Persoon_AdNummer ON pers.Persoon(AdNummer) INCLUDE (PersoonID)
GO
