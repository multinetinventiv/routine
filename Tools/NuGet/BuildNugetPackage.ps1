$nugetPath = resolve-path ".\Tools\NuGet\NuGet.exe"
$publishPath = resolve-path ".\Routine\Routine.csproj"
$outputDirectory = ".\Artifacts"

if(Test-Path .\Artifacts) 
{
	Remove-Item .\Artifacts -Force -Recurse 
}

function CreateNugetPackage(){
      
	Write-Output "Nuget package is creating..."
	
	New-Item -ItemType Directory -Force -Path $outputDirectory\Artifacts

	$executionQuery = "& $nugetpath pack $publishPath -Verbose -OutputDirectory $outputDirectory -Build -Properties Configuration=Release"

	Invoke-Expression $executionQuery

	Write-Output "Nuget package is created."
}

CreateNugetPackage