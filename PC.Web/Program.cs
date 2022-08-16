using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PC.Services.Core;
using PC.Services.Core.Security;
using PC.Services.DL;
using PC.Services.DL.DbContext;
using PC.Services.DL.Interfaces;
using PC.Services.DL.Interfaces.Repos;
using PC.Web.Filters;
using PC.Web.Initializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContext<AppDBContext>(
            options => options.UseSqlServer(builder.Configuration.GetConnectionString("ServiceDBConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                o =>
                {
                    // configure identity options
                    o.Password.RequireDigit = builder.Configuration.GetValue<bool>("AppSetting:PassRequireDigit");
                    o.Password.RequireLowercase = builder.Configuration.GetValue<bool>("AppSetting:PassRequireLowercase");
                    o.Password.RequireUppercase = builder.Configuration.GetValue<bool>("AppSetting:PassRequireUppercase");
                    o.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("AppSetting:PassRequireNonAlphanumeric");
                    o.Password.RequiredLength = builder.Configuration.GetValue<int>("AppSetting:UserPasswordLength");
                })
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("AppSetting:UserSessionTimeOut"));
    options.LoginPath = "/Account/Login";
    options.SlidingExpiration = true;
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddSession(options =>
 {
     options.IdleTimeout = TimeSpan.FromMinutes(60);
 });

builder.Services.AddControllersWithViews(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
}
);
//services.AddScoped<Ih, HttpContextFactory>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IUserAuthorityMatrixHelper, UserAuthorityMatrixHelper>();
//services.AddTransient<ISettingRepository, SettingRepository>();
//services.AddSingleton<IConfiguration>(Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//if (app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

var Initializer = app.Services.CreateScope().ServiceProvider.GetRequiredService<IDbInitializer>();
Initializer.Initialize();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

app.Run();
