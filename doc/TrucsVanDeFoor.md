Trucs van de foor
=================

GAV-rechten in bulk invoeren
----------------------------

Inserts genereren voor GAV-rechten op basis van csv-bestand (kolom 1
login, kolom 3 stamnr, kolom 2 niet gebruikt)

```
cat users.txt | cut -f1,3 -d\\; | sed
"s/\\(\[\^;\]**\\).\\(.**\\)/insert into
auth.gebruikersrecht(gavid,groepid,vervaldatum) select gav.gavid,
gr.groepid, '2010-08-31' from auth.gav gav, grp.groep gr where
gav.login='\\1' and gr.code='\\2'/" &gt; output.sql
```

Lijnen code tellen
------------------

(aangepast van
http://stackoverflow.com/questions/114814/count-non-blank-lines-of-code-in-bash/114870\#114870;
waarschijnlijk kan het eenvoudiger)

```
find . -print | egrep '(\\.cs|\\.aspx|\\.\[Cc\]onf|\\.css|\\.js)\$' |
grep -v '\\.svn' | sed 's/.\*/"\\0"/' | xargs cat | sed '/\^\\s\*\$/d' |
wc -l
```
