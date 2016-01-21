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

-- Migreer postnummer en postcode naar postcode voor buitenlandse adressen.
-- We proberen er ook een aantal veel voorkomende fouten uit te halen,
-- want er zijn meer buitenlandse adressen fout ingegeven dan juist.
-- Zie #1816

ALTER TABLE adr.BuitenlandsAdres ALTER COLUMN PostCode varchar(16);
GO

UPDATE ba
	SET ba.PostCode = LTRIM(RTRIM(tmp2.PostCode + ' ' + tmp2.PostNummer))
FROM (
	SELECT 
		BuitenlandsAdresID,
		PostCode,
		-- Als postnummer voorkomt in postcode, dan is dat waarschijnlijk fout, en mag postnummer
		-- genegeerd worden.
		CASE WHEN CHARINDEX(PostNummer, PostCode_Zonder_Spaties) > 0 THEN '' ELSE PostNummer END AS Postnummer
	FROM (
		-- Zet alles al eens om naar doordeweekse strings
		SELECT 
			BuitenlandsAdresID, 
			ISNULL (PostCode, '') AS PostCode, 
			-- postnummer 0 wil waarschijnlijk zeggen dat alle informatie in postcode zit.
			CASE PostNummer WHEN 0 THEN '' ELSE CONVERT(VARCHAR(9), PostNummer) END AS PostNummer, 
			REPLACE(PostCode, ' ', '') AS PostCode_Zonder_Spaties
		FROM adr.BuitenLandsAdres
	) tmp
) tmp2
JOIN adr.BuitenlandsAdres ba on tmp2.BuitenlandsAdresID = ba.BuitenlandsAdresID
GO

ALTER TABLE adr.BuitenlandsAdres DROP COLUMN PostNummer;
GO
