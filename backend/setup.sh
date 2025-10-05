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