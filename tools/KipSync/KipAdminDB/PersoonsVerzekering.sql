use kipadmin

create table verz.PersoonsVerzekering(
	PersoonsVerzekeringID int identity(1,1) not null primary key,
	AdNr int not null,
    ExtraVerzekeringID int not null)
go

--ALTER TABLE [verz].[PersoonsVerzekering] DROP CONSTRAINT [FK_PersoonsVerzekering_ExtraVerzekering]
--ALTER TABLE [verz].[PersoonsVerzekering] DROP CONSTRAINT [FK_PersoonsVerzekering_Persoon]

ALTER TABLE verz.PersoonsVerzekering ADD CONSTRAINT
	PK_PersoonsVerzekering PRIMARY KEY
	(
	PersoonsVerzekeringID
	)
GO

alter table verz.PersoonsVerzekering add constraint FK_PersoonsVerzekering_Persoon foreign key(AdNr) references kipPersoon(AdNr)
go

alter table verz.PersoonsVerzekering add constraint FK_PersoonsVerzekering_ExtraVerzekering foreign key(ExtraVerzekeringID) references verz.ExtraVerzekering(ExtraVerzekeringID)
go

