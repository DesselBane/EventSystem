@echo off

cd ../EventSystemWebApi

call dotnet build
call dotnet ef migrations remove -p ..\MySqlContext\MySqlContext.csproj --no-build
call dotnet ef migrations remove -p ..\MsSqlContext\MsSqlContext.csproj --no-build

cd ..\Skripte