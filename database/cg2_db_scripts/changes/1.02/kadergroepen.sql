USE gap;
GO

-- Creer tabel voor kadergroep

CREATE TABLE grp.KaderGroep(
	KaderGroepID INT NOT NULL, 
	Niveau INT NOT NULL,
	ParentID INT NULL,
	Versie TIMESTAMP NULL,
	CONSTRAINT PK_KaderGroep PRIMARY KEY (KaderGroepID),
	CONSTRAINT FK_KaderGroep_Groep FOREIGN KEY (KaderGroepID) REFERENCES grp.Groep(GroepID),
	CONSTRAINT FK_KaderGroep_KaderGroep FOREIGN KEY (ParentID) REFERENCES grp.KaderGroep(KaderGroepID));
GO

GRANT SELECT ON grp.KaderGroep TO GapRole
GO


-- Chirogroep moet kadergroep als parent hebben.  Voorlopig echter nullable,
-- omdat de kadergroepen nog niet geimporteerd zijn.

ALTER TABLE grp.ChiroGroep ADD
	KaderGroepID int NULL 
GO

ALTER TABLE grp.ChiroGroep ADD CONSTRAINT
	FK_ChiroGroep_KaderGroep FOREIGN KEY (KaderGroepID) REFERENCES grp.KaderGroep(KaderGroepID)
GO

-- importeer kadergroepen naar groepentabel

INSERT INTO grp.Groep(Code, Naam, OprichtingsJaar)
SELECT cg.StamNr, g.Naam, YEAR(g.StartDatum)
FROM Kipadmin.grp.Groep g JOIN Kipadmin.grp.ChiroGroep cg ON g.GroepID = cg.GroepID
WHERE (cg.Type='G' or cg.Type='V') and g.StopDatum is null
GO

-- importeer info voor kadergroepentabel

INSERT INTO grp.KaderGroep(KaderGroepID, Niveau, ParentID)
SELECT 
	g.GroepID AS KaderGroepID,
	CASE kcg.Type WHEN 'G' THEN 8 WHEN 'V' THEN 32 END AS Niveau,
	CASE kcg.Type WHEN 'G' THEN vg.GroepID ELSE NULL END AS ParentID
FROM Kipadmin.grp.Groep kg JOIN Kipadmin.grp.ChiroGroep kcg ON kg.GroepID = kcg.GroepID
JOIN grp.Groep g ON kcg.StamNr = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
LEFT OUTER JOIN Kipadmin.dbo.Verbond kv ON kcg.VerbondNr = kv.Nr
LEFT OUTER JOIN grp.Groep vg on kv.StamNr = vg.Code COLLATE SQL_Latin1_General_CP1_CI_AS
WHERE (kcg.Type='G' or kcg.Type='V') and kg.StopDatum is null
GO

-- koppel plaatselijke (Chiro)groepen aan hun gewest.

UPDATE cg
SET cg.KaderGroepID = gg.GroepID
FROM Kipadmin.grp.Groep kg JOIN Kipadmin.grp.ChiroGroep kcg ON kg.GroepID = kcg.GroepID
JOIN grp.Groep g ON kcg.StamNr = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN grp.ChiroGroep cg ON g.GroepID = cg.ChiroGroepID
LEFT OUTER JOIN Kipadmin.dbo.Gewest kgew ON kcg.GewestNr = kgew.Nr
LEFT OUTER JOIN grp.Groep gg on kgew.StamNr = gg.Code COLLATE SQL_Latin1_General_CP1_CI_AS
GO

-- verwijder testgroepen uit Live-db
-- Dat wil wel zeggen dat nu iedere keer de live-db terug naar de test wordt overgezet,
-- de testgroepen opnieuw moeten worden gemaakt!

EXEC data.spGroepVerwijderen 'tst/0001'
GO
EXEC data.spGroepVerwijderen 'WatiN'
GO

-- Nu iedere groep aan zijn gewest gekoppeld is, kan deze link 'geontnullabled' worden.

ALTER TABLE grp.ChiroGroep ALTER COLUMN KaderGroepID INT NOT NULL;
GO



