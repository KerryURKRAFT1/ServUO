@Echo OFF
Echo Clean executables and dlls? (Y/N)
Echo This is permanent
Echo Removes all the compiled files, logs and other stuff not needed on repo
Echo You will only need to use this if comitting

Echo.
:Ask
set INPUT=
set /P INPUT=Type input: %=%
If /I "%INPUT%"=="y" goto yes 
If /I "%INPUT%"=="n" goto no
Echo.
Echo Incorrect input & goto Ask
Echo.

:yes

IF EXIST "%CURPATH%Ultima\obj\." RMDIR /S /Q "%CURPATH%Ultima\obj"
IF EXIST "%CURPATH%Scripts\obj\." RMDIR /S /Q "%CURPATH%Scripts\obj"
IF EXIST "%CURPATH%Server\obj\." RMDIR /S /Q "%CURPATH%Server\obj"

IF EXIST "%CURPATH%*.log" DEL /F "%CURPATH%*.log"
IF EXIST "%CURPATH%*.pdb" DEL /F "%CURPATH%*.pdb"
IF EXIST "%CURPATH%*.mdb" DEL /F "%CURPATH%*.mdb"
IF EXIST "%CURPATH%*.bak" DEL /F "%CURPATH%*.bak"

IF EXIST "%CURPATH%Ultima.dll" DEL /F "%CURPATH%Ultima.dll"
IF EXIST "%CURPATH%Ultima.pdb" DEL /F "%CURPATH%Ultima.pdb"

IF EXIST "%CURPATH%Scripts.dll" DEL /F "%CURPATH%Scripts.dll"
IF EXIST "%CURPATH%Scripts.pdb" DEL /F "%CURPATH%Scripts.pdb"

IF EXIST "%CURPATH%Server.dll" DEL /F "%CURPATH%Server.dll"
IF EXIST "%CURPATH%Server.pdb" DEL /F "%CURPATH%Server.pdb"

IF EXIST "%CURPATH%ServUO.exe" DEL /F "%CURPATH%ServUO.exe"
IF EXIST "%CURPATH%ServUO-MONO.exe" DEL /F "%CURPATH%ServUO-MONO.exe"

:no

Echo.
Echo ..done
Echo.
@ECHO ON
@PAUSE







