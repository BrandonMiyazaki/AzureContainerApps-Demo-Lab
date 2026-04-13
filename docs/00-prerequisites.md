# Prerequisites

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

## Verify your tools

Open a terminal and run these commands. Each should print a version number:

```bash
dotnet --version      # Should show 9.x or higher
docker --version      # Should show Docker version 2x.x
az --version          # Should show azure-cli 2.x
git --version         # Should show git version 2.x
```

> **Troubleshooting:** If any command says "not found", revisit the install link above. On Windows, you may need to restart your terminal after installing.

---

[Next: Run Locally →](01-run-locally.md)
