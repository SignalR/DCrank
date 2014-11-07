param([string]$AzurePublishSettingsFile, [string]$AffinityGroupName, [string]$StorageAccountName, [string]$ServiceName)
Import-AzurePublishSettingsFile $AzurePublishSettingsFile
Publish-AzureServiceProject -ServiceName $ServiceName -AffinityGroup $AffinityGroupName -StorageAccountName $StorageAccountName


