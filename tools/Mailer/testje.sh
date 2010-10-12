#!/bin/sh

	ADuser=CHIROPUBLIC\\testjohan
	ADpass=(weggeknipt)
	gavLogin=testjohan
	groepId=490
	eindDatum="15-10-2010"
	adNr=`ldapsearch -LLL -H ldap://kipdc11.chiro.wereld:389 -b 'dc=chiro,dc=wereld' -D "$ADuser" -w "$ADpass" "(sAMAccountName=$gavLogin)"|grep pager|cut -f2 -d\:`
	if [ -n "$adNr" ]; then	
		eMail=`sed "s/_ADNUMMER_/$adNr/" EMail.sql | ./sqlclient.sh KipSyncApp KipSyncApp | grep '.\{1,\}@.\{1,\}\..*'`
		if [ -n "$eMail" ]; then
			echo $eindDatum login $gavLogin groep $groepId ad $adNr email $eMail
			sed "s/_DATUM_/$eindDatum/g" mailtje.txt | sed "s/_GROEPID_/$groepId/g" | mail -s "Leden en leiding in GAP" -t $eMail
			sleep 1
		fi
	fi
