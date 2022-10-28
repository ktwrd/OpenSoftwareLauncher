. $PSScriptRoot\Shared.ps1
Set-Location $env:TargetProjectDirectory
Write-Output "================================================================ Merging Binaries"
$APP_NAME="OpenSoftwareLauncher.DesktopClient.exe"
$ILMERGE_BUILD="Release"
$ILMERGE_VERSION="3.0.29"
$ILMERGE_PATH=Resolve-Path "~\.nuget\packages\ilmerge\${ILMERGE_VERSION}\tools\net452"

$ILMERGE_LibraryInclude=@(
    'kate.shared.dll',
    'System.Numerics.Vectors.dll',
    'System.Text.Encodings.Web.dll',
    'System.Runtime.CompilerServices.Unsafe.dll',
    'System.Text.Json.dll',
    'System.ValueTuple.dll'
)
$DLLFilesToCopy=(Get-ChildItem -Filter "bin\${ILMERGE_BUILD}\*.dll" | Select-Object -ExpandProperty Name | Out-String).Split([Environment]::NewLine, [StringSplitOptions]::RemoveEmptyEntries)
$DLLFilesToCopy | Where-Object { 
    $f = $_
    $ILMERGE_LibraryInclude | Where-Object { !$f.Contains($_) }
}
$DLLFilesToCopy = $DLLFilesToCopy | Select-String -NotMatch $ILMERGE_LibraryInclude
Remove-Item -Recurse -Force BuildScripts\release\
mkdir BuildScripts\release\
Copy-Item bin\${ILMERGE_BUILD}\* BuildScripts\release\
# Remove-Item BuildScripts\${ILMERGE_BUILD}\*.exe
Set-Location BuildScripts\${ILMERGE_BUILD}\
foreach ($itm in $ILMERGE_LibraryInclude)
{
    Remove-Item $itm
    Remove-Item ($itm -replace "dll$", "xml")
}
Set-Location $env:TargetProjectDirectory
Write-Output "Merging ${APP_NAME}"
$processThing = $(
    "${ILMERGE_PATH}\ILMerge.exe",
    "/target:winexe",
    "Bin\${ILMERGE_BUILD}\${APP_NAME}",
    "/lib:Bin\${ILMERGE_BUILD}",
    "/out:BuildScripts\release\${APP_NAME}",
    ($ILMERGE_LibraryInclude -join " ")
) -join " "
# powershell -Command "${processThing}"
pause
Set-Location $env:BuildScriptDirectory