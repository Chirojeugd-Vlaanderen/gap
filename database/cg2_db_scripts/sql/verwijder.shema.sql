SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[ufnSoundEx]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [core].[ufnSoundEx]
GO

DROP TABLE [adr].[Adres]
GO
DROP TABLE [adr].[Gemeente]
GO
DROP TABLE [adr].[PostNr]
GO
DROP TABLE [adr].[PostNrNaarGemeente]
GO
DROP TABLE [adr].[Straat]
GO
DROP TABLE [adr].[Subgemeente]
GO
DROP TABLE [auth].[Gav]
GO
DROP TABLE [auth].[GebruikersRecht]
GO
DROP TABLE [core].[Categorie]
GO
DROP TABLE [core].[VrijVeldType]
GO
DROP TABLE [grp].[ChiroGroep]
GO
DROP TABLE [grp].[Groep]
GO
DROP TABLE [grp].[GroepsAdres]
GO
DROP TABLE [grp].[GroepsWerkJaar]
GO
DROP TABLE [lid].[Afdeling]
GO
DROP TABLE [lid].[AfdelingsJaar]
GO
DROP TABLE [lid].[Kind]
GO
DROP TABLE [lid].[Leiding]
GO
DROP TABLE [lid].[LeidingInAfdelingsJaar]
GO
DROP TABLE [lid].[Lid]
GO
DROP TABLE [lid].[OfficieleAfdeling]
GO
DROP TABLE [pers].[AdresType]
GO
DROP TABLE [pers].[CommunicatieType]
GO
DROP TABLE [pers].[CommunicatieVorm]
GO
DROP TABLE [pers].[GelieerdePersoon]
GO
DROP TABLE [pers].[Persoon]
GO
DROP TABLE [pers].[PersoonsAdres]
GO
DROP TABLE [pers].[PersoonsCategorie]
GO
DROP TABLE [pers].[PersoonVrijVeld]
GO
DROP TABLE [pers].[PersoonVrijVeldType]
GO

-- Drop het schema
DROP SCHEMA [grp]
GO
DROP SCHEMA [pers]
GO
DROP SCHEMA [znd]
GO
DROP SCHEMA [lid]
GO
DROP SCHEMA [adr]
GO
DROP SCHEMA [core]
GO
DROP SCHEMA [auth]
GO
