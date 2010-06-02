CREATE PROCEDURE data.spCommunicatieUitKipadmin @stamNr VARCHAR(10) AS
-- Haalt communicatievormen in Kipadmin over naar GAP

DECLARE @groepID AS INT;

set @groepID = (SELECT GroepID FROM grp.Groep WHERE Code=@stamNr)

INSERT INTO pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, Nummer, Voorkeur)
	SELECT 	gp.GelieerdePersoonID, 
			pct.CommunicatieTypeID, 
			ci.Info,
			0 as voorkeur  -- Voorlopig nog geen voorkeur.
		FROM pers.Persoon p JOIN kipadmin.dbo.kipContactInfo ci 
				ON p.AdNummer = ci.AdNr
			JOIN pers.GelieerdePersoon gp 
				ON p.PersoonID = gp.PersoonID
			JOIN kipadmin.dbo.kipContactType ct 
				ON ci.ContactTypeID = ct.ContactTypeID
			JOIN pers.CommunicatieType pct 
				ON pct.Omschrijving = ct.Omschrijving COLLATE SQL_Latin1_General_CP1_CI_AI
		WHERE gp.GroepID = @GroepID
			AND NOT EXISTS (SELECT 1 FROM pers.CommunicatieVorm cv2 JOIN pers.GelieerdePersoon gp2 
										ON cv2.GelieerdePersoonID = gp2.GelieerdePersoonID
									JOIN pers.Persoon p2 
										ON gp2.PersoonID = p2.PersoonID
									WHERE p2.AdNummer = p.AdNummer
									AND cv2.CommunicatieTypeID = pct.CommunicatieTypeID
									AND (cv2.Nummer = ci.Info COLLATE SQL_Latin1_General_CP1_CI_AI
											OR cv2.CommunicatieTypeID <= 2 
												AND core.ufnEnkelCijfers(cv2.Nummer) = core.ufnEnkelCijfers(ci.Info)))


-- update voorkeur op basis van kipadmin

UPDATE cv 
	SET VoorKeur = CASE ci.VolgNr WHEN 1 THEN 1 ELSE 0 END
		FROM pers.CommunicatieVorm cv JOIN pers.GelieerdePersoon gp 
					ON cv.GelieerdePersoonID = gp.GelieerdePersoonID
				JOIN pers.Persoon p 
					ON gp.PersoonID = p.PersoonID
				JOIN kipadmin.dbo.kipContactInfo ci 
					ON p.AdNummer = ci.AdNr 
						AND ci.ContactTypeID = cv.CommunicatieTypeID 
						AND (cv.Nummer = ci.Info COLLATE SQL_Latin1_General_CP1_CI_AI
											OR cv.CommunicatieTypeID <= 2 
												AND core.ufnEnkelCijfers(cv.Nummer) = core.ufnEnkelCijfers(ci.Info))
WHERE gp.GroepID = @groepID
