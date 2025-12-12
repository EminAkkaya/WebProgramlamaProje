using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProgramlamaProje.Data;
using WebProgramlamaProje.Models;



var builder = WebApplication.CreateBuilder(args);

// 1. DbContext Ekleme
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity Ekleme
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Şifre kurallarını geliştirme aşamasında esnetebilirsin
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Kullanıcı login değilse buraya atar
    options.LoginPath = "/Account/Login";

    // Yetkisi yetmiyorsa buraya atar
    options.AccessDeniedPath = "/Account/AccessDenied";

    // Cookie süresi (Örn: 60 dakika)
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    // SlidingExpiration: Kullanıcı işlem yaptıkça süreyi uzatır
    options.SlidingExpiration = true;
});
// HttpClient servisini ekle
builder.Services.AddHttpClient();

// Gemini servisini sisteme tanıt
builder.Services.AddScoped<WebProgramlamaProje.Services.IGeminiService, WebProgramlamaProje.Services.GeminiService>();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapRazorPages();
app.Run();
