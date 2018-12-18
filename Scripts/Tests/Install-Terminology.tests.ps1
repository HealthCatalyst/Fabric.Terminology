$thisFile = Split-Path $MyInvocation.MyCommand.Path -Leaf
$here = Split-Path -Parent $PSScriptRoot

$files = (
    "Install-Terminology.ps1",
    "configuration.manifest",
    "Database\Fabric.Terminology.Database.dacpac",
    "Package\Fabric.Terminology.zip",
    "Metadata\SharedTerminology.json",
    "Metadata\Terminology.json",
    "nuget\Fabric.Terminology.InstallPackage.nuspec",
    "nuget\Fabric.Terminology.InstallPackage.targets"
)

Describe "$thisFile Tests" {
  
    Context "File Setup" {
        foreach ($file in $files) {
            It "$file should exist" {
                "$here\$file" | Should Exist
            }
        }
    }

    foreach ($file in $files | Where-Object {$_ -like "*.psm1" -or $_ -like "*.ps1"}) {
        $childItem = Get-ChildItem "$here\$file"

        Context "Test $($childItem.Name)" {
            
            It "$($childItem.Name) is valid PowerShell" {
                $psFile = Get-Content -Path $childItem.FullName -ErrorAction Stop
                $errors = $null
                $null = [System.Management.Automation.PSParser]::Tokenize($psFile, [ref]$errors)
                $errors.Count | Should Be 0
            }
            
            It "Tests\$($childItem.BaseName).tests.ps1 should exist" {
                "$here\Tests\$($childItem.BaseName).tests.ps1" | Should Exist
            }
            
        }
    }

}