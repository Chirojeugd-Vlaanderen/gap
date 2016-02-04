-- DIT IS ENKEL VOOR STAGING
-- Deze stored procedure haalt een echte database-backup van het GAP terug
-- (van 3 dagen oud, voor chirocivi test), en zet hem op de staging-database.

USE [msdb]
GO

/****** Object:  Job [restore-gap]    Script Date: 26/08/2015 10:05:14 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 26/08/2015 10:05:14 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'restore-gap', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'lap-jve-10\johan_000', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Het script]    Script Date: 26/08/2015 10:05:14 ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Het script', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'USE MASTER;

DECLARE @BackupFiles TABLE
(
	FileName sysname,
	Depth tinyint,
	IsFile tinyint
);

DECLARE @Recentste AS VARCHAR(MAX);

INSERT INTO @BackupFiles
EXEC xp_DirTree ''c:\tmp\gap'',1,1

-- Neem backup van 3 dagen terug, dat is op dit moment interessanter om de Chirocivi te testen
SET @Recentste = ''c:\tmp\gap\'' + (SELECT FileName FROM @BackupFiles WHERE FileName LIKE ''%fbu'' ORDER BY FileName DESC offset 3 rows fetch next 1 rows only)

RESTORE DATABASE gap_local FROM DISK = @recentste 
WITH  FILE = 1,  
MOVE N''gap1rc2'' TO N''C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\gap_local.mdf'',  MOVE N''gap1rc2_log'' TO N''C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\gap_local.ldf'',  NOUNLOAD,  REPLACE,  STATS = 10

GO

CREATE PROCEDURE [auth].[spWillekeurigeGroepToekennen] (@login varchar(40)) AS
-- Doel: Kent gebruikersrecht voor willekeurige actieve groep
-- toe aan user met gegeven login. (Enkel voor debugging purposes)
BEGIN 
	DECLARE @stamnr as varchar(10);

	set @stamnr = (select top 1 g.Code from grp.Groep g where g.StopDatum is null order by newid());
	exec auth.spGebruikersRechtToekennen @stamnr, @login
END


GO

CREATE ROLE GapHackRole;
GO

GRANT EXECUTE ON auth.spWillekeurigeGroepToekennen TO GapHackRole 
GO

go

create schema logging;
go

create table logging.Bericht(
	BerichtID integer identity(1,1) not null,
	Tijd datetime not null default getdate(),
	Niveau integer not null default 1,
	Boodschap varchar(max),
	StamNummer char(10) null,		-- stamnummer van een groep, indien van toepassing
	AdNummer integer null,				-- AD-nummer van een persoon, indien van toepassing
	PersoonID integer null,		-- persoonID (GAP) indien van toepassing
	GebruikerID integer null	-- gebruikerID, dat zullen we misschien in de toekomst gebruiken
);

alter table logging.Bericht add constraint PK_Bericht primary key(BerichtID);
alter table logging.Bericht add constraint FK_Bericht_Groep foreign key(StamNummer) references grp.Groep(code);
alter table logging.Bericht add constraint FK_Bericht_Persoon_Id foreign key (PersoonID) references  pers.Persoon(PersoonID);
alter table logging.Bericht add constraint FK_Bericht_Gebruiker foreign key (GebruikerID) references pers.Persoon(PersoonID);

create index IX_Bericht_Tijd on logging.Bericht(Tijd);
create index IX_Bericht_Niveau on logging.Bericht(Niveau);
create index IX_Bericht_StamNummer on logging.Bericht(StamNummer);
create index IX_Bericht_AdNummer on logging.Bericht(AdNummer);
create index IX_Bericht_PersoonID on logging.Bericht(PersoonID);

go


alter table pers.Persoon add LaatsteMembership int null;
go

update p
set p.LaatsteMembership = tmp.LaatsteMembership
from pers.persoon p join
(
select  l.adnr, max(l.werkjaar) as LaatsteMembership
from kipadmin_local.lid.lid l
left outer join kipadmin_local.lid.aansluiting a on l.groepid=a.groepid and l.werkjaar=a.werkjaar and l.aansl_nr=a.VolgNummer
left outer join kipadmin_local.dbo.rekening r on a.rekeningid = r.nr
where  l.soort = ''KA'' or (l.aansl_nr > 0 and r.DOORGEBOE=''j'')
group by l.adnr
) tmp on p.AdNummer=tmp.ADNR
go

-- Zet einde probeerperiode leden 2015-2016 al in augustus,
-- zodat we beter kunnen testen

update lid.lid set eindeinstapperiode=''2015-08-25'' where eindeinstapperiode=''2015-10-15''', 
		@database_name=N'master', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO

