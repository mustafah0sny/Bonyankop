# GitHub Actions Deployment Setup

This repository uses GitHub Actions to automatically deploy the Bonyankop API to MonsterASP (RunASP).

## Website Details
- **URL**: https://bonyankop.runasp.net
- **Hosting**: MonsterASP / RunASP
- **Control Panel**: https://admin.monsterasp.net
- **Login Email**: abdelaliemhosny18@gmail.com

## Required Secrets

You need to configure the following secrets in your GitHub repository:

### Go to: Repository → Settings → Secrets and variables → Actions → New repository secret

1. **PRODUCTION_CONNECTION_STRING**
   ```
   Server=db34725.databaseasp.net;Database=db34725;User Id=db34725;Password=3g#F=Rm79!Ea;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;
   ```

2. **JWT_SECRET_KEY**
   ```
   YourSuperSecretKeyForJwtTokenGeneration123456789
   ```
   ⚠️ **Important**: Change this to a new secure random key for production!

3. **FTP_SERVER**
   - Get from MonsterASP control panel (typically: `ftp.runasp.net` or `ftp.monsterasp.net`)
   - Login to: https://admin.monsterasp.net
   - Navigate to: File Manager or FTP Accounts section

4. **FTP_USERNAME**
   - Get from MonsterASP control panel under FTP/File Manager section
   - Usually matches your site name or email

5. **FTP_PASSWORD**
   - Get from MonsterASP control panel under FTP/File Manager section
   - May need to create/reset FTP password if not provided

## Setting Up Secrets in GitHub

1. Go to: https://github.com/mustafah0sny/Bonyankop/settings/secrets/actions
2. Click **New repository secret**
3. Add each secret name and value from above
4. Save each secret

## How to Deploy

### Automatic Deployment
Simply push your changes to the `main` branch:
```bash
git add .
git commit -m "Your commit message"
git push origin main
```

### Manual Deployment
Go to GitHub → Actions → Deploy to MonsterASP → Run workflow

## Workflow Steps

1. ✅ Checkout code from repository
2. ✅ Setup .NET 10.0 environment
3. ✅ Restore NuGet packages
4. ✅ Build the application in Release mode
5. ✅ Publish the application
6. ✅ Create production configuration file
7. ✅ Deploy files via FTP to MonsterASP

## Monitoring Deployments

- View deployment status: GitHub → Actions tab
- Check logs for any errors
- Deployment typically takes 3-5 minutes

## Local Development

For local development, use:
```bash
dotnet run
```

This uses the local database connection from `appsettings.json`.

## Production Database Migrations

To run migrations on production database:
```bash
dotnet ef database update --connection "Server=db34725.databaseasp.net;Database=db34725;User Id=db34725;Password=3g#F=Rm79!Ea;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True;"
```

Note: This requires VPN or running from within MonsterASP's network.
