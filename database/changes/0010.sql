ALTER TABLE pers.CommunicatieVorm
ADD IsVerdacht bit not null default(0)
GO
ALTER TABLE pers.CommunicatieVorm
ADD LaatsteControle datetime not null default(getdate())
GO