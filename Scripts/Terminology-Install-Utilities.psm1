function Get-UserValueOrDefault {
    param(
        [String] $Default,
        [String] $Prompt
    )

    Write-Host "Enter the " -NoNewline -ForegroundColor White
    Write-Host $Prompt  -NoNewline -ForegroundColor Yellow
    Write-Host " or hit enter to accept the default value [" -NoNewline -ForegroundColor White
    Write-Host $Default -NoNewline -ForegroundColor Cyan
    Write-Host "] " -NoNewline -ForegroundColor White
    $value = Read-Host

    if ([string]::IsNullOrWhiteSpace($value)) {
        return $Default
    }
    else {
        return $value
    }
}

function Get-ConfigValue {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Prompt,
        [String] $AdditionalPromptInfo,
        [String] $DefaultFromInstallConfig,
        [String] $DefaultFromParam,
        [bool] $Required = $true,
        [switch] $Quiet
    )

    if (-not ([string]::IsNullOrWhiteSpace($DefaultFromParam))) {
        return $DefaultFromParam
    }
    elseif (-not ([string]::IsNullOrWhiteSpace($DefaultFromInstallConfig))) {
        if ($Quiet -eq $true) {
            return $DefaultFromInstallConfig;
        }
        return Get-UserValueOrDefault -Default $DefaultFromInstallConfig -Prompt $Prompt
    }
    else {
        if ($Quiet -eq $true) {
            if ($Required -eq $true) {
                Write-DosMessage -Level "Error"  -Message "$Prompt is required and was not provided through the command parameter nor the install.config." -ErrorAction Stop
            }
        }
        elseif ($Required -eq $true) {
            while ([string]::IsNullOrWhiteSpace($result)) {
                Write-Host "Enter the " -NoNewline -ForegroundColor White
                Write-Host $Prompt  -NoNewline -ForegroundColor Yellow
                Write-Host " $AdditionalPromptInfo " -NoNewline -ForegroundColor White
                $result = Read-Host
            }
        }
        else {
            Write-Host "Enter the " -NoNewline -ForegroundColor White
            Write-Host $Prompt  -NoNewline -ForegroundColor Yellow
            Write-Host " $AdditionalPromptInfo " -NoNewline -ForegroundColor White
            $result = Read-Host
        }

        return $result
    }
}

function Get-FullyQualifiedMachineName() {
    return "$env:computername.$((Get-WmiObject Win32_ComputerSystem).Domain.tolower())";
}

function Get-ServiceFromDiscovery {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $DiscoveryUrl,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Name,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Version
    )

    $discoveryServiceQuery = "/Services?`$filter=ServiceName eq '$Name' and Version eq $Version&`$select=ServiceUrl"

    $uri = "$DiscoveryUrl$discoveryServiceQuery"

    try {
        $response = Invoke-WebRequest -Uri $uri -Method GET -UseDefaultCredentials -UseBasicParsing
    }
    catch [System.Net.WebException] {
        throw "There was an error communicating with the Discovery Service.`n
        Request: $uri`n
        Status Code: $($_.Exception.Response.StatusCode.value__)`n
        Message: $($_.Exception.Response.StatusDescription)"
    }

    if (($response | ConvertFrom-Json).value.Length -eq 0) {
        throw "$Name (v$Version) could not be found with the Discovery Service. Please verify $Name has been installed and registered then try again."
    }

    return ($response | ConvertFrom-Json).value.ServiceUrl
}

function Invoke-PingService {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $ServiceUrl,
        [String] $ServiceName
    )
    
    $uriWithoutVersion = $ServiceUrl.Substring(0, $ServiceUrl.LastIndexOf('/'))
    $uri = "$uriWithoutVersion/Ping"

    try {
        Write-DosMessage -Level "Information" -Message "Attempting to ping $ServiceName ($uri)"  
        Invoke-WebRequest -Uri $uri -Method GET -UseDefaultCredentials -UseBasicParsing | Out-Null
    }
    catch [System.Net.WebException] {
        throw "There was an error communicating with the service.`n
        Request: $uri`n
        Status Code: $($_.Exception.Response.StatusCode.value__)`n
        Message: $($_.Exception.Response.StatusDescription)"
    }
}

function Invoke-SqlCommand {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $SqlServerAddress,
        [String] $DatabaseName,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Query,
        [PSCustomObject] $Parameters = @{},
        [Boolean] $ReturnData = $false
    )

    $connectionString = "Data Source=$SqlServerAddress;Initial Catalog=$DatabaseName; Trusted_Connection=True;"
    $connection = New-Object System.Data.SqlClient.SQLConnection($connectionString)
    $command = New-Object System.Data.SqlClient.SqlCommand($Query, $connection)
    
    try {
        foreach ($p in $Parameters.Keys) {		
            $command.Parameters.AddWithValue("@$p", $Parameters[$p]) | Out-Null
        }

        $connection.Open() 

        if ($ReturnData) {
            $adapter = New-Object System.Data.sqlclient.sqlDataAdapter $command
            $dataset = New-Object System.Data.DataSet
            $adapter.Fill($dataSet) | Out-Null
            $dataSet.Tables
        }
        else {
            $command.ExecuteNonQuery() | Out-Null
        }

        $connection.Close()        
    }
    catch [System.Data.SqlClient.SqlException] {
        Write-DosMessage -Level "Error" -Message "An error ocurred while executing the command. 
        Connection String: $($connectionString)
        Query: $Query" -ErrorAction Continue
        throw $_.Exception
    }
}

function Get-InstallerAccessToken {
    param(
        [ValidateNotNullOrEmpty()]
        [String] $discoveryServiceUrl,
        [ValidateNotNullOrEmpty()]
        [String] $fabricInstallerSecret
    )
    $decryptedInstallerSecret = Get-DecryptedString $encryptionCertificate $fabricInstallerSecret
    $identityServiceUrl = Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrl -Name "IdentityService" -Version 1
    $fabricUser = "fabric-installer"
    $scopes = "dos/metadata dos/metadata.serviceAdmin fabric/authorization.read"
    Write-DosMessage -Level "Information" -Message "Retrieving access token from IdentityService: $identityServiceUrl as $fabricUser user and $scopes scopes"
    $accessToken = Get-AccessToken $identityServiceUrl $fabricUser $scopes $decryptedInstallerSecret
    return $accessToken
}

function Test-DatabaseExists {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $SqlAddress,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Name
    )
    $query = "IF DB_ID(@dbname) IS NULL
        BEGIN
        DECLARE @errorMessage NVARCHAR(256)
        SET @errorMessage = N'Could not confirm the ' + @dbname + ' database exists. Please verify the database was deployed then run this step again.'
        RAISERROR (@errorMessage, 11, 1)
        END"
    $parameters = @{dbname = "$Name"}
    try {
        Invoke-SqlCommand -SqlServerAddress $SqlAddress -Query $query -Parameters $parameters
    }
    catch [System.Data.SqlClient.SqlException] {
        Write-DosMessage -Level "Error" -Message $_.Exception.Message -ErrorAction Continue
        throw $_.Exception
    }
}

function Test-ViewExists {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $SqlAddress,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Database,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $Schema,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $View
    )
    $query = "IF EXISTS (SELECT 1 FROM sys.views WHERE object_id = OBJECT_ID(N'[$Schema].[$View]') )
    BEGIN
        SELECT 1 as 'EXISTS'
    END
    ELSE
    BEGIN
        SELECT 0 as 'EXISTS'
    END"
    $result = Invoke-SqlCommand -SqlServerAddress $SqlAddress -Query $query -Parameters $parameters -DatabaseName $Database -ReturnData $true
    $exists = $result.table.EXISTS -eq 1
    return $exists
}

function Get-TerminologyConfig {
    param(
        [PSCredential] $Credentials,
        [String] $DiscoveryServiceUrl,
        [String] $SqlAddress,
        [String] $EdwAddress,
        [String] $MetadataDbName,
        [String] $AppInsightsKey,
        [String] $IisUserName,
        [SecureString] $IisUserPassword,
        [String] $AppName,
        [String] $AppEndpoint,
        [String] $SqlDataDirectory,
        [String] $SqlLogDirectory,
        [String] $LoaderWindowsUserParam,
        [String] $ProcessingServiceWindowsUserParam,
        [switch] $Quiet
    )

    # Get Configuration
    $installSettings = Get-InstallationSettings "terminology"

    # Discovery Service Url
    if ([string]::IsNullOrWhiteSpace($installSettings.discoveryService)) {
        $discoveryServiceParam = "https://$(Get-FullyQualifiedMachineName)/DiscoveryService/v1"
    }
    else {
        $discoveryServiceParam = $installSettings.discoveryService
    }
    $discoveryServiceUrlConfig = Get-ConfigValue -Prompt "Discovery Service URI" -DefaultFromParam $DiscoveryServiceUrl -DefaultFromInstallConfig $discoveryServiceParam -Quiet:$Quiet

    if ([string]::IsNullOrWhiteSpace($installSettings.fabricInstallerSecret)) {
        Write-DosMessage -Level "Error"  -Message "Fabric Installer Secret and was not provided through the install.config. Please install fabric Identity and Authorization and retry running installation" -ErrorAction Stop
    }

    if ([string]::IsNullOrWhiteSpace($installSettings.encryptionCertificateThumbprint)) {
        Write-DosMessage -Level "Error"  -Message "Encryption certificate thumbprint is required and was not provided through the install.config. Please install fabric Identity and Authorization and retry running installation" -ErrorAction Stop
    }

    # Get Installer Access Token
    try {
        $accessToken = Get-InstallerAccessToken -DiscoveryServiceUrl $discoveryServiceUrlConfig -fabricInstallerSecret $installSettings.fabricInstallerSecret
        if ([string]::IsNullOrWhiteSpace($accessToken)) {
            throw "Access token is null or empty"
        }
    } 
    catch {
        Write-DosMessage -Level "Error" -Message "An error has occured while retrieving access token. Please verify Fabric Identity/Authentication are installed and setup correctly " -ErrorAction Continue
        throw $_.Exception
    }

    # Validate Service Dependencies
    $serviceName = "MetadataService";
    $serviceVersion = 2;
    Write-DosMessage -Level "Information" -Message "Verifying $serviceName (v$serviceVersion) is installed and registered"
    $mdsServiceUrl = Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrlConfig -Name $serviceName -Version $serviceVersion

    $serviceName = "DataProcessingService";
    $serviceVersion = 1;
    Write-DosMessage -Level "Information" -Message "Verifying $serviceName (v$serviceVersion) is installed and registered"
    $dpsServiceUrl = Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrlConfig -Name $serviceName -Version $serviceVersion

    $serviceName = "IdentityService";
    $serviceVersion = 1;
    Write-DosMessage -Level "Information" -Message "Verifying $serviceName (v$serviceVersion) is installed and registered"
    $identityServiceUrl = Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrlConfig -Name $serviceName -Version $serviceVersion

    $serviceName = "AuthorizationService";
    $serviceVersion = 1;
    Write-DosMessage -Level "Information" -Message "Verifying $serviceName (v$serviceVersion) is installed and registered"
    $authorizationServiceUrl = Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrlConfig -Name $serviceName -Version $serviceVersion
    

    # Get IIS User Credentials
    if ($Null -ne $Credentials) {
        $iisUserCredentials = $Credentials
    }
    else {
        $iisUserConfig = Get-ConfigValue -Prompt "IIS user name to run the app pool" -AdditionalPromptInfo "(ex. DOMAIN\USER)" -DefaultFromParam $IisUserName -DefaultFromInstallConfig $installSettings.iisUser -Quiet:$Quiet
        
        $passwordPrompt = "App pool user password for $iisUserConfig"

        if (-not ([string]::IsNullOrWhiteSpace($installSettings.iisUserPwd))) {
            if ($Quiet -eq $true) {
                $iisUserPasswordConfig = ConvertTo-SecureString -String $installSettings.iisUserPwd -AsPlainText -Force
            }
            else {
                $iisUserPasswordConfig = Read-Host "Enter the $passwordPrompt or hit enter to accept the stored password" -AsSecureString

                if ($iisUserPasswordConfig.Length -eq 0) {
                    $iisUserPasswordConfig = ConvertTo-SecureString -String $installSettings.iisUserPwd -AsPlainText -Force
                }
            }
        }
        else {
            if ($Quiet -eq $true) {
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
            Write-DosMessage -Level "Error"  -Message "Failed to connect to active directory: $_.Exception.Message" -ErrorAction Continue
            throw $_.Exception
        }

        $isValid = $pc.ValidateCredentials($credential.GetNetworkCredential().UserName, $credential.GetNetworkCredential().Password)
        if (!$isValid) {
            Write-DosMessage -Level "Error" -Message "Incorrect credentials for $iisUserConfig"  -ErrorAction Stop
        }

        $iisUserCredentials = New-Object System.Management.Automation.PSCredential ($iisUserConfig, $iisUserPasswordConfig)
    
        Add-InstallationSetting "terminology" "iisUser" "$iisUserConfig" | Out-Null
    }
    
    # App Name
    $appNameConfig = Get-ConfigValue -Prompt "Service Name" -DefaultFromParam $AppName -DefaultFromInstallConfig $installSettings.appName -Quiet:$Quiet

    # Terminology Service Endpoint
    if ([string]::IsNullOrWhiteSpace($installSettings.appEndpoint)) {
        $terminologyEndpointParam = "https://$(Get-FullyQualifiedMachineName)/TerminologyService"
    }
    else {
        $terminologyEndpointParam = $installSettings.appEndpoint
    }
    $terminologyEndpointConfig = Get-ConfigValue -Prompt "$appNameConfig endpoint" -DefaultFromParam $AppEnpoint -DefaultFromInstallConfig $terminologyEndpointParam  -Quiet:$Quiet

    # ETL Server Address
    if ([string]::IsNullOrWhiteSpace($installSettings.sqlServerAddress)) {
        $sqlAddressConfigParam = "$(Get-FullyQualifiedMachineName)"
    }
    else {
        $sqlAddressConfigParam = $installSettings.sqlServerAddress
    }
    $sqlAddressConfig = Get-ConfigValue -Prompt "address for ETL Server (server that hosts EDWAdmin database)" -AdditionalPromptInfo "(ex. SERVERNAME)" -DefaultFromParam $SqlAddress -DefaultFromInstallConfig $sqlAddressConfigParam -Quiet:$Quiet

    # Metadata DB Name
    if ([string]::IsNullOrWhiteSpace($installSettings.metadataDbName)) {
        $metadataDbNameParam = "EDWAdmin"
    }
    else {
        $metadataDbNameParam = $installSettings.metadataDbName
    }
    $metadataDbNameConfig = Get-ConfigValue -Prompt "metadata database name" -DefaultFromParam $MetadataDbName -DefaultFromInstallConfig $metadataDbNameParam -Quiet:$Quiet

    # Verify EDWAdmin exists
    Write-DosMessage -Level "Information" -Message "Verifying metadata database ($metadataDbNameConfig) exists"
    Test-DatabaseExists -SqlAddress $sqlAddressConfig -Name $metadataDbNameConfig

    # EDW Server Address
    if ([string]::IsNullOrWhiteSpace($installSettings.edwAddress)) {
        # Default to the ETL Server as many times they are the same
        $edwAddressConfigParam = $sqlAddressConfig
    }
    else {
        $query = "SELECT TOP 1 [AttributeValueTXT] FROM [$metadataDbNameConfig].[CatalystAdmin].[ObjectAttributeBASE] WHERE AttributeNM = 'EDWServerAndPort'"
        $objAttributesResults = Invoke-SqlCommand -SqlServerAddress $sqlAddressConfig -Query $query -DatabaseName $metadataDbNameConfig -ReturnData $true
        if ([string]::IsNullOrWhiteSpace($objAttributesResults.table.AttributeValueTXT)) {
            $edwAddressConfigParam = $installSettings.edwAddress
        }
        else {
            $edwAddressConfigParam = $objAttributesResults.table.AttributeValueTXT
        }
    }
    $edwAddressConfig = Get-ConfigValue -Prompt "address for EDW Server (server that hosts Shared/SAM database)" -AdditionalPromptInfo "(ex. SERVERNAME)" -DefaultFromParam $EdwAddress -DefaultFromInstallConfig $edwAddressConfigParam -Quiet:$Quiet

    # App Insights Key
    $appInsightsKeyConfig = Get-ConfigValue -Prompt "Application Insights key" -AdditionalPromptInfo "(optional)" -DefaultFromParam $AppInsightsKey -DefaultFromInstallConfig $installSettings.appInsightsKey -Required $false -Quiet:$Quiet    

    # Data Directory
    if ([string]::IsNullOrWhiteSpace($installSettings.defaultSqlDataDirectory)) {
        $dbDefaults = Get-DbaDefaultPath -SqlInstance $edwAddressConfig
        $sqlDataDirectoryParam = $dbDefaults.Data
    }
    else {
        $sqlDataDirectoryParam = $installSettings.defaultSqlDataDirectory
    }
    $sqlDataDirectoryConfig = Get-ConfigValue -Prompt "Data directory to create Terminology database" -DefaultFromParam $SqlDataDirectory -DefaultFromInstallConfig $sqlDataDirectoryParam -Quiet:$Quiet

    # Log Directory
    if ([string]::IsNullOrWhiteSpace($installSettings.defaultSqlLogDirectory)) {
        $dbDefaults = Get-DbaDefaultPath -SqlInstance $edwAddressConfig
        $sqlLogDirectoryParam = $dbDefaults.Log
    }
    else {
        $sqlLogDirectoryParam = $installSettings.defaultSqlLogDirectory
    }
    $sqlLogDirectoryConfig = Get-ConfigValue -Prompt "Log directory to create Terminology database" -DefaultFromParam $SqlLogDirectory -DefaultFromInstallConfig $sqlLogDirectoryParam -Quiet:$Quiet

    # Loader User
    $loaderWindowsUser = Get-ConfigValue -Prompt "Loader Windows username" -AdditionalPromptInfo "(ex. DOMAIN\USER)" -DefaultFromParam $LoaderWindowsUserParam -DefaultFromInstallConfig $installSettings.loaderWindowsUser -Quiet:$Quiet

    # DPS User
    $ProcessingServiceWindowsUser = Get-ConfigValue -Prompt "Data Processing Service App pool username" -AdditionalPromptInfo "(ex. DOMAIN\USER)" -DefaultFromParam $ProcessingServiceWindowsUserParam -DefaultFromInstallConfig $installSettings.processingServiceWindowsUser -Quiet:$Quiet

    Add-InstallationSetting "terminology" "appName" "$appNameConfig" | Out-Null
    Add-InstallationSetting "terminology" "appEndpoint" "$terminologyEndpointConfig" | Out-Null

    if (![string]::IsNullOrWhiteSpace($appInsightsKeyConfig)) {
        Add-InstallationSetting "terminology" "appInsightsInstrumentationKey" "$appInsightsKeyConfig" | Out-Null
    }

    Add-InstallationSetting "common" "discoveryService" "$discoveryServiceUrlConfig" | Out-Null
    Add-InstallationSetting "common" "sqlServerAddress" "$sqlAddressConfig" | Out-Null
    Add-InstallationSetting "common" "edwAddress" "$edwAddressConfig" | Out-Null
    Add-InstallationSetting "common" "metadataDbName" "$metadataDbNameConfig" | Out-Null
    Add-InstallationSetting "common" "sqlDataDirectory" "$sqlDataDirectoryConfig" | Out-Null
    Add-InstallationSetting "common" "sqlLogDirectory" "$sqlLogDirectoryConfig" | Out-Null
    Add-InstallationSetting "common" "loaderWindowsUser" "$loaderWindowsUser" | Out-Null
    Add-InstallationSetting "common" "processingServiceWindowsUser" "$ProcessingServiceWindowsUser" | Out-Null

    if ([string]::IsNullOrWhiteSpace($installSettings.appPool)) {
        Write-DosMessage -Level "Error"  -Message "App Pool is required and was not provided through the install.config." -ErrorAction Stop
    }

    if ([string]::IsNullOrWhiteSpace($installSettings.siteName)) {
        Write-DosMessage -Level "Error"  -Message "Site Name is required and was not provided through the install.config." -ErrorAction Stop
    }

    Write-DosMessage -Level "Information" -Message "Verifying Shared database exists"
    Test-DatabaseExists -SqlAddress $edwAddressConfig -Name "Shared"
    
    # Setup config
    $config = [PSCustomObject]@{
        iisUserCredentials           = $iisUserCredentials
        appName                      = $appNameConfig
        appEndpoint                  = $terminologyEndpointConfig
        appPool                      = $installSettings.appPool
        siteName                     = $installSettings.siteName
        discoveryServiceUrl          = $discoveryServiceUrlConfig
        sqlAddress                   = $sqlAddressConfig
        edwAddress                   = $edwAddressConfig
        metadataDbName               = $metadataDbNameConfig
        appInsightsKey               = $appInsightsKeyConfig
        sqlDataDirectory             = $sqlDataDirectoryConfig
        sqlLogDirectory              = $sqlLogDirectoryConfig
        accessToken                  = $accessToken
        sharedDbName                 = $installSettings.sharedDbName
        mdsServiceUrl                = $mdsServiceUrl
        dpsServiceUrl                = $dpsServiceUrl
        identityServiceUrl           = $identityServiceUrl
        authorizationServiceUrl      = $authorizationServiceUrl
        loaderWindowsUser            = $loaderWindowsUser
        processingServiceWindowsUser = $processingServiceWindowsUser
    };

    return $config
}

function Update-AppSettings {
    param(
        [PSCustomObject] $Config
    )

    $appPath = "$(Get-IISWebSitePath -WebSiteName $Config.siteName)\$($Config.appName)"

    #Give read and write access iisUser for logging permissions
    $accessControl = Get-Acl $appPath
    $accessControlRule = New-Object System.Security.AccessControl.FileSystemAccessRule($Config.iisUserCredentials.UserName,"Read, Write","ContainerInherit, ObjectInherit","None",'Allow')
    $accessControl.SetAccessRule($accessControlRule)
    Set-Acl $appPath $accessControl

    $appSettings = "$appPath\appsettings.json"
    $appSettingsJson = (Get-Content $appSettings -Raw) | ConvertFrom-Json 
    $appSettingsJson.BaseTerminologyEndpoint = $Config.terminologyEndpointConfig
    $appSettingsJson.TerminologySqlSettings.ConnectionString = "Data Source=$($Config.edwAddress);Initial Catalog=$($Config.sharedDbName); Trusted_Connection=True;"
    $appSettingsJson.IdentityServerSettings.ClientSecret = $Config.appName
    $appSettingsJson.DiscoveryServiceClientSettings.DiscoveryServiceUrl = $Config.discoveryServiceUrl
    if ([string]::IsNullOrWhiteSpace($Config.appInsightsKeyConfig)) {
        $appSettingsJson.ApplicationInsightsSettings.Enabled = $false
    } 
    else {
        $appSettingsJson.ApplicationInsightsSettings.InstrumentationKey = $Config.appInsightsKeyConfig    
        $appSettingsJson.ApplicationInsightsSettings.Enabled = $true
    }

    $appSettingsJson | ConvertTo-Json -Depth 10 | Set-Content $appSettings
}

function Update-DiscoveryService() {
    param(
        [Parameter(Mandatory = $true)]
        [PSCustomObject] $Config
    )

    $webroot = Get-WebFilePath -PSPath "IIS:\Sites\$($Config.siteName)\$($Config.appName)"
    $terminologyAssembly = [System.IO.Path]::Combine($webroot, "Fabric.Terminology.API.dll")
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($terminologyAssembly)
    $url = "$($Config.appEndpoint)/v$($version.FileMajorPart)"

    $discoveryPostBody = @{
        serviceName    = "TerminologyService"
        serviceVersion = "$($version.FileMajorPart)"
        friendlyName   = "Fabric.Terminology"
        description    = "The Fabric.Terminology Service provides shared healthcare terminology data."
        serviceUrl     = $url
        buildVersion   = "$($version.FileVersion)"
        isHidden       = $true
        discoveryType  = "Service"
    }

    Write-DosMessage -Level "Information" -Message "Registering TerminologyService with discovery service. buildVersion: $($version.FileVersion) serviceUrl: $url"
    $connectionString = "Data Source=$($Config.sqlAddress);Initial Catalog=$($Config.metadataDbName); Trusted_Connection=True;"
    Add-DiscoveryRegistrationSql -discoveryPostBody $discoveryPostBody -connectionString $connectionString | Out-Null
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
    
    Write-DosMessage -Level "Information" -Message "Creating or updating Terminology database on $($Config.edwAddress). This may take a few minutes"
    # Publish-DosDacPac -TargetSqlInstance $Config.sqlAddress -DacPacFilePath $Dacpac -TargetDb "Terminology" -PublishOptionsFilePath $PublishProfile -ErrorAction Stop
    Publish-DbaDacpac -SqlInstance $Config.edwAddress -Database "Terminology" -Path $Dacpac -PublishXml $PublishProfile -EnableException
}

function Get-RoleId {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $Role,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $SqlAddress
    )

    $parameters = @{Role = $($Role)}
    $query = "SELECT TOP (1) [RoleID] FROM [$metadataDatabase].[CatalystAdmin].[RoleBASE] WHERE RoleNM = @Role"
    $roleQueryResult = Invoke-SqlCommand -SqlServerAddress $SqlAddress -Query $query -DatabaseName $metadataDatabase -Parameters $parameters -ReturnData $true
    $roleId = $roleQueryResult.Table.RoleID

    if ($roleId) {
        Write-DosMessage -Level "Information" -Message "RoleID for $Role`: $roleId"
        
    }
    else {
        Write-DosMessage -Level "Error" -Message "No RoleID found for role '$Role'"
        throw "No RoleID found for role '$Role'"
    }

    return $roleId
}


function Invoke-PostToMds {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $name,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $path,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSCustomObject] $Config
    )
    Write-DosMessage -Level "Information" "Reading file $path"
    $dataMartJson = Get-Content -Raw -Path $path
    $headers = @{"Content-Type" = "application/json"}
    $headers.Add("Authorization", "Bearer $($Config.accessToken)")

    try {
        Write-DosMessage -Level "Information" -Message "Posting json metadata in $path for $name in Metadata Service $($Config.mdsServiceUrl)/DataMarts.`nThis may take several minutes"
        $response = Invoke-RestMethod -Uri "$($Config.mdsServiceUrl)/DataMarts" -Method POST -Body $dataMartJson -Headers $headers -TimeoutSec 600 -UseBasicParsing
        Write-DosMessage -Level "Information" -Message "Completed creating metadata for $name with data mart $($response.Id)"
        return $($response.Id)
    }
    catch [System.Net.WebException] {
        $error = $_.ErrorDetails.Message | ConvertFrom-Json | Select-Object -Expand message
        Write-DosMessage -Level "Error" -Message "Error: $error" -ErrorAction Continue
        Write-DosMessage -Level "Error" -Message "Message: $($_.Exception.Message)" -ErrorAction Continue
        Write-DosMessage -Level "Error" -Message "Response: $($_.Exception.Response)" -ErrorAction Continue
        Write-DosMessage -Level "Error" -Message "InnerException: $($_.Exception.InnerException)" -ErrorAction Continue
        throw $_.Exception
    }
}

function Get-DataMartId {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $Name,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSCustomObject] $Config
    )

    $headers = @{"Content-Type" = "application/json"}
    $headers.Add("Authorization", "Bearer $($Config.accessToken)")
    $url = "$($Config.mdsServiceUrl)/DataMarts?`$filter=Name%20eq%20%27$Name%27&`$select=Id"
    Write-DosMessage -Level "Information" -Message "Testing if $Name Datamart exists: $url"
    $response = Invoke-RestMethod -Uri $url -Method GET -Headers $headers
    $exists = $response.value.length -ge 1
    if ($exists) {
        $id = $response.value[0].Id
        Write-DosMessage -Level "Information" -Message "Datamart $Name with id $id exists"
        return $id
    }
    else {
        Write-DosMessage -Level "Information" -Message "Datamart $Name does not exist"
        return $null
    }
}

function Invoke-PostToDps {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $name,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $dataMartId,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSCustomObject] $Config
    )

    
    $headers = @{"Content-Type" = "application/json"}
    try {
        $body = "{ `"DataMartId`": $dataMartId, `"BatchExecution`": { `"PipelineType`": `"Migration`", `"OverrideLoadType`": `"Incremental`", `"LoggingLevel`": `"Diagnostic`" } }"
        Write-DosMessage -Level "Information" -Message "Scheduling job for '$name' with the Data Processing Service to creating physical tables.`n$($Config.dpsServiceUrl)/ExecuteDataMart`n$body"
        $response = Invoke-RestMethod -Uri "$($Config.dpsServiceUrl)/ExecuteDataMart" -Method POST -UseDefaultCredentials -Headers $headers -Body $body -UseBasicParsing
        $batchExecutionId = ($response.value | ConvertFrom-Json).Id
        Write-DosMessage -Level "Information" -Message  "Batch execution successfully sent to the data processing service."

        return $batchExecutionId;
    }
    catch [System.Net.WebException] {
        Write-DosMessage -Level "Error" -Message "Message: $($_.Exception.Message)" -ErrorAction Continue
        Write-DosMessage -Level "Error" -Message "Response: $($_.Exception.Response)" -ErrorAction Continue
        Write-DosMessage -Level "Error" -Message "InnerException: $($_.Exception.InnerException)" -ErrorAction Continue
        throw $_.Exception
    }
}

function Invoke-PollBatchExecutions {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $executionId,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSCustomObject] $Config,
        [switch] $NoError
    )

    $terminologyUrl = "$($Config.dpsServiceUrl)/BatchExecutions($executionId)"
    $response = "";
    $message = "";
    $wasSuccessful = 0;

    foreach ($i in 1..30) {
        Start-Sleep -s 5
        if ($response -ne "Failed") {
            $response = Invoke-RestMethod -Uri $terminologyUrl -Method GET -UseDefaultCredentials
        }

        if ($response.Status -eq "Failed") {
            $message = "The batch execution has failed. Please check EDW Console for additional logging."
            break
        }
        if ($response.Status -eq "Cancelled") {
            $message = "The batch execution has been cancelled. Please check EDW Console for additional logging."
            break
        }
        if ($response.Status -eq "Succeeded") {
            $message = "The batch execution was successful."
            $wasSuccessful = 1;
            break
        }

        Write-DosMessage -Level "Information" -Message  "Status: $($response.Status)"
    }

    if ($wasSuccessful -eq $true) {
        Write-DosMessage -Level "Information" -Message $message
    }
    elseif ($NoError -eq $false) {
        Write-DosMessage -Level "Error" -Message $message -ErrorAction Stop
        throw $message
    }

    return $wasSuccessful;

}

function Add-EdwAdminRole() {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $RoleName,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSobject] $Config
    )
    $metadataDatabase = $Config.metadataDbName
    $sqlAddress = $Config.sqlAddress

    # get IdentityID from IdentityBASE
    $user = "$env:USERDOMAIN\$env:USERNAME"
    $parameters = @{user = $($user)}
    $identityBaseQuery = "SELECT TOP (1) [IdentityID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityBASE] WHERE UPPER(IdentityNM) = UPPER(@user)"
    $identityQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityBaseQuery -Parameters $parameters -ReturnData $true
    $identityId = $identityQueryResult.Table.IdentityID

    if ($identityId) {
        Write-DosMessage -Level "Information" -Message  "IdentityID for $user`: $identityId"
    }
    else {
        Write-DosMessage -Level "Information" -Message   "User is not in IdentityBASE... Adding user: $user"
        Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query "INSERT INTO [$metadataDatabase].[CatalystAdmin].[IdentityBASE] (IdentityNM) VALUES (UPPER(@user))" -Parameters $parameters -ReturnData $true
        $parameters = @{user = $($user)}
        $identityBaseQuery = "SELECT TOP (1) [IdentityID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityBASE] WHERE UPPER(IdentityNM) = UPPER(@user)"
        $identityQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityBaseQuery -Parameters $parameters -ReturnData $true
        $identityId = $identityQueryResult.Table.IdentityID
    }

    $roleId = Get-RoleId -Role $RoleName -SqlAddress $sqlAddress
    
    # check IdentityRoleBASE for DPS role
    $parameters = @{identityId = $identityId; roleId = $roleId}
    $identityRoleBaseQuery = "SELECT TOP (1) [IdentityRoleID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityRoleBASE] WHERE IdentityId = @identityId AND RoleID = @roleId"
    $identityRoleQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityRoleBaseQuery -Parameters $parameters -ReturnData $true
    $identityRoleId = $identityRoleQueryResult.Table.IdentityRoleID

    if ($identityRoleId) {
        Write-DosMessage -Level "Information" -Message "User $user already has the $RoleName role"
        return $false
    }

    Write-DosMessage -Level "Information" -Message "User $user does not have the $RoleName role"
    Write-DosMessage -Level "Information" -Message "Adding $RoleName role now"
    $parameters = @{identityId = $identityId; roleId = $roleId}
    $identityRoleBaseQuery = "INSERT INTO [$metadataDatabase].[CatalystAdmin].[IdentityRoleBASE] (IdentityID, RoleID) VALUES (@identityId, @roleId)"
    Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityRoleBaseQuery -Parameters $parameters -ReturnData $true
    Write-DosMessage -Level "Information" -Message "$RoleName role added for user $user"
    return $true
}

function Remove-EdwAdminRole() {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $RoleName,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSobject] $Config
    )

    $metadataDatabase = $Config.metadataDbName
    $sqlAddress = $Config.sqlAddress

    # get IdentityID from IdentityBASE
    $user = "$env:USERDOMAIN\$env:USERNAME"
    $parameters = @{user = $($user)}
    $identityBaseQuery = "SELECT TOP (1) [IdentityID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityBASE] WHERE UPPER(IdentityNM) = UPPER(@user)"
    $identityQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityBaseQuery -Parameters $parameters -ReturnData $true
    $identityId = $identityQueryResult.Table.IdentityID
 
    $roleId = Get-RoleId -Role $RoleName -SqlAddress $sqlAddress

    Write-DosMessage -Level "Information" -Message "Removing $RoleName role from user $user"
    $parameters = @{identityId = $identityId; roleId = $roleId};
    $identityRoleBaseQuery = "DELETE FROM [$metadataDatabase].[CatalystAdmin].[IdentityRoleBASE] WHERE IdentityId = @identityId AND RoleID = @roleId"
    Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityRoleBaseQuery -Parameters $parameters
    Write-DosMessage -Level "Information" -Message "Role removed"
}

function Add-MetadataAndStructures() {
    param(
        [Parameter(Mandatory = $true)]
        [PSobject] $Config
    )
    $roleAdded = Add-EdwAdminRole -RoleName "DataProcessingServiceUser" -Config $Config

    $terminologyDataMartId = Get-DataMartId -Name "Terminology" -Config $Config
    if ($null -eq $terminologyDataMartId) {
        # POST Terminology data marts to MDS
        $terminologyDataMartId = Invoke-PostToMds -Config $Config -name "Terminology" -path ".\Terminology.json"

        if (!$terminologyDataMartId) {
            Write-DosMessage -ErrorAction Stop -Level "Error" -Message "The Terminology metadata could not be created. Please resolve the error with POSTing the Terminology data mart to the metadata service"#
        }
    }
    else {
        Write-DosMessage -Level "Information" -Message "Terminology metadata already exists, skipping step"
    }
    
    $sharedTerminologyDataMartId = Get-DataMartId -Name "SharedTerminology" -Config $Config
    if ($null -eq $sharedTerminologyDataMartId) {
        # POST Terminology data marts to MDS
        $sharedTerminologyDataMartId = Invoke-PostToMds -Config $Config -name "SharedTerminology" -path ".\SharedTerminology.json"
        if (!$sharedTerminologyDataMartId) {
            Write-DosMessage -ErrorAction Stop -Level "Error" -Message "The SharedTerminology metadata could not be created. Please resolve the error with POSTing the SharedTerminology data mart to the metadata service"
        }
    }
    else {
        Write-DosMessage -Level "Information" -Message "Shared Terminology metadata already exists, skipping step"
    }

    Test-DatabaseExists -SqlAddress $Config.edwAddress -Name "Terminology"

    $viewExists = Test-ViewExists -SqlAddress $Config.edwAddress -Database "Terminology" -Schema "Open" -View "HHSHCCOverrides"
    if ($viewExists -eq $false) {
        # POST Terminology  
        $terminologyBatchExecutionId = Invoke-PostToDps -Name "Terminology" -Config $Config -dataMartId $terminologyDataMartId
        
        # Poll batch executions for 5 minutes to determine if they've been successful
        $wasTerminologySuccessful = Invoke-PollBatchExecutions -Config $Config -executionId $terminologyBatchExecutionId
        if (!$wasTerminologySuccessful) {
            Write-DosMessage -ErrorAction Stop -Level "Error" -Message "The Terminology tables could not be created. Please resolve the error with POSTing the Terminology data mart to the data processing service"
            throw "job failed"
        }
    }
    else {
        Write-DosMessage -Level "Information" -Message "Terminology schema already exist, skipping batch to create tables/views"
    }

    Test-DatabaseExists -SqlAddress $Config.edwAddress -Name "Shared"

    $viewExists = Test-ViewExists -SqlAddress $Config.edwAddress -Database "Shared" -Schema "Terminology" -View "ValueSetDescription"
    if ($viewExists -eq $false) {
        # POST SharedTerminology
        $sharedTerminologyBatchExecutionId = Invoke-PostToDps -Name "SharedTerminology" -Config $Config -dataMartId $sharedTerminologyDataMartId
        
        # Poll batch executions for 5 minutes to determine if they've been successful
        $wasSharedTerminologySuccessful = Invoke-PollBatchExecutions -Config $Config -executionId $sharedTerminologyBatchExecutionId -NoError
        if (!$wasSharedTerminologySuccessful) {
            # POST SharedTerminology
            $sharedTerminologyBatchExecutionId = Invoke-PostToDps -Name "SharedTerminology" -Config $Config -dataMartId $sharedTerminologyDataMartId
            
            # Poll batch executions for 5 minutes to determine if they've been successful
            $wasSharedTerminologySuccessful = Invoke-PollBatchExecutions -Config $Config -executionId $sharedTerminologyBatchExecutionId
            if (!$wasSharedTerminologySuccessful) {
                Write-DosMessage -ErrorAction Stop -Level "Error" -Message "The SharedTerminology tables/views could not be created. Please resolve the error with POSTing the SharedTerminology data mart to the data processing service"
                throw "job failed"
            }
        }
    }
    else {
        Write-DosMessage -Level "Information" -Message "Shared Terminology schema already exist, skipping batch to create tables/views"
    }

    # if DPS role was added for user, remove the role
    if ($roleAdded) {
        Remove-EdwAdminRole -RoleName "DataProcessingServiceUser" -Config $Config
    }

    # Create role Terminology API on Terminology db
    $RolePermissions = @{
        "[Catalyst].[Code]"                = "SELECT";
        "[Catalyst].[CodeBASE]"            = "SELECT";
        "[Catalyst].[CodeSystem]"          = "SELECT";
        "[Catalyst].[CodeSystemBASE]"      = "SELECT";
        "[Open].[ValueSetCode]"            = "SELECT";
        "[Open].[ValueSetCodeCountBASE]"   = "SELECT";
        "[Open].[ValueSetDescriptionBASE]" = "SELECT";
    }
    Publish-DatabaseRole -Config $Config -DatabaseName "Terminology" -RoleName "TerminologyServiceRole" -User $Config.iisUserCredentials.UserName -RolePermissions $RolePermissions

    # Create role Terminology API on Shared db
    $RolePermissions = @{
        "[ClientTerm].[CodeBASE]"                = "SELECT";
        "[ClientTerm].[CodeSystemBASE]"          = "SELECT";
        "[ClientTerm].[ValueSetCode]"            = "SELECT";
        "[ClientTerm].[ValueSetDescription]"     = "SELECT";
        "[ClientTerm].[ValueSetCodeBASE]"        = "SELECT, INSERT, UPDATE, DELETE";
        "[ClientTerm].[ValueSetCodeCountBASE]"   = "SELECT, INSERT, UPDATE, DELETE";
        "[ClientTerm].[ValueSetDescriptionBASE]" = "SELECT, INSERT, UPDATE, DELETE";
        "[Terminology].[Code]"                   = "SELECT";
        "[Terminology].[CodeSystem]"             = "SELECT";
        "[Terminology].[ValueSetCode]"           = "SELECT";
        "[Terminology].[ValueSetCodeCount]"      = "SELECT";
        "[Terminology].[ValueSetDescription]"    = "SELECT";
    } 
    Publish-DatabaseRole -Config $Config -DatabaseName "Shared" -RoleName "TerminologySharedServiceRole" -User $Config.iisUserCredentials.UserName -RolePermissions $RolePermissions
    
}

function Publish-DatabaseRole() {
    param(
        [PSCustomObject] $Config,
        [String] $DatabaseName,
        [String] $RoleName,
        [String] $User,
        [Hashtable] $RolePermissions = @{}
    )
    
    $serverAddress = $Config.edwAddress
    
    Write-DosMessage -Level "Information" -Message "Verifying/Creating role for $RoleName for $DatabaseName on $serverAddress server" 
    $Query = "DECLARE @cmd nvarchar(max)
    IF DATABASE_PRINCIPAL_ID(@RoleName) IS NULL
    BEGIN
        print '-- Creating role ';
        SET @cmd = N'CREATE ROLE ' + quotename(@RoleName);
        EXEC(@cmd);
    END"    
    $parameters = @{RoleName = $RoleName}
    Invoke-SqlCommand -SqlServerAddress $serverAddress -DatabaseName $DatabaseName -Query $Query -Parameters $parameters

    if ($RolePermissions.Keys.length -ge 1) {
        $Query = ""
        foreach ($entity in $RolePermissions.Keys) {
            $access = $RolePermissions[$entity]
            Write-DosMessage -Level "Information" -Message "Verifying/Creating permission $access $entity for role $RoleName"
            $Query = "$($Query)GRANT $access ON $entity TO $RoleName;`n"
        }
        Invoke-SqlCommand -SqlServerAddress $serverAddress -DatabaseName $DatabaseName -Query $Query
    }
    

    Write-DosMessage -Level "Information" -Message "Verifying/Creating login for $User on $serverAddress server" 
    $Query = "DECLARE @cmd nvarchar(max)
    DECLARE @EscapeQuoteUser varchar(max)
    SET @EscapeQuoteUser = replace(@User, '''', '''''')
    IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE sid = suser_sid(@User))
    BEGIN
        print '-- Creating login ' + @User;
        SET @cmd = N'CREATE LOGIN ' + quotename(@EscapeQuoteUser) + N' FROM WINDOWS';
        EXEC(@cmd);
    END
    "
    $parameters = @{User = $User}
    Invoke-SqlCommand -SqlServerAddress $serverAddress -DatabaseName $DatabaseName -Query $Query -Parameters $parameters

    Write-DosMessage -Level "Information" -Message "Verifying/Creating login for $User on $serverAddress on database $DatabaseName" 
    $Query = "DECLARE @cmd nvarchar(max)
    DECLARE @EscapeQuoteUser varchar(max)
    SET @EscapeQuoteUser = replace(@User, '''', '''''')
    IF(NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = @User))
    BEGIN
        print ''-- Creating user'';
        SET @cmd = N'CREATE USER ' + quotename(@EscapeQuoteUser) + N' FOR LOGIN ' + quotename(@EscapeQuoteUser);
        EXEC(@cmd);
    END"
    $parameters = @{User = $User}
    Invoke-SqlCommand -SqlServerAddress $serverAddress -DatabaseName $DatabaseName -Query $Query -Parameters $parameters


    Write-DosMessage -Level "Information" -Message "Adding $User to $RoleName on $serverAddress"
    $Query = "DECLARE @cmd nvarchar(max)
    DECLARE @exists int
    DECLARE @EscapeQuoteUser varchar(max)
    SET @EscapeQuoteUser = replace(@User, '''', '''''')
    SELECT @exists = IS_ROLEMEMBER (@RoleName, @EscapeQuoteUser)
    IF (@exists IS NULL OR @exists = 0)
    BEGIN
        print '-- Adding user to role ' + @RoleName
        SET @cmd = N'ALTER ROLE ' + @RoleName + N' ADD MEMBER ' + quotename(@EscapeQuoteUser)
        EXEC(@cmd)
    END"
    $parameters = @{RoleName = $RoleName; User = $User}
    Invoke-SqlCommand -SqlServerAddress $serverAddress -DatabaseName $DatabaseName -Query $Query -Parameters $parameters
}

function Publish-TerminologyDatabaseUpdates() {
    param(
        [PSCustomObject] $Config,
        [string] $Dacpac,
        [String] $PublishProfile
    )
    
    Publish-TerminologyDacpac -Config $Config -Dacpac $Dacpac -PublishProfile $PublishProfile    
    
    Publish-DatabaseRole -Config $Config -DatabaseName "Terminology" -RoleName "db_owner" -User $Config.loaderWindowsUser
    Publish-DatabaseRole -Config $Config -DatabaseName "Shared" -RoleName "db_owner" -User $Config.loaderWindowsUser

    Publish-DatabaseRole -Config $Config -DatabaseName "Terminology" -RoleName "ddladmin" -User $Config.processingServiceWindowsUser
    Publish-DatabaseRole -Config $Config -DatabaseName "Terminology" -RoleName "db_datareader" -User $Config.processingServiceWindowsUser
    Publish-DatabaseRole -Config $Config -DatabaseName "Terminology" -RoleName "db_datawriter" -User $Config.processingServiceWindowsUser
    Publish-DatabaseRole -Config $Config -DatabaseName "Shared" -RoleName "ddladmin" -User $Config.processingServiceWindowsUser
    Publish-DatabaseRole -Config $Config -DatabaseName "Shared" -RoleName "db_datareader" -User $Config.processingServiceWindowsUser
    Publish-DatabaseRole -Config $Config -DatabaseName "Shared" -RoleName "db_datawriter" -User $Config.processingServiceWindowsUser
}

function Test-Terminology {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSobject] $Config
    )

    $terminologyUrl = Get-ServiceFromDiscovery -DiscoveryUrl $Config.discoveryServiceUrl -Name $Config.appName -Version 1
    Invoke-PingService -ServiceUrl $terminologyUrl -ServiceName $Config.appName

    Write-DosMessage -Level "Information" -Message "Verifying Terminology database exists"
    Test-DatabaseExists -SqlAddress $Config.edwAddress -Name "Terminology"
}

Export-ModuleMember -function Import-Modules
Export-ModuleMember -function Get-TerminologyConfig
Export-ModuleMember -function Update-DiscoveryService
Export-ModuleMember -function Update-AppSettings
Export-ModuleMember -function Publish-TerminologyDatabaseUpdates
Export-ModuleMember -function Add-MetadataAndStructures
Export-ModuleMember -function Test-Terminology