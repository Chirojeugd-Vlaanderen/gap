

declare @gg1Code as varchar(5); set @gg1Code='CP';
declare @gg2Code as varchar(5); set @gg2Code='GL';
declare @gv1Code as varchar(5); set @gv1Code='VB';
declare @fiCode as varchar(5); set @fiCode='FV';
declare @jrCode as varchar(5); set @jrCode='JR';
declare @kkCode as varchar(5); set @kkCode='CK';
declare @gpCode as varchar(5); set @gpCode='PR';



declare @gg1ID as int;
declare @gg2ID as int;
declare @gv1ID as int;
declare @fiID as int;
declare @jrID as int;
declare @kkID as int;
declare @gpID as int;


if not exists (select 1 from lid.Functie where code=@gg1Code)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('Contactpersoon', @gg1Code, 1, 1, 2, null, null);
	set @gg1ID = scope_identity();
end
else
begin
	set @gg1ID = (select FunctieID from lid.Functie where code=@gg1Code);
end

if not exists (select 1 from lid.Functie where code=@gg2Code)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('Groepsleiding', @gg2Code, 0, null, 2, null, null);
	set @gg2ID = scope_identity();
end
else
begin
	set @gg2ID = (select FunctieID from lid.Functie where code=@gg2Code);
end

if not exists (select 1 from lid.Functie where code=@gv1Code)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('VB', @gv1Code, 0, 0, 18, null, null);
	set @gv1ID = scope_identity();
end
else
begin
	set @gv1ID = (select FunctieID from lid.Functie where code=@gv1Code);
end

if not exists (select 1 from lid.Functie where code=@fiCode)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('Financieel verantwoordelijke', @fiCode, 1, 1, 2, null, null);
	set @fiID = scope_identity();
end
else
begin
	set @fiID = (select FunctieID from lid.Functie where code=@fiCode);
end

if not exists (select 1 from lid.Functie where code=@jrCode)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('Vertegenwoordiger in jeugdraad', @jrCode, 0, null, 3, null, null);
	set @jrID = scope_identity();
end
else
begin
	set @jrID = (select FunctieID from lid.Functie where code=@jrCode);
end

if not exists (select 1 from lid.Functie where code=@kkCode)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('Contactpersoon kookploeg', @kkCode, 0, null, 3, null, null);
	set @kkID = scope_identity();
end
else
begin
	set @kkID = (select FunctieID from lid.Functie where code=@kkCode);
end

if not exists (select 1 from lid.Functie where code=@gpCode)
begin
	insert into lid.Functie(Naam, Code, MinAantal, MaxAantal, LidType, WerkJaarVan, WerkJaarTot) values('Proost', @gpCode, 0, null, 2, null, null);
	set @gpID = scope_identity();
end
else
begin
	set @gpID = (select FunctieID from lid.Functie where code=@gpCode);
end

PRINT '// aan te passen in Gap.Orm.Functie.cs:'
PRINT ''
PRINT 'public enum GepredefinieerdeFunctie {';
PRINT '		Geen = 0,'
PRINT '		ContactPersoon = ' + CAST(@gg1ID AS VARCHAR(10)) + ',';
PRINT '		GroepsLeiding = ' + CAST(@gg2ID AS VARCHAR(10)) + ',';
PRINT '		Vb = ' + CAST(@gv1ID AS VARCHAR(10)) + ',';
PRINT '		FinancieelVerantwoordelijke = ' + CAST(@fiID AS VARCHAR(10)) + ',';
PRINT '		JeugdRaad = ' + CAST(@jrID AS VARCHAR(10)) + ',';
PRINT '		KookPloeg = ' + CAST(@kkID AS VARCHAR(10)) + ',';
PRINT '		Proost = ' + CAST(@gpID AS VARCHAR(10)) + ' };';

