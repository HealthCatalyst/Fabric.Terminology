Param(
	[string]$buildVersion
)
$version = '0.1.4'
$fullVersion = $version
$versionComment = 'alpha'

if ($versionComment -ne '' -and $buildVersion -ne '') {
    $fullVersion = $version + '.' + $buildVersion
}

echo $version

# update project files to set the 
$proj1 = '..\Fabric.Terminology.API\Fabric.Terminology.API.csproj'
$proj2 = '..\Fabric.Terminology.Domain\Fabric.Terminology.Domain.csproj'
$proj3 = '..\Fabric.Terminology.SqlServer\Fabric.Terminology.SqlServer.csproj'

dotnet restore '..\Fabric.Terminology.sln'
dotnet build $proj1 --configuration Release /property:Version=$version /property:AssemblyVersion=$fullVersion /property:FileVersion=$fullVersion