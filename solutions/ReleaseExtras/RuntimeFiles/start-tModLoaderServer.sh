#!/bin/sh
script_path=$(readlink -f "$0")
script_dir=$(dirname "$script_path")
cd "$script_dir"

. InstallNetFramework.sh
clear

read -p "Use Steam Server (y)/(n) " steam

if [ ! $steam == "y" ]; then
	clear
	NetFramework\dotnet\5.0.0\dotnet tModLoaderServer.dll -server -config serverconfig.txt
	exit
fi

read -p "Select Lobby Type (f)riends/(p)rivate " lobby
clear

if [ $lobby == "f" ]; then 
	NetFramework\dotnet\5.0.0\dotnet tModLoaderServer.dll -server -steam -lobby friends -config serverconfig.txt
	exit
fi

NetFramework\dotnet\5.0.0\dotnet tModLoaderServer.dll -server -steam -lobby private -config serverconfig.txt