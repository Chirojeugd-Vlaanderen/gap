-- STAP X: gebruikersrechten devs
-- (nodig voor dev, niet voor tst)

use gap_dev
go



exec auth.spGebruikersRechtToekennen 'MJ /0108', 'KIPDORP\gouweme'
exec auth.spGebruikersRechtToekennen 'BJ /0204', 'broes-laptop2\Broes'
exec auth.spGebruikersRechtToekennen 'OG /1504', 'KIPDORP\booneba'
exec auth.spGebruikersRechtToekennen 'AG /0312', 'ATOMIUM\MDAAF68'
exec auth.spGebruikersRechtToekennen 'OM /2514', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MM /0813', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MG /0100', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MJ /0106', 'kipdorp\keustma'
exec auth.spGebruikersRechtToekennen 'MJ /0106', 'Mathias-PC\Mathias'