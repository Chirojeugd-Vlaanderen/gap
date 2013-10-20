CREATE PROCEDURE [auth].[spGebruikersRechtOntnemen]
(
	@stamNr VARCHAR(10)
	, @login VARCHAR(40)
)
AS
-- Doel: Ontneemt gebruikersrecht op gegeven groep
BEGIN
    DELETE gr
	FROM grp.Groep g JOIN auth.Gebruikersrecht gr ON g.GroepID = gr.GroepID
					JOIN auth.Gav gav ON gr.GavID = gav.GavID
	WHERE g.Code=@stamNr AND gav.Login=@login
END