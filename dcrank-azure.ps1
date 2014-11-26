param([string]$Prefix, [string]$AzurePublishSettingsFile, [string]$StorageAccountName, [string]$Location, [string]$NumberOfAgents)

Import-AzurePublishSettingsFile $AzurePublishSettingsFile

$AffinityGroupName = $Prefix + "-dcrank-group"
New-AzureAffinityGroup -Name $AffinityGroupName -Location $Location

cd .\azure-packages\CrankService
$ServiceName = $Prefix + "CrankService"

[Environment]::CurrentDirectory = $(Get-Location)

$path = '.\ServiceDefinition.csdef'
$xml = [xml](Get-Content $path)
$node = $xml.ServiceDefinition.WorkerRole.Runtime.EntryPoint.ProgramEntryPoint
$node.commandLine = 'crank.exe /Mode:agent /ControllerUrl:http://'+ $ServiceName.ToLower() +'.cloudapp.net/'
$xml.Save($path);

<#
$path = '.\ServiceConfiguration.Cloud.cscfg'
$xml = [xml](Get-Content $path)
$node = $xml.ServiceConfiguration.Role.Instances
$node.count = $NumberOfAgents
$xml.Save('.\ServiceConfiguration.Cloud.cscfg');
#>

Write-Output $(Get-Location)
Publish-AzureServiceProject -ServiceName $ServiceName -AffinityGroup $AffinityGroupName -StorageAccountName $StorageAccountName
cd ..
cd ..
