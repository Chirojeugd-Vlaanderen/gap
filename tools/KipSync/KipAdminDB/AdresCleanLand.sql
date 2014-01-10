Use [KipAdmin]
GO

Begin transaction

UPDATE [dbo].[kipAdres]
   SET [Land] = ''
 WHERE [Land] in 
 (SELECT distinct [Land]
  FROM [KipAdmin].[dbo].[kipAdres]
  where [Land] like '%Bel%' or [Land] = 'BE' or [Land] = 'Begië')

UPDATE [dbo].[kipAdres]
   SET [Land] = ''
 WHERE [Land] is null
  
Commit
GO
