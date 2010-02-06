select 'INSERT INTO adr.PostNr(PostNr) VALUES(' + CAST(PostNr AS VARCHAR(10)) + ');' from adr.Postnr


-- OPGELET! Om de gegenereerde query's te laten lopen, iedere keer
-- 'GO' tussen zetten, want anders zijn het er te veel ineens.
-- (\r gebruiken in je search/replace, en dan komt het goed :-))

select distinct 'INSERT INTO adr.Straat(PostNr, Naam) VALUES(' 
    + CAST(PostNr AS VARCHAR(4)) + ', '''
	+ REPLACE(Naam , '''' , '''''' ) + ''');' from adr.Straat

select distinct 'INSERT INTO adr.Subgemeente(PostNr, Naam) VALUES(' 
    + CAST(PostNr AS VARCHAR(4)) + ', '''
	+ REPLACE(Naam , '''' , '''''' ) + ''');' from adr.Subgemeente
