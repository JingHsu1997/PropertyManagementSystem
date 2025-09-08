using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Core.Services;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Data.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 設定編碼以支援中文字元
Console.OutputEncoding = Encoding.UTF8;

// Add Razor Pages services
builder.Services.AddRazorPages();

// Add Dapper dependencies
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<PropertyDbContext>();

// Register services
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 註冊 Controller 路由 (MVC)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 註冊 Razor Pages (如有)
app.MapRazorPages();

app.Run();
