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
SELECT 
	p.PersoonID
	, p.VoorNaam
	, p.Naam
	, pc.Categorieen
	, s.Naam AS StraatNaam
	, a.HuisNr
	, sg.PostNr
	, sg.Naam AS SubGemeente
	, p.GeboorteDatum
	, l.EindeInstapPeriode
	, l.NonActief
	, tel.Nummer AS TelefoonNummer
	, eml.Nummer AS EMail
FROM pers.Persoon p 
JOIN pers.vPersoonsCategorieen pc ON p.PersoonID = pc.PersoonID
JOIN pers.PersoonsAdres pa ON p.PersoonID = pa.PersoonID
JOIN adr.Adres a ON pa.AdresID = a.AdresID
JOIN adr.Straat s ON a.StraatID = s.StraatID
JOIN adr.SubGemeente sg ON a.SubGemeenteID = sg.SubGemeenteID
LEFT OUTER JOIN lid.Lid l ON p.PersoonID = l.PersoonID
LEFT OUTER JOIN grp.GroepsWerkJaar gw ON l.GroepsWerkJaarID = gw.GroepsWerkJaarID
LEFT OUTER JOIN core.vHuidigWerkJaar hw ON gw.WerkJaar = hw.WerkJaar
LEFT OUTER JOIN pers.CommunicatieVorm tel ON p.PersoonID = tel.PersoonID AND tel.CommunicatieTypeID=1 AND tel.Voorkeur=1
LEFT OUTER JOIN pers.CommunicatieVorm eml ON p.PersoonID = eml.PersoonID AND eml.CommunicatieTypeID=3 AND eml.Voorkeur=1
WHERE pa.IsStandaard = 1 



CREATE VIEW core.vHuidigWerkJaar
AS
SELECT     2000 + CASE WHEN dateadd(year, datediff(year, '2000-08-08', GetDate()), '2000-08-08') > GetDate() THEN datediff(year, 
                      '2000-08-08', GetDate()) - 1 ELSE datediff(year, '2000-08-08', GetDate()) END AS WerkJaar
GO

