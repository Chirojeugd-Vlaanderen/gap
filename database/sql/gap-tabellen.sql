CREATE SCHEMA [abo]
GO
CREATE SCHEMA [adr]
GO
CREATE SCHEMA [auth]
GO
CREATE SCHEMA [biv]
GO
CREATE SCHEMA [core]
GO
CREATE SCHEMA [data]
GO
CREATE SCHEMA [diag]
GO
CREATE SCHEMA [GapStats]
GO
CREATE SCHEMA [grp]
GO
CREATE SCHEMA [lid]
GO
CREATE SCHEMA [logging]
GO
CREATE SCHEMA [pers]
GO
CREATE SCHEMA [verz]
GO

/****** Object:  Table [abo].[Abonnement]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [abo].[Abonnement]( [AbonnementID] [int] IDENTITY(1,1) NOT NULL, [GelieerdePersoonID] [int] NOT NULL, [PublicatieID] [int] NOT NULL, [GroepsWerkJaarID] [int] NOT NULL, [AanvraagDatum] [datetime] NOT NULL, [Versie] [timestamp] NULL, [Type] [int] NOT NULL, PRIMARY KEY CLUSTERED ( [AbonnementID] ASC))

GO
/****** Object:  Table [abo].[Publicatie]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [abo].[Publicatie]( [PublicatieID] [int] IDENTITY(1,1) NOT NULL, [Naam] [varchar](80) NOT NULL, [Versie] [timestamp] NULL, [Actief] [bit] NOT NULL, PRIMARY KEY CLUSTERED ( [PublicatieID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[Adres]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[Adres]( [Bus] [varchar](10) NULL, [HuisNr] [int] NULL, [PostCode] [varchar](10) NULL, [AdresID] [int] IDENTITY(1,1) NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_Adres] PRIMARY KEY CLUSTERED ( [AdresID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[BelgischAdres]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [adr].[BelgischAdres]( [BelgischAdresID] [int] NOT NULL, [StraatNaamID] [int] NOT NULL, [WoonPlaatsID] [int] NOT NULL, PRIMARY KEY CLUSTERED ( [BelgischAdresID] ASC))

GO
/****** Object:  Table [adr].[BuitenLandsAdres]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[BuitenLandsAdres]( [BuitenlandsAdresID] [int] NOT NULL, [PostCode] [varchar](16) NULL, [Straat] [varchar](80) NOT NULL, [WoonPlaats] [varchar](80) NOT NULL, [LandID] [int] NOT NULL, PRIMARY KEY CLUSTERED ( [BuitenlandsAdresID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[Land]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[Land]( [LandID] [int] IDENTITY(1,1) NOT NULL, [Naam] [varchar](80) NOT NULL, [IsoCode] [varchar](10) NOT NULL, PRIMARY KEY CLUSTERED ( [LandID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[PostNr]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [adr].[PostNr]( [PostNr] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_PostNr] PRIMARY KEY CLUSTERED ( [PostNr] ASC))

GO
/****** Object:  Table [adr].[StraatNaam]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[StraatNaam]( [StraatNaamID] [int] IDENTITY(1,1) NOT NULL, [PostNummer] [int] NOT NULL, [Naam] [varchar](80) NOT NULL, [TaalID] [int] NOT NULL, [CrabSubstraatID] [int] NULL, [Versie] [timestamp] NOT NULL, PRIMARY KEY CLUSTERED ( [StraatNaamID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [adr].[WoonPlaats]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [adr].[WoonPlaats]( [WoonPlaatsID] [int] IDENTITY(1,1) NOT NULL, [PostNummer] [int] NOT NULL, [Naam] [varchar](80) NOT NULL, [TaalID] [int] NOT NULL, [CrabPostKantonID] [int] NULL, [Versie] [timestamp] NOT NULL, PRIMARY KEY CLUSTERED ( [WoonPlaatsID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [auth].[GebruikersRechtv2]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [auth].[GebruikersRechtv2]( [GebruikersRechtV2ID] [int] IDENTITY(1,1) NOT NULL, [PersoonID] [int] NOT NULL, [GroepID] [int] NOT NULL, [VervalDatum] [datetime] NULL, [PersoonsPermissies] [int] NOT NULL, [GroepsPermissies] [int] NOT NULL, [AfdelingsPermissies] [int] NOT NULL, [IedereenPermissies] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_GebruikersrechtV2] PRIMARY KEY CLUSTERED ( [GebruikersRechtV2ID] ASC))

GO
/****** Object:  Table [biv].[Deelnemer]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [biv].[Deelnemer]( [DeelnemerID] [int] IDENTITY(1,1) NOT NULL, [UitstapID] [int] NOT NULL, [GelieerdePersoonID] [int] NOT NULL, [IsLogistieker] [bit] NOT NULL, [HeeftBetaald] [bit] NOT NULL, [MedischeFicheOk] [bit] NOT NULL, [Opmerkingen] [text] NULL, [Versie] [timestamp] NOT NULL, CONSTRAINT [PK_Deelnemer] PRIMARY KEY CLUSTERED ( [DeelnemerID] ASC))

GO
/****** Object:  Table [biv].[Plaats]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [biv].[Plaats]( [PlaatsID] [int] IDENTITY(1,1) NOT NULL, [Naam] [varchar](80) NOT NULL, [AdresID] [int] NOT NULL, [GelieerdePersoonID] [int] NULL, [GroepID] [int] NOT NULL, [Versie] [timestamp] NOT NULL, CONSTRAINT [PK_Plaats] PRIMARY KEY CLUSTERED ( [PlaatsID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [biv].[Uitstap]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [biv].[Uitstap]( [UitstapID] [int] IDENTITY(1,1) NOT NULL, [Naam] [varchar](120) NOT NULL, [IsBivak] [bit] NOT NULL, [DatumVan] [datetime] NOT NULL, [DatumTot] [datetime] NOT NULL, [Opmerkingen] [text] NULL, [PlaatsID] [int] NULL, [GroepsWerkJaarID] [int] NULL, [ContactDeelnemerID] [int] NULL, [Versie] [timestamp] NOT NULL, CONSTRAINT [PK_Uitstap] PRIMARY KEY CLUSTERED ( [UitstapID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [core].[Categorie]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [core].[Categorie]( [Naam] [varchar](80) NOT NULL, [Code] [varchar](10) NOT NULL, [CategorieID] [int] IDENTITY(1,1) NOT NULL, [GroepID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_Categorie] PRIMARY KEY CLUSTERED ( [CategorieID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [core].[Taal]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [core].[Taal]( [TaalID] [int] IDENTITY(1,1) NOT NULL, [Code] [varchar](5) NOT NULL, [Naam] [varchar](30) NOT NULL, PRIMARY KEY CLUSTERED ( [TaalID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [core].[VrijVeldType]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [core].[VrijVeldType]( [Naam] [varchar](80) NULL, [DataType] [int] NOT NULL, [VrijVeldTypeID] [int] NOT NULL, [GroepID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_VrijVeldType] PRIMARY KEY CLUSTERED ( [VrijVeldTypeID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [grp].[ChiroGroep]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [grp].[ChiroGroep]( [ChiroGroepID] [int] NOT NULL, [Versie] [timestamp] NULL, [Plaats] [varchar](60) NULL, [KaderGroepID] [int] NOT NULL, CONSTRAINT [PK_ChiroGroep] PRIMARY KEY CLUSTERED ( [ChiroGroepID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [grp].[Groep]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [grp].[Groep]( [Naam] [varchar](160) NOT NULL, [Code] [char](10) NULL, [OprichtingsJaar] [int] NULL, [WebSite] [varchar](160) NULL, [Logo] [image] NULL, [GroepID] [int] IDENTITY(1,1) NOT NULL, [Versie] [timestamp] NULL, [StopDatum] [datetime] NULL, [AdresID] [int] NULL, CONSTRAINT [PK_Groep] PRIMARY KEY CLUSTERED ( [GroepID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [grp].[GroepsAdres]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [grp].[GroepsAdres]( [AdresID] [int] NOT NULL, [GroepID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_GroepsAdres] PRIMARY KEY CLUSTERED ( [GroepID] ASC, [AdresID] ASC))

GO
/****** Object:  Table [grp].[GroepsWerkJaar]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [grp].[GroepsWerkJaar]( [WerkJaar] [int] NOT NULL, [GroepsWerkJaarID] [int] IDENTITY(1,1) NOT NULL, [GroepID] [int] NOT NULL, [Versie] [timestamp] NULL, [Datum] [datetime] NULL, CONSTRAINT [PK_GroepsWerkjaar] PRIMARY KEY CLUSTERED ( [GroepsWerkJaarID] ASC))

GO
/****** Object:  Table [grp].[KaderGroep]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [grp].[KaderGroep]( [KaderGroepID] [int] NOT NULL, [Niveau] [int] NOT NULL, [ParentID] [int] NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_KaderGroep] PRIMARY KEY CLUSTERED ( [KaderGroepID] ASC))

GO
/****** Object:  Table [lid].[Afdeling]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [lid].[Afdeling]( [AfdelingsNaam] [varchar](50) NOT NULL, [Afkorting] [varchar](10) NOT NULL, [AfdelingID] [int] IDENTITY(1,1) NOT NULL, [ChiroGroepID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_Afdeling] PRIMARY KEY CLUSTERED ( [AfdelingID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [lid].[AfdelingsJaar]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[AfdelingsJaar]( [GeboorteJaarTot] [int] NOT NULL, [GeboorteJaarVan] [int] NOT NULL, [AfdelingsJaarID] [int] IDENTITY(1,1) NOT NULL, [AfdelingID] [int] NOT NULL, [Geslacht] [int] NOT NULL, [GroepsWerkJaarID] [int] NOT NULL, [OfficieleAfdelingID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_AfdelingsJaar] PRIMARY KEY CLUSTERED ( [AfdelingsJaarID] ASC))

GO
/****** Object:  Table [lid].[Functie]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [lid].[Functie]( [Naam] [varchar](80) NOT NULL, [Code] [varchar](10) NOT NULL, [FunctieID] [int] IDENTITY(1,1) NOT NULL, [GroepID] [int] NULL, [Versie] [timestamp] NULL, [MaxAantal] [int] NULL, [MinAantal] [int] NOT NULL, [WerkJaarVan] [int] NULL, [WerkJaarTot] [int] NULL, [IsNationaal]  AS (case when [GroepID] IS NULL then CONVERT([bit],'true',0) else CONVERT([bit],'false',0) end), [Niveau] [int] NOT NULL, [LidType]  AS (([Niveau]&~(1))/(2)&(3)), CONSTRAINT [PK_Functie] PRIMARY KEY CLUSTERED ( [FunctieID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [lid].[Kind]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[Kind]( [kindID] [int] NOT NULL, [afdelingsJaarID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_Kind] PRIMARY KEY CLUSTERED ( [kindID] ASC))

GO
/****** Object:  Table [lid].[Leiding]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[Leiding]( [leidingID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_Leiding] PRIMARY KEY CLUSTERED ( [leidingID] ASC))

GO
/****** Object:  Table [lid].[LeidingInAfdelingsJaar]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[LeidingInAfdelingsJaar]( [AfdelingsJaarID] [int] NOT NULL, [LeidingID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_LeidingInAfdelingsJaar] PRIMARY KEY CLUSTERED ( [LeidingID] ASC, [AfdelingsJaarID] ASC))

GO
/****** Object:  Table [lid].[Lid]    Script Date: 21/02/2017 10:54:45 ******/
-- Hier zet ik alles op 1 lijn, want anders krijg ik een syntax error. wtf.
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[Lid]([LidgeldBetaald] [bit] NULL, [Verwijderd] [bit] NULL, [VolgendWerkjaar] [smallint] NULL, [LidID] [int] IDENTITY(1,1) NOT NULL, [GroepsWerkjaarID] [int] NOT NULL, [GelieerdePersoonID] [int] NOT NULL, [Versie] [timestamp] NULL, [EindeInstapPeriode] [smalldatetime] NULL, [UitschrijfDatum] [datetime] NULL, [NonActief]  AS (case when [UitschrijfDatum] IS NULL then CONVERT([bit],(0),0) else CONVERT([bit],(1),0) end), [IsAangesloten] [bit] NOT NULL)
GO
/****** Object:  Table [lid].[LidFunctie]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [lid].[LidFunctie]( [LidID] [int] NOT NULL, [FunctieID] [int] NOT NULL, CONSTRAINT [PK_LidFunctie] PRIMARY KEY CLUSTERED ( [LidID] ASC, [FunctieID] ASC))

GO
/****** Object:  Table [lid].[OfficieleAfdeling]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [lid].[OfficieleAfdeling]( [Naam] [varchar](50) NOT NULL, [OfficieleAfdelingID] [int] IDENTITY(1,1) NOT NULL, [LeefTijdVan] [int] NOT NULL, [LeefTijdTot] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_OfficieleAfdeling] PRIMARY KEY CLUSTERED ( [OfficieleAfdelingID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [logging].[Bericht]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [logging].[Bericht]( [BerichtID] [int] IDENTITY(1,1) NOT NULL, [Tijd] [datetime] NOT NULL, [Niveau] [int] NOT NULL, [Boodschap] [varchar](max) NULL, [StamNummer] [char](10) NULL, [AdNummer] [int] NULL, [PersoonID] [int] NULL, [GebruikerID] [int] NULL, CONSTRAINT [PK_Bericht] PRIMARY KEY CLUSTERED ( [BerichtID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[AdresType]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[AdresType]( [Omschrijving] [varchar](80) NULL, [AdresTypeID] [int] IDENTITY(1,1) NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_AdresType] PRIMARY KEY CLUSTERED ( [AdresTypeID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[CommunicatieType]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[CommunicatieType]( [Omschrijving] [varchar](80) NULL, [Validatie] [varchar](160) NULL, [CommunicatieTypeID] [int] IDENTITY(1,1) NOT NULL, [Voorbeeld] [varchar](160) NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_CommunicatieType] PRIMARY KEY CLUSTERED ( [CommunicatieTypeID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[CommunicatieVorm]    Script Date: 21/02/2017 10:54:45 ******/
-- Ook hier alles op 1 lijn. mssql magie.
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[CommunicatieVorm]([Nota] [varchar](320) NULL, [Nummer] [varchar](160) NOT NULL, [CommunicatieVormID] [int] IDENTITY(1,1) NOT NULL, [CommunicatieTypeID] [int] NOT NULL, [IsGezinsgebonden] [bit] NOT NULL, [Voorkeur] [bit] NOT NULL, [GelieerdePersoonID] [int] NULL, [Versie] [timestamp] NULL)
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[GelieerdePersoon]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[GelieerdePersoon]( [GroepID] [int] NOT NULL, [PersoonID] [int] NOT NULL, [ChiroLeefTijd] [int] NOT NULL, [GelieerdePersoonID] [int] IDENTITY(1,1) NOT NULL, [Versie] [timestamp] NULL, [VoorkeursAdresID] [int] NULL, CONSTRAINT [PK_GelieerdePersoon] PRIMARY KEY CLUSTERED ( [GelieerdePersoonID] ASC))

GO
/****** Object:  Table [pers].[Persoon]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[Persoon]( [AdNummer] [int] NULL, [Naam] [varchar](160) NOT NULL, [VoorNaam] [varchar](60) NOT NULL, [GeboorteDatum] [datetime] NULL, [Geslacht] [int] NOT NULL, [SterfDatum] [smalldatetime] NULL, [PersoonID] [int] IDENTITY(1,1) NOT NULL, [Versie] [timestamp] NULL, [AdInAanvraag] [bit] NOT NULL, [SeNaam]  AS (soundex([Naam])), [SeVoornaam]  AS (soundex([VoorNaam])), [NieuwsBrief] [bit] NOT NULL, CONSTRAINT [PK_Persoon] PRIMARY KEY CLUSTERED ( [PersoonID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[PersoonsAdres]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[PersoonsAdres]( [Opmerking] [text] NULL, [AdresID] [int] NOT NULL, [AdresTypeID] [int] NOT NULL, [Versie] [timestamp] NULL, [PersoonsAdresID] [int] IDENTITY(1,1) NOT NULL, [PersoonID] [int] NOT NULL, CONSTRAINT [PK_PersoonsAdres] PRIMARY KEY CLUSTERED ( [PersoonsAdresID] ASC))

GO
/****** Object:  Table [pers].[PersoonsCategorie]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[PersoonsCategorie]( [GelieerdePersoonID] [int] NOT NULL, [CategorieID] [int] NOT NULL, CONSTRAINT [PK_PersoonsCategorie] PRIMARY KEY CLUSTERED ( [GelieerdePersoonID] ASC, [CategorieID] ASC))

GO
/****** Object:  Table [pers].[PersoonVrijVeld]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [pers].[PersoonVrijVeld]( [Waarde] [varchar](320) NOT NULL, [GelieerdePersoonID] [int] NOT NULL, [PersoonVrijVeldTypeID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_PersoonVrijVeld] PRIMARY KEY CLUSTERED ( [GelieerdePersoonID] ASC, [PersoonVrijVeldTypeID] ASC))

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [pers].[PersoonVrijVeldType]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [pers].[PersoonVrijVeldType]( [PersoonVrijVeldTypeID] [int] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_PersoonVrijVeldType] PRIMARY KEY CLUSTERED ( [PersoonVrijVeldTypeID] ASC))

GO
/****** Object:  Table [verz].[PersoonsVerzekering]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [verz].[PersoonsVerzekering]( [PersoonsVerzekeringID] [int] IDENTITY(1,1) NOT NULL, [PersoonID] [int] NOT NULL, [VerzekeringsTypeID] [int] NOT NULL, [Van] [datetime] NOT NULL, [Tot] [datetime] NOT NULL, [Versie] [timestamp] NULL, CONSTRAINT [PK_PersoonsVerzekering] PRIMARY KEY CLUSTERED ( [PersoonsVerzekeringID] ASC))

GO
/****** Object:  Table [verz].[VerzekeringsType]    Script Date: 21/02/2017 10:54:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [verz].[VerzekeringsType]( [VerzekeringsTypeID] [int] IDENTITY(1,1) NOT NULL, [Code] [varchar](10) NOT NULL, [Naam] [varchar](40) NOT NULL, [Omschrijving] [text] NULL, [EnkelLeden] [bit] NOT NULL, [TotEindeWerkJaar] [bit] NOT NULL, CONSTRAINT [PK_VerzekeringsType] PRIMARY KEY CLUSTERED ( [VerzekeringsTypeID] ASC))
