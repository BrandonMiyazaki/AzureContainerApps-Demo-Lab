# Run Locally with Docker Compose

This is the fastest way to see the full app running. Docker Compose will start a SQL Server database, the Orders API, the Frontend, and both worker services — all with one command.

## What are containers?

A **container** is a lightweight, portable package that bundles your application code along with everything it needs to run (runtime, libraries, config). Think of it like a shipping container — it works the same way no matter where you run it (your laptop, a server, the cloud).

**Docker** is the tool that builds and runs containers. **Docker Compose** lets you run multiple containers together (like our 4 services + a database) with a single command.

---

## Step 1: Clone the repository

```bash
git clone https://github.com/<your-username>/AzureContainerApps-Retail-Example.git
cd AzureContainerApps-Retail-Example
```

## Step 2: Name your store

Open `src/Frontend/appsettings.json` and change the `StoreName` to whatever you want:

```json
"StoreName": "Contoso Retail"
```

This name appears in the browser tab titles and the sidebar. The default is "Miyazaki Retail".

## Step 3: Create your environment file

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

## Step 4: Make sure Docker Desktop is running

Look for the Docker whale icon in your system tray (Windows) or menu bar (Mac). If it's not there, open Docker Desktop and wait for it to say "Docker is running."

> **First time?** Docker Desktop may take a minute to start. On Windows, it may ask you to enable WSL 2 — follow the prompts.

## Step 5: Start everything

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

## Step 6: Open the app

Open your browser and go to: **http://localhost:8080**

You should see the retail dashboard with:
- **Dashboard** — summary cards (orders, revenue, customers, low-stock items)
- **Orders** — click any order to see its line items
- **Products** — product catalog with stock levels
- **Customers** — click any customer to expand their order history
- **New Order** — create an order manually

> **Not loading?** Wait 30 seconds — the Data Generator needs time to create initial data. Refresh the page.

## Step 7: Stop everything

Press `Ctrl+C` in the terminal where Docker Compose is running. Then clean up:

```bash
docker compose down
```

This stops and removes the containers. Your source code is untouched.

> **Want a fresh start?** Run `docker compose down -v` to also remove the database volume. Next time you start up, the database will be re-created from scratch.

---

[← Prerequisites](00-prerequisites.md) · [Next: Project Structure →](02-project-structure.md)
