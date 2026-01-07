# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY src/Fundo.Domain/Fundo.Domain.csproj Fundo.Domain/
COPY src/Fundo.Services/Fundo.Services.csproj Fundo.Services/
COPY src/Fundo.Infrastructure/Fundo.Infrastructure.csproj Fundo.Infrastructure/
COPY src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj Fundo.Applications.WebApi/

RUN dotnet restore Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj

# Copy all source code and build
COPY src/ .
RUN dotnet publish Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Set environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
