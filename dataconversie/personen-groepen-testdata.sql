

TRUNCATE TABLE pers.GelieerdePersoon;
DELETE FROM pers.Persoon WHERE PersoonID > 0;
DELETE FROM grp.Groep WHERE GroepID > 0;

INSERT INTO grp.Groep(Naam, Code, OprichtingsJaar, WebSite)
SELECT
  Naam
, StamNr AS Code
, Jr_Aanslui AS OprichtingsJaar
, lower(HomePage) AS WebSite
FROM KipAdmin.dbo.Groep g
WHERE g.StamNr IN
(
  'MG /0313','OM /2107','MJ /0206','LEM/0109','LM /0707','AM /0311','MM /0408','WM /1501','MG /0111','WJ /1803'
)

INSERT INTO pers.Persoon(AdNummer, Naam, VoorNaam, GeboorteDatum, Geslacht, ChiroLeefTijd)
SELECT DISTINCT
  p.AdNr AS AdNummer
, p.Naam, p.VoorNaam, p.GeboorteDatum, p.Geslacht
, 0 AS ChiroLeefTijd
FROM KipAdmin.dbo.kipPersoon p
JOIN KipadMin.dbo.kipLidLeidKad lk ON p.AdNr = lk.AdNr
JOIN grp.Groep g ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS

INSERT INTO pers.GelieerdePersoon
SELECT DISTINCT g.GroepID, p.PersoonID
FROM KipAdmin.dbo.kipLidLeidKad lk
JOIN grp.Groep g ON lk.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code COLLATE SQL_Latin1_General_CP1_CI_AS
JOIN pers.Persoon p ON lk.AdNr = p.AdNummer

