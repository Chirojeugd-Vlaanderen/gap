SELECT 
	schema_name(t.schema_id) as schema_name, t.name 
	, c.name
	, c.collation_name
FROM 
	sys.tables t
	inner join sys.columns c
		on t.object_id = c.object_id
WHERE 
	t.type = 'U' and t.name not like 'sys%' 
	and c.collation_name is not null
	and c.collation_name <> 'Latin1_General_CI_AI'