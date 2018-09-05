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
    [String] $MetadataDbName,
    [String] $AppInsightsKey,
    [String] $SqlDataDirectory,
    [String] $SqlLogDirectory,
    [String] $AppEndpoint,
    [switch] $Quiet
)

[string[]] $requiredFiles = @($InstallFile, $Dacpac, $PublishProfile, "$PSScriptRoot\Install.config", "$PSScriptRoot\Terminology-Install-Utilities.psm1", "$PSScriptRoot\registration.config")
For ($i = 0; $i -lt $requiredFiles.Length; $i++) {
    if (!(Test-Path -Path $requiredFiles[$i])) {
        Throw "$($requiredFiles[$i]) does not exist and is required for install."
    }
}

# Import Dos Install Utilities
$MinimumVersion = [Version]::new(1, 0, 163)
Import-Module -Name DosInstallUtilities -MinimumVersion $MinimumVersion -ErrorAction Stop

# DBA tools
Import-Module -Name dbatools -ErrorAction Stop
 
# Import Fabric Install Utilities
$fabricInstallUtilities = ".\Fabric-Install-Utilities.psm1"
if (!(Test-Path $fabricInstallUtilities -PathType Leaf)) {
    Write-DosMessage -Level "Warning" -Message "Could not find FabricInstallUtilities. Manually downloading and installing..."
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1 -Headers @{"Cache-Control" = "no-cache"} -OutFile $fabricInstallUtilities
}
Import-Module -Name $fabricInstallUtilities -Force

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
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/Fabric.Identity/master/Fabric.Identity.API/scripts/Register.ps1 -Headers @{"Cache-Control" = "no-cache"} -OutFile $fabricRegistration
}

Import-Module "$PSScriptRoot\Terminology-Install-Utilities.psm1" -Force

$config = Get-TerminologyConfig -Credentials $Credentials -DiscoveryServiceUrl $DiscoveryServiceUrl -SqlAddress $SqlAddress -MetadataDbName $MetadataDbName -SqlDataDirectory $SqlDataDirectory -SqlLogDirectory $SqlLogDirectory -AppEndpoint $AppEndpoint -Quiet:$Quiet

Publish-DosWebApplication -WebAppPackagePath $InstallFile -AppPoolName $config.appPool -AppPoolCredential $config.iisUserCredentials -AppName $config.appName -IISWebSite $config.siteName

Update-AppSettings -Config $config

Update-DiscoveryService -Config $config

Publish-TerminologyDatabaseUpdates -Config $config -Dacpac $Dacpac -PublishProfile $PublishProfile

Add-MetadataAndStructures -Config $config

Test-Terminology -Config $config

Write-DosMessage -Level "Information" -Message "Registering Terminology with Fabric Authorization"
& $fabricRegistration -discoveryServiceUrl $DiscoveryServiceUrl -quiet:$Quiet