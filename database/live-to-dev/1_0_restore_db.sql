-- STAP 1: haal gewoon backup van live binnen in test

USE master;
GO

-- aanpassen:
DECLARE @backupFile AS VARCHAR(200); SET @backupFile = 'C:\sqlbak\gap.fbu';
DECLARE @dbNaam AS VARCHAR(200); SET @dbNaam = 'gap_dev';

DECLARE @kipBackupFile AS VARCHAR(200); SET @kipBackupFile = 'C:\sqlbak\kip.fbu';
DECLARE @kipDdbNaam AS VARCHAR(200); SET @kipDdbNaam = 'kip_stg';


-- wordt berekend:
DECLARE @dataFile AS VARCHAR(200); SET @dataFile= 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL2K8\MSSQL\DATA\'+@dbNaam+'.mdf';
DECLARE @logFile AS VARCHAR(200); SET @logFile='C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL2K8\MSSQL\DATA\'+@dbNaam+'.ldf';

RESTORE DATABASE @dbNaam FROM DISK = @backupFile WITH  FILE = 1,  MOVE N'gap1rc2' TO @dataFile,  MOVE N'gap1rc2_log' TO @logFile,  NOUNLOAD,  REPLACE,  STATS = 10


SET @dataFile= 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL2K8\MSSQL\DATA\'+@kipDdbNaam+'.mdf';
SET @logFile='C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL2K8\MSSQL\DATA\'+@kipDdbNaam+'.ldf';

RESTORE DATABASE @kipDdbNaam FROM DISK = @kipBackupFile WITH  FILE = 1,  MOVE N'KipadminData' TO @dataFile,  MOVE N'KipadminLog' TO @logFile,  NOUNLOAD,  REPLACE,  STATS = 10

GO

-- En hieronder komen de dbnamen nog eens terug. Voorlopig manueel aan te passen.

USE [master]
GO
ALTER DATABASE [gap_dev] SET RECOVERY SIMPLE WITH NO_WAIT
GO
ALTER DATABASE [kip_stg] SET RECOVERY SIMPLE WITH NO_WAIT
GO
