SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Createie van de Schema's
EXEC sys.sp_executesql N'CREATE SCHEMA [adr] AUTHORIZATION [dbo]'
GO
EXEC sys.sp_executesql N'CREATE SCHEMA [auth] AUTHORIZATION [dbo]'
GO
EXEC sys.sp_executesql N'CREATE SCHEMA [core] AUTHORIZATION [dbo]'
GO
EXEC sys.sp_executesql N'CREATE SCHEMA [grp] AUTHORIZATION [dbo]'
GO
EXEC sys.sp_executesql N'CREATE SCHEMA [lid] AUTHORIZATION [dbo]'
GO
EXEC sys.sp_executesql N'CREATE SCHEMA [pers] AUTHORIZATION [dbo]'
GO
EXEC sys.sp_executesql N'CREATE SCHEMA [znd] AUTHORIZATION [dbo]'

-- Creatie van de tabellen:
-- ----------
-- --  grp --
-- ----------
BEGIN
	CREATE TABLE [grp].[Groep](
		[Naam] [varchar](160) NOT NULL,
		[Code] [char](10) NULL,
		[OprichtingsJaar] [int] NULL,
		[WebSite] [varchar](160) NULL,
		[Logo] [image] NULL,
		[GroepID] [int] IDENTITY(1,1) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Groep] PRIMARY KEY CLUSTERED ([GroepID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Groep_Code] ON [grp].[Groep] ([Code] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

-- ----------
-- --  adr --
-- ----------
BEGIN
	CREATE TABLE [adr].[Gemeente](
		[GemeenteID] [int] IDENTITY(1,1) NOT NULL,
		[NisGemeenteCode] [decimal](5, 0) NOT NULL,
		[Taal] [char](2) NOT NULL,
		[Naam] [varchar](40) NOT NULL,
		[Versie] [timestamp] NULL, 
		CONSTRAINT [PK_Gemeente] PRIMARY KEY CLUSTERED ([GemeenteID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
CREATE UNIQUE NONCLUSTERED INDEX [Gemeente_NisGemeenteCode] ON [adr].[Gemeente] ([NisGemeenteCode] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

--

BEGIN
	CREATE TABLE [adr].[PostNr](
		[PostNr] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_PostCode] PRIMARY KEY CLUSTERED ([PostNr] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

-- ----------
-- --  lid --
-- ----------
BEGIN
	CREATE TABLE [lid].[OfficieleAfdeling](
		[Naam] [varchar](50) NOT NULL,
		[officieleAfdelingID] [int] IDENTITY(1,1) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_OfficieleAfdeling] PRIMARY KEY CLUSTERED ([officieleAfdelingID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [UQ_OfficieleAfdeling_Naam] UNIQUE NONCLUSTERED ([Naam] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty 
		@name=N'MS_Description', 
		@value=N'De groepen kunnen ieder werkjaar naar hartelust zelf afdelingen uitvinden.  Maar elke afdeling moet gekoppeld zijn aan een ''officiele afdeling''.  De lijst van officiele afdelingen wordt nationaal beheerd.' ,
		@level0type=N'SCHEMA', 
		@level0name=N'lid', 
		@level1type=N'TABLE', 
		@level1name=N'OfficieleAfdeling'
GO

-- -----------
-- --  pers --
-- -----------
BEGIN
	CREATE TABLE [pers].[CommunicatieType](
		[Omschrijving] [varchar](80) NULL,
		[Validatie] [varchar](160) NULL,
		[CommunicatieTypeID] [int] IDENTITY(1,1) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_CommunicatieType] PRIMARY KEY CLUSTERED ([CommunicatieTypeID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

--

BEGIN
	CREATE TABLE [pers].[AdresType](
		[Omschrijving] [varchar](80) NULL,
		[AdresTypeID] [int] IDENTITY(1,1) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_AdresType] PRIMARY KEY CLUSTERED ([AdresTypeID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [auth].[Gav](
		[GavID] [int] IDENTITY(1,1) NOT NULL,
		[Login] [varchar](40) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Gav] PRIMARY KEY CLUSTERED ([GavID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Gav_Login] ON [auth].[Gav] ([Login] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

--

BEGIN
	CREATE TABLE [pers].[Persoon](
		[AdNummer] [int] NULL,
		[Naam] [varchar](160) NOT NULL,
		[VoorNaam] [varchar](60) NOT NULL,
		[GeboorteDatum] [smalldatetime] NULL,
		[Geslacht] [int] NOT NULL,
		[SterfDatum] [smalldatetime] NULL,
		[PersoonID] [int] IDENTITY(1,1) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Persoon] PRIMARY KEY CLUSTERED ([PersoonID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [pers].[PersoonsAdres](
		[Opmerking] [text] NULL,
		[AdresID] [int] NOT NULL,
		[AdresTypeID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		[PersoonsAdresID] [int] IDENTITY(1,1) NOT NULL,
		[PersoonID] [int] NOT NULL,
		CONSTRAINT [PK_PersoonsAdres] PRIMARY KEY CLUSTERED ([PersoonsAdresID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [grp].[GroepsAdres](
		[AdresID] [int] NOT NULL,
		[GroepID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
	CONSTRAINT [PK_GroepsAdres] PRIMARY KEY CLUSTERED ([GroepID] ASC,[AdresID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [lid].[Lid](
		[LidgeldBetaald] [bit] NULL,
		[NonActief] [bit] NULL,
		[Verwijderd] [bit] NULL,
		[VolgendWerkjaar] [smallint] NULL,
		[LidID] [int] IDENTITY(1,1) NOT NULL,
		[GroepsWerkjaarID] [int] NOT NULL,
		[GelieerdePersoonID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Lid] PRIMARY KEY NONCLUSTERED ([LidID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_Lid] ON [lid].[Lid] ([GroepsWerkjaarID] ASC, 	[GelieerdePersoonID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Als de groep ervan uitgaat dat het lid niet meer komt, kan het lid op non actief gezet worden.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Lid', 
	@level2type=N'COLUMN', 
	@level2name=N'NonActief'

GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Voor het einde van de instapperiode kunnen leden verwijderd worden.  Dat wordt hier aangevinkt.  Het record wordt niet verwijderd, om op die manier te kunnen onthouden dat de persoon al kandidaatlid is geweest.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Lid', 
	@level2type=N'COLUMN', 
	@level2name=N'Verwijderd'

GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Bij een nieuw werkjaar zijn er geen leden meer.  Maar aan de hand van wat er in ''VolgendWerkjaar'' staat, kan het programma aangeven of er nog leden zijn die waarschijnlijk nog ingeschreven moeten worden.

Als een lid van vorig werkjaar als toekomstperspectief ''KomtTerug'' heeft, dan herinnert het programma er aan dat die persoon nog aangesloten moet worden.  Is het toekomstperspectief ''KomtNietTerug'', dan zal het programma dat niet doen.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Lid', 
	@level2type=N'COLUMN', 
	@level2name=N'VolgendWerkjaar'

GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Zowel leiding als ''kinderen'' zijn leden.  Van zodra je als lid in de database zit, ben je ''aangesloten'', en kan je in principe een factuur krijgen.

Een lid is standaard verzekerd voor burgerlijke aansprakelijkheid.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Lid'
GO

BEGIN
	CREATE TABLE [lid].[AfdelingsJaar](
		[GeboorteJaarTot] [int] NOT NULL,
		[GeboorteJaarVan] [int] NOT NULL,
		[AfdelingsJaarID] [int] IDENTITY(1,1) NOT NULL,
		[AfdelingID] [int] NOT NULL,
		[GroepsWerkJaarID] [int] NOT NULL,
		[OfficieleAfdelingID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_AfdelingsJaar] PRIMARY KEY CLUSTERED ([AfdelingsJaarID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [UQ_AfdelingsJaar] UNIQUE NONCLUSTERED ([GroepsWerkJaarID] ASC,[AfdelingID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'geboortejaar ''tot en met''' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'AfdelingsJaar', 
	@level2type=N'COLUMN', 
	@level2name=N'GeboorteJaarTot'

GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'geboortejaar ''vanaf''' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'AfdelingsJaar', 
	@level2type=N'COLUMN', 
	@level2name=N'GeboorteJaarVan'
	
GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'De velden ''van'' en ''tot'' geven aan welke leden er standaard in deze afdeling terecht komen.

Als voor ''ketiranten 2007-2008'' van=1991 en tot=1993, dan zullen leden die geboren zijn van 1991 t/m 1993 automatisch ''ketirant'' worden als ze zich aansluiten voor 2007-2008' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'AfdelingsJaar'
GO

BEGIN
	CREATE TABLE [core].[VrijVeldType](
		[Naam] [varchar](80) NULL,
		[DataType] [int] NOT NULL,
		[VrijVeldTypeID] [int] NOT NULL,
		[GroepID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_VrijVeldType] PRIMARY KEY CLUSTERED ([VrijVeldTypeID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [lid].[Afdeling](
		[AfdelingsNaam] [varchar](50) NOT NULL,
		[Afkorting] [varchar](10) NOT NULL,
		[AfdelingID] [int] IDENTITY(1,1) NOT NULL,
		[GroepID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Afdeling] PRIMARY KEY CLUSTERED ([AfdelingID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [AK_Afdeling_GroepID_AfdelingsNaam] UNIQUE NONCLUSTERED ([GroepID] ASC,[AfdelingsNaam] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [AK_Afdeling_GroepID_Afkorting] UNIQUE NONCLUSTERED ([GroepID] ASC,[Afkorting] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'De gebruiker van het programma kan afdelingen maken zo veel hij/zij wil.  Als een afdeling bestaat in een werkjaar, dan vormt de combinatie afdeling-werkjaar een ''Afdelingsjaar''.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Afdeling'
GO

BEGIN
	CREATE TABLE [auth].[GebruikersRecht](
		[GebruikersRechtID] [int] IDENTITY(1,1) NOT NULL,
		[GavID] [int] NOT NULL,
		[GroepID] [int] NOT NULL,
		[VervalDatum] [datetime] NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Gebruikersrecht] PRIMARY KEY CLUSTERED ([GebruikersRechtID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_GebruikersRecht_GroepID_GavID] ON [auth].[GebruikersRecht] ([GroepID] ASC,[GavID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

BEGIN
	CREATE TABLE [grp].[ChiroGroep](
		[ChiroGroepID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		[Plaats] [varchar](60) NULL,
		CONSTRAINT [PK_ChiroGroep] PRIMARY KEY CLUSTERED ([ChiroGroepID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [pers].[GelieerdePersoon](
		[GroepID] [int] NOT NULL,
		[PersoonID] [int] NOT NULL,
		[ChiroLeefTijd] [int] NOT NULL CONSTRAINT [DF_GelieerdePersoon_ChiroLeefTijd]  DEFAULT ((0)),
		[GelieerdePersoonID] [int] IDENTITY(1,1) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_GelieerdePersoon] PRIMARY KEY CLUSTERED ([GelieerdePersoonID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

CREATE NONCLUSTERED INDEX [IDX_GelieerdePersoon_GroepID] ON [pers].[GelieerdePersoon] ([GroepID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_GelieerdePersoon_PersoonID] ON [pers].[GelieerdePersoon] ([PersoonID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

BEGIN
	CREATE TABLE [core].[Categorie](
		[Naam] [varchar](80) NOT NULL,
		[Code] [varchar](10) NOT NULL,
		[CategorieID] [int] IDENTITY(1,1) NOT NULL,
		[GroepID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Categorie] PRIMARY KEY CLUSTERED ([CategorieID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [AK_Categorie_GroepID_Code] UNIQUE NONCLUSTERED ([GroepID] ASC,[Code] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [AK_Categorie_GroepID_Naam] UNIQUE NONCLUSTERED ([GroepID] ASC,[Naam] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [grp].[GroepsWerkJaar](
		[WerkJaar] [int] NOT NULL,
		[GroepsWerkJaarID] [int] IDENTITY(1,1) NOT NULL,
		[GroepID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_GroepsWerkjaar] PRIMARY KEY CLUSTERED ([GroepsWerkJaarID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

CREATE UNIQUE NONCLUSTERED INDEX [IDX_GroepsWerkJaar_GroepID_WerkJaar] ON [grp].[GroepsWerkJaar] ([GroepID] ASC,[WerkJaar] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

BEGIN
	CREATE TABLE [pers].[PersoonsCategorie](
		[GelieerdePersoonID] [int] NOT NULL,
		[CategorieID] [int] NOT NULL,
		CONSTRAINT [PK_PersoonsCategorie] PRIMARY KEY CLUSTERED ([GelieerdePersoonID] ASC,[CategorieID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [pers].[CommunicatieVorm](
		[Nota] [varchar](320) NULL,
		[Nummer] [varchar](160) NULL,
		[CommunicatieVormID] [int] IDENTITY(1,1) NOT NULL,
		[CommunicatieTypeID] [int] NOT NULL,
		[IsGezinsgebonden] [bit] NOT NULL CONSTRAINT [DF_CommunicatieVorm_IsGezinsgebonden]  DEFAULT ((0)),
		[Voorkeur] [bit] NOT NULL CONSTRAINT [DF_CommunicatieVorm_Voorkeur]  DEFAULT ((0)),
		[GelieerdePersoonID] [int] NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_CommunicatieVorm] PRIMARY KEY CLUSTERED ([CommunicatieVormID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

CREATE NONCLUSTERED INDEX [IDX_CommunicatieVorm_GelieerdePersoonID_CommunicatieVormID] ON [pers].[CommunicatieVorm] ([GelieerdePersoonID] ASC,[CommunicatieVormID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

BEGIN
	CREATE TABLE [pers].[PersoonVrijVeld](
		[Waarde] [varchar](320) NOT NULL,
		[GelieerdePersoonID] [int] NOT NULL,
		[PersoonVrijVeldTypeID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_PersoonVrijVeld] PRIMARY KEY CLUSTERED ([GelieerdePersoonID] ASC,[PersoonVrijVeldTypeID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [adr].[PostNrNaarGemeente](
		[GemeenteID] [int] NOT NULL,
		[PostNr] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_PostcodeNaarGemeente] PRIMARY KEY CLUSTERED ([GemeenteID] ASC,[PostNr] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

BEGIN
	CREATE TABLE [adr].[Straat](
		[StraatID] [int] IDENTITY(1,1) NOT NULL,
		[PostNr] [int] NOT NULL,
		[Naam] [varchar](80) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Straat] PRIMARY KEY CLUSTERED ([StraatID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

BEGIN
	CREATE TABLE [adr].[Subgemeente](
		[SubgemeenteID] [int] IDENTITY(1,1) NOT NULL,
		[PostNr] [int] NOT NULL,
		[Naam] [varchar](80) NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Subgemeente] PRIMARY KEY CLUSTERED ([SubgemeenteID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

BEGIN
	CREATE TABLE [adr].[Adres](
		[Bus] [varchar](10) NOT NULL,
		[HuisNr] [int] NULL,
		[PostCode] [varchar](10) NOT NULL,
		[AdresID] [int] IDENTITY(1,1) NOT NULL,
		[StraatID] [int] NOT NULL,
		[SubgemeenteID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Adres] PRIMARY KEY CLUSTERED ([AdresID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
		CONSTRAINT [AK_Adres] UNIQUE NONCLUSTERED ([StraatID] ASC,[HuisNr] ASC,[Bus] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

BEGIN
	CREATE TABLE [lid].[LeidingInAfdelingsJaar](
		[AfdelingsJaarID] [int] NOT NULL,
		[LeidingID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_LeidingInAfdelingsJaar] PRIMARY KEY CLUSTERED ([LeidingID] ASC,[AfdelingsJaarID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

BEGIN
	CREATE TABLE [lid].[Leiding](
		[DubbelPuntAbonnement] [bit] NULL,
		[leidingID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Leiding] PRIMARY KEY CLUSTERED ([leidingID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Een leid(st)er is een speciaal geval van een lid.  Gedurende een werkjaar kan een leid(st)er aan meerdere afdelingsjaren gekoppeld zijn.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Leiding'
GO

BEGIN
	CREATE TABLE [lid].[Kind](
		[EindeInstapPeriode] [smalldatetime] NULL,
		[kindID] [int] NOT NULL,
		[afdelingsJaarID] [int] NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_Kind] PRIMARY KEY CLUSTERED ([kindID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Er worden enkel  facturen gemaakt voor dit lid als deze datum gepasseerd is.  Tot dat moment kan de groep beslissen om deze persoon uit te schrijven.  Daarna vanzelfsprekend niet meer.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Kind', 
	@level2type=N'COLUMN', 
	@level2name=N'EindeInstapPeriode'
GO

EXEC sys.sp_addextendedproperty 
	@name=N'MS_Description', 
	@value=N'Een ''Kind'' is een lid dat geen leiding/kadermedewerker is.  Een kind is altijd gekoppeld aan een ''afdelingsjaar'' voor het werkjaar waarin het lid is.' ,
	@level0type=N'SCHEMA', 
	@level0name=N'lid', 
	@level1type=N'TABLE', 
	@level1name=N'Kind'
GO

BEGIN
	CREATE TABLE [pers].[PersoonVrijVeldType](
		[PersoonVrijVeldTypeID] [int] NOT NULL,
		[Versie] [timestamp] NULL,
		CONSTRAINT [PK_PersoonVrijVeldType] PRIMARY KEY CLUSTERED ([PersoonVrijVeldTypeID] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
