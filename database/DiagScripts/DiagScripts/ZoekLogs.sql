use kipadmin;

select top 200 * from log.berichten 
--where bericht like '%152242%' 
order by id desc

--select * from log.berichten where id >= 942043 and id <= 942077