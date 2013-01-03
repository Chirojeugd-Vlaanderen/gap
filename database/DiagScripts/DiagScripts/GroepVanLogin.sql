use gap;

declare @login as varchar(40); set @login='CHIROPUBLIC\pellaed'

select gr.code, gr.naam from auth.gav g 
join auth.gavschap gs on g.gavid=gs.gavid
join pers.persoon p on gs.persoonid=p.persoonid
join pers.gelieerdepersoon gp on p.persoonid=gp.persoonid
join grp.groep gr on gp.groepid=gr.groepid
where g.login=@login