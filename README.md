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

Este proyecto contiene **unit tests** del backend, enfocados en validar la **lógica de negocio** y el comportamiento de la capa de **Services**.

## ¿Por qué unit tests y por qué en Services?
En esta solución, las reglas importantes viven en `Stock.Application.Services`:
- validaciones de negocio (ej. no permitir cantidades inválidas)
- decisiones según tipo de movimiento (Entry / Exit / Adjustment)
- soft delete y actualización controlada de productos
- delegación correcta hacia repositorios (contratos)

Por eso se priorizaron **unit tests**: son rápidos, aislados y verifican lo más crítico del challenge.

## Qué cubren estos tests

### StockService
Se testean reglas centrales del sistema de stock:
- **Cantidad inválida**: `quantity <= 0` debe fallar.
- **Producto inexistente/inactivo**: no se permite operar si el producto no está activo.
- **Salida con stock insuficiente**: en `Exit` debe rechazarse cuando no alcanza el stock.
- **Flujo correcto**: en un caso válido, el service llama exactamente una vez al repositorio (`ApplyMovementAsync`).

### ProductService
Se testea el comportamiento esperado para Products:
- **Paginado y filtros**: el service delega el paginado al repositorio.
- **GetById solo activos**: delega en `GetActiveByIdAsync`.
- **Create**: se aplica `Trim()` al nombre antes de persistir.
- **Update**:
  - si no existe → retorna `false` y no actualiza
  - si existe → actualiza campos y llama al repositorio
- **Delete**: usa soft delete (`SoftDeleteAsync`).

## Qué no se testea (y por qué)
- **Controllers**: se puede agregar con `WebApplicationFactory`/integration tests, pero no era prioritario para el challenge.
- **Repositories / EF**: se puede cubrir con InMemory o DB real (integration tests), pero el objetivo fue validar primero la lógica y reglas de negocio.
- **Autenticación JWT**: se validó funcionalmente con Swagger/Postman; tests automáticos serían un extra.

## Herramientas
- **xUnit**: framework de testing.
- **Moq**: mocks para repositorios (aislar la lógica del service).
- **Microsoft.NET.Test.Sdk + runner**: ejecución de tests en Visual Studio.

## Cómo ejecutar
Desde Visual Studio:
- `Prueba` → `Explorador de pruebas` → `Ejecutar todo`

o desde CLI:
```bash
dotnet test

