#!/bin/bash
#
# Dit script heeft als doel:
#    - Een gebruiker (login) te creeren/verwijderen, 
#    - Een database te ceeren/verwijderen voor die gebruiker op de database server. 
#
# Vermits dit een development database is per user, maak ik de database aan, 
# en creeer ook en gebruiker waarvan naam en paswoord de zelfde zijn als de database.
#
# Dit script werkt enkel binnen de Cygwin omgeving.  
# Alle hulp is welkom om dit script te converteren naar een M$ script. 
 
TMP_SQL_FILE=$(mktemp /tmp/cg2_database_create_sql_XXXX)
LOG_FILE=$(mktemp /tmp/$(basename $0)_XXXX)


(
  # Creatie van funties
  toon_help () 
  {
     cat <<EOF

	 Gebruik: $(basename $0) -h [-c | -d ] -n db_name -f -s SERVER
	 
		-h: Toon deze helppagina
		-c: We willen een database creeren
		-d: We willen een database verwijderen
		-n name: We willen de database met naam verwijderen/creeren.
		-f: Het script vraagt nooit voor confirmatie.
		-s: servernaam.  default: devserver
		-p: pad op de server (windowsnotatie) voor databasefiles
		    (standaard in C:\Program files\blablabla)
		
	 Dit script maakt een lege database.
	 Dit script verwacht dat de aanroeper de correcte permissies 
	 heeft op de database server.
	 
	 Mogelijke verbeteringen:
		- Toevoegen van -U/-P opties, om db_usernaam en db_paswoord 
		  van de database server te geven.
		- Toevoegen van -D/-H opties, om naar een andere DB server 
		  te gaan. 
EOF
  }  

  # Maak database script
  maak_create_script ()
  {
    cat >${TMP_SQL_FILE} <<EOF

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
-- Automatisch gegenereerd script door Microsoft SQL Server Management Studio, 
-- en gedeeltelijk aan elkaar geschreven.

CREATE DATABASE [${DB_NAME}] ON  PRIMARY 
( NAME = N'${DB_NAME}', FILENAME = N'${FILEPATH}\\${DB_NAME}.mdf' , SIZE = 5120KB , FILEGROWTH = 10%)
 LOG ON 
( NAME = N'${DB_NAME}_log', FILENAME = N'${FILEPATH}\\${DB_NAME}_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%)
COLLATE Latin1_General_CI_AI
GO
-- Sets certain database behaviors to be compatible with the specified earlier version of SQL Server. 
-- Set compatibel met: SQL Server 2005 
EXEC dbo.sp_dbcmptlevel @dbname=N'${DB_NAME}', @new_cmptlevel=90
GO

-- The full-text component is installed with the current instance of SQL Server.
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [${DB_NAME}].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO

ALTER DATABASE [${DB_NAME}] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [${DB_NAME}] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [${DB_NAME}] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [${DB_NAME}] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [${DB_NAME}] SET ARITHABORT OFF 
GO
ALTER DATABASE [${DB_NAME}] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [${DB_NAME}] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [${DB_NAME}] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [${DB_NAME}] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [${DB_NAME}] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [${DB_NAME}] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [${DB_NAME}] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [${DB_NAME}] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [${DB_NAME}] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [${DB_NAME}] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [${DB_NAME}] SET  ENABLE_BROKER 
GO
ALTER DATABASE [${DB_NAME}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [${DB_NAME}] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [${DB_NAME}] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [${DB_NAME}] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [${DB_NAME}] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [${DB_NAME}] SET  READ_WRITE 
GO
ALTER DATABASE [${DB_NAME}] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [${DB_NAME}] SET  MULTI_USER 
GO
ALTER DATABASE [${DB_NAME}] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [${DB_NAME}] SET DB_CHAINING OFF 

USE [${DB_NAME}]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [${DB_NAME}] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO

-- Creatie van de user, 
-- Opmerkingen zijn: Geen vervaldatum en policy voor de user.
-- (is een development database achter een goede firewall hoop ik)
-- 
-- Ik vermoed dat de user ook nog rechten moet kijgen op de default database, maar misschien kan dit automatisch gebeuren door de actie.
-- (niet zeker)

USE [master]
GO
CREATE LOGIN [${DB_NAME}] WITH PASSWORD=N'${DB_NAME}', DEFAULT_DATABASE=[${DB_NAME}], DEFAULT_LANGUAGE=[Nederlands], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

-- User mag enkel op zijn database.
USE [${DB_NAME}]
GO
CREATE USER [${DB_NAME}] FOR LOGIN [${DB_NAME}]
GO

-- Wat mag die user allemaal doen
USE [${DB_NAME}]
GO
EXEC sp_addrolemember N'db_owner', N'${DB_NAME}'
GO
EXEC sp_addrolemember N'db_datawriter', N'${DB_NAME}'
GO
EXEC sp_addrolemember N'db_datareader', N'${DB_NAME}'
GO


EOF
  }
 
  # verwijder database script
  verwijder_create_script ()
  {
    cat >${TMP_SQL_FILE} <<EOF
USE [master]
GO
DROP LOGIN [${DB_NAME}]
GO
DROP DATABASE [${DB_NAME}]
EOF
  }
 
  # Start Main
  
  # initialiseer variabelen 
  # (OPTARG en fags moeten niet geinit worden.)
 
  ACTIE="";
  FORCE=""; 
  DB_NAME=""  
  DBSERVER="DEVSERVER";   # default; straks misschien overschrijven
  FILEPATH='C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\Data';
  
  # Ophalen en controleren van de gegeven opies.
  while getopts  "cdn:s:p:fh" flag
  do 
	case ${flag} in 
		c) 	# Optie om database te creeren.
		    if [[ -z ${ACTIE} ]]; then
				ACTIE="CREATE";
			else
				echo "Fout: je kan otpies '-d' en '-c' te samen geven" >&2
				exit 1
			fi
			;;
		d) 	# Optie om database te verwijderen.
		    if [[ -z ${ACTIE} ]]; then
				ACTIE="DELETE";
			else
				echo "Fout: je kan otpies '-d' en '-c' te samen geven" >&2
				exit 1
			fi
			;;
		n) 	# Naam van de database te creeren. 
			DB_NAME=${OPTARG}
			;;
		s)	# Naam van de server
			DBSERVER=${OPTARG}
			;;
		p)	# Naam van de server
			FILEPATH=${OPTARG}
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
    echo "Create / Verwijdering van:"
	echo "   Actie           : ${ACTIE}"
    echo "   Gebruiker       : ${DB_NAME}"
    echo "   Met passwoord   : ${DB_NAME}"
    echo "   Database naam   : ${DB_NAME}"
    echo "   Databaseserver  : ${DBSERVER}"
    echo "   Bestandslocatie : ${FILEPATH}"
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
 
  # Creatie van het script
  case "${ACTIE}" in
    DELETE) verwijder_create_script
			;;
	CREATE) maak_create_script
	        ;;
	* )  echo "INTERNAL ERROR, ken optie ${ACTIE} niet bij creatie van scripts" >&2
	     echo "Will do nothing." >&2
	     exit 1
		 ;;
  esac
   
  # Ik veronderstel dat de persoon die dit uitvoert ADMIN rechten heeft op de database,
  # indien dit niet zo is dan kan met met de onderstaande opties de admin account doorgeven. 
  # -U loginid 
  # -P password
 
  cd /tmp  # M$ is niet instaat om met /tmp te werken, enkel gekend bij Cygwin.
  # TODO: Je kan dit properder via 'cygpath'

  sqlcmd -S ${DBSERVER} -H ${DBSERVER} -i $(basename ${TMP_SQL_FILE})
  cd -  
  
  echo "DEBUG: Uitgevoerd SQL script: ${TMP_SQL_FILE}" 
  echo "DEBUG: Mijn logs: ${TMP_SQL_FILE}"
  
)  2>&1 | tee ${LOG_FILE}
