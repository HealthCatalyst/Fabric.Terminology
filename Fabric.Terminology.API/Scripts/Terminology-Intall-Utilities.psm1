function Get-UserValueOrDefault {
    param(
        [String] $Default,
        [String] $Prompt
    )

    $value = Read-Host "$Prompt or hit enter to accept the default value [$Default]"

    if ([string]::IsNullOrWhiteSpace($value)) {
        return $Default
    }
    else {
        return $value
    }
}

function Get-ConfigValue {
    param(
        [String] $Prompt = $(throw "Please specify a prompt message"),
        [String] $AdditionalPromptInfo,
        [String] $DefaultFromInstallConfig = $(throw "Please specify a default value"),
        [String] $DefaultFromParam,
        [bool] $Required = $true
    )

    if (-not ([string]::IsNullOrWhiteSpace($DefaultFromParam))) {
        return $DefaultFromParam
    }
    elseif (-not ([string]::IsNullOrWhiteSpace($DefaultFromInstallConfig))) {
        return Get-UserValueOrDefault -Default $DefaultFromInstallConfig -Prompt $Prompt
    }
    else {
        if ($Required -eq $true) {
            while ([string]::IsNullOrWhiteSpace($result)) {
                $result = Read-Host "$Prompt $AdditionalPromptInfo".Trim()
            }
        }
        else {
            $result = Read-Host "$Prompt $AdditionalPromptInfo".Trim()
        }

        return $result
    }
}

function Invoke-SqlCommand{
    param(
        [String] $SqlServerAddress,
        [String] $DatabaseName,
        [String] $Query,
        [PSCustomObject] $Parameters= @{}
    )

    $connectionString = "Data Source=$SqlServerAddress;Initial Catalog=$($DatabaseName); Trusted_Connection=True;"
    $connection = New-Object System.Data.SqlClient.SQLConnection($connectionString)
    $command = New-Object System.Data.SqlClient.SqlCommand($Query, $connection)
    
    try {
        foreach($p in $Parameters.Keys){		
          $command.Parameters.AddWithValue("@$p",$Parameters[$p]) | Out-Null
         }

        $connection.Open() 
        $command.ExecuteNonQuery() | Out-Null
        $connection.Close()        
    }catch [System.Data.SqlClient.SqlException] {
        Write-DosMessage -Level "Error" -Message "An error ocurred while executing the command. 
        Connection String: $($connectionString)"
        throw
    }
}

function Get-TerminologyConfig {
    param(
        [PSCredential] $Credentials,
        [String] $DiscoveryServiceUrl,
        [String] $SqlAddress,
        [String] $MetadataDbName,
        [String] $AppInsightsKey,
        [String] $IisUserName,
        [SecureString] $IisUserPassword,
        [String] $AppName
    )

    # Get Configuration
    $installSettings = Get-InstallationSettings "terminology"
    Write-Host $installSettings

    # Get Credentials
    if ($Null -ne $Credentials) {
        $iisUserCredentials = $Credentials
    }
    else {
        $iisUserConfig = Get-ConfigValue -Prompt "Enter the IIS user name to run the app pool" -DefaultFromParam $IisUserName -DefaultFromInstallConfig $installSettings.iisUser
        
        $passwordPrompt = "Enter the IIS user password"
        if (-not ([string]::IsNullOrWhiteSpace($installSettings.iisUserPwd))) {
            $iisUserPasswordConfig = Read-Host "$passwordPrompt or hit enter to accept the stored password" -AsSecureString

            if ($iisUserPasswordConfig.Length -eq 0) {
                $iisUserPasswordConfig = ConvertTo-SecureString -String $installSettings.iisUserPwd -AsPlainText -Force
            }
        }
        else {
            $iisUserPasswordConfig = Read-Host $passwordPrompt -AsSecureString
        }

        $credential = New-Object -TypeName "System.Management.Automation.PSCredential" -ArgumentList $iisUserConfig, $iisUserPasswordConfig
        
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
            Write-DosMessage -Level "Error" -Message "Incorrect credentials for $iisUserConfig"  -ErrorAction Stop
        }

        $iisUserCredentials = New-Object System.Management.Automation.PSCredential ($iisUserConfig, $iisUserPasswordConfig)
    
        Add-InstallationSetting "terminology" "iisUser" "$iisUserConfig" | Out-Null
    }
    
    # App Name
    $appNameConfig = Get-ConfigValue -Prompt "Enter the service name" -DefaultFromParam $AppName -DefaultFromInstallConfig $installSettings.appName

    # Discovery Service Url
    $discoveryServiceUrlConfig = Get-ConfigValue -Prompt "Enter the Discovery Service URI" -AdditionalPromptInfo "(eg. https://SERVER/DiscoveryService/v1)" -DefaultFromParam $DiscoveryServiceUrl -DefaultFromInstallConfig $installSettings.discoveryServiceUrl

    # SQL Server Address
    $sqlAddressConfig = Get-ConfigValue -Prompt "Enter the address for SQL Server" -AdditionalPromptInfo "(eg. SERVER.DOMAIN.local)" -DefaultFromParam $SqlAddress -DefaultFromInstallConfig $installSettings.sqlServerAddress

    # App Insights Key
    $appInsightsKeyConfig = Get-ConfigValue -Prompt "Enter an Application Insights key" -AdditionalPromptInfo "(optional)" -DefaultFromParam $AppInsightsKey -DefaultFromInstallConfig $installSettings.appInsightsKey -Required $false

    # Metadata DB Name
    $metadataDbNameConfig = Get-ConfigValue -Prompt "Enter the metadata database name" -DefaultFromParam $MetadataDbName -DefaultFromInstallConfig $installSettings.metadataDbName

    Add-InstallationSetting "terminology" "appName" "$appNameConfig" | Out-Null
    Add-InstallationSetting "terminology" "discoveryServiceUrl" "$discoveryServiceUrlConfig" | Out-Null
    Add-InstallationSetting "terminology" "sqlServerAddress" "$sqlAddressConfig" | Out-Null
    Add-InstallationSetting "terminology" "appInsightsInstrumentationKey" "$appInsightsKeyConfig" | Out-Null
    Add-InstallationSetting "common" "metadataDbName" "$metadataDbNameConfig" | Out-Null


    # Setup config
    $config = [PSCustomObject]@{
        iisUserCredentials  = $iisUserCredentials
        appName             = $appNameConfig
        appPool             = $installSettings.appPool
        siteName            = $installSettings.siteName
        discoveryServiceUrl = $discoveryServiceUrlConfig
        sqlAddress          = $sqlAddressConfig
        metadataDbName      = $metadataDbNameConfig
        appInsightsKey      = $appInsightsKeyConfig
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

function Publish-TerminologyDatabaseRole() {
    param(
        [PSCustomObject] $config,
        [String] $DatabaseName,
        [String] $RoleName
    )

    $Query = "DECLARE @cmd nvarchar(max)
    IF DATABASE_PRINCIPAL_ID(@RoleName) IS NULL
    BEGIN
        print '-- Creating role '
        SET @cmd = N'CREATE ROLE ' + quotename(@RoleName, ']')
        EXEC(@cmd)
    END";
    # TODO: Add tables and views here
    # GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[TABLENAME] TO TerminologyServiceRole;
    # GO
    $Parameters = @{RoleName=$($RoleName)}
    Invoke-SqlCommand -SqlServerAddress $config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters


    Write-DosMessage -Level "Information" -Message "Creating login for $($config.iisUserCredentials.UserName) on $($config.sqlAddress)" 
    $Query = "DECLARE @cmd nvarchar(max)
    If Not exists (SELECT * FROM sys.server_principals
        WHERE sid = suser_sid(@User))
    BEGIN
        print '-- Creating login '
        SET @cmd = N'CREATE LOGIN ' + quotename(@User, ']') + N' FROM WINDOWS'
        EXEC(@cmd)
    END
    ";
    $Parameters = @{User=$($config.iisUserCredentials.UserName)}
    Invoke-SqlCommand -SqlServerAddress $config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters


    Write-DosMessage -Level "Information" -Message "Adding $($config.iisUserCredentials.UserName) to $RoleName on $($config.sqlAddress)"
    $Query = "DECLARE @cmd nvarchar(max)
    IF IS_ROLEMEMBER (@RoleName, @User) <> 1
    BEGIN
        print '-- Adding user to role '
        SET @cmd = N'ALTER ROLE ' + quotename(@RoleName, ']') + N' ADD MEMBER ' + quotename(@User, ']')
        EXEC(@cmd)
    END";
    $Parameters = @{User=$($config.iisUserCredentials.UserName);RoleName=$($RoleName)}
    Invoke-SqlCommand -SqlServerAddress $config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters
}

function Publish-TerminologyDatabaseUpdates() {
    param(
        [PSCustomObject] $config,
        [string] $Dacpac,
        [String] $PublishProfile
    )
    Import-Module dbatools

    Write-DosMessage -Level "Information" -Message "Creating or updating Terminology database on $($config.sqlAddress)"
    Publish-DosDacPac -TargetSqlInstance $config.sqlAddress -DacPacFilePath $Dacpac -TargetDb "Terminology" -PublishOptionsFilePath $PublishProfile

    Publish-TerminologyDatabaseRole -config $config -DatabaseName "Terminology" -RoleName = "TerminologyServiceRole";
    Publish-TerminologyDatabaseRole -config $config -DatabaseName "Shared" -RoleName = "TerminologySharedServiceRole";
    
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