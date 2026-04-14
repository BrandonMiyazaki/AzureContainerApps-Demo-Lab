# Deploy to Azure

Now let's run the same app in the cloud. We'll use **Azure Container Apps (ACA)** — a managed service that runs your containers without you managing any servers.

## What gets created in Azure

The Bicep templates in `infra/` will create:

| Resource | Purpose |
|----------|---------|
| **Virtual Network (VNet)** | Private network for all resources |
| **Azure SQL Database** | Stores customers, products, orders |
| **Azure Container Registry (ACR)** | Stores your container images (like a private Docker Hub) |
| **Azure Key Vault** | Stores secrets and configuration |
| **Container Apps Environment** | Runs your 4 container apps |
| **Managed Identity** | Allows apps to authenticate to Azure services without passwords |
| **Log Analytics** | Collects logs and metrics |

---

## Step 1: Log in to Azure

```bash
az login
```

This opens a browser window. Sign in with your Azure account. Then verify:

```bash
az account show --query "{name:name, id:id}" -o table
```

You should see your subscription name and ID. If you have multiple subscriptions, select the right one:

```bash
az account set --subscription "<subscription-name-or-id>"
```

## Step 2: Create a resource group

A resource group is a folder that holds related Azure resources. All our stuff goes here.

```bash
# Pick a name and region — these are examples
az group create --name rg-retail-demo --location westus2
```

> **Choosing a region:** Pick one close to you for lower latency. Common choices: `westus2`, `eastus`, `eastus2`, `centralus`. Run `az account list-locations -o table` to see all options.

## Step 3: Configure your deployment parameters

Open `infra/main.bicepparam` in your editor. Fill in the placeholder values:

```
param location = 'westus2'                // The region you chose above
param baseName = 'contoso-retail'          // A unique name (lowercase, hyphens OK)
```

> **The `baseName` matters!** It's used to name all your Azure resources (e.g., `sql-contoso-retail`, `acrcontosoretail`). ACR names must be globally unique and alphanumeric only — the Bicep template strips hyphens automatically.

Now set yourself as the SQL Server admin. Run these commands to get your Entra ID details:

```bash
# Get your Object ID
az ad signed-in-user show --query id -o tsv

# Get your UPN (email)
az ad signed-in-user show --query userPrincipalName -o tsv
```

Paste the values into `main.bicepparam`:

```
param sqlAdminObjectId = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx'   // Your Object ID
param sqlAdminLogin = 'you@yourdomain.com'                         // Your UPN
```

## Step 4: Deploy the infrastructure

This creates all the Azure resources using the Bicep templates:

```bash
az deployment group create \
  --resource-group rg-retail-demo \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

> **This takes 10-15 minutes.** It's creating a VNet, SQL Server, Container Registry, Key Vault, Log Analytics, the Container Apps Environment, and the 4 container apps. The container apps will fail initially because we haven't pushed any images yet — that's expected.

You can watch progress in a separate terminal:

```bash
az deployment group list \
  --resource-group rg-retail-demo \
  --query "[].{name:name, state:properties.provisioningState}" \
  -o table
```

## Step 5: Build and push container images

The container images need to be stored in your Azure Container Registry (ACR) before the container apps can run them.

First, temporarily enable public access on ACR (it's locked down by default):

```bash
# Get your ACR name from the deployment output
ACR_NAME=$(az deployment group show \
  --resource-group rg-retail-demo \
  --name main \
  --query "properties.outputs.acrName.value" -o tsv)

echo "Your ACR name is: $ACR_NAME"

# Enable public access so we can push images
az acr update --name $ACR_NAME --public-network-enabled true
```

Now build and push all 4 images. `az acr build` uploads your source code to Azure and builds the container image there — you don't need Docker running locally for this step:

```bash
az acr build --registry $ACR_NAME --image orders-api:latest src/OrdersApi/
az acr build --registry $ACR_NAME --image frontend:latest src/Frontend/
az acr build --registry $ACR_NAME --image data-generator:latest src/DataGenerator/
az acr build --registry $ACR_NAME --image inventory-service:latest src/InventoryService/
```

> **Each build takes ~1 minute.** You'll see build logs streaming in your terminal. Look for "Run ID: xxx was successful" at the end.

## Step 6: Redeploy to start the container apps

Now that the images exist in ACR, redeploy the Bicep template. This time the container apps will start successfully:

```bash
az deployment group create \
  --resource-group rg-retail-demo \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

## Step 7: Lock down ACR

Now that the images are pushed, disable public access on ACR. The container apps pull images through the private endpoint inside the VNet:

```bash
az acr update --name $ACR_NAME --public-network-enabled false
```

## Step 8: Grant the managed identity database access

Your user account is the SQL Server admin, but the container apps run as a **managed identity**. You need to grant that identity read/write access to the database.

First, get the managed identity name:

```bash
MI_NAME=$(az deployment group show \
  --resource-group rg-retail-demo \
  --name main \
  --query "properties.outputs.identityClientId.value" -o tsv)

echo "Managed Identity Client ID: $MI_NAME"
```

Now temporarily enable public access on SQL Server so you can connect:

```bash
SQL_SERVER=$(az deployment group show \
  --resource-group rg-retail-demo \
  --name main \
  --query "properties.outputs.sqlServerFqdn.value" -o tsv)

# Allow Azure services to connect
az sql server update \
  --name sql-$(az deployment group show --resource-group rg-retail-demo --name main --query "properties.outputs.sqlServerFqdn.value" -o tsv | cut -d. -f1 | sed 's/sql-//') \
  --resource-group rg-retail-demo \
  --enable-public-network true

az sql server firewall-rule create \
  --resource-group rg-retail-demo \
  --server $(az sql server list --resource-group rg-retail-demo --query "[0].name" -o tsv) \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

Connect to the database using the Azure CLI and run the SQL grant commands. Replace `<identity-name>` with the managed identity name (it follows the pattern `id-<baseName>`, e.g., `id-contoso-retail`):

```bash
# Get the identity name
IDENTITY_NAME=$(az identity list \
  --resource-group rg-retail-demo \
  --query "[0].name" -o tsv)

echo "Identity name: $IDENTITY_NAME"

# Connect and grant permissions
SQL_SERVER_NAME=$(az sql server list \
  --resource-group rg-retail-demo \
  --query "[0].name" -o tsv)

az sql db execute \
  --resource-group rg-retail-demo \
  --server-name $SQL_SERVER_NAME \
  --database-name RetailDb \
  --query-text "
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '$IDENTITY_NAME')
    BEGIN
      CREATE USER [$IDENTITY_NAME] FROM EXTERNAL PROVIDER;
    END;
    ALTER ROLE db_datareader ADD MEMBER [$IDENTITY_NAME];
    ALTER ROLE db_datawriter ADD MEMBER [$IDENTITY_NAME];
    ALTER ROLE db_ddladmin ADD MEMBER [$IDENTITY_NAME];
  "
```

> **What this does:** Creates a database user for the managed identity and grants it `db_datareader` (SELECT), `db_datawriter` (INSERT, UPDATE, DELETE), and `db_ddladmin` (CREATE/ALTER TABLE — needed for EF Core migrations on startup). The container apps use this identity to authenticate to SQL — no passwords needed.

Finally, lock down SQL Server public access again:

```bash
az sql server update \
  --name $SQL_SERVER_NAME \
  --resource-group rg-retail-demo \
  --enable-public-network false

az sql server firewall-rule delete \
  --resource-group rg-retail-demo \
  --server $SQL_SERVER_NAME \
  --name AllowAzureServices
```

## Step 9: Get your app URL

```bash
az deployment group show \
  --resource-group rg-retail-demo \
  --name main \
  --query "properties.outputs.frontendFqdn.value" -o tsv
```

Open the URL in your browser (add `https://` in front). You should see the same retail dashboard, now running in Azure!

> **Give it a minute.** The Data Generator runs a bulk seed on first startup — it creates 500 customers and 10,000 orders with dates spanning 10 years. This takes a few minutes to complete.

> **Stop the Data Generator when you're done seeding.** After the initial data is created, the Data Generator continues running and creates new customers and orders every 30 seconds. To stop it, scale it to zero replicas:
>
> ```bash
> az containerapp update --name ca-data-generator --resource-group rg-retail-demo --min-replicas 0 --max-replicas 0
> ```
>
> To start it again later, set `--min-replicas 1 --max-replicas 1`.

## Step 10: Verify everything is running

```bash
# Check all container apps
az containerapp list \
  --resource-group rg-retail-demo \
  --query "[].{name:name, status:properties.provisioningState}" \
  -o table

# Check logs for a specific app
az containerapp logs show \
  --name ca-data-generator \
  --resource-group rg-retail-demo \
  --follow
```

---

[← Project Structure](02-project-structure.md) · [Next: Clean Up & Troubleshooting →](04-cleanup-and-troubleshooting.md)
