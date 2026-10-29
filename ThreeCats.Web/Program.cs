using ThreeCats.Web.Components;
using ThreeCats.Web.Configuration;
using ThreeCats.Web.Application;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Web.Data;
using Microsoft.AspNetCore.Identity;
using ThreeCats.Web.Domain.Entities;

namespace ThreeCats.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddOptions<ClinicOptions>()
                .Bind(builder.Configuration.GetSection(ClinicOptions.SectionName))
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.Name),
                    "Clinic name must not be empty.")
                .ValidateOnStart();

            builder.Services.AddAplicationService();

            builder.Services.AddDbContextFactory<ApplicationDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
