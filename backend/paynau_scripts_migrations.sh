#!/bin/bash
# ========================================
# build.sh - Build Script
# ========================================
#!/bin/bash
echo "Building Paynau Backend..."
dotnet restore
dotnet build --configuration Release
echo "Build completed successfully!"

# ========================================
# test.sh - Test Script
# ========================================
#!/bin/bash
echo "Running tests..."
dotnet test --configuration Release --logger "console;verbosity=detailed"
echo "Tests completed!"

# ========================================
# migrate.sh - Migration Script
# ========================================
#!/bin/bash
echo "Running database migrations..."
cd src/Paynau.Api
dotnet ef database update
echo "Migrations completed!"

# ========================================
# docker-build.sh - Docker Build Script
# ========================================
#!/bin/bash
echo "Building Docker image..."
docker build -t paynau-backend:latest .
echo "Docker image built successfully!"

# ========================================
# docker-run.sh - Docker Run Script
# ========================================
#!/bin/bash
echo "Starting Paynau with Docker Compose..."
docker-compose up --build
echo "Application started!"

# ========================================
# clean.sh - Clean Script
# ========================================
#!/bin/bash
echo "Cleaning build artifacts..."
dotnet clean
rm -rf **/bin
rm -rf **/obj
echo "Clean completed!"

# ========================================
# setup.sh - Initial Setup Script
# ========================================
#!/bin/bash
echo "Setting up Paynau Backend..."

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build the project
echo "Building project..."
dotnet build --configuration Debug

# Install EF Core tools
echo "Installing EF Core tools..."
dotnet tool install --global dotnet-ef

# Create initial migration
echo "Creating initial migration..."
cd src/Paynau.Api
dotnet ef migrations add InitialCreate

echo "Setup completed! You can now run 'dotnet ef database update' to create the database."

# ========================================
# COMMANDS.md - Useful Commands Reference
# ========================================
# Paynau Backend - Useful Commands

## Development Setup

### Initial Setup
```bash
# Clone and navigate to backend folder
cd backend

# Restore dependencies
dotnet restore

# Build the solution
dotnet build
```

### Database Migrations

```bash
# Navigate to API project
cd src/Paynau.Api

# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create a new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script

# Drop database
dotnet ef database drop
```

## Running the Application

### Local Development
```bash
# Run the API
cd src/Paynau.Api
dotnet run

# Run with watch (auto-reload)
dotnet watch run

# Run specific environment
dotnet run --environment Production
```

### Docker Development
```bash
# Build and run with Docker Compose
docker-compose up --build

# Run in detached mode
docker-compose up -d

# View logs
docker-compose logs -f backend

# Stop containers
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test project
dotnet test tests/Paynau.Tests/Paynau.Tests.csproj
```

## Building

```bash
# Build in Debug mode
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Clean build artifacts
dotnet clean

# Publish for deployment
dotnet publish -c Release -o ./publish
```

## Code Quality

```bash
# Format code
dotnet format

# Restore NuGet packages
dotnet restore

# List outdated packages
dotnet list package --outdated

# Update a specific package
dotnet add package PackageName --version X.X.X
```

## Docker Commands

```bash
# Build Docker image
docker build -t paynau-backend:latest .

# Run Docker container
docker run -p 5001:5001 paynau-backend:latest

# View running containers
docker ps

# View logs
docker logs paynau-backend

# Stop container
docker stop paynau-backend

# Remove container
docker rm paynau-backend

# Remove image
docker rmi paynau-backend:latest
```

## Debugging

```bash
# Run with verbose logging
dotnet run --verbosity detailed

# Check application info
dotnet --info

# List all projects in solution
dotnet sln list

# Check for security vulnerabilities
dotnet list package --vulnerable
```

## Production Deployment

```bash
# Build optimized release
dotnet publish -c Release -o ./publish

# Create Docker image for production
docker build -t paynau-backend:v1.0.0 .

# Tag image
docker tag paynau-backend:v1.0.0 registry.example.com/paynau-backend:v1.0.0

# Push to registry
docker push registry.example.com/paynau-backend:v1.0.0
```

## Environment Variables

```bash
# Set environment variable (Linux/Mac)
export ASPNETCORE_ENVIRONMENT=Development

# Set environment variable (Windows PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Development"

# Set connection string
export ConnectionStrings__Default="Server=localhost;Database=PaynauDb;User=root;Password=pass;"
```

## Troubleshooting

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Rebuild solution
dotnet clean
dotnet restore
dotnet build

# Check database connection
dotnet ef dbcontext info

# List all migrations
dotnet ef migrations list

# View EF Core version
dotnet ef --version
```

# ========================================
# MIGRATION_GUIDE.md - Migration Guide
# ========================================
# Paynau Backend - Database Migration Guide

## Overview

This guide explains how to create, manage, and apply database migrations for the Paynau backend.

## Prerequisites

- .NET 8 SDK installed
- EF Core tools installed globally: `dotnet tool install --global dotnet-ef`
- MySQL server running (or use Docker Compose)

## Creating the Initial Migration

### Step 1: Navigate to the API Project
```bash
cd src/Paynau.Api
```

### Step 2: Create Initial Migration
```bash
dotnet ef migrations add InitialCreate
```

This will create:
- `Migrations/YYYYMMDDHHMMSS_InitialCreate.cs` - Migration code
- `Migrations/PaynauDbContextModelSnapshot.cs` - Model snapshot

### Step 3: Review the Migration
Open the generated migration file and verify:
- Table creation statements
- Column definitions
- Indexes and constraints
- Foreign key relationships

### Step 4: Apply the Migration
```bash
dotnet ef database update
```

This will:
1. Create the database if it doesn't exist
2. Apply all pending migrations
3. Update the `__EFMigrationsHistory` table

## Adding New Migrations

### Scenario: Adding a new field to Product

1. **Modify the Entity**
```csharp
// Paynau.Domain/Entities/Product.cs
public string Category { get; private set; } = string.Empty;
```

2. **Update Configuration**
```csharp
// Paynau.Infrastructure/Data/Configurations/ProductConfiguration.cs
builder.Property(p => p.Category)
    .HasMaxLength(100);
```

3. **Create Migration**
```bash
cd src/Paynau.Api
dotnet ef migrations add AddCategoryToProduct
```

4. **Review and Apply**
```bash
dotnet ef database update
```

## Managing Migrations

### List All Migrations
```bash
dotnet ef migrations list
```

### Remove Last Migration (if not applied)
```bash
dotnet ef migrations remove
```

### Revert to Specific Migration
```bash
dotnet ef database update PreviousMigrationName
```

### Generate SQL Script
```bash
# Generate SQL for all migrations
dotnet ef migrations script

# Generate SQL for specific range
dotnet ef migrations script InitialCreate AddCategoryToProduct

# Output to file
dotnet ef migrations script -o migration.sql
```

## Production Deployment

### Option 1: Apply Migrations Automatically
The application can apply migrations on startup (only in Development):
```csharp
// Already configured in Program.cs
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PaynauDbContext>();
    await context.Database.MigrateAsync();
}
```

### Option 2: Generate SQL Script
```bash
dotnet ef migrations script -o production-migration.sql
```

Then execute the SQL script manually on the production database.

### Option 3: Use Migration Bundle
```bash
dotnet ef migrations bundle --self-contained -r linux-x64
```

This creates a standalone executable that can apply migrations.

## Seed Data

### Development Environment
Seed data is automatically loaded from JSON files on application startup:
- `src/Paynau.Infrastructure/Seed/products.json`
- `src/Paynau.Infrastructure/Seed/orders.json`

### Production Environment
For production, consider:
1. Using EF Core's `HasData()` method in configurations
2. Creating dedicated seed scripts
3. Using database initialization scripts

## Troubleshooting

### Migration Already Applied
```bash
# Force remove migration (use with caution)
dotnet ef migrations remove --force
```

### Database Out of Sync
```bash
# Drop database and recreate
dotnet ef database drop
dotnet ef database update
```

### Concurrency Issues
The Product entity uses `RowVersion` for optimistic concurrency:
```csharp
public byte[] RowVersion { get; set; } = Array.Empty<byte>();
```

This is configured in the migration:
```csharp
builder.Property(p => p.RowVersion)
    .IsRowVersion()
    .IsConcurrencyToken();
```

## Docker Environment

When using Docker Compose, migrations are applied automatically on container startup in Development mode.

### Manual Migration in Docker
```bash
# Execute migration command inside container
docker-compose exec backend dotnet ef database update --project /app/Paynau.Api.csproj
```

## Best Practices

1. **Always review migrations before applying**
2. **Test migrations in development first**
3. **Keep migrations small and focused**
4. **Name migrations descriptively**
5. **Never modify applied migrations**
6. **Backup production database before migrations**
7. **Use transactions for data migrations**
8. **Document breaking changes**

## Migration Naming Conventions

- `InitialCreate` - First migration
- `AddXToY` - Adding column X to table Y
- `RemoveXFromY` - Removing column X from table Y
- `CreateZTable` - Creating new table Z
- `UpdateXConstraint` - Modifying constraint
- `AddXIndex` - Adding index

## Rollback Strategy

### Development
```bash
dotnet ef database update PreviousMigration
dotnet ef migrations remove
```

### Production
1. Keep backup SQL scripts
2. Test rollback in staging
3. Document rollback steps
4. Have a maintenance window
5. Monitor application after rollback

## Additional Resources

- [EF Core Migrations Documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Migration Best Practices](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing)
