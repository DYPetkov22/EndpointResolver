using EndpointResolver.Data;
using Microsoft.EntityFrameworkCore;

namespace EndpointResolver;

public class Program
{
    public static void Main(string[] args)
    {
        // Keep the original reflection-based console example available.
        if (args.Contains("--demo", StringComparer.OrdinalIgnoreCase))
        {
            ResolverDemo.Run(args);
            return;
        }

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection before starting the API.");
        }

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            // Create the local database and apply the included schema migrations.
            using var scope = app.Services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            database.Database.Migrate();

            app.MapOpenApi();
        }

        // The HTTP-only development profile has no HTTPS port to redirect to.
        // Launch-profile addresses are configuration values until the server starts.
        var hasHttpsEndpoint = app.Configuration["urls"]?
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) == true;
        if (!app.Environment.IsDevelopment() ||
            hasHttpsEndpoint || !string.IsNullOrEmpty(app.Configuration["HTTPS_PORT"]) ||
            !string.IsNullOrEmpty(app.Configuration["HTTPS_PORTS"]))
        {
            app.UseHttpsRedirection();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
