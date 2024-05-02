
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebBanGiay.DataAccess;
using WebBanGiay.Models;
using WebBanGiay.Models.Momo;
using WebBanGiay.Repositories;
using WebBanGiay.Services;
using Microsoft.AspNetCore.Authentication.Google;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);//th?i gian h?t h?n
    options.Cookie.HttpOnly = true;//Cookie ch? ???c truy c?p b?ng HTTP
    options.Cookie.IsEssential = true;//Cookie là b?t bu?c cho phiên
});
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");

        googleOptions.ClientId = googleAuthNSection["ClientId"];
        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
        googleOptions.CallbackPath = "/dang-nhap-tu-google";
    })
    .AddFacebook(facebookOptions => {
        // Đọc cấu hình
        IConfigurationSection facebookAuthNSection = configuration.GetSection("Authentication:Facebook");
        facebookOptions.AppId = facebookAuthNSection["AppId"];
        facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
        // Thiết lập đường dẫn Facebook chuyển hướng đến
        facebookOptions.CallbackPath = "/dang-nhap-tu-facebook";
    });


builder.Services.AddRazorPages();

builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// ??t tr??c UseRouting
app.UseSession();
app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapRazorPages();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    name: "employee",
    pattern: "{area:exists}/{controller=Employee}/{action=Index}/{id?}"
    );

app.Run();