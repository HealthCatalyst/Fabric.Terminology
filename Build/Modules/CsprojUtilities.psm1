<#
	.Synopsis
	Utility functions for reading and updating information in .csproj files.	
#>

Function Get-ProjectFileXml([string] $projFile) 
{
	[xml]$projXml = Get-Content -Path $projFile;
	return $projXml;
}

Function Get-ProjectFilesForSolution([string] $slnDir)
{
	return Get-ChildItem -Path $slnDir -Filter *.csproj -Recurse -File | ForEach-Object { $_.FullName }
}

Function Get-ProjectFilesExcludingTestsForSolution([string] $slnDir)
{
	return Get-ChildItem -Path $slnDir -Filter *.csproj -Recurse -File | ? { $_.Name -notmatch 'Test' } | ForEach-Object { $_.FullName }
}


Function Set-ProjectAssemblyInfoVersionsAndEnsureDefaults([string] $projFile, [object]$version) 
{
	$tmpFile = $projFile + ".tmp";
	$proj = Get-ProjectFileXml $projFile;

	# Find the TargetFramework node's parent Property group to keep things organized.
	$targetFramework = $proj.GetElementsByTagName("TargetFramework") | Select-Object -First 1;
	$propGroup = $targetFramework.ParentNode;	

	# set FileVersion
	if (!$propGroup.FileVersion) 
	{
		$fileVersion = $proj.CreateNode("element", "FileVersion", "");
		$propGroup.AppendChild($fileVersion);
	}
	$propGroup.FileVersion = $version.FileVersion;

	#set 
	if (!$propGroup.AssemblyVersion)
	{
		$assemblyVersion = $proj.CreateNode("element", "AssemblyVersion", "");
		$propGroup.AppendChild($assemblyVersion);
	}
	$propGroup.AssemblyVersion = $version.FileVersion;

	if (!$propGroup.Version)
	{
		$ngVersion = $proj.CreateNode("element", "Version", "");
		$propGroup.AppendChild($ngVersion);
	}
	$propGroup.Version = $version.NuSpecVersion;

	if (!$propGroup.Copyright)
	{
		$copy = $proj.CreateNode("element", "Copyright", "");
		$propGroup.AppendChild($copy);
	}
	$copyYear = Get-Date -Format yyyy;
	$propGroup.Copyright = "Copyright $($copyYear) $([char]0x00A9) Health Catalyst";
	
	$proj.save($tmpFile)
	Move-Item $tmpFile $projFile -Force;
}



Export-ModuleMember -Function Get-SolutionPaths
Export-ModuleMember -Function Get-ProjectFilesExcludingTestsForSolution
Export-ModuleMember -Function Set-ProjectAssemblyInfoVersionsAndEnsureDefaults