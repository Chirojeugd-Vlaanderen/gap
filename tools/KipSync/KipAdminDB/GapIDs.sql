ALTER TABLE dbo.kipPersoon ADD
	GapID int NULL;
GO

CREATE INDEX IX_Persoon_GapID ON dbo.KipPersoon(GapID)
GO

