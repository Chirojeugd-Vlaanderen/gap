CREATE FUNCTION auth.ufnGenereerGebruikersNaam(@voornaam VARCHAR(MAX), @familieNaam VARCHAR(MAX))
-- DOEL: hack om accenten te verwijderen; interessant bij genereren van gebruikersnamen
RETURNS VARCHAR(MAX) AS
BEGIN
  DECLARE @fn AS VARCHAR(MAX);
  DECLARE @vn AS VARCHAR(MAX);

  IF LEFT(@familieNaam, 7) = 'VAN DE ' 
  BEGIN
    SET @fn = SUBSTRING(@familienaam, 8, LEN(@familienaam) - 7)
  END
  ELSE IF LEFT(@familieNaam, 4) = 'VAN ' 
  BEGIN
	SET @fn = SUBSTRING(@familienaam, 5, LEN(@familienaam) - 4)
  END
  ELSE IF LEFT(@familieNaam, 3) = 'DE ' 
  BEGIN
    SET @fn = SUBSTRING(@familienaam, 4, LEN(@familienaam) - 3)
  END
  ELSE
  BEGIN
    SET @fn = @familieNaam;
  END

  SET @fn = REPLACE(@fn, ' ', '');
  SET @vn = REPLACE(@voornaam, ' ', '');

  -- gekke constructie zodat ook Jaak Van Hest -> HestJaa
  RETURN core.ufnVerwijderAccenten(LEFT(LEFT(@fn, 5) + @vn, 7));
END;
GO

