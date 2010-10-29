use kipadmin
update kipfunctie set soort=3 where code='jr'


use gap


-- gewestelijke dings

insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Verantwoordelijke groepsleidingsbijeenkomsten', 'GLB', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Somverantwoordelijke', 'SOM', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('IK-verantwoordelijke', 'IK', null, 0, 8);

insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Ribbelverantwoordelijke', 'RI', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Speelclubverantwoordelijke', 'SP', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Rakwiverantwoordelijke', 'RA', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Titoverantwoordelijke', 'TI', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Ketiverantwoordelijke', 'KE', null, 0, 8);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Aspiverantwoordelijke', 'AS', null, 0, 8);

-- verbondelijke dings (blijkbaar aparte somfuncties in kipadmin.  Losers.

insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Som gewesten', 'SOG', null, 0, 32);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Opvolger stadsgroepen', 'SG', null, 0, 32);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Verbondsraad', 'VR', null, 0, 32);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Verbondskern', 'VK', null, 0, 32);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('Startdagverantwoordelijke', 'SD', null, 0, 32);
insert into lid.functie(naam,code,maxaantal,minaantal,niveau) values ('SB-verantwoordelijke', 'SB', null, 0, 32);





