-- OPGELET: De user van het CiviSync process moet schrijfrechten hebben op de logging.Bericht tabel.

use gap_stg;
go

-- Wijzigingen voor #3392 - nieuwe bulk-mail dingen.

alter table pers.Persoon add NieuwsBrief bit not null default 0;
go

update p
set p.Nieuwsbrief = q.nieuwsbrief
from
pers.persoon p join
(
select persoonId, max(cast(isvooroptin as integer)) as nieuwsbrief from pers.gelieerdepersoon gp
left outer join pers.communicatievorm cv on cv.gelieerdepersoonid=gp.gelieerdepersoonid
where communicatietypeid=3
group by gp.persoonid
) q on p.persoonid = q.persoonid

go

alter table pers.CommunicatieVorm drop column IsVoorOptIn
go

alter table pers.CommunicatieType drop DF__Communica__IsOpt__51300E55
go

alter table pers.CommunicatieType drop column IsOptIn
go

-- Fix primary key op CommunicatieVorm
-- Toen ik een clustered index maakte op deze tabel, verwijderde
-- ik per ongeluk de primary key. Oeps.
DROP INDEX [PK_CommunicatieVorm] ON [pers].[CommunicatieVorm]
GO

ALTER TABLE pers.CommunicatieVorm ADD CONSTRAINT
	PK_CommunicatieVorm PRIMARY KEY NONCLUSTERED 
	(
	CommunicatieVormID
	)
GO



-- Wijzigingen voor #3140. Regel instapperiode via GAP.

go

alter table pers.Persoon add LaatsteMembership int null;
go

update p
set p.LaatsteMembership = tmp.LaatsteMembership
from pers.persoon p join
(
select  l.adnr, max(l.werkjaar) as LaatsteMembership
from kip_stg.lid.lid l
left outer join kip_stg.lid.aansluiting a on l.groepid=a.groepid and l.werkjaar=a.werkjaar and l.aansl_nr=a.VolgNummer
left outer join kip_stg.dbo.rekening r on a.rekeningid = r.nr
where  l.soort = 'KA' or (l.aansl_nr > 0 and r.DOORGEBOE='j')
group by l.adnr
) tmp on p.AdNummer=tmp.ADNR

-- tabel voor logberichten
go
create schema logging;
go

create table logging.Bericht(
	BerichtID integer identity(1,1) not null,
	Tijd datetime not null default getdate(),
	Niveau integer not null default 1,
	Boodschap varchar(max),
	StamNummer char(10) null,		-- stamnummer van een groep, indien van toepassing
	AdNummer integer null,				-- AD-nummer van een persoon, indien van toepassing
	PersoonID integer null,		-- persoonID (GAP) indien van toepassing
	GebruikerID integer null	-- gebruikerID, dat zullen we misschien in de toekomst gebruiken
);

alter table logging.Bericht add constraint PK_Bericht primary key(BerichtID);
alter table logging.Bericht add constraint FK_Bericht_Groep foreign key(StamNummer) references grp.Groep(code);
alter table logging.Bericht add constraint FK_Bericht_Persoon_Id foreign key (PersoonID) references  pers.Persoon(PersoonID);
alter table logging.Bericht add constraint FK_Bericht_Gebruiker foreign key (GebruikerID) references pers.Persoon(PersoonID);

create index IX_Bericht_Tijd on logging.Bericht(Tijd);
create index IX_Bericht_Niveau on logging.Bericht(Niveau);
create index IX_Bericht_StamNummer on logging.Bericht(StamNummer);
create index IX_Bericht_AdNummer on logging.Bericht(AdNummer);
create index IX_Bericht_PersoonID on logging.Bericht(PersoonID);

go

CREATE ROLE GapLogRole
GO

GRANT INSERT ON logging.Bericht TO GapLogRole
GO

CREATE USER [KIPDORP\kipsync_tst] FOR LOGIN [KIPDORP\kipsync_tst]
ALTER ROLE [GapLogRole] ADD MEMBER [KIPDORP\kipsync_tst]
GO




-- Geen idee wat het onderstaande doet.
