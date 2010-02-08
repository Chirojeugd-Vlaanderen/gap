CREATE FUNCTION core.ufnVerwijderAccenten(@tekst VARCHAR(MAX))
-- DOEL: hack om accenten te verwijderen; interessant bij genereren van gebruikersnamen
RETURNS VARCHAR(MAX) AS
BEGIN
RETURN REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
	REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
	REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
	REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
	REPLACE(REPLACE(@tekst COLLATE Latin1_General_CI_AI
	,'a','a'),'b','b'),'c','c'),'d','d'),'e','e'),'f', 'f')
	,'g','g'),'h','h'),'i','i'),'j','j'),'k','k'),'l', 'l')
	,'m','m'),'n','n'),'o','o'),'p','p'),'q','q'),'r', 'r')
	,'s','s'),'t','t'),'u','u'),'v','v'),'w','w'),'x', 'x')
	,'y','y'),'z','z')
END;
