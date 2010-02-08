INSERT INTO auth.Gav(Login)
SELECT Login FROM auth.vGebruikers


INSERT INTO auth.GebruikersRecht(GavID, GroepID, VervalDatum)
SELECT gav.GavID,g.GroepID,'2009-08-31' AS VervalDatum FROM auth.Gav gav
JOIN auth.vGebruikers u ON gav.Login = u.Login
JOIN grp.Groep g ON u.StamNr COLLATE SQL_Latin1_General_CP1_CI_AS = g.Code