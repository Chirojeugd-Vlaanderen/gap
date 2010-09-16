create procedure data.spDubbelePersoonVerwijderen (@foutPID as int, @juistPID as int) as
-- alle referenties van persoon met @foutPID veranderen naar die van persoon met @juistPID
begin

BEGIN TRAN

-- probeer in eerste instantie alle gelieerde personen van de
-- foute persoon naar de juiste persoon te laten wijzen

UPDATE gp1
SET gp1.PersoonID = @juistPID
FROM pers.GelieerdePersoon gp1 
WHERE gp1.PersoonID = @foutPID
AND NOT EXISTS
(SELECT 1 FROM pers.GelieerdePersoon gp2 
WHERE gp2.PersoonID = @juistPID
AND gp2.GroepID = gp1.GroepID)

-- voor de gelieerde personen waar dat niet lukt, verleggen we
-- de lidobjecten naar een bestaande gelieerde persoon van de
-- juiste persoon

UPDATE foutLid
SET foutLid.GelieerdePersoonID = JuisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN lid.Lid foutLID on fouteGp.GelieerdePersoonID = foutLid.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp ON fouteGp.PersoonID = juisteGp.PersoonID
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

-- We gaan ervan uit dat de juiste communicatievormen aan de juiste
-- persoon hangen.

DELETE cv
FROM pers.GelieerdePersoon fouteGp
JOIN pers.CommunicatieVorm cv on fouteGp.GelieerdePersoonID=cv.GelieerdePersoonID
WHERE fouteGp.PersoonID = @foutPID

DELETE FROM pers.GelieerdePersoon
WHERE PersoonID = @foutPID

-- We gaan ervan uit dat de goeie adressen aan de juiste persoon
-- gekoppeld zijn.
DELETE FROM pers.PersoonsAdres WHERE PersoonID=@foutPID

-- We gaan ervan uit dat de goeie informatie in het juiste
-- persoonsrecord zit
DELETE FROM pers.Persoon WHERE PersoonID=@foutPID

COMMIT TRAN

end
