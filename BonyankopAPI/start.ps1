# PowerShell startup script for Bonyankop API

Write-Host "🚀 Starting Bonyankop API with Docker..." -ForegroundColor Green
Write-Host ""

# Check if Docker is installed
try {
    docker --version | Out-Null
} catch {
    Write-Host "❌ Docker is not installed. Please install Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Check if Docker is running
docker info 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Start SQL Server
Write-Host "📦 Starting SQL Server container..." -ForegroundColor Cyan
docker-compose up -d sqlserver

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to start SQL Server container." -ForegroundColor Red
    exit 1
}

# Wait for SQL Server to be ready
Write-Host "⏳ Waiting for SQL Server to be ready (30 seconds)..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check if SQL Server is ready
Write-Host "🔍 Checking SQL Server status..." -ForegroundColor Cyan
docker-compose logs sqlserver | Select-String "SQL Server is now ready for client connections"

# Apply database migrations
Write-Host "🔄 Applying database migrations..." -ForegroundColor Cyan
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Database migrations applied successfully!" -ForegroundColor Green
} else {
    Write-Host "❌ Failed to apply database migrations. Please check the error above." -ForegroundColor Red
    Write-Host "You can manually run: dotnet ef database update" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "✅ Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Starting API..." -ForegroundColor Cyan
Write-Host ""
Write-Host "API endpoints:" -ForegroundColor Yellow
Write-Host "  - HTTP: http://localhost:5000" -ForegroundColor White
Write-Host "  - HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host ""
Write-Host "📱 For mobile testing:" -ForegroundColor Yellow

# Get local IP address
$ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.InterfaceAlias -notlike "*Loopback*" -and $_.InterfaceAlias -notlike "*VirtualBox*" -and $_.InterfaceAlias -notlike "*VMware*"} | Select-Object -First 1).IPAddress

if ($ipAddress) {
    Write-Host "  - Use: http://$ipAddress:5000" -ForegroundColor White
    Write-Host "  - Android Emulator: http://10.0.2.2:5000" -ForegroundColor White
} else {
    Write-Host "  - Run 'ipconfig' to find your IP address" -ForegroundColor White
}

Write-Host ""
Write-Host "Press Ctrl+C to stop the API" -ForegroundColor Yellow
Write-Host ""

# Start the API
dotnet run
