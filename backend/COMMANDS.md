# Guía de Comandos Útiles – Paynau Backend

## Configuración de desarrollo

### Configuración inicial

```bash
# Clonar y navegar al directorio del backend
cd backend

# Restaurar dependencias
dotnet restore

# Compilar la solución
dotnet build
```

### Migraciones de base de datos

```bash
# Navegar al proyecto API
cd src/Paynau.Api

# Instalar herramientas de EF Core (si no están instaladas)
dotnet tool install --global dotnet-ef

# Crear una nueva migración
dotnet ef migrations add InitialCreate

# Actualizar la base de datos
dotnet ef database update

# Eliminar la última migración
dotnet ef migrations remove

# Generar script SQL
dotnet ef migrations script

# Eliminar la base de datos
dotnet ef database drop
```

## Ejecución de la aplicación

### Desarrollo local

```bash
# Ejecutar la API
cd src/Paynau.Api
dotnet run

# Ejecutar con auto-reload
dotnet watch run

# Ejecutar en un entorno específico
dotnet run --environment Production
```

### Desarrollo con Docker

```bash
# Construir y ejecutar con Docker Compose
docker-compose up --build

# Ejecutar en modo desacoplado (background)
docker-compose up -d

# Ver logs
docker-compose logs -f backend

# Detener contenedores
docker-compose down

# Detener y eliminar volúmenes
docker-compose down -v
```

## Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas con cobertura
dotnet test /p:CollectCoverage=true

# Ejecutar pruebas con salida detallada
dotnet test --logger "console;verbosity=detailed"

# Ejecutar un proyecto de pruebas específico
dotnet test tests/Paynau.Tests/Paynau.Tests.csproj
```

## Compilación

```bash
# Compilar en modo Debug
dotnet build

# Compilar en modo Release
dotnet build --configuration Release

# Limpiar artefactos de compilación
dotnet clean

# Publicar para despliegue
dotnet publish -c Release -o ./publish
```

## Calidad del código

```bash
# Formatear el código
dotnet format

# Restaurar paquetes NuGet
dotnet restore

# Listar paquetes desactualizados
dotnet list package --outdated

# Actualizar un paquete específico
dotnet add package PackageName --version X.X.X
```

## Comandos Docker

```bash
# Construir imagen Docker
docker build -t paynau-backend:latest .

# Ejecutar contenedor Docker
docker run -p 5001:5001 paynau-backend:latest

# Ver contenedores en ejecución
docker ps

# Ver logs
docker logs paynau-backend

# Detener contenedor
docker stop paynau-backend

# Eliminar contenedor
docker rm paynau-backend

# Eliminar imagen
docker rmi paynau-backend:latest
```

## Depuración

```bash
# Ejecutar con logging detallado
dotnet run --verbosity detailed

# Ver información de la aplicación
dotnet --info

# Listar todos los proyectos en la solución
dotnet sln list

# Buscar vulnerabilidades de seguridad
dotnet list package --vulnerable
```

## Despliegue en producción

```bash
# Construir versión optimizada
dotnet publish -c Release -o ./publish

# Crear imagen Docker para producción
docker build -t paynau-backend:v1.0.0 .

# Etiquetar imagen
docker tag paynau-backend:v1.0.0 registry.example.com/paynau-backend:v1.0.0

# Subir a un registro
docker push registry.example.com/paynau-backend:v1.0.0
```

## Variables de entorno

```bash
# Establecer variable de entorno (Linux/Mac)
export ASPNETCORE_ENVIRONMENT=Development

# Establecer variable de entorno (Windows PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Development"

# Establecer cadena de conexión
export ConnectionStrings__Default="Server=localhost;Database=PaynauDb;User=root;Password=pass;"
```

## Solución de problemas

```bash
# Limpiar caché de NuGet
dotnet nuget locals all --clear

# Reconstruir solución
dotnet clean
dotnet restore
dotnet build

# Verificar conexión a la base de datos
dotnet ef dbcontext info

# Listar todas las migraciones
dotnet ef migrations list

# Ver versión de EF Core
dotnet ef --version
```

