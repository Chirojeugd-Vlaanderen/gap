-- Verwijdert dubbels uit GAP op basis van AD-nummer

use gap;


---- concentreren, Johan, want anders gaat het mis. Zoals meestal.

---- In de niet doorgekomen leden, zijn de kolommen: GAPAD blabla KIPAD
---- KIPAD is AdFoutePersoon. GAPAD is AdJuistePersoon
---- JuisteAd is AdFoutePersoon


-- TYPISCH LAAGSTE AD-NUMMER:
declare @AdFoutePersoon as int; set @AdFoutePersoon= 286115;		-- AD-nummer van dubbele persoon
-- TYPISCH HOOGSTE AD-NUMMER:
declare @AdJuistePersoon as int; set @AdJuistePersoon= 240513;	-- AD-nummer van de persoon met de juiste persoonsgegevens (adressen, contactinfo)
--declare @JuisteAd as int; set @JuisteAd=@AdJuistePersoon;		-- Het zou kunnen dat het ad-nr van de juiste persoon ook het juiste ad-nr is...
declare @JuisteAd as int; set @JuisteAd=@AdFoutePersoon;		-- ... maar typisch heeft de persoon met de juiste gegevens het verkeerde AD-nummer :-)

-- PersoonIDs opzoeken

declare @IdFoutePersoon as int;
declare @IdJuistePersoon as int;

set @IdFoutePersoon = (select persoonid from pers.persoon where adnummer=@AdFoutePersoon);
set @IdJuistePersoon = (select persoonid from pers.persoon where adnummer=@AdJuistePersoon);

print 'ID foute persoon: ' + cast(@IdFoutePersoon as varchar(15));
print 'ID juiste persoon: ' + cast(@IdJuistePersoon as varchar(15));

if (@IdJuistePersoon is not null)

-- Mergen

exec data.spDubbelePersoonVerwijderen @IdFoutePersoon, @IdJuistePersoon;

-- AD-nr fixen

update pers.persoon set adnummer=@JuisteAd where persoonid=@IdJuistePersoon;

