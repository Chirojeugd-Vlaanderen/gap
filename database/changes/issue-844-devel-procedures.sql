--   Wijzigingen in de GAP-database voor nieuw gebruikersbeheer.
--   Copyright 2014,2015 Chirojeugd-Vlaanderen vzw
--
--   Licensed under the Apache License, Version 2.0 (the "License");
--   you may not use this file except in compliance with the License.
--   You may obtain a copy of the License at
--
--       http://www.apache.org/licenses/LICENSE-2.0
--
--   Unless required by applicable law or agreed to in writing, software
--   distributed under the License is distributed on an "AS IS" BASIS,
--   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
--   See the License for the specific language governing permissions and
--   limitations under the License.

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