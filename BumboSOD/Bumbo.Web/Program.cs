using System.Globalization;
using Bumbo.Data;
using Bumbo.Data.DAL;
using Bumbo.Data.DAL.Interfaces;
using Bumbo.Data.Models;
using Bumbo.Data.Models.Enums;
using Bumbo.Prognosis;
using Bumbo.Prognosis.Repositories;
using Bumbo.Web.Services;
using Bumbo.Web.Services.HourRegistration;
using Bumbo.Web.Services.Interfaces;
using Bumbo.WorkingRules.CAORules;
using Bumbo.WorkingRules.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

var builder = WebApplication.CreateBuilder(args);

// Set culture to
var ci = new CultureInfo("nl-NL")
{
    DateTimeFormat =
    {
        DateSeparator = "-"
    },
    NumberFormat = NumberFormatInfo.InvariantInfo
};
CultureInfo.DefaultThreadCurrentCulture = ci;
CultureInfo.DefaultThreadCurrentUICulture = ci;

// Add services to the container.
builder.Services.AddDbContextPool<BumboDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BumboDB"),
        sqlOptions => sqlOptions.CommandTimeout(30)));
builder.Services.AddHttpContextAccessor();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddEntityFrameworkStores<BumboDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


// Add scoped repositories, ask for the interface, get the implementation
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkStandardsRepository, WorkStandardsRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IEmployeeExchangeRequestRepository, EmployeeExchangeRequestRepository>();
builder.Services.AddScoped<IEmployeeScheduleRepository, EmployeeScheduleRepository>();
builder.Services.AddScoped<IEmployeeWorkedHoursRepository, EmployeeWorkedHoursRepository>();
builder.Services.AddScoped<IEmployeeAvailabilityRepository, EmployeeAvailabilityRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IBranchManagementRepository, BranchManagementRepository>();
builder.Services.AddScoped<IManagerExchangeRequestRepository, ManagerExchangeRequestRepository>();
builder.Services.AddScoped<IPrognosisRepository, PrognosisEfCoreRepository>();
builder.Services.AddScoped<IPrognosisService, PrognosisService>();
builder.Services.AddScoped<IExportWorkedHoursRepository, ExportWorkedHoursRepository>();
builder.Services.AddScoped<ICaoRepository, CaoEfCoreRepository>();
builder.Services.AddScoped<IWorkingRules, WorkingRules>();
builder.Services.AddScoped<IUserManagerRepository, UserManagerRepository>();
builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IHourRegistrationService, HourRegistrationService>();

// Add singleton services, ask for the interface, get the implementation
builder.Services.AddSingleton<ITimelinePrognosisService, TimelinePrognosisService>();
builder.Services.AddSingleton<ITimelineSortingService, TimelineSortingService>();
builder.Services.AddSingleton<ITimelineModelService, TimelineModelService>();
builder.Services.AddSingleton<ITimelineItemUpdateService, TimelineItemUpdateService>();
builder.Services.AddSingleton<ITimelineService, TimelineService>();

// Add transient services, different with each request
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Default setup: User must be logged in to access all web pages.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("IsAdmin", policy => policy.RequireRole(nameof(Role.Admin)));
    options.AddPolicy("CanDeactivateAccounts", policy => policy.RequireRole(nameof(Role.Admin), nameof(Role.BranchManager)));
});

builder.Services.AddControllersWithViews();

// cookie setup
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("nl-NL");
});

// token config
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    o.TokenLifespan = TimeSpan.FromHours(3));

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Setup for Rotativa (publishing a schedule)
RotativaConfiguration.Setup((IHostingEnvironment)app.Environment);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();