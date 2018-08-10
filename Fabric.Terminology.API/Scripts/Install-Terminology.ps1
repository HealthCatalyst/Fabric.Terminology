param(
    [ValidateNotNullorEmpty()]
    [ValidateScript({
            if (!(Test-Path $_)) {
                Write-DosMessage -Level "Error" -Message "$_ does not exist. Please enter valid path." -ErrorAction Stop
            }
            else {
                return $true
            }
        })]
    [String] $installFile = "$PSScriptRoot\Fabric.Terminology.InstallPackage.zip",
    [PSCredential] $Credentials
)

# Import Dos Install Utilities
$dosInstallUtilities = Get-Childitem -Path ./**/DosInstallUtilities.psm1 -Recurse
if ($dosInstallUtilities.length -eq 0) {
    Write-DosMessage -Level "Warning" -Message "could not find dos install utilities. Manually installing"
    Install-Module DosInstallUtilities -Scope CurrentUser
    Import-Module DosInstallUtilities -Force
}
else {
    Write-DosMessage -Level "Verbose" -Message "Installing DosInstallUtilities at $($dosInstallUtilities.FullName)"
    Import-Module -Name $dosInstallUtilities.FullName -Force
}
 
# Import Fabric Install Utilities
$fabricInstallUtilities = ".\Fabric-Install-Utilities.psm1"
if (!(Test-Path $fabricInstallUtilities -PathType Leaf)) {
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1 -Headers @{"Cache-Control" = "no-cache"} -OutFile $fabricInstallUtilities
}
Import-Module -Name $fabricInstallUtilities -Force

Import-Module "$PSScriptRoot\Terminology-Intall-Utilities.psm1" -Force

$config = Get-TerminologyConfig -Credentials $Credentials

Publish-DosWebApplication -WebAppPackagePath $installFile -AppPoolName $config.appPool -AppPoolCredential $config.iisUserCredentials -AppName $config.appName -IISWebSite $config.siteName

Update-AppSettings $config

Update-DiscoveryService $config

#Publish-TerminologyDatabaseUpdates

#Test-Terminology