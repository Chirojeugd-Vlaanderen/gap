-- STAP 1: haal gewoon backup van live binnen in test

USE master;
GO

-- aanpassen:
DECLARE @backupFile AS VARCHAR(200); SET @backupFile = 'C:\tmp\gap_backup_2015_06_01_022737_6836549.fbu';
DECLARE @dbNaam AS VARCHAR(200); SET @dbNaam = 'gap_local';

-- wordt berekend:
DECLARE @dataFile AS VARCHAR(200); SET @dataFile= 'c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\'+@dbNaam+'.mdf';
DECLARE @logFile AS VARCHAR(200); SET @logFile='c:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\'+@dbNaam+'.ldf';

RESTORE DATABASE @dbNaam FROM DISK = @backupFile WITH  FILE = 1,  MOVE N'gap1rc2' TO @dataFile,  MOVE N'gap1rc2_log' TO @logFile,  NOUNLOAD,  REPLACE,  STATS = 10

GO

USE [master]
GO
-- hieronder ook aanpassen:
ALTER DATABASE [gap_local] SET RECOVERY SIMPLE WITH NO_WAIT
GO
