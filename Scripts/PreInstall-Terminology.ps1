<# 
.SYNOPSIS 
 
Setup all dependencies for the install script Install-Terminology.ps1 

.DESCRIPTION 
 
 The following are required to run .\Install-Terminology.ps1
 - DosInstallUtilites 
    Powershell modules created and maintained by devops team to install/upgrade DOS applications
    https://www.powershellgallery.com/packages/DosInstallUtilities
- dbatools
    Sql Server tools used to deploy databases
    https://www.powershellgallery.com/packages/dbatools
- Fabric-Install-Utilities.psm1
    Install scripts created and maintained by the fabric team to install fabric applications
    https://github.com/HealthCatalyst/InstallScripts
- Register.ps1
    Powershell script to read xml file and register application/scopes/permissions with fabric Identity
    https://github.com/HealthCatalyst/Fabric.Identity
- Fabric.Terminology.InstallPackage.zip
    Publish output of Fabric.Terminology.API project
- Fabric.Terminology.Database.dacpac
    Publish dacpac of Fabric.Terminology.Database SSDT project
- Install.config
    Shared install settings need to be copied from C:\Program Files\Health Catalyst

.EXAMPLE 
 
.\PreInstall-Terminology.ps1
 
#> 

# Download required Modules
if (!(Test-Path -Path "./DosInstallUtilities")) {
    Save-Module -Name DosInstallUtilities -Path "./"
}
if (!(Test-Path -Path "./dbatools")) {
    Save-Module -Name dbatools -Path "./"
}
if (!(Test-Path -Path "./Fabric-Install-Utilities.psm1")) {
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/HealthCatalyst/InstallScripts/master/common/Fabric-Install-Utilities.psm1" -Headers @{"Cache-Control"="no-cache"} -OutFile "./Fabric-Install-Utilities.psm1"
}
if (!(Test-Path -Path "./Register.ps1")) {
    Invoke-WebRequest -Uri "https://raw.githubusercontent.com/HealthCatalyst/Fabric.Identity/master/Fabric.Identity.API/scripts/Register.ps1" -Headers @{"Cache-Control"="no-cache"} -OutFile "./Register.ps1"
}

# Publish web app
dotnet publish "..\Fabric.Terminology.API\Fabric.Terminology.API.csproj" --configuration release --output "..\Fabric.Terminology.InstallPackage\"
if (Test-Path -Path ".\Fabric.Terminology.InstallPackage.zip") {
    Remove-Item -Path ".\Fabric.Terminology.InstallPackage.zip"
}
Compress-Archive -Path "..\Fabric.Terminology.InstallPackage\*" -DestinationPath ".\Fabric.Terminology.InstallPackage.zip"
if (Test-Path -Path "..\Fabric.Terminology.InstallPackage\") {
   Remove-Item -Path "..\Fabric.Terminology.InstallPackage\" -Recurse
}

#Build dacpac
try {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" "C:\source\github\Fabric.Terminology\Fabric.Terminology.Database\Fabric.Terminology.Database.sqlproj" /p:platform="any cpu" /p:configuration="release"    
}
catch {
    Write-Error -Message "Building dacpac failed. Please verify Visual Studio 2017 is installed with SSDT for visual studio 2017"
    throw $_.Exception
}
$dacpac = "..\Fabric.Terminology.Database\bin\output\Fabric.Terminology.Database.dacpac"
if (-not (Test-Path -Path $dacpac)) {
    throw "dacpac $dacpac does not exist"
}
Copy-Item -Path $dacpac -Destination ".\Fabric.Terminology.Database.dacpac"

#Copy shared config settings
$sharedConfigLocation = Join-Path -Path "$Env:Programfiles" -ChildPath "Health Catalyst\install.config"
if (!(Test-Path -Path $sharedConfigLocation)) {
    throw "Shared configuration does not exist in $sharedConfigLocation. Please verify you have installed DOS correctly including Fabric Identity and Fabric Authorization"
}

$localConfigLocation = "$PSScriptRoot\install.config"
if (!(Test-Path -Path $localConfigLocation)) {
    throw "Local configuration does not exist in $localConfigLocation."
}

[System.Xml.XmlDocument]$sharedConfig = new-object System.Xml.XmlDocument
$sharedConfig.load($sharedConfigLocation)
$fabricInstallerSecret = $sharedConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='fabricInstallerSecret']/@value").Value
Write-host "fabricInstallerSecret: $fabricInstallerSecret"
$encryptionCertificateThumbprint = $sharedConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='encryptionCertificateThumbprint']/@value").Value
Write-host "encryptionCertificateThumbprint: $encryptionCertificateThumbprint"
$sqlServerAddress = $sharedConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='sqlServerAddress']/@value").Value
Write-host "sqlServerAddress: $sqlServerAddress"
$metadataDbName = $sharedConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='metadataDbName']/@value").Value
Write-host "metadataDbName: $metadataDbName"
$discoveryService = $sharedConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='discoveryService']/@value").Value
Write-host "discoveryService: $discoveryService"

[System.Xml.XmlDocument]$localConfig = new-object System.Xml.XmlDocument
$localConfig.load($localConfigLocation)
$localConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='fabricInstallerSecret']/@value").Value = $fabricInstallerSecret
$localConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='encryptionCertificateThumbprint']/@value").Value = $encryptionCertificateThumbprint
$localConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='sqlServerAddress']/@value").Value = $sqlServerAddress
$localConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='metadataDbName']/@value").Value = $metadataDbName
$localConfig.SelectSingleNode("/installation/settings/scope[@name='common']/variable[@name='discoveryService']/@value").Value = $discoveryService
$localConfig.Save($localConfigLocation);