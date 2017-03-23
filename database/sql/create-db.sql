USE master
GO
IF EXISTS(SELECT * FROM sys.databases WHERE name='gap_local')
	DROP DATABASE gap_local
GO

CREATE DATABASE gap_local
GO

