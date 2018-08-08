param(
	$applicationZipFile = "$PSScriptRoot\Fabric.Terminology.InstallPackage.zip"
)

Import-Module "$PSScriptRoot\Terminology-Intall-Utilities.psm1" -Force

Import-Modules $moduleFilePath

$config = Get-TerminologyConfig $applicationZipFile

Publish-DosWebApplication -WebAppPackagePath $applicationZipFile -AppPoolName $config.appPool -AppPoolCredential $config.iisUserCredentials -AppName $config.appName -IISWebSite $config.siteName

Update-AppSettings $config

Update-DiscoveryService $config

#Publish-TerminologyDatabaseUpdates

#Test-Terminology