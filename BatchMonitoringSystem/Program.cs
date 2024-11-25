using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.Services;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Use ApplicationUser instead of IdentityUser
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365);
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
var emailConfig = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddSingleton<IConverter, SynchronizedConverter>(sp =>
{
    var config = new PdfTools();
    return new SynchronizedConverter(config);
});

builder.Services.AddScoped<IActualDataDbContextFactory, ActualDataDbContextFactory>();
builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BatchService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<BackupService>();
builder.Services.AddScoped<UserActivityService>();




// Register Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RegisterUserPolicy", policy =>
       policy.Requirements.Add(new PermissionRequirement("RegisterUser")));
    options.AddPolicy("CreateDepartmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Create Department")));
    options.AddPolicy("ViewUserPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View User")));
    options.AddPolicy("AssignRolePolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Assign Role")));
    options.AddPolicy("RevokeRolePolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Revoke Role")));
    options.AddPolicy("ModifyUserPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Modify User")));
    options.AddPolicy("ViewDepartmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Department")));
    options.AddPolicy("EditDepartmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Edit Department")));
    options.AddPolicy("DeleteDepartmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Delete Department")));
    options.AddPolicy("CreateRolePolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Create Role")));
    options.AddPolicy("ViewRolePolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Role")));
    options.AddPolicy("AssignPermissionPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Assign Permission")));
    options.AddPolicy("ViewProductPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View product")));
    options.AddPolicy("CreateProductPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Create product")));
    options.AddPolicy("EditProductPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Edit product")));
    options.AddPolicy("ViewEquipmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Equipment")));
    options.AddPolicy("CreateEquipmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Create Equipment")));
    options.AddPolicy("EditEquipmentPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Edit Equipment")));
    options.AddPolicy("ViewBatchPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Batch")));
    options.AddPolicy("CreateBatchPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Create Batch")));
    options.AddPolicy("MonitorBatchStatusPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Monitor Batch Status")));
    options.AddPolicy("ViewUserActivityReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View User Activity Report")));
    options.AddPolicy("ExportUserActivityReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Export User Activity Report")));
    options.AddPolicy("ViewProductReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Product Report")));
    options.AddPolicy("ExportProductReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Export Product Report")));
    options.AddPolicy("ViewBatchStatusReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Batch Status Report")));
    options.AddPolicy("ExportBatchStatusReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Export Batch Status Report")));
    options.AddPolicy("ViewBackupReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("View Backup Report")));
    options.AddPolicy("ExportBackupReportPolicy", policy =>
      policy.Requirements.Add(new PermissionRequirement("Export Backup Report")));
});


// Register PermissionHandler
// Register custom authorization components
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

var app = builder.Build();


// Seed the initial data
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.SeedAsync();
}

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
