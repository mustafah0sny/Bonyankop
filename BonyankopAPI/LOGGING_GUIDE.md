# Logging Guide - Bonyankop API

## Overview

The Bonyankop API uses **Serilog** for comprehensive logging. Logs are written to both the console and files, making it easy to diagnose issues in production.

## Log Files Location

Logs are stored in the `logs/` directory (automatically created if it doesn't exist):

- **`logs/bonyankop-YYYYMMDD.log`** - All logs (Info, Warning, Error, etc.)
- **`logs/bonyankop-errors-YYYYMMDD.log`** - Error logs only

## Log Retention

- **General logs**: Kept for 30 days
- **Error logs**: Kept for 90 days
- Logs roll over daily at midnight

## What Gets Logged

### 1. Application Lifecycle
- Application startup
- Application shutdown
- Fatal errors that cause the application to crash

### 2. HTTP Requests
Every HTTP request is logged with:
- Request method (GET, POST, etc.)
- Request path
- Response status code
- Response time in milliseconds
- Request host
- Request scheme (HTTP/HTTPS)
- Remote IP address
- User agent

### 3. Exceptions
All unhandled exceptions are logged with:
- Full exception message
- Stack trace
- Inner exception details
- Request context (path, method, IP)

### 4. Database Operations
- Entity Framework queries (in Information level)
- Database connection issues
- Migration errors

## Accessing Logs in Production

### Via FTP
1. Connect to your hosting server via FTP
2. Navigate to the application root directory
3. Open the `logs/` folder
4. Download the log files you need

### Via Hosting Control Panel
Most hosting providers (like MonsterASP) provide a file manager where you can:
1. Browse to the `logs/` directory
2. View or download log files

## Log Levels

The application uses the following log levels:

- **Debug**: Detailed information for diagnosing problems (development only)
- **Information**: General informational messages (startup, requests)
- **Warning**: Potentially harmful situations
- **Error**: Error events that might still allow the app to continue
- **Fatal**: Very severe error events that will lead the application to abort

## Reading Log Files

### Log Format
```
2025-12-31 14:30:45.123 +00:00 [INF] HTTP GET /api/health responded 200 in 15.2345 ms {"RequestHost":"bonyankop.runasp.net","RequestScheme":"https","RemoteIP":"192.168.1.1","UserAgent":"Mozilla/5.0..."}
```

### Example Log Entries

**Successful Request:**
```
2025-12-31 14:30:45.123 +00:00 [INF] HTTP GET /api/health responded 200 in 15.2345 ms
```

**Error:**
```
2025-12-31 14:31:22.456 +00:00 [ERR] An unhandled exception occurred while processing the request. Path: /api/users, Method: POST, RemoteIP: 192.168.1.1
System.NullReferenceException: Object reference not set to an instance of an object.
   at BonyankopAPI.Controllers.UserController.CreateUser(UserDto user) in /src/Controllers/UserController.cs:line 45
```

## Troubleshooting Production Issues

### Step 1: Check Error Logs
Start with the error-specific log file:
```
logs/bonyankop-errors-20251231.log
```

### Step 2: Find the Timestamp
Note the timestamp of when the issue occurred.

### Step 3: Check Full Logs
Open the general log file for that date and search for the timestamp:
```
logs/bonyankop-20251231.log
```

### Step 4: Look for Context
Check the requests that happened before and after the error to understand the context.

## Common Issues and Solutions

### Issue: 500 Internal Server Error

**Check logs for:**
- Database connection errors
- Null reference exceptions
- Configuration issues

**Example log entry:**
```
[ERR] An error occurred using the connection to database 'BonyankopDB' on server 'db34725.public.databaseasp.net'.
```

### Issue: Slow Performance

**Check logs for:**
- Long request durations
- Database query times

**Example:**
```
[INF] HTTP GET /api/projects responded 200 in 5234.5678 ms
```
(This indicates a 5+ second response time)

### Issue: Authentication Failures

**Check logs for:**
- JWT token validation errors
- User authentication attempts

## Configuration

### Changing Log Levels

Edit `appsettings.json` or `appsettings.Production.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    }
  }
}
```

### Disabling Specific Logs

To reduce log verbosity, increase the minimum level:
- `Debug` → `Information` (less verbose)
- `Information` → `Warning` (even less verbose)
- `Warning` → `Error` (only errors)

## Best Practices

1. **Check logs regularly** - Even if there are no reported issues
2. **Monitor error logs** - Set up alerts for new entries in error logs
3. **Archive old logs** - Download and archive logs before they're automatically deleted
4. **Don't log sensitive data** - Passwords, tokens, and personal data should never be logged
5. **Use log levels appropriately** - Don't log everything as Error

## Support

If you need help interpreting logs or diagnosing issues:
1. Download the relevant log file(s)
2. Search for ERROR or FATAL entries
3. Include the full error message and stack trace when seeking support

## Log File Sizes

Typical log file sizes:
- Low traffic: 1-10 MB per day
- Medium traffic: 10-100 MB per day
- High traffic: 100+ MB per day

If log files become too large, consider:
1. Increasing the minimum log level
2. Reducing request logging detail
3. Implementing log aggregation services

