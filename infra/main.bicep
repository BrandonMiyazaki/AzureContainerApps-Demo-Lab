targetScope = 'resourceGroup'

@description('Azure region for all resources')
param location string

@description('Base name used for generating resource names (e.g. "contoso-retail")')
param baseName string

@description('VNet address space')
param vnetAddressPrefix string = '10.31.0.0/16'

@description('ACA subnet address prefix')
param acaSubnetPrefix string = '10.31.0.0/23'

@description('Private endpoints subnet address prefix')
param privateEndpointsSubnetPrefix string = '10.31.2.0/24'

@description('SQL Database SKU name')
param sqlSkuName string = 'Basic'

@description('SQL Database DTU capacity')
param sqlSkuCapacity int = 5

@description('Container image tag')
param imageTag string = 'latest'

@description('Object ID of the deploying user — run: az ad signed-in-user show --query id -o tsv')
param sqlAdminObjectId string

@description('Login name (email/UPN) of the deploying user — run: az ad signed-in-user show --query userPrincipalName -o tsv')
param sqlAdminLogin string

// 1. Managed Identity (created first — referenced by SQL, ACR, Key Vault)
module identity 'modules/identity.bicep' = {
  name: 'identity'
  params: {
    location: location
    baseName: baseName
  }
}

// 2. Networking (VNet, subnets, NSGs, private DNS zones)
module networking 'modules/networking.bicep' = {
  name: 'networking'
  params: {
    location: location
    baseName: baseName
    vnetAddressPrefix: vnetAddressPrefix
    acaSubnetPrefix: acaSubnetPrefix
    privateEndpointsSubnetPrefix: privateEndpointsSubnetPrefix
  }
}

// 3. Log Analytics + ACA Environment (needs subnet, outputs workspace ID for diagnostics)
module logAnalyticsAca 'modules/loganalytics-aca.bicep' = {
  name: 'loganalytics-aca'
  params: {
    location: location
    baseName: baseName
    acaSubnetId: networking.outputs.acaSubnetId
  }
}

// 4. Azure SQL (needs PE subnet, DNS zone, identity, Log Analytics)
module sql 'modules/sql.bicep' = {
  name: 'sql'
  params: {
    location: location
    baseName: baseName
    privateEndpointsSubnetId: networking.outputs.privateEndpointsSubnetId
    sqlDnsZoneId: networking.outputs.sqlDnsZoneId
    sqlAdminObjectId: sqlAdminObjectId
    sqlAdminLogin: sqlAdminLogin
    logAnalyticsWorkspaceId: logAnalyticsAca.outputs.logAnalyticsWorkspaceId
    skuName: sqlSkuName
    skuCapacity: sqlSkuCapacity
  }
}

// 5. ACR (needs PE subnet, DNS zone, identity)
module acr 'modules/acr.bicep' = {
  name: 'acr'
  params: {
    location: location
    baseName: baseName
    privateEndpointsSubnetId: networking.outputs.privateEndpointsSubnetId
    acrDnsZoneId: networking.outputs.acrDnsZoneId
    managedIdentityPrincipalId: identity.outputs.identityPrincipalId
  }
}

// 6. Key Vault (needs PE subnet, DNS zone, identity, Log Analytics)
module keyvault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    location: location
    baseName: baseName
    privateEndpointsSubnetId: networking.outputs.privateEndpointsSubnetId
    kvDnsZoneId: networking.outputs.kvDnsZoneId
    managedIdentityPrincipalId: identity.outputs.identityPrincipalId
    logAnalyticsWorkspaceId: logAnalyticsAca.outputs.logAnalyticsWorkspaceId
  }
}

// 7. Container Apps (OrdersApi, Frontend, DataGenerator, InventoryService)
module containerApps 'modules/container-apps.bicep' = {
  name: 'container-apps'
  params: {
    location: location
    acaEnvironmentId: logAnalyticsAca.outputs.acaEnvironmentId
    acrLoginServer: acr.outputs.acrLoginServer
    identityId: identity.outputs.identityId
    identityClientId: identity.outputs.identityClientId
    sqlServerFqdn: sql.outputs.sqlServerFqdn
    sqlDbName: sql.outputs.sqlDbName
    imageTag: imageTag
  }
}

// Outputs for deployment and Phase 7 reference
output identityId string = identity.outputs.identityId
output identityClientId string = identity.outputs.identityClientId
output acrLoginServer string = acr.outputs.acrLoginServer
output acrName string = acr.outputs.acrName
output sqlServerFqdn string = sql.outputs.sqlServerFqdn
output sqlDbName string = sql.outputs.sqlDbName
output kvName string = keyvault.outputs.kvName
output kvUri string = keyvault.outputs.kvUri
output acaEnvironmentId string = logAnalyticsAca.outputs.acaEnvironmentId
output acaEnvironmentName string = logAnalyticsAca.outputs.acaEnvironmentName
output acaEnvironmentDefaultDomain string = logAnalyticsAca.outputs.acaEnvironmentDefaultDomain
output ordersApiFqdn string = containerApps.outputs.ordersApiFqdn
output frontendFqdn string = containerApps.outputs.frontendFqdn
