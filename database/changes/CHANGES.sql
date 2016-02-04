-- Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
-- top-level directory of this distribution, and at
-- https://gapwiki.chiro.be/copyright
-- 
-- Licensed under the Apache License, Version 2.0 (the "License");
-- you may not use this file except in compliance with the License.
-- You may obtain a copy of the License at
-- 
--     http://www.apache.org/licenses/LICENSE-2.0
-- 
-- Unless required by applicable law or agreed to in writing, software
-- distributed under the License is distributed on an "AS IS" BASIS,
-- WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
-- See the License for the specific language governing permissions and
-- limitations under the License.

-- Laat stored procedure voor toekennen rechten willekeurige groep het 
-- groepID opleveren. Dat lijkt me handig.
-- Zie #4825.

ALTER PROCEDURE [auth].[spWillekeurigeGroepToekennen] (@login varchar(40)) AS
-- Doel: Kent gebruikersrecht voor willekeurige actieve groep
-- toe aan user met gegeven login. (Enkel voor debugging purposes)
BEGIN 
	DECLARE @stamnr as varchar(10);

	set @stamnr = (select top 1 g.Code from grp.Groep g where g.StopDatum is null order by newid());
	exec auth.spGebruikersRechtToekennen @stamnr, @login;
	SELECT GroepID FROM grp.Groep WHERE Code=@stamnr;
END

GO