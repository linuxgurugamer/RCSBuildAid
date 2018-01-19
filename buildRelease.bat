
@echo off

rem Set variables here

set H=R:\KSP_1.3.1_dev
set GAMEDIR=RCSBuildAid
set LICENSE=LICENSE.md
set README=ReadMe.md

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"

rem Copy files to GameData locations


copy /Y "%1%2" "GameData\%GAMEDIR%\Plugins"

copy /Y %GAMEDIR%.version GameData\%GAMEDIR%
copy /Y ..\MiniAVC.dll GameData\%GAMEDIR%

if "%LICENSE%" NEQ "" copy /y  %LICENSE% GameData\%GAMEDIR%
if "%README%" NEQ "" copy /Y %README% GameData\%GAMEDIR%

rem Get Version info

set VERSIONFILE=%GAMEDIR%.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

echo Version:  %VERSION%

pause
rem Build the zip FILE

set FILE="%RELEASEDIR%\%GAMEDIR%-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData
