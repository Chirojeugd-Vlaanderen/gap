-- procedures, vooral voor intranet/extranet
CREATE PROCEDURE [auth].[spGebruikersRechtToekennenAd]
(
 @stamNr VARCHAR(10)
 , @adNr int
)
AS
-- Doel: Geeft login gebruikersrecht op gegeven groep
-- Als de persoon ooit gebruikersrecht had, wordt dit gewoon verlengd.
BEGIN
 -- bepaal de vervaldatum.
 declare @datum datetime
   , @jaar varchar(4)
 -- Vanaf juli gaan we ervan uit dat de login voor degene is
 -- die het volgende werkjaar de groepsadministratie zal regelen.
 IF MONTH(getdate()) >= 7
  set @jaar = cast(year(getdate()) + 1 as varchar)
 ELSE
  set @jaar = cast(year(getdate()) as varchar)
 set @datum =  @jaar + '-11-01';
 -- Verwijder eventueel oud gebruikersrecht.
 DELETE gr FROM auth.GebruikersRechtV2 gr
  JOIN pers.Persoon p ON gr.PersoonID = p.PersoonID
  JOIN grp.Groep g ON gr.GroepID = g.GroepID
 WHERE p.AdNummer=@adNr AND g.Code=@stamNr;
 INSERT INTO auth.GebruikersRechtV2(PersoonID, GroepID, VervalDatum, GroepsPermissies, IedereenPermissies, PersoonsPermissies)
 SELECT
   p.PersoonID, g.GroepID, @datum, 3, 3, 1 -- Je moet jezelf kunnen zien om toegang te hebben, zie #5618
 FROM
   pers.Persoon p
   , grp.Groep g
 WHERE
   p.AdNummer=@adNr
   AND g.Code=@stamnr
 IF @@ROWCOUNT <= 0
 BEGIN
  RAISERROR('Dat StamNr is niet gevonden in de tabel Groep', 16, 1)
 END
END
GO

-- procedures voor dev (moet weg uit live db)
CREATE PROCEDURE [auth].[spWillekeurigeGroepToekennenAd]
(
 @adNr int
)
AS
-- Doel: Kent rechten toe aan persoon met gegeven AD-nummer.
-- Als er geen gebruiker is met gegeven AD-nummer, wordt die gemaakt op basis
-- van meegeleverde naam en voornaam.
BEGIN
 DECLARE @stamnr as varchar(10);
 set @stamnr = (select top 1 g.Code from grp.Groep g where g.StopDatum is null order by newid());
 exec auth.spGebruikersRechtToekennenAd @stamnr, @adNr
END
GO
