USE gap;
GO

BEGIN TRAN

-- Niveau van een functie
--

ALTER TABLE lid.Functie
ADD Niveau INT NOT NULL DEFAULT 175  -- standaard: functie voor elk niveau
GO

--In praktijk zetten we alle bestaande lidtypes om naar niveaus, waarbij alle bestaande
--functies voor elk niveau behalve nationaal gebruikt mogen worden.

UPDATE lid.Functie SET Niveau = (LidType*2 | 45)

-- Voor de compatibiliteit voorlopig het oude veld (LidType) berekenen

ALTER TABLE lid.Functie DROP COLUMN LidType;
GO
ALTER TABLE	lid.Functie ADD LidType AS ((Niveau & ~1)/2) & 3;
GO

COMMIT TRAN