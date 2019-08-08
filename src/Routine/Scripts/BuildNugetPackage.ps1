$id = "Routine"
$projectName = "Routine"
$folderName = "Scripts"

$libFiles = New-Object System.Collections.ArrayList
$libFiles.Add(@{ assemblyName = "Routine.dll"})

Set-Location $PSScriptRoot
..\..\..\tools\Nuget\BuildNugetPackage.ps1 -id $id -folderName $folderName -projectName $projectName -libFiles $libFiles