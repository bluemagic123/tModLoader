:: After Pulling, Patching, and making sure the version number is changed in src, this bat will compile and create zips for all release.
:: It will also create a zip for ExampleMod

@ECHO off
:: Compile/Build exe 
echo "Building Release"
set tModLoaderVersion=v0.11.7.2
call buildRelease.bat

set destinationFolder=.\tModLoader %tModLoaderVersion% Release
@IF %ERRORLEVEL% NEQ 0 (
	pause
	EXIT /B %ERRORLEVEL%
)
@ECHO on

:: Make up-to-date Installers
::cd ..\installer2
::call createInstallers.bat
::cd ..\solutions

:: Folder for release
mkdir "%destinationFolder%"

:: Temp Folders
set win=%destinationFolder%\tModLoader Windows %tModLoaderVersion%
set mac=%destinationFolder%\tModLoader Mac %tModLoaderVersion%
set macReal=%destinationFolder%\tModLoader Mac %tModLoaderVersion%\tModLoader.app\Contents\MacOS
set lnx=%destinationFolder%\tModLoader Linux %tModLoaderVersion%
set mcfna=%destinationFolder%\ModCompile_FNA
set mcxna=%destinationFolder%\ModCompile_XNA
set pdbs=%destinationFolder%\pdbs

mkdir "%win%"
mkdir "%mac%"
mkdir "%lnx%"
mkdir "%mcfna%"
mkdir "%mcxna%"
mkdir "%pdbs%"

:: Windows release
robocopy /s ReleaseExtras\Content "%win%\Content"
robocopy /s ReleaseExtras\JourneysEndCompatibilityContent "%win%\Content"
robocopy /s ReleaseExtras\WindowsFiles "%win%"
copy ..\src\tModLoader\bin\WindowsRelease\net45\Terraria.exe "%win%\tModLoader.exe" /y
copy ..\src\tModLoader\bin\WindowsServerRelease\net45\Terraria.exe "%win%\tModLoaderServer.exe" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\tModLoader.pdb "%win%\tModLoader.pdb" /y
copy ..\src\tModLoader\bin\WindowsServerRelease\net45\tModLoaderServer.pdb "%win%\tModLoaderServer.pdb" /y
::copy ..\installer2\WindowsInstaller.jar "%win%\tModLoaderInstaller.jar" /y
::copy ReleaseExtras\README_Windows.txt "%win%\README.txt" /y
::copy ReleaseExtras\start-tModLoaderServer.bat "%win%" /y
::copy ReleaseExtras\start-tModLoaderServer-steam-friends.bat "%win%" /y
::copy ReleaseExtras\start-tModLoaderServer-steam-private.bat "%win%" /y

::call zipjs.bat zipDirItems -source "%win%" -destination "%win%.zip" -keep yes -force yes
call python ZipAndMakeExecutable.py "%win%" "%win%.zip"

:: Windows ModCompile
:: TODO: investigate why this isn't working on my machine
:: for /f %%i in ('..\setup\bin\setup --steamdir') do set steamdir=%%i
set steamdir=C:\Program Files (x86)\Steam\steamapps\common\tModLoader
:: Make sure to clear out ModCompile and run Setup Debugging so ModCompile folder is clean from old versions.
copy "%steamdir%\ModCompile" "%mcfna%"
del "%mcfna%"\buildlock 2>nul
copy ..\src\tModLoader\bin\WindowsRelease\net45\tModLoader.xml "%mcfna%" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\tModLoader.pdb "%mcfna%" /y
copy ..\references\MonoMod.RuntimeDetour.xml "%mcfna%" /y
copy ..\references\MonoMod.Utils.xml "%mcfna%" /y

::call zipjs.bat zipDirItems -source "%mcfna%" -destination "%mcfna%.zip" -keep yes -force yes
call python ZipAndMakeExecutable.py "%mcfna%" "%mcfna%.zip"

:: Linux release
robocopy /s ReleaseExtras\LinuxFiles "%lnx%"
robocopy /s ReleaseExtras\LinuxMacSharedFiles "%lnx%"
robocopy /s ReleaseExtras\Content "%lnx%\Content"
robocopy /s ReleaseExtras\JourneysEndCompatibilityContent "%lnx%\Content"
copy ..\src\tModLoader\bin\LinuxRelease\net45\Terraria.exe "%lnx%\tModLoader.exe" /y
copy ..\src\tModLoader\bin\LinuxServerRelease\net45\Terraria.exe "%lnx%\tModLoaderServer.exe" /y
copy ..\src\tModLoader\bin\LinuxRelease\net45\tModLoader.pdb "%lnx%\tModLoader.pdb" /y
copy ..\src\tModLoader\bin\LinuxServerRelease\net45\tModLoaderServer.pdb "%lnx%\tModLoaderServer.pdb" /y
copy ReleaseExtras\tModLoader-mono "%lnx%\tModLoader-mono" /y
copy ReleaseExtras\tModLoader-kick "%lnx%\tModLoader-kick" /y
copy ReleaseExtras\tModLoader-kick "%lnx%\tModLoader" /y
copy ReleaseExtras\tModLoader-kick "%lnx%\tModLoaderServer" /y
::copy ReleaseExtras\Terraria "%lnx%\Terraria" /y
::copy ..\references\I18N.dll "%lnx%\I18N.dll" /y
::copy ..\references\I18N.West.dll "%lnx%\I18N.West.dll" /y

::copy ..\installer2\LinuxInstaller.jar "%lnx%\tModLoaderInstaller.jar" /y
::copy ReleaseExtras\README_Linux.txt "%lnx%\README.txt" /y

::call zipjs.bat zipDirItems -source "%lnx%" -destination "%lnx%.zip" -keep yes -force yes
call python ZipAndMakeExecutable.py "%lnx%" "%lnx%.tar.gz"
call python ZipAndMakeExecutable.py "%lnx%" "%lnx%.zip"

:: Mac release
::copy "%lnx%" "%mac%"
robocopy /s ReleaseExtras\MacFiles "%mac%"
robocopy /s ReleaseExtras\LinuxMacSharedFiles "%macReal%"
robocopy /s ReleaseExtras\Content "%macReal%\Content"
robocopy /s ReleaseExtras\JourneysEndCompatibilityContent "%macReal%\Content"
copy ..\src\tModLoader\bin\MacRelease\net45\Terraria.exe "%macReal%\tModLoader.exe" /y
copy ..\src\tModLoader\bin\MacServerRelease\net45\Terraria.exe "%macReal%\tModLoaderServer.exe" /y
copy ..\src\tModLoader\bin\MacRelease\net45\tModLoader.pdb "%macReal%\tModLoader.pdb" /y
copy ..\src\tModLoader\bin\MacServerRelease\net45\tModLoaderServer.pdb "%macReal%\tModLoaderServer.pdb" /y
copy ReleaseExtras\tModLoader-mono "%macReal%\tModLoader-mono" /y
copy ReleaseExtras\tModLoader-kick "%macReal%\tModLoader-kick" /y
copy ReleaseExtras\tModLoader-kick "%macReal%\tModLoader" /y
copy ReleaseExtras\tModLoader-kick "%macReal%\tModLoaderServer" /y

::copy ..\installer2\MacInstaller.jar "%mac%\tModLoaderInstaller.jar" /y
::copy ReleaseExtras\README_Mac.txt "%mac%\README.txt" /y
::copy ReleaseExtras\osx\libMonoPosixHelper.dylib "%mac%\libMonoPosixHelper.dylib" /y

::call zipjs.bat zipDirItems -source "%mac%" -destination "%mac%.zip" -keep yes -force yes
call python ZipAndMakeExecutable.py "%mac%" "%mac%.zip"

:: Mono ModCompile
copy "%mcfna%" "%mcxna%"
del "%mcxna%\tModLoader.FNA.exe"
del "%mcxna%\FNA.dll"
del "%mcxna%\tModLoader.pdb"
copy ..\src\tModLoader\bin\MacRelease\net45\tModLoader.pdb "%mcxna%\tModLoader_Mac.pdb" /y
copy ..\src\tModLoader\bin\LinuxRelease\net45\tModLoader.pdb "%mcxna%\tModLoader_Linux.pdb" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\Terraria.exe "%mcxna%\tModLoader.XNA.exe" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\Microsoft.Xna.Framework.dll "%mcxna%" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\Microsoft.Xna.Framework.Game.dll "%mcxna%" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\Microsoft.Xna.Framework.Graphics.dll "%mcxna%" /y
copy ..\src\tModLoader\bin\WindowsRelease\net45\Microsoft.Xna.Framework.Xact.dll "%mcxna%" /y

::call zipjs.bat zipDirItems -source "%mcxna%" -destination "%mcxna%.zip" -keep yes -force yes
call python ZipAndMakeExecutable.py "%mcxna%" "%mcxna%.zip"

:: PDB backups
copy ..\src\tModLoader\bin\WindowsRelease\net45\tModLoader.pdb "%pdbs%\WindowsRelease.pdb" /y
copy ..\src\tModLoader\bin\WindowsServerRelease\net45\tModLoaderServer.pdb "%pdbs%\WindowsServerRelease.pdb" /y
copy ..\src\tModLoader\bin\MacRelease\net45\tModLoader.pdb "%pdbs%\MacRelease.pdb" /y
copy ..\src\tModLoader\bin\MacServerRelease\net45\tModLoaderServer.pdb "%pdbs%\MacServerRelease.pdb" /y
copy ..\src\tModLoader\bin\LinuxRelease\net45\tModLoader.pdb "%pdbs%\LinuxRelease.pdb" /y
copy ..\src\tModLoader\bin\LinuxServerRelease\net45\tModLoaderServer.pdb "%pdbs%\LinuxServerRelease.pdb" /y
call python ZipAndMakeExecutable.py "%pdbs%" "%pdbs%.zip"

:: CleanUp, Delete temp Folders
rmdir "%win%" /S /Q
rmdir "%mac%" /S /Q
rmdir "%lnx%" /S /Q
rmdir "%mcfna%" /S /Q
rmdir "%mcxna%" /S /Q
rmdir "%pdbs%" /S /Q

:: Copy to public DropBox Folder
::copy "%win%.zip" "C:\Users\Javid\Dropbox\Public\TerrariaModding\tModLoaderReleases\tModLoader Windows %tModLoaderVersion%.zip"
::copy "%mac%.zip" "C:\Users\Javid\Dropbox\Public\TerrariaModding\tModLoaderReleases\tModLoader Mac %tModLoaderVersion%.zip"
::copy "%lnx%.zip" "C:\Users\Javid\Dropbox\Public\TerrariaModding\tModLoaderReleases\tModLoader Linux %tModLoaderVersion%.zip"

:: ExampleMod.zip (TODO, other parts of ExampleMod release)
rmdir ..\ExampleMod\bin /S /Q
rmdir ..\ExampleMod\obj /S /Q
:: TODO: ignore .vs folder
::call zipjs.bat zipItem -source "..\ExampleMod" -destination "%destinationFolder%\ExampleMod %tModLoaderVersion%.zip" -keep yes -force yes
call python ZipAndMakeExecutable.py "..\ExampleMod" "%destinationFolder%\ExampleMod %tModLoaderVersion%.zip" ExampleMod\
::copy "%destinationFolder%\ExampleMod %tModLoaderVersion%.zip" "C:\Users\Javid\Dropbox\Public\TerrariaModding\tModLoaderReleases\"

echo(
echo(
echo(
echo tModLoader %tModLoaderVersion% ready to release.
echo Upload the 6 zip files to github.
echo(
echo(
pause
