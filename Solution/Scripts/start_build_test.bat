@echo off

@if not "%CG2_PROJECT_DIR%" == "" (
  set CG2_PROJECT_DIR=E:\%USERNAME%\Builds
)

SET CG2_PROJECT_DIR=E:\%USERNAME%\Builds
SET DAY_YYYYMMDD_HHMMSS=%DATE:~9,4%%DATE:~6,2%%DATE:~3,2%_%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%
SET MY_WORK_DIR=%CG2_PROJECT_DIR%\%DAY_YYYYMMDD_HHMMSS%

Rem Setup for M$-Visual Studio 9.0
call "C:\Program Files\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"

Rem Create Directory
mkdir %MY_WORK_DIR%
cd /d %MY_WORK_DIR%

Rem Do the svn call to checkout all stuff from project
svn co https://develop.chiro.be/subversion/cg2/trunk/Solution >%MY_WORK_DIR%/%DAY_YYYYMMDD_HHMMSS%_svn.log

cd /d %MY_WORK_DIR%\Solution

Rem Clean and Build.
DevEnv Cg2Solution.sln /Clean >%MY_WORK_DIR%/%DAY_YYYYMMDD_HHMMSS%_clean.log
DevEnv Cg2Solution.sln /Build >>%MY_WORK_DIR%/%DAY_YYYYMMDD_HHMMSS%_build.log

MSTest /testmetadata:Cg2Solution1.vsmdi >%MY_WORK_DIR%/%DAY_YYYYMMDD_HHMMSS%_test.log
Rem Een deeltje van de testen doe als: 
Rem MSTest /testmetadata:Cg2Solution1.vsmdi /testlist:Chiro.Gap.Data

