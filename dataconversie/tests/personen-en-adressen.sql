-- toont personen met adressen (rudimentair)

SELECT * 
FROM pers.Persoon p 
JOIN pers.PersoonsAdres pa ON p.PersoonID = pa.PersoonID
JOIN adr.Adres a ON pa.AdresID = a.AdresID
JOIN adr.Straat s ON a.StraatID = s.StraatID
JOIN adr.SubGemeente sg ON a.SubgemeenteID = sg.SubgemeenteID
ORDER BY p.Naam, p.VoorNaam