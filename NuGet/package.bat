nuget pack ..\Routine\Routine.csproj -Verbose -OutputDirectory Publish -Build -Properties Configuration=Release
nuget push Publish\*.nupkg -source http://nuget.multinet.com.tr/ -ConfigFile publish.config

IF EXIST Publish GOTO PublishExists
MD Publish

:PublishExists

IF EXIST PublishBackUp GOTO PublishBackUpExists
MD PublishBackUp

:PublishBackUpExists

xcopy /y Publish\*.nupkg PublishBackUp
del Publish\*.nupkg