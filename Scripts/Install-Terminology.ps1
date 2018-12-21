param(
    [pscredential] $AppPoolCredential,
    [Hashtable] $ConfigStore,
    [string] $ConfigManifestPath
)
Write-Host "TERMINOLOGY SERVICE INSTALLER`n" -ForegroundColor Magenta

# fail installation on first error
$ErrorActionPreference = "Stop"

# add default values to both the config store and configuration manifest
# only if they're not passed in from some external script
if (!$ConfigStore) {
    $ConfigStore = @{
        Name   = "StoreConfig"
        Type   = "File"
        Format = "XML"
        Path   = "C:\Program Files\Health Catalyst\install.config"
    }
}
if (!$ConfigManifestPath) {
    $ConfigManifestPath = "$PSScriptRoot\configuration.manifest"
}

Write-Host "IMPORTING NECESSARY MODULES AND FUNCTIONS"
# get all dependent modules and functions
# Install-Module -Name DgxInstallUtilities -Scope CurrentUser -Force
# Import-Module -Name DgxInstallUtilities -Force
Import-Module "C:\Source\CAP\DgxInstallUtilities" -Force
Write-Host "Success`n" -ForegroundColor Green

Write-Host "PRE-INSTALLATION CHECKS"
# setup dos logging with default parameters 
Write-DosMessage -Level "Information" -Message "Setup DOS logging based on logging parameters"
Set-DosMessageConfiguration -LoggingMode Both -MinimumLoggingLevel "Information" -LogFilePath "$((Get-Item $PSScriptRoot).Parent.Parent.FullName)\SetupOutput\Install-Terminology.log"
# make sure configuration paths exist
Write-DosMessage -Level "Information" -Message "Find install config [$($ConfigStore.Path)]"
if (!(Test-Path $ConfigStore.Path)) { throw "Can't find config store [$($ConfigStore.Path)]" }
Write-DosMessage -Level "Information" -Message "Find configuration manifest [$($ConfigManifestPath)]"
if (!(Test-Path $ConfigManifestPath)) { throw "Can't find configuration manifest [$($ConfigManifestPath)]" }

# check to make sure all the configurations in the manifest exist in the install.config
$validConfigStore = @{
    ConfigStorePath    = $ConfigStore.Path
    ConfigManifestPath = $ConfigManifestPath
}
Confirm-ValidConfigStore @validConfigStore
Write-Host "Success`n" -ForegroundColor Green

Write-Host "GETTING CONFIGURATION VALUES"
# get store configurations from the config store (both common and app)
Write-DosMessage -Level "Information" -Message "Getting store configs from $([io.path]::GetFileName($ConfigStore.Path))"
$commonConfig = Get-DosConfigValues -ConfigStore $ConfigStore -Scope "common"
$appConfig = Get-DosConfigValues -ConfigStore $ConfigStore -Scope "terminology"
$storeConfig = @{
    fabricInstallerSecret           = $commonConfig.fabricInstallerSecret
    encryptionCertificateThumbprint = $commonConfig.encryptionCertificateThumbprint
    sqlServerAddress                = $commonConfig.sqlServerAddress
    edwAddress                      = $commonConfig.edwAddress
    metadataDbName                  = $commonConfig.metadataDbName
    discoveryService                = $commonConfig.discoveryService
    loaderWindowsUser               = $commonConfig.loaderWindowsUser
    processingServiceWindowsUser    = $commonConfig.processingServiceWindowsUser
    appName                         = $appConfig.appName
    appPoolName                     = $appConfig.appPoolName
    appPoolUser                     = $appConfig.appPoolUser
    generalAccessADGroup            = $appConfig.generalAccessADGroup
    siteName                        = $appConfig.siteName
    appEndpoint                     = $appConfig.appEndpoint
    appInsightsInstrumentationKey   = $appConfig.appInsightsInstrumentationKey
    sqlDataDirectory                = $appConfig.sqlDataDirectory
    sqlLogDirectory                 = $appConfig.sqlLogDirectory
} | Push-ConfigType "store"

# get static or derived configurations 
Write-DosMessage -Level "Information" -Message "Getting derived and static configurations"
$staticConfig = @{
    installPath                   = $ConfigStore.Path
    <# path to the location of the zip file that contains the binaries for service #>
    appPackagePath                = "$PSScriptRoot\Fabric.Terminology.InstallPackage.zip"
    <# name of primary dll within the appPackagePath that versioning is to be based on #>
    appPackageBaseAssembly        = "Fabric.Terminology.API.dll"
    <# path to database dacpac file used to create database objects #>
    dacpacPath                    = "$PSScriptRoot\Fabric.Terminology.Database.dacpac"
    dacpacSqlInstance             = $storeConfig.edwAddress
    dacpacDatabase                = "Terminology"
    <# file name given to the publish.xml file; the installer will create this for you
       so it doesn't have to be a valid file name, only a valid directory #>
    publishFilePath               = "$PSScriptRoot\Fabric.Terminology.Database.publish.xml"
    <# connection string to the metadata database as well as a test script that if records return is valid #>
    metadataConnection            = @{ sqlConnection = "Data Source=$($storeConfig.sqlServerAddress);Initial Catalog=$($storeConfig.metadataDbName);Integrated Security=True;Application Name=$($storeConfig.appName);"; sqlTestCommand = "SELECT object_id FROM sys.columns WHERE name = 'BuildNumberTXT' AND OBJECT_ID = OBJECT_ID('CatalystAdmin.DiscoveryServiceBASE')" }
    appFriendlyName               = "Fabric.Terminology"
    appDescription                = "The Fabric.Terminology Service provides shared healthcare terminology data."
    appDiscoveryType              = "Service"
    appIsHidden                   = $true
    authorizationService          = (Get-ServiceFromDiscovery -name "AuthorizationService" -version 1 $storeConfig.discoveryService )
    identityService               = (Get-ServiceFromDiscovery -name "IdentityService" -version 1 $storeConfig.discoveryService )
    metaDataService               = (Get-ServiceFromDiscovery -name "MetaDataService" -version 2 $storeConfig.discoveryService )
    dataProcessingService         = (Get-ServiceFromDiscovery -name "DataProcessingService" -version 1 $storeConfig.discoveryService )
    metaDataTerminologyPath       = "$PSScriptRoot\Terminology.json"
    metaDataSharedTerminologyPath = "$PSScriptRoot\SharedTerminology.json"
    <# array of roles to be applied to the database table created #>
    databaseLoaderRoleUpdates     = @(
        @{    
            databaseName  = 'Shared'
            server        = $storeConfig.edwAddress
            user          = $storeConfig.loaderWindowsUser
            databaseRoles = @(@{name = "db_owner"})
        },
        @{    
            databaseName  = 'Shared'
            server        = $storeConfig.edwAddress
            user          = $storeConfig.processingServiceWindowsUser
            databaseRoles = @(@{name = "db_ddladmin"}, @{name = "db_datareader"}, @{name = "db_datawriter"})
        },
        @{    
            databaseName  = 'Terminology'
            server        = $storeConfig.edwAddress
            user          = $storeConfig.loaderWindowsUser
            databaseRoles = @(@{name = "db_owner"})
        },
        @{    
            databaseName  = 'Terminology'
            server        = $storeConfig.edwAddress
            user          = $storeConfig.processingServiceWindowsUser
            databaseRoles = @(@{name = "db_ddladmin"}, @{name = "db_datareader"}, @{name = "db_datawriter"})
        }
    )
    databaseAppRoleUpdates        = @(
        @{    
            databaseName  = 'Shared'
            server        = $storeConfig.edwAddress
            user          = $storeConfig.appPoolUser
            databaseRoles = @(
                @{
                    name        = "TerminologySharedServiceRole"
                    permissions = @{
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
                }
            )
        },
        @{
            databaseName  = 'Terminology'
            server        = $storeConfig.edwAddress
            user          = $storeConfig.appPoolUser
            databaseRoles = @(
                @{
                    name        = "TerminologyServiceRole"
                    permissions = @{
                        "[Catalyst].[Code]"                = "SELECT";
                        "[Catalyst].[CodeBASE]"            = "SELECT";
                        "[Catalyst].[CodeSystem]"          = "SELECT";
                        "[Catalyst].[CodeSystemBASE]"      = "SELECT";
                        "[Open].[ValueSetCode]"            = "SELECT";
                        "[Open].[ValueSetCodeCountBASE]"   = "SELECT";
                        "[Open].[ValueSetDescriptionBASE]" = "SELECT";
                    }
                }
            )
        }
    )
    <# configuration for both registration with fabric.identity as well as fabric.authorization #>
    apiRegistration               = @{
        name              = "terminology-api"
        userClaims        = @("name", "email", "role", "groups")
        scopes            = @(@{ name = "dos/valuesets"; displayName = "ValueSets" })
        clientId          = "terminology-service"
        clientName        = "terminology-client"
        allowedGrantTypes = @("delegation", "client_credentials")
        allowedScopes     = @("fabric/authorization.read")
        securableItems    = @(
            @{
                grain = "dos"
                name  = "valuesets"
                roles = @(
                    @{
                        name        = "valueset-publisher"
                        displayName = "Publisher"
                        description = "Create, read, update, and delete value sets"
                        group       = @{ groupName = "DosAdmins"; groupSource = "custom" }
                        permissions = @("publish", "access", "manageauthorization")
                    },
                    @{
                        name        = "valueset-accessor"
                        displayName = "Accessor"
                        description = "Read access to value sets"
                        group       = @{ groupName = $appConfig.generalAccessADGroup; groupSource = "directory" }
                        permissions = @("access")
                    }
                )
            }
        )
    }
} | Push-ConfigType "static";

# combine all configuration into one succint hashtable
$config = $storeConfig, $staticConfig | Merge-Hashtables

# assign a confirmation checkList for each configuration
$checkList = @{
    # store config
    fabricInstallerSecret           = @("isNotNull")
    encryptionCertificateThumbprint = @("isNotNull")
    sqlServerAddress                = @("isNotNull")
    edwAddress                      = @("isNotNull")
    metadataDbName                  = @("isNotNull")
    discoveryService                = @("isValidEndpoint")
    loaderWindowsUser               = @("isNotNull")
    processingServiceWindowsUser    = @("isNotNull")
    appName                         = @("isNotNull")
    appPoolName                     = @("isNotNull")
    appPoolUser                     = @("isNotNull")
    generalAccessADGroup            = @("isNotNull")
    siteName                        = @("isNotNull")
    appEndpoint                     = @("isNotNull")
    sqlDataDirectory                = @("isNotNull")
    sqlLogDirectory                 = @("isNotNull")
    # static or derived
    installPath                     = @("isValidPath")
    appPackagePath                  = @("isValidPath")
    appPackageBaseAssembly          = @("isNotNull")
    dacpacPath                      = @("isValidPath")
    dacpacSqlInstance               = @("isNotNull")
    dacpacDatabase                  = @("isNotNull")
    publishFilePath                 = @("isValidDir")
    metadataConnection              = @("isValidConnection")
    appFriendlyName                 = @("isNotNull")
    appDescription                  = @("isNotNull")
    appDiscoveryType                = @("isNotNull")
    appIsHidden                     = @("isBoolean")
    authorizationService            = @("isNotNull")
    identityService                 = @("isNotNull")
    metaDataService                 = @("isNotNull")
    dataProcessingService           = @("isNotNull")
    metaDataTerminologyPath         = @("isValidPath")
    metaDataSharedTerminologyPath   = @("isValidPath")
    databaseLoaderRoleUpdates       = @("isNotNull")
    databaseAppRoleUpdates          = @("isNotNull")
    apiRegistration                 = @("isNotNull")
} 
Write-Host "Success`n" -ForegroundColor Green

Write-Host "RUN CONFIGURATION CHECKS"
# check all configurations for issues 
Write-DosMessage -Level "Information" -Message "Checking all configurations for issues"
Confirm-Configurations -config $config -checkList $checkList
Write-Host "Success`n" -ForegroundColor Green

Write-Host "DEPLOY IIS WEB APPLICATION"
# deploy iis web application
if (!$AppPoolCredential) {
    $storedCredentialParams = @{
        Target   = "healthcatalyst:dos-installer:$($config.appName)"
        UserName = $config.appPoolUser
        Persist  = "Enterprise"
        Type     = "Generic"
        Message  = "Please Enter AppPool Credentials for [$($config.appPoolUser)]"
    }
    $AppPoolCredential = Get-CredentialsFromStore @storedCredentialParams
}
Write-DosMessage -Level "Information" -Message "Deploying IIS Web Application"
$dosWebApplicationParams = @{
    WebAppPackagePath = $config.appPackagePath
    AppPoolName       = $config.appPoolName
    AppPoolCredential = $AppPoolCredential
    AppName           = $config.appName
    IISWebSite        = $config.siteName
}
Publish-DosWebApplication @dosWebApplicationParams -ErrorAction Stop | Out-Null
Write-Host "Success`n" -ForegroundColor Green

Write-Host "REGISTER WITH DISCOVERY SERVICE"
# register new service with discovery
Write-DosMessage -Level "Information" -Message "Registering $($config.appFriendlyName) with discovery service"
$webroot = Get-WebFilePath -PSPath "IIS:\Sites\$($config.siteName)\$($config.appName)" -ErrorAction Stop
$assembly = [System.IO.Path]::Combine($webroot, $config.appPackageBaseAssembly)
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($assembly)
$discoveryRegistrationSqlParams = @{
    connectionString  = $config.metadataConnection.sqlConnection
    discoveryPostBody = @{
        serviceName    = "$($config.appName)"
        serviceVersion = "$($version.FileMajorPart)"
        friendlyName   = "$($config.appFriendlyName)"
        description    = "$($config.appDescription)"
        serviceUrl     = "$($config.appEndpoint)/v$($version.FileMajorPart)"
        buildVersion   = "$($version.FileVersion)"
        isHidden       = $config.appIsHidden
        discoveryType  = "$($config.appDiscoveryType)"
    }
}
Add-DiscoveryRegistrationSql @discoveryRegistrationSqlParams -ErrorAction Stop | Out-Null
Write-Host "Success`n" -ForegroundColor Green

Write-Host "REGISTER WITH FABRIC.IDENTITY"
# get access token
Write-DosMessage -Level "Information" -Message "Getting access token using encryption thumbprint and fabric installer secret"
$encryptionCertificate = Get-EncryptionCertificate $config.encryptionCertificateThumbprint
$secret = Get-DecryptedString -encryptionCertificate $encryptionCertificate -encryptedString $config.fabricInstallerSecret
$accessTokenParams = @{
    identityUrl = $config.identityService
    clientId    = "fabric-installer"
    scope       = "fabric/identity.manageresources fabric/authorization.write fabric/authorization.read fabric/authorization.dos.write fabric/authorization.manageclients dos/metadata dos/metadata.serviceAdmin"
    secret      = $secret
}
$accessToken = Get-AccessToken @accessTokenParams

# register new api with fabric.identity
Write-DosMessage -Level "Information" -Message "Registering api resources ""$($config.apiRegistration.name)"" with Fabric.Identity"
$apiRegistrationParams = @{
    identityUrl = $config.identityService
    accessToken = $accessToken
    body        = @{
        enabled    = $true
        name       = $config.apiRegistration.name
        userClaims = $config.apiRegistration.userClaims
        scopes     = $config.apiRegistration.scopes
    }
}
$apiSecret = New-ApiRegistration @apiRegistrationParams

# register new client with fabric.identity
Write-DosMessage -Level "Information" -Message "Registering client ""$($config.apiRegistration.clientName)"" with Fabric.Identity"
$clientRegistrationParams = @{
    identityUrl = $config.identityService
    accessToken = $accessToken
    body        = @{
        enabled           = $true
        clientId          = $config.apiRegistration.clientId
        clientName        = $config.apiRegistration.clientName
        allowedGrantTypes = $config.apiRegistration.allowedGrantTypes
        allowedScopes     = $config.apiRegistration.allowedScopes
    }
}
$clientSecret = New-ClientRegistration @clientRegistrationParams
Write-Host "Success`n" -ForegroundColor Green

Write-Host "REGISTER WITH FABRIC.AUTHORIZATION"
# register new client with fabric.authorization
# note: this automatically creates a new "[clientId]-admin" role of grain "app" with the permission name "manageauthorization" 
Write-DosMessage -Level "Information" -Message "Registering client ""$($config.apiRegistration.clientName)"" with Fabric.Authorization"
$clientAuthorizationParams = @{
    authUrl     = $config.authorizationService
    accessToken = $accessToken
    body        = @{
        id   = $config.apiRegistration.clientId
        name = $config.apiRegistration.clientName
    }
}
New-ClientAuthorization @clientAuthorizationParams | Out-Null

# register securable item roles and permissions with fabric.authorization
Write-DosMessage -Level "Information" -Message "Registering securable item roles with Fabric.Authorization"
foreach ($securableItem in $config.apiRegistration.securableItems) {
    foreach ($role in $securableItem.roles) {
        # register roles with fabric.authorization
        $roleAuthorizationParams = @{
            authUrl     = $config.authorizationService
            accessToken = $accessToken
            body        = @{
                name          = $role.name
                displayName   = $role.displayName
                description   = $role.description
                grain         = $securableItem.grain
                securableItem = $securableItem.name
            }
        }
        $roleResponse = New-RoleAuthorization @roleAuthorizationParams

        # only if the role was configured with a group name
        if (!([string]::IsNullOrEmpty($($role.group)))) {
            # register new groups associated with a role
            $groupAuthorizationParams = @{
                authUrl     = $config.authorizationService
                accessToken = $accessToken
                body        = @{
                    groupName   = $role.group.groupName
                    groupSource = $role.group.groupSource
                }
            }
            New-GroupAuthorization @groupAuthorizationParams | Out-Null
            
            # connect the role to the group
            $roleResponse.permissions = $null
            $groupRoleAuthorizationParams = @{
                authUrl     = $config.authorizationService
                group       = $role.group
                accessToken = $accessToken
                body        = @($roleResponse)
            }
            New-GroupRoleAuthorization @groupRoleAuthorizationParams | Out-Null
        }

        foreach ($permission in $role.permissions) {
            # register new permissions
            $permissionAuthorizationParams = @{
                authUrl     = $config.authorizationService
                accessToken = $accessToken
                body        = @{
                    name          = $permission
                    grain         = $securableItem.grain
                    securableItem = $securableItem.name
                }
            }
            $permissionResponse = New-PermissionAuthorization @permissionAuthorizationParams        

            # connect the permission to the role
            $permissionRoleAuthorizationParams = @{
                authUrl     = $config.authorizationService
                roleId      = $roleResponse.id
                roleName    = $roleResponse.name
                accessToken = $accessToken
                body        = @($permissionResponse)
            }
            New-PermissionRoleAuthorization @permissionRoleAuthorizationParams | Out-Null
        }
    }
}
Write-Host "Success`n" -ForegroundColor Green

Write-Host "PUBLISH TERMINOLOGY DATABASE WITH DACPAC"
# create a publish xml file with configured mount points for dacpac
Write-DosMessage -Level "Information" -Message "Creating a publish xml file with configured mount points for dacpac"
$dacPacPublishFileParams = @{
    publishFilePath  = $config.publishFilePath
    sqlDataDirectory = $config.sqlDataDirectory
    sqlLogDirectory  = $config.sqlLogDirectory
}
Set-DacPacPublishFile @dacPacPublishFileParams -ErrorAction Stop | Out-Null

# publish database objects using the configured dacpac file
Write-DosMessage -Level "Information" -Message "Publishing database objects using the configured DACPAC file"
$dosDacPacParams = @{
    DacPacFilePath         = $config.dacpacPath
    TargetSqlInstance      = $config.dacpacSqlInstance
    TargetDb               = $config.dacpacDatabase
    PublishOptionsFilePath = $config.publishFilePath
}
Publish-DosDacPac @dosDacPacParams -ErrorAction Stop | Out-Null
Write-Host "Success`n" -ForegroundColor Green

Write-Host "APPLY DATABASE LOADER PERMISSIONS"
# apply database permissions needed for data loading
foreach ($databaseUpdate in $config.databaseLoaderRoleUpdates) {
    Write-DosMessage -Level "Information" -Message "Applying $($databaseUpdate.databaseName) database permissions"
    $publishDatabaseRoleParams = @{
        databaseName     = $databaseUpdate.databaseName
        user             = $databaseUpdate.user
        sqlServerAddress = $databaseUpdate.server
        databaseRoles    = $databaseUpdate.databaseRoles
    }
    Publish-DatabaseRole @publishDatabaseRoleParams
}
Write-Host "Success`n" -ForegroundColor Green

Write-Host "REGISTER DATAMART METADATA WITH METADATA SERVICE"
# Terminology datamart
$datamartMetadataParams = @{
    metadataUrl = $config.metaDataService
    body        = (Get-Content $config.metaDataTerminologyPath -Raw)
    accessToken = $accessToken
}
$dataMartTerminology = New-DatamartMetadata @datamartMetadataParams

# SharedTerminology datamart
$datamartMetadataParams = @{
    metadataUrl = $config.metaDataService
    body        = (Get-Content $config.metaDataSharedTerminologyPath -Raw)
    accessToken = $accessToken
}
$dataMartSharedTerminology = New-DatamartMetadata @datamartMetadataParams
Write-Host "Success" -ForegroundColor Green

# Terminology datamart
$batchExecutionDataProcessingParams = @{
    dataProcessingUrl = $config.dataProcessingService
    body              = @{
        DataMartId      = $dataMartTerminology.Id
        BatchExecution  = @{
            PipelineType     = "Migration"
            OverrideLoadType = "Incremental"
            LoggingLevel     = "Diagnostic"
        }
        BatchDefinition = @{
            Name = "Terminology (DOSInstaller)"
        }
    }
    edwAdminRole      = @{
        roleName         = "DataProcessingServiceUser"
        sqlServerAddress = $config.sqlServerAddress
        metadataDbName   = $config.metadataDbName
    }
}
Invoke-BatchExecutionDataProcessing @batchExecutionDataProcessingParams | Out-Null
Write-DosMessage -Level "Information" -Message "Batch Completed"
Write-Host "Success" -ForegroundColor Green

# SharedTerminology (Custom Batch) datamart
$customEntititesParams = @{
    dataMartId       = $dataMartSharedTerminology.Id
    entities         = @(
        @{entitySchema = "ClientTerm"; entityName = "Term"}
        @{entitySchema = "ClientTerm"; entityName = "CodeSystem"}
        @{entitySchema = "ClientTerm"; entityName = "ValueSetMeasure"}
        @{entitySchema = "ClientTerm"; entityName = "ValueSetCodeCount"}
        @{entitySchema = "ClientTerm"; entityName = "Map"}
        @{entitySchema = "ClientTerm"; entityName = "MapSystem"}
        @{entitySchema = "ClientTerm"; entityName = "Relation"}
        @{entitySchema = "ClientTerm"; entityName = "ValueSetCode"}
        @{entitySchema = "ClientTerm"; entityName = "ValueSetDescriptionAttribute"}
        @{entitySchema = "ClientTerm"; entityName = "ValueSetDescription"}
        @{entitySchema = "TermMap"; entityName = "LocalMap"}
    )
    sqlServerAddress = $config.sqlServerAddress
    metadataDbName   = $config.metadataDbName
}
$customEntitites = Get-CustomEntities @customEntititesParams
$batchExecutionDataProcessingParams = @{
    dataProcessingUrl = $config.dataProcessingService
    body              = @{
        DataMartId      = $dataMartSharedTerminology.Id
        BatchExecution  = @{
            PipelineType     = "Migration"
            OverrideLoadType = "Incremental"
            LoggingLevel     = "Diagnostic"
        }
        BatchDefinition = @{
            Name                        = "SharedTerminology Step1 (DOSInstaller)"
            LoadType                    = "Custom Entities"
            IncludeDownstreamDependents = "false"
            CustomEntities              = $customEntitites
        }
    }
    edwAdminRole      = @{
        roleName         = "DataProcessingServiceUser"
        sqlServerAddress = $config.sqlServerAddress
        metadataDbName   = $config.metadataDbName
    }
}
Invoke-BatchExecutionDataProcessing @batchExecutionDataProcessingParams #| Out-Null
Write-DosMessage -Level "Information" -Message "Batch Completed"
Write-Host "Success" -ForegroundColor Green

# SharedTerminology datamart
$batchExecutionDataProcessingParams = @{
    dataProcessingUrl = $config.dataProcessingService
    body              = @{
        DataMartId      = $dataMartSharedTerminology.Id
        BatchExecution  = @{
            PipelineType     = "Migration"
            OverrideLoadType = "Incremental"
            LoggingLevel     = "Diagnostic"
        }
        BatchDefinition = @{
            Name = "SharedTerminology Step2 (DOSInstaller)"
        }
    }
    edwAdminRole      = @{
        roleName         = "DataProcessingServiceUser"
        sqlServerAddress = $config.sqlServerAddress
        metadataDbName   = $config.metadataDbName
    }
}
Invoke-BatchExecutionDataProcessing @batchExecutionDataProcessingParams | Out-Null
Write-DosMessage -Level "Information" -Message "Batch Completed"
Write-Host "Success`n" -ForegroundColor Green

Write-Host "APPLY DATABASE APPLICATION PERMISSIONS"
# apply database permissions needed for application users
foreach ($databaseUpdate in $config.databaseAppRoleUpdates) {
    Write-DosMessage -Level "Information" -Message "Applying $($databaseUpdate.databaseName) database permissions"
    $publishDatabaseRoleParams = @{
        databaseName     = $databaseUpdate.databaseName
        user             = $databaseUpdate.user
        sqlServerAddress = $databaseUpdate.server
        databaseRoles    = $databaseUpdate.databaseRoles
    }
    Publish-DatabaseRole @publishDatabaseRoleParams
}
Write-Host "Success`n" -ForegroundColor Green

Write-Host "UPDATE APPLICATION SETTINGS"
# update settings in web.config
Write-DosMessage -Level "Information" -Message "Updating settings in web.config"
$webConfigPath = [System.IO.Path]::Combine($webroot, 'web.config')
$webConfigParams = @(
    @{webConfigPath = $webConfigPath; settingKey = "ClientSecret"; settingValue = $clientSecret}
    @{webConfigPath = $webConfigPath; settingKey = "apiSecret"; settingValue = $apiSecret}
)
foreach ($webConfigParam in $webConfigParams) {
    Update-WebConfig @webConfigParam
}
Write-Host "Success`n" -ForegroundColor Green

# update settings in appsettings.json
Write-DosMessage -Level "Information" -Message "Updating settings in appsettings.json"
$appSettingsPath = [System.IO.Path]::Combine($webroot, 'appsettings.json')
$accessControl = Get-Acl $webroot
$accessControlRule = New-Object System.Security.AccessControl.FileSystemAccessRule($config.appPoolUser, "Read, Write", "ContainerInherit, ObjectInherit", "None", "Allow")
$accessControl.SetAccessRule($accessControlRule)
Set-Acl $webroot.FullName $accessControl
$appSettingsJson = (Get-Content $appSettingsPath -Raw) | ConvertFrom-Json 
$appSettingsJson.BaseTerminologyEndpoint = $config.appEndpoint
$appSettingsJson.TerminologySqlSettings.ConnectionString = "Data Source=$($config.edwAddress);Initial Catalog=Shared; Trusted_Connection=True;"
$appSettingsJson.IdentityServerSettings.ClientSecret = $config.appName
$appSettingsJson.DiscoveryServiceClientSettings.DiscoveryServiceUrl = $config.discoveryService
if ([string]::IsNullOrWhiteSpace($config.appInsightsInstrumentationKey)) {
    $appSettingsJson.ApplicationInsightsSettings.Enabled = $false
} 
else {
    $appSettingsJson.ApplicationInsightsSettings.InstrumentationKey = $config.appInsightsInstrumentationKey    
    $appSettingsJson.ApplicationInsightsSettings.Enabled = $true
}
$appSettingsJson | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath
Write-Host "Success`n" -ForegroundColor Green


Write-Host "INSTALLATION COMPLETED SUCCESSFULLY!" -ForegroundColor Green