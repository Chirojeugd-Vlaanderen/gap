create procedure data.spDubbelePersoonVerwijderen (@foutPID as int, @juistPID as int) as
-- alle referenties van persoon met @foutPID veranderen naar die van persoon met @juistPID
begin

BEGIN TRAN

-- probeer de gelieerde personen gekoppeld aan de 'verkeerde' persoon
-- te koppelen aan de juiste

UPDATE gp1
SET gp1.PersoonID = @juistPID
FROM pers.GelieerdePersoon gp1 
WHERE gp1.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.GelieerdePersoon gp2 
WHERE gp2.PersoonID = @juistPID
AND gp2.GroepID = gp1.GroepID)

-- als dat niet overal gelukt is, dan was 


-- voor de gelieerde personen waar dat niet lukt, verleggen we
-- de lidobjecten naar een bestaande gelieerde persoon van de
-- juiste persoon

UPDATE foutLid
SET foutLid.GelieerdePersoonID = JuisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp ON fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGP.PersoonID = @foutPID AND juisteGP.PersoonID = @juistPID
AND NOT EXISTS
(SELECT 1 FROM lid.Lid l2 
WHERE l2.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND l2.GroepsWerkJaarID = foutLid.GroepsWerkJaarID)

-- Als een persoon dubbel in de database zat, en gedurende
-- een bepaald werkjaar dubbel lid was van dezelfde groep,
-- dan is dat heel spijtig, maar gaan de functies van het foute
-- lid verloren.

DELETE fouteFunc
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN lid.LidFunctie fouteFunc on foutLid.LidID = fouteFunc.LidID
WHERE fouteGP.PersoonID = @foutPID

DELETE foutKind
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN lid.Kind foutKind on foutLid.LidID = foutKind.KindID
WHERE fouteGP.PersoonID = @foutPID

DELETE foutLeid
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN lid.Leiding foutLeid on foutLid.LidID = foutLeid.LeidingID
WHERE fouteGP.PersoonID = @foutPID

DELETE foutLid
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
WHERE fouteGP.PersoonID = @foutPID

-- Probeer eerst communicatievormen te verleggen naar juiste persoon

UPDATE fouteCv
SET fouteCv.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm fouteCv on fouteCv.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID
AND NOT EXISTS
(SELECT 1 FROM pers.CommunicatieVorm cv
WHERE cv.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND cv.Nummer = fouteCv.Nummer)

-- Diegene die niet verlegd kunnen worden (omdat nummer al bestaat bij juiste GP)
-- worden verwijderd.  Voorkeur, gezinsgebonden,... gaan verloren.

DELETE cv
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm cv on fouteGp.GelieerdePersoonID=cv.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

-- Probeer categorieen te verleggen naar juiste persoon

UPDATE foutePc
SET foutePc.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN pers.PersoonsCategorie foutePc on foutePc.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID
AND NOT EXISTS
(SELECT 1 FROM pers.PersoonsCategorie pc
WHERE pc.GelieerdePersoonID = juisteGp.GelieerdePersoonID
AND pc.CategorieID = foutePc.CategorieID)

-- verwijder overgebleven categorieen van foute gelieerde persoon

DELETE pc
FROM pers.GelieerdePersoon fouteGp
JOIN pers.PersoonsCategorie pc on fouteGp.GelieerdePersoonID=pc.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

-- op dit moment kunnen de foute gelieerde personen verdwijnen

DELETE FROM pers.GelieerdePersoon
WHERE PersoonID = @foutPID

-- Probeer de adressen te verleggen naar de goede persoon

UPDATE foutPa
SET foutPa.PersoonID = @juistPID
FROM pers.PersoonsAdres foutPa
WHERE foutPa.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.PersoonsAdres pa
WHERE pa.PersoonID = @juistPID
AND pa.AdresID = foutPa.AdresID)

-- De adressen die al aan de goede persoon gekoppeld waren, mogen weg van de
-- foute persoon.  We verliezen wel de opmerking

-- eerst nog eens kijken of het te verwijderen persoonsadres geen voorkeursadres
-- is van een gelieerde persoon.  Zo ja: wijzigen

UPDATE gp
SET gp.VoorkeursAdresID = juistePa.PersoonsAdresID
FROM pers.PersoonsAdres foutePa 
JOIN pers.GelieerdePersoon gp on gp.VoorkeursAdresID = foutePa.PersoonsAdresID
JOIN pers.PersoonsAdres juistePa on juistePa.AdresID = foutePa.AdresID
WHERE foutePa.PersoonID=@foutPID AND juistePa.PersoonID = @juistPID 

DELETE FROM pers.PersoonsAdres WHERE PersoonID=@foutPID

-- We gaan ervan uit dat de goeie informatie in het juiste
-- persoonsrecord zit
DELETE FROM pers.Persoon WHERE PersoonID=@foutPID

COMMIT TRAN

end
