#!/bin/bash

echo "🚀 Starting Bonyankop API with Docker..."
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Start SQL Server
echo "📦 Starting SQL Server container..."
docker-compose up -d sqlserver

# Wait for SQL Server to be ready
echo "⏳ Waiting for SQL Server to be ready (30 seconds)..."
sleep 30

# Apply database migrations
echo "🔄 Applying database migrations..."
dotnet ef database update

if [ $? -eq 0 ]; then
    echo "✅ Database migrations applied successfully!"
else
    echo "❌ Failed to apply database migrations. Please check the error above."
    exit 1
fi

# Start the API
echo "🚀 Starting API..."
dotnet run

echo ""
echo "✅ Setup complete!"
echo ""
echo "API is running at:"
echo "  - HTTP: http://localhost:5000"
echo "  - HTTPS: https://localhost:5001"
echo ""
echo "For mobile testing, use your computer's IP address instead of localhost."
echo "Run 'ipconfig' (Windows) to find your IP address."
