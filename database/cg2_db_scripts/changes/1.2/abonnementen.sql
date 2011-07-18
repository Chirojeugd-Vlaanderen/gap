CREATE SCHEMA abo
GO

CREATE TABLE abo.Publicatie(
	PublicatieID INT NOT NULL IDENTITY(1,1),
	Naam VARCHAR(80) NOT NULL,
	Versie TIMESTAMP NULL,
	Actief BIT NOT NULL DEFAULT 1,
	PRIMARY KEY(PublicatieID))

CREATE TABLE abo.Abonnement(
	AbonnementID INT NOT NULL IDENTITY(1,1),
	GelieerdePersoonID INT NOT NULL,
	PublicatieID INT NOT NULL,
	GroepsWerkJaarID INT NOT NULL,
	AanvraagDatum DATETIME NOT NULL DEFAULT getdate(),
	Versie TIMESTAMP NULL,
	PRIMARY KEY(AbonnementID))
	
ALTER TABLE abo.Abonnement 
ADD CONSTRAINT FK_Abonnement_GelieerdePersoon FOREIGN KEY(GelieerdePersoonID) REFERENCES pers.GelieerdePersoon;

ALTER TABLE abo.Abonnement 
ADD CONSTRAINT FK_Abonnement_Publicatie FOREIGN KEY(PublicatieID) REFERENCES abo.Publicatie;

ALTER TABLE abo.Abonnement 
ADD CONSTRAINT FK_Abonnement_GroepsWerkJaar FOREIGN KEY(GroepsWerkJaarID) REFERENCES grp.GroepsWerkJaar;

CREATE INDEX IDX_Abonnement_GelieerdePersoon ON abo.Abonnement(GelieerdePersoonID);
CREATE INDEX IDX_Abonnement_GroepsWerkJaar_Publicatie ON abo.Abonnement(GroepsWerkJaarID, PublicatieID);

INSERT INTO abo.Publicatie(Naam) VALUES('Dubbelpunt')

GRANT INSERT ON abo.Abonnement TO GapRole;


ALTER TABLE auth.Gav ADD PersoonID INT NULL;
ALTER TABLE auth.Gav
ADD CONSTRAINT FK_Gav_Persoon FOREIGN KEY(PersoonID) REFERENCES pers.Persoon(PersoonID);

