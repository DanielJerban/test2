using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.BuildingBlocks.Infrastructure.Extensions.Marten;

public static class AddMartenExtesions
{
    public static IServiceCollection AddMartenDependency(this IServiceCollection services, string cnnString, string schema)
    {

        services.AddMarten(cnnString);
        services.AddSingleton(CreateDocumentStore(cnnString, schema));
        return services;
    }
    public static IDocumentStore CreateDocumentStore(string cnnString, string schema)
    {
        return DocumentStore.For(_ =>
        {
            _.Connection(cnnString);
            _.DatabaseSchemaName = schema;
            _.Serializer(CustomSerializer());
        });
    }
    private static ISerializer CustomSerializer()
    {
        var serializer = new JsonNetSerializer();
        serializer.Customize(_ =>
        {
            _.ContractResolver = new ProtectedSettersContractResolver();
            _.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
        });
        return serializer;
    }
}