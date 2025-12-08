using Microsoft.AspNetCore.Identity;
using BonyankopAPI.Models;
using Microsoft.EntityFrameworkCore;
using Bogus;

namespace BonyankopAPI.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users (Admin, Engineers, Companies, Citizens)
            await SeedUsersAsync(userManager);

            // Seed Provider Profiles
            await SeedProviderProfilesAsync(context, userManager);
            
            // Seed Portfolio Items
            await SeedPortfolioItemsAsync(context);
            
            // Seed additional fake users and profiles using Bogus
            await SeedFakeDataAsync(userManager, context);
            
            // Seed Service Requests and Quotes
            await SeedServiceMarketplaceAsync(context, userManager);
            
            // Seed Projects and Ratings
            await SeedProjectExecutionAsync(context, userManager);
      
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roleNames = { "CITIZEN", "ENGINEER", "COMPANY", "GOVERNMENT", "ADMIN" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> 
                    { 
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // Seed Admin User
            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin@bonyankop.com",
                Email = "admin@bonyankop.com",
                EmailConfirmed = true,
                FullName = "System Administrator",
                Role = UserRole.ADMIN,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567890",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Admin@123456", "ADMIN");

            // Seed Engineers
            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "john.engineer@bonyankop.com",
                Email = "john.engineer@bonyankop.com",
                EmailConfirmed = true,
                FullName = "John Anderson",
                Role = UserRole.ENGINEER,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567891",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Engineer@123", "ENGINEER");

            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "sarah.engineer@bonyankop.com",
                Email = "sarah.engineer@bonyankop.com",
                EmailConfirmed = true,
                FullName = "Sarah Johnson",
                Role = UserRole.ENGINEER,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567892",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Engineer@123", "ENGINEER");

            // Seed Companies
            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "contact@buildpro.com",
                Email = "contact@buildpro.com",
                EmailConfirmed = true,
                FullName = "BuildPro Construction",
                Role = UserRole.COMPANY,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567893",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Company@123", "COMPANY");

            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "info@homefixit.com",
                Email = "info@homefixit.com",
                EmailConfirmed = true,
                FullName = "HomeFixIt Services",
                Role = UserRole.COMPANY,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567894",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Company@123", "COMPANY");

            // Seed Citizens
            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "alice.citizen@example.com",
                Email = "alice.citizen@example.com",
                EmailConfirmed = true,
                FullName = "Alice Cooper",
                Role = UserRole.CITIZEN,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567895",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Citizen@123", "CITIZEN");

            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "bob.citizen@example.com",
                Email = "bob.citizen@example.com",
                EmailConfirmed = true,
                FullName = "Bob Martinez",
                Role = UserRole.CITIZEN,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567896",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Citizen@123", "CITIZEN");

            // Seed Government User
            await SeedUserAsync(userManager, new User
            {
                Id = Guid.NewGuid(),
                UserName = "gov.official@city.gov",
                Email = "gov.official@city.gov",
                EmailConfirmed = true,
                FullName = "City Building Inspector",
                Role = UserRole.GOVERNMENT,
                IsVerified = true,
                IsActive = true,
                PhoneNumber = "+1234567897",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "Government@123", "GOVERNMENT");
        }

        private static async Task SeedUserAsync(UserManager<User> userManager, User user, string password, string role)
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email!);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static async Task SeedProviderProfilesAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            // Only seed if no provider profiles exist
            if (await context.ProviderProfiles.AnyAsync())
            {
                return;
            }

            // Get the engineer and company users
            var johnEngineer = await userManager.FindByEmailAsync("john.engineer@bonyankop.com");
            var sarahEngineer = await userManager.FindByEmailAsync("sarah.engineer@bonyankop.com");
            var buildProCompany = await userManager.FindByEmailAsync("contact@buildpro.com");
            var homeFixItCompany = await userManager.FindByEmailAsync("info@homefixit.com");

            var profiles = new List<ProviderProfile>();

            // Engineer Profile 1
            if (johnEngineer != null)
            {
                profiles.Add(new ProviderProfile
                {
                    ProviderId = Guid.NewGuid(),
                    UserId = johnEngineer.Id,
                    ProviderType = ProviderType.ENGINEER,
                    BusinessName = "John Anderson Engineering Services",
                    Description = "Licensed professional engineer specializing in residential and commercial structural assessments. Over 15 years of experience in construction engineering and building inspections.",
                    ServicesOffered = new List<string> { "Structural Assessment", "Foundation Inspection", "Renovation Planning", "Building Code Compliance", "Engineering Reports" },
                    Certifications = new List<string> { "Professional Engineer (PE)", "Structural Engineering Certification", "LEED AP", "ICC Building Inspector" },
                    CoverageAreas = new List<string> { "New York", "Brooklyn", "Manhattan", "Queens" },
                    LicenseNumber = "PE-NY-45678",
                    YearsOfExperience = 15,
                    AverageRating = 4.8m,
                    TotalProjects = 127,
                    TotalRatings = 98,
                    CompletionRate = 96.5m,
                    ResponseTimeHours = 4.5m,
                    IsVerified = true,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-18),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Engineer Profile 2
            if (sarahEngineer != null)
            {
                profiles.Add(new ProviderProfile
                {
                    ProviderId = Guid.NewGuid(),
                    UserId = sarahEngineer.Id,
                    ProviderType = ProviderType.ENGINEER,
                    BusinessName = "Sarah Johnson Consulting",
                    Description = "Electrical and mechanical engineering consultant with expertise in HVAC systems, electrical installations, and energy efficiency upgrades.",
                    ServicesOffered = new List<string> { "HVAC System Design", "Electrical Planning", "Energy Audits", "MEP Coordination", "Smart Home Integration" },
                    Certifications = new List<string> { "Professional Engineer (PE)", "HVAC Design Certification", "Energy Auditor", "Master Electrician License" },
                    CoverageAreas = new List<string> { "Los Angeles", "Orange County", "San Diego", "Riverside" },
                    LicenseNumber = "PE-CA-89012",
                    YearsOfExperience = 12,
                    AverageRating = 4.9m,
                    TotalProjects = 89,
                    TotalRatings = 76,
                    CompletionRate = 98.2m,
                    ResponseTimeHours = 3.0m,
                    IsVerified = true,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-12),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Company Profile 1
            if (buildProCompany != null)
            {
                profiles.Add(new ProviderProfile
                {
                    ProviderId = Guid.NewGuid(),
                    UserId = buildProCompany.Id,
                    ProviderType = ProviderType.COMPANY,
                    BusinessName = "BuildPro Construction Corp",
                    Description = "Full-service general contractor with 25 years of experience in residential and commercial construction. Licensed, bonded, and insured with A+ BBB rating.",
                    ServicesOffered = new List<string> { "General Construction", "Home Renovation", "Kitchen Remodeling", "Bathroom Remodeling", "Roofing", "Siding", "Foundation Repair", "Commercial Build-outs" },
                    Certifications = new List<string> { "General Contractor License", "OSHA Certified", "EPA Lead-Safe Certified", "BBB Accredited A+", "Contractor of the Year 2023" },
                    CoverageAreas = new List<string> { "Chicago", "Naperville", "Aurora", "Joliet", "Evanston" },
                    LicenseNumber = "GC-IL-12345",
                    YearsOfExperience = 25,
                    AverageRating = 4.7m,
                    TotalProjects = 342,
                    TotalRatings = 287,
                    CompletionRate = 94.8m,
                    ResponseTimeHours = 6.0m,
                    IsVerified = true,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-3),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Company Profile 2
            if (homeFixItCompany != null)
            {
                profiles.Add(new ProviderProfile
                {
                    ProviderId = Guid.NewGuid(),
                    UserId = homeFixItCompany.Id,
                    ProviderType = ProviderType.COMPANY,
                    BusinessName = "HomeFixIt Services LLC",
                    Description = "Your trusted partner for home repairs and maintenance. We specialize in plumbing, electrical work, and general handyman services with same-day emergency response.",
                    ServicesOffered = new List<string> { "Plumbing Repair", "Electrical Repair", "Handyman Services", "Emergency Repairs", "Appliance Installation", "Drywall Repair", "Painting", "Carpentry" },
                    Certifications = new List<string> { "Licensed Plumber", "Licensed Electrician", "Insured & Bonded", "Emergency Response Certified", "Customer Service Excellence Award" },
                    CoverageAreas = new List<string> { "Houston", "Sugar Land", "The Woodlands", "Pearland", "Katy" },
                    LicenseNumber = "HC-TX-67890",
                    YearsOfExperience = 10,
                    AverageRating = 4.6m,
                    TotalProjects = 456,
                    TotalRatings = 398,
                    CompletionRate = 92.3m,
                    ResponseTimeHours = 2.5m,
                    IsVerified = true,
                    IsFeatured = false,
                    CreatedAt = DateTime.UtcNow.AddYears(-2),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (profiles.Any())
            {
                await context.ProviderProfiles.AddRangeAsync(profiles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedFakeDataAsync(UserManager<User> userManager, ApplicationDbContext context)
        {
            // Check if we already have enough fake data
            var citizenCount = await context.Users.CountAsync(u => u.Role == UserRole.CITIZEN);
            var engineerCount = await context.Users.CountAsync(u => u.Role == UserRole.ENGINEER);
            var companyCount = await context.Users.CountAsync(u => u.Role == UserRole.COMPANY);

            // Seed 20 fake citizens if less than 10 exist
            if (citizenCount < 10)
            {
                var citizenFaker = new Faker<User>()
                    .RuleFor(u => u.Id, f => Guid.NewGuid())
                    .RuleFor(u => u.UserName, f => f.Internet.Email())
                    .RuleFor(u => u.Email, (f, u) => u.UserName)
                    .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName!.ToUpper())
                    .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email!.ToUpper())
                    .RuleFor(u => u.EmailConfirmed, f => true)
                    .RuleFor(u => u.FullName, f => f.Name.FullName())
                    .RuleFor(u => u.Role, f => UserRole.CITIZEN)
                    .RuleFor(u => u.IsVerified, f => f.Random.Bool(0.8f))
                    .RuleFor(u => u.IsActive, f => true)
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(u => u.CreatedAt, f => f.Date.Past(2))
                    .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt);

                var citizens = citizenFaker.Generate(20);
                foreach (var citizen in citizens)
                {
                    await SeedUserAsync(userManager, citizen, "Citizen@123", "CITIZEN");
                }
            }

            // Seed 10 fake engineers with provider profiles
            if (engineerCount < 6)
            {
                var engineerFaker = new Faker<User>()
                    .RuleFor(u => u.Id, f => Guid.NewGuid())
                    .RuleFor(u => u.UserName, f => f.Internet.Email())
                    .RuleFor(u => u.Email, (f, u) => u.UserName)
                    .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName!.ToUpper())
                    .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email!.ToUpper())
                    .RuleFor(u => u.EmailConfirmed, f => true)
                    .RuleFor(u => u.FullName, f => f.Name.FullName())
                    .RuleFor(u => u.Role, f => UserRole.ENGINEER)
                    .RuleFor(u => u.IsVerified, f => true)
                    .RuleFor(u => u.IsActive, f => true)
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(u => u.CreatedAt, f => f.Date.Past(3))
                    .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt);

                var engineers = engineerFaker.Generate(10);
                foreach (var engineer in engineers)
                {
                    await SeedUserAsync(userManager, engineer, "Engineer@123", "ENGINEER");
                    
                    // Create provider profile for engineer
                    var engineerProfile = CreateFakeEngineerProfile(engineer.Id);
                    await context.ProviderProfiles.AddAsync(engineerProfile);
                }
                await context.SaveChangesAsync();
            }

            // Seed 8 fake companies with provider profiles
            if (companyCount < 6)
            {
                var companyFaker = new Faker<User>()
                    .RuleFor(u => u.Id, f => Guid.NewGuid())
                    .RuleFor(u => u.UserName, f => f.Internet.Email())
                    .RuleFor(u => u.Email, (f, u) => u.UserName)
                    .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName!.ToUpper())
                    .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email!.ToUpper())
                    .RuleFor(u => u.EmailConfirmed, f => true)
                    .RuleFor(u => u.FullName, f => f.Company.CompanyName())
                    .RuleFor(u => u.Role, f => UserRole.COMPANY)
                    .RuleFor(u => u.IsVerified, f => true)
                    .RuleFor(u => u.IsActive, f => true)
                    .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                    .RuleFor(u => u.CreatedAt, f => f.Date.Past(5))
                    .RuleFor(u => u.UpdatedAt, (f, u) => u.CreatedAt);

                var companies = companyFaker.Generate(8);
                foreach (var company in companies)
                {
                    await SeedUserAsync(userManager, company, "Company@123", "COMPANY");
                    
                    // Create provider profile for company
                    var companyProfile = CreateFakeCompanyProfile(company.Id);
                    await context.ProviderProfiles.AddAsync(companyProfile);
                }
                await context.SaveChangesAsync();
            }
        }

        private static ProviderProfile CreateFakeEngineerProfile(Guid userId)
        {
            var faker = new Faker();
            var engineeringSpecialties = new[] {
                "Structural Engineering",
                "Electrical Engineering",
                "Mechanical Engineering",
                "Civil Engineering",
                "Environmental Engineering",
                "Geotechnical Engineering"
            };

            var certifications = new[] {
                "Professional Engineer (PE)",
                "Structural Engineering Certification",
                "LEED AP",
                "ICC Building Inspector",
                "HVAC Design Certification",
                "Energy Auditor",
                "Master Electrician License",
                "Green Building Certification"
            };

            var services = new[] {
                "Structural Assessment",
                "Foundation Inspection",
                "Renovation Planning",
                "Building Code Compliance",
                "Engineering Reports",
                "HVAC System Design",
                "Electrical Planning",
                "Energy Audits",
                "MEP Coordination",
                "Smart Home Integration",
                "Seismic Retrofitting",
                "Load Calculations"
            };

            var cities = new[] {
                "New York", "Los Angeles", "Chicago", "Houston", "Phoenix",
                "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose",
                "Austin", "Jacksonville", "Fort Worth", "Columbus", "Charlotte",
                "San Francisco", "Indianapolis", "Seattle", "Denver", "Boston"
            };

            var specialty = faker.PickRandom(engineeringSpecialties);
            var yearsExp = faker.Random.Int(5, 30);
            var totalProjects = faker.Random.Int(20, 200);
            var totalRatings = (int)(totalProjects * faker.Random.Decimal(0.6m, 0.9m));

            return new ProviderProfile
            {
                ProviderId = Guid.NewGuid(),
                UserId = userId,
                ProviderType = ProviderType.ENGINEER,
                BusinessName = $"{faker.Name.FullName()} {specialty} Services",
                Description = faker.Lorem.Paragraph(3),
                ServicesOffered = faker.PickRandom(services, faker.Random.Int(4, 8)).ToList(),
                Certifications = faker.PickRandom(certifications, faker.Random.Int(2, 5)).ToList(),
                CoverageAreas = faker.PickRandom(cities, faker.Random.Int(2, 5)).ToList(),
                LicenseNumber = $"PE-{faker.Address.StateAbbr()}-{faker.Random.Int(10000, 99999)}",
                YearsOfExperience = yearsExp,
                AverageRating = faker.Random.Decimal(3.5m, 5.0m),
                TotalProjects = totalProjects,
                TotalRatings = totalRatings,
                CompletionRate = faker.Random.Decimal(85.0m, 99.9m),
                ResponseTimeHours = faker.Random.Decimal(1.0m, 24.0m),
                IsVerified = faker.Random.Bool(0.8f),
                IsFeatured = faker.Random.Bool(0.3f),
                CreatedAt = faker.Date.Past(yearsExp / 2),
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static ProviderProfile CreateFakeCompanyProfile(Guid userId)
        {
            var faker = new Faker();
            var companyTypes = new[] {
                "Construction",
                "Remodeling",
                "Repair Services",
                "Home Services",
                "Contracting",
                "Building Solutions"
            };

            var certifications = new[] {
                "General Contractor License",
                "OSHA Certified",
                "EPA Lead-Safe Certified",
                "BBB Accredited A+",
                "Licensed & Insured",
                "Emergency Response Certified",
                "Customer Service Excellence Award",
                "Contractor of the Year",
                "Energy Star Partner",
                "Master Craftsman Certified"
            };

            var services = new[] {
                "General Construction",
                "Home Renovation",
                "Kitchen Remodeling",
                "Bathroom Remodeling",
                "Roofing",
                "Siding",
                "Foundation Repair",
                "Commercial Build-outs",
                "Plumbing Repair",
                "Electrical Repair",
                "Handyman Services",
                "Emergency Repairs",
                "Appliance Installation",
                "Drywall Repair",
                "Painting",
                "Carpentry",
                "Flooring Installation",
                "Window Replacement",
                "Deck Building",
                "Basement Finishing"
            };

            var cities = new[] {
                "New York", "Los Angeles", "Chicago", "Houston", "Phoenix",
                "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose",
                "Austin", "Jacksonville", "Fort Worth", "Columbus", "Charlotte",
                "San Francisco", "Indianapolis", "Seattle", "Denver", "Boston"
            };

            var companyType = faker.PickRandom(companyTypes);
            var yearsExp = faker.Random.Int(5, 40);
            var totalProjects = faker.Random.Int(50, 500);
            var totalRatings = (int)(totalProjects * faker.Random.Decimal(0.5m, 0.85m));

            return new ProviderProfile
            {
                ProviderId = Guid.NewGuid(),
                UserId = userId,
                ProviderType = ProviderType.COMPANY,
                BusinessName = $"{faker.Company.CompanyName()} {companyType}",
                Description = faker.Company.CatchPhrase() + ". " + faker.Lorem.Paragraph(2),
                ServicesOffered = faker.PickRandom(services, faker.Random.Int(5, 12)).ToList(),
                Certifications = faker.PickRandom(certifications, faker.Random.Int(3, 6)).ToList(),
                CoverageAreas = faker.PickRandom(cities, faker.Random.Int(3, 7)).ToList(),
                LicenseNumber = $"GC-{faker.Address.StateAbbr()}-{faker.Random.Int(10000, 99999)}",
                YearsOfExperience = yearsExp,
                AverageRating = faker.Random.Decimal(3.0m, 5.0m),
                TotalProjects = totalProjects,
                TotalRatings = totalRatings,
                CompletionRate = faker.Random.Decimal(80.0m, 98.0m),
                ResponseTimeHours = faker.Random.Decimal(2.0m, 48.0m),
                IsVerified = faker.Random.Bool(0.7f),
                IsFeatured = faker.Random.Bool(0.25f),
                CreatedAt = faker.Date.Past(yearsExp / 3),
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static async Task SeedPortfolioItemsAsync(ApplicationDbContext context)
        {
            if (await context.PortfolioItems.AnyAsync())
            {
                return; // Portfolio items already seeded
            }

            var faker = new Faker();
            var providerProfiles = await context.ProviderProfiles.ToListAsync();

            if (!providerProfiles.Any())
            {
                return; // No providers to add portfolio items for
            }

            var projectTypes = new[]
            {
                "Kitchen Renovation", "Bathroom Remodel", "Foundation Repair", "Roof Replacement",
                "Electrical Upgrade", "Plumbing Installation", "HVAC System", "Home Addition",
                "Deck Construction", "Basement Finishing", "Window Installation", "Siding Replacement",
                "Concrete Work", "Flooring Installation", "Paint Job", "Landscaping",
                "Pool Installation", "Garage Construction", "Solar Panel Installation", "Smart Home Integration"
            };

            var cities = new[]
            {
                "New York, NY", "Los Angeles, CA", "Chicago, IL", "Houston, TX",
                "Phoenix, AZ", "Philadelphia, PA", "San Antonio, TX", "San Diego, CA",
                "Dallas, TX", "San Jose, CA", "Austin, TX", "Jacksonville, FL",
                "Fort Worth, TX", "Columbus, OH", "Charlotte, NC", "San Francisco, CA"
            };

            var portfolioItems = new List<PortfolioItem>();

            foreach (var provider in providerProfiles)
            {
                // Each provider gets 2-8 portfolio items
                var itemCount = faker.Random.Int(2, 8);

                for (int i = 0; i < itemCount; i++)
                {
                    var projectType = faker.PickRandom(projectTypes);
                    var imageCount = faker.Random.Int(3, 10);
                    var images = new List<string>();

                    for (int j = 0; j < imageCount; j++)
                    {
                        images.Add($"https://picsum.photos/800/600?random={Guid.NewGuid()}");
                    }

                    var portfolioItem = new PortfolioItem
                    {
                        PortfolioId = Guid.NewGuid(),
                        ProviderId = provider.ProviderId,
                        Title = $"{projectType} - {faker.Address.City()}",
                        Description = faker.Lorem.Paragraphs(2, 3),
                        ProjectType = projectType,
                        Images = images,
                        ProjectDate = faker.Date.Past(Math.Min(provider.YearsOfExperience ?? 5, 10)),
                        Location = faker.PickRandom(cities),
                        DisplayOrder = i,
                        IsFeatured = i < 2 && faker.Random.Bool(0.6f), // First 2 items have higher chance of being featured
                        CreatedAt = faker.Date.Past(1),
                        UpdatedAt = DateTime.UtcNow
                    };

                    portfolioItems.Add(portfolioItem);
                }
            }

            await context.PortfolioItems.AddRangeAsync(portfolioItems);
            await context.SaveChangesAsync();
        }

        private static async Task SeedServiceMarketplaceAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (await context.ServiceRequests.AnyAsync())
            {
                return; // Already seeded
            }

            var faker = new Faker();

            // Get citizens and providers
            var citizens = await userManager.GetUsersInRoleAsync("CITIZEN");
            var providerProfiles = await context.ProviderProfiles.ToListAsync();

            if (!citizens.Any() || !providerProfiles.Any())
            {
                return; // Need citizens and providers first
            }

            var problemCategories = new[] { "PLUMBING", "ELECTRICAL", "STRUCTURAL", "HVAC", "ROOFING", "CARPENTRY", "PAINTING", "FLOORING" };
            var providerTypes = new[] { "ENGINEER", "COMPANY" };
            var propertyTypes = new[] { "Residential House", "Apartment", "Commercial Building", "Office", "Warehouse", "Restaurant", "Retail Store" };

            var serviceRequests = new List<ServiceRequest>();

            // Create 30 service requests
            for (int i = 0; i < 30; i++)
            {
                var citizen = faker.PickRandom(citizens.ToList());
                var category = faker.PickRandom(problemCategories);
                var imageCount = faker.Random.Int(1, 5);
                var images = new List<string>();

                for (int j = 0; j < imageCount; j++)
                {
                    images.Add($"https://picsum.photos/800/600?random={Guid.NewGuid()}");
                }

                var problemTitles = new Dictionary<string, string[]>
                {
                    ["PLUMBING"] = new[] { "Leaking pipe under sink", "Clogged main drain", "Water heater not working", "Low water pressure", "Burst pipe in basement" },
                    ["ELECTRICAL"] = new[] { "Circuit breaker keeps tripping", "Outlet not working", "Flickering lights", "Need new electrical panel", "Wiring upgrade needed" },
                    ["STRUCTURAL"] = new[] { "Cracks in foundation", "Sagging floor", "Wall damage", "Ceiling crack", "Support beam issue" },
                    ["HVAC"] = new[] { "AC not cooling", "Heating system malfunction", "Ductwork repair needed", "Thermostat not working", "Strange noise from HVAC" },
                    ["ROOFING"] = new[] { "Roof leak", "Missing shingles", "Damaged flashing", "Gutter repair needed", "Roof inspection required" },
                    ["CARPENTRY"] = new[] { "Door frame repair", "Cabinet installation", "Deck renovation", "Custom shelving", "Window frame repair" },
                    ["PAINTING"] = new[] { "Interior painting needed", "Exterior paint job", "Ceiling paint repair", "Cabinet refinishing", "Accent wall painting" },
                    ["FLOORING"] = new[] { "Hardwood floor refinishing", "Tile replacement", "Carpet installation", "Vinyl flooring repair", "Floor leveling needed" }
                };

                var title = faker.PickRandom(problemTitles[category]);
                var isRecent = faker.Random.Bool(0.7f); // 70% are recent requests
                var createdDate = isRecent ? faker.Date.Recent(14) : faker.Date.Past(2, DateTime.UtcNow.AddMonths(-1));

                var serviceRequest = new ServiceRequest
                {
                    RequestId = Guid.NewGuid(),
                    CitizenId = citizen.Id,
                    ProblemTitle = title,
                    ProblemDescription = faker.Lorem.Paragraphs(2, 3),
                    ProblemCategory = category,
                    AdditionalImages = images,
                    PreferredProviderType = faker.Random.Bool(0.6f) ? faker.PickRandom(providerTypes) : null,
                    PreferredServiceDate = faker.Random.Bool(0.5f) ? faker.Date.Soon(30) : null,
                    PropertyType = faker.PickRandom(propertyTypes),
                    PropertyAddress = faker.Address.FullAddress(),
                    ContactPhone = faker.Phone.PhoneNumber("+1##########"),
                    Status = RequestStatus.OPEN,
                    QuotesCount = 0,
                    ViewsCount = faker.Random.Int(5, 50),
                    ExpiresAt = faker.Random.Bool(0.7f) ? faker.Date.Soon(45) : null,
                    CreatedAt = createdDate,
                    UpdatedAt = createdDate
                };

                serviceRequests.Add(serviceRequest);
            }

            await context.ServiceRequests.AddRangeAsync(serviceRequests);
            await context.SaveChangesAsync();

            // Create quotes for service requests
            var quotes = new List<Quote>();

            foreach (var request in serviceRequests.Take(20)) // Only first 20 requests get quotes
            {
                var quoteCount = faker.Random.Int(1, 5); // Each request gets 1-5 quotes

                for (int i = 0; i < quoteCount; i++)
                {
                    var provider = faker.PickRandom(providerProfiles);

                    // Avoid duplicate quotes from same provider
                    if (quotes.Any(q => q.RequestId == request.RequestId && q.ProviderId == provider.ProviderId))
                    {
                        continue;
                    }

                    var laborCost = faker.Random.Decimal(500, 5000);
                    var materialsCost = faker.Random.Decimal(200, 3000);
                    var equipmentCost = faker.Random.Decimal(0, 1000);
                    var otherCosts = faker.Random.Decimal(0, 500);
                    var subtotal = laborCost + materialsCost + equipmentCost + otherCosts;
                    var taxAmount = subtotal * 0.08m;
                    var totalAmount = subtotal + taxAmount;

                    var costBreakdown = new CostBreakdown
                    {
                        LaborCost = laborCost,
                        MaterialsCost = materialsCost,
                        EquipmentCost = equipmentCost,
                        OtherCosts = otherCosts,
                        TaxAmount = taxAmount,
                        TotalAmount = totalAmount
                    };

                    var submittedDate = faker.Date.Between(request.CreatedAt, DateTime.UtcNow);
                    var validityDays = faker.Random.Int(15, 60);

                    var quote = new Quote
                    {
                        QuoteId = Guid.NewGuid(),
                        RequestId = request.RequestId,
                        ProviderId = provider.ProviderId,
                        EstimatedCost = totalAmount,
                        CostBreakdownJson = System.Text.Json.JsonSerializer.Serialize(costBreakdown),
                        EstimatedDurationDays = faker.Random.Int(1, 30),
                        TechnicalAssessment = faker.Lorem.Paragraph(),
                        ProposedSolution = faker.Lorem.Paragraphs(2),
                        MaterialsIncluded = faker.Random.Bool(0.7f),
                        WarrantyPeriodMonths = faker.Random.Int(6, 36),
                        TermsAndConditions = faker.Lorem.Paragraph(),
                        ValidityPeriodDays = validityDays,
                        Attachments = faker.Random.Bool(0.3f) 
                            ? new List<string> { $"https://example.com/quote-{Guid.NewGuid()}.pdf" } 
                            : new List<string>(),
                        Status = QuoteStatus.PENDING,
                        SubmittedAt = submittedDate,
                        ExpiresAt = submittedDate.AddDays(validityDays),
                        UpdatedAt = submittedDate
                    };

                    quotes.Add(quote);
                }
            }

            await context.Quotes.AddRangeAsync(quotes);
            await context.SaveChangesAsync();

            // Update service request quotes counts
            foreach (var request in serviceRequests.Take(20))
            {
                request.QuotesCount = quotes.Count(q => q.RequestId == request.RequestId);
                if (request.QuotesCount > 0)
                {
                    request.Status = RequestStatus.QUOTES_RECEIVED;
                }
            }

            context.ServiceRequests.UpdateRange(serviceRequests.Take(20));
            await context.SaveChangesAsync();
        }

        private static async Task SeedProjectExecutionAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (await context.Projects.AnyAsync())
            {
                return; // Projects already seeded
            }

            var faker = new Faker();
            
            // Get accepted quotes to create projects from
            var acceptedQuotes = await context.Quotes
                .Include(q => q.ServiceRequest)
                .Where(q => q.Status == QuoteStatus.PENDING)
                .Take(15)
                .ToListAsync();

            if (!acceptedQuotes.Any())
            {
                return; // Need quotes to create projects
            }

            var projects = new List<Project>();
            var now = DateTime.UtcNow;

            foreach (var quote in acceptedQuotes)
            {
                var request = quote.ServiceRequest;
                var scheduledStart = faker.Date.Between(now.AddDays(-60), now.AddDays(-30));
                var scheduledEnd = scheduledStart.AddDays(quote.EstimatedDurationDays ?? 7);
                
                // Determine project status based on timeline
                ProjectStatus status;
                DateTime? actualStart = null;
                DateTime? actualCompletion = null;
                var isCompleted = faker.Random.Bool(0.4f); // 40% completed
                var isInProgress = !isCompleted && faker.Random.Bool(0.5f); // 30% in progress
                
                if (isCompleted)
                {
                    status = ProjectStatus.COMPLETED;
                    actualStart = scheduledStart.AddDays(faker.Random.Int(-2, 2));
                    actualCompletion = actualStart.Value.AddDays(quote.EstimatedDurationDays ?? 7 + faker.Random.Int(-3, 5));
                }
                else if (isInProgress)
                {
                    status = ProjectStatus.IN_PROGRESS;
                    actualStart = scheduledStart.AddDays(faker.Random.Int(-2, 2));
                }
                else
                {
                    status = ProjectStatus.SCHEDULED;
                }

                var workNotes = new List<WorkNote>();
                if (status == ProjectStatus.IN_PROGRESS || status == ProjectStatus.COMPLETED)
                {
                    // Add initial work note
                    workNotes.Add(new WorkNote
                    {
                        Timestamp = actualStart ?? scheduledStart,
                        Author = "Provider",
                        Note = faker.PickRandom(
                            "Project started. Initial assessment completed.",
                            "Work commenced as scheduled. All materials on site.",
                            "Started work. Initial inspection shows scope is as expected.",
                            "Project kicked off. Team mobilized and ready."
                        ),
                        Images = new List<string>()
                    });

                    // Add progress notes for in-progress/completed
                    if (status == ProjectStatus.IN_PROGRESS || status == ProjectStatus.COMPLETED)
                    {
                        workNotes.Add(new WorkNote
                        {
                            Timestamp = (actualStart ?? scheduledStart).AddDays(faker.Random.Int(2, 4)),
                            Author = "Provider",
                            Note = faker.PickRandom(
                                "Good progress. 50% complete.",
                                "Work proceeding according to plan.",
                                "Milestone reached. On track for completion.",
                                "Phase 1 completed successfully."
                            ),
                            Images = new List<string> { "https://example.com/progress1.jpg" }
                        });
                    }

                    if (status == ProjectStatus.COMPLETED)
                    {
                        workNotes.Add(new WorkNote
                        {
                            Timestamp = (actualCompletion ?? scheduledEnd).AddDays(-1),
                            Author = "Provider",
                            Note = faker.PickRandom(
                                "Final touches being applied.",
                                "Nearing completion. Quality checks in progress.",
                                "Almost done. Final inspection tomorrow.",
                                "Completing remaining punch list items."
                            ),
                            Images = new List<string>()
                        });
                    }
                }

                var actualCost = quote.EstimatedCost + faker.Random.Decimal(-500, 1000);
                var costDifference = actualCost - quote.EstimatedCost;
                
                var project = new Project
                {
                    ProjectId = Guid.NewGuid(),
                    RequestId = quote.RequestId,
                    QuoteId = quote.QuoteId,
                    CitizenId = request.CitizenId,
                    ProviderId = quote.ProviderId,
                    ProjectTitle = $"Project: {request.ProblemTitle}",
                    ProjectDescription = request.ProblemDescription,
                    Status = status,
                    ScheduledStartDate = scheduledStart,
                    ActualStartDate = actualStart,
                    ScheduledEndDate = scheduledEnd,
                    ActualCompletionDate = actualCompletion,
                    AgreedCost = quote.EstimatedCost,
                    ActualCost = status == ProjectStatus.COMPLETED ? actualCost : null,
                    CostDifferenceReason = status == ProjectStatus.COMPLETED && Math.Abs(costDifference) > 100 
                        ? (costDifference > 0 
                            ? faker.PickRandom("Additional materials required", "Scope increased", "Unexpected complications", "Client requested upgrades")
                            : faker.PickRandom("Bulk discount on materials", "Efficient completion", "Less work than estimated"))
                        : null,
                    PaymentStatus = status == ProjectStatus.COMPLETED 
                        ? faker.PickRandom(PaymentStatus.COMPLETED, PaymentStatus.PARTIAL, PaymentStatus.PENDING)
                        : PaymentStatus.PENDING,
                    WorkNotesJson = System.Text.Json.JsonSerializer.Serialize(workNotes),
                    BeforeImages = status != ProjectStatus.SCHEDULED 
                        ? new List<string> { "https://example.com/before1.jpg", "https://example.com/before2.jpg" }
                        : new List<string>(),
                    DuringImages = status == ProjectStatus.IN_PROGRESS || status == ProjectStatus.COMPLETED
                        ? new List<string> { "https://example.com/during1.jpg", "https://example.com/during2.jpg" }
                        : new List<string>(),
                    AfterImages = status == ProjectStatus.COMPLETED
                        ? new List<string> { "https://example.com/after1.jpg", "https://example.com/after2.jpg", "https://example.com/after3.jpg" }
                        : new List<string>(),
                    TechnicalReportUrl = status == ProjectStatus.COMPLETED ? "https://example.com/technical-report.pdf" : null,
                    CompletionCertificateUrl = status == ProjectStatus.COMPLETED ? "https://example.com/certificate.pdf" : null,
                    WarrantyStartDate = status == ProjectStatus.COMPLETED ? actualCompletion : null,
                    WarrantyEndDate = status == ProjectStatus.COMPLETED && actualCompletion.HasValue
                        ? actualCompletion.Value.AddMonths(quote.WarrantyPeriodMonths ?? 12)
                        : null,
                    CitizenSatisfaction = status == ProjectStatus.COMPLETED 
                        ? faker.PickRandom("Very satisfied", "Satisfied", "Excellent work", "Met expectations", "Good quality")
                        : null,
                    CreatedAt = scheduledStart.AddDays(-7),
                    UpdatedAt = status == ProjectStatus.COMPLETED && actualCompletion.HasValue ? actualCompletion.Value : DateTime.UtcNow
                };

                projects.Add(project);

                // Update quote status to ACCEPTED
                quote.Status = QuoteStatus.ACCEPTED;
                quote.AcceptedAt = project.CreatedAt;
                
                // Update service request status
                request.Status = RequestStatus.PROVIDER_SELECTED;
                request.SelectedQuoteId = quote.QuoteId;
            }

            await context.Projects.AddRangeAsync(projects);
            await context.SaveChangesAsync();

            // Now seed ratings for completed projects
            var completedProjects = projects.Where(p => p.Status == ProjectStatus.COMPLETED).ToList();
            var ratings = new List<Rating>();

            foreach (var project in completedProjects)
            {
                // 80% of completed projects get ratings
                if (faker.Random.Bool(0.8f))
                {
                    var overallRating = faker.Random.Int(3, 5); // Mostly positive ratings
                    var rating = new Rating
                    {
                        RatingId = Guid.NewGuid(),
                        ProjectId = project.ProjectId,
                        CitizenId = project.CitizenId,
                        ProviderId = project.ProviderId,
                        OverallRating = overallRating,
                        QualityRating = faker.Random.Int(Math.Max(1, overallRating - 1), Math.Min(5, overallRating + 1)),
                        TimelinessRating = faker.Random.Int(Math.Max(1, overallRating - 1), Math.Min(5, overallRating + 1)),
                        ProfessionalismRating = faker.Random.Int(Math.Max(1, overallRating - 1), Math.Min(5, overallRating + 1)),
                        ValueRating = faker.Random.Int(Math.Max(1, overallRating - 1), Math.Min(5, overallRating + 1)),
                        CommunicationRating = faker.Random.Int(Math.Max(1, overallRating - 1), Math.Min(5, overallRating + 1)),
                        ReviewTitle = overallRating >= 4 
                            ? faker.PickRandom("Excellent work!", "Great service", "Highly recommended", "Professional team", "Very satisfied")
                            : faker.PickRandom("Good work", "Satisfactory", "Job completed", "Decent service", "As expected"),
                        ReviewText = overallRating >= 4
                            ? faker.PickRandom(
                                "The team was professional and completed the work on time. Very satisfied with the results.",
                                "Excellent service from start to finish. Would definitely use again.",
                                "High quality work and great communication throughout the project.",
                                "Very professional and efficient. The end result exceeded my expectations.",
                                "Outstanding work. The team was courteous and cleaned up thoroughly after completion.")
                            : faker.PickRandom(
                                "Work was completed as agreed. No major issues.",
                                "The job was done satisfactorily. Some minor delays but overall acceptable.",
                                "Good work quality. Could improve on communication.",
                                "Completed the project. Met basic expectations.",
                                "Decent work. A few rough edges but nothing major."),
                        WouldRecommend = overallRating >= 4,
                        ResponseFromProvider = faker.Random.Bool(0.6f) // 60% get provider responses
                            ? faker.PickRandom(
                                "Thank you for your feedback! It was a pleasure working with you.",
                                "We appreciate your kind words. Thank you for choosing us!",
                                "Thank you for your review. We're glad you're satisfied with our work.",
                                "We're happy to have met your expectations. Thank you!",
                                "Thanks for the positive feedback. We look forward to serving you again.")
                            : null,
                        ResponseAt = faker.Random.Bool(0.6f) && project.ActualCompletionDate.HasValue
                            ? project.ActualCompletionDate.Value.AddDays(faker.Random.Int(1, 7))
                            : null,
                        IsVerified = true, // All are verified since they completed actual projects
                        IsFeatured = overallRating == 5 && faker.Random.Bool(0.3f), // 30% of 5-star ratings are featured
                        HelpfulCount = faker.Random.Int(0, 25),
                        CreatedAt = project.ActualCompletionDate ?? DateTime.UtcNow,
                        UpdatedAt = project.ActualCompletionDate ?? DateTime.UtcNow
                    };
                    
                    // Adjust timestamps
                    if (project.ActualCompletionDate.HasValue)
                    {
                        rating.CreatedAt = project.ActualCompletionDate.Value.AddDays(faker.Random.Int(1, 5));
                        rating.UpdatedAt = rating.CreatedAt;
                    }

                    ratings.Add(rating);
                }
            }

            await context.Ratings.AddRangeAsync(ratings);
            await context.SaveChangesAsync();

            // Update provider profiles with rating statistics
            var providerIds = ratings.Select(r => r.ProviderId).Distinct();
            foreach (var providerId in providerIds)
            {
                var providerRatings = ratings.Where(r => r.ProviderId == providerId).ToList();
                var provider = await context.ProviderProfiles.FindAsync(providerId);
                
                if (provider != null)
                {
                    provider.TotalRatings = providerRatings.Count;
                    provider.AverageRating = (decimal)providerRatings.Average(r => r.OverallRating);
                    provider.TotalProjects = projects.Count(p => p.ProviderId == providerId);
                    provider.CompletionRate = projects.Count(p => p.ProviderId == providerId && p.Status == ProjectStatus.COMPLETED) * 100m 
                        / projects.Count(p => p.ProviderId == providerId);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSystemSettingsAsync(ApplicationDbContext context)
        {
            if (await context.SystemSettings.AnyAsync())
            {
                return;
            }

            var settings = new List<SystemSettings>
            {
                new SystemSettings
                {
                    SettingId = Guid.NewGuid(),
                    SettingKey = "MaxUploadSizeMB",
                    SettingValue = "10",
                    DataType = Models.DataType.NUMBER,
                    Category = "Application",
                    Description = "Maximum file upload size in megabytes",
                    IsPublic = true,
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new SystemSettings
                {
                    SettingId = Guid.NewGuid(),
                    SettingKey = "DefaultLanguage",
                    SettingValue = "en",
                    DataType = Models.DataType.STRING,
                    Category = "Application",
                    Description = "Default application language",
                    IsPublic = true,
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new SystemSettings
                {
                    SettingId = Guid.NewGuid(),
                    SettingKey = "MaintenanceMode",
                    SettingValue = "false",
                    DataType = Models.DataType.BOOLEAN,
                    Category = "Application",
                    Description = "Enable maintenance mode",
                    IsPublic = true,
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new SystemSettings
                {
                    SettingId = Guid.NewGuid(),
                    SettingKey = "CommissionRate",
                    SettingValue = "10",
                    DataType = Models.DataType.NUMBER,
                    Category = "Payment",
                    Description = "Platform commission rate in percentage",
                    IsPublic = false,
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new SystemSettings
                {
                    SettingId = Guid.NewGuid(),
                    SettingKey = "QuoteExpirationDays",
                    SettingValue = "30",
                    DataType = Models.DataType.NUMBER,
                    Category = "Business",
                    Description = "Number of days before a quote expires",
                    IsPublic = true,
                    IsEditable = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.SystemSettings.AddRangeAsync(settings);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAuditLogsAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (await context.AuditLogs.AnyAsync())
            {
                return;
            }

            var users = await userManager.Users.ToListAsync();
            var adminUser = users.FirstOrDefault(u => u.UserName == "admin@bonyankop.com");
            var citizenUser = users.FirstOrDefault(u => u.UserName == "citizen@example.com");
            var engineerUser = users.FirstOrDefault(u => u.UserName == "engineer1@example.com");
            
            var faker = new Faker();
            var auditLogs = new List<AuditLog>();

            if (citizenUser != null)
            {
                auditLogs.Add(new AuditLog
                {
                    LogId = Guid.NewGuid(),
                    UserId = citizenUser.Id,
                    ActionType = "USER_REGISTRATION",
                    EntityType = "User",
                    EntityId = citizenUser.Id,
                    ActionDescription = "New user registered as CITIZEN",
                    NewValuesJson = $"{{\"Email\":\"{citizenUser.Email}\",\"Role\":\"CITIZEN\"}}",
                    IpAddress = faker.Internet.Ip(),
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    RequestUrl = "/api/auth/register",
                    ResponseStatus = 201,
                    ExecutionTimeMs = faker.Random.Int(50, 200),
                    SessionId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                });
            }

            if (engineerUser != null)
            {
                auditLogs.Add(new AuditLog
                {
                    LogId = Guid.NewGuid(),
                    UserId = engineerUser.Id,
                    ActionType = "USER_LOGIN",
                    EntityType = "Auth",
                    EntityId = engineerUser.Id,
                    ActionDescription = "User logged in successfully",
                    IpAddress = faker.Internet.Ip(),
                    UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X)",
                    RequestUrl = "/api/auth/login",
                    ResponseStatus = 200,
                    ExecutionTimeMs = faker.Random.Int(100, 300),
                    SessionId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                });
            }

            var serviceRequest = await context.ServiceRequests.FirstOrDefaultAsync();
            if (serviceRequest != null && citizenUser != null)
            {
                auditLogs.Add(new AuditLog
                {
                    LogId = Guid.NewGuid(),
                    UserId = citizenUser.Id,
                    ActionType = "CREATE",
                    EntityType = "ServiceRequest",
                    EntityId = serviceRequest.RequestId,
                    ActionDescription = "Created new service request",
                    NewValuesJson = $"{{\"ProblemCategory\":\"{serviceRequest.ProblemCategory}\",\"ProblemTitle\":\"{serviceRequest.ProblemTitle}\"}}",
                    IpAddress = faker.Internet.Ip(),
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/91.0",
                    RequestUrl = "/api/servicerequests",
                    ResponseStatus = 201,
                    ExecutionTimeMs = faker.Random.Int(150, 400),
                    SessionId = Guid.NewGuid().ToString(),
                    CreatedAt = serviceRequest.CreatedAt
                });
            }

            var quote = await context.Quotes.FirstOrDefaultAsync();
            if (quote != null && engineerUser != null)
            {
                auditLogs.Add(new AuditLog
                {
                    LogId = Guid.NewGuid(),
                    UserId = engineerUser.Id,
                    ActionType = "CREATE",
                    EntityType = "Quote",
                    EntityId = quote.QuoteId,
                    ActionDescription = "Submitted quote for service request",
                    NewValuesJson = $"{{\"EstimatedCost\":{quote.EstimatedCost},\"EstimatedDurationDays\":{quote.EstimatedDurationDays}}}",
                    IpAddress = faker.Internet.Ip(),
                    UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/14.1",
                    RequestUrl = "/api/quotes",
                    ResponseStatus = 201,
                    ExecutionTimeMs = faker.Random.Int(200, 500),
                    SessionId = Guid.NewGuid().ToString(),
                    CreatedAt = quote.SubmittedAt
                });
            }

            if (adminUser != null)
            {
                auditLogs.Add(new AuditLog
                {
                    LogId = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    ActionType = "UPDATE",
                    EntityType = "SystemSettings",
                    EntityId = Guid.NewGuid(),
                    ActionDescription = "Updated system setting",
                    OldValuesJson = "{\"SettingKey\":\"MaxUploadSizeMB\",\"SettingValue\":\"5\"}",
                    NewValuesJson = "{\"SettingKey\":\"MaxUploadSizeMB\",\"SettingValue\":\"10\"}",
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Edge/91.0",
                    RequestUrl = "/api/systemsettings/MaxUploadSizeMB",
                    ResponseStatus = 200,
                    ExecutionTimeMs = faker.Random.Int(50, 150),
                    SessionId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                });
            }

            await context.AuditLogs.AddRangeAsync(auditLogs);
            await context.SaveChangesAsync();
        }
    }
}
