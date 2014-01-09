USE [KipAdmin]
GO
/****** Object:  Trigger [TR_HeeftFunctie_UniekeFuncties_AfterIU]    Script Date: 08/27/2010 11:44:41 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_HeeftFunctie_UniekeFuncties_AfterIU]'))
DROP TRIGGER [dbo].[TR_HeeftFunctie_UniekeFuncties_AfterIU]