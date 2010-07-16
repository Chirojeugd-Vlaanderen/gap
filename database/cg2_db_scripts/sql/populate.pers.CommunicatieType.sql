INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('Telefoonnummer', 0, '^0[1-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[1-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$', '03-231 07 95');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('Fax', 0, '^0[1-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$', '03-232 51 62');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('E-mail', 1, '^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$', 'iemand@chiro.be');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('Website', 0, '.*', 'www.chiro.be');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('MSN', 0, '^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$', 'iemand@chiro.be');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('XMPP (Jabber)', 0, '^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$', 'chiro@chat.facebook.com');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('Twitter', 0, '^@.*', '@chiro');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('StatusNet (identi.ca)', 0, '^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$', 'chiro@identi.ca');
GO
