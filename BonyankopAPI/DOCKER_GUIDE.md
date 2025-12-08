# Bonyankop API with Docker

## Quick Start with Docker

### 1. Start SQL Server Container

```bash
docker-compose up -d sqlserver
```

Wait for 30 seconds for SQL Server to fully start, then:

### 2. Apply Database Migrations

```bash
dotnet ef database update
```

### 3. Run the API (Option A: Locally)

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### 3. Run the API (Option B: In Docker)

```bash
docker-compose up -d
```

The API will be available at:
- HTTP: `http://localhost:5000`

## Docker Commands

### Start all services:
```bash
docker-compose up -d
```

### Stop all services:
```bash
docker-compose down
```

### View logs:
```bash
docker-compose logs -f
```

### View SQL Server logs:
```bash
docker-compose logs -f sqlserver
```

### View API logs:
```bash
docker-compose logs -f api
```

### Restart services:
```bash
docker-compose restart
```

## SQL Server Connection Details

When running in Docker:
- **Server**: `localhost,1433`
- **Username**: `sa`
- **Password**: `YourStrong@Passw0rd`
- **Database**: `BonyankopDB`

### Connect with SSMS or Azure Data Studio:
- Server: `localhost,1433`
- Authentication: SQL Server Authentication
- Login: `sa`
- Password: `YourStrong@Passw0rd`

## Mobile App Integration

### API Base URL

When testing on:

1. **Android Emulator**:
   ```
   http://10.0.2.2:5000/api
   ```

2. **iOS Simulator**:
   ```
   http://localhost:5000/api
   ```

3. **Physical Device** (same network):
   ```
   http://YOUR_COMPUTER_IP:5000/api
   ```
   
   Find your IP:
   ```bash
   # Windows
   ipconfig
   
   # Look for IPv4 Address under your active network adapter
   ```

### API Endpoints for Mobile

#### Sign Up
```http
POST http://YOUR_IP:5000/api/User/signup
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "password123"
}
```

#### Login
```http
POST http://YOUR_IP:5000/api/User/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

#### Response Format
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john@example.com",
  "username": "johndoe"
}
```

## Mobile App Code Examples

### Flutter Example

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class AuthService {
  static const String baseUrl = 'http://10.0.2.2:5000/api'; // Android Emulator
  
  Future<Map<String, dynamic>> signup(String username, String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/User/signup'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'username': username,
        'email': email,
        'password': password,
      }),
    );
    
    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception('Failed to sign up');
    }
  }
  
  Future<Map<String, dynamic>> login(String email, String password) async {
    final response = await http.post(
      Uri.parse('$baseUrl/User/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'email': email,
        'password': password,
      }),
    );
    
    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception('Failed to login');
    }
  }
}
```

### React Native Example

```javascript
const API_BASE_URL = 'http://10.0.2.2:5000/api'; // Android Emulator

export const signup = async (username, email, password) => {
  try {
    const response = await fetch(`${API_BASE_URL}/User/signup`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username,
        email,
        password,
      }),
    });
    
    const data = await response.json();
    
    if (response.ok) {
      return data;
    } else {
      throw new Error(data.message || 'Signup failed');
    }
  } catch (error) {
    throw error;
  }
};

export const login = async (email, password) => {
  try {
    const response = await fetch(`${API_BASE_URL}/User/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email,
        password,
      }),
    });
    
    const data = await response.json();
    
    if (response.ok) {
      return data;
    } else {
      throw new Error(data.message || 'Login failed');
    }
  } catch (error) {
    throw error;
  }
};
```

## Testing with Mobile

### Step 1: Find Your Computer's IP Address

**Windows:**
```bash
ipconfig
```
Look for "IPv4 Address" (e.g., 192.168.1.100)

### Step 2: Update Mobile App Base URL

Replace `localhost` with your computer's IP address in your mobile app code.

### Step 3: Ensure Firewall Allows Connection

**Windows Firewall:**
1. Go to Windows Defender Firewall
2. Click "Advanced settings"
3. Add inbound rule for port 5000

Or run PowerShell as Administrator:
```powershell
New-NetFirewallRule -DisplayName "Bonyankop API" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

### Step 4: Test from Mobile Device

Make sure your mobile device is on the same Wi-Fi network as your computer.

## Troubleshooting

### SQL Server Container Issues

**Container won't start:**
```bash
docker-compose logs sqlserver
```

**Reset SQL Server:**
```bash
docker-compose down -v
docker-compose up -d sqlserver
```

### API Connection Issues

**Check if containers are running:**
```bash
docker-compose ps
```

**Check API logs:**
```bash
docker-compose logs api
```

**Restart API:**
```bash
docker-compose restart api
```

### Mobile Can't Connect

1. Verify your computer's IP address
2. Ensure firewall allows port 5000
3. Check that both devices are on the same network
4. Try using `http` instead of `https`
5. Test the API from your browser first: `http://YOUR_IP:5000/api/User/signup`

## Production Deployment

Before deploying to production:

1. ✅ Change SQL Server SA password
2. ✅ Change JWT SecretKey
3. ✅ Enable HTTPS
4. ✅ Use environment variables for sensitive data
5. ✅ Implement rate limiting
6. ✅ Add proper error handling and logging
7. ✅ Set up database backups

## Environment Variables

Create `.env` file for production:
```env
SA_PASSWORD=YourProductionPassword123!
JWT_SECRET=YourProductionSecretKey456!
ASPNETCORE_ENVIRONMENT=Production
```

Update docker-compose.yml to use `.env`:
```yaml
environment:
  - SA_PASSWORD=${SA_PASSWORD}
  - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=BonyankopDB;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;
```
