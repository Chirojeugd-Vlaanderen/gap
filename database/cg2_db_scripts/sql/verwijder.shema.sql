
-- Verwijder stiekem een stored procedure
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[core].[ufnSoundEx]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [core].[ufnSoundEx]


-- Nu de tabellen
IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.PersoonsAdres'))
BEGIN
	DROP TABLE pers.PersoonsAdres
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('adr.Adres'))
BEGIN
	DROP TABLE adr.Adres
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('adr.WoonPlaats'))
BEGIN
	DROP TABLE adr.WoonPlaats
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('adr.StraatNaam'))
BEGIN
	DROP TABLE adr.StraatNaam
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('adr.PostNr'))
BEGIN
	DROP TABLE adr.PostNr
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('auth.Gav'))
BEGIN
	DROP TABLE auth.Gav
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('auth.GebruikersRecht'))
BEGIN
	DROP TABLE auth.GebruikersRecht
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('core.Caterie'))
BEGIN
	DROP TABLE core.Caterie
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('core.VrijVeldType'))
BEGIN
	DROP TABLE core.VrijVeldType
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('core.Taal'))
BEGIN
	DROP TABLE core.Taal
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('grp.ChiroGroep'))
BEGIN
	DROP TABLE grp.ChiroGroep
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('core.Categorie'))
BEGIN
	DROP TABLE core.Categorie
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.Kind'))
BEGIN
	DROP TABLE lid.Kind
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.LeidingInAfdelingsJaar'))
BEGIN
	DROP TABLE lid.LeidingInAfdelingsJaar
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.Leiding'))
BEGIN
	DROP TABLE lid.Leiding
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.LidFunctie'))
BEGIN
	DROP TABLE lid.LidFunctie
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.Lid'))
BEGIN
	DROP TABLE lid.Lid
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.PersoonsCategorie'))
BEGIN
	DROP TABLE pers.PersoonsCategorie
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.GelieerdePersoon'))
BEGIN
	DROP TABLE pers.GelieerdePersoon
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('grp.GroepsWerkJaar'))
BEGIN
	DROP TABLE grp.GroepsWerkJaar
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.Afdeling'))
BEGIN
	DROP TABLE lid.Afdeling
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.Functie'))
BEGIN
	DROP TABLE lid.Functie
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('grp.Groep'))
BEGIN
	DROP TABLE grp.Groep
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('grp.GroepsAdres'))
BEGIN
	DROP TABLE grp.GroepsAdres
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.AfdelingsJaar'))
BEGIN
	DROP TABLE lid.AfdelingsJaar
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('lid.OfficieleAfdeling'))
BEGIN
	DROP TABLE lid.OfficieleAfdeling
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.AdresType'))
BEGIN
	DROP TABLE pers.AdresType
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.CommunicatieType'))
BEGIN
	DROP TABLE pers.CommunicatieType
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.CommunicatieVorm'))
BEGIN
	DROP TABLE pers.CommunicatieVorm
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.Persoon'))
BEGIN
	DROP TABLE pers.Persoon
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.PersoonsCaterie'))
BEGIN
	DROP TABLE pers.PersoonsCaterie
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.PersoonVrijVeld'))
BEGIN
	DROP TABLE pers.PersoonVrijVeld
END

IF EXISTS(SELECT * FROM sys.tables WHERE object_id = OBJECT_ID('pers.PersoonVrijVeldType'))
BEGIN
	DROP TABLE pers.PersoonVrijVeldType
END

-- views

IF EXISTS(SELECT * FROM sys.views WHERE object_id = OBJECT_ID('auth.vGebruikers'))
BEGIN
	DROP VIEW auth.vGebruikers
END



-- schema's

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'grp')
BEGIN
	DROP SCHEMA [grp]
END

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'pers')
BEGIN
	DROP SCHEMA [pers]
END

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'lid')
BEGIN
	DROP SCHEMA [lid]
END

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'adr')
BEGIN
	DROP SCHEMA [adr]
END

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'core')
BEGIN
	DROP SCHEMA [core]
END

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'auth')
BEGIN
	DROP SCHEMA [auth]
END

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'data')
BEGIN
	DROP SCHEMA [data]
END
