@description('Azure region for all resources')
param location string

@description('Base name used for generating resource names')
param baseName string

@description('ACA subnet ID for VNet integration')
param acaSubnetId string

// Log Analytics Workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-${baseName}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// Container Apps Environment (VNet-integrated, internal)
resource acaEnv 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'cae-${baseName}'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    vnetConfiguration: {
      infrastructureSubnetId: acaSubnetId
      internal: false
    }
    peerAuthentication: {
      mtls: {
        enabled: true
      }
    }
  }
}

output logAnalyticsWorkspaceId string = logAnalytics.id
output logAnalyticsWorkspaceName string = logAnalytics.name
output acaEnvironmentId string = acaEnv.id
output acaEnvironmentName string = acaEnv.name
output acaEnvironmentDefaultDomain string = acaEnv.properties.defaultDomain
output acaEnvironmentStaticIp string = acaEnv.properties.staticIp
