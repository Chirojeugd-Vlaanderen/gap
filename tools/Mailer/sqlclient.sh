#!/bin/sh

# Stuur sql query naar server
# Contact: Johan Vervloet <johan.vervloet@chiro.be>

DBSERVER=database.chiro.lokaal
DBPORT=2142
DBUSER=CgApp
DBPASS=doemaariets

SQSHCMD="sqsh -h -s, -S $DBSERVER:$DBPORT -U $DBUSER -P $DBPASS"

cat | $SQSHCMD


