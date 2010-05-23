SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_Adres] FOREIGN KEY([AdresID]) REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_AdresType] FOREIGN KEY([AdresTypeID]) REFERENCES [pers].[AdresType] ([AdresTypeID])
GO

ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_Persoon] FOREIGN KEY([PersoonID]) REFERENCES [pers].[Persoon] ([PersoonID])
GO

ALTER TABLE [grp].[GroepsAdres]  WITH CHECK ADD  CONSTRAINT [FK_GroepsAdres_Adres] FOREIGN KEY([AdresID]) REFERENCES [adr].[Adres] ([AdresID])
GO

ALTER TABLE [grp].[GroepsAdres]  WITH CHECK ADD  CONSTRAINT [FK_GroepsAdres_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [lid].[Lid]  WITH CHECK ADD  CONSTRAINT [FK_Lid_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID]) REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO

ALTER TABLE [lid].[Lid]  WITH CHECK ADD  CONSTRAINT [FK_Lid_GroepsWerkjaar] FOREIGN KEY([GroepsWerkjaarID]) REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO

ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_Afdeling] FOREIGN KEY([AfdelingID]) REFERENCES [lid].[Afdeling] ([AfdelingID])
GO

ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_GroepsWerkjaar] FOREIGN KEY([GroepsWerkJaarID]) REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO

ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_OfficieleAfdeling] FOREIGN KEY([OfficieleAfdelingID]) REFERENCES [lid].[OfficieleAfdeling] ([officieleAfdelingID])
GO

ALTER TABLE [core].[VrijVeldType]  WITH CHECK ADD  CONSTRAINT [FK_VrijVeldType_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [lid].[Afdeling]  WITH CHECK ADD  CONSTRAINT [FK_Afdeling_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [auth].[GebruikersRecht]  WITH CHECK ADD  CONSTRAINT [FK_GebruikersRecht_Gav] FOREIGN KEY([GavID]) REFERENCES [auth].[Gav] ([GavID])
GO

ALTER TABLE [auth].[GebruikersRecht]  WITH CHECK ADD  CONSTRAINT [FK_GebruikersRecht_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [grp].[ChiroGroep]  WITH CHECK ADD  CONSTRAINT [FK_ChiroGroep_Groep] FOREIGN KEY([ChiroGroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_Persoon] FOREIGN KEY([PersoonID]) REFERENCES [pers].[Persoon] ([PersoonID])
GO

ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_PersoonsAdres] FOREIGN KEY([VoorkeursAdresID]) REFERENCES [pers].[PersoonsAdres] ([PersoonsAdresID])
GO

ALTER TABLE [core].[Categorie]  WITH CHECK ADD  CONSTRAINT [FK_Categorie_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE lid.Functie  ADD  CONSTRAINT FK_Functie_Groep FOREIGN KEY(GroepID) REFERENCES grp.Groep (GroepID)
GO

ALTER TABLE lid.LidFunctie  ADD  CONSTRAINT FK_LidFunctie_Lid FOREIGN KEY(LidID) REFERENCES lid.Lid (LidID)
GO

ALTER TABLE lid.LidFunctie  ADD  CONSTRAINT FK_LidFunctie_Functie FOREIGN KEY(FunctieID) REFERENCES lid.Functie (FunctieID)
GO

ALTER TABLE [grp].[GroepsWerkJaar]  WITH CHECK ADD  CONSTRAINT [FK_GroepsWerkjaar_Groep] FOREIGN KEY([GroepID]) REFERENCES [grp].[Groep] ([GroepID])
GO

ALTER TABLE [pers].[PersoonsCategorie]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsCategorie_Categorie] FOREIGN KEY([CategorieID]) REFERENCES [core].[Categorie] ([CategorieID])
GO

ALTER TABLE [pers].[PersoonsCategorie]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsCategorie_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID]) REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO

ALTER TABLE [pers].[CommunicatieVorm]  WITH CHECK ADD  CONSTRAINT [FK_CommunicatieVorm_CommunicatieType] FOREIGN KEY([CommunicatieTypeID]) REFERENCES [pers].[CommunicatieType] ([CommunicatieTypeID])
GO

ALTER TABLE [pers].[CommunicatieVorm]  WITH CHECK ADD  CONSTRAINT [FK_CommunicatieVorm_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID]) REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO

ALTER TABLE [pers].[PersoonVrijVeld]  WITH CHECK ADD  CONSTRAINT [FK_Persoon_PersoonVrijVeldType] FOREIGN KEY([PersoonVrijVeldTypeID]) REFERENCES [pers].[PersoonVrijVeldType] ([PersoonVrijVeldTypeID])
GO

ALTER TABLE [pers].[PersoonVrijVeld]  WITH CHECK ADD  CONSTRAINT [FK_PersoonVrijVeld_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID]) REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO

ALTER TABLE adr.WoonPlaats
ADD CONSTRAINT FK_WoonPlaats_PostNummer
FOREIGN KEY(PostNummer) REFERENCES adr.PostNr(PostNr);
GO

ALTER TABLE adr.WoonPlaats
ADD CONSTRAINT FK_WoonPlaats_Taal
FOREIGN KEY(TaalID) REFERENCES Core.Taal(TaalID);
GO

ALTER TABLE adr.StraatNaam
ADD CONSTRAINT FK_StraatNaam_PostNummer
FOREIGN KEY(PostNummer) REFERENCES adr.PostNr(PostNr);
GO

ALTER TABLE adr.StraatNaam
ADD CONSTRAINT FK_StraatNaam_Taal
FOREIGN KEY(TaalID) REFERENCES Core.Taal(TaalID);
GO


ALTER TABLE [adr].[Adres]  WITH CHECK ADD  CONSTRAINT [FK_Adres_StraatNaam] FOREIGN KEY([StraatNaamID]) REFERENCES [adr].[StraatNaam] ([StraatNaamID])
GO

ALTER TABLE [adr].[Adres]  WITH CHECK ADD  CONSTRAINT [FK_Adres_WoonPlaats] FOREIGN KEY([WoonPlaatsID]) REFERENCES [adr].[WoonPlaats] ([WoonPlaatsID])
GO

ALTER TABLE [lid].[LeidingInAfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_LeidingInAfdelingsJaar_AfdelingsJaar] FOREIGN KEY([AfdelingsJaarID]) REFERENCES [lid].[AfdelingsJaar] ([AfdelingsJaarID])
GO

ALTER TABLE [lid].[LeidingInAfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_LeidingInAfdelingsJaar_Leiding] FOREIGN KEY([LeidingID]) REFERENCES [lid].[Leiding] ([leidingID])
GO

ALTER TABLE [lid].[Leiding]  WITH CHECK ADD  CONSTRAINT [FK_Leiding_Lid] FOREIGN KEY([leidingID]) REFERENCES [lid].[Lid] ([LidID])
GO

ALTER TABLE [lid].[Kind]  WITH CHECK ADD  CONSTRAINT [FK_Kind_AfdelingsJaar] FOREIGN KEY([afdelingsJaarID]) REFERENCES [lid].[AfdelingsJaar] ([AfdelingsJaarID])
GO

ALTER TABLE [lid].[Kind]  WITH CHECK ADD  CONSTRAINT [FK_Kind_Lid] FOREIGN KEY([kindID]) REFERENCES [lid].[Lid] ([LidID])
GO

ALTER TABLE [pers].[PersoonVrijVeldType]  WITH CHECK ADD  CONSTRAINT [FK_PersoonVrijVeldType_VrijVeldType] FOREIGN KEY([PersoonVrijVeldTypeID]) REFERENCES [core].[VrijVeldType] ([VrijVeldTypeID])
GO

alter table lid.functie
add CONSTRAINT [AK_Functie_GroepID_Code] UNIQUE NONCLUSTERED 
(
	[GroepID] ASC,
	[Code] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

alter table lid.functie
add CONSTRAINT [AK_Functie_GroepID_Naam] UNIQUE NONCLUSTERED 
(
	[GroepID] ASC,
	[Naam] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO