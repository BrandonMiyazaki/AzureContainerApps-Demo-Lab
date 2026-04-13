# Understand the Project Structure

Before deploying to Azure, let's understand what we're working with.

## Folder layout

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

## How the services connect

Each service is **independent** — they communicate over HTTP:

| Service | Role | Ingress |
|---------|------|---------|
| **Orders API** | Central data layer — CRUD for customers, products, orders | Internal (other services call it) |
| **Frontend** | Web dashboard for users | External (public) |
| **Data Generator** | Creates fake data by POSTing to the API | None (outbound only) |
| **Inventory Service** | Polls the API for new orders, updates stock levels | None (outbound only) |

**Key concept:**
- The API URL is configured via the `API_BASE_URL` environment variable
- In Docker Compose, services share a private network and use service names as hostnames (e.g., `http://orders-api:8080`)
- In Azure, they share a Container Apps Environment and use internal FQDNs

## What's a Dockerfile?

A `Dockerfile` is a recipe for building a container image. Each of our services has one. Here's what they do:

1. **Build stage** — Uses the .NET SDK to compile the project
2. **Runtime stage** — Copies just the compiled output into a lightweight image
3. The final image is small (~100 MB) and contains only what's needed to run

## What's docker-compose.yml?

It's a configuration file that says: "run these 5 containers together, connect them on a network, and set these environment variables." It's the local equivalent of what Azure Container Apps does in the cloud.

---

[← Run Locally](01-run-locally.md) · [Next: Deploy to Azure →](03-deploy-to-azure.md)
