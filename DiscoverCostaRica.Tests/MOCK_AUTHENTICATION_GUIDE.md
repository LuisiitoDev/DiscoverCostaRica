# Mock Authentication Implementation Guide

Este guía explica cómo implementar autenticación mock apropiada (Opción 1) para los tests de integración.

## Problema Actual

Actualmente, los tests desactivan completamente la autenticación cuando no hay `TenantId` configurado. Esto no es una buena práctica porque:

❌ No valida que el pipeline de autenticación funcione correctamente  
❌ Los endpoints no requieren autenticación en tests  
❌ No se prueban los roles y claims  

## Solución Recomendada: TestAuthenticationHandler

La infraestructura para autenticación mock ya está creada:
- `TestAuthenticationHandler.cs`: Handler que provee claims de prueba
- `TestAuthenticationExtensions.cs`: Extensiones para configurar en tests

### Implementación Paso a Paso

#### 1. Modificar Program.cs de cada API

Actualiza cada archivo `Program.cs` de tus APIs para usar autenticación de prueba en ambiente Testing:

**Antes:**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddEntraIdAuthentication();  // Siempre usa Entra ID
```

**Después:**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Usar autenticación de prueba en Testing, Entra ID en otros ambientes
if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddAuthentication(
        DiscoverCostaRica.Tests.Infraestructure.TestAuthenticationHandler.AuthenticationScheme)
        .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions,
                   DiscoverCostaRica.Tests.Infraestructure.TestAuthenticationHandler>(
            DiscoverCostaRica.Tests.Infraestructure.TestAuthenticationHandler.AuthenticationScheme,
            options => { });
}
else
{
    builder.AddEntraIdAuthentication();
}
```

#### 2. Agregar referencia al proyecto de tests (solo para Testing)

En cada archivo `.csproj` de tus APIs, agrega:

```xml
<ItemGroup Condition="'$(Configuration)' == 'Debug'">
  <ProjectReference Include="..\..\DiscoverCostaRica.Tests\DiscoverCostaRica.Tests.csproj" />
</ItemGroup>
```

**Alternativa mejor**: Mover `TestAuthenticationHandler.cs` a un proyecto compartido (ej: `DiscoverCostaRica.Shared`) para evitar referencias circulares.

#### 3. Actualizar appsettings.Testing.json

El archivo ya está configurado, pero ahora los valores pueden ser más explícitos:

```json
{
  "Testing": {
    "UseTestAuthentication": true
  },
  "EntraId": {
    "TenantId": "test-tenant",
    "ClientId": "test-client",
    "Instance": "https://test.local/",
    "Audience": "api://test",
    "Scopes": "test.read test.write"
  }
}
```

### Beneficios de esta Implementación

✅ **Autenticación activa**: El pipeline de autenticación funciona normalmente  
✅ **Claims válidos**: Los tests tienen roles y claims apropiados  
✅ **Mejor cobertura**: Valida que la autorización funciona correctamente  
✅ **Separación clara**: Testing usa mock, producción usa Entra ID real  
✅ **Sin cambios en lógica de negocio**: Solo cambios en configuración  

## Claims Proporcionados por TestAuthenticationHandler

El handler de prueba proporciona los siguientes claims:

```csharp
- ClaimTypes.Name: "TestUser"
- ClaimTypes.NameIdentifier: "test-user-id"
- "roles": "Beaches.Read"
- "roles": "Volcano.Read"
- "roles": "Culture.Read"
- "roles": "Geo.Read"
- ClaimTypes.Email: "testuser@discovercostarica.test"
```

Puedes personalizar estos claims editando `TestAuthenticationHandler.cs`.

## Alternativa: Mover TestAuthenticationHandler a Shared

Para evitar referencias al proyecto de tests, puedes:

### 1. Crear en DiscoverCostaRica.Shared/Testing/

```
DiscoverCostaRica.Shared/
  Testing/
    TestAuthenticationHandler.cs
    TestAuthenticationExtensions.cs
```

### 2. Actualizar namespaces

```csharp
namespace DiscoverCostaRica.Shared.Testing;
```

### 3. Condicional en Shared.csproj

```xml
<ItemGroup Condition="'$(Configuration)' == 'Debug'">
  <!-- Solo incluir archivos de testing en Debug -->
  <Compile Include="Testing\**\*.cs" />
</ItemGroup>
```

## Validación de los Tests

Con TestAuthenticationHandler activo, tus tests pueden validar:

### 1. Autenticación funciona
```csharp
[Fact]
public async Task Endpoint_RequiresAuthentication()
{
    // El endpoint debe responder 200 con el TestAuthenticationHandler
    // Sin autenticación, respondería 401
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### 2. Autorización por roles funciona
```csharp
[Fact]
public async Task Endpoint_RequiresCorrectRole()
{
    // El usuario de prueba tiene el rol "Beaches.Read"
    // Por lo tanto puede acceder a endpoints con [Authorize(Roles = "Beaches.Read")]
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### 3. Claims están disponibles
```csharp
[Fact]
public async Task Service_CanAccessUserClaims()
{
    // Los servicios pueden acceder a HttpContext.User.Claims
    // que contienen los claims del TestAuthenticationHandler
}
```

## Comparación: Implementación Actual vs Recomendada

| Aspecto | Actual (Sin Auth) | Recomendada (TestAuthenticationHandler) |
|---------|-------------------|------------------------------------------|
| Seguridad en tests | ❌ Desactivada | ✅ Activa con mock |
| Pipeline de auth | ❌ Omitido | ✅ Ejecutado |
| Claims disponibles | ❌ No | ✅ Sí |
| Validación de roles | ❌ No | ✅ Sí |
| Cambios en API | ✅ Ninguno | ⚠️ Program.cs |
| Mejores prácticas | ❌ No | ✅ Sí |

## Siguientes Pasos

1. **Decide dónde colocar el código**:
   - Opción A: Dejar en proyecto Tests (requiere referencia)
   - Opción B: Mover a DiscoverCostaRica.Shared (más limpio)

2. **Actualiza los Program.cs** de cada API

3. **Ejecuta los tests** para verificar que funcionan

4. **Opcionalmente, agrega tests específicos** para validar autenticación y autorización

## Soporte

Si necesitas ayuda implementando esto:
1. Decide qué opción prefieres (A o B arriba)
2. Puedo generar los cambios específicos para tu código
3. Puedo crear un ejemplo completo para una API como referencia
