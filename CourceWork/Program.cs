using CourceWork.Data;
using CourceWork.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// --- Конфигурация базы данных ---
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

// --- Регистрация сервисов ---
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<TvshowServices>();
builder.Services.AddScoped<GuestsService>();
builder.Services.AddScoped<UserService>();

// --- Регистрация кэша ---
builder.Services.AddMemoryCache();

// --- Настройка аутентификации ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
    });

// --- Регистрация контроллеров и представлений ---
builder.Services.AddControllersWithViews();

// --- Настройка логирования ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Добавляем консольный провайдер

var app = builder.Build();

// --- Настройка middleware ---
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

app.UseAuthentication();    // Аутентификация
app.UseAuthorization();     // Авторизация

// --- Настройка маршрутов ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();