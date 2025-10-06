# 📘 Documento 6 – Historias de Usuario, Casos de Uso, Casos de Prueba y Datos de Prueba

## 1. 🎯 Introducción y objetivos del documento

Este documento define de forma detallada las **Historias de Usuario (HU)**, **Casos de Uso (CU)**, **Casos de Prueba (TC)** y **Datos de Prueba** para el proyecto **Paynau**. Su objetivo es proporcionar un marco completo y trazable para que el sistema cumpla con los requerimientos funcionales y de negocio, garantizando calidad, consistencia y automatización en el desarrollo.

La información aquí contenida servirá de guía para:

* 🧪 Implementar pruebas unitarias y funcionales automatizadas.
* 📋 Validar el cumplimiento de las reglas de negocio.
* 🧰 Preparar datos semilla y escenarios controlados para ambientes de desarrollo y QA.

---

## 2. 🧩 Historias de Usuario (HU)

> Las HU están redactadas desde la perspectiva del **rol de usuario**.

| ID     | Prioridad | Historia de Usuario                                                                                                                             |
| ------ | --------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| HU-001 | Alta      | Como **administrador**, quiero **crear productos con nombre, descripción, precio y stock** para que estén disponibles en el catálogo.           |
| HU-002 | Media     | Como **usuario**, quiero **ver un listado actualizado de productos** con su stock disponible para decidir mi compra.                            |
| HU-003 | Alta      | Como **cliente**, quiero **crear órdenes de compra con productos disponibles** para completar mi compra exitosamente.                           |
| HU-004 | Alta      | Como **cliente**, quiero **ver el stock actualizado inmediatamente después de realizar una orden** para saber qué productos siguen disponibles. |

### Criterios de aceptación (Gherkin)

#### HU-001 – Crear producto

* ✅ **Scenario:** Crear producto válido
  **Given** que el administrador completa todos los campos requeridos
  **When** confirma el registro
  **Then** el producto queda disponible con su stock inicial.
* ❌ **Scenario:** Stock negativo
  **Given** que el administrador ingresa un stock menor a 0
  **Then** el sistema muestra un error y rechaza el producto.

#### HU-002 – Listar productos

* ✅ **Scenario:** Listado con productos
  **Then** el usuario ve nombre, descripción, precio y stock.
* ✅ **Scenario:** Sin stock
  **Then** el sistema indica que no hay disponibilidad o no muestra el producto.

#### HU-003 – Crear orden

* ✅ **Scenario:** Orden válida
  **Then** el sistema crea la orden y descuenta el stock.
* ❌ **Scenario:** Stock insuficiente
  **Then** el sistema muestra error y no crea la orden.

#### HU-004 – Stock actualizado

* ✅ **Scenario:** Ver stock luego de compra
  **Then** el stock reflejado es el actualizado tras la orden.

---

## 3. 📜 Casos de Uso (CU)

> Los CU están redactados con un enfoque **técnico**, orientado al comportamiento del sistema.

---

### CU-001 – Registrar producto

**Actor:** Administrador
**Precondiciones:** Autenticación válida.
**Flujo principal:**

1. El sistema recibe un `POST /api/products` con nombre, descripción, precio y stock.
2. Valida que los datos sean correctos.
3. Persiste el producto en la base de datos.
4. Devuelve `201 Created` con el producto creado.

**Flujos alternativos:**

* A1: Stock negativo → `400 Bad Request`.
* A2: Nombre duplicado → `409 Conflict`.

---

### CU-002 – Listar productos

**Actor:** Usuario
**Precondiciones:** Existen productos en la base de datos.
**Flujo principal:**

1. El sistema recibe un `GET /api/products`.
2. Obtiene el listado desde la base de datos.
3. Devuelve `200 OK` con nombre, descripción, precio y stock.

**Flujos alternativos:**

* A1: No hay productos → `200 OK` con lista vacía.

---

### CU-003 – Crear orden

**Actor:** Cliente
**Precondiciones:** Productos existentes y stock disponible.
**Flujo principal:**

1. El sistema recibe un `POST /api/orders` con `ProductId` y `Quantity`.
2. Valida existencia del producto y stock suficiente.
3. Calcula el `Total`.
4. Descuenta el stock.
5. Persiste la orden.
6. Devuelve `201 Created`.

**Flujos alternativos:**

* A1: Producto no existe → `404 Not Found`.
* A2: Stock insuficiente → `400 Bad Request`.
* A3: Cantidad inválida (0 o < 0) → `400 Bad Request`.
* A4: Concurrencia (stock agotado en orden paralela) → `409 Conflict`

---

### CU-004 – Actualizar producto

**Actor:** Administrador
**Precondiciones:** Producto existente.

**Flujo principal:**

1. El sistema recibe un `PUT /api/products/{id}` con los campos a actualizar.
2. Valida que los datos sean correctos.
3. Actualiza el producto.
4. Devuelve **`200 OK`**.

**Flujos alternativos:**

* A1: Producto no existe → `404 Not Found`**
* A2: Datos inválidos → `400 Bad Request`**
* A3: Nombre duplicado → `409 Conflict`**

---

### CU-005 – Eliminar producto

**Actor:** Administrador

**Flujo principal:**

1. El sistema recibe un `DELETE /api/products/{id}`.
2. Elimina el producto.
3. Devuelve **`204 No Content`**.

**Flujos alternativos:**

* A1: Producto no existe → `404 Not Found`**
* A2: Producto con órdenes activas → `409 Conflict`**

---

### CU-006 – Obtener producto por ID

**Actor:** Usuario / Cliente

**Flujo principal:**

1. El sistema recibe un `GET /api/products/{id}`.
2. Devuelve **`200 OK`** con los detalles del producto.

**Flujos alternativos:**

* A1: Producto no existe → `404 Not Found`**

---

### CU-007 – Listar órdenes

**Actor:** Administrador / Cliente

**Flujo principal:**

1. El sistema recibe un `GET /api/orders`.
2. Devuelve **`200 OK`** con el listado de órdenes.

**Flujos alternativos:**

* A1: No existen órdenes → `204 No Content`**

---

## 4. 🧪 Casos de Prueba (TC)

> Casos unitarios y funcionales clásicos.

| ID     | Caso de Prueba                               | Precondiciones                | Pasos principales                             | Resultado esperado                                    |
| ------ | -------------------------------------------- | ----------------------------- | --------------------------------------------- | ----------------------------------------------------- |
| TC-001 | Crear producto válido                        | Usuario admin autenticado     | POST `/api/products` con datos correctos      | `201 Created`, producto visible en listado            |
| TC-002 | Error por stock negativo                     | Usuario admin autenticado     | POST `/api/products` con stock -5             | `400 Bad Request`, mensaje `Stock cannot be negative` |
| TC-003 | Listar productos con resultados              | Productos existentes          | GET `/api/products`                           | `200 OK`, listado no vacío                            |
| TC-004 | Listar productos sin resultados              | No hay productos              | GET `/api/products`                           | `200 OK`, lista vacía                                 |
| TC-005 | Crear orden válida                           | Producto con stock suficiente | POST `/api/orders` con `quantity <= stock`    | `201 Created`, stock actualizado                      |
| TC-006 | Crear orden con stock insuficiente           | Producto con stock insuf.     | POST `/api/orders` con `quantity > stock`     | `400 Bad Request`, error de negocio                   |
| TC-007 | Crear orden con producto inexistente         | Producto no existe            | POST `/api/orders` con `productId` inválido   | `404 Not Found`                                       |
| TC-008 | Crear orden con cantidad cero o negativa     | Producto existe               | POST `/api/orders` con `quantity = 0` o `< 0` | `400 Bad Request`                                     |
| TC-009 | Concurrencia al crear órdenes simultáneas    | Stock bajo                    | Dos POST `/api/orders` simultáneos            | Una orden exitosa, otra `409 Conflict`                |
| TC-010 | Verificar stock actualizado después de orden | Producto con stock inicial    | POST `/api/orders` → GET `/api/products`      | Stock descontado reflejado correctamente              |

---

## 🧪 Casos de Prueba Adicionales – Extensión CRUD y Órdenes

---

### 🔧 CU-004 – Actualizar producto (`PUT /api/products/{id}`)

| ID     | Caso de Prueba                  | Precondiciones                             | Pasos principales                                                              | Resultado esperado                               |
| ------ | ------------------------------- | ------------------------------------------ | ------------------------------------------------------------------------------ | ------------------------------------------------ |
| TC-011 | Actualizar producto válido      | Producto existente                         | 1. Enviar `PUT /api/products/1` con campos válidos. <br>2. Consultar producto. | ✅ `200 OK` – Producto actualizado correctamente. |
| TC-012 | Actualizar producto inexistente | Producto no existe                         | 1. Enviar `PUT /api/products/999` con datos válidos.                           | ❌ `404 Not Found` – Producto no encontrado.      |
| TC-013 | Actualizar con datos inválidos  | Producto existente                         | 1. Enviar `PUT /api/products/1` con `stock: -5` o `price: -100`.               | ❌ `400 Bad Request` – Validación fallida.        |
| TC-014 | Actualizar con nombre duplicado | Producto existente y otro con mismo nombre | 1. Enviar `PUT /api/products/1` con nombre igual a producto existente.         | ❌ `409 Conflict` – Nombre duplicado.             |

---

### 🗑️ CU-005 – Eliminar producto (`DELETE /api/products/{id}`)

| ID     | Caso de Prueba                          | Precondiciones                           | Pasos principales                                              | Resultado esperado                                            |
| ------ | --------------------------------------- | ---------------------------------------- | -------------------------------------------------------------- | ------------------------------------------------------------- |
| TC-015 | Eliminar producto válido                | Producto existente                       | 1. Enviar `DELETE /api/products/1`. <br>2. Consultar producto. | ✅ `204 No Content` – Producto eliminado.                      |
| TC-016 | Eliminar producto inexistente           | Producto no existe                       | 1. Enviar `DELETE /api/products/999`.                          | ❌ `404 Not Found` – Producto no encontrado.                   |
| TC-017 | Eliminar producto con órdenes asociadas | Producto con órdenes previas registradas | 1. Crear orden. <br>2. Intentar eliminar producto asociado.    | ❌ `409 Conflict` – Eliminación no permitida por dependencias. |

---

### 🔍 CU-006 – Obtener producto por ID (`GET /api/products/{id}`)

| ID     | Caso de Prueba                 | Precondiciones     | Pasos principales                  | Resultado esperado                             |
| ------ | ------------------------------ | ------------------ | ---------------------------------- | ---------------------------------------------- |
| TC-018 | Consultar producto existente   | Producto creado    | 1. Enviar `GET /api/products/1`.   | ✅ `200 OK` – Detalles del producto retornados. |
| TC-019 | Consultar producto inexistente | Producto no existe | 1. Enviar `GET /api/products/999`. | ❌ `404 Not Found` – Producto no encontrado.    |

---

### 📜 CU-007 – Listar órdenes (`GET /api/orders`)

| ID     | Caso de Prueba                | Precondiciones                 | Pasos principales            | Resultado esperado                                         |
| ------ | ----------------------------- | ------------------------------ | ---------------------------- | ---------------------------------------------------------- |
| TC-020 | Listar órdenes existentes     | Al menos una orden registrada  | 1. Enviar `GET /api/orders`. | ✅ `200 OK` – Lista de órdenes retornada correctamente.     |
| TC-021 | Listar órdenes sin resultados | No existen órdenes registradas | 1. Enviar `GET /api/orders`. | ✅ `204 No Content` – Respuesta sin contenido pero exitosa. |

---

## 📊 Cobertura Final de Casos de Prueba (Resumen)

| Módulo                  | Rango de TC     | Estado     |
| ----------------------- | --------------- | ---------- |
| Crear producto          | TC-001 – TC-002 | ✅ Completo |
| Listar productos        | TC-003 – TC-004 | ✅ Completo |
| Crear orden             | TC-005 – TC-010 | ✅ Completo |
| Actualizar producto     | TC-011 – TC-014 | ✅ Completo |
| Eliminar producto       | TC-015 – TC-017 | ✅ Completo |
| Obtener producto por ID | TC-018 – TC-019 | ✅ Completo |
| Listar órdenes          | TC-020 – TC-021 | ✅ Completo |

---

## 5. 📊 Matriz de trazabilidad HU ↔ CU ↔ TC

| Historia de Usuario (HU)                                                                                          | Caso de Uso (CU)                 | Casos de Prueba Asociados                                                                                                                    |
| ----------------------------------------------------------------------------------------------------------------- | -------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| **HU-001 – Crear producto**<br>Como administrador quiero crear productos con nombre, descripción, precio y stock. | CU-001 – Registrar producto      | TC-001 (crear producto válido)<br>TC-002 (stock negativo)<br>TC-010 (duplicado)                                                              |
| **HU-001 – Crear producto**                                                                                       | CU-004 – Actualizar producto     | TC-011 (actualizar producto válido)<br>TC-012 (producto inexistente)<br>TC-013 (datos inválidos)<br>TC-014 (nombre duplicado)                |
| **HU-001 – Crear producto**                                                                                       | CU-005 – Eliminar producto       | TC-015 (eliminar producto válido)<br>TC-016 (producto inexistente)<br>TC-017 (conflicto por órdenes activas)                                 |
| **HU-002 – Listar productos**<br>Como usuario quiero ver el listado de productos con su stock disponible.         | CU-002 – Listar productos        | TC-003 (listado con resultados)<br>TC-004 (lista vacía)                                                                                      |
| **HU-002 – Listar productos**                                                                                     | CU-006 – Obtener producto por ID | TC-018 (producto existente)<br>TC-019 (producto inexistente)                                                                                 |
| **HU-003 – Crear orden**<br>Como cliente quiero generar órdenes válidas con stock suficiente.                     | CU-003 – Crear orden             | TC-005 (orden válida)<br>TC-006 (stock insuficiente)<br>TC-007 (producto inexistente)<br>TC-008 (cantidad inválida)<br>TC-009 (concurrencia) |
| **HU-004 – Ver stock actualizado**<br>Como cliente quiero ver el stock reflejado tras realizar la compra.         | CU-003 – Crear orden             | TC-010 (stock actualizado)                                                                                                                   |
| **HU-004 – Ver stock actualizado**                                                                                | CU-007 – Listar órdenes          | TC-020 (listar órdenes existentes)<br>TC-021 (sin órdenes registradas)                                                                       |

---

## 6. 📦 Datos de prueba detallados

Los datos de prueba necesarios para ejecutar los casos descritos se encuentran en los siguientes archivos:

* 📁 **`products.json`** – Contiene datos de productos válidos, inválidos, duplicados, con stock negativo, etc.
* 📁 **`orders.json`** – Contiene casos de órdenes válidas e inválidas, concurrencia, productos inexistentes, etc.

👉 **Ver archivos:**

* `products.json`
* `orders.json`

---

## 7. ✅ Definición de “Hecho” (DoD) ampliada

* ✅ Todas las Historias de Usuario cuentan con al menos un Caso de Uso y uno o más Casos de Prueba asociados.
* ✅ Todos los flujos principales y alternativos están cubiertos por casos de prueba automatizables con sus códigos de respuesta esperados (`200`, `201`, `400`, `404`, `409`, etc.).
* ✅ Todos los endpoints REST implementados están cubiertos por pruebas unitarias y funcionales.
* ✅ La cobertura de pruebas unitarias es ≥ **70 %** en los módulos críticos (servicios de productos, órdenes y validaciones de stock).
* ✅ El sistema valida todas las reglas de negocio en la capa de dominio, incluyendo concurrencia, integridad de stock y condiciones de error.
* ✅ Los mensajes de error devueltos por la API son específicos, verificables y coherentes con los códigos de estado.
* ✅ Los datos de prueba (`products.json`, `orders.json`) cubren escenarios válidos, inválidos, duplicados, negativos y de concurrencia.
* ✅ La API genera correctamente la documentación **Swagger/OpenAPI**, y los contratos están listos para ser consumidos desde el frontend.
* ✅ El entorno de desarrollo puede levantarse completamente mediante `docker-compose` (incluyendo API y MySQL) usando variables de entorno definidas.
* ✅ Los scripts de build y test están declarados explícitamente tanto en backend como en frontend para permitir su ejecución en CI/CD.

---

### 📍 Conclusión

Este documento representa el **artefacto QA base** del proyecto Paynau.
Claude Sonnet puede usarlo para:

* Este documento constituye la **especificación QA completa y definitiva** del proyecto **Paynau**.
* Define de forma detallada las historias de usuario, casos de uso, casos de prueba, criterios de aceptación y trazabilidad.
* Incluye escenarios positivos, negativos, de error y de concurrencia con sus códigos de respuesta esperados.
* Permite generar suites de pruebas unitarias y funcionales automatizadas sin información adicional.
* Facilita la validación de contratos de API y garantiza que el comportamiento de todos los endpoints REST sea verificable.
* Contiene datos de prueba (`products.json` y `orders.json`) listos para ejecutar pruebas de negocio automáticamente.
* Asegura trazabilidad completa desde las historias de usuario hasta los casos de prueba.
* Permite crear fixtures, asserts y validadores automáticos integrables en pipelines de CI/CD.
* Sirve como base confiable para auditorías técnicas, revisiones de código y pruebas de aceptación.

---