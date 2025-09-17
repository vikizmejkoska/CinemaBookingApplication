using CinemaBookingApplication.Domain.Identity;
using CinemaBookingApplication.Repository.Data;
using CinemaBookingApplication.Repository.Implementation;
using CinemaBookingApplication.Repository.Interface;
using CinemaBookingApplication.Service.Implementation;
using CinemaBookingApplication.Service.Interface;
using CinemaBookingApplication.Service.Options;
using CinemaBookingApplication.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Bind TMDB options
builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("Tmdb"));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity + Roles
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // << enable roles
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Repos & Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IMovieService, MovieService>();
builder.Services.AddTransient<IScreeningService, ScreeningService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IHallService, HallService>();

// Typed HttpClient for TMDB
builder.Services.AddHttpClient<IDataFetchService, TmdbFetchService>((sp, http) =>
{
    var opt = sp.GetRequiredService<IOptions<TmdbOptions>>().Value;

    // Ensure BaseAddress ends with /3/
    var baseUrl = (opt.BaseUrl ?? "https://api.themoviedb.org/3").TrimEnd('/');
    if (!baseUrl.EndsWith("/3", StringComparison.Ordinal)) baseUrl += "/3";
    http.BaseAddress = new Uri(baseUrl + "/");

    http.DefaultRequestHeaders.Accept.ParseAdd("application/json");

    // Use v4 read access token (store it in user-secrets)
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opt.ApiKey);
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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Seed Admin role + user (simple)
await SeedAdminAsync(app);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// ====== Seeder ======
static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // Ensure roles exist
    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Create admin user from config or defaults
    var email = config["Admin:Email"] ?? "admin@cinema.local";
    var pass = config["Admin:Password"] ?? "Admin123!";

    var admin = await userManager.FindByEmailAsync(email);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = "Admin"
        };
        var result = await userManager.CreateAsync(admin, pass);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new Exception("Failed to create admin user: " + errors);
        }
    }

    if (!await userManager.IsInRoleAsync(admin, "Admin"))
    {
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}
