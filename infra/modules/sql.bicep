@description('Azure region for all resources')
param location string

@description('Base name used for generating resource names')
param baseName string

@description('Private endpoints subnet ID')
param privateEndpointsSubnetId string

@description('SQL private DNS zone ID')
param sqlDnsZoneId string

@description('Principal ID of the managed identity for Entra admin')
param managedIdentityPrincipalId string

@description('Name of the managed identity for Entra admin')
param managedIdentityName string

@description('Object ID of the deploying user (Entra admin for SQL Server)')
param sqlAdminObjectId string

@description('Login name of the deploying user (email or UPN)')
param sqlAdminLogin string

@description('Log Analytics workspace ID for diagnostics')
param logAnalyticsWorkspaceId string

@description('SQL Database SKU name')
param skuName string = 'Basic'

@description('SQL Database DTU capacity')
param skuCapacity int = 5

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: 'sql-${baseName}'
  location: location
  properties: {
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Disabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: sqlAdminLogin
      sid: sqlAdminObjectId
      principalType: 'User'
      tenantId: tenant().tenantId
    }
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: 'RetailDb'
  location: location
  sku: {
    name: skuName
    capacity: skuCapacity
  }
  properties: {}
}

// Private endpoint
resource sqlPe 'Microsoft.Network/privateEndpoints@2024-05-01' = {
  name: 'pe-sql-${baseName}'
  location: location
  properties: {
    subnet: {
      id: privateEndpointsSubnetId
    }
    privateLinkServiceConnections: [
      {
        name: 'sql-connection'
        properties: {
          privateLinkServiceId: sqlServer.id
          groupIds: [
            'sqlServer'
          ]
        }
      }
    ]
  }
}

resource sqlPeDnsGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2024-05-01' = {
  parent: sqlPe
  name: 'sql-dns-group'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'privatelink-database-windows-net'
        properties: {
          privateDnsZoneId: sqlDnsZoneId
        }
      }
    ]
  }
}

// Auditing via Log Analytics
resource sqlAudit 'Microsoft.Sql/servers/auditingSettings@2023-08-01-preview' = {
  parent: sqlServer
  name: 'default'
  properties: {
    state: 'Enabled'
    isAzureMonitorTargetEnabled: true
  }
}

resource sqlDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'sql-diagnostics'
  scope: sqlDb
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'SQLSecurityAuditEvents'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'Basic'
        enabled: true
      }
    ]
  }
}

output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output sqlDbName string = sqlDb.name
