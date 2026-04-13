# Azure Container Apps Retail Example — Step-by-Step Instructions

Welcome! This guide walks you through running the Miyazaki Retail application — first on your own machine, then deployed to Azure. No prior experience with containers is needed.

> **What you'll build:** A multi-service retail app with a web dashboard, a REST API, a fake-data generator, and an inventory tracker — all running in containers.

## Guide

Follow the docs in order:

| # | Section | What you'll do |
|---|---------|---------------|
| 0 | [Prerequisites](docs/00-prerequisites.md) | Install the required tools (.NET, Docker, Azure CLI, Git) |
| 1 | [Run Locally](docs/01-run-locally.md) | Start the full app on your machine with `docker compose up` |
| 2 | [Project Structure](docs/02-project-structure.md) | Understand how the services, Dockerfiles, and folders are organized |
| 3 | [Deploy to Azure](docs/03-deploy-to-azure.md) | Provision infrastructure with Bicep, push images, and go live on Azure Container Apps |
| 4 | [Clean Up & Troubleshooting](docs/04-cleanup-and-troubleshooting.md) | Delete Azure resources and fix common issues |
