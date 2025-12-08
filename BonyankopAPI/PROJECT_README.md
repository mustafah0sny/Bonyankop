# 🚀 Bonyankop API - Complete Setup

A complete .NET Web API with JWT authentication, SQL Server in Docker, and mobile app integration.

## 📋 What's Included

✅ .NET 10.0 Web API with JWT Authentication
✅ SQL Server running in Docker
✅ User Registration & Login endpoints
✅ Password hashing with BCrypt
✅ CORS enabled for mobile apps
✅ Health check endpoints
✅ Complete mobile integration examples (Flutter & React Native)
✅ Docker Compose setup
✅ Automated startup scripts

## 🎯 Quick Start

### 1. Start Everything with One Command

```powershell
cd BonyankopAPI
.\start.ps1
```

This will:
- ✅ Check Docker is running
- ✅ Start SQL Server container
- ✅ Create database
- ✅ Apply migrations
- ✅ Start the API

### 2. Test the API

Open browser: http://localhost:5000/api/Health

Or use the included test file: `api-tests.http`

### 3. Connect Your Mobile App

- **Android Emulator**: `http://10.0.2.2:5000/api`
- **iOS Simulator**: `http://localhost:5000/api`
- **Physical Device**: `http://YOUR_IP:5000/api`

## 📱 Mobile Integration

### Flutter Example
```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class ApiService {
  static const String baseUrl = 'http://10.0.2.2:5000/api';
  
  Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/User/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'email': email, 'password': password}),
    );
    return jsonDecode(response.body);
  }
}
```

See `MOBILE_INTEGRATION.md` for complete examples.

## 🐳 Docker Commands

```bash
# Start SQL Server only
docker-compose up -d sqlserver

# Start all services (API + SQL Server)
docker-compose up -d

# Stop all services
docker-compose down

# View logs
docker-compose logs -f

# Remove everything (including data)
docker-compose down -v
```

## 📚 Documentation

| File | Description |
|------|-------------|
| `QUICK_START.md` | Fast setup guide |
| `DOCKER_GUIDE.md` | Complete Docker documentation |
| `MOBILE_INTEGRATION.md` | Mobile app integration with code examples |
| `README.md` | API documentation |
| `api-tests.http` | API endpoint tests |

## 🔌 API Endpoints

### Health Check
```
GET /api/Health
GET /api/Health/db
```

### Authentication
```
POST /api/User/signup
POST /api/User/login
```

**Sign Up Request:**
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

## 🔧 Configuration

### SQL Server Connection
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BonyankopDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### JWT Settings
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJwtTokenGeneration123456789",
    "Issuer": "BonyankopAPI",
    "Audience": "BonyankopAPIUsers",
    "ExpiryMinutes": 60
  }
}
```

## 🛠️ Development

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 📦 Project Structure

```
BonyankopAPI/
├── Controllers/
│   ├── UserController.cs      # Auth endpoints
│   └── HealthController.cs    # Health checks
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
│   ├── LoginDto.cs
│   ├── SignUpDto.cs
│   └── AuthResponseDto.cs
├── Models/
│   └── User.cs
├── Migrations/
├── docker-compose.yml
├── Dockerfile
├── start.ps1                   # Windows startup script
├── start.sh                    # Linux/Mac startup script
└── appsettings.json
```

## 🐛 Troubleshooting

### SQL Server won't start
```bash
docker-compose logs sqlserver
docker-compose restart sqlserver
```

### Port 1433 already in use
Change port in `docker-compose.yml`:
```yaml
ports:
  - "1434:1433"
```

### Mobile can't connect
1. Check API is running: http://localhost:5000/api/Health
2. Use correct URL:
   - Android Emulator: `http://10.0.2.2:5000/api`
   - Physical Device: `http://YOUR_IP:5000/api`
3. Allow firewall (Run as Admin):
   ```powershell
   New-NetFirewallRule -DisplayName "Bonyankop API" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
   ```

## 🔒 Security Notes

⚠️ **Before Production:**
1. Change SQL Server SA password
2. Change JWT SecretKey
3. Use environment variables for secrets
4. Enable HTTPS only
5. Implement rate limiting
6. Add input validation
7. Set up proper logging

## 🚀 Deployment

### Using Docker
```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Manual Deployment
1. Publish the app:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
2. Deploy to your server
3. Configure connection strings
4. Set up reverse proxy (nginx/IIS)

## 📞 Support

- 📖 Full API Docs: `README.md`
- 🐳 Docker Guide: `DOCKER_GUIDE.md`
- 📱 Mobile Integration: `MOBILE_INTEGRATION.md`
- 🚀 Quick Start: `QUICK_START.md`

## 📝 License

MIT License

---

**Built with ❤️ using .NET 10.0, SQL Server, and Docker**
