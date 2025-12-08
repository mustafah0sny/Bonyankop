# Bonyankop API

A .NET Web API project with user authentication using JWT tokens and SQL Server.

## Features

- User Registration (Sign Up)
- User Login
- JWT Token Authentication
- SQL Server Database
- Password Hashing with BCrypt

## Prerequisites

- .NET 10.0 SDK
- SQL Server (LocalDB or SQL Server Express)
- Visual Studio Code or Visual Studio

## Setup Instructions

### 1. Update Database Connection String

Edit `appsettings.json` and update the connection string if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=BonyankopDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

For SQL Server Authentication, use:
```json
"DefaultConnection": "Server=localhost;Database=BonyankopDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
```

### 2. Update Database

Run the following command to create the database:

```bash
dotnet ef database update
```

### 3. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## API Endpoints

### 1. Sign Up
**POST** `/api/User/signup`

**Request Body:**
```json
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john@example.com",
  "username": "johndoe"
}
```

### 2. Login
**POST** `/api/User/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john@example.com",
  "username": "johndoe"
}
```

## Using the JWT Token

After login or signup, you'll receive a JWT token. Use this token in the Authorization header for protected endpoints:

```
Authorization: Bearer <your_token_here>
```

## Project Structure

```
BonyankopAPI/
├── Controllers/
│   └── UserController.cs       # User authentication endpoints
├── Data/
│   └── ApplicationDbContext.cs # Database context
├── DTOs/
│   ├── LoginDto.cs             # Login request model
│   ├── SignUpDto.cs            # Sign up request model
│   └── AuthResponseDto.cs      # Authentication response model
├── Models/
│   └── User.cs                 # User entity model
├── Migrations/                 # EF Core migrations
├── Program.cs                  # Application entry point
└── appsettings.json           # Configuration settings
```

## Security Notes

⚠️ **IMPORTANT**: Before deploying to production:

1. Change the `JwtSettings:SecretKey` in `appsettings.json` to a strong, randomly generated key
2. Store sensitive configuration in environment variables or Azure Key Vault
3. Enable HTTPS only in production
4. Implement rate limiting
5. Add input validation and sanitization
6. Implement proper error logging

## JWT Settings

The JWT token configuration can be modified in `appsettings.json`:

```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyForJwtTokenGeneration123456789",
  "Issuer": "BonyankopAPI",
  "Audience": "BonyankopAPIUsers",
  "ExpiryMinutes": 60
}
```

## Testing with cURL

### Sign Up:
```bash
curl -X POST https://localhost:5001/api/User/signup \
  -H "Content-Type: application/json" \
  -d '{"username":"johndoe","email":"john@example.com","password":"password123"}'
```

### Login:
```bash
curl -X POST https://localhost:5001/api/User/login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@example.com","password":"password123"}'
```

## License

MIT License
