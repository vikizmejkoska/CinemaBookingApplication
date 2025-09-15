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
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
// Bind TMDB options
builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("Tmdb"));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();



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
    http.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", opt.ApiKey);
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
