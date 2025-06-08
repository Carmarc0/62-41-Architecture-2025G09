# HES-SO Print Payment System

**Architecture Project 2025 - Group 2025G09**  
*Filipovic Marko, Marques Conde Carolina*

## Description

Integrated payment system for university printers, enabling print quota management through distinct web interfaces for students and administration.

## Architecture

```
MVC Web App (Interface) → WebAPI (Business Logic) → Azure SQL Database
```

### Projects
- **DAL**: Data Access Layer with Entity Framework
- **WebAPI**: REST API for business logic
- **MVC**: Web interface (Faculties + Point of Sale)
- **TestApp**: Testing application

## Azure Deployment

### Deployed Services
- **WebAPI**: `https://printsystem-webapi-2025g9-xxx.azurewebsites.net`
- **MVC**: `https://printsystem-mvc-2025g9-xxx.azurewebsites.net`
- **Database**: Azure SQL Database

## Test Accounts

### Students
- **SCoppey** / welcome
- **JDupont** / password123
- **AMartin** / mdp123

### Administrators
- **admin** / admin123
- **prof** / profpass

## Currency Conversion

- **1 A4 page = 0.08 CHF**
- **1 CHF = 12.5 pages**

## Local Installation

1. **Prerequisites**: .NET 8, SQL Server LocalDB
2. **Clone** the repository
3. **Restore** NuGet packages
4. **Configure** connection strings in appsettings.Development.json
5. **Run** WebAPI and MVC simultaneously

## Features

### Faculties Interface (Admin)
- Secure authentication
- Semester quota allocation
- Student balance consultation

### Point of Sale Interface (Students)
- University authentication
- Balance inquiry
- Credit addition

## Technologies

- **Backend**: ASP.NET Core 8, Entity Framework Core
- **Frontend**: ASP.NET MVC, Bootstrap, JavaScript
- **Database**: SQL Server / Azure SQL
- **Cloud**: Microsoft Azure (App Services)
- **Architecture**: Clean Architecture, Dependency Injection

## Testing

Use **TestApp** to test APIs:
- Authentication
- Quota management
- Balance consultation

## Configuration

### Development
Configure `appsettings.Development.json` with local connection strings.

### Production
Update `appsettings.Production.json` with your Azure resources:
- Database connection string
- WebAPI base URL

## API Endpoints

### Authentication
- `POST /api/Authentication/login` - Generic user authentication
- `POST /api/Authentication/login-admin` - Admin-specific authentication with role validation
- `POST /api/Authentication/login-student` - Student-specific authentication with role validation
- `GET /api/Authentication/user-exists/{username}` - Check if user exists in Active Directory

### Quota Management
- `POST /api/Quota/add-amount` - Add CHF amount to user's print quota (converts to pages)
- `GET /api/Quota/balance/{username}` - Get user's current balance in CHF
- `GET /api/Quota/all-balances` - Get all user balances for admin interface
- `POST /api/Quota/semester-quota` - Add semester quota (Faculty interface only)
- `GET /api/Quota/username/{uid}` - Convert UID to username (for card scanner integration)

### User Management
- `GET /api/Users/all` - Get all users with their quota information and roles

---
*Project developed for the 62-41 Architecture course*