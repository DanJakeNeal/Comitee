using Microsoft.Extensions.DependencyInjection;

namespace Comitee.Hosting;

public sealed class ComiteeBuilder : IComiteeBuilder
{
    internal ComiteeBuilder(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        Services = services;
    }

    public IServiceCollection Services { get; }
}