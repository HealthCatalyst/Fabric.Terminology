function Get-UserValueOrDefault {
    param(
        [String] $Default,
        [String] $Prompt
    )

    $value = Read-Host "Enter the $Prompt or hit enter to accept the default value [$Default]"

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
                $result = Read-Host "Enter the $Prompt $AdditionalPromptInfo".Trim()
            }
        }
        else {
            $result = Read-Host "Enter the $Prompt $AdditionalPromptInfo".Trim()
        }

        return $result
    }
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
        $response = Invoke-WebRequest -Uri $uri -Method GET -UseDefaultCredentials
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
        Invoke-WebRequest -Uri $uri -Method GET -UseDefaultCredentials | Out-Null
    }
    catch [System.Net.WebException] {
        throw "There was an error communicating with the service.`n
        Request: $uri`n
        Status Code: $($_.Exception.Response.StatusCode.value__)`n
        Message: $($_.Exception.Response.StatusDescription)"
    }
}

function Invoke-ValidateServiceDependencies {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [String] $DiscoveryUrl
    )

    $services = @{ServiceName = "MetadataService"; Version = 2}, @{ServiceName = "DataProcessingService"; Version = 1}, @{ServiceName = "IdentityService"; Version = 1}, @{ServiceName = "AuthorizationService"; Version = 1}
    foreach ($service in $services) {
        Write-Host "Verifying $($service.ServiceName) (v$($service.Version))"
        Get-ServiceFromDiscovery -DiscoveryUrl $DiscoveryUrl -Name $service.ServiceName -Version $service.Version
    }
}

function Invoke-SqlCommand {
    param(
        [String] $SqlServerAddress,
        [String] $DatabaseName,
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
        Connection String: $($connectionString)"
        throw
    }
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
        SET @errorMessage = N'Could not confirm the ' + @dbname + ' database exists. Please verify the database was deployed. You may run this step again.'
        RAISERROR (@errorMessage, 11, 1)
        END"
    $parameters = @{dbname = "$Name"}
    Invoke-SqlCommand -SqlServerAddress $SqlAddress -Query $query -Parameters $parameters
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
        [String] $AppEndpoint,
        [String] $SqlDataDirectory,
        [String] $SqlLogDirectory,
        [switch] $Quiet
    )

    # Get Configuration
    $installSettings = Get-InstallationSettings "terminology"

    # Discovery Service Url
    $discoveryServiceUrlConfig = Get-ConfigValue -Prompt "Discovery Service URI" -AdditionalPromptInfo "(eg. https://SERVER/DiscoveryService/v1)" -DefaultFromParam $DiscoveryServiceUrl -DefaultFromInstallConfig $installSettings.discoveryServiceUrl -Quiet:$Quiet

    # Validate Service Dependencies
    $services = @{ServiceName = "MetadataService"; Version = 2}, @{ServiceName = "DataProcessingService"; Version = 1}, @{ServiceName = "IdentityService"; Version = 1}, @{ServiceName = "AuthorizationService"; Version = 1}
    foreach ($service in $services) {
        Write-DosMessage -Level "Information" -Message "Verifying $($service.ServiceName) (v$($service.Version)) is installed and registered"
        Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrlConfig -Name $service.ServiceName -Version $service.Version
    }

    # Get Credentials
    if ($Null -ne $Credentials) {
        $iisUserCredentials = $Credentials
    }
    else {
        $iisUserConfig = Get-ConfigValue -Prompt "IIS user name to run the app pool" -DefaultFromParam $IisUserName -DefaultFromInstallConfig $installSettings.iisUser -Quiet:$Quiet
        
        $passwordPrompt = "App pool user password"

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
    $appNameConfig = Get-ConfigValue -Prompt "service name" -DefaultFromParam $AppName -DefaultFromInstallConfig $installSettings.appName -Quiet:$Quiet

    # Terminology Service Endpoint
    $terminologyEndpointConfig = Get-ConfigValue -Prompt "$appNameConfig endpoint" -AdditionalPromptInfo "(https://SERVER/TerminologyService)" -DefaultFromParam $AppEnpoint -DefaultFromInstallConfig $installSettings.appEndpoint -Quiet:$Quiet

    # SQL Server Address
    $sqlAddressConfig = Get-ConfigValue -Prompt "address for SQL Server" -AdditionalPromptInfo "(eg. SERVER.DOMAIN.local)" -DefaultFromParam $SqlAddress -DefaultFromInstallConfig $installSettings.sqlServerAddress -Quiet:$Quiet

    # App Insights Key
    $appInsightsKeyConfig = Get-ConfigValue -Prompt "Application Insights key" -AdditionalPromptInfo "(optional)" -DefaultFromParam $AppInsightsKey -DefaultFromInstallConfig $installSettings.appInsightsKey -Required $false -Quiet:$Quiet

    # Metadata DB Name
    if ([string]::IsNullOrWhiteSpace($installSettings.metadataDbName)) {
        $metadataDbNameParam = "EDWAdmin"
    }
    else {
        $metadataDbNameParam = $installSettings.metadataDbName
    }
    $metadataDbNameConfig = Get-ConfigValue -Prompt "metadata database name" -DefaultFromParam $MetadataDbName -DefaultFromInstallConfig $metadataDbNameParam -Quiet:$Quiet

    # Data Directory
    if ([string]::IsNullOrWhiteSpace($installSettings.defaultSqlDataDirectory)) {
        $dbDefaults = Get-DbaDefaultPath -SqlInstance "localhost"
        $sqlDataDirectoryParam = $dbDefaults.Data
    }
    else {
        $sqlDataDirectoryParam = $installSettings.defaultSqlDataDirectory
    }
    $sqlDataDirectoryConfig = Get-ConfigValue -Prompt "Data directory to create Terminology database" -DefaultFromParam $SqlDataDirectory -DefaultFromInstallConfig $sqlDataDirectoryParam -Quiet:$Quiet

    # Log Directory
    if ([string]::IsNullOrWhiteSpace($installSettings.defaultSqlLogDirectory)) {
        $dbDefaults = Get-DbaDefaultPath -SqlInstance "localhost"
        $sqlLogDirectoryParam = $dbDefaults.Log
    }
    else {
        $sqlLogDirectoryParam = $installSettings.defaultSqlLogDirectory
    }
    $sqlLogDirectoryConfig = Get-ConfigValue -Prompt "Log directory to create Terminology database" -DefaultFromParam $SqlLogDirectory -DefaultFromInstallConfig $sqlLogDirectoryParam -Quiet:$Quiet

    Add-InstallationSetting "terminology" "appName" "$appNameConfig" | Out-Null
    Add-InstallationSetting "terminology" "appEndpoint" "$terminologyEndpointConfig" | Out-Null
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

    Write-DosMessage -Level "Information" -Message "Verifying metadata database ($metadataDbNameConfig) exists"
    Test-DatabaseExists -SqlAddress $sqlAddressConfig -Name $metadataDbNameConfig

    Write-DosMessage -Level "Information" -Message "Verifying Shared database exists"
    Test-DatabaseExists -SqlAddress $sqlAddressConfig -Name "Shared"
    
    # Setup config
    $config = [PSCustomObject]@{
        iisUserCredentials  = $iisUserCredentials
        appName             = $appNameConfig
        appEndpoint         = $terminologyEndpointConfig
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
        [Parameter(Mandatory = $true)]
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
        serviceUrl     = "$($Config.appEndpoint)/v$version"
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

function Get-RoleId {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $Role,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $SqlAddress
    )

    $query = "SELECT TOP (1) [RoleID] FROM [$metadataDatabase].[CatalystAdmin].[RoleBASE] WHERE RoleNM = '$Role'"
    $roleQueryResult = Invoke-SqlCommand -SqlServerAddress $SqlAddress -Query $query -DatabaseName $metadataDatabase -ReturnData $true
    $roleId = $roleQueryResult.Table.RoleID

    if ($roleId) {
        Write-Host "RoleID for $Role`: $roleId"
    }
    else {
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
        [string] $discoveryServiceUrl
    )

    # TODO THIS NEEDS TO CHANGE AFTER THE FABRIC UPDATES!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    $authToken = Read-Host "Log into Atlas was a DosAdmin, and copy the access token from one of the auth headers to MDS and copy it here. This is a temporary workaround"
    $dataMartJson = Get-Content -Raw -Path $path
    $headers = @{"Content-Type" = "application/json"}
    $headers.Add("Authorization", "Bearer $authToken")

    $mdsUrl = Get-ServiceFromDiscovery -DiscoveryUrl $discoveryServiceUrl -Name "MetadataService" -Version 2
       
    try {
        Write-DosMessage -Level "Information" -Message "Starting to create metadata for $name"
        $response = Invoke-RestMethod -Uri $mdsUrl/DataMarts -Method POST -Body $dataMartJson -Headers $headers
        Write-DosMessage -Level "Information" -Message "Completed creating metadata for $name"
        return $response.Id
    }
    catch [System.Net.WebException] {
        Write-DosMessage -Level "Error" -Message "Creating metadata for $name stopped"
        Write-DosMessage -Level "Error" -Message "Description:" $_.Exception.Response.StatusDescription
        Write-DosMessage -Level "Error" -Message  $_.ErrorDetails.Message | ConvertFrom-Json | Select-Object -Expand message
    }
}

function Invoke-PostToDps {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $dataMartId,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $discoveryServiceUrl
    )

    $headers = @{"Content-Type" = "application/json"}
    $dpsUrl = Get-ServiceFromDiscovery- DiscoveryUrl $discoveryServiceUrl -Name "DataProcessingService" -version 1

    try {
        $response = Invoke-RestMethod -Uri $dpsUrl/ExecuteDataMart -Method POST -UseDefaultCredentials -Headers $headers -Body "{ `"DataMartId`": $dataMartId, `"BatchExecution`": { `"PipelineType`": `"Migration`", `"OverrideLoadType`": `"Incremental`", `"LoggingLevel`": `"Diagnostic`" } }"
        $batchExecutionId = ($response.value | ConvertFrom-Json).Id
        Write-DosMessage -Level "Info" -Message  "Batch execution successfully sent to the data processing service."

        return $batchExecutionId;
    }
    catch [System.Net.WebException] {
        Write-DosMessage -Level "Error" -Message  "POST for '$name' failed with status code:" $_.Exception.Response.StatusCode.value__ 
        Write-DosMessage -Level "Error" -Message  "Description:" $_.Exception.Response.StatusDescription
    }
}

function Invoke-PollBatchExecutions {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $terminologyBatchExecutionId,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $sharedTerminologyBatchExecutionId
    )

    $dpsUrl = Get-ServiceFromDiscovery -name "DataProcessingService" -version 1
    $terminologyUrl = "$dpsUrl/BatchExecutions($terminologyBatchExecutionId)"
    $terminologyResponse = "";
    $sharedTerminologyResponse = "";
    $wasSuccessful = 0;

    foreach ($i in 1..30) {
        Start-Sleep -s 5
        if ($terminologyResponse -ne "Failed") {
            $terminologyResponse = Invoke-RestMethod -Uri $terminologyUrl -Method GET -UseDefaultCredentials
        }
        if ($sharedTerminologyResponse -ne "Failed") {
            $sharedTerminologyResponse = Invoke-RestMethod -Uri $terminologyUrl -Method GET -UseDefaultCredentials
        }

        if ($terminologyResponse.Status -eq "Failed" -or $sharedTerminologyResponse.Status -eq "Failed") {
            Write-DosMessage -Level "Error" -Error "One of the batch executions has failed. Please check EDW Console for additional logging. Halting the terminology registration."
            break
        }
        if ($terminologyResponse.Status -eq "Success" -and $sharedTerminologyResponse.Status -eq "Success") {
            Write-DosMessage -Level "Error" -Message "Both batch executions have run successfully."
            $wasSuccessful = 1;
            break
        }

        Write-DosMessage -Level "Information" -Message  "Terminology data mart status: $($terminologyResponse.Status)"
        Write-DosMessage -Level "Information" -Message  "SharedTerminology data mart status: $($sharedTerminologyResponse.Status)"
    }
    return $wasSuccessful;

}

function Add-MetadataAndStructures() {
    param(
        [Parameter(Mandatory = $true)]
        [PSobject] $Config
    )
    $discoveryServiceUrl = $Config.discoveryServiceUrl
    $metadataDatabase = $Config.metadataDbName
    $sqlAddress = $Config.sqlAddress
      
   
    # get IdentityID from IdentityBASE
    $user = "$env:USERDOMAIN\$env:USERNAME"
    $identityBaseQuery = "SELECT TOP (1) [IdentityID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityBASE] WHERE UPPER(IdentityNM) = UPPER('$user')"
    $identityQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityBaseQuery -ReturnData $true
    $identityId = $identityQueryResult.Table.IdentityID

    if ($identityId) {
        Write-DosMessage -Level "Information" -Message  "IdentityID for $user`: $identityId"
    }
    else {
        Write-DosMessage -Level "Information" -Message   "User is not in IdentityBASE... Adding user: $user"
        Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query "INSERT INTO [$metadataDatabase].[CatalystAdmin].[IdentityBASE] (IdentityNM) VALUES (UPPER('$user'))" -ReturnData $true
        $identityBaseQuery = "SELECT TOP (1) [IdentityID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityBASE] WHERE UPPER(IdentityNM) = UPPER('$user')"
        $identityQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityBaseQuery -ReturnData $true
        $identityId = $identityQueryResult.Table.IdentityID
    }

    $dataProcessingRole = "DataProcessingServiceUser"
    $dataProcessingServiceRoleId = Get-RoleId -Role $dataProcessingRole -SqlAddress $sqlAddress
    
    # check IdentityRoleBASE for DPS role
    $identityRoleBaseQuery = "SELECT TOP (1) [IdentityRoleID] FROM [$metadataDatabase].[CatalystAdmin].[IdentityRoleBASE] WHERE IdentityId = $identityId AND RoleID = $dataProcessingServiceRoleId"
    $identityRoleQueryResult = Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityRoleBaseQuery -ReturnData $true
    $dataProcessingRoleId = $identityRoleQueryResult.Table.IdentityRoleID
    $dataProcessingRoleAdded = $false

    if ($dataProcessingRoleId) {
        Write-Host "User $user already has the $dataProcessingRole role"
    }
    else {
        Write-Host "User $user does not have the $dataProcessingRole role"
        Write-Host "Adding $dataProcessingRole role now"
        $identityRoleBaseQuery = "INSERT INTO [$metadataDatabase].[CatalystAdmin].[IdentityRoleBASE] (IdentityID, RoleID) VALUES ($identityId, $dataProcessingServiceRoleId)"
        Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityRoleBaseQuery -ReturnData $true
        Write-Host "$dataProcessingRole role added for user $user"
        $dataProcessingRoleAdded = $true
    }

    # POST Terminology data marts to MDS
    Write-DosMessage -Level "Information" -Message "Creating Terminology metadata. This will take several minutes."
    $terminologyDataMartId = Invoke-PostToMds -discoveryServiceUrl $discoveryServiceUrl -name "Terminology" -path ".\Terminology.json"

    Write-DosMessage -Level "Information" -Message "Creating SharedTerminology metadata. This will take several minutes."
    $sharedTerminologyDataMartId = Invoke-PostToMds -discoveryServiceUrl $discoveryServiceUrl -name "SharedTerminology" -path ".\SharedTerminologyFiveNPEs.json"


    # POST executions 
    Write-DosMessage -Level "Information" -Message "Creating physical tables for Terminology and SharedTerminology data marts"
    $terminologyBatchExecutionId = Invoke-PostToDps -discoveryServiceUrl $discoveryServiceUrl -dataMartId $terminologyDataMartId
    $sharedTerminologyBatchExecutionId = Invoke-PostToDps -discoveryServiceUrl $discoveryServiceUrl -dataMartId $sharedTerminologyDataMartId

    # Poll batch executions for 5 minutes to determine if they've been successful
    $wasSuccessful = Invoke-PollBatchExecutions -terminologyBatchExecutionId $terminologyBatchExecutionId -sharedTerminologyBatchExecutionId $sharedTerminologyBatchExecutionId

    # if DPS role was added for user, remove the role
    if ($dataProcessingRoleAdded) {
        Write-DosMessage -Level "Information" -Message "Removing $dataProcessingRole role from user $user"
        $identityRoleBaseQuery = "DELETE FROM [$metadataDatabase].[CatalystAdmin].[IdentityRoleBASE] WHERE IdentityId = $identityId AND RoleID = $dataProcessingServiceRoleId"
        Invoke-SqlCommand -SqlServerAddress $sqlAddress -DatabaseName $metadataDatabase -Query $identityRoleBaseQuery
        Write-DosMessage -Level "Information" -Message "Role removed"
    }

    if (!$wasSuccessful) {
        Write-DosMessage -ErrorAction Stop -Level "Error" -Message "Terminology installation is halting, since batches could not be executed properly. Please check the logs in EDW Console and requeue if necessary."
    }
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
    $Parameters = @{RoleName = $($RoleName)}
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
    $Parameters = @{User = $($Config.iisUserCredentials.UserName)}
    Invoke-SqlCommand -SqlServerAddress $Config.sqlAddress -DatabaseName $DatabaseName -Query $Query -Parameters $Parameters


    Write-DosMessage -Level "Information" -Message "Adding $($Config.iisUserCredentials.UserName) to $RoleName on $($Config.sqlAddress)"
    $Query = "DECLARE @cmd nvarchar(max)
    IF IS_ROLEMEMBER (@RoleName, @User) <> 1
    BEGIN
        print '-- Adding user to role '
        SET @cmd = N'ALTER ROLE ' + quotename(@RoleName, ']') + N' ADD MEMBER ' + quotename(@User, ']')
        EXEC(@cmd)
    END";
    $Parameters = @{User = $($Config.iisUserCredentials.UserName); RoleName = $($RoleName)}
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
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [PSobject] $Config
    )

    $terminologyUrl = Get-ServiceFromDiscovery -DiscoveryUrl $Config.discoveryServiceUrl -Name $Config.appName -Version 1
    Invoke-PingService -ServiceUrl $terminologyUrl -ServiceName $Config.appName

    Write-DosMessage -Level "Information" -Message "Verifying Terminology database exists"
    Test-DatabaseExists -SqlAddress $Config.sqlAddress -Name "Terminology"
}

Export-ModuleMember -function Import-Modules
Export-ModuleMember -function Get-TerminologyConfig
Export-ModuleMember -function Update-DiscoveryService
Export-ModuleMember -function Update-AppSettings
Export-ModuleMember -function Publish-TerminologyDatabaseUpdates
Export-ModuleMember -function Add-MetadataAndStructures
Export-ModuleMember -function Test-Terminology