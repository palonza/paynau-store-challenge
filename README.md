
# Paynau Store

Este documento explica cómo **construir, ejecutar y verificar** el entorno completo del proyecto **Paynau** usando Docker.
Incluye backend (.NET 8), frontend (React + Vite) y base de datos MySQL, conforme a las guías oficiales del proyecto.

---

## ⚙️ Requisitos previos

Antes de ejecutar el entorno asegurate de tener instalados:

* **Docker** y **Docker Compose**
* **.NET 8 SDK** (para desarrollo local, opcional)
* **Node.js 20+** (solo si querés correr el frontend fuera del contenedor)

---

## Ejecución paso a paso

### Backend (API .NET 8)

```bash
cd backend/
./docker-build.sh && ./docker-run.sh
```

Esto:

* Construye la imagen **multi-stage** (`sdk → runtime`).
* Levanta la API en `http://localhost:5001`.
* Conecta automáticamente con el contenedor MySQL.

---

### Frontend (React + Vite)

```bash
cd ../frontend/
docker compose up -d
```

Esto inicia:

* El contenedor del frontend servido con **Nginx**.
* Accesible en `http://localhost:3001`.

---

## Verificación de datos

### Verificar productos cargados

```bash
docker exec -it paynau-mysql mysql -u root -pMySqlP@ssw0rd \
  -e "SELECT COUNT(*) FROM PaynauDb.Products;"
```

### Verificar órdenes cargadas

```bash
docker exec -it paynau-mysql mysql -u root -pMySqlP@ssw0rd \
  -e "SELECT COUNT(*) FROM PaynauDb.Orders;"
```

---

## Probar el backend (API)

### Llamada básica

```bash
curl -i http://localhost:5001/api/products
```

Deberías obtener una respuesta `200 OK` con un listado de productos y un token **JWT** generado automáticamente.

---

## Accesos rápidos

| Servicio     | URL                                                            | Descripción                   |
| ------------ | -------------------------------------------------------------- | ----------------------------- |
| **Frontend** | [http://localhost:3001](http://localhost:3001)                 | UI React (Vite + Material UI) |
| **Backend**  | [http://localhost:5001](http://localhost:5001)                 | API REST .NET 8               |
| **Swagger**  | [http://localhost:5001/swagger](http://localhost:5001/swagger) | Documentación OpenAPI         |
| **MySQL**    | localhost:3306                                                 | Base de datos `PaynauDb`      |

