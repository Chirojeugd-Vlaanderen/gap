use kipadmin;

-- past de adressen in Kipadmin aan, zodat de initiele import ze herkent in GAP.

update kipAdres set gemeente='Sint-Amandsberg (Gent)' where gemeente like '%amandsberg%';
update kipAdres set gemeente='Deurne (Antwerpen)' where gemeente like '%deurne%';
update kipAdres set gemeente='Kessel-Lo (Leuven)' where gemeente like '%kessel-lo%';
update kipAdres set gemeente='Kapellen (Antw.)' where gemeente like '%kapellen%';
update kipAdres set gemeente='Sankt Vith' where gemeente like '%vith%';
update kipAdres set gemeente='Nieuwkerken-Waas' where gemeente like '%nieuwkerken%';
update kipAdres set gemeente='Wijgmaal (Brabant)' where gemeente like '%wijgmaal%';
update kipAdres set gemeente='Borsbeek (Antw.)' where gemeente like '%borsbeek%';
