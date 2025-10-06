# Guía de Migraciones - Paynau Backend

## Descripción general

Esta guía explica cómo crear, gestionar y aplicar migraciones de base de datos para el backend de Paynau.

## Requisitos previos

* SDK de .NET 8 instalado
* Herramientas de EF Core instaladas globalmente:

  ```bash
  dotnet tool install --global dotnet-ef
  ```
* Servidor MySQL en ejecución (o usar Docker Compose)

## Crear la migración inicial

### Paso 1: Ir al proyecto API

```bash
cd src/Paynau.Api
```

### Paso 2: Crear la migración inicial

```bash
dotnet ef migrations add InitialCreate
```

Esto creará:

* `Migrations/YYYYMMDDHHMMSS_InitialCreate.cs` – Código de migración
* `Migrations/PaynauDbContextModelSnapshot.cs` – Snapshot del modelo

### Paso 3: Revisar la migración

Verifica en el archivo generado:

* Creación de tablas
* Definición de columnas
* Índices y restricciones
* Relaciones de claves foráneas

### Paso 4: Aplicar la migración

```bash
dotnet ef database update
```

Esto hará:

1. Crear la base de datos si no existe
2. Aplicar todas las migraciones pendientes
3. Actualizar la tabla `__EFMigrationsHistory`

## Agregar nuevas migraciones

### Ejemplo: agregar un nuevo campo a `Product`

1. **Modificar la entidad**
2. **Actualizar la configuración**
3. **Crear migración**
4. **Revisar y aplicar**

## Gestión de migraciones

* Listar todas las migraciones:

  ```bash
  dotnet ef migrations list
  ```

* Eliminar la última migración (si no fue aplicada):

  ```bash
  dotnet ef migrations remove
  ```

* Revertir a una migración específica:

  ```bash
  dotnet ef database update PreviousMigrationName
  ```

* Generar script SQL:

  ```bash
  dotnet ef migrations script
  ```

## Despliegue en producción

* **Opción 1:** Aplicar migraciones automáticamente en desarrollo
* **Opción 2:** Generar script SQL y ejecutarlo manualmente
* **Opción 3:** Usar Migration Bundle

## Datos iniciales (Seed)

* **Desarrollo:** Se cargan automáticamente desde archivos JSON.
* **Producción:**

  * Usar `HasData()`
  * Crear scripts dedicados
  * Usar scripts de inicialización

## Solución de problemas

* Migración ya aplicada → `dotnet ef migrations remove --force`
* Base de datos fuera de sincronía → Drop y recreate
* Problemas de concurrencia → usar `RowVersion`

## Entorno Docker

En modo desarrollo, las migraciones se aplican automáticamente al iniciar el contenedor.

Migración manual en Docker:

```bash
docker-compose exec backend dotnet ef database update --project /app/Paynau.Api.csproj
```

## Buenas prácticas

1. Revisa siempre las migraciones antes de aplicarlas
2. Prueba las migraciones primero en desarrollo
3. Mantén las migraciones pequeñas y específicas
4. Nombra las migraciones de forma descriptiva
5. Nunca modifiques migraciones ya aplicadas
6. Realiza backup antes de aplicar en producción
7. Usa transacciones en migraciones de datos
8. Documenta los cambios importantes

## Convenciones de nombres

* `InitialCreate` – Primera migración
* `AddXToY` – Agrega columna X a tabla Y
* `RemoveXFromY` – Elimina columna X de tabla Y
* `CreateZTable` – Crea la tabla Z
* `UpdateXConstraint` – Modifica una restricción
* `AddXIndex` – Agrega un índice

## Estrategia de rollback

* **Desarrollo:** Revertir con `dotnet ef database update PreviousMigration`
* **Producción:**

  1. Mantener scripts SQL de respaldo
  2. Probar rollback en staging
  3. Documentar pasos
  4. Tener ventana de mantenimiento
  5. Monitorear la app después