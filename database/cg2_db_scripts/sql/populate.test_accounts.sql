
-- De volgende accounts nooit invoegen hier, deze worden ingevoegd voor en door
-- de unit_tests sql scripts
--     * Yvonne
--     * Yvette

-- Het is hier ook niet nodig om gebruikers al rechten te geven tot groepen, 
-- deze worden op een andere plaats aangemaakt.

-- KIPDORP Domain accounts
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\vervljo');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\bertepe');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\haepeto');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\decatbr');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\booneba');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\michama');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\LemmeSt');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('KIPDORP\meersko');

-- Administrator
INSERT INTO [auth].[Gav] ([Login]) VALUES ('administrator');

-- ????
INSERT INTO [auth].[Gav] ([Login]) VALUES ('nietgebruiken');

-- Persoonlijke accounts
INSERT INTO [auth].[Gav] ([Login]) VALUES ('CORP\THaeper');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('broes-laptop\broes de cat');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('SCOOTER\Steven');
INSERT INTO [auth].[Gav] ([Login]) VALUES ('HILBERT\peter');


