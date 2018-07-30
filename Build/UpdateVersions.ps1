<#
	.Synopsis
	Updates corresponding .csproj files versioning information with version set in the build "version.json" file 
	to ensure version consistency between like projects, binaries and NuGet packages.
	Intended to be used in VSTS CI Build workflow.

	.Pre-requisites
	 -> .\Build\Modules\CsprojUtilities.psm1;
	 -> .\Build\Modules\SolutionVersionConfig.psm1;
#>
Param(
	[string]$buildNumber = "0"
)
Import-Module .\Build\Modules\CsprojUtilities.psm1;
Import-Module .\Build\Modules\SolutionVersionConfig.psm1;

Function Set-InstallerNuSpecVersion
{
	$nuspecFile = "$(Get-Location)\Fabric.Terminology.API\Fabric.Terminology.InstallPackage.nuspec";

	Write-Host "$($OFS)Updating DOS Installer NuSpec" -ForegroundColor Green;
	[xml]$nuspec = Get-Content -Path $nuspecFile;
	$nuspecTemp = $nuspecFile + ".tmp";

	$metadata = $nuspec.GetElementsByTagName("metadata") | Select-Object -First 1

	# set FileVersion
	if (!$metadata.version) 
	{
		$nuspecVersion = $nuspec.CreateNode("element", "version", "");
		$metadata.AppendChild($nuspecVersion);
	}
	$metadata.version = $version.NuSpecVersion;

	$nuspec.save($nuspecTemp)
	Move-Item $nuspecTemp $nuspecFile -Force;
}

Function Publish-TerminologyVersionUpdate([string] $projectPath, [string] $releaseComment)
{
	$terminologyVersion = "$projectPath\Configuration\TerminologyVersion.cs";
	echo $terminologyVersion;
	if (Test-Path $terminologyVersion -PathType Leaf)
	{
		Write-Host "Beginning TerminologyVersion.cs update." -ForegroundColor Gray;
		$tempFile = $suiteVersion + ".tmp"
		$currentComment = 'CurrentComment => "' + $releaseComment +'";';

		Get-Content $terminologyVersion |
			%{$_ -replace 'CurrentComment => .*;', $currentComment } > $tempFile;

		Move-Item $tempFile $terminologyVersion -Force;

		Write-Host "Completed TerminologyVersion.cs update." -ForegroundColor Gray;
	}
}


$OFS = "`r`n`r`n";
$buildDir = "$(Get-Location)\build";

## Get the version config for the solution.
$version = Get-SolutionVersionFromConfig $buildDir $buildNumber;

Write-Host ($version | Format-Table | Out-String)

## Gets all of the csproj files (including tests) for the solution.
$projects = Get-ProjectFilesExcludingTestsForSolution $(Get-Location);
Write-Host "Updating Versions in Project Files" -ForegroundColor Green;
Foreach($csproj in $projects)
{
	$projName = [io.path]::GetFileNameWithoutExtension($csproj);
	Write-Host "Setting $projName to version $($version.NuSpecVersion)" -ForegroundColor Gray;
	Set-ProjectAssemblyInfoVersionsAndEnsureDefaults $csproj $version;
	if ($projName.EndsWith("API"))
	{
		$apiProjPath = "$(Get-Location)\$projName";
		Publish-TerminologyVersionUpdate $apiProjPath $version.ReleaseComment
	}
}

Set-InstallerNuSpecVersion

Remove-Module "CsprojUtilities";
Remove-Module "SolutionVersionConfig";


