#!/bin/bash

# account die AD kan lezen
# todo: een dedicated voor maken

ADuser="CHIROPUBLIC\\testjohan"
ADpass="(weggehaald voor commit)"

for lijn in `./sqlclient.sh < TeMailenGavs.sql|grep -vi kipdorp|cut -f2,3 -d,`
do
	gavLogin=`echo $lijn | cut -f2 -d, | cut -f2 -d\\\\`
	groepId=`echo $lijn | cut -f1 -d,`
	adNr=`ldapsearch -LLL -H ldap://kipdc11.chiro.wereld:389 -b 'dc=chiro,dc=wereld' -D "$ADuser" -w "$ADpass" "(sAMAccountName=$gavLogin)"|grep pager|cut -f2 -d\:`
	echo login $gavLogin groep $groepID ad $adNr
done
