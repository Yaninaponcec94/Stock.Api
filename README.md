📦 Stock API

API REST desarrollada en ASP.NET Core para la gestión de productos y stock, como parte de un challenge 
técnico con seguimiento.

El objetivo del proyecto es demostrar buenas prácticas de arquitectura, separación de responsabilidades y 
un flujo de desarrollo incremental y controlado.

🚀 Tecnologías utilizadas

ASP.NET Core
Entity Framework Core
SQL Server
Swagger / OpenAPI

🧱 Arquitectura

El proyecto está organizado siguiendo una arquitectura en capas, separando claramente responsabilidades:

Stock.Api
Stock.Application
Stock.Infrastructure

🔹 Stock.Api (Presentation)

Expone los endpoints HTTP
Maneja DTOs y validaciones básicas de entrada
Configura middlewares, Swagger y DI

🔹 Stock.Application (Business)

Contiene la lógica de negocio
Define contratos (interfaces) para servicios y repositorios
Modelos de dominio desacoplados de la persistencia

🔹 Stock.Infrastructure (Data)

Implementa la persistencia con EF Core
Define entidades de base de datos
Configuraciones de EF y migraciones

Esta separación evita acoplamientos innecesarios y facilita mantenimiento, testing y escalabilidad.

🧠 Decisiones de diseño relevantes
🔸 Separación de modelos y entidades

Existen dos clases Product:

- Application.Models.Product → modelo de negocio
- Infrastructure.Entities.Product → entidad de persistencia

Esto desacopla la lógica de negocio de EF Core y de la base de datos.

🔸 Repositorio + Service
El acceso a datos se realiza mediante repositorios, mientras que los servicios encapsulan reglas de negocio y flujos de trabajo.

🔸 EF Core con migraciones
El modelo se definió primero en código y luego se generó la base de datos mediante migraciones, evitando dependencias tempranas con la BD.

🔸 Desarrollo incremental
Se comenzó con InMemoryDatabase para validar la arquitectura y luego se migró a SQL Server sin modificar la lógica.

📚 Funcionalidad implementada
✔ CRUD de Products

Endpoints disponibles:

GET /api/products
GET /api/products/{id}
POST /api/products
PUT /api/products/{id}
DELETE /api/products/{id}

El CRUD fue probado completamente desde Swagger, validando:

Inyección de dependencias
Persistencia en SQL Server
Correcto flujo DTO → Servicio → Repositorio → Base de datos

🛠 Base de datos

SQL Server
Migraciones con Entity Framework Core
Tablas generadas automáticamente a partir del modelo

📌 Estado actual

✔ Arquitectura base completa
✔ CRUD funcional y probado
✔ Proyecto listo para extender con:

validaciones
soft delete
paginado y filtros
nuevas entidades relacionadas a stock

📝 Notas

Este repositorio refleja el progreso real del challenge, priorizando claridad, buenas prácticas y 
decisiones justificadas por sobre soluciones rápidas o acopladas.