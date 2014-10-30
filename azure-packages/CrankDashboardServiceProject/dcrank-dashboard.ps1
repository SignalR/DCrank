param([string]$AzurePublishSettingsFile, [string]$AffinityGroupName, [string]$Location, [string]$StorageAccountName, [string]$ServiceName)
Import-AzurePublishSettingsFile $AzurePublishSettingsFile
#New-AzureAffinityGroup -Name $AffinityGroupName -Location $Location
Publish-AzureServiceProject -ServiceName $ServiceName -AffinityGroup $AffinityGroupName -StorageAccountName $StorageAccountName