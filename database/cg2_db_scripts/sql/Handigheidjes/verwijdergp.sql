
create procedure data.spGelieerdePersoonVerwijderen @gpid as integer as
-- doel: Verwijderen van een gelieerde persoon (persoon blijft wel bestaan)
begin
delete from pers.PersoonsCategorie where GelieerdePersoonID = @gpid;
delete from pers.CommunicatieVorm where GelieerdePersoonID = @gpid;
delete from pers.GelieerdePersoon where GelieerdePersoonID = @gpid;
end
