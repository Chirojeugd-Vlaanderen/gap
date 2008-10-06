USE ChiroGroep
go

CREATE SCHEMA lid
go

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
	LidgeldBetaald bit,
	NonActief bit,
	Verwijderd bit,
	VolgendWerkjaar integer,
	lidID Integer NOT NULL,
	groepsWerkjaarID Integer,
	persoonID Integer
)
go

CREATE TABLE grp.GroepsWerkJaar ( 
	WerkJaar int,
	groepsWerkjaarID Integer NOT NULL,
	groepID Integer
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
