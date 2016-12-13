[LaTeX](LaTeX.md) mini howto
========================

(Opgelet: hier is blijkbaar wel wat misgelopen bij de migratie van trac
naar svn.)

Het document over onze C\#-coding standard, is gemaakt met
[LaTeX](LaTeX.md). Op deze pagina worden de basics van [LaTeX](LaTeX.md)
uitgelegd, zodat je het document kan wijzigen als je dat wil.

Software
--------

Er is heel wat software beschikbaar om met LaTeXdocumenten te werken. In
principe heb je een teksteditor nodig, en een 'compiler' die de
texbroncode omzet naar bijv. pdf.

In deze tekst werken we met [MikTeX](MikTeX.md). [MikTeX](MikTeX.md) bevat een
soort mini IDE'tje: TeXworks. TeXworks is eigenlijk gewoon een editor,
met een knop die de compiler aanroept.

Je kan [MikTeX](MikTeX.md) downloaden op http://miktex.org/2.8/setup. De
basisinstallatie moet genoeg zijn.

Manier van werken
-----------------

[LaTeX](LaTeX.md) is - in tegenstelling tot bijv. Word - niet WYSIWYG (What
you see is what you get). Je schrijft je tekst in een soort van
markuptaal, en daarna compileer je die tot bijvoorbeeld een
pdf-document; je kan het een beetje vergelijken met HTML.

Qua opmaak heb je weinig in de pap te brokken. Jij bepaalt de structuur,
[LaTeX](LaTeX.md) bepaalt de opmaak. Maar gelukkig ziet het resultaat er
meestal nogal goed uit.

Hello world
-----------

Een 'hello-world-document' ziet er bijvoorbeeld als volgt uit:

```
\\documentclass\[a4paper, 11pt\]{article}
\\usepackage\[dutch\]{babel}
\\begin{document}
Hallo \\LaTeX!
\\end{document}
```

In TeXworks kan je op de groene knop klikken om deze sourcecode om te
zetten naar een pdf. De groene knop voert in principe drie operaties
uit:

**** pdflatex document.tex

**** makeindex document

**** bibtex document
Enkel 'pdflatex' is relevant voor dit document; makeindex is nodig om
een index te maken - die hebben we hier niet, en bibtex dient om een
mooie bibliografie toe te voegen aan je document; ook niet nodig. Dus
voor een eenvoudig document als dit, kan je de functie van de groene
knop wijzigen naar enkel 'pdflatex'.

Als alles goed gaat, krijg je de gecompileerde pdf te zien. In het
andere geval krijg je een foutmelding, en zal je de fout moeten fixen
alvorens opnieuw te compileren.

Basisdingen
-----------

### Letters met accenten, speciale tekens

Je mag geen letters met accenten gebruiken in je [LaTeX](LaTeX.md)-broncode.
Om een accent op een letter te zetten, gebruik je dit soort
constructies:

  -- ------------- -- ------------ --
     **gebruik**      **output**   
     \\'a             á            
     \\@a             à            
     \\"a             ä            
     \\\^a            â            
     \\,c             ç            
  -- ------------- -- ------------ --

Je mag in principe speciale tekens gebruiken in een tex-document. Voor
alle speciale tekens, bestaat er wel een 'escape'. In sommige gevallen
is die heel eenvoudig:

```
\\\$, \\%, \\\_, \\}, \\&, \\\#, \\{,
```

Soms is het echter vervelend om het juiste symbool te vinden, zeker
omdat de lijst zo uitgebreid is. Zie
http://www.ctan.org/tex-archive/info/symbols/.../symbols-a4.pdf

### Aanhalingstekens, streepjes, puntjes

Aanhalingstekens zijn een beetje tricky. Je moet zelf het onderscheid
maken tussen het openen van aanhalingstekens (@; de backquote), en het
sluiten (' ; de gewone quote) van aanhalingstekens. Bijvoorbeeld:

(Voor de mensen met een Belgisch toetsenbord: de backquote staat meestal
2 toetsen rechts van de m; je hebt er de alt-gr-toets voor nodig.)

Ook zijn er een aantal speciallekes voor streepjes en puntjes:

### Titeltjes

```
\\section{\\LaTeX mini howto}
\\subsection{Basisdingen}

Een aantal basisdingen voor opmaak:

\\subsubsection{Letters met accenten}

\\'E\\'en twee drie vier, hoedje van p\\\^apier.

\\subsubsection{Aanhalingstekens, puntjes, streepjes}

`Enkele' en `@dubbele\_ quotes.
```

### Vet en cursief

Dit voorbeeld spreekt voor zich:

```
Je kan stukken tekst in \\textbf{vet} zetten, of \\emph{emphasis}
gebruiken. Meestal is dat cursief.

\\emph{Je kan `emph' ook nesten.  In een blok cursieve tekst, zal 
het stuk met \emph{emphasis} in `gewone rechte letters' getypeset
worden.}
```

### Opsommingen

Opsommingen kunnen via opsommingstekens:

```
\\begin{itemize}
\\item Chiro
\\item Kakajo
\\item IJsje
\\begin{itemize}
% Geneste itemize
% (O ja, % is voor commentaar)
\\item Waterijsje
\\item Cornetto
\\item \\ldots
\\end{itemize}
\\end{itemize}
```

maar ook met cijfers en letters

```
\\begin{enumerate}
\\item Chiro
\\item Kakajo
\\item IJsje
\\begin{enumerate}
\\item Waterijsje
\\item Cornetto
\\item \\ldots
\\end{enumerate}
\\end{enumerate}
```

of gemengd

```
\\begin{itemize}
\\item Chiro
\\item Kakajo
\\item IJsje
\\begin{enumerate}
\\item Waterijsje
\\item Cornetto
\\item \\ldots
\\end{enumerate}
\\end{itemize}
```

### Referenties

```
\\subsection{Hier ga ik naar verwijzen}
\\label{mijnlabel}
Dit is een zeer interessante tekst. Hier kom ik zeker op terug.

\\ldots

\\subsection{Vele secties later}

Herinner dat we het in \\ref{mijnlabel} hadden over \\ldots
```

Om het systeem van referenties goed te doen werken, moet je je document
twee maal compileren. De eerste keer maakt latex een hulpbestand aan,
met informatie over waar de referenties zich bevinden. Pas bij de tweede
compilatie worden de refs correct ingevuld.

Een iets uitgebreidere handleiding
----------------------------------

Een uitgebreidere handleiding voor [LaTeX](LaTeX.md), maar nog niet té, vind
je hier: http://mirror.ctan.org/info/lshort/dutch/lshort-nl-1.3.pdf

Specifiek voor de coding standard
=================================

Het document voor onze coding standard is bij opgenomen in de repo:
\[source:doc/codingstandard-cs\]. Om dit document te compileren, moet je
niet enkel latex gebruiken, maar ook makeindex. (om de index te maken,
inderdaad.)

In de coding standard worden een aantal specifieke truken gebruikt, die
misschien ook wel wat toelichting vragen.

Extra pakketten
---------------

De coding standaard gebruikt een aantal niet-standaard Latexpakketten
(enumitem, listings, textpos,...). Bij de eerste compilatie van het
document, zal gevraagd worden of die pakketten geinstalleerd mogen
worden. Klik gewoon 'ja'.

Chirohuisstijl
--------------

Om de coding style er een beetje te laten uitzien als een Chirodocument,
is er geprutst met het standaardlettertype en de standaardmarges. Ook
staat rechtsboven het typische 'Chirodocumentkadertje' met documentnr
e.d. De manier waarop dit gebeurt, is niet geweldig mooi. Maar het kan
dienen.

C\#-code
--------

Om C\#-code op te nemen in het document, gebruik je deze constructie
voor blokken code:

```
\\begin{lstlisting}
var gelieerdePersonen =
pm.PaginaOphalenMetLidInfo(
groepID,
pagina,
paginaGrootte,
out aantalTotaal);
\\end{lstlisting}
```

en deze constructie voor 'inline' code:

```
Documenteer alle \\lstinline ![](public) en \\lstinline ![](protected)
property's en methods.
```

(Let op de uitroeptekens.) Op die manier krijg je overal consistente
syntaxhighlighting.

De index
--------

```
Underscores\\index{underscores} zijn enkel toegelaten als prefix van
een member\\index{membervariabelen!namen} variable.
% Het uitroepteken wordt gebruikt om meerdere inveaus in de index
% te gebruiken.
```

Nummering van de guidelines
---------------------------

De guidelines van de coding standard zijn genummerd, en die nummering
loopt door van de ene 'enumerate' naar de andere. Dat is dankzij deze
truc:

```
\\begin{enumerate}\[resume\]
\\item Gebruik nooit de globale namespace.
\\item \\ldots
\\end{enumerate}
```

Als er een guideline verdwijnt, en je wil niet dat de nummering van de
volgende guidelines verandert, kan je op deze manier met de teller
prutsen:

```
\\addtocounter{enumi}{1}
```

Om een richtlijn ergens tussen te schuiven (100-a) zal ook wel een truc
bestaan, maar daar heb ik nog niet achter gezocht.
