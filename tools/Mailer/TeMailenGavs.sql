use gap
go

declare @dagenopvoorhand as integer; set @dagenopvoorhand = 6

select distinct gr.GroepID,gav.Login from lid.Lid l
join grp.GroepsWerkJaar gwj on l.GroepsWerkJaarID = gwj.GroepsWerkJaarID
join auth.GebruikersRecht gr on gwj.GroepID = gr.GroepID
join auth.Gav on gr.GavID = gav.GavID
where datediff(day, getdate(), EindeInstapPeriode) = 6 
go
