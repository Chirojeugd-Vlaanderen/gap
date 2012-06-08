-- STAP X: gebruikersrechten devs


-- op dev-db

exec auth.spGebruikersRechtToekennen 'MJ /0108', 'KIPDORP\gouweme'
exec auth.spGebruikersRechtToekennen 'BJ /0204', 'broes-laptop2\Broes'
exec auth.spGebruikersRechtToekennen 'OG /1504', 'KIPDORP\booneba'
exec auth.spGebruikersRechtToekennen 'AG /0312', 'ATOMIUM\MDAAF68'
exec auth.spGebruikersRechtToekennen 'OM /2514', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MM /0813', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MG /0100', 'lap-jve2\johanv'
exec auth.spGebruikersRechtToekennen 'MJ /0106', 'kipdorp\keustma'
exec auth.spGebruikersRechtToekennen 'MJ /0106', 'Mathias-PC\Mathias'
exec auth.spGebruikersRechtToekennen 'MJ /0108', 'LBNL06686\THaeper'
exec auth.spGebruikersRechtToekennen 'BJ /0204', 'LBNL06686\THaeper'

-- op test-db

exec auth.spGebruikersRechtToekennen 'MJ /0106', 'kipdorp\keustma'
exec auth.spGebruikersRechtToekennen 'OM /2514', 'chiropublic\johan4'
exec auth.spGebruikersRechtToekennen 'MM /0813', 'chiropublic\johan4'
exec auth.spGebruikersRechtToekennen 'MG /0100', 'chiropublic\johan4'
