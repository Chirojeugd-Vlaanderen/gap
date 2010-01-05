#!/bin/bash
#
# Dit script heeft als doel een al aangemaakte database te bewerken.
# Mogelijke acties:
#	- Creatie / Verwijdering shemas
#   - Creatie / Verwijdering constraints
#   - Populeren van standaard gegevens.
#
# Bij Creatie/Verwijdering acties veronderstellen we dat er niks/alles in de database zit,
# indien dit niet zo is dan kunnen er fouten op het scherm komen
#
# Dit script werkt enkel binnen de Cygwin omgeving.  
# Alle hulp is welkom om dit script te converteren naar een M$ script. 

TMP_SQL_FILE=$(mktemp /tmp/$(basename $0)_sql_XXXX)
LOG_FILE=$(mktemp /tmp/$(basename $0)_XXXX)

(
  toon_help () 
  {
     cat <<EOF
	 
	 Gebruik: $(basename $0) -h -a [creatie | verwijder | beide ]  -t [ shema | constraints | stored_procedures | gegevens | test ] -n <db_name> -f
	 
	   -h: Toon deze help pagina
	   -a: De actie die je wil doen.
	       We kennen de volgende acties:
				- creatie : We gaan enkel creaties doen.
				            (We houden geen rekening met de relaties tussen verschillende scripts)
				- verwijder: We gaan enkel verwijderen van gegevens / structuur
						    (We houden geen rekening met de relaties tussen verschillende scripts)
                - beide: We gaan eerst verwijderen en dan her creeren.
				         Bij het verwijderen houden we rekening houden met relaties tussen de scripts, 
						 eerst verwijderen we alle mogelijkde data, dit is in volgorde: constraints, shema.
						 De gegevens en test gegevens gaan samen weg met het verwijderen van de de tabellen (shema)
						 Het creeren moet in de volgorde: shema, constraints, gegevens, test.
						 Waar we stoppen is afhankelijk van de '-t' optie. 
				
        -t: Welk type actie wil je doen
            We kennen de volgende types:
                - shema: We gaan het shema creeren of verwijderen (afh van optie -a)
                - constraints: We gaan de constraints creeren of verwijderen (afh van optie -a)
				- stored_procedure: We gaan de stored procedures creeren of verwijderen (afh van optie -a)
                - gegevens: We gaan de standaard gegevens creeren of verwijderen (afh van optie -a)
				            standaard gegevens: GIS data, officiele afdelingen, Communictie types, .... 
                - test: We gaan de geevens nodig voor de test omgeving creeren of verwijderen (afh van optie -a)
				
		-n name: We willen de database met naam verwijderen/creeren.
		
		-f: Het script vraagt nooit voor confirmatie.				
				
EOF
  }

  # Ophalen en controleren van de gegeven opies.
  while getopts  "a:t:n:fh" flag
  do 
	case ${flag} in 
		a) 	# Optie om actie door te geven
			case ${OPTARG} in
				creatie | verwijder | beide ) ACTIE=${OPTARG}
			                        ;;
				*)	echo "Fout: optie '${OPTARG}' kent de waarde '${OPTARG}' niet" >&2
				    toon_help
					exit 1
					;;
			esac
			;;
		t) 	# Optie type van de actie door te geven.
		    case ${OPTARG} in
				shema | constraints | stored_procedures | gegevens | test) 	TYPE=${OPTARG}
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
	    f) 	# Optie om aan te geven dat ik weet wat ik doe, 
			# en nooit een confirmatie scherm wens te krijgen.
			# Kan gebruikt worden in geautomatiseerde scripts.
			FORCE=true
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
  
  if [[ -n ${FORCE} ]]; then
    # Bij force optie doen we niks.
	:
  else
    # Vraag de user voor confirmatie. (als hij niet -f)
    # gegeven heeft.
    echo ""
    echo "We gaan de volgende acies ondernemen:"
	echo "   Actie           : ${ACTIE}"
	echo "   Type            : ${TYPE}"
    echo "   Gebruiker       : ${DB_NAME}"
    echo "   Met passwoord   : ${DB_NAME}"
    echo "   Database naam   : ${DB_NAME}"
    echo "Is dit correct? (J/N)"
    read CONFIRM
	
	# CONFIRM string naar hoofdletters transformeren 
	# (zodat we kunnen kijken voor J en j) 
	CONFIRM=$(echo ${CONFIRM} | tr [:lower:] [:upper:] )

    if [[ "${CONFIRM}" != "J" ]]; then
      echo "Abort, door de gebruiker" >&2
	  exit 1
    fi
  fi
  
  # Zeker van zijn dat we in de correcte database zitten.
  echo "use [${DB_NAME}]" > ${TMP_SQL_FILE}
  echo "GO"               >>${TMP_SQL_FILE} 

  if [[ "${ACTIE}" != "beide" ]]; then
    # Creatie van het script
    # De naam van het script is:
    SQL_SCRIPT="${PWD}/sql/${ACTIE}.${TYPE}.sql"
	
    cat ${SQL_SCRIPT}       >>${TMP_SQL_FILE}
  else
    cat ${PWD}/sql/verwijder.stored_procedures.sql            >>${TMP_SQL_FILE}
    cat ${PWD}/sql/verwijder.constraints.sql                  >>${TMP_SQL_FILE}
    cat ${PWD}/sql/verwijder.shema.sql                        >>${TMP_SQL_FILE}

	# shema is het laagste dat we kunnen maken, dus dit doen we altijd.
    cat ${PWD}/sql/creatie.shema.sql                          >>${TMP_SQL_FILE}
    if [[ "${TYPE}" != "shema" ]]; then
      cat ${PWD}/sql/creatie.constraints.sql                  >>${TMP_SQL_FILE}
	  if [[ "${TYPE}" != "constraints" ]]; then 
	    cat ${PWD}/sql/creatie.gegevens.sql                   >>${TMP_SQL_FILE}
		if [[ "${TYPE}" != "stored_procedures" ]]; then
		  cat ${PWD}/sql/creatie.stored_procedures.sql        >>${TMP_SQL_FILE}		
		  if [[ "${TYPE}" != "gegevens" ]]; then
		  cat ${PWD}/sql/creatie.test.sql                     >>${TMP_SQL_FILE}
		  fi
		fi
	  fi
	fi
  fi
   
  # Ik veronderstel dat de persoon die dit uitvoert de correcte rechten heeft op de database,
  # (database is aangemaakt door een dbadmin die het cg2_database.sh heeft gebruikt, om db en dbuser aan te maken)
  # indien dit niet zo is dan kan met met de onderstaande opties de admin account doorgeven. 
  # -U loginid 
  # -P password
 
  sqlcmd -S DEVSERVER -H DEVSERVER -U ${DB_NAME} -P ${DB_NAME} -i "$(cygpath.exe -w ${TMP_SQL_FILE})" -o "$(cygpath.exe -w ${TMP_SQL_FILE}.out)"
  
  # Toon de output file maar laat een aanal niet intressante dingen weg:
  cat ${TMP_SQL_FILE}.out | grep -v 'rows affected' | grep -e '[:alpha:]'

)  2>&1 | tee ${LOG_FILE}