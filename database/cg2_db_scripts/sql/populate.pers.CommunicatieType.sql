INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[Validatie],[Voorbeeld]) VALUES ('Telefoonnummer', '^0[1-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[1-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$', '03-231 07 95');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[Validatie],[Voorbeeld]) VALUES ('Fax', '^0[1-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$', '03-232 51 62');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[Validatie],[Voorbeeld]) VALUES ('E-mail', '^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$', 'iemand@chiro.be');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[Validatie],[Voorbeeld]) VALUES ('Website', '.*', 'www.chiro.be');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[Validatie],[Voorbeeld]) VALUES ('MSN', '^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$', 'iemand@chiro.be');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[Validatie],[Voorbeeld]) VALUES ('Jabber (Google)', '.*', '');
GO
