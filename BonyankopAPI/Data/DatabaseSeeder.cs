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
    }
}
