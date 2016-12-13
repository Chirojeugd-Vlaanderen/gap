EA: Problemen met locks
=======================

Om de UML-specificaties via Enterprise Architect te kunnen wijzigen,
moet je de betreffende files locken. Geen probleem; EA doet dat
automatisch voor jou. Maar nadien moet je er wel aan denken om je
wijzigingen te committen, anders blijft de lock behouden, en kan de rest
geen wijzigingen meer maken.

Als in EA in de contextmenu's 'Check out branch...' 'gegreyed' is, dan
heeft er vermoedelijk iemand een lock niet vrijgegeven. Wat te doen:

&lt;pre&gt;

&lt;pre&gt;\
E:\\dev\\gap\\Analyse&gt;svn status --show-updates\
O 1218 eaB\\B1667C7A94F8.xml\
O 1218 Business Layer.xml\
O 1218 Deployment Model.EAB\
O 1218 Domain Model.xml\
O 1218 Design Model.xml\
O 1218 eaF\\FAE6A451C334.xml\
O 1218 Full use cases.xml\
O 1218 Model.xml\
O 1218 DDL.xml\
O 1218 Requirements Model.xml\
O 1218 ea0\\00CA9A122BE9.xml\
O 1218 ea1\\1DE8DC603189.xml\
O 1218 ea1\\1F147E4127AE.xml\
O 1218 ea2\\24939507E71B.xml\
O 1218 Use Case Model.xml\
O 1218 ea8\\86EC43F943D4.xml\
Status against revision: 1218\
&lt;/pre&gt;

De lijnen die beginnen met de letter 'O', duiden erop dat een user de
betreffende file gelockt heeft. Om uit te vinden om welke user het gaat,
gebruik je 'svn info' op de url van zo'n bestand in de repository. Bijv:

&lt;pre&gt;\
E:\\dev\\gap\\Analyse&gt;svn info
https://develop.chiro.be/subversion/cg2/trunk/Analyse/Model.xml\
Path: Model.xml\
Name: Model.xml\
URL: https://develop.chiro.be/subversion/cg2/trunk/Analyse/Model.xml\
Repository Root: https://develop.chiro.be/subversion/cg2\
Repository UUID: 99b415e2-69d1-4072-8d93-0ed3a15484ab\
Revision: 1218\
Node Kind: file\
Last Changed Author: decatbr\
Last Changed Rev: 211\
Last Changed Date: 2009-01-19 01:36:13 +0100 (ma, 19 jan 2009)\
Lock Token: opaquelocktoken:886481f2-8a96-46a4-ae29-270b183dd99c\
Lock Owner: meersko\
Lock Created: 2009-12-08 18:53:55 +0100 (di, 08 dec 2009)\
Lock Comment (1 line):\
Proberen EA te updaten\
&lt;/pre&gt;

In dit geval is de user 'meersko' dus de boosdoener. Best hem eens
contacteren met de vraag het lock zo snel mogelijk vrij te geven. Indien
onmogelijk, kan je het lock ook 'breaken':
http://svnbook.red-bean.com/en/1.2/svn.advanced.locking.html\#svn.advanced.locking.break-steal
