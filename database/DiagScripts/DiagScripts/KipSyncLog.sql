-- Toon de recentste berichten van KipSync

use kipadmin;

select top 100 * from log.berichten where 
gebruiker like 'kipsync%' 
and ernstig=1
order by id desc

select top 100 * from log.berichten where bericht like '%163782%' order by id desc