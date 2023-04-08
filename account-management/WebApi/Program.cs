using PlatformPlatform.AccountManagement.Application;
using PlatformPlatform.AccountManagement.Infrastructure;
using PlatformPlatform.AccountManagement.WebApi.Endpoints;

namespace PlatformPlatform.AccountManagement.WebApi;

public class Program
{
    internal static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddWebApiServices();

        var app = builder.Build();

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
            app.UseHsts(); // Defaults to 30 days. See https://aka.ms/aspnetcore-hsts.
        }

        app.UseHttpsRedirection();

        app.MapGet("/", () => "Hello World!").ExcludeFromDescription();

        app.MapTenantEndpoints();

        app.Run();
    }
}