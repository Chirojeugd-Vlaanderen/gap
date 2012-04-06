use gap
go

declare @dagenopvoorhand as integer; set @dagenopvoorhand = 4

declare @eindePeriode as char(10); 
set @eindePeriode=convert(varchar(10), dateadd(day, @dagenopvoorhand, getdate()), 103);

select distinct gr.GroepID, @eindePeriode,gav.Login from lid.Lid l
join grp.GroepsWerkJaar gwj on l.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join auth.GebruikersRecht gr on gwj.GroepID = gr.GroepID
join auth.Gav on gr.GavID = gav.GavID
where datediff(day, getdate(), EindeInstapPeriode) = @dagenopvoorhand 
and (gr.VervalDatum is null or gr.Vervaldatum >= getdate())
and l.NonActief = 0
go
