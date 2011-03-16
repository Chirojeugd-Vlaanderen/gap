use gap

CREATE TABLE adr.BelgischAdres(
	BelgischAdresID INT NOT NULL PRIMARY KEY,
	StraatNaamID INT NOT NULL,
	WoonPlaatsID INT NOT NULL);

ALTER TABLE adr.BelgischAdres ADD CONSTRAINT FK_BelgischAdres_Adres
FOREIGN KEY(BelgischAdresID) REFERENCES adr.Adres(AdresID);
	
ALTER TABLE adr.BelgischAdres ADD CONSTRAINT FK_BelgischAdres_StraatNaam
FOREIGN KEY(StraatNaamID) REFERENCES adr.StraatNaam(StraatNaamID);

ALTER TABLE adr.BelgischAdres ADD CONSTRAINT FK_BelgischAdres_WoonPlaats
FOREIGN KEY(WoonPlaatsID) REFERENCES adr.WoonPlaats(WoonPlaatsID);

CREATE INDEX IDX_BelgischAdres_StraatNaamID_WoonPlaatsID ON adr.BelgischAdres(StraatNaamID, WoonPlaatsID)

INSERT INTO adr.BelgischAdres(BelgischAdresID, StraatNaamID, WoonPlaatsID)
SELECT AdresID, StraatNaamID, WoonPlaatsID FROM adr.Adres

ALTER TABLE adr.Adres DROP CONSTRAINT AK_Adres
DROP INDEX IDX_Adres_StraatNaamID ON adr.Adres
ALTER TABLE adr.Adres DROP CONSTRAINT FK_Adres_StraatNaam
ALTER TABLE adr.Adres DROP CONSTRAINT FK_Adres_WoonPlaats

ALTER TABLE adr.Adres DROP COLUMN StraatNaamID
ALTER TABLE adr.Adres DROP COLUMN WoonPlaatsID

CREATE TABLE adr.Land(
	LandID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Naam VARCHAR(80) NOT NULL)

CREATE UNIQUE INDEX AK_Land_Naam ON adr.Land(NAAM)

-- toevoegen van een aantal landen.  Gewoon een willekeurige selectie,
-- op basis van wat er al in Kipadmin te vinden was.

INSERT INTO adr.Land(Naam) VALUES('Albanië')
INSERT INTO adr.Land(Naam) VALUES('België')
INSERT INTO adr.Land(Naam) VALUES('Bulgarijë')
INSERT INTO adr.Land(Naam) VALUES('Burundi')
INSERT INTO adr.Land(Naam) VALUES('Chili')
INSERT INTO adr.Land(Naam) VALUES('China')
INSERT INTO adr.Land(Naam) VALUES('Congo')
INSERT INTO adr.Land(Naam) VALUES('Denemarken')
INSERT INTO adr.Land(Naam) VALUES('Dominicaanse Republiek')
INSERT INTO adr.Land(Naam) VALUES('Duitsland')
INSERT INTO adr.Land(Naam) VALUES('Ecuador')
INSERT INTO adr.Land(Naam) VALUES('El Salvador')
INSERT INTO adr.Land(Naam) VALUES('Filipijnen')
INSERT INTO adr.Land(Naam) VALUES('Frankrijk')
INSERT INTO adr.Land(Naam) VALUES('Ghana')
INSERT INTO adr.Land(Naam) VALUES('Guatemala')
INSERT INTO adr.Land(Naam) VALUES('Haïti')
INSERT INTO adr.Land(Naam) VALUES('Hongarije')
INSERT INTO adr.Land(Naam) VALUES('Ierland')
INSERT INTO adr.Land(Naam) VALUES('Ijsland')
INSERT INTO adr.Land(Naam) VALUES('Indië')
INSERT INTO adr.Land(Naam) VALUES('Italië')
INSERT INTO adr.Land(Naam) VALUES('Kroatië')
INSERT INTO adr.Land(Naam) VALUES('Litouwen')
INSERT INTO adr.Land(Naam) VALUES('Luxemburg')
INSERT INTO adr.Land(Naam) VALUES('Malta')
INSERT INTO adr.Land(Naam) VALUES('Namibië')
INSERT INTO adr.Land(Naam) VALUES('Nederland')
INSERT INTO adr.Land(Naam) VALUES('Oeganda')
INSERT INTO adr.Land(Naam) VALUES('Oekraïne')
INSERT INTO adr.Land(Naam) VALUES('Oostenrijk')
INSERT INTO adr.Land(Naam) VALUES('Paraguay')
INSERT INTO adr.Land(Naam) VALUES('Polen')
INSERT INTO adr.Land(Naam) VALUES('Portugal')
INSERT INTO adr.Land(Naam) VALUES('Rwanda')
INSERT INTO adr.Land(Naam) VALUES('Schotland')
INSERT INTO adr.Land(Naam) VALUES('Sierra Leone')
INSERT INTO adr.Land(Naam) VALUES('Slowakije')
INSERT INTO adr.Land(Naam) VALUES('Spanje')
INSERT INTO adr.Land(Naam) VALUES('Sri Lanka')
INSERT INTO adr.Land(Naam) VALUES('Taiwan')
INSERT INTO adr.Land(Naam) VALUES('Tsjechië')
INSERT INTO adr.Land(Naam) VALUES('Venezuela')
INSERT INTO adr.Land(Naam) VALUES('Verenigd Koninkrijk')
INSERT INTO adr.Land(Naam) VALUES('Verenigde Staten')
INSERT INTO adr.Land(Naam) VALUES('Zuid-Afrika')
INSERT INTO adr.Land(Naam) VALUES('Zweden')
INSERT INTO adr.Land(Naam) VALUES('Zwitserland')



CREATE TABLE adr.BuitenLandsAdres(
	BuitenlandsAdresID INT NOT NULL PRIMARY KEY,
	PostCode VARCHAR(10),
	Straat VARCHAR(80) NOT NULL,
	WoonPlaats VARCHAR(80) NOT NULL,
	LandID INT NOT NULL)
	
ALTER TABLE adr.BuitenLandsAdres ADD CONSTRAINT FK_BuitenlandsAdres_Adres
FOREIGN KEY(BuitenLandsAdresID) REFERENCES adr.Adres(AdresID);

ALTER TABLE adr.BuitenLandsAdres ADD CONSTRAINT FK_BuitenlandsAdres_Land
FOREIGN KEY(LandID) REFERENCES adr.Land(LandID);

CREATE INDEX IDX_BuitenLandsAdres_Straat_WoonPlaats_LandID 
ON adr.BuitenLandsAdres(Straat, WoonPlaats, LandID)

