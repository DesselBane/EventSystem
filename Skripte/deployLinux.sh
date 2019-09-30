#!/usr/bin/env bash

cd ../

echo "####################"
echo "# Pulling git Repo #"
echo "####################"

git pull

if [ $? -ne 0 ]; then
	echo $?
	exit 1
fi
cd Angular/eventmanager

echo "###########################"
echo "# Installing NPM Packages #"
echo "###########################"

npm update

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
