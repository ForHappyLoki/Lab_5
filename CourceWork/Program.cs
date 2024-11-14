using CourceWork.Data;
using CourceWork.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var builderDB = new ConfigurationBuilder();
builderDB.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."));
builderDB.AddJsonFile("appsettings.json");
var config = builderDB.Build();
string? connectionString = config.GetConnectionString("DefaultConnection");

var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
var options = optionsBuilder
    .UseSqlServer(connectionString)
    .Options;

// установка конфигурации подключенияIServiceCollection
var builderCookie = new ServiceCollection();

// Добавление аутентификации
builderCookie.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => //CookieAuthenticationOptions
        {
            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Authentication/Login");
        });
StaticData.StaticDB = new DatabaseContext(options);

// Configure the HTTP request pipeline.
var builderWebApplication = WebApplication.CreateBuilder(args);

// Add services to the container.
builderWebApplication.Services.AddControllersWithViews();

// Configure database context
builderWebApplication.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builderWebApplication.Configuration.GetConnectionString("DefaultConnection")));

// Add authentication
builderWebApplication.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Authentication/Login");
        options.AccessDeniedPath = "/Authentication/AccessDenied";
    });

var app = builderWebApplication.Build();
builderWebApplication.Services.AddMemoryCache();
var scope = app.Services.CreateScope();
var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
StaticData.StaticCacheUserService = new UserService(StaticData.StaticDB, memoryCache);
// Добавление логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Добавляем консольный провайдер


// Настройка middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Страница ошибок в разработке
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Страница ошибок для продакшена
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();  