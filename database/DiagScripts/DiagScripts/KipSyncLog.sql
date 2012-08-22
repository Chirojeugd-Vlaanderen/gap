-- Toon de recentste berichten van KipSync

use kipadmin;

select top 100 * from log.berichten where gebruiker like 'kipsync%' order by id desc