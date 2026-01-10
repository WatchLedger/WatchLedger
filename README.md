# WatchPlatform - Distributed Watch Collection & Valuation System

A comprehensive cloud-based microservices platform built with .NET 8, Azure, and modern cloud architecture patterns. WatchPlatform enables users to manage their watch collections, list advertisements, place bids, and get valuations through a distributed system of interconnected APIs.

## 🎯 Project Overview

**WatchPlatform** is a production-grade Azure cloud application demonstrating:
- **Microservices Architecture** with 3 independent APIs
- **Identity & Security** via centralized Identity Server with OAuth2/OpenID Connect
- **Multiple Storage Solutions** including Azure SQL, Azure Cosmos DB, and Azure Blob Storage
- **Inter-service Communication** with resilience patterns
- **API Documentation** via OpenAPI (Swagger) and Postman collections
- **Cloud-native Practices** with Key Vault integration and rate limiting

### Core Services

| Service | Purpose | Technologies |
|---------|---------|--------------|
| **IdentityServer** | Centralized authentication & authorization | Duende IdentityServer, Azure SQL, Razor Pages |
| **WatchCollection.Api** | Manage watch inventory and advertisements | ASP.NET Core, Entity Framework, Azure SQL, Blob Storage |
| **WatchValuation.Api** | Evaluate watch prices and market data | ASP.NET Core, Azure Cosmos DB, External APIs |

## 🚀 Live Deployment

All services are deployed on Azure App Service and fully operational:

```
Identity Server:       https://identityserver-watchcollection.azurewebsites.net
WatchCollection API:   https://watchcollectionservice.azurewebsites.net
WatchValuation API:    https://watchvaluationservice.azurewebsites.net
```

## 📋 Prerequisites

- **.NET 9 SDK And 8**
- **Visual Studio 2022** / **VS Code** with C# extension
- **SQL Server** (local or Azure)
- **Azure Account** (for cloud deployment)
- **Git**

## 🔧 Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd WatchPlatform
```

### 2. Build the Solution
```bash
cd WatchPlatform
dotnet build WatchPlatform.sln
```

### 3. Database Configuration

#### Identity Server Migrations
```bash
cd IdentityServer
dotnet ef database update --startup-project . --project .
```

#### WatchCollection Database Migrations
```bash
cd WatchCollection.Storage
dotnet ef database update --startup-project ../WatchCollection.Api --project .
```

### 4. Configure Azure Resources

Update appsettings.json in each service with your Azure resources:
- **Key Vault URI**: `https://<your-vault>.vault.azure.net/`
- **SQL Database Connection String**: Add to Key Vault as `IdentityServerProductionSqlString`
- **Cosmos DB Connection String**: Add to Key Vault as `CosmosDbConnectionString`
- **Blob Storage Connection String**: Add to Key Vault as `BlobStorageConnectionString`

### 5. Run Services Locally

**Terminal 1 - Identity Server (Port 5001)**
```bash
cd IdentityServer
dotnet run
```

**Terminal 2 - WatchCollection API (Port 5097)**
```bash
cd WatchCollection.Api
dotnet run
```

**Terminal 3 - WatchValuation API (Port 5005)**
```bash
cd WatchValuation.Api
dotnet run
```

## 📚 API Documentation

### OpenAPI/Swagger UI

Once services are running, access the interactive API documentation:

- **WatchCollection API**: `http://localhost:5097/scalar/v1`
- **WatchValuation API**: `http://localhost:5100/scalar/v1`
- **Identity Server**: Razor Pages UI at `http://localhost:5000`

### Postman Collection

Import the provided collection for complete API testing:
```
WatchPlatform-RESTful-API-documentation.postman_collection.json
```

**Key Endpoints:**
- `POST /api/Watch` - Create new watch
- `POST /api/Advertisement` - Create advertisement
- `POST /api/Bid` - Place bid on advertisement
- `GET /api/valuation?referenceNumber={id}` - Get watch valuation
- `GET /api/brands` - Get available watch brands

## 🏗️ Architecture & Technologies

### Technology Stack
- **Framework**: ASP.NET Core 8
- **Authentication**: Duende IdentityServer 6 with OAuth2/OpenID Connect
- **Database**: 
  - Azure SQL (Relational data)
  - Azure Cosmos DB (Caching & NoSQL)
  - Azure Blob Storage (Image storage)
- **Cloud Platform**: Microsoft Azure
- **API Documentation**: OpenAPI 3.0 with Scalar
- **Monitoring**: Azure Key Vault, Serilog

### Key Features
- ✅ Role-based Authorization (RBAC)
- ✅ Claim-based Authorization
- ✅ Rate Limiting on API endpoints
- ✅ CORS configuration for cross-origin requests
- ✅ Centralized Exception Handling Middleware
- ✅ Resilient HTTP client with Polly retry policies
- ✅ Clean Architecture with layered structure

### Project Structure
```
WatchPlatform/
├── IdentityServer/                          # Central auth server
├── WatchCollection.Api/                     # Main API for watch management
│   ├── Controllers/                         # API endpoints
│   ├── Services/                            # Authorization role logic
│   └── Middleware/                          # Exception handling
├── WatchCollection.Domain.Services/         # Core business logic
├── WatchCollection.Storage/                 # Data access layer
├── WatchCollection.Infrastructure/          # Azure services integration
├── WatchValuation.Api/                      # Valuation microservice
├── WatchValuation.Domain.Services/          # Valuation logic
├── WatchValuation.Storage/                  # Cosmos DB access
├── WatchValuation.Infrastructure/           # External API clients
└── WatchPlatform.sln                        # Solution file
```

## 🔐 Authentication Flow

1. User logs in via Identity Server OAuth2 endpoint
2. Identity Server validates credentials against Azure SQL
3. JWT token issued with user scopes and claims
4. Client includes token in `Authorization: Bearer <token>` header
5. APIs validate token against Identity Server's public key
6. Authorization policies enforce role/claim requirements

## 📡 Service Communication

- **WatchCollection → WatchValuation**: HTTP client with JWT token exchange
- **External API Integration**: TheWatchApi for market data and pricing
- **Resilience**: Polly-based retry policies with exponential backoff

## Building

### Build Solution
```bash
dotnet build WatchPlatform.sln
```

## 📊 Monitoring & Logging

- **Serilog** integration for structured logging
- **Azure Key Vault** for secrets management
- **Application Insights** ready for diagnostic tracking
- **Exception Middleware** captures and logs all unhandled exceptions

## 🚢 CI/CD Pipeline

Azure Pipelines configured for automated:
- Build compilation
- NuGet package restoration
- Unit test execution
- Artifact staging

View pipeline configuration: `azure-pipelines.yml`

## 📝 API Contracts

### Request/Response Objects
- `WatchRequestContract` / `WatchResponseContract`
- `AdvertisementRequestContract` / `AdvertisementResponseContract`
- `BidRequestContract` / `BidResponseContract`
- `WatchImageResponseContract`

See `WatchCollection.Api.Contracts/` for full contract definitions.

## 📄 License

This project is for educational purposes.

## 👤 Author

Nathan Geleyn - Academic Year 2025-2026, Semester 1

## 🔗 Resources

- [Microsoft Azure Documentation](https://docs.microsoft.com/azure/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/dotnet/core/aspnet/)
- [Duende IdentityServer](https://duendesoftware.com/products/identityserver)
- [Azure Cosmos DB Best Practices](https://docs.microsoft.com/azure/cosmos-db/)
- [OpenAPI Specification](https://spec.openapis.org/)