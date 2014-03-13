nuget pack ..\Routine\Routine.csproj -Verbose -OutputDirectory Publish -Build -Properties Configuration=Release
nuget push Publish\*.nupkg -source http://nuget.multinet.com.tr/ -ConfigFile publish.config
xcopy /y Publish\*.nupkg PublishBackUp\
del Publish\*.nupkg