@echo off

cd ..\EventSystemWebApi
call dotnet build
cd ..\Angular\eventmanager
call npm install
call ng build --op ..\..\EventSystemWebApi\wwwroot
cd ..\..\Skripte
call dropDatabase.bat -no-build
call updateDatabase.bat -no-build