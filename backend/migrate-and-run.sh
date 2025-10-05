#!/bin/sh
set -e

echo "⏳ Waiting for MySQL to be ready on host 'mysql'..."
until nc -z mysql 3306; do
  echo "MySQL is unavailable - sleeping..."
  sleep 3
done

echo "✅ MySQL is up - applying migrations..."
# Llama a la DLL de migraciones directamente
dotnet /app/Paynau.Api.dll --migrate

echo "🚀 Starting Paynau API..."
exec dotnet Paynau.Api.dll
