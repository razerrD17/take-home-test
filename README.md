# Fundo - Loan Management System

A full-stack **Loan Management System** built with **.NET 6 (C#)** backend and **Angular 19** frontend.

## Project Structure

```
├── backend/                    # .NET 6 REST API
│   ├── src/                    # Source code (Clean Architecture)
│   │   ├── Fundo.Domain/       # Entities and interfaces
│   │   ├── Fundo.Services/     # Business logic layer
│   │   ├── Fundo.Infrastructure/  # Data access layer
│   │   ├── Fundo.Applications.WebApi/  # API controllers
│   │   └── Fundo.Services.Tests/  # Unit tests
│   └── Dockerfile              # Backend containerization
│
├── frontend/                   # Angular 19 application
│   └── src/app/                # Components, services, models
│
├── .github/workflows/          # CI/CD pipelines
│   └── backend-ci.yml          # Backend build & test pipeline
│
└── docker-compose.yml          # Full stack orchestration
```

## Features

### Backend
- **Clean Architecture** with domain-driven design
- **JWT Authentication** for secure API access
- **Entity Framework Core** with SQL Server
- **AutoMapper** for entity-to-DTO mapping
- **FluentValidation** for request validation
- **Structured Logging** with Serilog
- **Unit Tests** (16 tests with xUnit, Moq, FluentAssertions)
- **Docker Support** with multi-stage builds
- **CI/CD Pipeline** with GitHub Actions

### Frontend
- **Angular 19** with standalone components
- **Responsive UI** displaying loan data in a table
- **HTTP Client** integration with backend API
- **Environment Configuration** for API URLs

## Quick Start

### Using Docker (Recommended)
**Backend**

```sh
# Start the entire stack (SQL Server + Backend API)
docker-compose up --build
```

- **Backend API**: http://localhost:5000 (Swagger UI at root)
- **SQL Server**: localhost:1434

**Frontend**
```sh
# install dependencies only first time
npm install
```

```sh
# up angular app
npm start
```

- **Frontend**: `http://localhost:4200/` in your browser.

### Running Locally and more details

See individual READMEs for detailed instructions:
- [Backend README](backend/src/README.md)
- [Frontend README](frontend/README.md)

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/auth/login` | Get JWT token | No |
| GET | `/loans` | List all loans | No |
| GET | `/loans/{id}` | Get loan by ID | Yes |
| POST | `/loans` | Create new loan | Yes |
| POST | `/loans/{id}/payment` | Make payment | Yes |

## Default Credentials

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| demo | Demo@123 | User |

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| Backend | .NET 6, Entity Framework Core, SQL Server, AutoMapper, FluentValidation, Serilog, BCrypt, JWT |
| Frontend | Angular 19, TypeScript, RxJS |
| DevOps | Docker, Docker Compose, GitHub Actions |
| Testing | xUnit, Moq, FluentAssertions |

## Implementation Approach

### Architecture Decisions
- **Clean Architecture**: Chose this pattern to ensure separation of concerns, testability, and maintainability. The domain layer has no external dependencies, making business logic easy to test and modify.
- **Repository Pattern**: Abstracts data access, allowing easy swapping of data providers and simplified unit testing with mocks.
- **ServiceResult Pattern**: Provides consistent error handling across all services without relying on exceptions for control flow.
- **Standalone Components (Angular)**: Used Angular 19's modern standalone component approach instead of NgModules for simpler, more modular code.

### Challenges Faced
- **Docker SQL Server Health Checks**: Initial setup required proper health check configuration to ensure the API waits for SQL Server to be fully ready before starting.
- **Port Conflicts**: Local SQL Server instances can conflict with Docker containers; resolved by using external port 1434.
- **EF Core Migrations in Docker**: Ensured migrations run automatically on startup via DbInitializer to simplify deployment.

### Bonus Features Completed
- **Structured Logging**: Implemented Serilog with enrichers, request logging middleware, and TraceId for request correlation.
- **JWT Authentication**: Full authentication system with login endpoint, BCrypt password hashing, and protected endpoints.
- **GitHub Actions CI/CD**: Automated pipeline for building and testing the backend on every push/PR.

## Future Improvements

Given more time, the following enhancements could be made:

### Backend
- Add integration tests with TestContainers for database testing
- Implement refresh tokens for better session management
- Add rate limiting and request throttling
- Implement pagination for the loans list endpoint
- Add role-based authorization (Admin vs User permissions)
- Add audit logging for loan modifications

### Frontend
- Implement login page and token management
- Add create loan and make payment forms
- Add loading states and error handling UI
- Implement responsive design for mobile devices
- Add unit tests with Jasmine/Karma

### DevOps
- Add frontend to Docker Compose
- Implement multi-environment configurations (staging, production)
- Add database backup/restore scripts
- Implement health check endpoints for container orchestration
