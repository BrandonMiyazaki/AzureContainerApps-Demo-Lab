using 'main.bicep'

param location = '<your-azure-region>'       // e.g. 'westus2'
param baseName = '<your-base-name>'            // e.g. 'contoso-retail'

// Networking defaults
param vnetAddressPrefix = '10.31.0.0/16'
param acaSubnetPrefix = '10.31.0.0/23'
param privateEndpointsSubnetPrefix = '10.31.2.0/24'

// SQL — scale up for production
param sqlSkuName = 'Basic'
param sqlSkuCapacity = 5
