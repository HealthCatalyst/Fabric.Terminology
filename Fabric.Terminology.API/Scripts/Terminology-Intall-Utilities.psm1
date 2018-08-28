function Get-TerminologyConfig {
    param(
        [PSCredential] $Credentials,
        [String] $DiscoveryServiceUrl,
        [String] $SqlAddress,
        [String] $SharedDbName,
        [String] $AppInsightsKey
    )

    # Get Credentials
    If ($Null -ne $Credentials) {
        $iisUserCredentials = $Credentials
    }
    else {
        $iisUser = Read-Host "Please enter the IIS App Pool User"
        $userEnteredPassword = Read-Host "Enter the password for $iisUser" -AsSecureString
        $credential = New-Object -TypeName "System.Management.Automation.PSCredential" -ArgumentList $iisUser, $userEnteredPassword
        
        [System.Reflection.Assembly]::LoadWithPartialName("System.DirectoryServices.AccountManagement") | Out-Null
        $ct = [System.DirectoryServices.AccountManagement.ContextType]::Domain
        try {
            $pc = New-Object System.DirectoryServices.AccountManagement.PrincipalContext -ArgumentList $ct, $credential.GetNetworkCredential().Domain
        }
        catch [System.Management.Automation.MethodInvocationException] {
            Write-DosMessage -Level "Error"  -Message "Failed to connect to active directory: $_.Exception.Message" -ErrorAction Stop
        }

        $isValid = $pc.ValidateCredentials($credential.GetNetworkCredential().UserName, $credential.GetNetworkCredential().Password)
        if (!$isValid) {
            Write-DosMessage -Level "Error" -Message "Incorrect credentials for $iisUser"  -ErrorAction Stop
        }

        $iisUserCredentials = New-Object System.Management.Automation.PSCredential ($iisUser, $userEnteredPassword)
    }

    # Get Configuration
    $installSettings = Get-InstallationSettings "terminology"

    # Discovery Service Url
    if (-not ([string]::IsNullOrWhiteSpace($DiscoveryServiceUrl))) {
        $discoveryServiceUrlConfig = $DiscoveryServiceUrl
    }
    elseif (-not ([string]::IsNullOrWhiteSpace($installSettings.discoveryServiceUrl))) {
        $discoveryServiceUrlConfig = $installSettings.discoveryServiceUrl
    }
    else {
        $discoveryServiceUrlConfig = Read-Host "Please enter the discovery service Uri (eg. https://SERVER/discoveryservice/v1)"
    }

    # App insights key
    
    # Shared db connection
    if (-not ([string]::IsNullOrWhiteSpace($SharedDbName))) {
        $sharedDbNameConfig = $SharedDbName
    }
    elseif (-not ([string]::IsNullOrWhiteSpace($installSettings.sqlServerAddress))) {
        $sharedDbNameConfig = $installSettings.sqlServerAddress
    }
    else {
        $sharedDbNameConfig = Read-Host "Please enter the discovery service Uri (eg. https://SERVER/discoveryservice/v1)"
    }

    # Setup config
    $config = [PSCustomObject]@{
        iisUserCredentials = $iisUserCredentials
        appName = $installSettings.appName
        appPool = $installSettings.appPool
        siteName = $installSettings.siteName
        discoveryServiceUrl = $discoveryServiceUrlConfig
        sqlAddress = $SqlAddress
        sharedDbName = $sharedDbNameConfig
        appInsightsKey = $AppInsightsKey
    };

    return $config
}

function Update-AppSettings {
    param(
        [PSCustomObject] $config
    )

    $appSettings = "$(Get-IISWebSitePath -WebSiteName $config.siteName)\$($config.appName)\appsettings.json"
    $appSettingsJson = (Get-Content $appSettings -Raw) | ConvertFrom-Json 
    $appSettingsJson.BaseTerminologyEndpoint = $config.applicationEndpoint
    $appSettingsJson.TerminologySqlSettings.ConnectionString = "Data Source=$($config.sqlServerAddress);Initial Catalog=$($config.sharedDbName); Trusted_Connection=True;"
    $appSettingsJson.IdentityServerSettings.ClientSecret = $config.appName
    $appSettingsJson.DiscoveryServiceClientSettings.DiscoveryServiceUrl = $config.discoveryServiceUrl
    $appSettingsJson.ApplicationInsightsSettings.InstrumentationKey = $config.appInsightsKey
    $appSettingsJson.ApplicationInsightsSettings.Enabled = $FALSE

    $appSettingsJson | ConvertTo-Json -Depth 10 | Set-Content $appSettings
}

function Update-DiscoveryService() {
    param(
        [PSCustomObject] $config
    )

    Import-Module WebAdministration

    $webroot = Get-WebFilePath -PSPath "IIS:\Sites\$($config.siteName)\$($config.appName)"
    $terminologyAssembly = [System.IO.Path]::Combine($webroot, "Fabric.Terminology.API.dll")
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($terminologyAssembly).FileMajorPart

    $discoveryPostBody = @{
        serviceName    = "TerminologyService"
        serviceVersion = $version
        friendlyName   = "Fabric.Terminology"
        description    = "The Fabric.Terminology Service provides shared healthcare terminology data."
        serviceUrl     = "http://localhost/TerminologyService/v$version"# TODO: replace with application endpoint
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