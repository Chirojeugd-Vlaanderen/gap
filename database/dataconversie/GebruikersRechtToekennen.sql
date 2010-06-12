CREATE PROCEDURE auth.spGebruikersRechtToekennen @stamNr VARCHAR(10), @login VARCHAR(20) AS
-- Geeft login gebruikersrecht op gegeven groep
INSERT INTO auth.Gav(Login)
SELECT @login WHERE NOT EXISTS (SELECT 1 FROM auth.Gav WHERE Login=@login)

INSERT INTO auth.GebruikersRecht(GavID, GroepID, VervalDatum)
SELECT GavID, GroepID, DateAdd(year, 1, getDate())  FROM auth.Gav gav, grp.Groep g
WHERE gav.Login=@login
AND g.Code=@stamnr
AND NOT EXISTS (SELECT 1 FROM auth.GebruikersRecht WHERE GavID = gav.GavID AND GroepID = g.GroepID)