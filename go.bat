@echo Loading %*
cd bin
set SimName=%*
set OSIM_LOGPATH = "%~dp0\bin\Regions\%SimName%"
@echo OpenSim.exe -inidirectory="Regions\%SimName%"

OpenSim.exe -inidirectory="Regions\%SimName%"
cd ..
echo %SimName% has exited.
