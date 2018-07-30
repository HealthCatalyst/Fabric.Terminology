<# TODO: Dev Ops install experience

- create (or update) Terminology app pool for {iisUser} (support for non AD appPool user??? ie. `IIS AppPool\DefaultAppPool`)
- unzip {zipPackage} to iis under {siteName}/{appName}
- set app pool to Terminology
- edit appsettings.json (see https://github.com/HealthCatalyst/Fabric.Terminology/blob/71d8f52111d493f21c4f7394d390a8326898d1c5/Fabric.Terminology.API/appsettings.json)
	- set sql server connection string to Data Source={sqlServerAddress};Integrated Security=SSPI;Initial Catalog={sharedDbName};Application Name=Terminology;
	- set discovery service url
- setup acl's
- grant db role {databaseRole} for {iisUser}

#>

param(
	$configFilePath = "$PSScriptRoot\Install.config",
	$applicationZipFile = "$PSScriptRoot\Fabric.Terminology.InstallPackage.zip",
	$moduleFilePath = ""
)

if([string]::IsNullOrEmpty($moduleFilePath)){
	Install-Module DosInstallUtilities -Scope CurrentUser

	Import-Module DosInstallUtilities -Force
}
else{
	Import-Module $moduleFilePath -Force
}

if(!(Test-Path .\Fabric-Install-Utilities.psm1)){
    Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1 -Headers @{"Cache-Control"="no-cache"} -OutFile Fabric-Install-Utilities.psm1
}
Import-Module -Name .\Fabric-Install-Utilities.psm1 -Force

$installSettings = Get-InstallationSettings "terminology"

[string] $credentialUsername = $installSettings.iisUser

#Todo: maybe we should prompt and not pick this up from the config file?
[securestring] $credentialPassword = ConvertTo-SecureString $installSettings.iisUserPwd -AsPlainText -Force
[PSCredential] $identityCredentials = New-Object System.Management.Automation.PSCredential ($credentialUsername, $credentialPassword)

#Not sure if we want to fetch the zip file from the config - if so, remove as a param.
Publish-DosWebApplication -WebAppPackagePath $applicationZipFile -AppPoolName $installSettings.appPool -AppPoolCredential $identityCredentials -AppName $installSettings.appName -IISWebSite $installSettings.siteName

$configurationReplacementTargets = @("**SqlServerConnectionString**","**DiscoveryServiceUri**")

$configurationReplacementValues = @($installSettings.sqlServerAddress, $installSettings.discoveryService)

$terminologyWebInstallFolder = Get-IISWebSitePath -WebSiteName $installSettings.siteName

Update-DosConfigFile -TargetPatterns $configurationReplacementTargets -ReplacementPattern $configurationReplacementValues -Delimiter "**" -FilePath "$terminologyWebInstallFolder\$($installSettings.appName)\appsettings.json"


<# TODO: Ben create Terminology shared db role for iis user

Shared Database Role
        
VIEW    Terminology.Code                    Read
VIEW    Terminology.CodeSystem              Read
VIEW    Terminology.ValueSetCode            Read
VIEW    Terminology.ValueSetCodeCount       Read
VIEW    Terminology.ValueSetDescription     Read
TABLE   ClientTerm.ValueSetCodeBASE         Read/Write
TABLE   ClientTerm.ValueSetCodeCountBASE    Read/Write
TABLE   ClientTerm.ValueSetDescriptionBASE  Read/Write

#>

<# TODO: Ben register Terminology with auth/identity

Fabric.Identity

	ApiResource:	terminology-api
	Scopes:			fabric/terminology.read, fabric/terminology.write
	Client:			terminology-client
	Scopes:			fabric/authorization.read
	Grant-types:	delegation, client_credentials

Fabric.Authorization

	Grain:			app
	Securable Item:	name: terminology         client-owner: terminology-client
	Role			terminology-publisher   securableItem: terminology
	Permission		grain: app           securableItem:  terminology       name: publisher
	RolePermission	terminology-publisher  has app/terminology.publisher
#>

<# TODO: Ben create terminology db through dacpac
- add the following to Fabric.Identity.InstallPackage.targets: https://github.com/HealthCatalyst/Fabric.Identity/blob/master/Fabric.Identity.API/scripts/Fabric.Identity.InstallPackage.targets
#>