/*
 * Script on Versie kolommen toe te voegen op alle user tabellen
 *
 */


-- Delete de not nullable versie kolommen

DECLARE curTables CURSOR
READ_ONLY
FOR 
SELECT schema_name(t.schema_id) as schema_name, t.name 
FROM 
	sys.tables t
	inner join sys.columns c
		on t.object_id = c.object_id and c.name = 'Versie'
WHERE 
	t.type = 'U' and t.name not like 'sys%' 
	and c.name is not null and c.is_nullable = 0
	
DECLARE @schemaname nvarchar(40), @tablename nvarchar(40)
OPEN curTables

FETCH NEXT FROM curTables INTO @schemaname,  @tablename
WHILE (@@fetch_status <> -1)
BEGIN
	IF (@@fetch_status <> -2)
	BEGIN
		DECLARE @sql nvarchar(1000)
		SET @sql = 'ALTER TABLE ' + @schemaname + '.' + @tablename + ' DROP COLUMN Versie;'
		PRINT @sql
		EXEC(@sql)
	END
	FETCH NEXT FROM curTables INTO @schemaname, @tablename 
END

CLOSE curTables
DEALLOCATE curTables
GO

-- Maak Versie kolommen aan, nullable
DECLARE curTables CURSOR
READ_ONLY
FOR 
SELECT schema_name(t.schema_id) as schema_name, t.name 
FROM 
	sys.tables t
	left outer join sys.columns c
		on t.object_id = c.object_id and c.name = 'Versie'
WHERE 
	t.type = 'U' and t.name not like 'sys%' and c.name is null

DECLARE @schemaname nvarchar(40), @tablename nvarchar(40)
OPEN curTables

FETCH NEXT FROM curTables INTO @schemaname,  @tablename
WHILE (@@fetch_status <> -1)
BEGIN
	IF (@@fetch_status <> -2)
	BEGIN
		DECLARE @sql nvarchar(1000)
		SET @sql = 'ALTER TABLE ' + @schemaname + '.' + @tablename + ' ADD Versie Timestamp NULL;'
		PRINT @sql
		EXEC(@sql)
	END
	FETCH NEXT FROM curTables INTO @schemaname, @tablename 
END

CLOSE curTables
DEALLOCATE curTables
GO

