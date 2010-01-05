#!/bin/bash
#
# Dit script heeft als doel om in een bestaande database user accounts toe te voegen.
# Mogelijke acties:
#	- Aanmaken van gebruikers.
#   - Hernoemen van gebruikers namen.
#   - Rechten toekennen of verwijderen van gebruikers naar bepaalde groepen.
#
# Bij het aanmaken van gebruikers controlleren we niet of de gebruikers naam gekend is door het systeem.
# (authenticatie wordt door M$ gedaan)

TMP_SQL_FILE=$(mktemp /tmp/$(basename $0)_sql_XXXX)
LOG_FILE=$(mktemp /tmp/$(basename $0)_XXXX)

(
  toon_help ()
  {
    cat <<EOF
	
	Gebruik: $(basename $0) -h -a [ creatie | hernoem | rechten ] -n <db_name> -f
	 
EOF
  }

  # Ophalen en controleren van de gegeven opies.
  while getopts  "a:n:fh" flag
  do 
	case ${flag} in 
		a) 	# Optie om actie door te geven
			case ${OPTARG} in
				creatie | hernoem | rechten ) ACTIE=${OPTARG}
			     ;;
				*)	echo "Fout: optie '${OPTARG}' kent de waarde '${OPTARG}' niet" >&2
				    toon_help
					exit 1
					;;
			esac
			;;
		n) 	# Naam van de database die we bewerken. 
			DB_NAME=${OPTARG}
			;;
		h)  # in alle andere gevallen hebben we een niet verwachte 
		    # optie gevonden of '-h'
			toon_help
			exit 0
			;;	
	esac
  done
 
  if [[ -z ${DB_NAME} ]]; then
    if [[ -n ${FORCE} ]]; then
	  echo "Bij optie '-f' (force) moet je een DB_NAAM geven (optie: '-n')" >&2
	  exit 1
	fi
	
    # Database name te creeren is niet meegegeven als optie, 
	# Ga deze nu vragen.
	echo ""
	echo "Geef de naam van de database dat je wil creeren."
    read DB_NAME
  fi
 
  # Gebruik de correcte database.
  echo "use [${DB_NAME}]" > ${TMP_SQL_FILE}
  echo "GO"               >>${TMP_SQL_FILE} 
  
  case ${ACTIE} in
    creatie)
	    echo "Welke gebruikersnaam wens je toe te voegen:"
		read USER_NAME
		echo "We gaan gebruiker ${USER_NAME} toevoegen daan de database; ${DB_NAME}"
		echo "Is dit correct? (J/n)" 
		read CONFIRM
	
	    # CONFIRM string naar hoofdletters transformeren 
	    # (zodat we kunnen kijken voor J en j) 
	    CONFIRM=$(echo ${CONFIRM} | tr [:lower:] [:upper:] )

        if [[ "${CONFIRM}" != "J" ]]; then
          echo "Abort, door de gebruiker" >&2
	      exit 1
        fi
        cat >>${TMP_SQL_FILE} <<EOF
		INSERT INTO auth.gav (Login) VALUES ('${USER_NAME}');
		GO
EOF
		;;
	hernoem)
	    echo "Welke gebruikersnaam wens je toe te vervangen:"
		echo "!! Let er op om '\' te escapen met '\\'"
		read OLD_USER_NAME
	    echo "Welke niewe naam wens je die te geven:"
		echo "!! Let er op om '\' te escapen met '\\'"
		read NEW_USER_NAME	
		echo "We gaan gebruiker ${OLD_USER_NAME} vervangen door gebruikersnaam: ${NEW_USER_NAME} in database: ${DB_NAME}"
		echo "Is dit correct? (J/n)" 
		read CONFIRM

	    # CONFIRM string naar hoofdletters transformeren 
	    # (zodat we kunnen kijken voor J en j) 
	    CONFIRM=$(echo ${CONFIRM} | tr [:lower:] [:upper:] )

        if [[ "${CONFIRM}" != "J" ]]; then
          echo "Abort, door de gebruiker" >&2
	      exit 1
        fi
		cat >>${TMP_SQL_FILE} <<EOF
		UPDATE auth.gav SET Login = '${NEW_USER_NAME}' WHERE Login = '${OLD_USER_NAME}';
		GO
EOF
	;;
	rechten)
		echo "Welke gebruikersnaam wens je rechten toe te kennen?"
		echo "!! Let er op om '\' te escapen met '\\'"
		read USER_NAME
		echo "De gebruiker (${USER_NAME}) krijgt toegang tot Groep (StamNr): "
		read STAMNR
		echo "Dit recht is geldig tot: DD/MM/YYYY"
		read TOT_DATUM
		echo "We gaan gebruiker ${USER_NAME}, rechten geven tot groep: ${STAMNR} tot datum: ${TOT_DATUM} in database: ${DB_NAME}"
		echo "Is dit correct? (J/n)" 
		read CONFIRM

	    # CONFIRM string naar hoofdletters transformeren 
	    # (zodat we kunnen kijken voor J en j) 
	    CONFIRM=$(echo ${CONFIRM} | tr [:lower:] [:upper:] )

        if [[ "${CONFIRM}" != "J" ]]; then
          echo "Abort, door de gebruiker" >&2
	      exit 1
        fi
		
		cat >>${TMP_SQL_FILE} <<EOF
		SET DateFormat dmy;

		DECLARE @MyGavID AS INT;
		SET	@MyGavID = (SELECT GavID FROM auth.Gav WHERE Login = '${USER_NAME}')
		DECLARE @MyGrpID AS INT;
		SET	@MyGrpID = (SELECT GroepID FROM grp.Groep WHERE Code = '${STAMNR}')

		INSERT INTO auth.GebruikersRecht (GavID, GroepID, VervalDatum)
			VALUES (@MyGavID, @MyGrpID,'${TOT_DATUM} 00:00:00'); 
			
		PRINT 'User: ${USER_NAME} (' + CAST (@MyGavID AS VARCHAR(10)) + ') heeft nu toegang tot groep ${STAMNR} (' + CAST (@MyGrpID AS VARCHAR(10)) + ')'
		PRINT 'Heeft nu toegang tot ${TOT_DATUM}'
		GO
EOF
	;;
  esac
  
  # Ik veronderstel dat de persoon die dit uitvoert de correcte rechten heeft op de database,
  # (database is aangemaakt door een dbadmin die het cg2_database.sh heeft gebruikt, om db en dbuser aan te maken)
  # indien dit niet zo is dan kan met met de onderstaande opties de admin account doorgeven. 
  # -U loginid 
  # -P password
 
  sqlcmd -S DEVSERVER -H DEVSERVER -U ${DB_NAME} -P ${DB_NAME} -i "$(cygpath.exe -w ${TMP_SQL_FILE})" -o "$(cygpath.exe -w ${TMP_SQL_FILE}.out)"
  
  # Toon de output file maar laat een aanal niet intressante dingen weg:
  cat ${TMP_SQL_FILE}.out | grep -v 'rows affected' | grep -e '[:alpha:]'
  
) 2>&1 | tee ${LOG_FILE}
