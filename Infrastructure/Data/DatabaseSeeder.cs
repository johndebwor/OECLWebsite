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
            await SeedSystemSettingsAsync(context);
            await SeedProjectTypesSettingAsync(context);
            await SeedEquipmentCategoriesAsync(context);
            await SeedEquipmentAsync(context);
            await SeedEquipmentHeroPhotoAsync(context);
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

    private static async Task SeedSystemSettingsAsync(AppDbContext context)
    {
        if (await context.SystemSettings.AnyAsync()) return;

        context.SystemSettings.AddRange(
            new SystemSetting { Key = "Projects:Types", Value = "FEED,EPCC,O&M,Consulting,Project Management", Description = "Comma-separated list of project type options", Category = "General" },
            new SystemSetting { Key = "Email:To", Value = "info@oecl-ss.com", Description = "Primary recipient for contact form emails", Category = "Email" },
            new SystemSetting { Key = "Email:Cc", Value = "", Description = "CC address for contact form emails (optional)", Category = "Email" },
            new SystemSetting { Key = "Email:SmtpHost", Value = "", Description = "SMTP server hostname (e.g. smtp.gmail.com)", Category = "Email" },
            new SystemSetting { Key = "Email:SmtpPort", Value = "587", Description = "SMTP port — 587 for STARTTLS, 465 for SSL", Category = "Email" },
            new SystemSetting { Key = "Email:SmtpUsername", Value = "", Description = "SMTP login username / email", Category = "Email" },
            new SystemSetting { Key = "Email:SmtpPassword", Value = "", Description = "SMTP login password", Category = "Email", IsSecret = true },
            new SystemSetting { Key = "Email:FromAddress", Value = "noreply@oecl-ss.com", Description = "Sender email address shown to recipients", Category = "Email" },
            new SystemSetting { Key = "Email:FromName", Value = "Oval Engineering Website", Description = "Sender display name shown to recipients", Category = "Email" },
            new SystemSetting { Key = "Email:EnableSsl", Value = "true", Description = "Use STARTTLS/SSL for SMTP connection (true/false)", Category = "Email" }
        );
        await context.SaveChangesAsync();
    }

    private static async Task SeedProjectTypesSettingAsync(AppDbContext context)
    {
        if (await context.SystemSettings.AnyAsync(s => s.Key == "Projects:Types")) return;
        context.SystemSettings.Add(new SystemSetting
        {
            Key = "Projects:Types",
            Value = "FEED,EPCC,O&M,Consulting,Project Management",
            Description = "Comma-separated list of project type options",
            Category = "General"
        });
        await context.SaveChangesAsync();
    }

    private static async Task SeedEquipmentCategoriesAsync(AppDbContext context)
    {
        var existing = await context.EquipmentCategories.ToListAsync();

        var seedCategories = new List<(string Name, string Description, string Icon, int Order)>
        {
            ("Well Services Equipment", "Pumping, mixing, filtration, and storage equipment for well workover, completion, and stimulation operations", "Plumbing", 1),
            ("Well Testing Equipment", "Separators, well control equipment, and surface testing units for data acquisition and flow management", "Science", 2),
            ("Drilling Tools", "Coiled tubing, downhole tools, and surface drilling equipment for well intervention and drilling operations", "Hardware", 3),
            ("Wireline & Rigless Units", "Rigless electric wireline (REW) units for well logging, perforation, plug setting, and diagnostics operations", "CableAlt", 4),
            ("Slickline Units", "Mechanical well intervention units for gauge runs, plug setting/pulling, fishing, and valve operations", "LinearScale", 5),
        };

        foreach (var (name, desc, icon, order) in seedCategories)
        {
            if (!existing.Any(c => c.Name == name))
            {
                context.EquipmentCategories.Add(new EquipmentCategory
                {
                    Name = name, Description = desc, IconName = icon, DisplayOrder = order, IsActive = true
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedEquipmentAsync(AppDbContext context)
    {
        var categories = await context.EquipmentCategories.ToListAsync();
        if (!categories.Any()) return;

        var wellServicesId = categories.First(c => c.Name.Contains("Well Services")).Id;
        var wellTestingId = categories.First(c => c.Name.Contains("Well Testing")).Id;
        var drillingToolsId = categories.First(c => c.Name.Contains("Drilling Tools")).Id;
        var wirelineId = categories.First(c => c.Name.Contains("Wireline")).Id;
        var slicklineId = categories.First(c => c.Name.Contains("Slickline")).Id;

        var existingEquipment = await context.Equipment.ToListAsync();

        var allEquipment = new List<Equipment>
        {
            // ── Well Services Equipment (13 items) ──────────────────────────
            new()
            {
                CategoryId = wellServicesId, Title = "Triplex Twin Pump",
                ShortDescription = "High-pressure dual-engine pumping system for well service treatments including cementing, acidizing, and fracturing operations.",
                DetailedDescription = "The Triplex Twin Pump is a compact, lightweight, high-efficiency pumping unit comprising two diesel engines, two power shift transmissions, and two triplex pumps. Designed for continuous well service operations, it delivers reliable high-pressure pumping for cementing, acidizing, water injection, and fracturing treatments. The dual-engine configuration provides redundancy and maximum uptime in demanding oilfield environments.",
                TechnicalSpecs = "Engines: 2× Caterpillar C15 (595 HP each) | Pumps: SPM TWS 600S Triplex | Transmissions: Allison 4700 OFS | Piping: 15,000 PSI rated | Configuration: Skid-mounted, dual-engine",
                ImageUrl = "/images/equipment/triplex-twin-pump.jpg",
                CatalogReference = "Pages 1-2", DisplayOrder = 1, IsActive = true, IsFeatured = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Single Triplex Pump (HT 400)",
                ShortDescription = "Individual high-pressure triplex pump with hard surface plungers for cement, acid, water, and industrial fluids.",
                DetailedDescription = "The HT 400 Single Triplex Pump is a versatile high-pressure pumping unit featuring hard surface plungers for extended service life. Suitable for a wide range of well service applications including cementing, acidizing, water injection, and other industrial fluid pumping operations. Engineered for reliability in harsh oilfield conditions.",
                TechnicalSpecs = "Type: Triplex positive displacement | Plungers: Hard surface coated | Fluids: Cement, acid, water, industrial | Pressure: High-pressure rated",
                CatalogReference = "Pages 3-4", DisplayOrder = 2, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "C-Pump",
                ShortDescription = "Specialized centrifugal pump for high-volume fluid transfer in oilfield operations.",
                DetailedDescription = "The C-Pump is a heavy-duty centrifugal pump designed for high-volume fluid transfer applications in the oil and gas industry. Ideal for moving large quantities of water, brine, and other fluids during well service operations, tank transfers, and facility support tasks.",
                TechnicalSpecs = "Type: Centrifugal | Application: High-volume fluid transfer | Configuration: Skid-mounted",
                ImageUrl = "/images/equipment/c-pump.jpg",
                CatalogReference = "Page 5", DisplayOrder = 3, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Nitrogen Pump 180K",
                ShortDescription = "High-capacity 180,000 SCF nitrogen pumping unit for well stimulation, pipeline purging, and pressure testing.",
                DetailedDescription = "The Nitrogen Pump 180K is a high-capacity nitrogen pumping unit delivering up to 180,000 SCF for well stimulation, pipeline purging, pressure testing, and inert gas blanket applications. Built on a DNV 2.7-1 certified frame with Zone II hazardous area classification, it meets the highest safety and performance standards for offshore and onshore operations.",
                TechnicalSpecs = "Power: 600 HP | Flow Rate: 6,000 scfm | Pressure: 15,000 PSI | Frame: DNV 2.7-1 certified | Zone: Zone II hazardous area rated | Capacity: 180,000 SCF",
                ImageUrl = "/images/equipment/nitrogen-pump-180k.png",
                CatalogReference = "Page 6", DisplayOrder = 4, IsActive = true, IsFeatured = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Nitrogen Tanks Services",
                ShortDescription = "Cryogenic nitrogen storage and delivery tanks for continuous supply during well services and pipeline operations.",
                DetailedDescription = "Cryogenic nitrogen storage tanks providing continuous nitrogen supply during well service operations, pipeline purging, and pressure testing. Designed for safe storage and transport of liquid nitrogen with controlled vaporization for sustained delivery to pumping units.",
                TechnicalSpecs = "Type: Cryogenic storage | Medium: Liquid nitrogen | Delivery: Continuous vaporized supply | Transport: Road-transportable",
                ImageUrl = "/images/equipment/nitrogen-tanks.png",
                CatalogReference = "Page 7", DisplayOrder = 5, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Acid & Cement Batch Mixer (Skid)",
                ShortDescription = "Skid-mounted batch mixing unit for preparing acid and cement slurries with multiple tank capacity options.",
                DetailedDescription = "The Acid & Cement Batch Mixer is a skid-mounted mixing unit engineered for preparing acid and cement slurries for well treatment and cementing operations. Available in multiple tank capacity configurations, it provides precise mixing control for consistent slurry quality in demanding field conditions.",
                TechnicalSpecs = "Engine: Caterpillar C9 | Capacities: 100 / 120 / 150 / 200 BBL | Mounting: Skid | Application: Acid and cement slurry preparation",
                CatalogReference = "Page 8", DisplayOrder = 6, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Batch Mixer Mounting Trailer",
                ShortDescription = "Mobile trailer-mounted batch mixing platform for on-site preparation of treatment fluids.",
                DetailedDescription = "A mobile trailer-mounted batch mixing platform designed for rapid deployment and on-site preparation of treatment fluids. The trailer configuration enables easy transport between well locations while providing full mixing capability for acid, cement, and other oilfield fluids.",
                TechnicalSpecs = "Mounting: Trailer-mounted | Mobility: Road-transportable | Application: Field-deployable batch mixing",
                ImageUrl = "/images/equipment/batch-mixer-trailer.png",
                CatalogReference = "Page 9", DisplayOrder = 7, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Skid Mounted Filtration Equipment",
                ShortDescription = "Industrial fluid filtration system on a transportable skid for removing solids and contaminants from oilfield fluids.",
                DetailedDescription = "Industrial-grade fluid filtration system mounted on a transportable skid for removing solids, particulates, and contaminants from oilfield fluids. Essential for maintaining fluid quality during well operations, protecting downstream equipment, and ensuring treatment fluid integrity.",
                TechnicalSpecs = "Mounting: Transportable skid | Function: Solids/contaminant removal | Application: Oilfield fluid filtration",
                ImageUrl = "/images/equipment/filtration-equipment.png",
                CatalogReference = "Page 10", DisplayOrder = 8, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "500 BBL Crude Oil/Water Storage Tanks",
                ShortDescription = "Engineered 500-barrel storage tanks for crude oil and water containment with leak-proof construction.",
                DetailedDescription = "Engineered 500-barrel capacity storage tanks for crude oil and water containment, built to rigorous quality standards with leak-proof construction. Available with customizable specifications to meet project-specific requirements for fluid storage during production, testing, and workover operations.",
                TechnicalSpecs = "Capacity: 500 BBL | Construction: Leak-proof, engineered | Fluids: Crude oil, water | Specs: Customizable",
                ImageUrl = "/images/equipment/storage-tanks-500bbl.jpg",
                CatalogReference = "Page 15", DisplayOrder = 9, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Frac Tank",
                ShortDescription = "Fracturing fluid storage tank for temporary on-site storage during well stimulation operations.",
                DetailedDescription = "Purpose-built fracturing fluid storage tank designed for temporary on-site storage of treatment fluids during well stimulation operations. Robust construction for reliable containment of frac fluids, water, and other treatment chemicals.",
                TechnicalSpecs = "Application: Fracturing fluid storage | Type: Temporary on-site containment | Construction: Heavy-duty oilfield grade",
                ImageUrl = "/images/equipment/frac-tank.jpg",
                CatalogReference = "Page 16", DisplayOrder = 10, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Acid Tank Mounted Trailer",
                ShortDescription = "Mobile acid containment tank on trailer chassis for safe transportation and on-site delivery.",
                DetailedDescription = "Mobile acid containment tank mounted on a trailer chassis for safe transportation and on-site delivery of acid treatment fluids. Acid-resistant construction with safety features for handling corrosive chemicals during well stimulation and treatment operations.",
                TechnicalSpecs = "Mounting: Trailer chassis | Material: Acid-resistant | Application: Acid transport and on-site delivery",
                ImageUrl = "/images/equipment/acid-tank-trailer.png",
                CatalogReference = "Page 18", DisplayOrder = 11, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Safety Caravans",
                ShortDescription = "Mobile safety units providing on-site safety facilities, emergency response stations, and crew welfare amenities.",
                DetailedDescription = "Mobile safety units providing comprehensive on-site safety facilities including emergency response stations, first aid areas, safety briefing rooms, and crew welfare amenities. Essential for HSE compliance at remote well sites and construction locations.",
                TechnicalSpecs = "Type: Mobile safety unit | Features: Emergency response, first aid, briefing room | Mobility: Road-transportable",
                ImageUrl = "/images/equipment/safety-caravans.jpg",
                CatalogReference = "Page 18", DisplayOrder = 12, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellServicesId, Title = "Water Tank (Meters Tank)",
                ShortDescription = "Water storage and metering tank for well service operations, fluid management, and volume tracking.",
                DetailedDescription = "Water storage and metering tank designed for well service operations, providing accurate volume tracking and fluid management during cementing, stimulation, and completion activities. Equipped with metering capabilities for precise fluid measurement and inventory control.",
                TechnicalSpecs = "Type: Storage and metering | Medium: Water | Application: Well service fluid management | Feature: Integrated metering",
                CatalogReference = "Page 17", DisplayOrder = 13, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },

            // ── Well Testing Equipment (6 items) ────────────────────────────
            new()
            {
                CategoryId = wellTestingId, Title = "Silo Tank",
                ShortDescription = "Bulk material storage silo for cement, barite, and dry materials used in well operations.",
                DetailedDescription = "Bulk material storage silo engineered for cement, barite, and other dry materials used in well operations. Available in multiple capacity configurations, constructed from A36 steel for durability and corrosion resistance in harsh oilfield environments.",
                TechnicalSpecs = "Capacities: 1,050 / 3,000 cu ft | Material: A36 Steel | Contents: Cement, barite, dry bulk materials",
                ImageUrl = "/images/equipment/silo-tank.png",
                CatalogReference = "Page 11", DisplayOrder = 1, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellTestingId, Title = "Surge Tank",
                ShortDescription = "Pressure surge containment vessel for absorbing pressure fluctuations and protecting downstream equipment.",
                DetailedDescription = "Pressure surge containment vessel designed to absorb pressure fluctuations and protect downstream equipment during well testing and production operations. Acts as a buffer between the wellhead and processing equipment to ensure safe, controlled flow.",
                TechnicalSpecs = "Function: Pressure surge absorption | Protection: Downstream equipment | Application: Well testing, production",
                ImageUrl = "/images/equipment/surge-tank.png",
                CatalogReference = "Page 12", DisplayOrder = 2, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellTestingId, Title = "1440 Separator",
                ShortDescription = "Three-phase oil/gas/water separation unit with 1440 PSI working pressure for surface well testing.",
                DetailedDescription = "Three-phase (oil/gas/water) separation unit rated at 1440 PSI working pressure for surface well testing and production operations. Provides efficient separation of produced fluids for flow measurement, sampling, and safe disposal during well testing programs.",
                TechnicalSpecs = "Type: Three-phase separator | Working Pressure: 1,440 PSI | Separation: Oil / Gas / Water | Application: Surface well testing, early production",
                ImageUrl = "/images/equipment/separator-1440.jpg",
                CatalogReference = "Page 13", DisplayOrder = 3, IsActive = true, IsFeatured = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellTestingId, Title = "2160 Separator",
                ShortDescription = "Large-capacity three-phase separator rated at 2160 PSI for high-pressure well testing.",
                DetailedDescription = "Large-capacity three-phase separator rated at 2160 PSI for high-pressure well testing and early production facilities. Designed to handle high-pressure, high-temperature wells with efficient oil/gas/water separation and accurate flow measurement.",
                TechnicalSpecs = "Type: Three-phase separator | Working Pressure: 2,160 PSI | Separation: Oil / Gas / Water | Application: High-pressure well testing",
                CatalogReference = "Page 14", DisplayOrder = 4, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellTestingId, Title = "BOP 4-1/16\"",
                ShortDescription = "4-1/16 inch Blowout Preventer for well control during testing, workover, and completion operations.",
                DetailedDescription = "4-1/16 inch bore Blowout Preventer (BOP) providing critical well control during testing, workover, and completion operations. Essential safety equipment for pressure containment and well barrier management, designed to API standards for reliable shut-in capability.",
                TechnicalSpecs = "Bore: 4-1/16 inch | Type: Blowout Preventer | Standard: API compliant | Application: Well control — testing, workover, completion",
                CatalogReference = "Page 17", DisplayOrder = 5, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = wellTestingId, Title = "Choke Manifold",
                ShortDescription = "Flow and pressure control manifold for well testing and well control operations.",
                DetailedDescription = "Flow and pressure control manifold used during well testing and well control operations to regulate wellhead pressure safely. Features adjustable and fixed chokes for precise pressure management, critical for safe well testing and kick control procedures.",
                TechnicalSpecs = "Dimensions: 3.38 × 1.54 × 0.97 m | Weight: 2,100 lbs | Pressure Rating: 10,000 PSI | Chokes: Adjustable and fixed",
                ImageUrl = "/images/equipment/choke-manifold.png",
                CatalogReference = "Page 17", DisplayOrder = 6, IsActive = true,
                ApprovalStatus = ApprovalStatus.Approved
            },

            // ── Drilling Tools (7 items) ────────────────────────────────────
            new()
            {
                CategoryId = drillingToolsId, Title = "Coiled Tubing Trailer Mounted",
                ShortDescription = "Complete coiled tubing unit with power pack, control cabin, CT reel, injector head, and BOP.",
                DetailedDescription = "Complete coiled tubing unit package mounted on a trailer for rapid deployment. Includes open-loop design power pack, control cabin, CT reel, injector head, BOP, stuffing box, and power hose reel. Suitable for well intervention, cleanouts, nitrogen lifts, and stimulation operations.",
                TechnicalSpecs = "Components: Power pack (open loop), control cabin, CT reel, injector head, BOP, stuffing box, power hose reel | Mounting: Trailer | Application: Well intervention, cleanout, nitrogen lift",
                ImageUrl = "/images/equipment/coiled-tubing.png",
                CatalogReference = "Page 19", DisplayOrder = 1, IsActive = true, IsFeatured = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = drillingToolsId, Title = "Stabilizers",
                ShortDescription = "Downhole stabilization tools for directional drilling and wellbore trajectory control.",
                DetailedDescription = "Downhole stabilization tools for directional drilling operations, providing wellbore trajectory control and reducing vibration. Available in various sizes and blade configurations for different formation types and drilling requirements.",
                TechnicalSpecs = "Type: Downhole stabilizer | Application: Directional drilling, vibration control | Availability: Various sizes and blade configurations",
                CatalogReference = "Page 20", DisplayOrder = 2, IsActive = true,
                AvailabilityNote = "Available for rental and sale",
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = drillingToolsId, Title = "Near Bit Reamers",
                ShortDescription = "Near-bit reaming tools for wellbore enlargement and hole conditioning.",
                DetailedDescription = "Near-bit reaming tools positioned close to the drill bit for wellbore enlargement, hole conditioning, and improved hole quality. Helps maintain gauge hole and reduces the risk of stuck pipe by keeping the wellbore clean and properly sized.",
                TechnicalSpecs = "Type: Near-bit reamer | Function: Wellbore enlargement, hole conditioning | Position: Near drill bit",
                CatalogReference = "Page 20", DisplayOrder = 3, IsActive = true,
                AvailabilityNote = "Available for rental and sale",
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = drillingToolsId, Title = "Drill Collars",
                ShortDescription = "Heavy-weight tubulars providing weight-on-bit for efficient drilling operations.",
                DetailedDescription = "Heavy-weight drill collars providing the necessary weight-on-bit (WOB) for efficient drilling operations. Manufactured from high-strength steel with precision threading for reliable connections in demanding downhole conditions.",
                TechnicalSpecs = "Type: Heavy-weight tubular | Function: Weight-on-bit | Material: High-strength steel | Threading: Precision machined",
                CatalogReference = "Page 20", DisplayOrder = 4, IsActive = true,
                AvailabilityNote = "Available for rental and sale",
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = drillingToolsId, Title = "Drilling Jars",
                ShortDescription = "Mechanical and hydraulic jars for stuck pipe release and jarring operations.",
                DetailedDescription = "Mechanical and hydraulic drilling jars providing impact forces for stuck pipe release and freeing operations. Essential downhole tools for mitigating non-productive time caused by differential sticking, key seating, and other stuck pipe scenarios.",
                TechnicalSpecs = "Types: Mechanical and hydraulic | Function: Stuck pipe release, jarring | Application: Stuck pipe mitigation",
                CatalogReference = "Page 20", DisplayOrder = 5, IsActive = true,
                AvailabilityNote = "Available for rental and sale",
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = drillingToolsId, Title = "Crossover Subs",
                ShortDescription = "Connection adapters between different thread types and tubular sizes.",
                DetailedDescription = "Crossover subs providing connection adapters between different thread types, sizes, and tubular configurations. Essential for assembling drill strings with mixed components and ensuring compatible connections throughout the bottom hole assembly.",
                TechnicalSpecs = "Type: Connection adapter | Function: Thread/size adaptation | Application: Drill string assembly",
                CatalogReference = "Page 20", DisplayOrder = 6, IsActive = true,
                AvailabilityNote = "Available for rental and sale",
                ApprovalStatus = ApprovalStatus.Approved
            },
            new()
            {
                CategoryId = drillingToolsId, Title = "Shock Tools",
                ShortDescription = "Vibration dampening tools protecting drill string components and surface equipment.",
                DetailedDescription = "Shock tools providing vibration dampening and impact absorption to protect drill string components, MWD/LWD tools, and surface equipment from harmful drilling vibrations. Extends equipment life and improves drilling performance in hard formations.",
                TechnicalSpecs = "Type: Vibration dampener | Function: Shock absorption, equipment protection | Application: Hard formation drilling",
                CatalogReference = "Page 20", DisplayOrder = 7, IsActive = true,
                AvailabilityNote = "Available for rental and sale",
                ApprovalStatus = ApprovalStatus.Approved
            },

            // ── Wireline & Rigless Units (1 item) ──────────────────────────
            new()
            {
                CategoryId = wirelineId, Title = "REW Unit (Rigless Electric Wireline)",
                ShortDescription = "Rigless electric wireline unit for well logging, perforation, plug setting, and CBL/VDL diagnostics.",
                DetailedDescription = "Rigless Electric Wireline (REW) unit providing comprehensive wireline services without the need for a drilling rig. Capable of well logging, perforation, plug setting and retrieval, CBL/VDL cement bond diagnostics, and other downhole interventions. Self-contained and trailer-mounted for rapid mobilization.",
                TechnicalSpecs = "Type: Electric wireline unit | Configuration: Rigless, self-contained | Services: Logging, perforation, plug setting, CBL/VDL diagnostics | Mounting: Trailer-mounted",
                CatalogReference = "Page 21", DisplayOrder = 1, IsActive = true, IsFeatured = true,
                ApprovalStatus = ApprovalStatus.Approved
            },

            // ── Slickline Units (1 item) ────────────────────────────────────
            new()
            {
                CategoryId = slicklineId, Title = "Slickline Unit",
                ShortDescription = "Mechanical well intervention unit for gauge runs, plug setting/pulling, fishing, and valve operations.",
                DetailedDescription = "Slickline unit for mechanical well intervention operations including gauge runs, plug setting and pulling, fishing, valve shifting, and other downhole mechanical operations. Provides cost-effective well intervention without the need for workover rigs or electric wireline units.",
                TechnicalSpecs = "Type: Mechanical wireline (slickline) | Services: Gauge runs, plug setting/pulling, fishing, valve operations | Advantage: Cost-effective well intervention",
                CatalogReference = "Page 22", DisplayOrder = 1, IsActive = true, IsFeatured = true,
                ApprovalStatus = ApprovalStatus.Approved
            },
        };

        // Add only equipment items that don't already exist (match by Title)
        var toAdd = allEquipment.Where(e => !existingEquipment.Any(ex => ex.Title == e.Title)).ToList();

        // Update existing items with enriched data
        foreach (var existing in existingEquipment)
        {
            var enriched = allEquipment.FirstOrDefault(e => e.Title == existing.Title);
            if (enriched == null) continue;

            if (string.IsNullOrEmpty(existing.DetailedDescription))
                existing.DetailedDescription = enriched.DetailedDescription;
            if (string.IsNullOrEmpty(existing.TechnicalSpecs))
                existing.TechnicalSpecs = enriched.TechnicalSpecs;
            if (string.IsNullOrEmpty(existing.ImageUrl) && !string.IsNullOrEmpty(enriched.ImageUrl))
                existing.ImageUrl = enriched.ImageUrl;
            if (!existing.IsFeatured && enriched.IsFeatured)
                existing.IsFeatured = true;
            if (string.IsNullOrEmpty(existing.AvailabilityNote) && !string.IsNullOrEmpty(enriched.AvailabilityNote))
                existing.AvailabilityNote = enriched.AvailabilityNote;
            // Update ShortDescription if the enriched one is more detailed
            if (!string.IsNullOrEmpty(enriched.ShortDescription) && enriched.ShortDescription.Length > (existing.ShortDescription?.Length ?? 0))
                existing.ShortDescription = enriched.ShortDescription;
        }

        if (toAdd.Any())
        {
            context.Equipment.AddRange(toAdd);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedEquipmentHeroPhotoAsync(AppDbContext context)
    {
        if (await context.SitePhotos.AnyAsync(p => p.PhotoKey == "hero-equipment")) return;

        context.SitePhotos.Add(new SitePhoto
        {
            PhotoKey = "hero-equipment",
            Title = "Equipment Page Hero Background",
            DisplayOrder = 8,
            IsActive = true
        });
        await context.SaveChangesAsync();
    }
}
