-- STAP 4: migreer database van live naar dev

-- Hier moeten alle aanpassingen komen aan de database die nodig zijn om de live-db
-- om te zetten naar de db die gebruikt wordt in dev.

USE gap_tst;

ALTER TABLE lid.Lid ADD UitschrijfDatum DATETIME NULL;
GO

UPDATE lid.Lid SET UitschrijfDatum = '2012-08-01' WHERE NonActief=1;

DROP INDEX [IDX_Lid_EindeInstapPeriode_NonActief] ON [lid].[Lid]

DROP INDEX [IDX_Lid_GroepsWerkJaarID_NonActief] ON [lid].[Lid]

ALTER TABLE lid.Lid DROP COLUMN NonActief;

ALTER TABLE lid.Lid ADD NonActief AS (CASE WHEN UitschrijfDatum IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END);

CREATE NONCLUSTERED INDEX [IDX_Lid_EindeInstapPeriode_UitschrijfDatum] ON [lid].[Lid]
(
	[EindeInstapPeriode] ASC,
	[UitschrijfDatum] ASC
)

CREATE NONCLUSTERED INDEX [IDX_Lid_GroepsWerkJaarID_UitschrijfDatum] ON [lid].[Lid]
(
	[GroepsWerkjaarID] ASC,
	[UitschrijfDatum] ASC
)
