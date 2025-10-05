#!/bin/bash
echo "Running tests..."
dotnet test --configuration Release --logger "console;verbosity=detailed"
echo "Tests completed!"