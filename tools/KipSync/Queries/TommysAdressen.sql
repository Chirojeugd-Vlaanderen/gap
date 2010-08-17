Use KipAdmin
GO

SELECT     kipPersoon.AdNr, kipPersoon.Naam, kipPersoon.VoorNaam, kipPersoon.GeboorteDatum, kipPersoon.Geslacht, kipPersoon.Nationaliteit, 
                      kipPersoon.BurgerlijkeStandId, kipPersoon.RekeningNr, kipPersoon.Beroep, kipPersoon.BeroepsStatusId, kipPersoon.Overleden, kipPersoon.KlantBoekhouding, 
                      kipPersoon.GegevensBoekhoudingOvergezet, kipPersoon.Stempel, kipWoont.Geldig, kipWoont.GeenMailings, kipWoont.VolgNr, kipAdresType.Omschrijving, 
                      kipAdresType.Code, kipAdres.Straat, kipAdres.Nr, kipAdres.PostNr, kipAdres.Gemeente, kipAdres.Land
FROM         kipPersoon INNER JOIN
                      kipWoont ON kipPersoon.AdNr = kipWoont.AdNr INNER JOIN
                      kipAdres ON kipWoont.AdresId = kipAdres.AdresId INNER JOIN
                      kipAdresType ON kipWoont.AdresTypeId = kipAdresType.AdresTypeId
WHERE     (kipPersoon.Naam = 'Haepers') AND (kipPersoon.VoorNaam = 'Tommy')
  
  
  
GO


