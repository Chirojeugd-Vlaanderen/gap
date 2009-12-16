--
-- We kunnen truncate table niet gebruiken want sommige tabellen hebben een foreign key
--
 
DELETE FROM pers.AdresType
GO
DELETE FROM lid.OfficieleAfdeling
GO
DELETE FROM pers.CommunicatieType
GO
DELETE FROM adr.Straat
GO
DELETE FROM adr.Subgemeente
GO
DELETE FROM adr.PostNrNaarGemeente
GO
DELETE FROM adr.PostNr
GO
DELETE FROM adr.Gemeente
GO 