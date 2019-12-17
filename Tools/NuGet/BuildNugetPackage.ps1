param(
    [Parameter(Mandatory=$true)][string]$id,    
    [string]$folderName = "",    
    [string]$projectName,
    [string]$version,
    [string]$title,
    [string]$author,
    [string]$iconUrl = "https://github.com/multinetinventiv/routine/blob/feature/71368-routine-nuget-deployment/docs/routine-logo.png",
    [string]$requireLicenseAcceptance = "true",
    [string]$licenseUrl = "http://routineframework.org",
    [string]$description,
    [string]$copyright,  
    [System.Collections.ArrayList]$assemblyNames,
    [System.Collections.ArrayList]$contentFiles,
    [System.Collections.ArrayList]$libFiles,
    [System.Collections.ArrayList]$files
)
	
$ErrorActionPreference = "Stop"
$nuspecFileEmptyContent = "<?xml version=""1.0""?><package><metadata><id></id><version></version><title></title><authors></authors><owners></owners><iconUrl></iconUrl><requireLicenseAcceptance></requireLicenseAcceptance><licenseUrl></licenseUrl><projectUrl></projectUrl><description></description><copyright></copyright><frameworkAssemblies></frameworkAssemblies></metadata><files></files></package>"
$nuspecFileName = "$PSScriptRoot\" + ([guid]::newguid()).ToString() + ".nuspec"
$projectPath = ($MyInvocation.PSScriptRoot).ToString().Replace("\$folderName","")
$projectAssemblyInfoPath = "$projectPath\Properties\AssemblyInfo.cs"
$projectUrl = "http://routineframework.org"
$dotnetExePath = "dotnet.exe"
$publishFolderPath = ".\..\..\artifacts"

$targetFrameworks = New-Object System.Collections.ArrayList
$targetFrameworks.Add(@{name = ".NETFramework4.5.2"; shortName = "net452"})
$targetFrameworks.Add(@{name = ".NETFramework4.6"; shortName = "net46"})
$targetFrameworks.Add(@{name = ".NETFramework4.7"; shortName = "net47"})
$targetFrameworks.Add(@{name = ".NETFramework4.8"; shortName = "net48"})


#region Function: SetUp()
function SetUp(){
    Set-Location $PSScriptRoot
}

#endregion

#region Function: GetValueFromAssemblyInfo($key)
function GetValueFromAssemblyInfo($key){

    $content = Get-Content $projectAssemblyInfoPath

    foreach ($line in $content){
        if($line -like '*assembly: Assembly*'){
            $lineParts = $line.replace('[assembly: Assembly','').replace('")]','') -split '\("';
            
                if( $lineParts[0] -eq $key){
                    return $lineParts[1]
                }
        }
    }
}
#endregion

#region Function: GetTargetFrameworksInline
function GetTargetFrameworksInline(){
    $targetFrameworkInline = ""

    foreach($targetFramework in $targetFrameworks){    
        if($targetFrameworkInline -eq ""){
            $targetFrameworkInline += $targetFramework.name
        }
        else
        {
            $targetFrameworkInline += ", "+$targetFramework.name
        }
    }

    return $targetFrameworkInline
}

#endregion

#region Function: CreateNuspecFile
function CreateNuspecFile(){

    Write-Host "Nuspec file..." -ForegroundColor Cyan
    Write-Host "Creating..." -ForegroundColor Cyan

    New-Item -Path $nuspecFileName -Value $nuspecFileEmptyContent -ItemType File

    Write-Host "Created" -ForegroundColor Cyan
}

#endregion

#region Function: SetContent2Node($nodeName,$content)
function SetContent2Node($nodeName,$content){
    [xml]$xml = Get-Content $nuspecFileName
    
    $xml.SelectSingleNode("//$nodeName").InnerText = $content

    $xml.OuterXml | Out-File $nuspecFileName -encoding "UTF8"
}

#endregion

#region Function: AddFrameworkAssemblyNode($assemblyName)
function AddFrameworkAssemblyNode($assemblyName){

    [xml]$xml = Get-Content $nuspecFileName
    
    $targetFrameworkInline = GetTargetFrameworksInline

    $frameworkAssembly = $xml.CreateElement("frameworkAssembly")
    $frameworkAssembly.SetAttribute("assemblyName",$assemblyName)
    $frameworkAssembly.SetAttribute("targetFramework",$targetFrameworkInline)

    $xml.SelectSingleNode("//frameworkAssemblies").AppendChild($frameworkAssembly)

    $xml.OuterXml | Out-File $nuspecFileName -encoding "UTF8"
}

#endregion

#region Function: AddContentFileNode($assemblyName,$targetFolder)
function AddContentFileNode($assemblyName,$targetFolder,$inRootFolder){
    [xml]$xml = Get-Content $nuspecFileName
    $targetFolder += (&{If($targetFolder) {"\"} Else {""}})    
    
    foreach($targetFramework in $targetFrameworks){        
        $targetFrameworkShortName = $targetFramework.shortName
        $assemblyPath = (&{If($inRootFolder -eq "true") {"$projectPath\$assemblyName"} Else {"$projectPath\bin\Release\$targetFrameworkShortName\$assemblyName"}})

        $file = $xml.CreateElement("file")
        $file.SetAttribute("src",$assemblyPath)
        $file.SetAttribute("target","content\$targetFrameworkShortName\$targetFolder$assemblyName")

        $xml.SelectSingleNode("//files").AppendChild($file)    
    }

    $xml.OuterXml | Out-File $nuspecFileName -encoding "UTF8"
}

#endregion

#region Function: AddLibFileNode($assemblyName)
function AddLibFileNode($assemblyName){
    [xml]$xml = Get-Content $nuspecFileName
    
    foreach($targetFramework in $targetFrameworks){        

        $targetFrameworkShortName = $targetFramework.shortName
        $file = $xml.CreateElement("file")
        $file.SetAttribute("src","$projectPath\bin\Release\$targetFrameworkShortName\$assemblyName")
        $file.SetAttribute("target","lib\$targetFrameworkShortName\$assemblyName")

        $xml.SelectSingleNode("//files").AppendChild($file)
    }

    $xml.OuterXml | Out-File $nuspecFileName -encoding "UTF8"
}

#endregion

#region Function: AddFileNode($assemblyName)
function AddFileNode($fileName){
    [xml]$xml = Get-Content $nuspecFileName
        
        $file = $xml.CreateElement("file")
        $file.SetAttribute("src","$projectPath\$fileName")
        $file.SetAttribute("target","$fileName")

        $xml.SelectSingleNode("//files").AppendChild($file)
    
    $xml.OuterXml | Out-File $nuspecFileName -encoding "UTF8"
}

#endregion 

#region Function: SetNuspecFileContents()
function SetNuspecFileContents(){

	Write-Host "Nuspec file contents..." -ForegroundColor Cyan
    Write-Host "Filling..." -ForegroundColor Cyan

    #Metadata alt�ndaki node'lar�n i�erikleri doldurulur.
    SetContent2Node "id" $id
    SetContent2Node "version" $version
    SetContent2Node "title" $title
    SetContent2Node "authors" $author
    SetContent2Node "iconUrl" $iconUrl
    SetContent2Node "requireLicenseAcceptance" $requireLicenseAcceptance
    SetContent2Node "licenseUrl" $licenseUrl
    SetContent2Node "projectUrl" $projectUrl
    SetContent2Node "description" $description    
    SetContent2Node "copyright" $copyright  

    $assemblyNames.ForEach({AddFrameworkAssemblyNode $_.assemblyName })   
    $contentFiles.ForEach({AddContentFileNode $_.assemblyName $_.targetFolder $_.inRootFolder})
    $libFiles.ForEach({AddLibFileNode $_.assemblyName })
    $files.ForEach({AddFileNode $_.fileName })

    Write-Host "Nuspec file contents are filled." -ForegroundColor Cyan
}

#endregion

#region Function: CreateNugetPackage()
function CreateNugetPackage(){
    
	Write-Host "Nuget package..." -ForegroundColor Cyan
    Write-Host "Creating..." -ForegroundColor Cyan

    $executionQuery = "& $dotnetExePath pack ""$projectPath\$projectName.csproj"" --force -v normal -o ""$publishFolderPath"" -c Release /p:NuspecFile=`"$nuspecFileName`""

    Invoke-Expression $executionQuery

    Write-Host "Created ($publishFolderPath\$id.$version.nupkg)" -ForegroundColor Cyan
}

#endregion

#region Function: DeleteNuspecFile()
function DeleteNuspecFile(){

	Write-Host "Temporarily created nuspec file ($nuspecFileName) is being deleted..." -ForegroundColor Cyan

    Remove-Item -Path $nuspecFileName

	Write-Host "Temporarily created nuspec file ($nuspecFileName) deleted" -ForegroundColor Cyan
}

#endregion

SetUp

$projectName = (&{If($projectName) {$projectName} Else {$id}})

$version = (&{If($version) {$version} Else {GetValueFromAssemblyInfo("InformationalVersion")}})

$title = (&{If($title) {$title} Else {GetValueFromAssemblyInfo("title")}})

$author= (&{If($author) {$author} Else {GetValueFromAssemblyInfo("company")}})

$description= (&{If($description) {$description} Else {GetValueFromAssemblyInfo("description")}})

$copyright = (&{If($copyright) {$copyright} Else {GetValueFromAssemblyInfo("copyright")}})

CreateNuspecFile 

SetNuspecFileContents

CreateNugetPackage

DeleteNuspecFile