use gap_local_auth;
GO

CREATE PROCEDURE [auth].[spWillekeurigeGroepToekennenAd]
(
	@adNr int,
	@naam varchar(max),
	@voornaam varchar(max),
	@geboorteDatum DateTime,
	@geslacht int -- 1 man, 2 vrouw
)
AS
-- Doel: Kent rechten toe aan persoon met gegeven AD-nummer.
-- Als er geen gebruiker is met gegeven AD-nummer, wordt die gemaakt op basis
-- van meegeleverde naam en voornaam.
BEGIN
	DECLARE @stamnr as varchar(10);

	insert into pers.persoon(AdNummer, Naam, VoorNaam, GeboorteDatum, Geslacht)
	select @adNr, @naam, @voornaam, @geboortedatum, @geslacht
	where not exists (select adNummer from pers.Persoon where adNummer=@adnr);

	set @stamnr = (select top 1 g.Code from grp.Groep g where g.StopDatum is null order by newid());
	exec auth.spGebruikersRechtToekennenAd @stamnr, @adNr
END