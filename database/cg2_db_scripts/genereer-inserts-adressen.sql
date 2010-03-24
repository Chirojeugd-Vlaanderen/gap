
select 'INSERT INTO adr.PostNr(PostNr) VALUES(' + CAST(PostNr AS VARCHAR(10)) + ');' from adr.Postnr


-- OPGELET! Om de gegenereerde query's te laten lopen, iedere keer
-- 'GO' tussen zetten, want anders zijn het er te veel ineens.
-- Dat lukt met dit commando in vi:
--  :%s/$/\rGO/


select distinct 'INSERT INTO adr.WoonPlaats(PostNummer, Naam, TaalID, CrabPostKantonID) VALUES(' 
    + CAST(po.PostNummer AS VARCHAR(4)) + ', ' 
    + '''' + REPLACE(po.Naam , '''' , '''''' ) + ''', ' 
	+ CAST(t.TaalID AS VARCHAR(4)) + ', '
	+ CAST(po.PostKantonID AS VARCHAR(10))
	+ ');'
from crab_koen.dbo.Created_PeterPostOmschrijving po join core.taal t on po.Taal = t.Code



select distinct 'INSERT INTO adr.StraatNaam(PostNummer, Naam, TaalID, CrabSubStraatID) VALUES(' 
    + CAST(min(pk.PostNummer) AS VARCHAR(4)) + ', ' 
    + '''' + REPLACE(min(s.Naam) , '''' , '''''' ) + ''', ' 
	+ CAST(min(t.TaalID) AS VARCHAR(4)) + ', '
	+ CAST(min(s.CrabSubStraatID) AS VARCHAR(10))
	+ ');'
from crab_koen.dbo.PeterStraat s join core.Taal t on s.Taal = t.Code
join 
(
select distinct PostKantonID, PostNummer
from crab_koen.dbo.Created_PeterPostOmschrijving
) pk on s.PostKantonID = pk.PostKantonID
group by pk.PostNummer, s.Naam