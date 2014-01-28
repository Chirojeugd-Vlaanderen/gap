--   Wijzigingen in de GAP-database voor upgrade van 1.9 naar 2.0
--   Copyright 2013 Chirojeugd-Vlaanderen vzw
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


use gap_dev;

alter table pers.persoon add CiviID int null;
alter table adr.land add IsoCode varchar(10);
go

-- landcodes.
-- Schotland is al geen land.
delete from adr.Land where naam='Schotland' 

update adr.land set IsoCode='AL' where Naam='Albanië';
update adr.land set IsoCode='BE' where Naam='België';
update adr.land set IsoCode='BG' where Naam='Bulgarijë';
update adr.land set IsoCode='BI' where Naam='Burundi';
update adr.land set IsoCode='CL' where Naam='Chili';
update adr.land set IsoCode='CN' where Naam='China';
update adr.land set IsoCode='CG' where Naam='Congo';
update adr.land set IsoCode='DK' where Naam='Denemarken';
update adr.land set IsoCode='CD' where Naam='Dominicaanse Republiek';
update adr.land set IsoCode='DE' where Naam='Duitsland';
update adr.land set IsoCode='EC' where Naam='Ecuador';
update adr.land set IsoCode='SV' where Naam='El Salvador';
update adr.land set IsoCode='PH' where Naam='Filipijnen';
update adr.land set IsoCode='FR' where Naam='Frankrijk';
update adr.land set IsoCode='GH' where Naam='Ghana';
update adr.land set IsoCode='GT' where Naam='Guatemala';
update adr.land set IsoCode='HT' where Naam='Haïti';
update adr.land set IsoCode='HU' where Naam='Hongarije';
update adr.land set IsoCode='IE' where Naam='Ierland';
update adr.land set IsoCode='IS' where Naam='Ijsland';
update adr.land set IsoCode='IN' where Naam='Indië';
update adr.land set IsoCode='IT' where Naam='Italië';
update adr.land set IsoCode='HR' where Naam='Kroatië';
update adr.land set IsoCode='LT' where Naam='Litouwen';
update adr.land set IsoCode='LU' where Naam='Luxemburg';
update adr.land set IsoCode='MT' where Naam='Malta';
update adr.land set IsoCode='MX' where Naam='Mexico';
update adr.land set IsoCode='NA' where Naam='Namibië';
update adr.land set IsoCode='NL' where Naam='Nederland';
update adr.land set IsoCode='UG' where Naam='Oeganda';
update adr.land set IsoCode='UA' where Naam='Oekraïne';
update adr.land set IsoCode='AT' where Naam='Oostenrijk';
update adr.land set IsoCode='PY' where Naam='Paraguay';
update adr.land set IsoCode='PL' where Naam='Polen';
update adr.land set IsoCode='PT' where Naam='Portugal';
update adr.land set IsoCode='RO' where Naam='Roemenië';
update adr.land set IsoCode='RW' where Naam='Rwanda';
update adr.land set IsoCode='SL' where Naam='Sierra Leone';
update adr.land set IsoCode='SI' where Naam='Slovenië';
update adr.land set IsoCode='SK' where Naam='Slowakije';
update adr.land set IsoCode='ES' where Naam='Spanje';
update adr.land set IsoCode='LK' where Naam='Sri Lanka';
update adr.land set IsoCode='TW' where Naam='Taiwan';
update adr.land set IsoCode='CZ' where Naam='Tsjechië';
update adr.land set IsoCode='TN' where Naam='Tunesië';
update adr.land set IsoCode='VE' where Naam='Venezuela';
update adr.land set IsoCode='GB' where Naam='Verenigd Koninkrijk';
update adr.land set IsoCode='US' where Naam='Verenigde Staten';
update adr.land set IsoCode='ZA' where Naam='Zuid-Afrika';
update adr.land set IsoCode='SE' where Naam='Zweden';
update adr.land set IsoCode='CH' where Naam='Zwitserland';

alter table adr.land alter column IsoCode varchar(10) not null
alter table adr.land add constraint AK_Land_IsoCode unique(IsoCode)
