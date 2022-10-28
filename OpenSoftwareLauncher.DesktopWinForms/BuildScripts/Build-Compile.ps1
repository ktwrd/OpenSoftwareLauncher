. $PSScriptRoot\Shared.ps1
Include-VisualStudioTools

$env:BuildScriptDirectory=$pwd;
Set-Location ../
$env:TargetProjectDirectory=$pwd;

Write-Output "================================================================ Restoring Packages"
msbuild /t:Restore  /p:Configuration=Release -maxcpucount:4
Write-Output "================================================================ Compiling"
msbuild /p:Configuration=Release -maxcpucount:4

Remove-Item -Recurse -Force BuildScripts\release
mkdir BuildScripts\release\
Copy-Item bin\Release\* BuildScripts\release\

# . $PSScriptRoot\Build-ILMerge.ps1

Set-Location $env:BuildScriptDirectory

. $PSScriptRoot\Build-Installer.ps1
pause