param(
    [ValidateNotNullorEmpty()]
    [ValidateScript({
            if (!(Test-Path $_)) {
                Throw "$_ does not exist. Please enter valid path."
            }
            return $true
        })]
    [String] $InstallFile = "$PSScriptRoot\Fabric.Terminology.InstallPackage.zip",
    [ValidateNotNullorEmpty()]
    [ValidateScript({
            if (!(Test-Path $_)) {
                Throw "$_ does not exist. Please enter valid path."
            }
            return $true
        })]
    [String] $Dacpac = "$PSScriptRoot\Fabric.Terminology.Database.dacpac",
    [ValidateNotNullorEmpty()]
    [ValidateScript({
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
    [switch] $Silent
)

# Import Dos Install Utilities
$dosInstallUtilities = Get-Childitem -Path ./**/DosInstallUtilities.psm1 -Recurse
if ($dosInstallUtilities.length -eq 0) {
    Install-Module DosInstallUtilities -Scope CurrentUser
    Import-Module DosInstallUtilities -Force
    Write-DosMessage -Level "Warning" -Message "Could not find dos install utilities. Manually installing"
}
else {
    Import-Module -Name $dosInstallUtilities.FullName
    Write-DosMessage -Level "Verbose" -Message "Installing DosInstallUtilities at $($dosInstallUtilities.FullName)"
}
 
# Import Fabric Install Utilities
$fabricInstallUtilities = ".\Fabric-Install-Utilities.psm1"
if (!(Test-Path $fabricInstallUtilities -PathType Leaf)) {
    Write-DosMessage -Level "Warning" -Message "Could not find fabric install utilities. Manually downloading and installing"
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1 -Headers @{"Cache-Control" = "no-cache"} -OutFile $fabricInstallUtilities
}
Import-Module -Name $fabricInstallUtilities -Force

Import-Module "$PSScriptRoot\Terminology-Install-Utilities.psm1" -Force

$config = Get-TerminologyConfig -Credentials $Credentials -DiscoveryServiceUrl $DiscoveryServiceUrl -SqlAddress $SqlAddress -MetadataDbName $MetadataDbName -Silent $Silent

Publish-DosWebApplication -WebAppPackagePath $InstallFile -AppPoolName $config.appPool -AppPoolCredential $config.iisUserCredentials -AppName $config.appName -IISWebSite $config.siteName

Update-AppSettings -Config $config

Update-DiscoveryService -Config $config

Publish-TerminologyDatabaseUpdates -Config $config -Dacpac $Dacpac -PublishProfile $PublishProfile

#Test-Terminology