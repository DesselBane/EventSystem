#!/usr/bin/env bash

cd ../Angular/eventmanager

echo "##############################################"
echo "# Starting NG build in Screen 'angularBuild' #"
echo "##############################################"

ng build -op ../../EventSystemWebApi/wwwroot

if [ $? -ne 0 ]; then
	echo $?
	exit 2
fi

screen -dmS angularBuild bash -c 'ng build -op ../../EventSystemWebApi/wwwroot --watch'
screen -ls
cd ../../EventSystemWebApi

echo "############################"
echo "# Restoring NuGet Packages #"
echo "############################"

dotnet restore

echo "############################################"
echo "# Starting webserver in Screen 'webserver' #"
echo "############################################"

dotnet build

if [ $? -ne 0 ]; then
	echo $?
	exit 3
fi

screen -dmS webserver bash -c 'dotnet run'
screen -ls
