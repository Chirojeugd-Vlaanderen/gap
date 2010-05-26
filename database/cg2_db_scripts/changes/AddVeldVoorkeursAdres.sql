use gap
go

ALTER TABLE pers.GelieerdePersoon ADD
	VoorkeursAdresID int NULL
GO

ALTER TABLE [pers].[GelieerdePersoon]  WITH CHECK ADD  CONSTRAINT [FK_GelieerdePersoon_PersoonsAdres] FOREIGN KEY([VoorkeursAdresID]) REFERENCES [pers].[PersoonsAdres] ([PersoonsAdresID])
GO
