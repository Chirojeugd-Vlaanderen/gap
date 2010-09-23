INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('Telefoonnummer', 0, '^0[1-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$', 'gebruik de vorm 03-123 45 67, 0474-12 34 56 of 070-12 34 56 (dus geen haakjes, puntjes of een schuine streep)');
GO
INSERT INTO [pers].[CommunicatieType] ([Omschrijving],[IsOptIn],[Validatie],[Voorbeeld]) VALUES ('Fax', 0, '^0[1-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$', 'gebruik de vorm 03-232 51 62 (dus geen haakjes, puntjes of een schuine streep)');
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
