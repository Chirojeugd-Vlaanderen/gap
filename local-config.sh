#!/bin/sh

cat << EOF
In onze repository zitten connection strings
die verwijzen naar de devserver op Kipdorp. Dat is een bug. (1744)

De repository bevat voor de betreffende configuratiefiles
ook .example-files, die geconfigureerd zijn voor een lokale
database, zoals beschreven in de wiki.

Dit script kopieert overschrijft de configuratiefiles door de
respectievelijke .example-files, en configureert git zodanig dat
de wijzigingen aan die bestanden genegeerd worden.

Dit is eigenlijk een lelijke workaround. Maar op die manier breek
ik niets bij de mensen die de configfiles uit de repository
gebruiken.

Druk ENTER om de configuratiebestanden te vervangen door de .example-
bestanden.
EOF

read a

cp    Solution/Chiro.Gap.Poco.Context/App.Config.example    Solution/Chiro.Gap.Poco.Context/App.Config
cp    Solution/Chiro.Gap.Services/Web.config.example    Solution/Chiro.Gap.Services/Web.config
cp    Solution/Chiro.Gap.UpdateSvc.ConsoleServiceHost/App.config.example    Solution/Chiro.Gap.UpdateSvc.ConsoleServiceHost/App.config
cp    Solution/Chiro.Gap.UpdateSvc.ServiceHost/App.config.example    Solution/Chiro.Gap.UpdateSvc.ServiceHost/App.config
cp    Solution/Chiro.Gap.WebApi/Web.config.example    Solution/Chiro.Gap.WebApi/Web.config
cp    Solution/Chiro.Gap.WebApp/Web.config.example    Solution/Chiro.Gap.WebApp/Web.config
cp    Solution/TestProjecten/Chiro.Gap.Services.Test/App.config.example    Solution/TestProjecten/Chiro.Gap.Services.Test/App.config
cp    tools/KipSync/Chiro.Kip.ConsoleServiceHost/app.config.example    tools/KipSync/Chiro.Kip.ConsoleServiceHost/app.config
cp    tools/KipSync/Chiro.Kip.Data/App.Config.example    tools/KipSync/Chiro.Kip.Data/App.Config
cp    tools/KipSync/Chiro.Kip.SyncService/App.config.example    tools/KipSync/Chiro.Kip.SyncService/App.config

git update-index --skip-worktree Solution/Chiro.Gap.Poco.Context/App.Config Solution/Chiro.Gap.Services/Web.config Solution/Chiro.Gap.UpdateSvc.ConsoleServiceHost/App.config Solution/Chiro.Gap.UpdateSvc.ServiceHost/App.config Solution/Chiro.Gap.WebApi/Web.config Solution/Chiro.Gap.WebApp/Web.config Solution/TestProjecten/Chiro.Gap.Services.Test/App.config tools/KipSync/Chiro.Kip.ConsoleServiceHost/app.config tools/KipSync/Chiro.Kip.Data/App.Config tools/KipSync/Chiro.Kip.SyncService/App.config
