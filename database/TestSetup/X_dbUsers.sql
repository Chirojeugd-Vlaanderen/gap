﻿-- rechten op de dev-db via SQL authenticatie
-- (omdat de developers typisch niet aan het werken zijn met hun kipdorplogin)


USE [gap_dev]
GO
/****** Object:  User [CgApp]    Script Date: 02/20/2012 12:05:51 ******/
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'CgApp')
DROP USER [CgApp]
GO
CREATE USER [CgApp] FOR LOGIN [CgApp]
GO
EXEC sp_addrolemember N'GapRole', N'CgApp'
GO

CREATE USER [CgDevelop] FOR LOGIN [CgDevelop]
GO
EXEC sp_addrolemember N'db_owner', N'CgDevelop'
GO