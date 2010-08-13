--
-- We kunnen truncate table niet gebruiken want sommige tabellen hebben een foreign key
--

DELETE FROM pers.AdresType
GO
DELETE FROM lid.OfficieleAfdeling
GO
DELETE FROM pers.CommunicatieType
GO
DELETE FROM adr.StraatNaam
GO
DELETE FROM adr.Woonplaats
GO
DELETE FROM adr.PostNr
GO
