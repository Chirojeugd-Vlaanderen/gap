-- OPGELET! Dit is een job voor op devsrv1 (of op de dev PC)
-- NIET VOOR OP DE LIVE DATABASE SERVER!

USE [msdb]
GO

/****** Object:  Job [restore-kip]    Script Date: 25/08/2015 22:07:55 ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]]    Script Date: 25/08/2015 22:07:56 ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'restore-kip', 
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
/****** Object:  Step [Het script]    Script Date: 25/08/2015 22:07:56 ******/
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
EXEC xp_DirTree ''c:\tmp\kipadmin'',1,1

SET @Recentste = ''c:\tmp\kipadmin\'' + (SELECT TOP 1 FileName FROM @BackupFiles WHERE FileName LIKE ''%fbu'' ORDER BY FileName DESC);

RESTORE DATABASE kipadmin_local FROM DISK = @recentste 
WITH  FILE = 1,  
MOVE N''kipadminData'' TO N''C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\kipadmin.mdf'',  MOVE N''kipadminLog'' TO N''C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\kipadmin_log.ldf'',  NOUNLOAD,  REPLACE,  STATS = 10

GO
', 
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


