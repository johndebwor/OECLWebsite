using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using OECLWebsite.Components;
using OECLWebsite.Data;
using OECLWebsite.Data.Interceptors;
using OECLWebsite.Features.Auth;
using OECLWebsite.Infrastructure.Data;
using OECLWebsite.Infrastructure.Identity;
using OECLWebsite.Infrastructure.Middleware;
using OECLWebsite.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// HttpContextAccessor (needed by AuditInterceptor)
builder.Services.AddHttpContextAccessor();

// Database
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditInterceptor>();
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        })
        .AddInterceptors(interceptor);
}, ServiceLifetime.Transient);

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password Policy
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 4;

    // Lockout Policy
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User Policy
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Set to true when email service is configured
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

// Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/auth/access-denied";
});

// Authorization Policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireCEO", policy => policy.RequireRole("CEO"))
    .AddPolicy("RequireManagement", policy => policy.RequireRole("CEO", "GeneralManager"))
    .AddPolicy("RequireDepartmentHead", policy => policy.RequireRole("CEO", "GeneralManager", "DepartmentHead"))
    .AddPolicy("RequireContentCreator", policy => policy.RequireRole("CEO", "GeneralManager", "DepartmentHead", "SeniorOfficer", "ContentEditor"))
    .AddPolicy("RequireViewer", policy => policy.RequireRole("CEO", "GeneralManager", "DepartmentHead", "SeniorOfficer", "ContentEditor", "Viewer"));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// MudBlazor
builder.Services.AddMudServices();

// Memory Cache
builder.Services.AddMemoryCache();

// File Upload
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 6 * 1024 * 1024; // 6 MB for file uploads
});

var app = builder.Build();

// Seed Database
await DatabaseSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseSecurityHeaders();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseStaticFiles(); // Serve runtime-uploaded files from wwwroot
app.MapStaticAssets();

// Auth endpoints
app.MapAuthEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
