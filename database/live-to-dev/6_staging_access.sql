; Deze developers krijgen expliciete toegang tot de staging,
; om aan hun client voor de GAP-API te werken:

; Ik zet dit expliciet in de source code, zodat het voor iedereen
; duidelijk is wie er los van de gebruikersrechten op live
; toegang krijgt tot de staging-omgeving.

EXEC auth.spGebruikersRechtToekennen 'MG /0113', '260005';
GO

EXEC auth.spGebruikersRechtToekennen 'OJ /2511', '173010';
GO

