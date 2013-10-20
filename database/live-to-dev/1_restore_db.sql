-- STAP 1: haal gewoon backup van live binnen in test

USE master;
GO

-- aanpassen:
DECLARE @backupFile AS VARCHAR(200); SET @backupFile = 'C:\sqlbak\gap\gap_backup_201310132056.fbu';
DECLARE @dbNaam AS VARCHAR(200); SET @dbNaam = 'gap_tst';

-- wordt berekend:
DECLARE @dataFile AS VARCHAR(200); SET @dataFile= 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL2K8\MSSQL\DATA\'+@dbNaam+'.mdf';
DECLARE @logFile AS VARCHAR(200); SET @logFile='C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL2K8\MSSQL\DATA\'+@dbNaam+'.ldf';

RESTORE DATABASE @dbNaam FROM DISK = @backupFile WITH  FILE = 1,  MOVE N'gap1rc2' TO @dataFile,  MOVE N'gap1rc2_log' TO @logFile,  NOUNLOAD,  REPLACE,  STATS = 10

GO

USE [master]
GO
ALTER DATABASE [gap_tst] SET RECOVERY SIMPLE WITH NO_WAIT
GO
