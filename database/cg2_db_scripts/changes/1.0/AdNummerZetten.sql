
CREATE PROCEDURE pers.spAdNummerZetten @PersoonID int, @AdNummer INT AS
-- DOEL: Het AD-Nummer van een gelieerde persoon zetten
UPDATE pers.Persoon SET AdNummer=@AdNummer, AdInAanvraag=0 WHERE PersoonID=@PersoonID
GO
