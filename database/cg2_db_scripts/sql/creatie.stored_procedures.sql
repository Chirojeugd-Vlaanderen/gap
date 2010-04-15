
:r 	../dataconversie/GroepsInfoUitKipadmin.sql
GO
:r ../dataconversie/groepVolledigVerwijderen.sql
GO

CREATE FUNCTION core.ufnSoundEx
(
  @tekst VARCHAR(MAX)
)
RETURNS VARCHAR(8) AS
-- Doel: UDF die enkel soundex uitvoert, zodat we deze kunnen gebruiken in
-- entity framework
BEGIN
  RETURN
  (
	SELECT SOUNDEX(@tekst)
  )
END;
GO