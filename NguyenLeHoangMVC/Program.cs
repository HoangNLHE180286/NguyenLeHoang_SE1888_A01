using Microsoft.EntityFrameworkCore;
using NguyenLeHoangMVC_DataLayer.Models;
using NguyenLeHoangMVC_DataLayer.Repositories;
using NguyenLeHoangMVC_BusinessLayer.Services;

namespace NguyenLeHoangMVC {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<FunewsManagementContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IRepository<Category>>((serviceProvider) => new Repository<Category>(serviceProvider.GetRequiredService<FunewsManagementContext>()));
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<INewsService, NewsService>();

            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}/{id?}");
            app.MapControllerRoute(name: "accountmanagement", pattern: "{controller=Account}/{action=AccountManagement}/{id?}");

            app.Run();
        }
    }
}
