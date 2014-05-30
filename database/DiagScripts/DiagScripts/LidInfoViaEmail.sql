use kipadmin;

select werkjaar, naam, voornaam, p.adnr, cg.groepid, stamnr from kipContactInfo ci 
join kipPersoon p on ci.AdNr = p.AdNr
join lid.lid l on l.ADNR = p.AdNr
join grp.ChiroGroep cg on l.GroepID = cg.GroepID
where ci.info='elien_beck@hotmail.com'