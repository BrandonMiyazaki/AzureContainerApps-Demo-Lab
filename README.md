# Miyazaki Retail — Azure Container Apps Data Generator

> **Disclaimer:** This repository is provided as-is for learning purposes and is not actively maintained. Dependencies, Azure services, and APIs may change over time. Use at your own discretion.

A hands-on lab for building and deploying a multi-service retail application on **Azure Container Apps**, with a fully private, production-grade architecture.

## Architecture

```mermaid
graph TB
    subgraph Internet
        Browser["🌐 Browser"]
    end

    subgraph Azure["Azure Resource Group"]
        subgraph VNet["VNet (10.31.0.0/16)"]
            subgraph ACA["snet-aca (10.31.0.0/23) — Container Apps Environment"]
                Frontend["Blazor Frontend\n(external ingress)"]
                API["Orders API\n(internal ingress)"]
                DG["Data Generator\n(no ingress)"]
                IS["Inventory Service\n(no ingress)"]
            end

            subgraph PE["snet-private-endpoints (10.31.2.0/24)"]
                SQL_PE["SQL Private\nEndpoint"]
                ACR_PE["ACR Private\nEndpoint"]
                KV_PE["Key Vault Private\nEndpoint"]
            end
        end

        SQL[("Azure SQL\nDatabase")]
        ACR["Azure Container\nRegistry"]
        KV["Azure Key Vault"]
        MI["Managed Identity\n(Entra ID)"]
        LA["Log Analytics"]
    end

    Browser -->|HTTPS| Frontend
    Frontend -->|HTTP internal| API
    DG -->|HTTP internal| API
    IS -->|HTTP internal| API
    API --> SQL_PE --> SQL
    ACA -.->|image pull| ACR_PE --> ACR
    ACA -.->|secrets| KV_PE --> KV
    MI -.->|auth| SQL
    MI -.->|auth| ACR
    MI -.->|auth| KV
    LA -.->|diagnostics| SQL
    LA -.->|diagnostics| KV

    style Internet fill:#e1f5fe,stroke:#0288d1
    style VNet fill:#e8f5e9,stroke:#388e3c
    style ACA fill:#fff3e0,stroke:#f57c00
    style PE fill:#fce4ec,stroke:#c62828
    style SQL fill:#e8eaf6,stroke:#3f51b5
    style ACR fill:#e8eaf6,stroke:#3f51b5
    style KV fill:#e8eaf6,stroke:#3f51b5
```

## Services

| Service | Type | Description |
|---------|------|-------------|
| **Orders API** | ASP.NET Core Web API | CRUD for customers, products, and orders. EF Core → Azure SQL. |
| **Frontend** | Blazor Server | Retail dashboard UI — orders, products, customers, inventory. |
| **Data Generator** | .NET Worker Service | Generates fake data with Bogus and posts to the Orders API. |
| **Inventory Service** | .NET Worker Service | Polls for new orders and tracks stock levels per product. |

## Security

- **Frontend** has external ingress on Azure Container Apps — all other services use internal-only ingress
- **Private endpoints** for Azure SQL, ACR, and Key Vault (all traffic on Azure backbone)
- **Managed Identity (Entra ID)** for SQL auth, ACR pull, and Key Vault access — no stored passwords

## Project Structure

```
├── src/
│   ├── OrdersApi/          # ASP.NET Core Web API
│   ├── Frontend/           # Blazor Server app
│   ├── DataGenerator/      # .NET Worker Service
│   └── InventoryService/   # .NET Worker Service
├── infra/                  # Bicep modules for Azure infrastructure
├── .github/
│   └── workflows/          # CI/CD pipelines
├── docker-compose.yml      # Local development
├── plan.md                 # Lab plan and design decisions
├── instructions.md         # Step-by-step lab instructions
└── README.md
```

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- An Azure subscription
- (Optional) [Microsoft Fabric](https://www.microsoft.com/microsoft-fabric) capacity for Power BI reporting

## Getting Started

### Local Development

1. Clone the repo
2. Copy the environment template and set your SQL password:
   ```bash
   cp .env.example .env
   ```
3. Start all services:
   ```bash
   docker-compose up --build
   ```
4. Open `http://localhost:8080` in your browser

### Azure Deployment

1. Update `infra/main.bicepparam` with your desired Azure region and base name
2. Configure the following GitHub repository **secrets** for CI/CD:
   - `AZURE_CLIENT_ID` — Service principal / federated credential client ID
   - `AZURE_TENANT_ID` — Entra ID tenant ID
   - `AZURE_SUBSCRIPTION_ID` — Azure subscription ID
3. Configure the following GitHub repository **variables**:
   - `AZURE_RESOURCE_GROUP` — Target resource group name
   - `ACR_NAME` — Azure Container Registry name (without `.azurecr.io`)

Follow the [Lab Instructions](instructions.md) for the full step-by-step walkthrough.

## License

See [LICENSE](LICENSE) for details.