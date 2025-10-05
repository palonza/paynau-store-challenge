# Paynau Backend

## Descripción general

Paynau Backend es una API REST lista para producción desarrollada con .NET 8, que implementa principios de Clean Architecture, el patrón CQRS con MediatR y Entity Framework Core con MySQL.

## Características principales

* Clean Architecture (Domain, Application, Infrastructure, API)
* CQRS con MediatR
* Entity Framework Core con MySQL
* Documentación Swagger/OpenAPI
* Autenticación JWT
* Serilog con integración en Logtail
* Soporte para Docker
* Carga inicial automática de base de datos (seeding)
* Validaciones con FluentValidation
* Control de concurrencia optimista
* Manejo exhaustivo de excepciones

## Requisitos previos

* .NET 8 SDK
* Docker y Docker Compose
* MySQL 8.0+ (si se ejecuta sin Docker)

## Primeros pasos

### Usando Docker Compose (recomendado)

1. Clonar el repositorio
2. Navegar a la carpeta `backend`
3. Ejecutar la aplicación:

```bash
docker-compose up --build
```

La API estará disponible en: `http://localhost:5001`

### Ejecución local

1. Actualizar la cadena de conexión en `appsettings.json`
2. Ejecutar las migraciones:

```bash
cd src/Paynau.Api
dotnet ef database update
```

3. Iniciar la aplicación:

```bash
dotnet run
```

## Endpoints de la API

### Productos

* `GET /api/products` - Obtener todos los productos
* `GET /api/products/{id}` - Obtener un producto por ID
* `POST /api/products` - Crear un nuevo producto
* `PUT /api/products/{id}` - Actualizar un producto
* `DELETE /api/products/{id}` - Eliminar un producto

### Órdenes

* `GET /api/orders` - Obtener todas las órdenes
* `POST /api/orders` - Crear una nueva orden

## Documentación Swagger

Podés acceder a la interfaz Swagger en:
`http://localhost:5001/swagger`

## Estructura del proyecto

```
backend/
├── src/
│   ├── Paynau.Domain/          # Entidades de dominio, excepciones, servicios
│   ├── Paynau.Application/     # Casos de uso, DTOs, validadores
│   ├── Paynau.Infrastructure/  # Acceso a datos, repositorios, EF Core
│   └── Paynau.Api/             # Controladores, middleware, startup
└── tests/
    └── Paynau.Tests/           # Pruebas unitarias e integrales
```

## Ejecución de pruebas

```bash
dotnet test
```

## Variables de entorno

Configurá las siguientes variables en el archivo `.env`:

* `MYSQL_ROOT_PASSWORD` - Contraseña del usuario root de MySQL
* `JWT_ISSUER` - Emisor del token JWT
* `JWT_AUDIENCE` - Audiencia del token JWT
* `JWT_KEY` - Clave de firma JWT (mínimo 32 caracteres)
* `LOGTAIL_ENABLED` - Habilitar el logging en Logtail
* `LOGTAIL_SOURCE_TOKEN` - Token de fuente para Logtail

## Migraciones de base de datos

Crear una nueva migración:

```bash
cd src/Paynau.Api
dotnet ef migrations add MigrationName
```

Aplicar migraciones:

```bash
dotnet ef database update
```

## Reglas de negocio

* Los productos deben tener nombre, descripción, precio y stock
* El stock no puede ser negativo
* Solo se pueden crear órdenes si hay stock suficiente
* El stock se descuenta inmediatamente al crear una orden
* Se aplica control de concurrencia optimista en actualizaciones de productos
* Los productos asociados a órdenes no pueden eliminarse
