CREATE UNIQUE INDEX AK_Taal_Code ON core.Taal(Code)
GO

CREATE UNIQUE INDEX AK_Groep_Code ON grp.Groep(Code)
GO

CREATE UNIQUE INDEX AK_Gav_Login ON auth.Gav(Login)
GO

CREATE UNIQUE INDEX AK_Lid_GelieerdePersoonID_GroepsWerkJaarID ON lid.Lid(GelieerdePersoonID, GroepsWerkJaarID)
GO

CREATE INDEX IDX_Lid_GroepsWerkJaarID ON lid.LID(GroepsWerkJaarID)
GO

CREATE UNIQUE INDEX AK_GebruikersRecht_GroepID_GavID ON auth.GebruikersRecht(GroepID, GavID)
GO

CREATE INDEX IDX_GelieerdePersoon_GroepID ON pers.GelieerdePersoon(GroepID)
GO

CREATE INDEX IDX_GelieerdePersoon_PersoonID ON pers.GelieerdePersoon(PersoonID)
GO

CREATE INDEX IDX_GelieerdePersoon_VoorkeursAdresID ON pers.GelieerdePersoon(VoorkeursAdresID)
GO

CREATE UNIQUE INDEX IDX_GroepsWerkJaar_GroepID_WerkJaar ON grp.GroepsWerkJaar(GroepID, WerkJaar)
GO

CREATE UNIQUE INDEX AK_StraatNaam_PostNummer_Naam ON adr.StraatNaam(PostNummer, Naam)
GO

-- Onderstaande zou uniek moeten zijn, maar CRAB is CRAP!
CREATE INDEX AK_StraatNaam_CrabSubStraatID_TaalID ON adr.StraatNaam(CrabSubStraatID, TaalID)
GO

CREATE UNIQUE INDEX AK_WoonPlaats_PostNummer_Naam ON adr.WoonPlaats(PostNummer, Naam)
GO

CREATE UNIQUE INDEX IX_Functie_Naam_GroepID ON lid.Functie(Naam, GroepID)
GO

CREATE UNIQUE INDEX AK_PersoonsAdres_PersoonID_AdresID ON pers.PersoonsAdres(PersoonID, AdresID) INCLUDE (AdresTypeID, Versie, PersoonsAdresID)
GO

CREATE INDEX IX_PersoonsAdres_AdresID on pers.PersoonsAdres(AdresID)

CREATE INDEX IDX_CommunicatieVorm_GelieerdePersoonID_CommunicatieTypeID_CommunicatieVormID ON pers.CommunicatieVorm(GelieerdePersoonID, CommunicatieTypeID, CommunicatieVormID) INCLUDE (Nota, Nummer, IsGezinsgebonden, Voorkeur, Versie) 
GO

CREATE INDEX IDX_Lid_GroepsWerkJaarID ON lid.LID(GroepsWerkJaarID)
GO
