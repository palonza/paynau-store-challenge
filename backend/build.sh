#!/bin/bash
echo "Building Paynau Backend..."
dotnet restore
dotnet build --configuration Release
echo "Build completed successfully!"