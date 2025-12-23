📦 Stock API

API REST desarrollada en ASP.NET Core para la gestión de productos y stock, como parte de un challenge técnico con seguimiento.

El objetivo del proyecto es demostrar buenas prácticas de arquitectura, separación de responsabilidades y un flujo de desarrollo incremental y controlado.

🚀 Tecnologías utilizadas

ASP.NET Core Entity Framework Core SQL Server Swagger / OpenAPI

🧱 Arquitectura

El proyecto está organizado siguiendo una arquitectura en capas, separando claramente responsabilidades:

Stock.Api Stock.Application Stock.Infrastructure

🔹 Stock.Api (Presentation)

Expone los endpoints HTTP Maneja DTOs y validaciones básicas de entrada Configura middlewares, Swagger y DI

🔹 Stock.Application (Business)

Contiene la lógica de negocio Define contratos (interfaces) para servicios y repositorios Modelos de dominio desacoplados de la persistencia

🔹 Stock.Infrastructure (Data)

Implementa la persistencia con EF Core Define entidades de base de datos Configuraciones de EF y migraciones

Esta separación evita acoplamientos innecesarios y facilita mantenimiento, testing y escalabilidad.

🧠 Decisiones de diseño relevantes 🔸 Separación de modelos y entidades

Existen dos clases Product:

Application.Models.Product → modelo de negocio
Infrastructure.Entities.Product → entidad de persistencia
Esto desacopla la lógica de negocio de EF Core y de la base de datos.

🔸 Repositorio + Service El acceso a datos se realiza mediante repositorios, mientras que los servicios encapsulan reglas de negocio y flujos de trabajo.

🔸 EF Core con migraciones El modelo se definió primero en código y luego se generó la base de datos mediante migraciones, evitando dependencias tempranas con la BD.

🔸 Desarrollo incremental Se comenzó con InMemoryDatabase para validar la arquitectura y luego se migró a SQL Server sin modificar la lógica.

📚 Funcionalidad implementada ✔ CRUD de Products

Endpoints disponibles:

GET /api/products GET /api/products/{id} POST /api/products PUT /api/products/{id} DELETE /api/products/{id}

El CRUD fue probado completamente desde Swagger, validando:

Inyección de dependencias Persistencia en SQL Server Correcto flujo DTO → Servicio → Repositorio → Base de datos

🛠 Base de datos

SQL Server Migraciones con Entity Framework Core Tablas generadas automáticamente a partir del modelo

📌 Estado actual

✔ Arquitectura base completa ✔ CRUD funcional y probado ✔ Proyecto listo para extender con:

validaciones soft delete paginado y filtros nuevas entidades relacionadas a stock

📝 Notas

Este repositorio refleja el progreso real del challenge, priorizando claridad, buenas prácticas y decisiones justificadas por sobre soluciones rápidas o acopladas.

19/12 La búsqueda se centralizó en un único endpoint utilizando query parameters, evitando duplicación de endpoints y retornando siempre estructuras válidas con 200 OK, incluso cuando no hay resultados.

---

# Stock.Tests (xUnit)

Este proyecto contiene **unit tests** del backend. El foco está en validar:
- reglas de negocio (Services)
- validaciones de DTOs (FluentValidation)
- comportamiento de Repositories usando **EF Core InMemory**

La idea es cubrir lo más evaluable del challenge con pruebas rápidas, aisladas y fáciles de ejecutar.

---

## ¿Por qué unit tests y por qué en Services?

En esta solución, las reglas importantes viven en `Stock.Application.Services`:

- Validaciones de negocio (ej. no permitir cantidades inválidas).
- Decisiones según tipo de operación (Entry / Exit / Adjustment).
- Soft delete y actualización controlada de productos.
- Delegación correcta hacia repositorios (contratos).

Por eso se priorizaron unit tests: son rápidos, aislados y verifican lo más crítico del sistema.

---

## Qué cubren estos tests

### ✅ StockServiceTests
Reglas centrales del sistema de stock:
- `quantity <= 0` debe fallar.
- Producto inexistente/inactivo: no se permite operar.
- `Exit` con stock insuficiente: se rechaza.
- Caso válido: el service llama exactamente una vez a `ApplyMovementAsync`.

### ✅ ProductServiceTests
Comportamiento esperado para Products:
- Paginado y filtros: el service delega el paginado al repositorio.
- `GetById`: delega en `GetActiveByIdAsync`.
- `Create`: se aplica `Trim()` al nombre antes de persistir.
- `Update`:
  - si no existe → retorna `false` y no actualiza
  - si existe → actualiza campos + trim y llama al repo
- `Delete`: usa soft delete (`SoftDeleteAsync`).

### ✅ ProductValidatorsTests (FluentValidation)
Validación de DTOs:
- `Name` obligatorio.
- `MinStock >= 0`.
- Casos válidos pasan correctamente.

> Nota: se testean validators directamente con `validator.Validate(dto)` (sin TestHelper).

### ✅ ProductRepositoryTests (EF Core InMemory)
Pruebas de persistencia con DB en memoria:
- `GetPagedActiveAsync` devuelve **solo activos**
- Aplica filtro por nombre + paginado.

> Nota: EF InMemory puede comportarse distinto a SQL Server en filtros por string (case-sensitive).  
> Por eso los datos del test se preparan acorde para que el comportamiento sea estable.

### ✅ StockAlertsFlagTests (EF Core InMemory)
Valida la regla de **alerta de stock mínimo** como flag en `GET /api/stock`:
- `IsBelowMinStock = true` cuando `Quantity <= MinStock`.
- `false` cuando `Quantity > MinStock`.

> Nota: `ProductStock.RowVersion` es obligatorio en tests con InMemory por la configuración de concurrency,
> por eso se setea manualmente en los registros de prueba.

---

## Qué no se testea (y por qué)

- **Controllers**: se pueden cubrir con integration tests (WebApplicationFactory), pero no era prioritario.
- **Auth JWT**: se validó funcionalmente en Swagger/Postman; automatizarlo sería un extra.
- **SQL Server real**: las pruebas de repo se hicieron con InMemory para mantener los tests rápidos y ejecutables en cualquier entorno.

---

## Herramientas

- **xUnit**: framework de testing.
- **Moq**: mocks para repositorios (aislar lógica del service).
- **EF Core InMemory**: pruebas de repositorios sin DB real.
- **Microsoft.NET.Test.Sdk + runner**: ejecución de tests en Visual Studio.

---

## Cómo ejecutar

### Desde Visual Studio
`Prueba` → `Explorador de pruebas` → `Ejecutar todo`

### Desde CLI
```bash
dotnet test
