-- STAP X: gebruikersrechten devs
-- (enkel nodig voor dev, niet voor tst)

use gap_dev
go

-- Merijn Gouwelose
exec auth.spGebruikersRechtToekennen 'MJ /0108', 'KIPDORP\gouweme'

-- Bart B
exec auth.spGebruikersRechtToekennen 'OG /1504', 'KIPDORP\booneba'

-- Don't Know
exec auth.spGebruikersRechtToekennen 'AG /0312', 'ATOMIUM\MDAAF68'

-- Johan the man
exec auth.spGebruikersRechtToekennen 'OM /2514', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MM /0813', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MG /0100', 'lap-jve2\johanv'

-- Mathias Keustermans
exec auth.spGebruikersRechtToekennen 'MJ /0106', 'kipdorp\keustma'

-- Mattias Deparcq
exec auth.spGebruikersRechtToekennen 'MJ /0106', 'Mathias-PC\Mathias'

-- Tommy Haepers
exec auth.spGebruikersRechtToekennen 'OM /2514', 'CORP\THaeper'
exec auth.spGebruikersRechtToekennen 'OG /1504', 'CORP\THaeper'
exec auth.spGebruikersRechtToekennen 'MG /0300', 'CORP\THaeper'
exec auth.spGebruikersRechtToekennen 'tst/0001', 'CORP\THaeper'


-- Broes De Cat
exec auth.spGebruikersRechtToekennen 'BJ /0204', 'broes-laptop2\Broes'
exec auth.spGebruikersRechtToekennen 'MG /0200', 'broes-laptop2\Broes'
exec auth.spGebruikersRechtToekennen 'BG /0200', 'broes-laptop2\Broes'
