#!/bin/bash
set -e

echo "Waiting for MySQL to be ready..."

MYSQL_HOST="mysql"
MYSQL_PORT="3306"
MAX_RETRIES=30
RETRY_INTERVAL=2

for i in $(seq 1 $MAX_RETRIES); do
    if timeout 2 bash -c "cat < /dev/null > /dev/tcp/${MYSQL_HOST}/${MYSQL_PORT}" 2>/dev/null; then
        echo "MySQL is available at ${MYSQL_HOST}:${MYSQL_PORT}"
        break
    else
        if [ $i -eq $MAX_RETRIES ]; then
            echo "Failed to connect to MySQL after $MAX_RETRIES attempts"
            exit 1
        fi
        echo "Attempt $i/$MAX_RETRIES: MySQL not ready, waiting ${RETRY_INTERVAL}s..."
        sleep $RETRY_INTERVAL
    fi
done

echo "Giving MySQL additional time to initialize..."
sleep 5

dotnet Paynau.Api.dll --apply-migrations

echo "Starting Paynau API..."
echo "Migrations and seeding will be applied automatically"

exec dotnet /app/Paynau.Api.dll
