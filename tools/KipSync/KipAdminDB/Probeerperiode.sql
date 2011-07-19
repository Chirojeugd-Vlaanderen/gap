USE Kipadmin;

ALTER TABLE lid.Lid
ADD EindeInstapPeriode DATETIME NULL;

ALTER TABLE rekening
ADD FacturerenVanaf DATETIME NULL;
