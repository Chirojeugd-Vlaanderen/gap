CREATE VIEW pers.vPersoonsCategorieen AS
-- Categorieen van een persoon in een lijstje met komma's ertussen
SELECT p.PersoonID, CAST(STUFF((
	SELECT ',' + c.Code
	FROM pers.PersoonsCategorie pc
	JOIN core.Categorie c ON pc.CategorieID = c.CategorieID
	WHERE pc.PersoonID = p.PersoonID
	FOR XML PATH('')),1,1,'') AS VARCHAR(MAX))
	AS Categorieen
FROM pers.Persoon p

CREATE VIEW pers.vPersoonsInfo
AS
SELECT p.PersoonID ,p.VoorNaam, p.Naam, pc.Categorieen, s.Naam AS StraatNaam, a.HuisNr, sg.PostNr, sg.Naam AS SubGemeente, p.GeboorteDatum
FROM pers.Persoon p 
JOIN pers.vPersoonsCategorieen pc ON p.PersoonID = pc.PersoonID
JOIN pers.PersoonsAdres pa ON p.PersoonID = pa.PersoonID
JOIN adr.Adres a ON pa.AdresID = a.AdresID
JOIN adr.Straat s ON a.StraatID = s.StraatID
JOIN adr.SubGemeente sg ON a.SubGemeenteID = sg.SubGemeenteID
WHERE pa.IsStandaard = 1
