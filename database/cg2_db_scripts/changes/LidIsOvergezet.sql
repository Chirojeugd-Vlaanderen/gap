ALTER TABLE lid.Lid ADD
	IsOvergezet bit NOT NULL CONSTRAINT DF_Lid_IsOvergezet DEFAULT 0

GO

update lid.lid set eindeinstapperiode=null
GO