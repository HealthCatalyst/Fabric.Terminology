Class TerminologyConfig
{
    [String]$siteName
    [String]$appName
    [String]$sqlServerAddress
    [String]$appPool
    [String]$applicationEndpoint
    [String]$discoveryServiceUrl
    [PSCredential]$iisUserCredentials
}
function Import-Modules() {
    # Dos Install Utilities
    $dosInstallUtilities = Get-Childitem -Path ./**/DosInstallUtilities.psm1 -Recurse
    if($dosInstallUtilities.length -eq 0) {
        Write-Warning "could not find dos install utilities. Manually installing "
        Install-Module DosInstallUtilities -Scope CurrentUser
        Import-Module DosInstallUtilities -Force
    } else {
        Import-Module -Name $dosInstallUtilities.FullName -Force
    }
	
	# Fabric Install Utilities
	$fabricInstallUtilities = ".\Fabric-Install-Utilities.psm1"
    if(!(Test-Path $fabricInstallUtilities -PathType Leaf)){
        Invoke-WebRequest -Uri https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1 -Headers @{"Cache-Control"="no-cache"} -OutFile $dosInstallUtilities
    }
    Import-Module -Name $fabricInstallUtilities -Force
}

function Get-TerminologyConfig{
    param(
        [string] $Path
    )
    
    $installSettings = Get-InstallationSettings "terminology"

    $iisUser = Read-Host "Press Enter to accept the default IIS App Pool User"
    $userEnteredPassword = Read-Host "Enter the password for $iisUser" -AsSecureString
    $credential = New-Object -TypeName "System.Management.Automation.PSCredential" -ArgumentList $iisUser, $userEnteredPassword
    [System.Reflection.Assembly]::LoadWithPartialName("System.DirectoryServices.AccountManagement") | Out-Null
    $ct = [System.DirectoryServices.AccountManagement.ContextType]::Domain
    $pc = New-Object System.DirectoryServices.AccountManagement.PrincipalContext -ArgumentList $ct, $credential.GetNetworkCredential().Domain
    $isValid = $pc.ValidateCredentials($credential.GetNetworkCredential().UserName, $credential.GetNetworkCredential().Password)
    if (!$isValid) {
        Write-Error "Incorrect credentials for $iisUser"
        throw
    }

    $config = [TerminologyConfig]::new()
    $config.iisUserCredentials = New-Object System.Management.Automation.PSCredential ($iisUser, $userEnteredPassword)
    $config.appName = $installSettings.appName
    $config.appPool = $installSettings.appPool
    $config.siteName = $installSettings.siteName
    if ([string]::IsNullOrWhiteSpace($installSettings.discoveryServiceUrl)) {
        $config.discoveryServiceUrl = Read-Host "Please enter the discovery service Uri (eg. https://SERVER/discoveryservice/v1)"
    } else {
        $config.discoveryServiceUrl = $installSettings.discoveryServiceUrl
    }
    

	return $config
}

function Update-AppSettings{
    param(
        [TerminologyConfig] $config
    )

    $appPath = Get-WebURL "IIS:\Sites\$($config.siteName)\$($config.appName)"
    $config.applicationEndpoint = $appPath.ResponseUri

    $appSettings = "$(Get-IISWebSitePath -WebSiteName $config.siteName)\$($config.appName)\appsettings.json"
    $appSettingsJson = (Get-Content $appSettings -Raw) | ConvertFrom-Json 
    $appSettingsJson.BaseTerminologyEndpoint = $config.applicationEndpoint
    $appSettingsJson.TerminologySqlSettings.ConnectionString = $config.sqlServerAddress
    $appSettingsJson.SwaggerRootBasePath = $config.appName
    $appSettingsJson.IdentityServerSettings.ClientSecret = $config.appName
    $appSettingsJson.DiscoveryServiceClientSettings.DiscoveryServiceUrl = $config.discoveryServiceUrl
    $appSettingsJson.ApplicationInsightsSettings.InstrumentationKey = "???"
    $appSettingsJson.ApplicationInsightsSettings.Enabled = $FALSE

    $appSettingsJson | ConvertTo-Json | Set-Content $appSettings
}

function Update-DiscoveryService() {
    param(
        [TerminologyConfig] $config
    )

    #$appDirectory = [System.IO.Path]::Combine($webroot, $appName)

    $discoveryPostBody = @{
        buildVersion = "0.1"# TODO: [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$appDirectory\Fabric.Terminology.API.dll").FileVersion;
        serviceName = "TerminologyService";
        serviceVersion = 1;
        friendlyName = "Fabric.Terminology";
        description = "The Fabric.Terminology Service provides shared healthcare terminology data.";
        serviceUrl = $config.applicationEndpoint;
    }
    Add-DiscoveryRegistration $config.discoveryServiceUrl $config.iisUserCredentials $discoveryPostBody
}

function Publish-TerminologyDatabaseUpdates {
    <# TODO: Ben create terminology db through dacpac
    - add the following to Fabric.Identity.InstallPackage.targets: https://github.com/HealthCatalyst/Fabric.Identity/blob/master/Fabric.Identity.API/scripts/Fabric.Identity.InstallPackage.targets
    #>
    
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
}

function Test-Terminology {
    # TODO: verify install was successful
    # Tests
    # 1. Ping
    # 2. Shared Db exists and ClientTerm.ValueSetCodeCountBASE Table exists (exists in 2.0)
}

Export-ModuleMember -function Import-Modules
Export-ModuleMember -function Get-TerminologyConfig
Export-ModuleMember -function Update-DiscoveryService
Export-ModuleMember -function Update-AppSettings
Export-ModuleMember -function Publish-TerminologyDatabaseUpdates
Export-ModuleMember -function Test-Terminology