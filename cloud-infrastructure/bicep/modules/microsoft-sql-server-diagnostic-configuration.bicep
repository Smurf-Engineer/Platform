param storageAccountName string
param microsoftSqlServerName string
param principalId string
param dianosticStorageAccountSubscriptionId string
param dianosticStorageAccountBlobEndpoint string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  scope: resourceGroup()
  name: storageAccountName
}

@description('This is the built-in Contributor role. See https://docs.microsoft.com/azure/role-based-access-control/built-in-roles#contributor')
resource storageBlobDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  scope: storageAccount
  name: guid(storageAccount.id, principalId, storageBlobDataContributorRoleDefinition.id)
  properties: {
    roleDefinitionId: storageBlobDataContributorRoleDefinition.id
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}

resource microsoftSqlServer 'Microsoft.Sql/servers@2022-05-01-preview' existing = {
  name: microsoftSqlServerName
}

resource microsoftSqlServerOutboundFirewallRules 'Microsoft.Sql/servers/outboundFirewallRules@2022-05-01-preview' = {
  parent: microsoftSqlServer
  name: storageAccountName
  dependsOn: [ roleAssignment ]
}

resource microsoftSqlServerAuditingSettings 'Microsoft.Sql/servers/auditingSettings@2022-05-01-preview' = {
  parent: microsoftSqlServer
  name: 'default'
  properties: {
    retentionDays: 90
    auditActionsAndGroups: [
      'SUCCESSFUL_DATABASE_AUTHENTICATION_GROUP'
      'FAILED_DATABASE_AUTHENTICATION_GROUP'
      'BATCH_COMPLETED_GROUP'
    ]
    isAzureMonitorTargetEnabled: true
    isManagedIdentityInUse: true
    state: 'Enabled'
    storageEndpoint: dianosticStorageAccountBlobEndpoint
    storageAccountSubscriptionId: dianosticStorageAccountSubscriptionId
  }
  dependsOn: [ microsoftSqlServerOutboundFirewallRules ]
}
