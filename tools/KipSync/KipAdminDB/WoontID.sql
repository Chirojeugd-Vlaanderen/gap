/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.kipWoont ADD
	WoontID int NOT NULL IDENTITY (1, 1)
GO
ALTER TABLE dbo.kipWoont
	DROP CONSTRAINT PK__kipWoont__5AE46118
GO
ALTER TABLE dbo.kipWoont ADD CONSTRAINT
	PK__kipWoont__5AE46118 PRIMARY KEY CLUSTERED 
	(
	WoontID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX AK_Woont_AdNr_AdresID ON dbo.kipWoont
	(
	AdNr,
	AdresId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT