using PlatformPlatform.AccountManagement.Application;
using PlatformPlatform.AccountManagement.Infrastructure;
using PlatformPlatform.AccountManagement.WebApi.Endpoints;

namespace PlatformPlatform.AccountManagement.WebApi;

/// <summary>
///     The Program class is the main entry point. In .NET 6 it is no longer necessary to create a Program class.
///     However for UnitTests, to create a WebApplicationFactory we need a class that can be referenced.
/// </summary>
public class Program
{
    internal static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure services for the Application, Infrastructure, and WebApi layers.
        builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddWebApiServices();

        var app = builder.Build();

        // Enable the developer exception page and Swagger UI in the development environment.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger(); //Generate a swagger.json file
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AccountManagement API - Unstable")
            );
        }
        else
        {
            // Adds middleware for using HSTS, which adds the Strict-Transport-Security header
            // Defaults to 30 days. See https://aka.ms/aspnetcore-hsts, so be careful during development.
            app.UseHsts();

            // Adds middleware for redirecting HTTP Requests to HTTPS.
            app.UseHttpsRedirection();
        }
    
        // Add a default "Hello World!" endpoint at the root path.
        app.MapGet("/", () => "Hello World!").ExcludeFromDescription();

        // Map tenant-related endpoints.
        app.MapTenantEndpoints();

        // Run the web application.
        app.Run();
    }
}