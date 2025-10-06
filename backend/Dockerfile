# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Paynau.sln", "./"]
COPY ["src/Paynau.Domain/Paynau.Domain.csproj", "src/Paynau.Domain/"]
COPY ["src/Paynau.Application/Paynau.Application.csproj", "src/Paynau.Application/"]
COPY ["src/Paynau.Infrastructure/Paynau.Infrastructure.csproj", "src/Paynau.Infrastructure/"]
COPY ["src/Paynau.Api/Paynau.Api.csproj", "src/Paynau.Api/"]

# Restore dependencies
RUN dotnet restore "Paynau.sln"

# Copy all source code (INCLUDING MIGRATIONS)
COPY . .

# Build the application
WORKDIR "/src/src/Paynau.Api"
RUN dotnet build "Paynau.Api.csproj" -c Release -o /app/build

# ---- Publish stage ----
FROM build AS publish
RUN dotnet publish "Paynau.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime stage ----
# 🔧 FIX: Usar Debian en lugar de Alpine para evitar segfaults
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Copy seed files
COPY ["src/Paynau.Infrastructure/Seed/products.json", "Seed/"]
COPY ["src/Paynau.Infrastructure/Seed/orders.json", "Seed/"]

# Copiar archivos de migración (necesarios para EF Core)
COPY --from=build /src/src/Paynau.Infrastructure/Migrations /app/Migrations

# Copy custom entrypoint script
COPY migrate-and-run.sh /app/

RUN chmod +x /app/migrate-and-run.sh

EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001

ENTRYPOINT ["bash", "/app/migrate-and-run.sh"]