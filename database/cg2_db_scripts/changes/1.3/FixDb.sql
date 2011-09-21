
ALTER TABLE auth.Gav DROP CONSTRAINT FK_Gav_Persoon
ALTER TABLE auth.Gav DROP COLUMN PersoonID
ALTER TABLE pers.Persoon DROP CONSTRAINT DF__Persoon__DubbelP__57DD0BE4
ALTER TABLE pers.Persoon DROP COLUMN DubbelPuntAbonnement
DROP INDEX IDX_Lid_EindeInstapPeriode_IsOvergezet_NonActief ON lid.Lid
CREATE INDEX IDX_Lid_EindeInstapPeriode_NonActief ON lid.Lid(EindeInstapPeriode, NonActief)
ALTER TABLE lid.Lid DROP CONSTRAINT DF_Lid_IsOvergezet
ALTER TABLE lid.Lid DROP COLUMN IsOvergezet

CREATE TABLE auth.GavSchap
(
	GavID INT NOT NULL,
	PersoonID INT NOT NULL
);

ALTER TABLE auth.GavSchap ADD CONSTRAINT PK_GavSchap PRIMARY KEY (PersoonID, GavID);

ALTER TABLE auth.GavSchap ADD CONSTRAINT FK_GavSchap_Gav FOREIGN KEY(GavID) REFERENCES auth.Gav(GavID);
ALTER TABLE auth.GavSchap ADD CONSTRAINT FK_GavSchap_Persoon FOREIGN KEY(PersoonID) REFERENCES pers.Persoon(PersoonID);


