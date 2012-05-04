USE [gap_tst]
GO
/****** Object:  User [KIPDORP\gapservice]    Script Date: 05/03/2012 13:43:34 ******/
DROP USER [KIPDORP\gapservice]
GO

USE [master]
GO
CREATE LOGIN [KIPDORP\gapservice_tst] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
GO
USE [gap_tst]
GO
CREATE USER [kipdorp\gapservice_tst] FOR LOGIN [kipdorp\gapservice_tst]
GO
USE [gap_tst]
GO
EXEC sp_addrolemember N'GapRole', N'kipdorp\gapservice_tst'
GO
USE [master]
GO
CREATE LOGIN [KIPDORP\kipsync_tst] FROM WINDOWS WITH DEFAULT_DATABASE=[kipadmin_tst]
GO
USE [kipadmin_tst]
GO
CREATE USER [KIPDORP\kipsync_tst] FOR LOGIN [KIPDORP\kipsync_tst]
GO
USE [kipadmin_tst]
GO
EXEC sp_addrolemember N'KipSyncRole', N'KIPDORP\kipsync_tst'
GO
