# Expense Approval API

Sistema de aprobación de gastos construido con **.NET 9**, Clean Architecture, CQRS con MediatR, y un API Gateway con Ocelot. 
Se usa esta arquitectura con el fin de mantener el tema de responsabilidad unica el patron CQRS ayuda a tener mas simplicidad en las clases, ideal para aplicacion que tienen reglas de negocio que deben validar estados(como es este proyecto), entre otras

---

## Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway                              │
│                   (Ocelot · port 5000)                          │
│         JWT validation · Route forwarding                       │
└───────────┬─────────────────────────────┬───────────────────────┘
            │                             │
            ▼                             ▼
┌───────────────────────┐   ┌───────────────────────────┐
│   Expense API (5001)  │   │   User API (5002)         │
│   8 endpoints         │   │   8 endpoints             │
│   · CRUD expenses     │   │   · Users CRUD            │
│   · Approve / Reject  │   │   · Roles & Claims        │
│   · Filter & Metrics  │   │   · Auth0 Login           │
└───────────┬───────────┘   └───────────┬───────────────┘
            │                           │
            └─────────┬─────────────────┘
                      ▼
        ┌───────────────────────┐
        │    Infrastructure     │
        │  · EF Core + SQL Srv  │
        │  · Repositories       │
        │  · Middlewares        │
        └───────────┬───────────┘
                    ▼
        ┌───────────────────────┐
        │     Application       │
        │  · CQRS (MediatR)     │
        │  · UseCases           │
        │  · DTOs & Validators  │
        └───────────┬───────────┘
                    ▼
        ┌───────────────────────┐
        │       Domain          │
        │  · Entities           │
        │  · Enums              │
        │  · Repository Intfs   │
        └───────────────────────┘
```

### Capas

| Capa | Proyecto | Responsabilidad |
|------|----------|-----------------|
| **Domain** | `ExpenseApproval.Domain` | Entidades (`AppUser`, `AppRole`, `AppRoleClaim`, `Category`, `ExpenseRequest`), Enums (`ExpenseStatus`), Interfaces de repositorios |
| **Application** | `ExpenseApproval.Application` | DTOs, CQRS Commands/Queries + Handlers (MediatR), UseCases, Validators (FluentValidation) |
| **Infrastructure** | `ExpenseApproval.Infrastructure` | EF Core DbContext, Repositorios, Middlewares (`PermissionMiddleware`, `ClaimRequirementHandler`, `JwtValidationDelegatingHandler`), Seed de datos |
| **Presentation** | `ExpenseApproval.Api` | 8 controllers de gastos, ExceptionHandlingMiddleware, Swagger |
| **Presentation** | `ExpenseApproval.User` | 3 controllers de usuarios, 4 de roles, 1 de login (Auth0), Swagger |
| **Presentation** | `ExpenseApproval.Gateway` | API Gateway con Ocelot, validación JWT centralizada |
| **Tests** | `ExpenseApproval.Tests` | Tests unitarios con xUnit, Moq, FluentAssertions |

### Estructura del proyecto

```
ExpenseApprovalApi/
├── src/
│   ├── ExpenseApproval.Domain/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Interfaces/
│   ├── ExpenseApproval.Application/
│   │   ├── DTOs/
│   │   ├── Features/
│   │   │   ├── Auth/Commands/
│   │   │   ├── Expenses/Commands/ & Queries/
│   │   │   ├── Roles/Commands/ & Queries/
│   │   │   └── Users/Commands/ & Queries/
│   │   ├── Interfaces/
│   │   ├── UseCases/
│   │   └── Validators/
│   ├── ExpenseApproval.Infrastructure/
│   │   ├── Data/
│   │   ├── Middlewares/
│   │   └── Repositories/
│   └── Presentation/
│       ├── ExpenseApproval.Api/
│       ├── ExpenseApproval.User/
│       └── ExpenseApproval.Gateway/
├── tests/
│   └── ExpenseApproval.Tests/
├── docker-compose.yml
├── .github/workflows/ci-cd.yml
├── ExpenseApproval.postman_collection.json
├── ExpenseApproval.local.postman_environment.json
└── ExpenseApproval.docker.postman_environment.json
```

---

## Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server) (local o vía Docker)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para ejecución con contenedores)
- Cuenta de [Auth0](https://auth0.com/) con:
  - Una aplicación configurada (Resource Owner Password Grant habilitado)
  - Un API registrado con las audiencias correspondientes

---

## Variables de entorno

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `CONNECTION_STRING` | Cadena de conexión a SQL Server | `Server=localhost,1433;Database=ExpenseApprovalDb;User Id=sa;Password=...;TrustServerCertificate=True;` |
| `AUTH0_DOMAIN` | Dominio de tu tenant Auth0 | `your-tenant.us.auth0.com` |
| `AUTH0_AUDIENCE` | Audience de tu API en Auth0 | `https://expense-approval-api` |
| `AUTH0_CLIENT_ID` | Client ID de tu aplicación Auth0 | (solo requerido por User API) |
| `AUTH0_CLIENT_SECRET` | Client Secret de tu aplicación Auth0 | (solo requerido por User API) |

---

## Ejecución local

### 1. SQL Server

Puedes usar una instancia local o levantar solo el contenedor de SQL Server:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password*2025*" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configurar variables de entorno

**PowerShell:**
```powershell
$env:CONNECTION_STRING = "Server=localhost,1433;Database=ExpenseApprovalDb;User Id=sa;Password=Password*2025*;TrustServerCertificate=True;"
$env:AUTH0_DOMAIN = "your-tenant.us.auth0.com"
$env:AUTH0_AUDIENCE = "https://expense-approval-api"
$env:AUTH0_CLIENT_ID = "TnJBONEDiVFjP2KZujtFpLFHXiYfosq3"
$env:AUTH0_CLIENT_SECRET = "3Jj4vvNe2GBQ052uXOBE37OmrktXLjvC2vGiL8odhUFx3AhLZyORH8ImT4QwytDv"
```

**Bash:**
```bash
export CONNECTION_STRING="Server=localhost,1433;Database=ExpenseApprovalDb;User Id=sa;Password=Password*2025*;TrustServerCertificate=True;"
export AUTH0_DOMAIN="your-tenant.us.auth0.com"
export AUTH0_AUDIENCE="https://expense-approval-api"
export AUTH0_CLIENT_ID="TnJBONEDiVFjP2KZujtFpLFHXiYfosq3"
export AUTH0_CLIENT_SECRET="3Jj4vvNe2GBQ052uXOBE37OmrktXLjvC2vGiL8odhUFx3AhLZyORH8ImT4QwytDv"
```

### 3. Restaurar dependencias y compilar

```bash
dotnet restore ExpenseApproval.sln
dotnet build ExpenseApproval.sln
```

### 4. Ejecutar los servicios

Abre 3 terminales y ejecuta cada servicio:

```bash
# Terminal 1 - Expense API (puerto 5001)
dotnet run --project src/Presentation/ExpenseApproval.Api

# Terminal 2 - User API (puerto 5002)
dotnet run --project src/Presentation/ExpenseApproval.User

# Terminal 3 - Gateway (puerto 5000)
dotnet run --project src/Presentation/ExpenseApproval.Gateway
```

### 5. Verificar

| Servicio | URL | Swagger |
|----------|-----|---------|
| Gateway | `http://localhost:5000` | N/A |
| Expense API | `https://localhost:7221` | `https://localhost:7221/swagger` |
| User API | `https://localhost:7198` | `https://localhost:7198/swagger` |

> La base de datos se crea y se alimenta con datos iniciales (seed) automáticamente al iniciar el servicio de Api.

---

## Ejecución con Docker

### 1. Crear archivo `.env`

Crea un archivo `.env` en la raíz del proyecto:

```env
AUTH0_DOMAIN=your-tenant.us.auth0.com
AUTH0_AUDIENCE=https://expense-approval-api
AUTH0_CLIENT_ID=your-client-id
AUTH0_CLIENT_SECRET=your-client-secret
```

### 2. Levantar todos los servicios

```bash
docker compose up --build
```

### 3. Verificar

| Servicio | URL |
|----------|-----|
| Gateway | `http://localhost:5000` |
| Expense API | `http://localhost:5001` |
| User API | `http://localhost:5002` |
| SQL Server | `localhost:1433` |

### 4. Detener los servicios

```bash
docker compose down
```

Para eliminar también los volúmenes de datos:

```bash
docker compose down -v
```

---

## Frontend Angular (ExpenseApprovalUI)

La aplicación frontend está construida con **Angular 19** y se conecta al backend a través del API Gateway.

### Ejecución local

#### 1. Instalar dependencias

```bash
cd ExpenseApprovalUI
npm install
```

#### 2. Iniciar el servidor de desarrollo

```bash
npm start
```

Angular se levanta en `http://localhost:4200` y apunta al Gateway local en `http://localhost:5000/api`.

> **Requisito:** Los 3 servicios del backend (Api, User, Gateway) deben estar corriendo localmente (ver sección [Ejecución local](#ejecución-local)).

#### 3. Abrir en el navegador

| URL | Descripción |
|-----|-------------|
| `http://localhost:4200` | Aplicación Angular |
| `http://localhost:4200/login` | Página de inicio de sesión |

### Ejecución con Docker

Cuando se levanta todo el stack con `docker compose up --build`, el servicio `expense-ui` se construye y sirve la aplicación Angular con Nginx.

- Nginx sirve los archivos estáticos del build de producción.
- Las peticiones a `/api/` se redirigen automáticamente al Gateway (`expense-gateway:8080`).

#### Abrir en el navegador

| URL | Descripción |
|-----|-------------|
| `http://localhost:4200` | Aplicación Angular (Nginx) |
| `http://localhost:4200/login` | Página de inicio de sesión |


> En Docker, la aplicación usa rutas relativas (`/api`) gracias a la configuración de proxy en Nginx, por lo que no es necesario configurar la URL del backend manualmente.

### Ejecutar tests del frontend

```bash
cd ExpenseApprovalUI
npx jest
```

Para ejecutar con cobertura:

```bash
npx jest --coverage
```

---

## Ejecutar tests del backend

```bash
dotnet test ExpenseApprovalApi/ExpenseApproval.sln --verbosity normal
```

Los tests unitarios cubren:
- **UseCases**: Expense requests, Roles, Users
- **Validators**: FluentValidation para DTOs de creación y actualización

---

## CI/CD

El pipeline de GitHub Actions (`.github/workflows/ci-cd.yml`) se ejecuta en cada push y PR a `main`:

| Job | Descripción |
|-----|-------------|
| **Build & Test** | Restaura, compila y ejecuta tests contra un servicio de SQL Server 2022 |
| **Docker Build** | Construye las 3 imágenes Docker en paralelo (Api, User, Gateway) |
| **Docker Compose Validation** | Valida la sintaxis del `docker-compose.yml` |

### Secretos de GitHub requeridos

- `SQL_SA_PASSWORD`
- `AUTH0_DOMAIN`
- `AUTH0_AUDIENCE`

---

## API Endpoints

Todas las peticiones pasan por el **Gateway** (`http://localhost:5000`).

### Auth

| Método | Ruta | Descripción | Auth |
|--------|------|-------------|------|
| POST | `/api/auth/login` | Autentica con email y password via Auth0 | No |


---

## Postman

Se incluyen archivos para importar en Postman:

| Archivo | Descripción |
|---------|-------------|
| `ExpenseApproval.postman_collection.json` | Colección con los 16 endpoints |
| `ExpenseApproval.local.postman_environment.json` | Variables de entorno para ejecución local |
| `ExpenseApproval.docker.postman_environment.json` | Variables de entorno para ejecución con Docker |

Para importar: **Postman → Import → seleccionar los 3 archivos**.

El endpoint de **Login** guarda automáticamente el `access_token` como variable de colección.

---

## Roles predefinidos (Seed)

| Rol | Permisos |
|-----|----------|
| **SuperAdmin** | Todos los 16 permisos |
| **Approver** | `expenses:list`, `expenses:read`, `expenses:approve`, `expenses:reject`, `expenses:filter`, `expenses:metrics` |
| **Requester** | `expenses:list`, `expenses:create`, `expenses:read`, `expenses:edit`, `expenses:filter` |

---

## Tecnologías

| Tecnología | Versión | Uso |
|------------|---------|-----|
| .NET | 9.0 | Framework principal |
| MediatR | 14.1.0 | CQRS (Commands/Queries) |
| EF Core | 9.x | ORM + SQL Server |
| Ocelot | 23.x | API Gateway |
| FluentValidation | 12.1.1 | Validación de DTOs |
| Auth0 + JWT Bearer | 9.x | Autenticación y autorización |
| Swashbuckle | 6.9.0 | Documentación Swagger/OpenAPI |
| xUnit | 2.9.2 | Framework de testing |
| Moq | 4.20.72 | Mocking |
| FluentAssertions | 8.9.0 | Assertions legibles |
| Docker | Multi-stage | Contenedorización |


### Observaciones

Para acceder al sistema por primera vez se adicionó en el usuario admin@expenseapproval.com
el password es Lv#ZRwM3fJKw5q2