#!/bin/bash
#
# Dit script heeft als doel om in een bestaande database groepen te migreren van KipAdmin.
# Mogelijke acties:
#	- Migreren van 1 bepaalde groep.
#   - Verwijderen van 1 bepaalde groep.
#   - Migreren van alle groepen.
#   - Verwijderen van alle groepen.
#
#  Voordat we dit kunnen doen, moet de database account waar naar we gaan migreren de volledige 
#  lees rechten heeft tot KipAdmin database. (die op de zelfde database server staat)
#
# Te doen: ik mis er nog een paar
#   use [KipAdmin]
#   GO
#   GRANT EXECUTE ON [dbo].[enkelCijfers] TO [ChiroGroep_meersko]
#   GO


TMP_SQL_FILE=$(mktemp /tmp/$(basename $0)_sql_XXXX)
LOG_FILE=$(mktemp /tmp/$(basename $0)_XXXX)

(
  toon_help ()
  {
    cat <<EOF
	
	Gebruik: $(basename $0) -h -a [ migratie | verwijder ] -g [ stamnr | alle_groepen] -n <db_name> -f -s <servernaam>
	 
EOF
  }

  DBSERVER="DEVSERVER";

  # Ophalen en controleren van de gegeven opies.
  while getopts  "a:g:n:s:fh" flag
  do 
	case ${flag} in 
		a) 	# Optie om actie door te geven
			case ${OPTARG} in
				migratie | verwijder) ACTIE=${OPTARG}
			     ;;
				*)	echo "Fout: optie '${OPTARG}' kent de waarde '${OPTARG}' niet" >&2
				    toon_help
					exit 1
					;;
			esac
			;;
		g)  #Naam van de groep de te migreren is
            STAMNR=${OPTARG}
            ;;			
		n) 	# Naam van de database die we bewerken. 
			DB_NAME=${OPTARG}
			;;

		s) 	# Naam van de database die we bewerken. 
			DBSERVER=${OPTARG}
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
	echo "Geef de naam van de database die je wil bewerken."
    read DB_NAME
  fi
 
  # Gebruik de correcte database.
  echo "use [${DB_NAME}]" > ${TMP_SQL_FILE}
  echo "GO"               >>${TMP_SQL_FILE} 
  
  case ${ACTIE} in
    migratie)
	    if [[ -z ${STAMNR} ]]; then
	      echo "Welke groep wens je toe te migreren?:"
		  read STAMNR
		fi
		echo "We gaan groep ${STAMNR} migreren van uit KipAdmin naar de database; ${DB_NAME}"
		echo "Is dit correct? (J/n)" 
		read CONFIRM
	
	    # CONFIRM string naar hoofdletters transformeren 
	    # (zodat we kunnen kijken voor J en j) 
	    CONFIRM=$(echo ${CONFIRM} | tr [:lower:] [:upper:] )

        if [[ "${CONFIRM}" != "J" ]]; then
          echo "Abort, door de gebruiker" >&2
	      exit 1
        fi
		
		if [[ "${STAMNR}" = "alle_groepen" ]]; then
		    cat >>${TMP_SQL_FILE} <<EOF
			  SET DateFormat dmy;
			  DECLARE	@return_value int
			  DECLARE @DoGroepID varchar(10)
			  DECLARE Cursor_GroepID CURSOR FOR SELECT DISTINCT StamNr from KipAdmin.dbo.Groep
			  OPEN Cursor_GroepID
			  FETCH NEXT FROM Cursor_GroepID into @DoGroepID
			  WHILE @@FETCH_STATUS = 0
			  BEGIN
			    EXEC	@return_value = [data].[spGroepUitKipadmin]
		              @stamNr = @DoGroepID
			    SELECT	'Return Value' = @return_value
			    FETCH NEXT FROM Cursor_GroepID into @DoGroepID
		      END
			  CLOSE Cursor_GroepID
			  DEALLOCATE Cursor_GroepID
EOF
		else
			cat >>${TMP_SQL_FILE} <<EOF
			DECLARE	@return_value int

			EXEC	@return_value = [data].[spGroepUitKipadmin]
				@stamNr = N'${STAMNR}'

			SELECT	'Return Value' = @return_value
		
			GO
EOF
		fi
		;;
	verwijder)
	    if [[ -z ${STAMNR} ]]; then
	      echo "Welke groep wens je toe te verwijderen?:"
		  read STAMNR
		fi
		echo "We gaan groep ${STAMNR} verwijderen uit KipAdmin in database; ${DB_NAME}"
		echo "Is dit correct? (J/n)" 
		read CONFIRM
	
	    # CONFIRM string naar hoofdletters transformeren 
	    # (zodat we kunnen kijken voor J en j) 
	    CONFIRM=$(echo ${CONFIRM} | tr [:lower:] [:upper:] )

        if [[ "${CONFIRM}" != "J" ]]; then
          echo "Abort, door de gebruiker" >&2
	      exit 1
        fi
		
		if [[ "${STAMNR}" = "alle_groepen" ]]; then
		    cat >>${TMP_SQL_FILE} <<EOF
			DECLARE	@return_value int
			DECLARE @DoGroepID char(10)
			DECLARE Cursor_GroepID CURSOR FOR SELECT DISTINCT StamNr from KipAdmin.dbo.Groep
			OPEN Cursor_GroepID
			FETCH NEXT FROM Cursor_GroepID into @DoGroepID
			WHILE @@FETCH_STATUS = 0
			BEGIN
			  EXEC	@return_value = [data].[spGroepVerwijderen]
			    @stamNr = N'@DoGroepID'
			  SELECT	'Return Value' = @return_value
			  FETCH NEXT FROM Cursor_GroepID into @DoGroepID
		    END
			CLOSE Cursor_GroepID
			DEALLOCATE Cursor_GroepID
EOF
		else
			cat >>${TMP_SQL_FILE} <<EOF
			DECLARE	@return_value int

			EXEC	@return_value = [data].[spGroepVerwijderen]
				@stamNr = N'${STAMNR}'

			SELECT	'Return Value' = @return_value
		
			GO
EOF
		fi
	;;
  esac
	
  
  # Ik veronderstel dat de persoon die dit uitvoert de correcte rechten heeft op de database,
  # (database is aangemaakt door een dbadmin die het cg2_database.sh heeft gebruikt, om db en dbuser aan te maken)
  # indien dit niet zo is dan kan met met de onderstaande opties de admin account doorgeven. 
  # -U loginid 
  # -P password
 
  sqlcmd -S ${DBSERVER} -H ${DBSERVER} -U ${DB_NAME} -P ${DB_NAME} -i "$(cygpath.exe -w ${TMP_SQL_FILE})" -o "$(cygpath.exe -w ${TMP_SQL_FILE}.out)"
  
  # Toon de output file maar laat een aanal niet intressante dingen weg:
  cat ${TMP_SQL_FILE}.out | grep -v 'rows affected' | grep -e '[:alpha:]'
  
) 2>&1 | tee ${LOG_FILE}
