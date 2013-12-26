USE [gap_stg]
GO
/****** Object:  User [KIPDORP\gapservice]    Script Date: 05/03/2012 13:43:34 ******/
DROP USER [KIPDORP\gapservice]
GO

USE [gap_stg]
GO
EXEC sp_addrolemember N'GapRole', N'kipdorp\gapservice_tst'
GO

-- LET OP: rechten voor devs vis sql server login: zie script X_dbUsers.sql



