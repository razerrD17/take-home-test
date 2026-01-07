# Backend - Loan Management API

A .NET 6 RESTful API for managing loans, built following **Clean Architecture** principles with Entity Framework Core and SQL Server.

## Architecture

The project follows **Clean Architecture** with clear separation of concerns:

```
src/
├── Fundo.Domain/                    # Core business entities and interfaces
│   ├── Entities/                    # Loan, User entities
│   └── Interfaces/                  # Repository interfaces (ILoanRepository, IUserRepository)
│
├── Fundo.Services/                  # Application/Business logic layer
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Enums/                       # ServiceErrorType enum
│   ├── Interfaces/                  # Service interfaces (ILoanService, IAuthService)
│   ├── Implementations/             # Service implementations
│   ├── Mappings/                    # AutoMapper profiles
│   ├── Validators/                  # FluentValidation validators
│   ├── Common/                      # ServiceResult pattern
│   └── DependencyInjection/         # Service layer DI registration
│
├── Fundo.Infrastructure/            # Data access and external concerns
│   ├── Data/                        # LoanDbContext
│   ├── Repositories/                # Repository implementations
│   ├── Configuration/               # JwtSettings
│   ├── Seed/                        # Database seeding (DbInitializer)
│   ├── Common/                      # ErrorResponse
│   └── DependencyInjection/         # Infrastructure layer DI registration
│
├── Fundo.Applications.WebApi/       # Presentation layer (API)
│   ├── Controllers/                 # LoanManagementController, AuthController
│   ├── Middleware/                  # ExceptionHandlingMiddleware
│   ├── Program.cs                   # Application entry point
│   └── Startup.cs                   # DI and middleware configuration
│
└── Fundo.Services.Tests/            # Unit tests
    └── Unit/                        # LoanServiceTests, AuthServiceTests
```

## Key Features

- **Clean Architecture**: Domain-driven design with dependency inversion
- **JWT Authentication**: Secure API endpoints with Bearer token authentication
- **AutoMapper**: Automatic entity-to-DTO mapping
- **FluentValidation**: Request validation with custom validators
- **Repository Pattern**: Abstracted data access
- **ServiceResult Pattern**: Consistent error handling across services
- **Global Exception Handling**: Middleware for standardized error responses
- **Structured Logging**: Serilog with request logging and contextual information
- **Database Seeding**: Automatic migration and seed data on startup
- **Docker Support**: Containerized deployment with Docker Compose
- **CI/CD Pipeline**: GitHub Actions for automated build and test

## Prerequisites

- .NET 6.0 SDK
- SQL Server (local or Docker)
- Docker (optional, for containerized deployment)

## Running with Docker (Recommended)

The easiest way to run the backend is using Docker Compose, which sets up both SQL Server and the API:

```sh
# From the root of the project (where docker-compose.yml is located)
docker-compose up --build
```

This will:
1. Start SQL Server on port 1434
2. Wait until SQL Server is healthy
3. Start the Backend API on port 5000
4. Apply migrations and seed the database automatically

The API will be available at `http://localhost:5000` with Swagger UI at the root.

### Docker Commands

```sh
# Start services
docker-compose up --build

# Start in background (detached mode)
docker-compose up -d --build

# Rebuild only the backend (faster if SQL Server is already running)
docker-compose up --build backend

# View logs
docker-compose logs -f backend

# Stop services
docker-compose down

# Stop and remove volumes (resets the database)
docker-compose down -v
```

## Running Locally (Without Docker)

### 1. Database Setup

Run SQL Server via Docker:

```sh
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configuration

You can use the current development settings in the project or update the connection string and JWT settings in `appsettings.Development.json` as needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=LoanManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-here-minimum-32-chars",
    "Issuer": "Fundo",
    "Audience": "FundoUsers",
    "ExpirationInMinutes": 60
  }
}
```

### 3. Build and Run

```sh
cd src
dotnet build
cd Fundo.Applications.WebApi
dotnet run
```

The API will be available at `http://localhost:5000`. Swagger UI is available at the root URL.

### 4. Database Migration

Migrations are applied automatically on startup via `DbInitializer`. To manually manage migrations:

```sh
cd Fundo.Applications.WebApi
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/auth/login` | Authenticate and get JWT token | No |

### Loans

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/loans` | List all loans | No |
| GET | `/loans/{id}` | Get a specific loan | Yes |
| POST | `/loans` | Create a new loan | Yes |
| POST | `/loans/{id}/payment` | Make a payment on a loan | Yes |

### Request/Response Examples

**Login (POST /auth/login)**
```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@fundo.com",
  "role": "Admin"
}
```

**Create Loan (POST /loans)**
```json
{
  "amount": 10000.00,
  "applicantName": "John Doe"
}
```

**Make Payment (POST /loans/{id}/payment)**
```json
{
  "amount": 500.00
}
```

**Loan Response**
```json
{
  "id": 1,
  "amount": 10000.00,
  "currentBalance": 9500.00,
  "applicantName": "John Doe",
  "status": "active"
}
```

## Running Tests

```sh
cd src
dotnet test
```

The test project includes **16 unit tests** covering:
- **LoanService**: CRUD operations, payment logic, validation
- **AuthService**: Login, password hashing, token generation

Tests use **Moq** for mocking and **FluentAssertions** for readable assertions.

## Seed Data

### Users

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| demo | Demo@123 | User |

### Loans

| Applicant | Amount | Balance | Status |
|-----------|--------|---------|--------|
| John Doe | $10,000 | $8,500 | active |
| Jane Smith | $25,000 | $25,000 | active |
| Bob Johnson | $15,000 | $0 | paid |
| Alice Williams | $50,000 | $42,000 | active |
| Charlie Brown | $7,500 | $3,200 | active |

## Security Features

- **JWT Authentication**: All loan endpoints require valid Bearer token
- **Password Hashing**: BCrypt with salt for secure password storage
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy
- **CORS**: Configured for Angular frontend at `http://localhost:4200`
- **Input Validation**: FluentValidation on all request DTOs

## CI/CD Pipeline

The project includes a GitHub Actions pipeline that automatically runs on:
- Push to `main` branch (when backend files change)
- Pull requests to `main` branch (when backend files change)
- Manual trigger via workflow_dispatch

The pipeline performs:
1. **Restore** - Downloads NuGet dependencies
2. **Build** - Compiles the solution in Release mode
3. **Test** - Runs all 16 unit tests
4. **Upload Results** - Stores test results as artifacts (7 days retention)

Pipeline file: `.github/workflows/backend-ci.yml`

## Technology Stack

- .NET 6.0
- Entity Framework Core 6.0
- SQL Server
- AutoMapper 12.0
- FluentValidation 11.9
- Serilog (Structured Logging)
- BCrypt.Net-Next 4.0
- JWT Bearer Authentication
- Swashbuckle (Swagger)
- Docker
- GitHub Actions (CI/CD)
- xUnit + Moq + FluentAssertions (Testing)
