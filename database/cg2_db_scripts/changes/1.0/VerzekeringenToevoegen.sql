EXEC sys.sp_executesql N'CREATE SCHEMA [verz] AUTHORIZATION [dbo]'
GO

-- verzekeringstype

CREATE TABLE verz.VerzekeringsType(
	VerzekeringsTypeID INT IDENTITY(1,1) NOT NULL,
	Code VARCHAR(10) NOT NULL,
	Naam VARCHAR(40) NOT NULL,
	Omschrijving TEXT,
	CONSTRAINT PK_VerzekeringsType PRIMARY KEY(VerzekeringsTypeID))
GO

-- persoonsverzekering

CREATE TABLE verz.PersoonsVerzekering(
	PersoonID INT NOT NULL,
	VerzekeringsTypeID INT NOT NULL,
	Van DATETIME NOT NULL,
	Tot DATETIME NOT NULL,
	CONSTRAINT PK_PersoonsVerzekering PRIMARY KEY(PersoonID, VerzekeringsTypeID))
GO

-- foreign keys

ALTER TABLE verz.PersoonsVerzekering
ALTER TABLE verz.PersoonsVerzekering
values('LV', 'Loonverlies', 
'Deze verzekering vult voor ��n jaar het bedrag aan dat de ziekteverzekering uitbetaalt als je door een ongeval niet meer kunt gaan werken.');