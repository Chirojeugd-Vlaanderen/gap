USE ChiroGroep
GO

DROP TABLE auth.GebruikersRecht;
DROP TABLE auth.Gav;
GO

--CREATE SCHEMA auth
--GO

CREATE TABLE auth.Gav
(
	GavID INT NOT NULL IDENTITY(1,1)
	, Login VARCHAR(40) NOT NULL
);
GO

ALTER TABLE auth.Gav ADD CONSTRAINT PK_Gav
	PRIMARY KEY CLUSTERED(GavID)
GO

CREATE UNIQUE INDEX AK_Gav_Login ON auth.Gav(Login);
GO

CREATE TABLE auth.GebruikersRecht
(
	GebruikersRechtID INT NOT NULL IDENTITY(1,1)
	, GavID INT NOT NULL
    , GroepID INT NOT NULL
	, VervalDatum DATETIME
	, Guid UNIQUEIDENTIFIER NOT NULL
	, Versie TIMESTAMP
);
GO

ALTER TABLE auth.GebruikersRecht ADD CONSTRAINT PK_Gebruikersrecht
PRIMARY KEY CLUSTERED(GebruikersRechtID)
GO

ALTER TABLE auth.GebruikersRecht ADD CONSTRAINT FK_GebruikersRecht_Gav
FOREIGN KEY(GavID) REFERENCES auth.Gav(GavID);
GO

ALTER TABLE auth.GebruikersRecht ADD CONSTRAINT FK_GebruikersRecht_Groep
FOREIGN KEY(GroepID) REFERENCES grp.Groep(GroepID);
GO

CREATE UNIQUE INDEX AK_GebruikersRecht_GroepID_GavID 
ON auth.GebruikersRecht(GroepID, GavID);
GO

