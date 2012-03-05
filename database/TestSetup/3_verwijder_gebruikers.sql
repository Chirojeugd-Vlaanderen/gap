-- STAP 3: verwijder niet-testers, om te vermijden dat personen per ongeluk hun administratie in test doen.
--   * Iedereen in kipdorpdomein is sowieso ook tester
--   * Rest van de testers: hardgecodeerde lijst ad-nummers verderop.


use gap_tst;

delete gr 
from auth.Gav gav
join auth.GebruikersRecht gr on gav.GavID = gr.GavID
left outer join auth.GavSchap gs on gav.GavID=gs.GavID
left outer join pers.Persoon p on gs.PersoonID = p.PersoonID
where (gav.Login not like 'kipdorp\' and (p.AdNummer is null or p.AdNummer not in 
(168532, 68248, 60693, 120443, 60115, 21638, 45985, 121868, 106890, 45769, 128851, 166144, 74251, 17903, 127679, 148569, 135074, 161936, 143602, 141913, 102279, 36844, 122902, 117084, 59046, 138792, 24549, 118206, 134720, 32515, 108153, 145287, 138722, 113353, 62503, 126367, 22067, 62549, 144578, 141007, 70659, 156424, 39198, 76523)))


-- gebruikers testsysteem opnieuw toevoegen

exec auth.spGebruikersRechtToekennen 'mm /0706', 'chiropublic\vermeti'
