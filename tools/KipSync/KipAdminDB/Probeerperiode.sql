USE Kipadmin;

ALTER TABLE lid.Lid
ADD EindeInstapPeriode DATETIME NULL;

ALTER TABLE rekening
ADD FacturerenVanaf DATETIME NULL;

GRANT UPDATE ON rekening TO kipSyncRole;

-- We hacken de view vi_REKENING_GROEP_PERSOON
-- om op die manier te vermijden dat facturen die
-- nog niet gefactureerd mogen worden, daadwerkelijk
-- niet gefactureerd worden.

ALTER VIEW [dbo].[vi_REKENING_GROEP_PERSOON]
AS
SELECT REKENING.NR, REKENING.TYPE, REKENING.REK_BRON, 
    REKENING.VERWIJSNR, REKENING.STAMNR, 
    REKENING.DAT_REK, REKENING.BEDRAG_BEF, 
    REKENING.BEDRAG_EUR, REKENING.FACTUUR, 
    REKENING.FACTUUR2, REKENING.AFDRUK_DAT, 
    REKENING.MEDEDELING, REKENING.FACTUURNR, 
    REKENING.DOORGEBOE, REKENING.FACLIJNEN, 
    REKENING.BET_VOOR, REKENING.BOEKING1, 
    REKENING.BOEKING2, REKENING.BOEKING3, 
    REKENING.BOEKING4, REKENING.BOEKING5, 
    REKENING.BOEKING6, REKENING.BOEKING7, 
    REKENING.BOEKING8, REKENING.BOEKING9, 
    REKENING.OPSPL_BEF1, REKENING.OPSPL_BEF2, 
    REKENING.OPSPL_BEF3, REKENING.OPSPL_BEF4, 
    REKENING.OPSPL_BEF5, REKENING.OPSPL_BEF6, 
    REKENING.OPSPL_BEF7, REKENING.OPSPL_BEF8, 
    REKENING.OPSPL_BEF9, REKENING.OPSPL_EUR1, 
    REKENING.OPSPL_EUR2, REKENING.OPSPL_EUR3, 
    REKENING.OPSPL_EUR4, REKENING.OPSPL_EUR5, 
    REKENING.OPSPL_EUR6, REKENING.OPSPL_EUR7, 
    REKENING.OPSPL_EUR8, REKENING.OPSPL_EUR9, 
    GROEP.NAAM AS GROEPSNAAM, 
    GROEP.TYPE AS GROEPSTYPE, GROEP.ACTIEF, 
    GROEP.NIEUW, GROEP.KOPPIG, GROEP.JR_STOP, 
    GROEP.STOP_CODE, GROEP.BET_ADNR, PERSOON.NAAM, 
    PERSOON.STRAAT_NR, PERSOON.POSTNR, 
    PERSOON.GEMEENTE, PERSOON.LAND, 
    PERSOON.ADR_TYPE1, PERSOON.ONGELDIG, PERSOON.TEL, 
    PERSOON.DOMICILIE
	, Bet_AdNr as AdNr, '' as Extra
FROM REKENING INNER JOIN
    GROEP ON 
    REKENING.STAMNR = GROEP.STAMNR LEFT OUTER JOIN
    PERSOON ON GROEP.BET_ADNR = PERSOON.ADNR
WHERE (FacturerenVanaf IS NULL OR FacturerenVanaf <= getdate())