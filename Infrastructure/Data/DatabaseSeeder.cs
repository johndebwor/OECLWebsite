using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OECLWebsite.Data;
using OECLWebsite.Data.Entities;
using OECLWebsite.Infrastructure.Identity;

namespace OECLWebsite.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            await context.Database.MigrateAsync();
            await SeedRolesAsync(roleManager);
            await MigrateAdminEmailAsync(userManager);
            await SeedAdminUserAsync(userManager);
            await SeedServiceCategoriesAsync(context);
            await SeedServicesAsync(context);
            await SeedProjectsAsync(context);
            await SeedContentPagesAsync(context);
            await SeedSitePhotosAsync(context);
            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new List<ApplicationRole>
        {
            new() { Name = "CEO", Description = "Chief Executive Officer - Full access", PermissionLevel = 1, CanApprove = true, CanPublish = true },
            new() { Name = "GeneralManager", Description = "General Manager - Manage departments, approve major changes", PermissionLevel = 2, CanApprove = true, CanPublish = true },
            new() { Name = "DepartmentHead", Description = "Department Head - Manage department content and users", PermissionLevel = 3, CanApprove = true, CanPublish = true },
            new() { Name = "SeniorOfficer", Description = "Senior Officer - Create and edit content", PermissionLevel = 4, CanApprove = false, CanPublish = false },
            new() { Name = "ContentEditor", Description = "Content Editor - Create and edit assigned content", PermissionLevel = 5, CanApprove = false, CanPublish = false },
            new() { Name = "Viewer", Description = "Viewer - Read-only access to admin area", PermissionLevel = 6, CanApprove = false, CanPublish = false },
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task MigrateAdminEmailAsync(UserManager<ApplicationUser> userManager)
    {
        const string oldEmail = "admin@ovalengineering.com";
        var user = await userManager.FindByEmailAsync(oldEmail);
        if (user is null) return;

        const string newEmail = "admin@oecl-ss.com";
        user.Email = newEmail;
        user.UserName = newEmail;
        user.NormalizedEmail = newEmail.ToUpperInvariant();
        user.NormalizedUserName = newEmail.ToUpperInvariant();
        await userManager.UpdateAsync(user);
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@oecl-ss.com";

        if (await userManager.FindByEmailAsync(adminEmail) is not null) return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator",
            EmployeeId = "OE-001",
            Department = "Executive",
            Position = "CEO",
            IsActive = true,
        };

        var result = await userManager.CreateAsync(admin, "Admin@123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "CEO");
        }
    }

    private static async Task SeedServiceCategoriesAsync(AppDbContext context)
    {
        if (await context.ServiceCategories.AnyAsync()) return;

        var categories = new List<ServiceCategory>
        {
            new() { Name = "Engineering & Design (FEED)", Description = "Front-End Engineering Design services including feasibility studies, conceptual design, and detailed engineering", IconName = "Engineering", DisplayOrder = 1, IsActive = true },
            new() { Name = "Construction & Installation (EPCC)", Description = "Engineering, Procurement, Construction, and Commissioning services for oil and gas facilities", IconName = "Construction", DisplayOrder = 2, IsActive = true },
            new() { Name = "Operations & Maintenance (O&M)", Description = "Facility operations, preventive maintenance, and asset management services", IconName = "Build", DisplayOrder = 3, IsActive = true },
            new() { Name = "Project Management", Description = "Comprehensive project management, planning, scheduling, and cost control services", IconName = "Assignment", DisplayOrder = 4, IsActive = true },
            new() { Name = "Technical Consulting", Description = "Expert advisory services for oil and gas operations, safety, and regulatory compliance", IconName = "Support", DisplayOrder = 5, IsActive = true },
        };

        context.ServiceCategories.AddRange(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedServicesAsync(AppDbContext context)
    {
        if (await context.Services.AnyAsync()) return;

        var categories = await context.ServiceCategories.ToListAsync();
        if (!categories.Any()) return;

        var services = new List<Service>
        {
            // Engineering & Design
            new() { CategoryId = categories.First(c => c.Name.Contains("Engineering")).Id, Title = "Feasibility Studies", ShortDescription = "Comprehensive feasibility assessments for oil and gas projects, including economic evaluation, risk analysis, and technical viability.", DisplayOrder = 1, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Engineering")).Id, Title = "Conceptual Design", ShortDescription = "Early-stage engineering design to define project scope, layout options, and cost estimates for informed decision-making.", DisplayOrder = 2, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Engineering")).Id, Title = "Detailed Engineering", ShortDescription = "Complete engineering deliverables including P&IDs, equipment specifications, structural design, and construction drawings.", DisplayOrder = 3, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },

            // Construction & Installation
            new() { CategoryId = categories.First(c => c.Name.Contains("Construction")).Id, Title = "Pipeline Construction", ShortDescription = "End-to-end pipeline construction services including route surveys, welding, hydrotesting, and commissioning.", DisplayOrder = 1, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Construction")).Id, Title = "Facility Construction", ShortDescription = "Construction of processing facilities, tank farms, pump stations, and associated infrastructure.", DisplayOrder = 2, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Construction")).Id, Title = "Commissioning & Startup", ShortDescription = "Systematic commissioning, pre-commissioning, and startup support to ensure safe and efficient facility handover.", DisplayOrder = 3, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },

            // Operations & Maintenance
            new() { CategoryId = categories.First(c => c.Name.Contains("Operations")).Id, Title = "Preventive Maintenance", ShortDescription = "Scheduled maintenance programs to maximize equipment reliability, minimize downtime, and extend asset life.", DisplayOrder = 1, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Operations")).Id, Title = "Asset Integrity Management", ShortDescription = "Inspection, corrosion monitoring, and integrity assessments to ensure safe and compliant facility operations.", DisplayOrder = 2, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Operations")).Id, Title = "Turnaround Services", ShortDescription = "Planning and execution of facility shutdowns for major maintenance, upgrades, and statutory inspections.", DisplayOrder = 3, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },

            // Project Management
            new() { CategoryId = categories.First(c => c.Name.Contains("Project Management")).Id, Title = "Project Planning & Scheduling", ShortDescription = "Detailed project planning, scheduling, and progress monitoring using industry-standard tools and methodologies.", DisplayOrder = 1, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Project Management")).Id, Title = "Cost Control & Estimation", ShortDescription = "Accurate cost estimation, budgeting, and cost control throughout the project lifecycle.", DisplayOrder = 2, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },

            // Technical Consulting
            new() { CategoryId = categories.First(c => c.Name.Contains("Consulting")).Id, Title = "HSE Consulting", ShortDescription = "Health, Safety, and Environment advisory services including risk assessments, audits, and compliance programs.", DisplayOrder = 1, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
            new() { CategoryId = categories.First(c => c.Name.Contains("Consulting")).Id, Title = "Regulatory Compliance", ShortDescription = "Expert guidance on local and international regulatory requirements for oil and gas operations.", DisplayOrder = 2, IsActive = true, ApprovalStatus = ApprovalStatus.Approved },
        };

        context.Services.AddRange(services);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProjectsAsync(AppDbContext context)
    {
        if (await context.Projects.AnyAsync()) return;

        var projects = new List<Project>
        {
            new()
            {
                ProjectName = "Paloch Oil Processing Facility Upgrade",
                Client = "Greater Pioneer Operating Company",
                Location = "Upper Nile State, South Sudan",
                ProjectType = "EPCC",
                StartDate = new DateOnly(2023, 3, 1),
                CompletionDate = new DateOnly(2024, 8, 15),
                Description = "Complete upgrade of crude oil processing facilities including new separation units, storage tanks, and associated piping. The project involved detailed engineering, procurement of critical equipment, and construction in a remote location.",
                Status = ProjectStatus.Completed,
                IsShowcase = true,
                DisplayOrder = 1,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                ProjectName = "Juba Industrial Park Infrastructure",
                Client = "Ministry of Petroleum",
                Location = "Juba, South Sudan",
                ProjectType = "FEED",
                StartDate = new DateOnly(2024, 1, 15),
                CompletionDate = new DateOnly(2024, 6, 30),
                Description = "Front-End Engineering Design for a new industrial park infrastructure including road networks, utilities, drainage systems, and facility layouts to support downstream petroleum operations.",
                Status = ProjectStatus.Completed,
                IsShowcase = true,
                DisplayOrder = 2,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                ProjectName = "Unity Oilfield Pipeline Network",
                Client = "Dar Petroleum Operating Company",
                Location = "Unity State, South Sudan",
                ProjectType = "EPCC",
                StartDate = new DateOnly(2024, 6, 1),
                Description = "Engineering, procurement, and construction of a 45km flowline network connecting satellite wells to the central processing facility, including pig launchers/receivers and metering stations.",
                Status = ProjectStatus.Ongoing,
                IsShowcase = true,
                DisplayOrder = 3,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                ProjectName = "Adar Yale Gas Compression Station",
                Client = "Nile Petroleum Corporation",
                Location = "Upper Nile State, South Sudan",
                ProjectType = "EPCC",
                StartDate = new DateOnly(2024, 9, 1),
                Description = "Design and construction of a gas compression station to reduce gas flaring and improve production efficiency. Includes compressor packages, coolers, and control systems.",
                Status = ProjectStatus.Ongoing,
                IsShowcase = false,
                DisplayOrder = 4,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                ProjectName = "Tank Farm Maintenance Program",
                Client = "Sudd Petroleum Operating Company",
                Location = "Upper Nile State, South Sudan",
                ProjectType = "O&M",
                StartDate = new DateOnly(2023, 1, 1),
                Description = "Comprehensive annual maintenance program for a 12-tank crude oil storage farm including API 653 inspections, cathodic protection surveys, and preventive maintenance activities.",
                Status = ProjectStatus.Completed,
                IsShowcase = false,
                DisplayOrder = 5,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                ProjectName = "Tharjath Field Development Plan",
                Client = "Sudd Petroleum Operating Company",
                Location = "Unity State, South Sudan",
                ProjectType = "FEED",
                StartDate = new DateOnly(2025, 1, 15),
                Description = "Feasibility study and conceptual design for field development including well pad layouts, gathering system design, and facility sizing for a new production block.",
                Status = ProjectStatus.Planned,
                IsShowcase = false,
                DisplayOrder = 6,
                ApprovalStatus = ApprovalStatus.Approved
            },
        };

        context.Projects.AddRange(projects);
        await context.SaveChangesAsync();
    }

    private static async Task SeedContentPagesAsync(AppDbContext context)
    {
        if (await context.ContentPages.AnyAsync()) return;

        var pages = new List<ContentPage>
        {
            new() { PageKey = "about", PageTitle = "About Oval Engineering", ApprovalStatus = ApprovalStatus.Approved },
            new() { PageKey = "vision", PageTitle = "Our Vision", ApprovalStatus = ApprovalStatus.Approved },
            new() { PageKey = "mission", PageTitle = "Our Mission", ApprovalStatus = ApprovalStatus.Approved },
            new() { PageKey = "values", PageTitle = "Our Values", ApprovalStatus = ApprovalStatus.Approved },
        };

        context.ContentPages.AddRange(pages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSitePhotosAsync(AppDbContext context)
    {
        if (await context.SitePhotos.AnyAsync()) return;

        var photos = new List<SitePhoto>
        {
            new() { PhotoKey = "company-logo", Title = "Company Logo", DisplayOrder = 0, IsActive = true },
            new() { PhotoKey = "hero-home", Title = "Home Page Hero Background", DisplayOrder = 1, IsActive = true },
            new() { PhotoKey = "hero-about", Title = "About Page Hero Background", DisplayOrder = 2, IsActive = true },
            new() { PhotoKey = "hero-services", Title = "Services Page Hero Background", DisplayOrder = 3, IsActive = true },
            new() { PhotoKey = "hero-projects", Title = "Projects Page Hero Background", DisplayOrder = 4, IsActive = true },
            new() { PhotoKey = "hero-contact", Title = "Contact Page Hero Background", DisplayOrder = 5, IsActive = true },
            new() { PhotoKey = "about-team", Title = "About Page Team Photo", DisplayOrder = 6, IsActive = true },
            new() { PhotoKey = "cta-background", Title = "Call to Action Background", DisplayOrder = 7, IsActive = true },
        };

        context.SitePhotos.AddRange(photos);
        await context.SaveChangesAsync();
    }
}
