use kipadmin

CREATE SCHEMA biv;


ALTER TABLE dbo.kipBivak ADD
	b_AangifteID int NULL,
	u_AangifteID int NULL,
	s_AangifteID int NULL,
	GroepID int NULL,
	GroepID2 int NULL;
	
CREATE INDEX IDX_Bivak_BAangifteID ON kipBivak(B_AangifteID);
CREATE INDEX IDX_Bivak_UAangifteID ON kipBivak(U_AangifteID);
CREATE INDEX IDX_Bivak_SAangifteID ON kipBivak(S_AangifteID);

UPDATE b
SET b.GroepID = cg.GroepID
FROM dbo.kipBivak b JOIN grp.ChiroGroep cg on b.StamNr = cg.StamNr

UPDATE b
SET b.GroepID2 = cg.GroepID
FROM dbo.kipBivak b JOIN grp.ChiroGroep cg on b.StamNr2 = cg.StamNr

ALTER TABLE dbo.kipBivak DROP CONSTRAINT FK__kipBivak__STAMNR__36DC0ACC
ALTER TABLE dbo.kipBivak DROP CONSTRAINT FK__kipBivak__STAMNR__39B87777

ALTER TABLE dbo.kipBivak ADD CONSTRAINT FK_Bivak_Groep FOREIGN KEY(GroepID) REFERENCES grp.Groep(GroepID);
ALTER TABLE dbo.kipBivak ADD CONSTRAINT FK_Bivak_Groep2 FOREIGN KEY(GroepID2) REFERENCES grp.Groep(GroepID);

ALTER TABLE dbo.kipBivak DROP CONSTRAINT IX_kipBivak

ALTER TABLE dbo.kipBivak ALTER COLUMN GroepID INT NOT NULL

CREATE UNIQUE INDEX AK_Bivak_GroepID_WerkJaar ON dbo.kipBivak(GroepID, WerkJaar)

ALTER TABLE dbo.kipBivak DROP COLUMN StamNr;
ALTER TABLE dbo.kipBivak DROP COLUMN StamNr2;


EXEC sp_Rename 'dbo.kipBivak', 'BivakOverzicht'
GO

ALTER SCHEMA biv TRANSFER dbo.BivakOverzicht;
GO

CREATE VIEW dbo.kipBivak AS
SELECT bo.*, cg1.StamNr AS StamNr, cg2.StamNr AS StamNr2
FROM biv.BivakOverzicht bo
JOIN grp.ChiroGroep cg1 on bo.GroepID = cg1.GroepID
LEFT OUTER JOIN grp.ChiroGroep cg2 on bo.GroepID2 = cg2.GroepID


CREATE TABLE biv.BivakAangifte(
	BivakAangifteID INT NOT NULL IDENTITY(1,1),
	GapUitstapID INT NOT NULL,
	GroepID INT NOT NULL,
	WerkJaar INT NOT NULL,
	DatumVan DATETIME NOT NULL,
	DatumTot DATETIME NOT NULL,
	BivakNaam VARCHAR(120) NULL,
	Opmerking TEXT NULL,	
	BivakPlaatsNaam VARCHAR(80),
	AdresID INT NULL, -- nullable omdat de groepen dit mogelijk niet direct in orde brengen
	ContactAD INT NULL, -- idem
	PRIMARY KEY(BivakAangifteID));
GO

ALTER TABLE biv.BivakAangifte ADD CONSTRAINT FK_BivakAangifte_Groep FOREIGN KEY(GroepID) REFERENCES grp.Groep(GroepID);
ALTER TABLE biv.BivakAangifte ADD CONSTRAINT FK_BivakAangifte_Adres FOREIGN KEY(AdresID) REFERENCES dbo.kipAdres(AdresID);
ALTER TABLE biv.BivakAangifte ADD CONSTRAINT FK_BivakAangifte_Persoon FOREIGN KEY(ContactAD) REFERENCES kipPersoon(AdNr);
	
CREATE INDEX IDX_BivakAangifte_Groep_WerkJaar ON biv.BivakAangifte(GroepID, WerkJaar);
CREATE INDEX IDX_BivakAangifte_Adres ON biv.BivakAangifte(AdresID);

ALTER TABLE biv.BivakOverzicht ADD CONSTRAINT FK_BivakOverzicht_BAangifte FOREIGN KEY(b_AangifteID) REFERENCES biv.BivakAangifte(BivakAangifteID);
ALTER TABLE biv.BivakOverzicht ADD CONSTRAINT FK_BivakOverzicht_UAangifte FOREIGN KEY(u_AangifteID) REFERENCES biv.BivakAangifte(BivakAangifteID);
ALTER TABLE biv.BivakOverzicht ADD CONSTRAINT FK_BivakOverzicht_SAangifte FOREIGN KEY(s_AangifteID) REFERENCES biv.BivakAangifte(BivakAangifteID);


GRANT INSERT, UPDATE, SELECT ON biv.BivakOverzicht TO KipSyncRole
GO
	
GRANT INSERT, UPDATE, SELECT, DELETE ON biv.BivakAangifte TO KipSyncRole
GO
	
-- Omdat een bivak in GAP los ingegeven wordt van de plaats, komen er ook bivakken zonder plaats
-- door naar Kipadmin.  Dat moeten we ondersteunen.

ALTER TABLE biv.BivakOverzicht ALTER COLUMN B_Straat VARCHAR(40) NULL
ALTER TABLE biv.BivakOverzicht ALTER COLUMN B_Postnr VARCHAR(8) NULL
ALTER TABLE biv.BivakOverzicht ALTER COLUMN B_Gemeente VARCHAR(40) NULL