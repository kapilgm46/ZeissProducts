# ZeissProducts
# Product Management API (.NET Core 8)

A robust and scalable RESTful API built with ASP.NET Core 8 for managing products. It uses Entity Framework Core (code-first) for database operations and supports full CRUD functionality.

## Product Model

Each product includes the following properties:

- `ProjectId` (int): Unique identifier
- `Name` (string): Name of the product
- `Quantity` (int): Available stock
- `Price` (decimal): Unit price
- `Description` (string): Product details

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- SQL Server (default, configurable)
- AutoMapper (optional)
- NUnit + Moq (for unit testing)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/kapilgm46/ZeissProducts.git
cd ProductApi
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Create the Database Migration

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the API

```bash
dotnet run
```



