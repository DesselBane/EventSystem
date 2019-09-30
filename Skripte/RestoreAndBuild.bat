cd ..\Angular\eventmanager
call npm install
call ng build -op ..\..\EventSystemWebApi\wwwroot
cd ..\..\
call dotnet restore
call dotnet build
cd Skripte
call .\updateDatabase.bat