. $PSScriptRoot\Shared.ps1
Include-VisualStudioTools

$env:BuildScriptDirectory=$pwd;

Set-Location ../
$env:TargetProjectDirectory=$pwd;

Write-Output "================================================================ Restoring Packages"
msbuild /t:Restore  /p:Configuration=Release -maxcpucount:4
Write-Output "================================================================ Compiling"
msbuild /p:Configuration=Release -maxcpucount:4
$env:OSLDesktopVersion=((Get-Item $env:TargetProjectDirectory\bin\Release\OpenSoftwareLauncher.DesktopWinForms.exe).VersionInfo.FileVersion);

Remove-Item -Recurse -Force BuildScripts\release
mkdir BuildScripts\release\
Copy-Item bin\Release\* BuildScripts\release\
Copy-Item Locale\*.locale BuildScripts\release\Locale\
Copy-Item ..\LICENSE BuildScripts\release\LICENSE.txt
Copy-Item ..\CHANGELOG.txt BuildScripts\release\CHANGELOG.txt
Remove-Item BuildScripts\release\config.ini
Remove-Item BuildScripts\release\*.xml
Remove-Item BuildScripts\release\*.pdb
Remove-Item BuildScripts\release\*.dylib
Remove-Item BuildScripts\release\*.so

# . $PSScriptRoot\Build-ILMerge.ps1

Set-Location $env:BuildScriptDirectory
pause

. $PSScriptRoot\Build-Installer.ps1

cd $env:BuildScriptDirectory\minalyzeDeploy
.\publish.bat