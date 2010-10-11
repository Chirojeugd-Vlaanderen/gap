#!/bin/sh

# Stuur sql query naar server
# Contact: Johan Vervloet <johan.vervloet@chiro.be>

DBSERVER=database.chiro.lokaal
DBPORT=2142
DBUSER=$1
DBPASS=$2

SQSHCMD="sqsh -h -s, -S $DBSERVER:$DBPORT -U $DBUSER -P $DBPASS"

cat | $SQSHCMD

