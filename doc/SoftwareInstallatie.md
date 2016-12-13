Gebruikte software
==================

-   In theorie heb je Visual Studio 2010 SP 1 of later nodig. Maar dat
    hebben we al lang niet meer geprobeerd. We gebruiken meestal Visual
    Studio 2013 of 2015. Heb je nog geen Visual Studio, download dan
    gratis [Visual Studio Community
    Edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx).
-   In principe zit er wel wat git-support in Visual Studio 2013+, maar
    daar hebben we weinig ervaring mee. Voor git onder Windows raden we
    Git Extensions aan: http://gitextensions.github.io/. Zie
    [GitExtensions](GitExtensions.md) voor tips bij de installatie (doen!).
-   Verder gebruikt het GAP een SQL Server database. Heb je dat niet,
    dan zal Visual Studio je bij het openen van de gap solution
    voorstellen om Sql Server Express te installeren. Installeer ook de
    SQL Server Management Studio. (kies uit de lijst van downloads
    SQLEXPRADV, en niet SQLEXPR, dan kun je de management studio meteen
    installeren met de installatie van Sql Server Express.)

Een aantal andere zaken komen binnen met NuGet-magie. Daarvoor moet je
NuGet wel enablen: rechtsklik op je Solution en klik op 'Enable NuGet
restore'.

Voor de LoginService-solution gaat dat over de volgende NuGet-packages:

\* Unity: installeren in Chiro.Cdf.Ioc en Chiro.Cdf.UnityWcfExtensions\
\* Moq: installeren in Chiro.Ad.Workers.Test

Als je als GAP-developer geïnteresseerd bent om Resharper te gebruiken,
dan mag je Johan contacteren voor een key.
