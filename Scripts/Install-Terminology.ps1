#Requires -RunAsAdministrator
#Requires -Version 4.0
#Requires -Modules PowerShellGet, PackageManagement

param(
    [ValidateNotNullorEmpty()]
    [ValidateScript( {
            if (!(Test-Path $_)) {
                Throw "$_ does not exist. Please enter valid path."
            }
            return $true
        })]
    [String] $InstallFile = "$PSScriptRoot\Fabric.Terminology.InstallPackage.zip",
    [ValidateNotNullorEmpty()]
    [ValidateScript( {
            if (!(Test-Path $_)) {
                Throw "$_ does not exist. Please enter valid path."
            }
            return $true
        })]
    [String] $Dacpac = "$PSScriptRoot\Fabric.Terminology.Database.dacpac",
    [ValidateNotNullorEmpty()]
    [ValidateScript( {
            if (!(Test-Path $_)) {
                Throw "$_ does not exist. Please enter valid path."
            }
            return $true
        })]
    [String] $PublishProfile = "$PSScriptRoot\Fabric.Terminology.Database.publish.xml",
    [String] $DiscoveryServiceUrl,
    [PSCredential] $Credentials,
    [String] $SqlAddress,
    [String] $EdwAddress,
    [String] $MetadataDbName,
    [String] $AppInsightsKey,
    [String] $SqlDataDirectory,
    [String] $SqlLogDirectory,
    [String] $AppEndpoint,
    [String] $LoaderWindowsUser,
    [String] $ProcessingServiceWindowsUser,
    [switch] $Quiet
)

# Fail installation on first error
$ErrorActionPreference = "Stop"

[string[]] $requiredFiles = @($InstallFile, $Dacpac, $PublishProfile, "$PSScriptRoot\Install.config", "$PSScriptRoot\terminology-registration.config", "$PSScriptRoot\Terminology.json", "$PSScriptRoot\SharedTerminology.json", "$PSScriptRoot\Terminology-Install-Utilities.psm1")
For ($i = 0; $i -lt $requiredFiles.Length; $i++) {
    if (!(Test-Path -Path $requiredFiles[$i])) {
        Throw "$($requiredFiles[$i]) does not exist and is required for install."
    }
}

# Import Dos Install Utilities
$minVersion = [System.Version]::new(1, 0, 164 , 0)
$dosInstallUtilities = Get-Childitem -Path ./**/DosInstallUtilities.psm1 -Recurse
if ($dosInstallUtilities.length -eq 0) {
    $installed = Get-Module -Name DosInstallUtilities
    if ($null -eq $installed) {
        $installed = Get-InstalledModule -Name DosInstallUtilities
    }

    if (($null -eq $installed) -or ($installed.Version.CompareTo($minVersion) -lt 0)) {
        Write-Host "Installing DosInstallUtilities from Powershell Gallery"
        Install-Module DosInstallUtilities -Scope CurrentUser -MinimumVersion 1.0.164.0 -Force
        Import-Module DosInstallUtilities -Force
    }
}
else {
    Write-Host "Installing DosInstallUtilities at $($dosInstallUtilities.FullName)"
    Import-Module -Name $dosInstallUtilities.FullName
}

# Fabric install utilities
if (!(Test-Path .\Fabric-Install-Utilities.psm1)) {
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1 -OutFile Fabric-Install-Utilities.psm1 -UseBasicParsing
}
Import-Module -Name .\Fabric-Install-Utilities.psm1 -Force

# DBA tools
$dbatools = Get-Childitem -Path ./**/dbatools.psm1 -Recurse
$minVersion = [System.Version]::new(0, 9, 12 , 0)
if ($dbatools.length -eq 0) {
    $installed = Get-Module dbatools
    if ($null -eq $installed) {
        $installed = Get-InstalledModule -Name dbatools
    }
    if (($null -eq $installed) -or ($installed.Version.CompareTo($minVersion) -lt 0)) {
        Write-DosMessage -Level "Warning" -Message "Installing dbatools from Powershell Gallery"
        Install-Module dbatools -Scope CurrentUser -MinimumVersion 0.9.12 -Force
        Import-Module dbatools -Force
    }
}
else {
    Write-DosMessage -Level "Verbose" -Message "Installing dbatools at $($dbatools.FullName)"
    Import-Module -Name $dbatools.FullName
}
 
# IIS web administration
try {
    Import-Module WebAdministration
}
catch {
    Write-Error -Message "Error importing module WebAdministration. Please verify IIS installed and you are running Windows Server 2012 or above"
    throw $_.Exception
}

# Download Registration Script
$fabricRegistration = ".\Register.ps1"
if (!(Test-Path $fabricRegistration -PathType Leaf)) {
    Write-DosMessage -Level "Warning" -Message "Could not find Fabric registration script. Manually downloading..."
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/Fabric.Identity/master/Fabric.Identity.API/scripts/Register.ps1 -Headers @{"Cache-Control" = "no-cache"} -OutFile $fabricRegistration -UseBasicParsing
}

# Terminology Install Utilities
Import-Module "$PSScriptRoot\Terminology-Install-Utilities.psm1" -Force

# .NET Core Server Hosting
if (!(Test-Prerequisite "*.NET Core*Windows Server Hosting*" 2.0.7)) {    
    try {
        Write-DosMessage -Level "Warning" -Message ".NET Core Runtime & Hosting Bundle for Windows not installed...installing version 2.1.3"        
        Invoke-WebRequest -Uri https://download.microsoft.com/download/6/E/B/6EBD972D-2E2F-41EB-9668-F73F5FDDC09C/dotnet-hosting-2.1.3-win.exe -OutFile $env:Temp\bundle.exe -UseBasicParsing
        Start-Process $env:Temp\bundle.exe -Wait -ArgumentList '/quiet /install'
        net stop was /y
        net start w3svc
    }
    catch {
        Write-DosMessage -Level "Error" -Message "Could not install .NET Windows Server Hosting bundle. Please install the hosting bundle before proceeding. https://www.microsoft.com/net/download/dotnet-core/2.0"
        throw $_.Exception
    }
    try {
        Remove-Item $env:Temp\bundle.exe
    }
    catch {
        Write-DosMessage -Level "Error" -Message "Unable to remove Server Hosting bundle exe" 
        throw $_.Exception
    }

}

$config = Get-TerminologyConfig -Credentials $Credentials -DiscoveryServiceUrl $DiscoveryServiceUrl -SqlAddress $SqlAddress -MetadataDbName $MetadataDbName -SqlDataDirectory $SqlDataDirectory -SqlLogDirectory $SqlLogDirectory -AppEndpoint $AppEndpoint -LoaderWindowsUserParam $LoaderWindowsUser -ProcessingServiceWindowsUserParam $ProcessingServiceWindowsUser -Quiet:$Quiet

Publish-DosWebApplication -WebAppPackagePath $InstallFile -AppPoolName $config.appPool -AppPoolCredential $config.iisUserCredentials -AppName $config.appName -IISWebSite $config.siteName

Update-AppSettings -Config $config

Update-DiscoveryService -Config $config

Publish-TerminologyDatabaseUpdates -Config $config -Dacpac $Dacpac -PublishProfile $PublishProfile

Add-MetadataAndStructures -Config $config

Write-DosMessage -Level "Information" -Message "Registering Terminology with Fabric Authorization"
& $fabricRegistration -discoveryServiceUrl $config.discoveryServiceUrl -authorizationServiceUrl $config.authorizationServiceUrl -identityServiceUrl $Config.identityServiceUrl -registrationFile "$PSScriptRoot\terminology-registration.config" -quiet -ErrorAction Stop

Test-Terminology -Config $config