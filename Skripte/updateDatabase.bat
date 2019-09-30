@setlocal enableextensions enabledelayedexpansion
@echo off

set Backup_Env = %ASPNETCORE_ENVIRONMENT%
set buildOption = ""

:loop
IF NOT "%1"=="" (
    IF "%1"=="-e" (
        SET ASPNETCORE_ENVIRONMENT=%2
        SHIFT
    )
    IF "%1"=="-no-build" (
        set buildOption="--no-build"
    )
    SHIFT
    GOTO :loop
)

cd ..\EventSystemWebApi

for /F "delims=" %%a in ('findstr /I "\"Provider\"" appsettings.%ASPNETCORE_ENVIRONMENT%.json') do set "Line=%%a"
set "ProviderLine=%Line:"=%"
set "ProviderLine=%ProviderLine::=%"
set "ProviderLine=%ProviderLine: =%"
set "ProviderLine=%ProviderLine:,=%"
set "ProviderLine=%ProviderLine:Provider=%"

set str1=%ProviderLine%

if not x%ProviderLine:MySql=%==x%ProviderLine% set ProjectPath="../MySqlContext/MySqlContext.csproj"
if not x%ProviderLine:MsSql=%==x%ProviderLine% set ProjectPath="../MsSqlContext/MsSqlContext.csproj"


if [%ProjectPath%] == [] exit 1

call dotnet ef database update -p %ProjectPath% %buildOption%

cd ..\Skripte
set ASPNETCORE_ENVIRONMENT = %Backup_Env%
endlocal