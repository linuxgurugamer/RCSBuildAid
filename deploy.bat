
@echo off

set H=R:\KSP_1.3.1_dev
set GAMEDIR=RCSBuildAid

echo %H%

copy /Y "%1%2" "GameData\%GAMEDIR%\Plugins"



mkdir "%H%\GameData\%GAMEDIR%"
xcopy  /E /y GameData\%GAMEDIR% "%H%\GameData\%GAMEDIR%"
