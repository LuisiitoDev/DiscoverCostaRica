# DiscoverCostaRica

## src

### DiscoverCostaRica.Api

- **Proyecto principal de la API**
    - **Controllers**: Controladores de la API
    - **Models**: Modelos y DTOs (Data Transfer Objects)
    - **Services**: Servicios para lógica de negocio
    - **Repositories**: Repositorios para acceso a datos
    - **Configurations**: Configuraciones de la API
    - **Mappings**: Configuraciones de AutoMapper
    - **Middleware**: Middlewares personalizados
    - **Startup.cs**: Configuración de inicio
    - **Program.cs**: Punto de entrada de la aplicación

### DiscoverCostaRica.Domain

- **Proyecto para la capa de dominio**
    - **Entities**: Entidades del dominio (Modelos principales)
    - **ValueObjects**: Objetos de valor del dominio
    - **Interfaces**: Interfaces para repositorios y servicios
    - **Services**: Servicios de dominio
    - **Enums**: Enums del dominio

### DiscoverCostaRica.Infrastructure

- **Proyecto para la infraestructura**
    - **Data**: Contexto de datos y migraciones
    - **Repositories**: Implementaciones de repositorios
    - **Services**: Servicios de infraestructura (e.g., Blob Storage)
    - **Configurations**: Configuraciones de infraestructura (e.g., conexiones a bases de datos)
    - **Extensions**: Extensiones para la infraestructura

### DiscoverCostaRica.Tests

- **Proyecto para pruebas**
    - **UnitTests**: Pruebas unitarias
    - **IntegrationTests**: Pruebas de integración
