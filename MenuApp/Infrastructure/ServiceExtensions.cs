using FluentValidation.AspNetCore;
using MenuApp.Core.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis.Host;

namespace MenuApp.Infrastructure;

public static class ServiceExtensions
{
    public static void ServiceRegisterer(this IServiceCollection services, IConfiguration configuration)
    {
        // LANGUAGE SERVICES
        // services.AddScoped<ILanguageService, LanguageService>();
        // services.AddScoped<ILocalizationService, LocalizationService>();
        // services.AddLocalization();

        services.AddAntiforgery(options =>
        {
            // Set a different cookie name
            options.Cookie.Name = "MenuApp.AntiforgeryToken";

            // Configure cookie settings
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;

            // Configure form field names
            options.FormFieldName = "__AntiForgeryField";
            options.HeaderName = "X-CSRF-TOKEN";
        });

        // LOG SERVICES
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddLogging();

        //MAPPER
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        //CACHE SERICES
        services.AddMemoryCache();
        // services.AddRedisCache(configuration);
        
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
    }

    // private static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    // {
    //     try
    //     {
    //         var redisConnection = configuration.GetValue<string>("RedisCacheUrl");
    //
    //         services.AddStackExchangeRedisCache(options =>
    //         {
    //             options.Configuration = redisConnection;
    //             options.InstanceName = "MenuApp:";
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         // Handle the exception (log, throw, or handle as appropriate)
    //         Console.WriteLine($"Error in AddRedisCache: {ex.Message}");
    //         throw;
    //     }
    // }
}