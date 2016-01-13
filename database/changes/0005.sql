USE gap_local;

-- Bewaar op lidniveau wie zeker een membership heeft in Civi.
ALTER TABLE lid.Lid
ADD IsAangesloten BIT NOT NULL DEFAULT 0;
GO

-- Neem bij op in index.

DROP INDEX [IDX_Lid_EindeInstapPeriode_UitschrijfDatum] ON [lid].[Lid]
GO

CREATE NONCLUSTERED INDEX [IDX_Lid_EindeInstapPeriode_UitschrijfDatum_IsAangesloten] ON [lid].[Lid]
(
	[EindeInstapPeriode] ASC,
	[UitschrijfDatum] ASC,
	[IsAangesloten] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

-- Leden van voor 2015-2016 mogen allemaal IsAangesloten = 1 krijgen.

UPDATE l
SET IsAangesloten = 1
FROM lid.Lid l
JOIN grp.GroepsWerkJaar gwj ON l.GroepsWerkJaarID = gwj.GroepsWerkJaarID
WHERE gwj.WerkJaar < 2015

-- Zet IsAangesloten voor alle leden van 2015-2016 waarvan
--   het werkjaar van laatste aansluiting voor de persoon 2015-2016 is
--   de persoon niet aangesloten is in meer dan 1 ploeg
--   de persoon geen lid was van een nationale ploeg in de oude kipadmin

UPDATE l1
SET l1.IsAangesloten = 1
FROM lid.Lid l1
JOIN pers.GelieerdePersoon gp1 ON l1.GelieerdePersoonID = gp1.GelieerdePersoonID
JOIN pers.Persoon p1 ON gp1.PersoonID = p1.PersoonID
JOIN grp.GroepsWerkJaar gwj ON l1.GroepsWerkjaarID = gwj.GroepsWerkJaarID AND p1.LaatsteMembership = gwj.WerkJaar
JOIN (
	SELECT p.PersoonID, COUNT(*) AS aantalAansluitingen
	FROM pers.Persoon p
	JOIN pers.GelieerdePersoon gp ON p.PersoonID = gp.PersoonID
	JOIN lid.Lid l ON gp.GelieerdePersoonID = l.GelieerdePersoonID
	JOIN grp.GroepsWerkJaar gwj ON l.GroepsWerkjaarID = gwj.GroepsWerkJaarID
	WHERE gwj.WerkJaar = 2015
	GROUP BY p.PersoonId
	) aa ON p1.PersoonID = aa.PersoonID AND aa.aantalAansluitingen = 1
LEFT OUTER JOIN kipadmin.lid.Lid kl ON kl.ADNR = p1.AdNummer AND kl.GroepID = 1044 AND kl.werkjaar = gwj.WerkJaar
LEFT OUTER JOIN kipadmin.dbo.kipHeeftFunctie khf ON kl.id = khf.leidkad
LEFT OUTER JOIN kipadmin.dbo.kipFunctie kf ON khf.functie = kf.id AND kf.PloegInCivi = 1
WHERE gwj.WerkJaar = 2015 AND kf.id IS NULL

-- Door alle leden uit meer dan 1 ploeg geen 'IsAangesloten' te geven, geven we
-- in principe een aantal lidrelaties ten onrechte geen 'IsAangesloten'. Maar
-- dat is niet erg, want civisync zal wel merken dat er toch een membership bestaat.
GO

ALTER TABLE pers.Persoon DROP COLUMN LaatsteMembership;
