@echo off
TITLE DBC DB2 ADB to SQL
COLOR 0A
:menu
cls
ECHO                      *************************************
ECHO                      *         DBC DB2 ADB to SQL        *         
ECHO                      *                                   *
ECHO                      *         1 - DB2 to SQL            *
ECHO                      *                                   *
ECHO                      *         2 - ADB to SQL            *
ECHO                      *                                   *
ECHO                      *         3 - DBC to SQL            *
ECHO                      *                                   *
ECHO                      *         4 - Exit                  *
ECHO                      *************************************
ECHO                      *    E-mail:jenascore@gmail.com     *
ECHO                      *************************************
ECHO.

set /p input= Please enter numbers: 
IF %input%==* GOTO error
REM mangos Import
IF %input%==1 GOTO DB2
IF %input%==2 GOTO ADB
IF %input%==3 GOTO DBC
IF %input%==4 GOTO EXIT
GOTO error

:DB2
CLS
ConverSQL.dll db2\Item-sparse.db2
ConverSQL.dll db2\Item.db2
PAUSE
GOTO menu

:ADB
CLS
ECHO.
ConverSQL.dll adb\Item-sparse.adb
ConverSQL.dll adb\Item.adb
PAUSE
GOTO menu

:DBC
CLS
ECHO  Example: XXXX.dbc
ECHO.
SET /p dbc= Please enter the dbc name: 
if %dbc%. == . set dbc=
ConverSQL.dll dbc\%dbc%
PAUSE
GOTO menu

:EXIT
EXIT

:error
CLS
COLOR 0A
ECHO.
ECHO  Error,please enter again!
ECHO.
PAUSE
GOTO menu