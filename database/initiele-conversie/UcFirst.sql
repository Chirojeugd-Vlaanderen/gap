CREATE FUNCTION core.ufnUcFirst(@Tekst VARCHAR(MAX))
RETURNS VARCHAR(MAX) AS
-- Doel: maakt van eerste letter een hoofdletter, en van de rest kleine letters
BEGIN
  RETURN
  (
    SELECT CASE
      WHEN LEN(@Tekst) <= 1 THEN UPPER(@Tekst)
      ELSE UPPER(LEFT(@Tekst,1)) + LOWER(RIGHT(@Tekst, LEN(@Tekst) - 1))
    END AS UcFirst
  )
END;