# Azure Container Apps Demo Lab — Step-by-Step Instructions

Welcome! This guide walks you through running the Miyazaki Retail application — first on your own machine, then deployed to Azure. No prior experience with containers is needed. We'll explain every step.

> **What you'll build:** A multi-service retail app with a web dashboard, a REST API, a fake-data generator, and an inventory tracker — all running in containers.

---

## What are containers?

A **container** is a lightweight, portable package that bundles your application code along with everything it needs to run (runtime, libraries, config). Think of it like a shipping container — it works the same way no matter where you run it (your laptop, a server, the cloud).

**Docker** is the tool that builds and runs containers. **Docker Compose** lets you run multiple containers together (like our 4 services + a database) with a single command.

**Azure Container Apps (ACA)** is a managed cloud service that runs your containers in Azure without you having to manage servers.

---

## Prerequisites

Install these before starting:

| Tool | What it does | Install link |
|------|-------------|-------------|
| **.NET SDK 9.0+** | Builds and runs the C# projects | [Download](https://dotnet.microsoft.com/download) |
| **Docker Desktop** | Runs containers on your machine | [Download](https://www.docker.com/products/docker-desktop) |
| **Azure CLI** | Manages Azure resources from the command line | [Download](https://learn.microsoft.com/cli/azure/install-azure-cli) |
| **Git** | Clones the repo | [Download](https://git-scm.com/downloads) |

You'll also need:
- A free [Azure account](https://azure.microsoft.com/free/)
- A code editor (we recommend [VS Code](https://code.visualstudio.com/))

### Verify your tools

Open a terminal and run these commands. Each should print a version number:

```bash
dotnet --version      # Should show 9.x or higher
docker --version      # Should show Docker version 2x.x
az --version          # Should show azure-cli 2.x
git --version         # Should show git version 2.x
```

> **Troubleshooting:** If any command says "not found", revisit the install link above. On Windows, you may need to restart your terminal after installing.

---

## Phase 1 — Run Locally with Docker Compose

This is the fastest way to see the full app running. Docker Compose will start a SQL Server database, the Orders API, the Frontend, and both worker services — all with one command.

### Step 1: Clone the repository

```bash
git clone https://github.com/<your-username>/AzureContainerApps-Retail-Example.git
cd AzureContainerApps-Retail-Example
```

### Step 2: Create your environment file

The app needs a SQL Server password. We provide a template — you just need to copy it:

```bash
# On Mac/Linux:
cp .env.example .env

# On Windows (Command Prompt):
copy .env.example .env

# On Windows (PowerShell):
Copy-Item .env.example .env
```

Open `.env` in your editor. You'll see:

```
SA_PASSWORD=YourStr0ngP@ssword!
```

You can leave the default or change it. The password must meet [SQL Server complexity requirements](https://learn.microsoft.com/en-us/sql/relational-databases/security/password-policy) (8+ characters, uppercase, lowercase, number, special character).

> **Important:** The `.env` file is git-ignored — it will **never** be committed to your repo. This is intentional for security.

### Step 3: Make sure Docker Desktop is running

Look for the Docker whale icon in your system tray (Windows) or menu bar (Mac). If it's not there, open Docker Desktop and wait for it to say "Docker is running."

> **First time?** Docker Desktop may take a minute to start. On Windows, it may ask you to enable WSL 2 — follow the prompts.

### Step 4: Start everything

From the repo root folder, run:

```bash
docker compose up --build
```

**What's happening:**
1. Docker reads the `docker-compose.yml` file
2. It **builds** a container image for each service (Orders API, Frontend, Data Generator, Inventory Service) using their `Dockerfile`
3. It **pulls** a SQL Server image from Microsoft's container registry
4. It starts all 5 containers and connects them on a private network
5. The Orders API waits for SQL Server to be healthy, then runs database migrations (creates tables and seed data)
6. The Data Generator starts creating fake customers and orders
7. The Inventory Service starts tracking stock levels

> **First run takes 2-5 minutes** — it needs to download base images (~500 MB) and build the projects. Subsequent runs are much faster because Docker caches the layers.

You'll see logs scrolling from all services. Look for:
- `Orders API is ready` — the API started successfully
- `Data Generator starting` — fake data generation is running
- `Inventory Service starting` — stock tracking is active

### Step 5: Open the app

Open your browser and go to: **http://localhost:8080**

You should see the retail dashboard with:
- **Dashboard** — summary cards (orders, revenue, customers, low-stock items)
- **Orders** — click any order to see its line items
- **Products** — product catalog with stock levels
- **Customers** — click any customer to expand their order history
- **New Order** — create an order manually

> **Not loading?** Wait 30 seconds — the Data Generator needs time to create initial data. Refresh the page.

### Step 6: Stop everything

Press `Ctrl+C` in the terminal where Docker Compose is running. Then clean up:

```bash
docker compose down
```

This stops and removes the containers. Your source code is untouched.

> **Want a fresh start?** Run `docker compose down -v` to also remove the database volume. Next time you start up, the database will be re-created from scratch.

---

## Phase 2 — Understand the Project Structure

Before deploying to Azure, let's understand what we're working with:

```
├── src/
│   ├── OrdersApi/          # REST API — connects to SQL, serves data
│   │   └── Dockerfile      # Instructions to build this service's container
│   ├── Frontend/           # Blazor web dashboard — calls the Orders API
│   │   └── Dockerfile
│   ├── DataGenerator/      # Background worker — creates fake data
│   │   └── Dockerfile
│   └── InventoryService/   # Background worker — tracks stock levels
│       └── Dockerfile
├── infra/                  # Bicep files — defines Azure infrastructure
│   ├── main.bicep          # Orchestrates all modules
│   ├── main.bicepparam     # Your deployment parameters (region, names)
│   └── modules/            # One file per Azure resource type
├── docker-compose.yml      # Runs everything locally
├── .env.example            # Template for local passwords
└── .github/workflows/      # CI/CD pipeline (optional)
```

**Key concept — each service is independent:**
- Each has its own `Dockerfile` (build instructions)
- They communicate over HTTP (the API URL is configured via environment variables)
- In Docker Compose, they share a private network
- In Azure, they'll share a Container Apps Environment

---

## Phase 3 — Deploy to Azure

Now let's run the same app in the cloud. We'll use **Azure Container Apps** — a managed service that runs containers without you managing any servers.

### What gets created in Azure

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

### Step 1: Log in to Azure

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

### Step 2: Create a resource group

A resource group is a container that holds related Azure resources. All our stuff goes here.

```bash
# Pick a name and region — these are examples
az group create --name rg-retail-demo --location westus2
```

> **Choosing a region:** Pick one close to you for lower latency. Common choices: `westus2`, `eastus`, `eastus2`, `centralus`. Run `az account list-locations -o table` to see all options.

### Step 3: Configure your deployment parameters

Open `infra/main.bicepparam` in your editor. Fill in the placeholder values:

```
param location = 'westus2'                // The region you chose above
param baseName = 'contoso-retail'          // A unique name (lowercase, hyphens OK)
```

> **The `baseName` matters!** It's used to name all your Azure resources (e.g., `sql-contoso-retail`, `acr-contoso-retail`). ACR names must be globally unique and alphanumeric only — the Bicep template strips hyphens automatically.

### Step 4: Deploy the infrastructure

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

### Step 5: Build and push container images

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

### Step 6: Redeploy to start the container apps

Now that the images exist in ACR, redeploy the Bicep template. This time the container apps will start successfully:

```bash
az deployment group create \
  --resource-group rg-retail-demo \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

### Step 7: Lock down ACR

Now that the images are pushed, disable public access on ACR. The container apps pull images through the private endpoint inside the VNet:

```bash
az acr update --name $ACR_NAME --public-network-enabled false
```

### Step 8: Get your app URL

```bash
az deployment group show \
  --resource-group rg-retail-demo \
  --name main \
  --query "properties.outputs.frontendFqdn.value" -o tsv
```

Open the URL in your browser (add `https://` in front). You should see the same retail dashboard, now running in Azure!

> **Give it a minute.** The Data Generator runs a bulk seed on first startup — it creates 500 customers and 10,000 orders with dates spanning 10 years. This takes a few minutes to complete.

### Step 9: Verify everything is running

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

## Clean Up

When you're done, delete the resource group to avoid charges:

```bash
az group delete --name rg-retail-demo --yes --no-wait
```

This deletes **everything** in the resource group (VNet, SQL, ACR, Key Vault, all container apps). The `--no-wait` flag returns immediately — deletion happens in the background.

---

## Troubleshooting

### Docker Compose won't start

| Problem | Solution |
|---------|----------|
| "Docker daemon is not running" | Open Docker Desktop and wait for it to start |
| "port 1433 already in use" | Another SQL Server is running. Stop it or change the port in `docker-compose.yml` |
| "password validation failed" | Your `SA_PASSWORD` in `.env` doesn't meet complexity requirements |
| Build fails with "SDK not found" | Make sure you're using the correct .NET SDK version (check `Dockerfile` for the version) |

### Azure deployment issues

| Problem | Solution |
|---------|----------|
| "container app failed to provision" | Images don't exist in ACR yet. Push images (Step 5) then redeploy (Step 6) |
| "ACR access denied during build" | ACR public access may not have propagated yet. Wait 30 seconds and retry |
| "SQL connection failed" | The managed identity needs a few minutes to propagate after deployment |
| Container app shows 0 replicas | Check logs: `az containerapp logs show --name <app> --resource-group <rg>` |

### Frontend shows no data

- The Data Generator needs time to seed (500 customers + 10,000 orders). Check its logs.
- Make sure the Orders API is running — check its health endpoint or logs.
- The Frontend connects to the API via the `API_BASE_URL` environment variable. Verify it's set correctly in the container app configuration.

---

## Next Steps

Once everything is running, try:

1. **Create an order manually** — Use the "New Order" page in the Frontend
2. **Explore the API** — The Orders API has OpenAPI/Swagger in development mode
3. **Check the infrastructure** — Browse the Azure Portal to see all the resources that were created
4. **Set up CI/CD** — See `.github/workflows/build-deploy.yml` for a GitHub Actions pipeline that auto-deploys on push to `main`
5. **Connect Power BI** — Use Microsoft Fabric to mirror the Azure SQL database and build analytics dashboards
