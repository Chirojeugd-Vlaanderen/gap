
create function core.ufnEnkelCijfers (
@string varchar(MAX)
) returns varchar(MAX)
AS
-- Doel: alle cijfers uit een string extraheren
BEGIN

DECLARE @newstring varchar(MAX)
declare @num int
select @newstring = ''
select @num = 1

while @num < len(@string)+1
begin

if Substring(@string, @num, 1) >= '0' and Substring(@string, @num, 1) <= '9'
set @newstring = @newstring + Substring(@string, @num, 1)

set @num = @num + 1
end
RETURN @newstring
END
