using Blazored.LocalStorage;
using ImageCompressor.Web.Auth;
using ImageCompressor.Web.Components;
using ImageCompressor.Web.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System;

namespace ImageCompressor.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // --- AUTH EKLEMELERÝ ---
            // 1. Kimlik doðrulama ve yetkilendirme servislerini ekleyin
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
{
    options.LoginPath = "/login"; // Yetkisiz kullanýcý buraya atýlsýn
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});
            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState(); // Blazor'da auth state'i tüm bileþenlere yaymak için gerekli

            // Sizin mevcut servis kayýtlarýnýz (daha önce eklediyseniz burada kalsýnlar)
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<HistoryService>();
            builder.Services.AddScoped<SystemMonitorService>();
            builder.Services.AddBlazoredLocalStorage();

            // -----------------------

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // --- GÜVENLÝ VERÝTABANI BAÞLATMA KODU ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AppDbContext>();

                    // Veritabanýnýn uyanmasýný bekle (Basit Retry Mantýðý)
                    var canConnect = false;
                    for (int i = 0; i < 5; i++) // 5 kere dene
                    {
                        if (dbContext.Database.CanConnect())
                        {
                            canConnect = true;
                            break;
                        }
                        Console.WriteLine($"Veritabaný bekleniyor... ({i + 1}/5)");
                        System.Threading.Thread.Sleep(2000); // 2 saniye bekle
                    }

                    if (canConnect)
                    {
                        dbContext.Database.EnsureCreated(); // Tablolarý oluþtur
                        Console.WriteLine("Veritabaný baþarýyla baðlandý ve hazýr!");
                    }
                    else
                    {
                        Console.WriteLine("HATA: Veritabanýna baðlanýlamadý.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Veritabaný Hatasý: {ex.Message}");
                }
            }
            // ----------------------------------------

            app.Run();
        }
    }
}
