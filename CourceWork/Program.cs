using CourceWork.Data;
using CourceWork.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// --- ������������ ���� ������ ---
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString));

// --- ����������� �������� ---
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<TvshowServices>();
builder.Services.AddScoped<GuestsService>();
builder.Services.AddScoped<UserService>();

// --- ����������� ���� ---
builder.Services.AddMemoryCache();

// --- ��������� �������������� ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
    });

// --- ����������� ������������ � ������������� ---
builder.Services.AddControllersWithViews();

// --- ��������� ����������� ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // ��������� ���������� ���������

var app = builder.Build();

// --- ��������� middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // �������� ������ � ����������
}
else
{
    app.UseExceptionHandler("/Home/Error"); // �������� ������ ��� ����������
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();    // ��������������
app.UseAuthorization();     // �����������

// --- ��������� ��������� ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();