using Banky.Modules.Accounts;
using Banky.Modules.Clients;
using Scalar.AspNetCore;

namespace Banky.Server;

public static class DependencyInjection
{
    public static void AddBankyModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();
        services.AddAccountsModule(configuration);
        services.AddClientsModule(configuration);
    }

    public static void MapBankyApiRoutes(this WebApplication app)
    {
        app.UseCors(config => config.AllowAnyOrigin().AllowAnyHeader());

        var apiGroup = app.MapGroup("/api/v1");
        apiGroup.WithOpenApi();

        apiGroup.MapAccountsRoutes();
        apiGroup.MapClientsRoutes();
    }

    public static async Task ConfigureDatabase(this WebApplication app)
    {
        // Ideally, should be done as part of the CI/CD pipeline in upper environments
        await app.ConfigureAccountsDatabase();
        await app.ConfigureClientsDatabase();
    }

    public static void AddBankyOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(
                (opt, context, ct) =>
                {
                    opt.Info.Title = "Banky API | v1";
                    return Task.CompletedTask;
                }
            );
            options.AddSchemaTransformer(
                (schema, context, ct) =>
                {
                    var schemaType = context.JsonTypeInfo.Type;

                    schema.Title = schemaType.DeclaringType is null
                        ? schema.Title
                        : $"{schemaType.DeclaringType.Name}{schemaType.Name}";

                    return Task.FromResult(schema);
                }
            );
        });
    }

    public static void UseBankyOpenApi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}
