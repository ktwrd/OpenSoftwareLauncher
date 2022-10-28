function Get-VSInstallPath {
    [CmdletBinding()] param (
        [Parameter()] [bool] $Prerelease = $true
    )

    Write-Verbose -Message "`$Prerelease == $Prerelease"
    $OutputContent = ""
    if ($Prerelease) {
        Write-Verbose -Message '"${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -prerelease -latest -property installationPath;'
        $OutputContent = & "${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -prerelease -latest -property installationPath | Out-String;
    }
    else {
        Write-Verbose -Message '"${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -latest -property installationPath;'
        $OutputContent = & "${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -latest -property installationPath | Out-String;
    }
    return $OutputContent.Trim()
}
function Get-VSInstanceID {
    [CmdletBinding()] param (
        [Parameter()] [bool] $Prerelease = $true
    )

    Write-Verbose -Message "`$Prerelease == $Prerelease"
    $OutputContent = ""
    if ($Prerelease) {
        Write-Verbose -Message '"${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -prerelease -latest -property instanceId;'
        $OutputContent = & "${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -prerelease -latest -property instanceId | Out-String;
    }
    else {
        Write-Verbose -Message '"${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -latest -property instanceId;'
        $OutputContent = & "${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -latest -property instanceId | Out-String;
    }
    return $OutputContent.Trim()
}
function Include-VisualStudioTools {
    $VisualStudioPath = (Get-VSInstallPath | Out-String).Trim()
    $VisualStudioInstanceID = (Get-VSInstanceID | Out-String).Trim()
    Import-Module "${VisualStudioPath}\Common7\Tools\Microsoft.VisualStudio.DevShell.dll";
    $currentDirectory = $pwd;
    Enter-VsDevShell $VisualStudioInstanceID
    Set-Location "${currentDirectory}"
}