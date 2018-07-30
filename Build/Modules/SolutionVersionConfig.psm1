<#
	.Synopsis
	Functions for reading and parsing Version.json configuration files at individual solution levels.	
#>
Set-Variable VersionFileName -Option Constant -Value "version.json";

Function TryParseBool([string]$value)
{
	try {
	  $result = [System.Convert]::ToBoolean($value) 
	} catch [FormatException] {
	  $result = $false
	}
	return $result;
}

Function New-VersionObjectFromJson
{
	Param(
		[string] $jsonPath,
		[int] $revision = 0
	)

	$json = Get-Content $jsonPath | Out-String | ConvertFrom-Json;

	# Validate the version in 'version.json' is valid SemVer
	$r= [System.Text.RegularExpressions.Regex]::Match($json.version, "^[0-9]+(\.[0-9]+){1,2}$")
	if (!$r.Success)
	{
		throw "The version property found in $($jsonPath) must be valid SemVer '[MAJOR].[MINOR].[PATCH]'  Value found was $($json.version)";
	}

	$rawversion = $json.version;
	
	$nuspecVersion = $rawversion;
	if ($json.releaseComment -ne "")
	{
		$nuspecVersion = "$($nuspecVersion)-$($json.releaseComment)";
		if ($revision -ne "0") { $nuspecVersion = "$($nuspecVersion).$($revision)"; }
	}

	$version = New-Object -TypeName PSObject;
	Add-Member -InputObject $version -MemberType NoteProperty -Name RawVersion -Value $rawversion;
	Add-Member -InputObject $version -MemberType NoteProperty -Name ReleaseComment -Value $json.releaseComment;
	Add-Member -InputObject $version -MemberType NoteProperty -Name FileVersion -Value "$($rawversion).$($revision)";
	Add-Member -InputObject $version -MemberType NoteProperty -Name NuSpecVersion -Value $nuspecVersion;

	return $version;
}

Function Get-SolutionVersionFromConfig
{
	Param(
		[string] $buildDir,
		[int] $revision = 0
	)

	if (!$buildDir.EndsWith("\")) { $buildDir = "$($buildDir)\" };
	$versionFileFullPath = "$($buildDir)$($VersionFileName)";

	return New-VersionObjectFromJson $versionFileFullPath $revision;
	
}

Export-ModuleMember -Function Get-SolutionVersionFromConfig