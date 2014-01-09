ALTER TABLE kipUitbreidingVerzekering ADD GroepID INT;
GO


UPDATE v
SET v.GroepID = cg.GroepID
FROM kipUitbreidingVerzekering v JOIN grp.ChiroGroep cg ON v.StamNr = cg.StamNr
GO

CREATE SCHEMA verz;
GO

CREATE TABLE verz.ExtraVerzekering(
	ExtraVerzekeringID INT NOT NULL IDENTITY(1,1),
	GroepID INT NOT NULL,
	WerkJaar INT NOT NULL,
	VolgNummer INT NOT NULL,
	Datum DATETIME NOT NULL,
	RekeningID INT,
	Wijze CHAR(1) NOT NULL,
	Noot VARCHAR(240),	-- niet meer dan 255, anders niet goed getoond op form (delphi brol)
	Stempel DATETIME,
	DoodInvaliditeit SMALLINT NULL,
	LoonVerlies SMALLINT NOT NULL);
GO


GRANT SELECT ON verz.ExtraVerzekering TO [KipSyncRole]
GO
GRANT INSERT ON verz.ExtraVerzekering TO [KipSyncRole]
GO
GRANT UPDATE ON verz.ExtraVerzekering TO [KipSyncRole]
GO

ALTER TABLE verz.ExtraVerzekering ADD CONSTRAINT PK_ExtraVerzekering PRIMARY KEY(ExtraVerzekeringID)
GO
ALTER TABLE verz.ExtraVerzekering ADD CONSTRAINT FK_ExtraVerzekering_Rekening FOREIGN KEY(RekeningID) REFERENCES Rekening(NR);
GO
ALTER TABLE verz.ExtraVerzekering ADD CONSTRAINT FK_ExtraVerzekering_Groep FOREIGN KEY(GroepID) REFERENCES grp.Groep(GroepID);
GO

INSERT INTO verz.ExtraVerzekering(GroepID, WerkJaar, VolgNummer, Datum, RekeningID, Wijze,
	Stempel, Noot, DoodInvaliditeit, Loonverlies)
SELECT cg.GroepID, WerkJaar, Verz_Nr, Datum, R_Ref, Wijze, 
	v.Stempel, v.Nota, D_I, D_I_L
FROM kipUitbreidingVerzekering v JOIN grp.ChiroGroep cg ON v.StamNr = cg.StamNr
GO

DROP TABLE kipUitbreidingVerzekering
GO

CREATE VIEW dbo.kipUitbreidingVerzekering AS
SELECT cg.StamNr, VolgNummer AS Verz_Nr, cg.Type AS GroepType,
	Datum, RekeningID AS R_Ref, Wijze, DoodInvaliditeit AS D_I, LoonVerlies AS D_I_L,
	v.Stempel,Noot AS Nota, WerkJaar, ExtraVerzekeringID AS ID
FROM verz.ExtraVerzekering v LEFT OUTER JOIN grp.ChiroGroep cg ON v.GroepID = cg.GroepID
GO


CREATE INDEX IX_ExtraVerzekering_GroepID_WerkJaar_VolgNummer ON verz.ExtraVerzekering(GroepID, WerkJaar, VolgNummer)
GO

