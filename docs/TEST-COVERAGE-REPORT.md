# TEST COVERAGE REPORT

---

### **Capa DOMAIN**

#### `ProductTests.cs`

| #  | Descripción                                                           | Estado |
| -- | --------------------------------------------------------------------- | ------ |
| 1  | Crear producto válido                                                 | [OK]   |
| 2  | Crear producto con nombre vacío → `DomainException`                   | [OK]   |
| 3  | Crear producto con precio negativo → `DomainException`                | [OK]   |
| 4  | Crear producto con stock negativo → `DomainException`                 | [OK]   |
| 5  | Disminuir stock con stock suficiente                                  | [OK]   |
| 6  | Disminuir stock con stock insuficiente → `InsufficientStockException` | [OK]   |
| 7  | Disminuir stock con cantidad cero → `InvalidQuantityException`        | [OK]   |
| 8  | Actualizar producto con datos válidos                                 | [OK]   |
| 9  | Validar `HasSufficientStock()` con stock suficiente                   | [OK]   |
| 10 | Validar `HasSufficientStock()` con stock insuficiente                 | [OK]   |

➡️ **Cobertura:** 100% de reglas de validación y stock en entidad `Product`.

---

#### `OrderDomainServiceTests.cs`

| # | Descripción                                                       | Estado  |
| - | ----------------------------------------------------------------- | ------- |
| 1 | Crear orden con stock suficiente → crea orden y descuenta stock   | [OK]    |
| 2 | Crear orden con stock insuficiente → `InsufficientStockException` | [OK]    |
| 3 | Crear orden con producto nulo → `ArgumentNullException`           | [OK]    |
| 4 | Crear dos órdenes simultáneas (verificación de concurrencia)      | [FALTA] |

➡️ **Cobertura:** Lógica de negocio [OK]; falta escenario de **concurrencia optimista** (O4 de reglas de negocio).

---

### **Capa APPLICATION**

#### `CreateProductHandlerTests.cs`

| # | Descripción                                            | Estado |
| - | ------------------------------------------------------ | ------ |
| 1 | Crear producto válido → persiste y guarda cambios      | [OK]   |
| 2 | Crear producto duplicado → `DuplicateProductException` | [OK]   |

➡️ **Cobertura:** Casos principales de creación de producto [OK].

---

#### `CreateOrderHandlerTests.cs`

| # | Descripción                                                                              | Estado  |
| - | ---------------------------------------------------------------------------------------- | ------- |
| 1 | Crear orden válida → genera orden y descuenta stock                                      | [OK]    |
| 2 | Producto inexistente → `ProductNotFoundException`                                        | [OK]    |
| 3 | Stock insuficiente → `DomainException` con `InnerException` `InsufficientStockException` | [OK]    |
| 4 | Concurrencia simultánea (dos órdenes al mismo producto)                                  | [FALTA] |

➡️ **Cobertura:** Flujo de negocio de orden correcto [OK]; **no valida concurrencia [FALTA]**.

---

#### `ValidatorTests.cs`

| # | Descripción                                | Estado |
| - | ------------------------------------------ | ------ |
| 1 | Validar `CreateProductCommand` correcto    | [OK]   |
| 2 | `Name` vacío → error en `Name`             | [OK]   |
| 3 | `Price` negativo → error en `Price`        | [OK]   |
| 4 | Validar `CreateOrderCommand` correcto      | [OK]   |
| 5 | `Quantity` igual a 0 → error en `Quantity` | [OK]   |

➡️ **Cobertura:** Validadores de comandos [OK].

---

### **Resumen general**

| Categoría                 | Archivo                        | Estado general                    |
| ------------------------- | ------------------------------ | --------------------------------- |
| Entidad Product           | `ProductTests.cs`              | [OK]                              |
| Servicio de dominio Order | `OrderDomainServiceTests.cs`   | [FALTA] (test de concurrencia)    |
| Handler de producto       | `CreateProductHandlerTests.cs` | [OK]                              |
| Handler de orden          | `CreateOrderHandlerTests.cs`   | [FALTA] (concurrencia simultánea) |
| Validadores               | `ValidatorTests.cs`            | [OK]                              |

---