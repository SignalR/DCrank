param([string]$Prefix, [string]$AzurePublishSettingsFile, [string]$StorageAccountName, [string]$Location, [string]$NumberOfAgents)

Import-AzurePublishSettingsFile $AzurePublishSettingsFile

$AffinityGroupName = $Prefix + "-drank-group"
New-AzureAffinityGroup -Name $AffinityGroupName -Location $Location

cd .\azure-packages\CrankDashboardServiceProject\
$ServiceName = $Prefix + "CrankDashboardService"
Publish-AzureServiceProject -ServiceName $ServiceName -AffinityGroup $AffinityGroupName -StorageAccountName $StorageAccountName
cd ..

cd .\CrankWorkerServiceProject\
$path = '.\ServiceDefinition.csdef'
$xml = [xml](Get-Content $path)
$node = $xml.ServiceDefinition.WorkerRole.Runtime.EntryPoint.ProgramEntryPoint
$node.commandLine = 'crank.exe /Mode:agent /ControllerUrl:http://'+ $ServiceName.ToLower() +'.cloudapp.net/'
$xml.Save('C:\Users\abnanda\Desktop\DCrank\azure-packages\CrankWorkerServiceProject\ServiceDefinition.csdef');

$path = '.\ServiceConfiguration.Cloud.cscfg'
$xml = [xml](Get-Content $path)
$node = $xml.ServiceConfiguration.Role.Instances
$node.count = $NumberOfAgents
$xml.Save('C:\Users\abnanda\Desktop\DCrank\azure-packages\CrankWorkerServiceProject\ServiceConfiguration.Cloud.cscfg');

$ServiceName = $Prefix + "CrankWorkerService"
Publish-AzureServiceProject -ServiceName $ServiceName -AffinityGroup $AffinityGroupName -StorageAccountName $StorageAccountName
