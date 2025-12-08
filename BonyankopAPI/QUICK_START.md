# 🚀 Quick Start Guide

Get your API and mobile app running in minutes!

## Prerequisites

✅ .NET 10.0 SDK installed
✅ Docker Desktop installed and running
✅ SQL Server Management Studio or Azure Data Studio (optional)

## Step 1: Start SQL Server with Docker

Open PowerShell in the project directory and run:

```powershell
.\start.ps1
```

This script will:
1. ✅ Check if Docker is running
2. ✅ Start SQL Server in a Docker container
3. ✅ Wait for SQL Server to be ready
4. ✅ Apply database migrations
5. ✅ Start the API

**Or manually:**

```bash
# Start SQL Server
docker-compose up -d sqlserver

# Wait 30 seconds, then apply migrations
dotnet ef database update

# Run the API
dotnet run
```

## Step 2: Verify API is Running

Open your browser and navigate to:
- http://localhost:5000/api/Health

You should see:
```json
{
  "status": "healthy",
  "timestamp": "2025-12-07T10:30:00Z",
  "version": "1.0.0",
  "message": "Bonyankop API is running"
}
```

## Step 3: Test the API

### Option A: Using VS Code REST Client

Open `api-tests.http` and click "Send Request" above any endpoint.

### Option B: Using PowerShell

**Sign Up:**
```powershell
$body = @{
    username = "testuser"
    email = "test@example.com"
    password = "test123456"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/User/signup" -Method POST -Body $body -ContentType "application/json"
```

**Login:**
```powershell
$body = @{
    email = "test@example.com"
    password = "test123456"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/User/login" -Method POST -Body $body -ContentType "application/json"
```

## Step 4: Connect Mobile App

### For Android Emulator:
Use base URL: `http://10.0.2.2:5000/api`

### For iOS Simulator:
Use base URL: `http://localhost:5000/api`

### For Physical Device:

1. Find your computer's IP address:
   ```powershell
   ipconfig
   ```
   Look for "IPv4 Address" (e.g., 192.168.1.100)

2. Use base URL: `http://YOUR_IP:5000/api`
   Example: `http://192.168.1.100:5000/api`

3. Allow firewall access (Run as Administrator):
   ```powershell
   New-NetFirewallRule -DisplayName "Bonyankop API" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
   ```

## Step 5: Implement in Your Mobile App

See detailed implementation guides:
- 📱 **Flutter**: Check `MOBILE_INTEGRATION.md`
- 🔧 **Docker**: Check `DOCKER_GUIDE.md`
- 📚 **API Docs**: Check `README.md`

## Quick Reference

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Health` | Check API health |
| GET | `/api/Health/db` | Check database connection |
| POST | `/api/User/signup` | Register new user |
| POST | `/api/User/login` | Login user |

### Docker Commands

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# View logs
docker-compose logs -f

# View SQL Server logs
docker-compose logs -f sqlserver

# Restart services
docker-compose restart

# Remove everything (including data)
docker-compose down -v
```

### Connection Strings

**Local Development:**
```
Server=localhost,1433;Database=BonyankopDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

**Connect with SSMS:**
- Server: `localhost,1433`
- Authentication: SQL Server Authentication
- Login: `sa`
- Password: `YourStrong@Passw0rd`

## Common Issues

### ❌ "Cannot connect to SQL Server"

**Solution:**
```bash
# Check if SQL Server is running
docker-compose ps

# View SQL Server logs
docker-compose logs sqlserver

# Restart SQL Server
docker-compose restart sqlserver
```

### ❌ "Port 1433 is already in use"

**Solution:**
Stop any existing SQL Server instance or change the port in `docker-compose.yml`:
```yaml
ports:
  - "1434:1433"  # Use port 1434 instead
```

Then update connection string:
```
Server=localhost,1434;...
```

### ❌ "Migration failed"

**Solution:**
```bash
# Remove existing migrations
rm -r Migrations/

# Create new migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

### ❌ "Mobile app can't connect"

**Checklist:**
1. ✅ Is the API running? Check http://localhost:5000/api/Health
2. ✅ Are you using the correct base URL?
   - Android Emulator: `http://10.0.2.2:5000/api`
   - Physical Device: `http://YOUR_IP:5000/api`
3. ✅ Is firewall blocking the connection?
4. ✅ Are both devices on the same Wi-Fi network?

## Next Steps

1. ✅ Customize the User model for your needs
2. ✅ Add more endpoints (profile, password reset, etc.)
3. ✅ Implement refresh tokens
4. ✅ Add role-based authorization
5. ✅ Set up production environment
6. ✅ Deploy to Azure/AWS

## Support

For detailed documentation:
- 📱 Mobile Integration: `MOBILE_INTEGRATION.md`
- 🐳 Docker Setup: `DOCKER_GUIDE.md`
- 📖 API Documentation: `README.md`

---

**Happy Coding! 🎉**
