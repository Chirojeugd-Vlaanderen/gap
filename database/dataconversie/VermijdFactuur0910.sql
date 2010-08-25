update l
set l.isovergezet=1, l.eindeinstapperiode=null
from lid.lid l join grp.groepswerkjaar gwj on l.groepswerkjaarid=gwj.groepswerkjaarid
where gwj.werkjaar=2009