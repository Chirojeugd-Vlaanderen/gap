use gap
go

create schema biv;
go

create table biv.Plaats
(
	PlaatsID int not null identity(1,1),
	Naam varchar(80) not null,
	AdresID int not null,
	GelieerdePersoonID int null,		-- contactpersoon
	GroepID int not null,				-- ingevende groep
	Versie timestamp
);

alter table biv.Plaats add constraint PK_Plaats primary key(PlaatsID);
alter table biv.Plaats add constraint FK_Plaats_Adres foreign key(AdresID) references adr.Adres(AdresID);
alter table biv.Plaats add constraint FK_Plaats_GelieerdePersoon_Contact foreign key (GelieerdePersoonID) references pers.GelieerdePersoon(GelieerdePersoonID);
alter table biv.Plaats add constraint FK_Plaats_Groep foreign key(GroepID) references grp.Groep(GroepID);

create unique index AK_Plaats_AdresID_GroepID_Naam on biv.Plaats(AdresID, GroepID, Naam)

grant select,insert,update,delete on biv.Plaats to GapRole;


create table biv.Uitstap
(
	UitstapID int not null identity(1,1),
	Naam varchar(120) not null,
	IsBivak bit not null,
	DatumVan datetime not null,
	DatumTot datetime not null,
	Opmerkingen text,
	PlaatsID int,
	GroepsWerkJaarID int,
	ContactDeelnemerID int,
	Versie timestamp
)

alter table biv.Uitstap add constraint PK_Uitstap primary key(UitstapID);
alter table biv.Uitstap add constraint FK_Uitstap_Plaats foreign key(PlaatsID) references biv.Plaats(PlaatsID);
alter table biv.Uitstap add constraint FK_Uitstap_GroepsWerkJaar foreign key(GroepsWerkJaarID) references grp.GroepsWerkJaar(GroepsWerkJaarID);

grant select,insert,update,delete on biv.Uitstap to GapRole;

create table biv.Deelnemer
(
  DeelnemerID int not null identity (1,1),
  UitstapID int not null,
  GelieerdePersoonID int not null,
  IsLogistieker bit not null,
  HeeftBetaald bit not null,
  MedischeFicheOk bit not null,
  Opmerkingen text,
  Versie timestamp
);

alter table biv.Deelnemer add constraint PK_Deelnemer primary key(DeelnemerID);
alter table biv.Uitstap add constraint FK_Uitstap_Deelnemer_Contact foreign key(ContactDeelnemerID) references biv.Deelnemer(DeelnemerID);
alter table biv.Deelnemer add constraint FK_Deelnemer_Uitstap foreign key(UitstapID) references biv.Uitstap(UitstapID);
alter table biv.Deelnemer add constraint FK_Deelnemer_GelieerdePersoon foreign key(GelieerdePersoonID) references pers.GelieerdePersoon(GelieerdePersoonID);

create unique index AK_Bivak_DeelnemerID_UitstapID on biv.Deelnemer(DeelnemerID, UitstapID);

grant select,insert,update,delete on biv.Deelnemer to GapRole;

go

create procedure biv.DeelnemerVerwijderen (@deelnemerID as int) as
-- verwijder een deelnemer van zijn uitstap
begin

begin tran

update biv.Uitstap set ContactDeelnemerID=null where ContactDeelnemerID=@deelnemerID
delete from biv.Deelnemer where DeelnemerID=@deelnemerID

commit tran

end

grant exec on biv.DeelnemerVerwijderen to GapRole
go