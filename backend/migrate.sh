#!/bin/bash
echo "Running database migrations..."
cd src/Paynau.Api
dotnet ef database update
echo "Migrations completed!"