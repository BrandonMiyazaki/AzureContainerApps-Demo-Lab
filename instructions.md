# Miyazaki Retail — Lab Instructions

Step-by-step instructions for building and deploying the Miyazaki Retail application on Azure Container Apps.

> **Before you begin:** Make sure you have all [prerequisites](README.md#prerequisites) installed.

---

## Phase 1 — Orders API

Build the core Web API that all other services depend on.

### 1.1 Create the project

```bash
mkdir -p src/OrdersApi
cd src/OrdersApi
dotnet new webapi -n OrdersApi
cd OrdersApi
```

### 1.2 Add NuGet packages

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Azure.Identity
```

### 1.3 Define entity models

Create the following models in a `Models/` folder:

- **Customer** — Id, FirstName, LastName, Email, City, State, CreatedAt
- **Product** — Id, Name, Category, Price, StockQuantity, CreatedAt
- **Order** — Id, CustomerId, OrderDate, TotalAmount, Customer (nav), OrderItems (nav)
- **OrderItem** — Id, OrderId, ProductId, Quantity, UnitPrice, Order (nav), Product (nav)

### 1.4 Create the DbContext

Create `Data/RetailDbContext.cs`:
- Register `DbSet<>` for each entity
- Add seed data in `OnModelCreating` using `HasData` for base Products (with initial stock quantities) and Customers

### 1.5 Create the initial migration

```bash
dotnet ef migrations add InitialCreate
```

### 1.6 Implement API endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/customers` | List all customers |
| POST | `/api/customers` | Create a customer |
| GET | `/api/products` | List all products |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}/stock` | Update stock quantity (for Inventory Service) |
| GET | `/api/orders` | List orders (supports `?since={timestamp}` filter) |
| POST | `/api/orders` | Create an order with line items |
| GET | `/healthz` | Health check |

### 1.7 Configure SQL connection

In `Program.cs`, set up dual connection modes:

- **Local dev:** Read connection string from `appsettings.Development.json` (SQL auth for the local SQL container)
- **Azure:** Use `Azure.Identity` + `DefaultAzureCredential` for managed identity auth (no passwords)

### 1.8 Validate

```bash
dotnet run
# Test endpoints with curl, Postman, or the Swagger UI
```

---

## Phase 2 — Blazor Frontend

Build the retail dashboard UI.

### 2.1 Create the project

```bash
cd src
dotnet new blazor --interactivity Server -n Frontend
```

### 2.2 Add a typed HttpClient

Register a typed `HttpClient` in `Program.cs` for calling the Orders API. The base URL should be configurable via the `API_BASE_URL` environment variable.

### 2.3 Build pages

| Page | Description |
|------|-------------|
| **Dashboard** | Summary cards — total orders, revenue, active customers, low-stock count |
| **Orders** | Table of recent orders with search/filter, click-through to order detail |
| **Products** | Product catalog grid with stock levels (read from Orders API product data) |
| **Customers** | Customer list with order history |

### 2.4 Add security headers

- Enable anti-forgery token protection
- Add Content Security Policy (CSP) headers

### 2.5 Add health check

Add a `/healthz` endpoint for container orchestrator probes.

### 2.6 Validate

```bash
# Start the Orders API first, then:
cd src/Frontend
API_BASE_URL=http://localhost:5000 dotnet run
# Open browser and verify pages render with data
```

---

## Phase 3 — Data Generator

Build the synthetic data generator.

### 3.1 Create the project

```bash
cd src
dotnet new worker -n DataGenerator
cd DataGenerator
dotnet add package Bogus
```

### 3.2 Define Bogus fakers

Create faker classes for:
- `CustomerFaker` — realistic names, emails, cities
- `ProductFaker` — product names, categories, prices
- `OrderFaker` — random orders referencing existing customers and products

### 3.3 Implement the BackgroundService

- Run on a configurable timer interval
- POST Customers and Products first, then create Orders referencing them
- Make batch size and timer interval configurable via `appsettings.json`

### 3.4 Configure

Set `API_BASE_URL` to point to the Orders API (default: `http://localhost:5000`).

### 3.5 Validate

```bash
# With the Orders API running:
cd src/DataGenerator
dotnet run
# Verify data appears in the API / database
```

---

## Phase 4 — Inventory Service

Build the stock tracking service.

### 4.1 Create the project

```bash
cd src
dotnet new worker -n InventoryService
```

### 4.2 Implement the polling BackgroundService

- On each interval, call `GET /api/orders?since={lastChecked}` to detect new orders
- Maintain an in-memory stock ledger per product
- Decrement stock when new orders are detected
- Call `PUT /api/products/{id}/stock` on the Orders API to persist updated levels
- Log warnings when a product drops below a configurable low-stock threshold

### 4.3 Add a debug endpoint

Expose an internal `/api/inventory` endpoint (minimal API) to view current stock state during development.

### 4.4 Validate

```bash
# With the Orders API and Data Generator running:
cd src/InventoryService
dotnet run
# Verify stock levels decrement as orders arrive
```

---

## Phase 5 — Dockerize & Test Locally

Containerize all services and validate the full flow.

### 5.1 Write Dockerfiles

Create a `Dockerfile` in each service folder (`src/OrdersApi/`, `src/Frontend/`, `src/DataGenerator/`, `src/InventoryService/`). Use multi-stage builds:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "<ProjectName>.dll"]
```

> **Note:** Use `mcr.microsoft.com/dotnet/runtime:10.0` as the base image for the worker services (DataGenerator, InventoryService) instead of `aspnet` — unless they expose HTTP endpoints.

### 5.2 Create docker-compose.yml

Create a `.env` file by copying the template (this file is git-ignored):

```bash
cp .env.example .env
# Edit .env and set your SA_PASSWORD
```

Create `docker-compose.yml` in the repo root:

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "${SA_PASSWORD}"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$${SA_PASSWORD}" -Q "SELECT 1" -C -b
      interval: 10s
      retries: 5

  orders-api:
    build: src/OrdersApi
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__RetailDb: "Server=sqlserver;Database=RetailDb;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True"
    ports:
      - "5000:8080"
    depends_on:
      sqlserver:
        condition: service_healthy

  frontend:
    build: src/Frontend
    environment:
      API_BASE_URL: "http://orders-api:8080"
    ports:
      - "8080:8080"
    depends_on:
      - orders-api

  data-generator:
    build: src/DataGenerator
    environment:
      API_BASE_URL: "http://orders-api:8080"
    depends_on:
      - orders-api

  inventory-service:
    build: src/InventoryService
    environment:
      API_BASE_URL: "http://orders-api:8080"
    depends_on:
      - orders-api
```

### 5.3 Run and validate

```bash
docker-compose up --build
```

Open `http://localhost:8080` in your browser. Verify:
- [ ] Frontend loads and shows the dashboard
- [ ] Data Generator is creating orders (check Orders page)
- [ ] Product stock levels are decrementing (check Products page)

---

## Phase 6 — Azure Infrastructure (Bicep)

Provision all Azure resources with a secure, private architecture.

### 6.1 Create the infra folder structure

```
infra/
├── main.bicep
├── main.bicepparam
└── modules/
    ├── networking.bicep
    ├── sql.bicep
    ├── acr.bicep
    ├── keyvault.bicep
    ├── loganalytics-aca.bicep
    └── identity.bicep
```

### 6.2 Networking module

Provision:
- **VNet** with two subnets:
  - `snet-aca` — delegated to `Microsoft.App/environments`
  - `snet-private-endpoints` — for private endpoints
- **Private DNS zones:**
  - `privatelink.database.windows.net`
  - `privatelink.azurecr.io`
  - `privatelink.vaultcore.azure.net`
- Link each private DNS zone to the VNet

### 6.3 Azure SQL module

- Azure SQL Server with **Entra ID-only authentication** (no SQL admin password)
- Set the User-Assigned Managed Identity as the Entra admin
- Create a database with an appropriate SKU
- **Private endpoint** on `snet-private-endpoints` with DNS zone group for `privatelink.database.windows.net`
- Set `publicNetworkAccess: 'Disabled'`

### 6.4 ACR module

- Azure Container Registry — **Premium** SKU (required for private endpoint)
- **Private endpoint** on `snet-private-endpoints` with DNS zone group for `privatelink.azurecr.io`
- Disable public network access
- Assign `AcrPull` role to the managed identity

### 6.5 Key Vault module

- Key Vault with **RBAC authorization** (not access policies)
- **Private endpoint** on `snet-private-endpoints` with DNS zone group for `privatelink.vaultcore.azure.net`
- Disable public network access
- Assign `Key Vault Secrets User` role to the managed identity
- Store non-identity config as secrets (API URLs, feature flags)

### 6.6 Log Analytics + ACA Environment module

- Log Analytics Workspace
- Container Apps Environment deployed into `snet-aca` (VNet-integrated)
- Internal-only environment — individual apps control their own ingress visibility

### 6.7 Managed Identity module

- User-Assigned Managed Identity
- Role assignments:
  - `AcrPull` on the ACR
  - `Key Vault Secrets User` on the Key Vault
  - Entra admin on the SQL Server

### 6.8 Orchestrate and deploy

```bash
# Preview the deployment
az deployment group create \
  --resource-group <rg-name> \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam \
  --what-if

# Deploy for real
az deployment group create \
  --resource-group <rg-name> \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

---

## Phase 7 — Deploy to ACA

Push container images and deploy to Azure Container Apps.

### 7.1 Build and push images

```bash
ACR_NAME=<your-acr-name>

az acr build --registry $ACR_NAME --image orders-api:latest src/OrdersApi/
az acr build --registry $ACR_NAME --image frontend:latest src/Frontend/
az acr build --registry $ACR_NAME --image data-generator:latest src/DataGenerator/
az acr build --registry $ACR_NAME --image inventory-service:latest src/InventoryService/
```

> **Note:** `az acr build` runs remotely inside ACR — no need for local Docker. If ACR has public access disabled, run from a build agent or jumpbox with VNet access.

### 7.2 Deploy container apps

Deploy each container app with the following configuration:

| App | Ingress | Scaling | Notes |
|-----|---------|---------|-------|
| **OrdersApi** | Internal only | KEDA HTTP (concurrent requests) | Managed identity for SQL auth via `DefaultAzureCredential` |
| **Frontend** | External | KEDA HTTP (concurrent requests) | `API_BASE_URL` → OrdersApi internal FQDN. Publicly accessible via ACA FQDN. |
| **DataGenerator** | None | Min: 1, Max: 1 | `API_BASE_URL` → OrdersApi internal FQDN |
| **InventoryService** | None | Min: 1, Max: 1 | `API_BASE_URL` → OrdersApi internal FQDN |

All apps should be assigned the same **User-Assigned Managed Identity**.

### 7.3 Configure secrets

Use Key Vault secret references for non-identity config (API URLs, feature flags). SQL auth is handled by managed identity — no connection string secret needed.

### 7.4 End-to-end validation

Test the full flow:

```
Browser → Frontend (external ACA FQDN) → OrdersApi (internal) → Azure SQL (private endpoint)
                                                                    ↑
                                          InventoryService (polling) ┘
```

- [ ] Open the Frontend ACA URL in a browser — Frontend should load
- [ ] Verify the Data Generator is creating data (check the Orders page)
- [ ] Verify stock levels are updating (check the Products page)
- [ ] Check container logs: `az containerapp logs show --name <app> --resource-group <rg>`

---

## Phase 8 — Fabric + Power BI

Connect Microsoft Fabric to Azure SQL and build reports.

### 8.1 Set up Fabric workspace

Create a Microsoft Fabric workspace (or use an existing one with sufficient capacity).

### 8.2 Configure managed private endpoint

1. In the Fabric workspace, create a **managed private endpoint** targeting the Azure SQL Server
2. Go to the Azure Portal → Azure SQL Server → Networking → Private endpoint connections
3. **Approve** the pending private endpoint connection from Fabric
4. No public firewall rules are needed

### 8.3 Set up Fabric Mirroring

1. In the Fabric workspace, create a **mirrored database**
2. Connect to Azure SQL using the managed private endpoint
3. Select the tables to replicate: Customers, Products, Orders, OrderItems
4. Verify data is syncing in near-real-time

### 8.4 Build the semantic model

Create a Power BI semantic model (dataset) on top of the mirrored data:

- **Relationships:**
  - Order → OrderItem (one-to-many)
  - OrderItem → Product (many-to-one)
  - Order → Customer (many-to-one)
- **Measures:**
  - Total Revenue = `SUM(OrderItem[Quantity] * OrderItem[UnitPrice])`
  - Order Count = `COUNTROWS(Orders)`
  - Avg Order Value = `[Total Revenue] / [Order Count]`
  - Stock Level = `SUM(Product[StockQuantity])`

### 8.5 Build the Power BI report

Create a report with four pages:

| Page | Content |
|------|---------|
| **Sales Overview** | Revenue over time, order counts, average order value |
| **Product Performance** | Top sellers, revenue by category, stock levels |
| **Customer Insights** | Customer count growth, repeat buyers, geographic breakdown |
| **Operations / Inventory** | Current stock, low-stock alerts, order fulfillment rate |

### 8.6 Publish

Publish the report to the Fabric workspace.

---

## Phase 9 — CI/CD (Stretch)

Automate build and deployment with GitHub Actions.

### 9.1 Create the workflow

Create `.github/workflows/build-deploy.yml`:

- **Trigger:** On push to `main`
- **Jobs:**
  1. Build Docker images for all four services
  2. Push to ACR
  3. Deploy updated container revisions to ACA via `az containerapp update`

### 9.2 Configure authentication

Set up **workload identity federation** (OIDC) for GitHub Actions → Azure. This avoids storing Azure credentials as GitHub secrets.

1. Create an App Registration in Entra ID
2. Add a federated credential for your GitHub repo/branch
3. Grant the service principal the necessary roles (ACR push, ACA contributor)
4. Configure the GitHub Actions workflow to use `azure/login` with OIDC

### 9.3 Handle private ACR

Since ACR has public access disabled, either:
- Use `az acr build` (runs inside ACR, no network access needed from the runner)
- Use a **self-hosted GitHub runner** deployed in the VNet

### 9.4 Optional: Approval gate

Add a manual approval step using GitHub Environments for production deployments.
