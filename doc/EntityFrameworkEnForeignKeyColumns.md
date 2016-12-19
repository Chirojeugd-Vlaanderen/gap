We gebruiken geen foreign key columns in ons entity model
=========================================================

Als je nieuwe entiteiten toevoegt aan het entity model
(ChiroGroepModel.edmx), zorg er dan voor dat 'Include foreign key
columns in the model' uitgevinkt is. Anders krijgen we problemen met
[AttachObjectGraph](AttachObjectGraph.md), die op hun beurt kunnen leiden tot fouten ivm
updatepermissies die je niet nodig hebt.

[Image(Capture.PNG)](Image(Capture.PNG).md)
