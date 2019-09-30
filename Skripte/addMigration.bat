@echo off

set migrationName=%1

cd ../EventSystemWebApi
call dotnet build
call dotnet ef migrations add %migrationName% -p ..\MySqlContext\MySqlContext.csproj --no-build
call dotnet ef migrations add %migrationName% -p ..\MsSqlContext\MsSqlContext.csproj --no-build
cd ..\Skripte