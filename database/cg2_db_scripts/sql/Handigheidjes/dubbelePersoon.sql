CREATE procedure [data].[spDubbelePersoonVerwijderen] (@foutPID as int, @juistPID as int) as
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


-- voor de gelieerde personen waar dat niet lukt, verleggen we
-- de lidobjecten naar een bestaande gelieerde persoon van de
-- juiste persoon

-- om te vermijden dat we straks actieve leden gaan verwijderen,
-- en inactieve leden gaan behouden, verwijderen we eerst alle
-- inactieve leden gekoppeld aan de 'juiste' persoon.
-- (op die manier worden straks eventuele lidobjecten van de foute
-- persoon proper overgezet naar de juiste)

-- (het nadeel is dat gemergede personen die inactief lid waren,
-- mogelijk een nieuwe probeerperiode krijgen als ze opnieuw lid 
-- worden.)

DELETE lf
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE k
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Kind k on l.LidID = k.KindID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE lia
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
JOIN lid.LeidingInAfdelingsJaar lia on ld.LeidingID = lia.LeidingID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE ld
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

DELETE l
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
WHERE gp.PersoonID = @juistPID AND l.NonActief = 1

-- nu proberen we bestaande lidobjecten van de dubbele
-- persoon over te zetten naar de originele.  Eventuele
-- inactieve lidobjecten voor de originele staan hiervoor
-- nu niet meer in de weg.

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

-- Het zou kunnen dat zowel de dubbele als de originele persoon
-- actieve lidobjecten heeft.  In dat geval zijn die van de dubbele
-- nog steeds aan de dubbele gekoppeld.  Heel jammer, maar die
-- van de dubbele worden dan verwijderd.  (incl. afdelingen en functies)

-- Dit stuk code lijkt zeer hard op wat we hierboven doen voor de
-- inactieve leden van de originele persoon.

DELETE lf
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.LidFunctie lf on l.LidID = lf.LidID
WHERE gp.PersoonID = @foutPID

DELETE k
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Kind k on l.LidID = k.KindID
WHERE gp.PersoonID = @foutPID

DELETE lia
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
JOIN lid.LeidingInAfdelingsJaar lia on ld.LeidingID = lia.LeidingID
WHERE gp.PersoonID = @foutPID

DELETE ld
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
JOIN lid.Leiding ld on l.LidID = ld.LeidingID
WHERE gp.PersoonID = @foutPID

DELETE l
FROM pers.GelieerdePersoon gp
JOIN lid.Lid l on gp.GelieerdePersoonID = l.GelieerdePersoonID
WHERE gp.PersoonID = @foutPID


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

-- probeer deelnemers te verleggen

UPDATE fouteDn
SET fouteDn.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN biv.Deelnemer fouteDn on fouteDn.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID

-- foute abonnementen

UPDATE fouteAb
SET fouteAb.GelieerdePersoonID=juisteGp.GelieerdePersoonID
FROM pers.GelieerdePersoon fouteGp
JOIN abo.Abonnement fouteAb on fouteAb.GelieerdePersoonID = fouteGp.GelieerdePersoonID
JOIN pers.GelieerdePersoon juisteGp on fouteGp.GroepID = juisteGp.GroepID
WHERE fouteGp.PersoonID=@foutPID AND juisteGp.PersoonID=@juistPID

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

-- Persoonsverzekeringen verleggen
-- (Dit gaat wel problemen geven als we op termijn de verzekeringen aan
-- gelieerde personen gaan koppelen)

UPDATE pv
SET pv.PersoonID = @juistPID
FROM verz.PersoonsVerzekering pv
WHERE pv.PersoonID = @foutPID

-- GAV-schap verleggen

UPDATE gs
SET gs.PersoonID = @juistPID
FROM auth.gavSchap gs
WHERE gs.PersoonID = @foutPID

-- We gaan ervan uit dat de goeie informatie in het juiste
-- persoonsrecord zit
DELETE FROM pers.Persoon WHERE PersoonID=@foutPID

COMMIT TRAN

end



GO



