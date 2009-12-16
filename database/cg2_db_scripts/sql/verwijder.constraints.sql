-- ---------
-- -- adr --
-- ---------
ALTER TABLE [adr].[Adres] DROP CONSTRAINT [FK_Adres_Straat]
GO
ALTER TABLE [adr].[Adres] DROP CONSTRAINT [FK_Adres_Subgemeente]
GO
ALTER TABLE [adr].[PostNrNaarGemeente] DROP CONSTRAINT [FK_PostCodeNaarGemeente_Gemeente]
GO
ALTER TABLE [adr].[PostNrNaarGemeente] DROP CONSTRAINT [FK_PostCodeNaarGemeentes_PostCode]
GO
ALTER TABLE [adr].[Straat] DROP CONSTRAINT [FK_Straat_PostCode]
GO
ALTER TABLE [adr].[Subgemeente] DROP CONSTRAINT [FK_SubGemeente_PostCode]
GO

-- ----------
-- -- auth --
-- ----------
ALTER TABLE [auth].[GebruikersRecht] DROP CONSTRAINT [FK_GebruikersRecht_Gav]
GO
ALTER TABLE [auth].[GebruikersRecht] DROP CONSTRAINT [FK_GebruikersRecht_Groep]
GO

-- ----------
-- -- core --
-- ----------
ALTER TABLE [core].[Categorie] DROP CONSTRAINT [FK_Categorie_Groep]
GO
ALTER TABLE [core].[VrijVeldType] DROP CONSTRAINT [FK_VrijVeldType_Groep]
GO

-- ---------
-- -- grp --
-- ---------
ALTER TABLE [grp].[ChiroGroep] DROP CONSTRAINT [FK_ChiroGroep_Groep]
GO
ALTER TABLE [grp].[GroepsAdres] DROP CONSTRAINT [FK_GroepsAdres_Adres]
GO
ALTER TABLE [grp].[GroepsAdres] DROP CONSTRAINT [FK_GroepsAdres_Groep]
GO
ALTER TABLE [grp].[GroepsWerkJaar] DROP CONSTRAINT [FK_GroepsWerkjaar_Groep]
GO

-- ---------
-- -- lid --
-- ---------
ALTER TABLE [lid].[Afdeling] DROP CONSTRAINT [FK_Afdeling_Groep]
GO
ALTER TABLE [lid].[AfdelingsJaar] DROP CONSTRAINT [FK_AfdelingsJaar_Afdeling]
GO
ALTER TABLE [lid].[AfdelingsJaar] DROP CONSTRAINT [FK_AfdelingsJaar_GroepsWerkjaar]
GO
ALTER TABLE [lid].[AfdelingsJaar] DROP CONSTRAINT [FK_AfdelingsJaar_OfficieleAfdeling]
GO
ALTER TABLE [lid].[Kind] DROP CONSTRAINT [FK_Kind_AfdelingsJaar]
GO
ALTER TABLE [lid].[Kind] DROP CONSTRAINT [FK_Kind_Lid]
GO
ALTER TABLE [lid].[Leiding] DROP CONSTRAINT [FK_Leiding_Lid]
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar] DROP CONSTRAINT [FK_LeidingInAfdelingsJaar_AfdelingsJaar]
GO
ALTER TABLE [lid].[LeidingInAfdelingsJaar] DROP CONSTRAINT [FK_LeidingInAfdelingsJaar_Leiding]
GO
ALTER TABLE [lid].[Lid] DROP CONSTRAINT [FK_Lid_GelieerdePersoon]
GO
ALTER TABLE [lid].[Lid] DROP CONSTRAINT [FK_Lid_GroepsWerkjaar]
GO

-- ----------
-- -- pers --
-- ----------
ALTER TABLE [pers].[CommunicatieVorm] DROP CONSTRAINT [FK_CommunicatieVorm_CommunicatieType]
GO
ALTER TABLE [pers].[CommunicatieVorm] DROP CONSTRAINT [FK_CommunicatieVorm_GelieerdePersoon]
GO
ALTER TABLE [pers].[GelieerdePersoon] DROP CONSTRAINT [FK_GelieerdePersoon_Groep]
GO
ALTER TABLE [pers].[GelieerdePersoon] DROP CONSTRAINT [FK_GelieerdePersoon_Persoon]
GO
ALTER TABLE [pers].[PersoonsAdres] DROP CONSTRAINT [FK_PersoonsAdres_Adres]
GO
ALTER TABLE [pers].[PersoonsAdres] DROP CONSTRAINT [FK_PersoonsAdres_AdresType]
GO
ALTER TABLE [pers].[PersoonsAdres] DROP CONSTRAINT [FK_PersoonsAdres_Persoon]
GO
ALTER TABLE [pers].[PersoonsCategorie] DROP CONSTRAINT [FK_PersoonsCategorie_Categorie]
GO
ALTER TABLE [pers].[PersoonsCategorie] DROP CONSTRAINT [FK_PersoonsCategorie_GelieerdePersoon]
GO
ALTER TABLE [pers].[PersoonVrijVeld] DROP CONSTRAINT [FK_Persoon_PersoonVrijVeldType]
GO
ALTER TABLE [pers].[PersoonVrijVeld] DROP CONSTRAINT [FK_PersoonVrijVeld_GelieerdePersoon]
GO
ALTER TABLE [pers].[PersoonVrijVeldType] DROP CONSTRAINT [FK_PersoonVrijVeldType_VrijVeldType]  
GO
