use gap

select * from pers.persoon
where adnummer in
(
select adnummer
from pers.persoon p
where p.adnummer is not null
group by adnummer
having count(*) > 1
)
order by adnummer

select 'exec data.spDubbelePersoonVerwijderen ', max(persoonid),',' , min(persoonid)
from pers.persoon p
where p.adnummer is not null
group by adnummer
having count(*) > 1
