ALTER TABLE znd.Abonnement ADD GroepID INT;
GO

UPDATE ab
SET ab.GroepID = cg.GroepID
FROM znd.Abonnement ab JOIN grp.ChiroGroep cg ON ab.StamNr = cg.StamNr
GO

ALTER TABLE znd.Abonnement ADD CONSTRAINT FK_Abonnement_Groep FOREIGN KEY(GroepID) REFERENCES grp.Groep(GroepID)
GO

ALTER TABLE znd.Abonnement DROP CONSTRAINT IX_kipAbonnement
GO

DROP INDEX IDX_Abonnement_WerkJaar_UitgCode_AdNr ON znd.Abonnement
GO

CREATE UNIQUE INDEX IX_Abonnement_WerkJaar_UitgCode_AdNr_Exemplaar_GroepID ON znd.Abonnement(WerkJaar, Uitg_Code, AdNr, Exemplaar, GroepID);
GO

DROP INDEX IDX_Abonnement_AdNr_WerkJaar ON znd.Abonnement
GO

CREATE INDEX IX_Abonnement_AdNr_WerkJaar ON znd.Abonnement(AdNr, WerkJaar)
GO

CREATE INDEX IX_Abonnement_Rref ON znd.Abonnement(R_Ref)
GO

DROP STATISTICS znd.Abonnement._dta_stat_1378819974_1_57_5_58
GO

DROP STATISTICS znd.Abonnement._dta_stat_1378819974_2_57_1_5_58
GO

ALTER TABLE znd.Abonnement DROP COLUMN StamNr
GO

ALTER VIEW [dbo].[kipAbonnement] AS
-- Geen paniek; automatisch gegenereerd :-)
SELECT	
	a.Uitg_Code
	, a.AdNr
	, a.Exemplaar
	, a.Aant_Exem
	, cg.StamNr
	, a.Gratis
	, a.Reden_Gra
	, a.Aanvr_Dat
	, a.Besteld1, a.Besteld2, a.Besteld3, a.Besteld4, a.Besteld5, a.Besteld6, a.Besteld7, a.Besteld8, a.Besteld9, a.Besteld10, a.Besteld11, a.Besteld12, a.Besteld13, a.Besteld14, a.Besteld15
, CASE WHEN zp1.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang1
, CASE WHEN zp2.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang2
, CASE WHEN zp3.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang3
, CASE WHEN zp4.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang4
, CASE WHEN zp5.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang5
, CASE WHEN zp6.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang6
, CASE WHEN zp7.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang7
, CASE WHEN zp8.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang8
, CASE WHEN zp9.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang9
, CASE WHEN zp10.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang10
, CASE WHEN zp11.Datum IS NULL THEN 'N' ELSE 'J' END AS Ontvang11
, 'N' AS Ontvang12
, 'N' AS Ontvang13
, 'N' AS Ontvang14
, 'N' AS Ontvang15
, zp1.Datum AS Dat_Ontv1
, zp2.Datum AS Dat_Ontv2
, zp3.Datum AS Dat_Ontv3
, zp4.Datum AS Dat_Ontv4
, zp5.Datum AS Dat_Ontv5
, zp6.Datum AS Dat_Ontv6
, zp7.Datum AS Dat_Ontv7
, zp8.Datum AS Dat_Ontv8
, zp9.Datum AS Dat_Ontv9
, zp10.Datum AS Dat_Ontv10
, zp11.Datum AS Dat_Ontv11
, null AS Dat_Ontv12
, null AS Dat_Ontv13
, null AS Dat_Ontv14
, null AS Dat_Ontv15
	, a.R_Ref
	, a.Stempel
	, CASE WHEN zp11.Datum IS NULL THEN 'N' ELSE 'J' END AS grasduiner_ontv
	, a.WerkJaar
	, a.Id
FROM znd.Abonnement a
LEFT OUTER JOIN grp.ChiroGroep cg ON a.GroepID = cg.GroepID
JOIN znd.Editie e1 ON a.WerkJaar = e1.JaarGang
JOIN znd.Zending z1 ON e1.ZendingID = z1.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp1
  ON zp1.ZendingID = z1.ZendingID and zp1.AdNr = a.AdNr
JOIN znd.Editie e2 ON a.WerkJaar = e2.JaarGang
JOIN znd.Zending z2 ON e2.ZendingID = z2.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp2
  ON zp2.ZendingID = z2.ZendingID and zp2.AdNr = a.AdNr
JOIN znd.Editie e3 ON a.WerkJaar = e3.JaarGang
JOIN znd.Zending z3 ON e3.ZendingID = z3.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp3
  ON zp3.ZendingID = z3.ZendingID and zp3.AdNr = a.AdNr
JOIN znd.Editie e4 ON a.WerkJaar = e4.JaarGang
JOIN znd.Zending z4 ON e4.ZendingID = z4.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp4
  ON zp4.ZendingID = z4.ZendingID and zp4.AdNr = a.AdNr
JOIN znd.Editie e5 ON a.WerkJaar = e5.JaarGang
JOIN znd.Zending z5 ON e5.ZendingID = z5.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp5
  ON zp5.ZendingID = z5.ZendingID and zp5.AdNr = a.AdNr
JOIN znd.Editie e6 ON a.WerkJaar = e6.JaarGang
JOIN znd.Zending z6 ON e6.ZendingID = z6.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp6
  ON zp6.ZendingID = z6.ZendingID and zp6.AdNr = a.AdNr
JOIN znd.Editie e7 ON a.WerkJaar = e7.JaarGang
JOIN znd.Zending z7 ON e7.ZendingID = z7.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp7
  ON zp7.ZendingID = z7.ZendingID and zp7.AdNr = a.AdNr
JOIN znd.Editie e8 ON a.WerkJaar = e8.JaarGang
JOIN znd.Zending z8 ON e8.ZendingID = z8.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp8
  ON zp8.ZendingID = z8.ZendingID and zp8.AdNr = a.AdNr
JOIN znd.Editie e9 ON a.WerkJaar = e9.JaarGang
JOIN znd.Zending z9 ON e9.ZendingID = z9.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp9
  ON zp9.ZendingID = z9.ZendingID and zp9.AdNr = a.AdNr
JOIN znd.Editie e10 ON a.WerkJaar = e10.JaarGang
JOIN znd.Zending z10 ON e10.ZendingID = z10.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp10
  ON zp10.ZendingID = z10.ZendingID and zp10.AdNr = a.AdNr
JOIN znd.Editie e11 ON a.WerkJaar = e11.JaarGang
JOIN znd.Zending z11 ON e11.ZendingID = z11.ZendingID
LEFT OUTER JOIN znd.ZendingNaarPersoon zp11
  ON zp11.ZendingID = z11.ZendingID and zp11.AdNr = a.AdNr
WHERE e1.Nummer = 1
AND e2.Nummer = 2
AND e3.Nummer = 3
AND e4.Nummer = 4
AND e5.Nummer = 5
AND e6.Nummer = 6
AND e7.Nummer = 7
AND e8.Nummer = 8
AND e9.Nummer = 9
AND e10.Nummer = 10
AND e11.Nummer = 11
GO

UPDATE znd.Abonnement SET R_REF = null WHERE R_Ref = 0 OR R_Ref= 13682
GO

ALTER TABLE znd.Abonnement ADD CONSTRAINT FK_Abonnement_Rekening FOREIGN KEY(R_Ref) REFERENCES Rekening(NR);
GO

------------------------------------------------------------
------------------------------------------------------------
---
--- aanpassen dubbels verwijderen
---
USE [KipAdmin]
GO
/****** Object:  StoredProcedure [dbo].[verwijderDubbelePersoon]    Script Date: 09/14/2010 10:01:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER  PROCEDURE [dbo].[verwijderDubbelePersoon] 
@oud ud_Adnr,
@nieuw ud_Adnr
AS
update lokaal set adnr=@nieuw where adnr=@oud;
update stage set adnr=@nieuw where adnr=@oud;
update stage set begeleider=@nieuw where begeleider=@oud;
update oud.stdag_cont set adnr=@nieuw where adnr=@oud;

-- indien 2 adnrs tegelijkertijd lid: verwijder het oude --

update project set adnr=@nieuw where adnr=@oud;
update tent set adnr=@nieuw where adnr=@oud;

-- indien 2 adnrs tegelijk leiding: verwijder oude --

-- verwijder functies oud als ze ook aan nieuw gekoppeld zijn --

delete from F1
from kipHeeftFunctie F1 
join kipLeidKad L1 on F1.leidkad = L1.id
join kipLeidKad L2 on L1.stamnr=L2.stamnr and L1.werkjaar=L2.werkjaar
join kipHeeftFunctie F2 on L2.id =F2.leidkad
where L1.adnr=@oud and L2.adnr=@nieuw and F1.functie=F2.functie;

-- zet oude functies over naar nieuwe

update kipHeeftFunctie
set kipHeeftFunctie.LeidKad = L2.id
from kipHeeftFunctie 
join kipLeidKad as L1 on kipHeeftFunctie.LeidKad = L1.id
join kipLeidKad as L2 on L1.stamnr=L2.stamnr and L1.werkjaar=L2.werkjaar
where L1.adnr=@oud and L2.adnr=@nieuw;

-- verwijder dubbele leden, leiding, kader

delete from L1
from lid.lid as L1 join lid.lid as L2 on L1.GroepID=L2.GroepID and L1.werkjaar=L2.werkjaar
where L1.adnr=@oud and L2.adnr=@nieuw;

update lid.lid set adnr=@nieuw where adnr=@oud;



delete from AP1
from attest_persoon as AP1 join attest_persoon as AP2 on AP1.ap_attid = AP2.ap_attid
where AP1.ap_adnr=@oud and AP2.ap_adnr=@nieuw;

update attest_persoon set ap_adnr=@nieuw where ap_adnr=@oud;
update cursus set c_hverantw=@nieuw where c_hverantw=@oud;
update cursus set c_verantw=@nieuw where c_verantw=@oud;
update cursus set c_finverantw=@nieuw where c_finverantw=@oud;

-- mogelijke dubbele cursusinschrijving oplossen

delete from PC1
from pers_per_cur as PC1 join pers_per_cur as PC2 on PC1.pc_cid=Pc2.pc_cid
where PC1.pc_adnr=@oud and PC2.pc_adnr=@nieuw;

-- verwijder ook eventuele dubbele zendingsrecords

delete from z1
from znd.ZendingNaarPersoon as z1 join znd.ZendingNaarPersoon as z2 on z1.ZendingId=z2.ZendingId 
where z1.AdNr=@oud and z2.AdNr=@nieuw;

-- verwijder eventuele dubbele abonnementen

delete from a1
from znd.Abonnement as a1 
join znd.Abonnement as a2 on a1.uitg_code=a2.uitg_code and a1.exemplaar=a2.exemplaar and a1.werkjaar=a2.werkjaar and a1.GroepID=a2.GroepID
where a1.AdNr=@oud and a2.AdNr=@nieuw;


update kipDeelnemer set persoon=@nieuw where persoon=@oud;
update pers_per_cur set pc_adnr=@nieuw where pc_adnr=@oud;
update kipInschrijving set verantwoordelijke = @nieuw where verantwoordelijke = @oud;
update rekening set adnr=@nieuw where adnr=@oud;
update oud.aspi_ho set adnr_cont=@nieuw where adnr_cont=@oud;
update oud.aspitrant set adnr=@nieuw where adnr=@oud;
update oud.krink_ho set adnr_cont=@nieuw where adnr_cont=@oud;
update kipAbonnement set adnr=@nieuw where adnr=@oud;
update kipPersoonlijkeInschrijving set factuurverantwoordelijke = @nieuw where factuurverantwoordelijke = @oud;
update kipBivak set aspi_adnr=@nieuw where aspi_adnr=@oud;
update kipBivak set veran_adnr=@nieuw where veran_adnr=@oud;
update kipBivak set kampe_adnr=@nieuw where kampe_adnr=@oud;
update gift set adnr=@nieuw where adnr=@oud;

update znd.ZendingNaarPersoon set AdNr=@nieuw where adnr=@oud;

delete from kipWoont where adnr=@oud;
delete from kipContactInfo where adnr=@oud;
delete from kipPersoon where adnr=@oud;

