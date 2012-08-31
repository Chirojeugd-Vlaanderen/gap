-- Verwijdert dubbels uit GAP op basis van AD-nummer

use gap;


declare @AdFoutePersoon as int; set @AdFoutePersoon= 167314;		-- AD-nummer van dubbele persoon
declare @AdJuistePersoon as int; set @AdJuistePersoon= 239585;	-- AD-nummer van de persoon met de juiste persoonsgegevens (adressen, contactinfo)
declare @JuisteAd as int; set @JuisteAd=@AdFoutePersoon;		-- Typisch heeft de persoon met de juiste gegevens het verkeerde AD-nummer :-)

-- PersoonIDs opzoeken

declare @IdFoutePersoon as int;
declare @IdJuistePersoon as int;

set @IdFoutePersoon = (select persoonid from pers.persoon where adnummer=@AdFoutePersoon);
set @IdJuistePersoon = (select persoonid from pers.persoon where adnummer=@AdJuistePersoon);

print 'ID foute persoon: ' + cast(@IdFoutePersoon as varchar(15));
print 'ID juiste persoon: ' + cast(@IdJuistePersoon as varchar(15));


-- Mergen

exec data.spDubbelePersoonVerwijderen @IdFoutePersoon, @IdJuistePersoon;

-- AD-nr fixen

update pers.persoon set adnummer=@JuisteAd where persoonid=@IdJuistePersoon;

