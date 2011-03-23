use kipadmin

update lid.lid set freelance='n' where freelance is null
go

alter table lid.lid alter column freelance ud_Boolean not null
go

ALTER TABLE lid.Lid ADD CONSTRAINT
	DF_Lid_FREELANCE DEFAULT 'n' FOR FREELANCE
GO