using ThreeCats.Web.Components;
using ThreeCats.Application;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Infrastructure.Data;
using ThreeCats.Infrastructure;
using ThreeCats.Application.Options;

namespace ThreeCats.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
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
                        
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            await using (var scope = app.Services.CreateAsyncScope())
            {
                var dbFactory = scope.ServiceProvider
                    .GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

                await using var dbContext = await dbFactory.CreateDbContextAsync();

                await dbContext.Database.MigrateAsync();
            }

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
