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

Import-Module "$PSScriptRoot\Terminology-Intall-Utilities.psm1" -Force

Import-Modules $moduleFilePath

$config = Get-TerminologyConfig -Credentials $Credentials

Publish-DosWebApplication -WebAppPackagePath $installFile -AppPoolName $config.appPool -AppPoolCredential $config.iisUserCredentials -AppName $config.appName -IISWebSite $config.siteName

Update-AppSettings $config

Update-DiscoveryService $config

#Publish-TerminologyDatabaseUpdates

#Test-Terminology