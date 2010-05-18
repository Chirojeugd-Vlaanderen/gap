CREATE PROCEDURE auth.spGebruikersRechtToekennen @stamNr VARCHAR(10), @login VARCHAR(20) AS
-- Verwijdert alle gelieerde personen uit een groep.

INSERT INTO auth.GebruikersRecht(GavID, GroepID)
SELECT GavID, GroepID FROM auth.Gav gav, grp.Groep g
WHERE gav.Login=@login
AND g.Code=@stamnr
AND NOT EXISTS (SELECT 1 FROM auth.GebruikersRecht WHERE GavID = gav.GavID AND GroepID = g.GroepID)