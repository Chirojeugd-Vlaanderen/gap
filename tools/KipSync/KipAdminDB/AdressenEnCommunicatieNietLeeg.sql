use kipadmin


delete w from 
kipWoont w join kipAdres a on w.AdresID = a.AdresID
where a.postnr is null or postnr=''

delete from kipAdres where postnr is null or postnr=''

DROP INDEX IDX_Adres_Straat_Nr_PostNr_Gemeente_Land_AdresId ON dbo.kipAdres

DROP STATISTICS dbo.kipAdres._dta_stat_1332915820_1_4
DROP STATISTICS dbo.kipAdres._dta_stat_1332915820_2_1_4
DROP STATISTICS dbo.kipAdres._dta_stat_1332915820_4_6_1
DROP STATISTICS dbo.kipAdres._dta_stat_1332915820_4_5_2_1
DROP STATISTICS dbo.kipAdres._dta_stat_1332915820_2_3_1_4_5

alter table kipAdres alter column postnr varchar(10) not null


DELETE FROM kipContactInfo WHERE info=''

ALTER TABLE dbo.kipContactInfo ADD CONSTRAINT
	CK_kipContactInfo CHECK ([info]<>'')