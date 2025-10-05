FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY ["Paynau.sln", "./"]
COPY ["src/Paynau.Domain/Paynau.Domain.csproj", "src/Paynau.Domain/"]
COPY ["src/Paynau.Application/Paynau.Application.csproj", "src/Paynau.Application/"]
COPY ["src/Paynau.Infrastructure/Paynau.Infrastructure.csproj", "src/Paynau.Infrastructure/"]
COPY ["src/Paynau.Api/Paynau.Api.csproj", "src/Paynau.Api/"]

# Restore dependencies
RUN dotnet restore "Paynau.sln"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/Paynau.Api"
RUN dotnet build "Paynau.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Paynau.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage - runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Copy seed files
COPY ["src/Paynau.Infrastructure/Seed/products.json", "Seed/"]
COPY ["src/Paynau.Infrastructure/Seed/orders.json", "Seed/"]

EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001

ENTRYPOINT ["dotnet", "Paynau.Api.dll"]