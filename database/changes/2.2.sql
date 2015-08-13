-- OPGELET: De user van het CiviSync process moet schrijfrechten hebben op de logging.Bericht tabel.

use gap;
go

alter table pers.Persoon add LaatsteMembership int null;
go

update p
set p.LaatsteMembership = tmp.LaatsteMembership
from pers.persoon p join
(
select  l.adnr, max(l.werkjaar) as LaatsteMembership
from kipadmin.lid.lid l
join kipadmin.lid.aansluiting a on l.groepid=a.groepid and l.werkjaar=a.werkjaar and l.aansl_nr=a.VolgNummer
join kipadmin.dbo.rekening r on a.rekeningid = r.nr
where l.aansl_nr > 0
and r.DOORGEBOE='j'
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
