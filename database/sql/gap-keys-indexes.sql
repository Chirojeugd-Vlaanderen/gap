GO
/****** Object:  Index [AK_Land_IsoCode]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [adr].[Land] ADD  CONSTRAINT [AK_Land_IsoCode] UNIQUE NONCLUSTERED
(
	[IsoCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [AK_Categorie_GroepID_Code]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [core].[Categorie] ADD  CONSTRAINT [AK_Categorie_GroepID_Code] UNIQUE NONCLUSTERED
(
	[GroepID] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [AK_Categorie_GroepID_Naam]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [core].[Categorie] ADD  CONSTRAINT [AK_Categorie_GroepID_Naam] UNIQUE NONCLUSTERED
(
	[GroepID] ASC,
	[Naam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [AK_Afdeling_GroepID_AfdelingsNaam]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[Afdeling] ADD  CONSTRAINT [AK_Afdeling_GroepID_AfdelingsNaam] UNIQUE NONCLUSTERED
(
	[ChiroGroepID] ASC,
	[AfdelingsNaam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [AK_Afdeling_GroepID_Afkorting]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[Afdeling] ADD  CONSTRAINT [AK_Afdeling_GroepID_Afkorting] UNIQUE NONCLUSTERED
(
	[ChiroGroepID] ASC,
	[Afkorting] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UQ_AfdelingsJaar]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[AfdelingsJaar] ADD  CONSTRAINT [UQ_AfdelingsJaar] UNIQUE NONCLUSTERED
(
	[GroepsWerkJaarID] ASC,
	[AfdelingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [AK_Functie_GroepID_Code]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[Functie] ADD  CONSTRAINT [AK_Functie_GroepID_Code] UNIQUE NONCLUSTERED
(
	[GroepID] ASC,
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [AK_Functie_GroepID_Naam]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[Functie] ADD  CONSTRAINT [AK_Functie_GroepID_Naam] UNIQUE NONCLUSTERED
(
	[GroepID] ASC,
	[Naam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_Lid]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[Lid] ADD  CONSTRAINT [PK_Lid] PRIMARY KEY NONCLUSTERED
(
	[LidID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_OfficieleAfdeling_Naam]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [lid].[OfficieleAfdeling] ADD  CONSTRAINT [UQ_OfficieleAfdeling_Naam] UNIQUE NONCLUSTERED
(
	[Naam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_CommunicatieVorm]    Script Date: 21/02/2017 10:54:45 ******/
ALTER TABLE [pers].[CommunicatieVorm] ADD  CONSTRAINT [PK_CommunicatieVorm] PRIMARY KEY NONCLUSTERED
(
	[CommunicatieVormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [abo].[Abonnement] ADD  DEFAULT (getdate()) FOR [AanvraagDatum]
GO
ALTER TABLE [abo].[Abonnement] ADD  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [abo].[Publicatie] ADD  DEFAULT ((1)) FOR [Actief]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [PersoonsPermissies]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [GroepsPermissies]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [AfdelingsPermissies]
GO
ALTER TABLE [auth].[GebruikersRechtv2] ADD  DEFAULT ((0)) FOR [IedereenPermissies]
GO
ALTER TABLE [grp].[GroepsWerkJaar] ADD  DEFAULT (getdate()) FOR [Datum]
GO
ALTER TABLE [lid].[Functie] ADD  DEFAULT ((175)) FOR [Niveau]
GO
ALTER TABLE [lid].[Lid] ADD  DEFAULT ((0)) FOR [IsAangesloten]
GO
ALTER TABLE [logging].[Bericht] ADD  DEFAULT (getdate()) FOR [Tijd]
GO
ALTER TABLE [logging].[Bericht] ADD  DEFAULT ((1)) FOR [Niveau]
GO
ALTER TABLE [pers].[CommunicatieVorm] ADD  CONSTRAINT [DF_CommunicatieVorm_IsGezinsgebonden]  DEFAULT ((0)) FOR [IsGezinsgebonden]
GO
ALTER TABLE [pers].[CommunicatieVorm] ADD  CONSTRAINT [DF_CommunicatieVorm_Voorkeur]  DEFAULT ((0)) FOR [Voorkeur]
GO
ALTER TABLE [pers].[GelieerdePersoon] ADD  CONSTRAINT [DF_GelieerdePersoon_ChiroLeefTijd]  DEFAULT ((0)) FOR [ChiroLeefTijd]
GO
ALTER TABLE [pers].[Persoon] ADD  CONSTRAINT [DF_Persoon_AdInAanvraag]  DEFAULT ((0)) FOR [AdInAanvraag]
GO
ALTER TABLE [pers].[Persoon] ADD  DEFAULT ((0)) FOR [NieuwsBrief]
GO
ALTER TABLE [abo].[Abonnement]  WITH CHECK ADD  CONSTRAINT [FK_Abonnement_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [abo].[Abonnement] CHECK CONSTRAINT [FK_Abonnement_GelieerdePersoon]
GO
ALTER TABLE [abo].[Abonnement]  WITH CHECK ADD  CONSTRAINT [FK_Abonnement_GroepsWerkJaar] FOREIGN KEY([GroepsWerkJaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [abo].[Abonnement] CHECK CONSTRAINT [FK_Abonnement_GroepsWerkJaar]
GO
ALTER TABLE [abo].[Abonnement]  WITH CHECK ADD  CONSTRAINT [FK_Abonnement_Publicatie] FOREIGN KEY([PublicatieID])
REFERENCES [abo].[Publicatie] ([PublicatieID])
GO
ALTER TABLE [abo].[Abonnement] CHECK CONSTRAINT [FK_Abonnement_Publicatie]
GO
ALTER TABLE [adr].[BelgischAdres]  WITH CHECK ADD  CONSTRAINT [FK_BelgischAdres_Adres] FOREIGN KEY([BelgischAdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [adr].[BelgischAdres] CHECK CONSTRAINT [FK_BelgischAdres_Adres]
GO
ALTER TABLE [adr].[BelgischAdres]  WITH CHECK ADD  CONSTRAINT [FK_BelgischAdres_StraatNaam] FOREIGN KEY([StraatNaamID])
REFERENCES [adr].[StraatNaam] ([StraatNaamID])
GO
ALTER TABLE [adr].[BelgischAdres] CHECK CONSTRAINT [FK_BelgischAdres_StraatNaam]
GO
ALTER TABLE [adr].[BelgischAdres]  WITH CHECK ADD  CONSTRAINT [FK_BelgischAdres_WoonPlaats] FOREIGN KEY([WoonPlaatsID])
REFERENCES [adr].[WoonPlaats] ([WoonPlaatsID])
GO
ALTER TABLE [adr].[BelgischAdres] CHECK CONSTRAINT [FK_BelgischAdres_WoonPlaats]
GO
ALTER TABLE [adr].[BuitenLandsAdres]  WITH CHECK ADD  CONSTRAINT [FK_BuitenlandsAdres_Adres] FOREIGN KEY([BuitenlandsAdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [adr].[BuitenLandsAdres] CHECK CONSTRAINT [FK_BuitenlandsAdres_Adres]
GO
ALTER TABLE [adr].[BuitenLandsAdres]  WITH CHECK ADD  CONSTRAINT [FK_BuitenlandsAdres_Land] FOREIGN KEY([LandID])
REFERENCES [adr].[Land] ([LandID])
GO
ALTER TABLE [adr].[BuitenLandsAdres] CHECK CONSTRAINT [FK_BuitenlandsAdres_Land]
GO
ALTER TABLE [adr].[StraatNaam]  WITH CHECK ADD  CONSTRAINT [FK_StraatNaam_PostNummer] FOREIGN KEY([PostNummer])
REFERENCES [adr].[PostNr] ([PostNr])
GO
ALTER TABLE [adr].[StraatNaam] CHECK CONSTRAINT [FK_StraatNaam_PostNummer]
GO
ALTER TABLE [adr].[StraatNaam]  WITH CHECK ADD  CONSTRAINT [FK_StraatNaam_Taal] FOREIGN KEY([TaalID])
REFERENCES [core].[Taal] ([TaalID])
GO
ALTER TABLE [adr].[StraatNaam] CHECK CONSTRAINT [FK_StraatNaam_Taal]
GO
ALTER TABLE [adr].[WoonPlaats]  WITH CHECK ADD  CONSTRAINT [FK_WoonPlaats_PostNummer] FOREIGN KEY([PostNummer])
REFERENCES [adr].[PostNr] ([PostNr])
GO
ALTER TABLE [adr].[WoonPlaats] CHECK CONSTRAINT [FK_WoonPlaats_PostNummer]
GO
ALTER TABLE [adr].[WoonPlaats]  WITH CHECK ADD  CONSTRAINT [FK_WoonPlaats_Taal] FOREIGN KEY([TaalID])
REFERENCES [core].[Taal] ([TaalID])
GO
ALTER TABLE [adr].[WoonPlaats] CHECK CONSTRAINT [FK_WoonPlaats_Taal]
GO
ALTER TABLE [auth].[GebruikersRechtv2]  WITH CHECK ADD  CONSTRAINT [FK_GebruikersRechtV2_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [auth].[GebruikersRechtv2] CHECK CONSTRAINT [FK_GebruikersRechtV2_Groep]
GO
ALTER TABLE [auth].[GebruikersRechtv2]  WITH CHECK ADD  CONSTRAINT [FK_GebruikersRechtV2_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [auth].[GebruikersRechtv2] CHECK CONSTRAINT [FK_GebruikersRechtV2_Persoon]
GO
ALTER TABLE [biv].[Deelnemer]  WITH CHECK ADD  CONSTRAINT [FK_Deelnemer_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [biv].[Deelnemer] CHECK CONSTRAINT [FK_Deelnemer_GelieerdePersoon]
GO
ALTER TABLE [biv].[Deelnemer]  WITH CHECK ADD  CONSTRAINT [FK_Deelnemer_Uitstap] FOREIGN KEY([UitstapID])
REFERENCES [biv].[Uitstap] ([UitstapID])
GO
ALTER TABLE [biv].[Deelnemer] CHECK CONSTRAINT [FK_Deelnemer_Uitstap]
GO
ALTER TABLE [biv].[Plaats]  WITH CHECK ADD  CONSTRAINT [FK_Plaats_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [biv].[Plaats] CHECK CONSTRAINT [FK_Plaats_Adres]
GO
ALTER TABLE [biv].[Plaats]  WITH CHECK ADD  CONSTRAINT [FK_Plaats_GelieerdePersoon_Contact] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [biv].[Plaats] CHECK CONSTRAINT [FK_Plaats_GelieerdePersoon_Contact]
GO
ALTER TABLE [biv].[Plaats]  WITH CHECK ADD  CONSTRAINT [FK_Plaats_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [biv].[Plaats] CHECK CONSTRAINT [FK_Plaats_Groep]
GO
ALTER TABLE [biv].[Uitstap]  WITH CHECK ADD  CONSTRAINT [FK_Uitstap_Deelnemer_Contact] FOREIGN KEY([ContactDeelnemerID])
REFERENCES [biv].[Deelnemer] ([DeelnemerID])
GO
ALTER TABLE [biv].[Uitstap] CHECK CONSTRAINT [FK_Uitstap_Deelnemer_Contact]
GO
ALTER TABLE [biv].[Uitstap]  WITH CHECK ADD  CONSTRAINT [FK_Uitstap_GroepsWerkJaar] FOREIGN KEY([GroepsWerkJaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [biv].[Uitstap] CHECK CONSTRAINT [FK_Uitstap_GroepsWerkJaar]
GO
ALTER TABLE [biv].[Uitstap]  WITH CHECK ADD  CONSTRAINT [FK_Uitstap_Plaats] FOREIGN KEY([PlaatsID])
REFERENCES [biv].[Plaats] ([PlaatsID])
GO
ALTER TABLE [biv].[Uitstap] CHECK CONSTRAINT [FK_Uitstap_Plaats]
GO
ALTER TABLE [core].[Categorie]  WITH CHECK ADD  CONSTRAINT [FK_Categorie_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [core].[Categorie] CHECK CONSTRAINT [FK_Categorie_Groep]
GO
ALTER TABLE [core].[VrijVeldType]  WITH CHECK ADD  CONSTRAINT [FK_VrijVeldType_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [core].[VrijVeldType] CHECK CONSTRAINT [FK_VrijVeldType_Groep]
GO
ALTER TABLE [grp].[ChiroGroep]  WITH CHECK ADD  CONSTRAINT [FK_ChiroGroep_Groep] FOREIGN KEY([ChiroGroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[ChiroGroep] CHECK CONSTRAINT [FK_ChiroGroep_Groep]
GO
ALTER TABLE [grp].[ChiroGroep]  WITH CHECK ADD  CONSTRAINT [FK_ChiroGroep_KaderGroep] FOREIGN KEY([KaderGroepID])
REFERENCES [grp].[KaderGroep] ([KaderGroepID])
GO
ALTER TABLE [grp].[ChiroGroep] CHECK CONSTRAINT [FK_ChiroGroep_KaderGroep]
GO
ALTER TABLE [grp].[Groep]  WITH CHECK ADD  CONSTRAINT [FK_Groep_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [grp].[Groep] CHECK CONSTRAINT [FK_Groep_Adres]
GO
ALTER TABLE [grp].[GroepsAdres]  WITH CHECK ADD  CONSTRAINT [FK_GroepsAdres_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [grp].[GroepsAdres] CHECK CONSTRAINT [FK_GroepsAdres_Adres]
GO
ALTER TABLE [grp].[GroepsAdres]  WITH CHECK ADD  CONSTRAINT [FK_GroepsAdres_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[GroepsAdres] CHECK CONSTRAINT [FK_GroepsAdres_Groep]
GO
ALTER TABLE [grp].[GroepsWerkJaar]  WITH CHECK ADD  CONSTRAINT [FK_GroepsWerkjaar_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[GroepsWerkJaar] CHECK CONSTRAINT [FK_GroepsWerkjaar_Groep]
GO
ALTER TABLE [grp].[KaderGroep]  WITH CHECK ADD  CONSTRAINT [FK_KaderGroep_Groep] FOREIGN KEY([KaderGroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [grp].[KaderGroep] CHECK CONSTRAINT [FK_KaderGroep_Groep]
GO
ALTER TABLE [grp].[KaderGroep]  WITH CHECK ADD  CONSTRAINT [FK_KaderGroep_KaderGroep] FOREIGN KEY([ParentID])
REFERENCES [grp].[KaderGroep] ([KaderGroepID])
GO
ALTER TABLE [grp].[KaderGroep] CHECK CONSTRAINT [FK_KaderGroep_KaderGroep]
GO
ALTER TABLE [lid].[Afdeling]  WITH CHECK ADD  CONSTRAINT [FK_Afdeling_ChiroGroep] FOREIGN KEY([ChiroGroepID])
REFERENCES [grp].[ChiroGroep] ([ChiroGroepID])
GO
ALTER TABLE [lid].[Afdeling] CHECK CONSTRAINT [FK_Afdeling_ChiroGroep]
GO
ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_Afdeling] FOREIGN KEY([AfdelingID])
REFERENCES [lid].[Afdeling] ([AfdelingID])
GO
ALTER TABLE [lid].[AfdelingsJaar] CHECK CONSTRAINT [FK_AfdelingsJaar_Afdeling]
GO
ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_GroepsWerkjaar] FOREIGN KEY([GroepsWerkJaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [lid].[AfdelingsJaar] CHECK CONSTRAINT [FK_AfdelingsJaar_GroepsWerkjaar]
GO
ALTER TABLE [lid].[AfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_AfdelingsJaar_OfficieleAfdeling] FOREIGN KEY([OfficieleAfdelingID])
REFERENCES [lid].[OfficieleAfdeling] ([OfficieleAfdelingID])
GO
ALTER TABLE [lid].[AfdelingsJaar] CHECK CONSTRAINT [FK_AfdelingsJaar_OfficieleAfdeling]
GO
ALTER TABLE [lid].[Functie]  WITH CHECK ADD  CONSTRAINT [FK_Functie_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [lid].[Functie] CHECK CONSTRAINT [FK_Functie_Groep]
GO
ALTER TABLE [lid].[Kind]  WITH CHECK ADD  CONSTRAINT [FK_Kind_AfdelingsJaar] FOREIGN KEY([afdelingsJaarID])
REFERENCES [lid].[AfdelingsJaar] ([AfdelingsJaarID])
GO
ALTER TABLE [lid].[Kind] CHECK CONSTRAINT [FK_Kind_AfdelingsJaar]
GO
ALTER TABLE [lid].[Kind]  WITH CHECK ADD  CONSTRAINT [FK_Kind_Lid] FOREIGN KEY([kindID])
REFERENCES [lid].[Lid] ([LidID])
GO
ALTER TABLE [lid].[Kind] CHECK CONSTRAINT [FK_Kind_Lid]
GO
ALTER TABLE [lid].[Leiding]  WITH CHECK ADD  CONSTRAINT [FK_Leiding_Lid] FOREIGN KEY([leidingID])
REFERENCES [lid].[Lid] ([LidID])
GO
ALTER TABLE [lid].[Leiding] CHECK CONSTRAINT [FK_Leiding_Lid]
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_LeidingInAfdelingsJaar_AfdelingsJaar] FOREIGN KEY([AfdelingsJaarID])
REFERENCES [lid].[AfdelingsJaar] ([AfdelingsJaarID])
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar] CHECK CONSTRAINT [FK_LeidingInAfdelingsJaar_AfdelingsJaar]
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar]  WITH CHECK ADD  CONSTRAINT [FK_LeidingInAfdelingsJaar_Leiding] FOREIGN KEY([LeidingID])
REFERENCES [lid].[Leiding] ([leidingID])
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar] CHECK CONSTRAINT [FK_LeidingInAfdelingsJaar_Leiding]
GO
ALTER TABLE [lid].[Lid]  WITH CHECK ADD  CONSTRAINT [FK_Lid_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [lid].[Lid] CHECK CONSTRAINT [FK_Lid_GelieerdePersoon]
GO
ALTER TABLE [lid].[Lid]  WITH CHECK ADD  CONSTRAINT [FK_Lid_GroepsWerkjaar] FOREIGN KEY([GroepsWerkjaarID])
REFERENCES [grp].[GroepsWerkJaar] ([GroepsWerkJaarID])
GO
ALTER TABLE [lid].[Lid] CHECK CONSTRAINT [FK_Lid_GroepsWerkjaar]
GO
ALTER TABLE [lid].[LidFunctie]  WITH CHECK ADD  CONSTRAINT [FK_LidFunctie_Functie] FOREIGN KEY([FunctieID])
REFERENCES [lid].[Functie] ([FunctieID])
GO
ALTER TABLE [lid].[LidFunctie] CHECK CONSTRAINT [FK_LidFunctie_Functie]
GO
ALTER TABLE [lid].[LidFunctie]  WITH CHECK ADD  CONSTRAINT [FK_LidFunctie_Lid] FOREIGN KEY([LidID])
REFERENCES [lid].[Lid] ([LidID])
GO
ALTER TABLE [lid].[LidFunctie] CHECK CONSTRAINT [FK_LidFunctie_Lid]
GO
ALTER TABLE [logging].[Bericht]  WITH CHECK ADD  CONSTRAINT [FK_Bericht_Gebruiker] FOREIGN KEY([GebruikerID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [logging].[Bericht] CHECK CONSTRAINT [FK_Bericht_Gebruiker]
GO
ALTER TABLE [logging].[Bericht]  WITH CHECK ADD  CONSTRAINT [FK_Bericht_Persoon_Id] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [logging].[Bericht] CHECK CONSTRAINT [FK_Bericht_Persoon_Id]
GO
ALTER TABLE [pers].[CommunicatieVorm]  WITH CHECK ADD  CONSTRAINT [FK_CommunicatieVorm_CommunicatieType] FOREIGN KEY([CommunicatieTypeID])
REFERENCES [pers].[CommunicatieType] ([CommunicatieTypeID])
GO
ALTER TABLE [pers].[CommunicatieVorm] CHECK CONSTRAINT [FK_CommunicatieVorm_CommunicatieType]
GO
ALTER TABLE [pers].[CommunicatieVorm]  WITH CHECK ADD  CONSTRAINT [FK_CommunicatieVorm_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [pers].[CommunicatieVorm] CHECK CONSTRAINT [FK_CommunicatieVorm_GelieerdePersoon]
GO
ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_Groep] FOREIGN KEY([GroepID])
REFERENCES [grp].[Groep] ([GroepID])
GO
ALTER TABLE [pers].[GelieerdePersoon] CHECK CONSTRAINT [FK_GelieerdePersoon_Groep]
GO
ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [pers].[GelieerdePersoon] CHECK CONSTRAINT [FK_GelieerdePersoon_Persoon]
GO
ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_PersoonsAdres] FOREIGN KEY([VoorkeursAdresID])
REFERENCES [pers].[PersoonsAdres] ([PersoonsAdresID])
GO
ALTER TABLE [pers].[GelieerdePersoon] CHECK CONSTRAINT [FK_GelieerdePersoon_PersoonsAdres]
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_Adres] FOREIGN KEY([AdresID])
REFERENCES [adr].[Adres] ([AdresID])
GO
ALTER TABLE [pers].[PersoonsAdres] CHECK CONSTRAINT [FK_PersoonsAdres_Adres]
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_AdresType] FOREIGN KEY([AdresTypeID])
REFERENCES [pers].[AdresType] ([AdresTypeID])
GO
ALTER TABLE [pers].[PersoonsAdres] CHECK CONSTRAINT [FK_PersoonsAdres_AdresType]
GO
ALTER TABLE [pers].[PersoonsAdres]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsAdres_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [pers].[PersoonsAdres] CHECK CONSTRAINT [FK_PersoonsAdres_Persoon]
GO
ALTER TABLE [pers].[PersoonsCategorie]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsCategorie_Categorie] FOREIGN KEY([CategorieID])
REFERENCES [core].[Categorie] ([CategorieID])
GO
ALTER TABLE [pers].[PersoonsCategorie] CHECK CONSTRAINT [FK_PersoonsCategorie_Categorie]
GO
ALTER TABLE [pers].[PersoonsCategorie]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsCategorie_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [pers].[PersoonsCategorie] CHECK CONSTRAINT [FK_PersoonsCategorie_GelieerdePersoon]
GO
ALTER TABLE [pers].[PersoonVrijVeld]  WITH CHECK ADD  CONSTRAINT [FK_Persoon_PersoonVrijVeldType] FOREIGN KEY([PersoonVrijVeldTypeID])
REFERENCES [pers].[PersoonVrijVeldType] ([PersoonVrijVeldTypeID])
GO
ALTER TABLE [pers].[PersoonVrijVeld] CHECK CONSTRAINT [FK_Persoon_PersoonVrijVeldType]
GO
ALTER TABLE [pers].[PersoonVrijVeld]  WITH CHECK ADD  CONSTRAINT [FK_PersoonVrijVeld_GelieerdePersoon] FOREIGN KEY([GelieerdePersoonID])
REFERENCES [pers].[GelieerdePersoon] ([GelieerdePersoonID])
GO
ALTER TABLE [pers].[PersoonVrijVeld] CHECK CONSTRAINT [FK_PersoonVrijVeld_GelieerdePersoon]
GO
ALTER TABLE [pers].[PersoonVrijVeldType]  WITH CHECK ADD  CONSTRAINT [FK_PersoonVrijVeldType_VrijVeldType] FOREIGN KEY([PersoonVrijVeldTypeID])
REFERENCES [core].[VrijVeldType] ([VrijVeldTypeID])
GO
ALTER TABLE [pers].[PersoonVrijVeldType] CHECK CONSTRAINT [FK_PersoonVrijVeldType_VrijVeldType]
GO
ALTER TABLE [verz].[PersoonsVerzekering]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsVerzekering_Persoon] FOREIGN KEY([PersoonID])
REFERENCES [pers].[Persoon] ([PersoonID])
GO
ALTER TABLE [verz].[PersoonsVerzekering] CHECK CONSTRAINT [FK_PersoonsVerzekering_Persoon]
GO
ALTER TABLE [verz].[PersoonsVerzekering]  WITH CHECK ADD  CONSTRAINT [FK_PersoonsVerzekering_VerzekeringsType] FOREIGN KEY([VerzekeringsTypeID])
REFERENCES [verz].[VerzekeringsType] ([VerzekeringsTypeID])
GO
ALTER TABLE [verz].[PersoonsVerzekering] CHECK CONSTRAINT [FK_PersoonsVerzekering_VerzekeringsType]
GO
