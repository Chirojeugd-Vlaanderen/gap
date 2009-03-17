USE ChiroGroep
go

--CREATE SCHEMA lid
--go

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id('FK_Lid_GroepsWerkjaar') AND OBJECTPROPERTY(id, 'IsForeignKey') = 1)
ALTER TABLE lid.Lid DROP CONSTRAINT FK_Lid_GroepsWerkjaar
go

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id('FK_Lid_Persoon') AND OBJECTPROPERTY(id, 'IsForeignKey') = 1)
ALTER TABLE lid.Lid DROP CONSTRAINT FK_Lid_Persoon
go

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id('FK_GroepsWerkjaar_Groep') AND OBJECTPROPERTY(id, 'IsForeignKey') = 1)
ALTER TABLE grp.GroepsWerkJaar DROP CONSTRAINT FK_GroepsWerkjaar_Groep
go

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS WHERE id = object_id('lid.Lid') AND  OBJECTPROPERTY(id, 'IsUserTable') = 1)
DROP TABLE lid.Lid
go

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS WHERE id = object_id('grp.GroepsWerkJaar') AND  OBJECTPROPERTY(id, 'IsUserTable') = 1)
DROP TABLE grp.GroepsWerkJaar
go

CREATE TABLE lid.Lid ( 
	EindeInstapPeriode datetime,
	LidgeldBetaald bit NOT NULL DEFAULT 0,
	NonActief bit NOT NULL DEFAULT 0,
	Verwijderd bit NOT NULL DEFAULT 0,
	VolgendWerkjaar integer NOT NULL DEFAULT 0,
	lidID Integer NOT NULL,
	GroepsWerkjaarID Integer NOT NULL,
	PersoonID Integer NOT NULL
)
go

CREATE TABLE grp.GroepsWerkJaar ( 
	WerkJaar int NOT NULL,
	GroepsWerkjaarID Integer NOT NULL,
	GroepID Integer IDENTITY (1,1) NOT NULL
)
go


ALTER TABLE lid.Lid ADD CONSTRAINT PK_Lid 
	PRIMARY KEY CLUSTERED (lidID)
go

ALTER TABLE grp.GroepsWerkJaar ADD CONSTRAINT PK_GroepsWerkjaar 
	PRIMARY KEY CLUSTERED (groepsWerkjaarID)
go

ALTER TABLE lid.Lid ADD CONSTRAINT FK_Lid_GroepsWerkjaar 
	FOREIGN KEY (groepsWerkjaarID) REFERENCES grp.GroepsWerkJaar (groepsWerkjaarID)
go

ALTER TABLE lid.Lid ADD CONSTRAINT FK_Lid_Persoon 
	FOREIGN KEY (persoonID) REFERENCES pers.Persoon (persoonID)
go

ALTER TABLE grp.GroepsWerkJaar ADD CONSTRAINT FK_GroepsWerkjaar_Groep 
	FOREIGN KEY (groepID) REFERENCES grp.Groep (groepID)
go

-- wijzigingen achteraf...

ALTER TABLE lid.Lid
	DROP CONSTRAINT FK_Lid_Persoon
GO
COMMIT
BEGIN TRANSACTION
GO
COMMIT
BEGIN TRANSACTION
GO
EXECUTE sp_rename N'lid.Lid.PersoonID', N'Tmp_GelieerdePersoonID', 'COLUMN' 
GO
EXECUTE sp_rename N'lid.Lid.Tmp_GelieerdePersoonID', N'GelieerdePersoonID', 'COLUMN' 
GO
ALTER TABLE lid.Lid ADD CONSTRAINT
	FK_Lid_GelieerdePersoon FOREIGN KEY
	(
	GelieerdePersoonID
	) REFERENCES pers.GelieerdePersoon
	(
	GelieerdePersoonID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE lid.Lid
	DROP CONSTRAINT FK_Lid_GelieerdePersoon
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE lid.Lid ADD CONSTRAINT
	FK_Lid_GelieerdePersoon FOREIGN KEY
	(
	LidID
	) REFERENCES pers.GelieerdePersoon
	(
	GelieerdePersoonID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE lid.Lid
	DROP COLUMN GelieerdePersoonID
GO


-- het was niet juist :-(

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE lid.Lid
	DROP CONSTRAINT FK_Lid_GelieerdePersoon
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE lid.Lid
	DROP CONSTRAINT FK_Lid_GroepsWerkjaar
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE lid.Lid
	DROP CONSTRAINT DF__Lid__LidgeldBeta__056ECC6A
GO
ALTER TABLE lid.Lid
	DROP CONSTRAINT DF__Lid__NonActief__0662F0A3
GO
ALTER TABLE lid.Lid
	DROP CONSTRAINT DF__Lid__Verwijderd__075714DC
GO
ALTER TABLE lid.Lid
	DROP CONSTRAINT DF__Lid__VolgendWerk__084B3915
GO
CREATE TABLE lid.Tmp_Lid
	(
	EindeInstapPeriode datetime NULL,
	LidgeldBetaald bit NOT NULL,
	NonActief bit NOT NULL,
	Verwijderd bit NOT NULL,
	VolgendWerkjaar int NOT NULL,
	LidID int NOT NULL IDENTITY (1, 1),
	GroepsWerkjaarID int NOT NULL,
	GelieerdePersoonID int NOT NULL,
	Guid uniqueidentifier NOT NULL,
	Versie timestamp NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE lid.Tmp_Lid ADD CONSTRAINT
	DF__Lid__LidgeldBeta__056ECC6A DEFAULT ((0)) FOR LidgeldBetaald
GO
ALTER TABLE lid.Tmp_Lid ADD CONSTRAINT
	DF__Lid__NonActief__0662F0A3 DEFAULT ((0)) FOR NonActief
GO
ALTER TABLE lid.Tmp_Lid ADD CONSTRAINT
	DF__Lid__Verwijderd__075714DC DEFAULT ((0)) FOR Verwijderd
GO
ALTER TABLE lid.Tmp_Lid ADD CONSTRAINT
	DF__Lid__VolgendWerk__084B3915 DEFAULT ((0)) FOR VolgendWerkjaar
GO
SET IDENTITY_INSERT lid.Tmp_Lid ON
GO
IF EXISTS(SELECT * FROM lid.Lid)
	 EXEC('INSERT INTO lid.Tmp_Lid (EindeInstapPeriode, LidgeldBetaald, NonActief, Verwijderd, VolgendWerkjaar, LidID, GroepsWerkjaarID)
		SELECT EindeInstapPeriode, LidgeldBetaald, NonActief, Verwijderd, VolgendWerkjaar, LidID, GroepsWerkjaarID FROM lid.Lid WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT lid.Tmp_Lid OFF
GO
DROP TABLE lid.Lid
GO
EXECUTE sp_rename N'lid.Tmp_Lid', N'Lid', 'OBJECT' 
GO
ALTER TABLE lid.Lid ADD CONSTRAINT
	PK_Lid PRIMARY KEY CLUSTERED 
	(
	LidID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE lid.Lid ADD CONSTRAINT
	FK_Lid_GroepsWerkjaar FOREIGN KEY
	(
	GroepsWerkjaarID
	) REFERENCES grp.GroepsWerkJaar
	(
	GroepsWerkjaarID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE lid.Lid ADD CONSTRAINT
	FK_Lid_GelieerdePersoon FOREIGN KEY
	(
	GelieerdePersoonID
	) REFERENCES pers.GelieerdePersoon
	(
	GelieerdePersoonID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
go

-- unique key 
CREATE UNIQUE NONCLUSTERED INDEX AK_Lid_GelieerdePersoonID_GroepsWerkJaarID ON lid.Lid
	(
	GelieerdePersoonID,
	GroepsWerkjaarID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO