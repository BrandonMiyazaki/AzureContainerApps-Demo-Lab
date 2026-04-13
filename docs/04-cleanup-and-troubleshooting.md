# Clean Up & Troubleshooting

## Clean Up Azure Resources

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

---

[← Deploy to Azure](03-deploy-to-azure.md)
