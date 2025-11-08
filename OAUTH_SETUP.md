# OAuth2 (Microsoft Entra ID) Setup and Testing Guide

This guide provides step-by-step instructions for configuring and testing OAuth2 authentication with Microsoft Entra ID (formerly Azure AD) in the DiscoverCostaRica microservices solution.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Azure AD App Registration](#azure-ad-app-registration)
3. [Local Development Configuration](#local-development-configuration)
4. [Testing Authentication](#testing-authentication)
5. [Troubleshooting](#troubleshooting)

## Prerequisites

- Azure subscription with permissions to create App Registrations
- .NET 9.0 SDK installed
- Azure CLI (optional, for automation)
- A REST client (curl, Postman, or similar)

## Azure AD App Registration

### Step 1: Create App Registration

1. Navigate to [Azure Portal](https://portal.azure.com)
2. Go to **Azure Active Directory** → **App registrations** → **New registration**
3. Configure the registration:
   - **Name**: `DiscoverCostaRica API`
   - **Supported account types**: Choose based on your requirements
     - Single tenant (recommended for internal apps)
     - Multi-tenant (for public APIs)
   - **Redirect URI**: Leave blank for API-only authentication
4. Click **Register**

### Step 2: Configure API Permissions and Scopes

1. In your App Registration, go to **Expose an API**
2. Click **Set** next to Application ID URI (default: `api://<CLIENT_ID>`)
3. Click **Add a scope** and create the following scopes:

   | Scope Name | Display Name | Description | Admin Consent |
   |------------|--------------|-------------|---------------|
   | Beaches.Read | Read beach data | Allows reading beach information | No |
   | Beaches.Write | Write beach data | Allows creating/updating beaches | Yes |
   | Volcano.Read | Read volcano data | Allows reading volcano information | No |
   | Volcano.Write | Write volcano data | Allows creating/updating volcanoes | Yes |
   | Culture.Read | Read culture data | Allows reading cultural information | No |
   | Culture.Write | Write culture data | Allows creating/updating culture sites | Yes |
   | Geo.Read | Read geo data | Allows reading geographical information | No |
   | Geo.Write | Write geo data | Allows creating/updating geo locations | Yes |

4. Note your **Application (client) ID** and **Directory (tenant) ID** from the Overview page

### Step 3: Create Client Secret (for testing)

1. Go to **Certificates & secrets** → **Client secrets**
2. Click **New client secret**
3. Add description: `Development Testing`
4. Choose expiration: 90 days (for testing) or appropriate duration
5. Click **Add**
6. **IMPORTANT**: Copy the secret value immediately (it won't be shown again)

## Local Development Configuration

### Option 1: Using User Secrets (Recommended)

User secrets keep sensitive data out of source control and are ideal for local development.

#### For AppHost:
```bash
cd DiscoverCostaRica.AppHost

dotnet user-secrets set "EntraId:TenantId" "YOUR_TENANT_ID"
dotnet user-secrets set "EntraId:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "EntraId:Audience" "api://YOUR_CLIENT_ID"
```

#### For Each Microservice (example for Beaches API):
```bash
cd src/DiscoverCostaRica.Beaches/DiscoverCostaRica.Beaches.Api

dotnet user-secrets set "EntraId:TenantId" "YOUR_TENANT_ID"
dotnet user-secrets set "EntraId:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "EntraId:Audience" "api://YOUR_CLIENT_ID"
```

Repeat for other services (Volcano, Culture, Geo) with their respective scopes.

### Option 2: Using Environment Variables

```bash
export EntraId__TenantId="YOUR_TENANT_ID"
export EntraId__ClientId="YOUR_CLIENT_ID"
export EntraId__Audience="api://YOUR_CLIENT_ID"
```

### Option 3: Development appsettings (Not Recommended - for quick testing only)

Create `appsettings.Development.json` in each project:

```json
{
  "EntraId": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "Audience": "api://YOUR_CLIENT_ID",
    "Scopes": "Beaches.Read,Beaches.Write"
  }
}
```

**⚠️ WARNING**: Never commit real credentials to source control!

## Testing Authentication

### Step 1: Start the Application

```bash
# From the solution root
cd DiscoverCostaRica.AppHost
dotnet run
```

The Aspire dashboard will show all services and the YARP gateway endpoint.

### Step 2: Obtain an Access Token

#### Using Client Credentials Flow (Service-to-Service)

```bash
# Get access token using client credentials
curl -X POST "https://login.microsoftonline.com/{TENANT_ID}/oauth2/v2.0/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id={CLIENT_ID}" \
  -d "client_secret={CLIENT_SECRET}" \
  -d "scope=api://{CLIENT_ID}/.default" \
  -d "grant_type=client_credentials"
```

Response will include an `access_token` field. Copy this token.

#### Using Azure CLI (Alternative)

```bash
az login
az account get-access-token --resource api://{CLIENT_ID}
```

### Step 3: Test API Endpoints

Replace `{GATEWAY_URL}` with the YARP gateway endpoint from Aspire dashboard (typically `https://localhost:7xxx`).

#### Test Beaches Service

```bash
# Without authentication (should return 401 Unauthorized)
curl -i https://localhost:7xxx/api/beaches/v1/beaches

# With authentication (replace {TOKEN} with your access token)
curl -i https://localhost:7xxx/api/beaches/v1/beaches \
  -H "Authorization: Bearer {TOKEN}"
```

#### Test Volcano Service

```bash
curl -i https://localhost:7xxx/api/volcano/v1/volcanoes \
  -H "Authorization: Bearer {TOKEN}"
```

#### Test Culture Service

```bash
curl -i https://localhost:7xxx/api/culture/v1/sites \
  -H "Authorization: Bearer {TOKEN}"
```

#### Test Geo Service

```bash
curl -i https://localhost:7xxx/api/geo/v1/locations \
  -H "Authorization: Bearer {TOKEN}"
```

### Step 4: Verify Token Claims

You can decode your JWT token at [jwt.ms](https://jwt.ms) to verify:
- `aud` (audience) matches your ClientId
- `scp` (scopes) contains the required scopes
- `tid` (tenant ID) matches your Azure AD tenant
- `iss` (issuer) is `https://login.microsoftonline.com/{TENANT_ID}/v2.0`

## Route Mapping

The YARP gateway maps external routes to internal service routes:

| External Route | Internal Service | Internal Route |
|----------------|------------------|----------------|
| `/api/beaches/*` | Beaches API | `/v1/*` |
| `/api/volcano/*` | Volcano API | `/v1/*` |
| `/api/culture/*` | Culture API | `/v1/*` |
| `/api/geo/*` | Geo API | `/v1/*` |

All requests automatically forward the `Authorization` header to backend services.

## Testing Scope-Based Authorization

If you've implemented endpoint-level authorization with policies, test with different scopes:

### Token with Read-Only Scope
```bash
# Token should have only *.Read scopes
# GET requests should succeed
curl -i https://localhost:7xxx/api/beaches/v1/beaches \
  -H "Authorization: Bearer {READ_ONLY_TOKEN}"

# POST/PUT/DELETE should return 403 Forbidden
curl -i -X POST https://localhost:7xxx/api/beaches/v1/beaches \
  -H "Authorization: Bearer {READ_ONLY_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Beach"}'
```

### Token with Write Scope
```bash
# Token should have *.Write scopes
# All operations should succeed
curl -i -X POST https://localhost:7xxx/api/beaches/v1/beaches \
  -H "Authorization: Bearer {WRITE_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Beach"}'
```

## Troubleshooting

### Issue: 401 Unauthorized

**Possible Causes:**
1. Token not included in request
2. Token expired
3. Invalid token signature

**Solutions:**
- Verify Authorization header format: `Bearer {token}`
- Check token expiration at jwt.ms
- Ensure TenantId and ClientId match your app registration
- Verify token was obtained for the correct audience (`api://{CLIENT_ID}`)

### Issue: 403 Forbidden

**Possible Causes:**
1. Token doesn't have required scopes
2. Policy requirements not met

**Solutions:**
- Decode token and verify `scp` claim contains required scope
- Ensure app registration has exposed the API scopes
- Grant admin consent for scopes requiring it

### Issue: 500 Internal Server Error

**Possible Causes:**
1. EntraId configuration missing or incorrect
2. Microsoft.Identity.Web package issues

**Solutions:**
- Check service logs in Aspire dashboard
- Verify EntraId configuration exists in appsettings or user-secrets
- Ensure all configuration values are set (TenantId, ClientId, Audience)

### Issue: Token validation fails

**Check these configuration values:**
```bash
# Verify Instance + TenantId forms correct authority
Authority: https://login.microsoftonline.com/{TENANT_ID}

# Verify Audience matches token's aud claim
Audience: api://{CLIENT_ID}

# Check that token issuer matches expected format
Expected Issuer: https://login.microsoftonline.com/{TENANT_ID}/v2.0
```

## Advanced Configuration

### Disable Authentication for Development

To temporarily disable authentication (not recommended for production):

Comment out or remove the EntraId section from configuration. The `AddEntraIdAuthentication` method will log a warning and skip authentication setup.

### Custom Claims and Policies

To add custom authorization policies, modify `ServiceDefaults/Extensions.cs`:

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CustomPolicy", policy =>
        policy.RequireClaim("custom_claim", "expected_value"));
```

### Logging

Authentication events are logged at Information level. To see detailed logs:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.Identity.Web": "Debug"
    }
  }
}
```

## Security Best Practices

1. **Never commit secrets** to source control
2. **Use managed identities** in Azure for production
3. **Rotate secrets regularly** (90-180 days)
4. **Use short token lifetimes** for sensitive operations
5. **Implement proper scope granularity** (separate read/write)
6. **Log authentication failures** for security monitoring
7. **Use HTTPS** in all environments
8. **Validate token issuer and audience** on every request

## Production Deployment

For production deployments:

1. Use Azure Key Vault for secrets
2. Configure managed identity for services
3. Set up proper monitoring and alerting
4. Implement rate limiting on authentication endpoints
5. Use appropriate token cache strategies
6. Configure proper CORS policies
7. Set up Azure Front Door or API Management for additional security layers

## References

- [Microsoft Identity Platform Documentation](https://docs.microsoft.com/azure/active-directory/develop/)
- [Microsoft.Identity.Web Documentation](https://docs.microsoft.com/azure/active-directory/develop/microsoft-identity-web)
- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
