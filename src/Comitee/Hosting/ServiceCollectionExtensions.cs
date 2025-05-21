using Microsoft.Extensions.DependencyInjection;

namespace Comitee.Hosting;

public static class ServiceCollectionExtensions
{
    public static ComiteeBuilder AddComitee(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        // Add common registration code...
        
        return new ComiteeBuilder(services);
    }
}