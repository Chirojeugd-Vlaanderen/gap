#!/bin/bash

# let op dat de mail gestuurd wordt van helpdesk@chiro.be!
# (de listserver zou hiervoor wel leuk zijn, maar OS kan niet
# overweg met AD 2008 r2)

# account die AD kan lezen
# todo: een dedicated voor maken

ADuser="CHIROPUBLIC\\testjohan"
ADpass="(weggeknipt)"

for lijn in `./sqlclient.sh CgApp doemaariets < TeMailenGavs.sql|grep -vi kipdorp|cut -f2-4 -d,`
do
	gavLogin=`echo $lijn | cut -f3 -d, | cut -f2 -d\\\\`
	groepId=`echo $lijn | cut -f1 -d,`
	eindDatum=`echo $lijn | cut -f2 -d, | tr / -`
	adNr=`ldapsearch -LLL -H ldap://kipdc11.chiro.wereld:389 -b 'dc=chiro,dc=wereld' -D "$ADuser" -w "$ADpass" "(sAMAccountName=$gavLogin)"|grep pager|cut -f2 -d\:`
	if [ -n "$adNr" ]; then	
		eMail=`sed "s/_ADNUMMER_/$adNr/" EMail.sql | ./sqlclient.sh KipSyncApp KipSyncApp`
		if [ -n "$eMail" ]; then
			echo $eindDatum login $gavLogin groep $groepId ad $adNr email $eMail
			sed "s/_DATUM_/$eindDatum/g" mailtje.txt | sed "s/_GROEPID_/$groepId/g" | mail -s "Leden en leiding in GAP" -t $eMail
			sleep 1
		fi
	fi
done
