use kipadmin;

-- (Deze lijst bevat wss ook de bivakaangiftes zonder adres)

select cg.stamnr, ba.BivakNaam, ba.datumvan, ba.datumtot, ba.GapUitstapID 
--select distinct 'exec auth.spTijdelijkeGebruiker ''' + cg.stamnr + ''', ''chiropublic\johan4''', cg.stamnr
from biv.bivakaangifte ba
join huidigwerkjaar wj on ba.WerkJaar = wj.werkjaar
join grp.chirogroep cg on ba.groepid=cg.groepid
left outer join biv.bivakoverzicht bo on ba.werkjaar = bo.werkjaar and ba.groepid=bo.groepid
where bo.id is null
order by cg.stamnr