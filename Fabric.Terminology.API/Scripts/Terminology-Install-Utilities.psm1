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
        [bool] $Required = $true,
        [switch] $Silent
    )

    if (-not ([string]::IsNullOrWhiteSpace($DefaultFromParam))) {
        return $DefaultFromParam
    }
    elseif (-not ([string]::IsNullOrWhiteSpace($DefaultFromInstallConfig))) {
        if ($Silent -eq $true) {
            return $DefaultFromInstallConfig;
        }
        return Get-UserValueOrDefault -Default $DefaultFromInstallConfig -Prompt $Prompt
    }
    else {
        if ($Silent -eq $true) {
            if ($Required -eq $true) {
                Write-DosMessage -Level "Error"  -Message "$Prompt is required and was not provided through the command parameter nor the install.config." -ErrorAction Stop
            }
        }
        elseif ($Required -eq $true) {
            while ([string]::IsNullOrWhiteSpace($result)) {
                $result = Read-Host "Enter the $Prompt $AdditionalPromptInfo".Trim()
            }
        }
        else {
            $result = Read-Host "Enter the $Prompt $AdditionalPromptInfo".Trim()
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
        [String] $AppName,
        [String] $SqlDataDirectory,
        [String] $SqlLogDirectory,
        [switch] $Silent
    )

    # Get Configuration
    $installSettings = Get-InstallationSettings "terminology"

    # Get Credentials
    if ($Null -ne $Credentials) {
        $iisUserCredentials = $Credentials
    }
    else {
        $iisUserConfig = Get-ConfigValue -Prompt "IIS user name to run the app pool" -DefaultFromParam $IisUserName -DefaultFromInstallConfig $installSettings.iisUser -Silent:$Silent
        
        $passwordPrompt = "App pool user password"

        if (-not ([string]::IsNullOrWhiteSpace($installSettings.iisUserPwd))) {
            if ($Silent -eq $true) {
                $iisUserPasswordConfig = ConvertTo-SecureString -String $installSettings.iisUserPwd -AsPlainText -Force
            }
            else {
                $iisUserPasswordConfig = Read-Host "$passwordPrompt or hit enter to accept the stored password" -AsSecureString

                if ($iisUserPasswordConfig.Length -eq 0) {
                    $iisUserPasswordConfig = ConvertTo-SecureString -String $installSettings.iisUserPwd -AsPlainText -Force
                }
            }
        }
        else {
            if ($Silent -eq $true) {
                Write-DosMessage -Level "Error"  -Message "$passwordPrompt is required and was not provided through the command parameter nor the install.config." -ErrorAction Stop
            }
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
    $appNameConfig = Get-ConfigValue -Prompt "service name" -DefaultFromParam $AppName -DefaultFromInstallConfig $installSettings.appName -Silent:$Silent

    # Discovery Service Url
    $discoveryServiceUrlConfig = Get-ConfigValue -Prompt "Discovery Service URI" -AdditionalPromptInfo "(eg. https://SERVER/DiscoveryService/v1)" -DefaultFromParam $DiscoveryServiceUrl -DefaultFromInstallConfig $installSettings.discoveryServiceUrl -Silent:$Silent

    # SQL Server Address
    $sqlAddressConfig = Get-ConfigValue -Prompt "address for SQL Server" -AdditionalPromptInfo "(eg. SERVER.DOMAIN.local)" -DefaultFromParam $SqlAddress -DefaultFromInstallConfig $installSettings.sqlServerAddress -Silent:$Silent

    # App Insights Key
    $appInsightsKeyConfig = Get-ConfigValue -Prompt "Application Insights key" -AdditionalPromptInfo "(optional)" -DefaultFromParam $AppInsightsKey -DefaultFromInstallConfig $installSettings.appInsightsKey -Required $false -Silent:$Silent

    # Metadata DB Name
    $metadataDbNameConfig = Get-ConfigValue -Prompt "metadata database name" -DefaultFromParam $MetadataDbName -DefaultFromInstallConfig $installSettings.metadataDbName -Silent:$Silent

    # Data Directory
    if ([string]::IsNullOrWhiteSpace($installSettings.defaultSqlDataDirectory)) {
        $dbDefaults = Get-DbaDefaultPath -SqlInstance "localhost"
        $sqlDataDirectoryParam = $dbDefaults.Data
    } else {
        $sqlDataDirectoryParam = $installSettings.defaultSqlDataDirectory
    }
    $sqlDataDirectoryConfig = Get-ConfigValue -Prompt "Data directory to create Terminology database" -DefaultFromParam $SqlDataDirectory -DefaultFromInstallConfig $sqlDataDirectoryParam -Silent:$Silent

    # Log Directory
    if ([string]::IsNullOrWhiteSpace($installSettings.defaultSqlLogDirectory)) {
        $dbDefaults = Get-DbaDefaultPath -SqlInstance "localhost"
        $sqlLogDirectoryParam = $dbDefaults.Log
    } else {
        $sqlLogDirectoryParam = $installSettings.defaultSqlLogDirectory
    }
    $sqlLogDirectoryConfig = Get-ConfigValue -Prompt "Log directory to create Terminology database" -DefaultFromParam $SqlLogDirectory -DefaultFromInstallConfig $sqlLogDirectoryParam -Silent:$Silent

    Add-InstallationSetting "terminology" "appName" "$appNameConfig" | Out-Null
    Add-InstallationSetting "terminology" "discoveryServiceUrl" "$discoveryServiceUrlConfig" | Out-Null
    Add-InstallationSetting "terminology" "sqlServerAddress" "$sqlAddressConfig" | Out-Null
    Add-InstallationSetting "terminology" "appInsightsInstrumentationKey" "$appInsightsKeyConfig" | Out-Null
    Add-InstallationSetting "common" "metadataDbName" "$metadataDbNameConfig" | Out-Null
    Add-InstallationSetting "common" "sqlDataDirectory" "$sqlDataDirectoryConfig" | Out-Null
    Add-InstallationSetting "common" "sqlLogDirectory" "$sqlLogDirectoryConfig" | Out-Null

    if ([string]::IsNullOrWhiteSpace($installSettings.appPool)) {
        Write-DosMessage -Level "Error"  -Message "App Pool is required and was not provided through the install.config." -ErrorAction Stop
    }

    if ([string]::IsNullOrWhiteSpace($installSettings.siteName)) {
        Write-DosMessage -Level "Error"  -Message "Site Name is required and was not provided through the install.config." -ErrorAction Stop
    }
    
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
        sqlDataDirectory    = $sqlDataDirectoryConfig
        sqlLogDirectory     = $sqlLogDirectoryConfig
    };

    return $config
}

function Update-AppSettings {
    param(
        [PSCustomObject] $Config
    )

    $appSettings = "$(Get-IISWebSitePath -WebSiteName $Config.siteName)\$($Config.appName)\appsettings.json"
    $appSettingsJson = (Get-Content $appSettings -Raw) | ConvertFrom-Json 
    $appSettingsJson.BaseTerminologyEndpoint = $Config.applicationEndpoint
    $appSettingsJson.TerminologySqlSettings.ConnectionString = "Data Source=$($Config.sqlServerAddress);Initial Catalog=$($Config.sharedDbName); Trusted_Connection=True;"
    $appSettingsJson.IdentityServerSettings.ClientSecret = $Config.appName
    $appSettingsJson.DiscoveryServiceClientSettings.DiscoveryServiceUrl = $Config.discoveryServiceUrl
    $appSettingsJson.ApplicationInsightsSettings.InstrumentationKey = $Config.appInsightsKey
    $appSettingsJson.ApplicationInsightsSettings.Enabled = $FALSE

    $appSettingsJson | ConvertTo-Json -Depth 10 | Set-Content $appSettings
}

function Update-DiscoveryService() {
    param(
        [PSCustomObject] $Config
    )

    $webroot = Get-WebFilePath -PSPath "IIS:\Sites\$($Config.siteName)\$($Config.appName)"
    $terminologyAssembly = [System.IO.Path]::Combine($webroot, "Fabric.Terminology.API.dll")
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($terminologyAssembly).FileMajorPart

    $discoveryPostBody = @{
        serviceName    = "TerminologyService"
        serviceVersion = $version
        friendlyName   = "Fabric.Terminology"
        description    = "The Fabric.Terminology Service provides shared healthcare terminology data."
        serviceUrl     = "http://localhost/TerminologyService/v$version"# TODO: replace with application endpoint
    }
    Add-DiscoveryRegistration $Config.discoveryServiceUrl $Config.iisUserCredentials $discoveryPostBody
}

function Publish-TerminologyDacpac() {
    param(
        [PSCustomObject] $Config,
        [string] $Dacpac,
        [String] $PublishProfile
    )

    Write-DosMessage -Level "Information" -Message "Updating data and log mount points in: $publishProfileXml"
    [System.Xml.XmlDocument]$publishProfileXml = new-object System.Xml.XmlDocument
    $publishProfileXml.load($PublishProfile)
    [System.Xml.XmlNamespaceManager]$nsmgr = new-object System.Xml.XmlNamespaceManager $publishProfileXml.NameTable
    $nsmgr.AddNamespace("msft", "http://schemas.microsoft.com/developer/msbuild/2003")
    $data = $publishProfileXml.SelectSingleNode("/msft:Project/msft:ItemGroup/msft:SqlCmdVariable[@Include='DataMountPoint']/msft:Value", $nsmgr);
    $data.InnerText = $Config.sqlDataDirectory
    $log = $publishProfileXml.SelectSingleNode("/msft:Project/msft:ItemGroup/msft:SqlCmdVariable[@Include='LogMountPoint']/msft:Value", $nsmgr);
    $log.InnerText = $Config.sqlDataDirectory
    $publishProfileXml.Save($PublishProfile);
    
    Write-DosMessage -Level "Information" -Message "Creating or updating Terminology database on $($Config.sqlAddress)"
    Publish-DosDacPac -TargetSqlInstance $Config.sqlAddress -DacPacFilePath $Dacpac -TargetDb "Terminology" -PublishOptionsFilePath $PublishProfile
}

function Publish-TerminologyDatabaseRole() {
    param(
        [PSCustomObject] $Config,
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
    Invoke-SqlCommand -SqlServerAddress $Config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters


    Write-DosMessage -Level "Information" -Message "Creating login for $($Config.iisUserCredentials.UserName) on $($Config.sqlAddress)" 
    $Query = "DECLARE @cmd nvarchar(max)
    If Not exists (SELECT * FROM sys.server_principals
        WHERE sid = suser_sid(@User))
    BEGIN
        print '-- Creating login '
        SET @cmd = N'CREATE LOGIN ' + quotename(@User, ']') + N' FROM WINDOWS'
        EXEC(@cmd)
    END
    ";
    $Parameters = @{User=$($Config.iisUserCredentials.UserName)}
    Invoke-SqlCommand -SqlServerAddress $Config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters


    Write-DosMessage -Level "Information" -Message "Adding $($Config.iisUserCredentials.UserName) to $RoleName on $($Config.sqlAddress)"
    $Query = "DECLARE @cmd nvarchar(max)
    IF IS_ROLEMEMBER (@RoleName, @User) <> 1
    BEGIN
        print '-- Adding user to role '
        SET @cmd = N'ALTER ROLE ' + quotename(@RoleName, ']') + N' ADD MEMBER ' + quotename(@User, ']')
        EXEC(@cmd)
    END";
    $Parameters = @{User=$($Config.iisUserCredentials.UserName);RoleName=$($RoleName)}
    Invoke-SqlCommand -SqlServerAddress $Config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters
}

function Publish-TerminologyDatabaseUpdates() {
    param(
        [PSCustomObject] $Config,
        [string] $Dacpac,
        [String] $PublishProfile
    )
    
    Publish-TerminologyDacpac -Config $Config -Dacpac $Dacpac -PublishProfile $PublishProfile
    Publish-TerminologyDatabaseRole -Config $Config -DatabaseName "Terminology" -RoleName = "TerminologyServiceRole";
    Publish-TerminologyDatabaseRole -Config $Config -DatabaseName "Shared" -RoleName = "TerminologySharedServiceRole";
    
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