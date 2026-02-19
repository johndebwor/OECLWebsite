using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OECLWebsite.Data.Entities;
using OECLWebsite.Infrastructure.Identity;

namespace OECLWebsite.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectImage> ProjectImages => Set<ProjectImage>();
    public DbSet<ContentPage> ContentPages => Set<ContentPage>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<ApprovalWorkflow> ApprovalWorkflows => Set<ApprovalWorkflow>();
    public DbSet<ApprovalHistory> ApprovalHistory => Set<ApprovalHistory>();
    public DbSet<PageView> PageViews => Set<PageView>();
    public DbSet<ServiceInquiry> ServiceInquiries => Set<ServiceInquiry>();
    public DbSet<SitePhoto> SitePhotos => Set<SitePhoto>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filters for soft-delete
        modelBuilder.Entity<ServiceCategory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Project>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ContentPage>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Announcement>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServiceInquiry>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SitePhoto>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<GalleryImage>().HasQueryFilter(e => !e.IsDeleted);
    }
}
