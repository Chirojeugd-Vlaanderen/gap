-- Een paar fixes in de overzichtstabel voor het bivak,
-- om verwarring bij het secretariaat te vermijden.


-- Verwijder afdelingsbivakken zonder adres uit de overzichtstabel

update biv.BivakOverzicht
set s_aangifteID=null, s_begin=null, s_eind=null, s_naam=null, s_straat=null, s_postnr=null, s_gemeente=null, s_land=null,s_provinci=null, s_tel=null, s_apart='N'
where werkjaar=2010 and (s_begin is not null or s_apart='J')
and (s_postnr is null or s_postnr='')

-- Verwijder 'gewone' bivakken zonder adres uit overzichtstabel

update biv.BivakOverzicht
set b_aangifteID=null, veran_adnr = null
where werkjaar=2010 and veran_adnr is not null
and (b_postnr is null or b_postnr='')

-- Buitenlands bivak en 'afdelingsbivak' expliciet aanvinken, indien ze bestaan

update biv.BivakOverzicht
set u_buiten='J'
where werkjaar=2010 and u_begin is not null and (u_buiten is null or u_buiten<>'J')

update biv.BivakOverzicht
set s_apart='J'
where werkjaar=2010 and s_begin is not null and (s_apart is null or s_apart<>'J')
