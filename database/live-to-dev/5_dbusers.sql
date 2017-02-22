/****** Object:  User [KIPDORP\gapservice]    Script Date: 05/03/2012 13:43:34 ******/
DROP USER [KIPDORP\gapservice]
GO

EXEC sp_addrolemember N'GapRole', N'kipdorp\gapservice_tst'
GO

GRANT EXECUTE ON auth.spWillekeurigeGroepToekennenAd TO GapHackRole;
GO


-- LET OP: rechten voor devs via sql server login: zie script X_dbUsers.sql



