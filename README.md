# PristaneLaverieSmart

PristaneLaverieSmart is a full-stack sample application built with **.NET 8**, following **Clean Architecture principles**.

It demonstrates how to design and implement a modern backend using **ASP.NET Core**, **Entity Framework Core**, and a **Blazor Server** frontend.

This project was created as part Pristane Laverie Project backend and web UI development.

---

## 🏗 Architecture

The solution follows Clean Architecture principles:

PristaneLaverieSmart
├── PristaneLaverieSmart.Domain → Core business entities and enums
├── PristaneLaverieSmart.Application → Use cases and abstractions
├── PristaneLaverieSmart.Infrastructure → EF Core and persistence implementation
├── PristaneLaverieSmart.API → ASP.NET Core REST API (Minimal APIs)
├── PristaneLaverieSmart.UI → Blazor Server frontend
└── tests → Unit test projects

### Dependency Rules

- Domain → no dependencies
- Application → depends only on Domain
- Infrastructure → depends on Application + Domain
- API → depends on Application + Infrastructure
- UI → consumes the API

This ensures separation of concerns and maintainability.

---

## 🚀 Features Implemented (v0.1)

- Machine entity
- MachineStatus enum
- Repository abstraction pattern
- EF Core with SQLite
- Database migrations
- Minimal API endpoint:
  - `GET /api/machines`
- Seed endpoint for development
- Blazor UI displaying machine list
- End-to-end integration between UI and API

---

## 🛠 Tech Stack

- .NET 8
- ASP.NET Core
- Entity Framework Core (SQLite)
- Blazor Server
- Swagger
- xUnit (test projects prepared)

---

## ▶ Running the Application

### 1️⃣ Apply database migrations

From solution root:

```bash
dotnet ef database update \
  --project src/PristaneLaverieSmart.Infrastructure \
  --startup-project src/SmartLaundry.API
```

### 2️⃣ Run the API

From solution root:

```bash
dotnet run --project src/SmartLaundry.API
```

### 3️⃣ SRun the UI

From solution root:

```bash
dotnet run --project src/SmartLaundry.UI
```

---

## 👤 Author

Patrick Djomo
Software Engineer | Backend Developer