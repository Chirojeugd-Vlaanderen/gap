use gap
go

create schema biv;
go

create table biv.Plaats
(
	PlaatsID int not null identity(1,1),
	Naam varchar(80) not null,
	AdresID int not null,
	GelieerdePersoonID int not null,	-- contactpersoon
	Versie timestamp
);

alter table biv.Plaats add constraint PK_Plaats primary key(PlaatsID);
alter table biv.Plaats add constraint FK_Plaats_Adres foreign key(AdresID) references adr.Adres(AdresID);
alter table biv.Plaats add constraint FK_Plaats_GelieerdePersoon_Contact foreign key (GelieerdePersoonID) references pers.GelieerdePersoon(GelieerdePersoonID);

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
	Versie timestamp
)

alter table biv.Uitstap add constraint PK_Uitstap primary key(UitstapID);
alter table biv.Uitstap add constraint FK_Uitstap_Plaats foreign key(PlaatsID) references biv.Plaats(PlaatsID);
alter table biv.Uitstap add constraint FK_Uitstap_GroepsWerkJaar foreign key(GroepsWerkJaarID) references grp.GroepsWerkJaar(GroepsWerkJaarID);

grant select,insert,update,delete on biv.Uitstap to GapRole;


