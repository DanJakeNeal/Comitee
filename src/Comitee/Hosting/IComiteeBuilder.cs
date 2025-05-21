using Microsoft.Extensions.DependencyInjection;

namespace Comitee.Hosting;

public interface IComiteeBuilder
{
    IServiceCollection Services { get; }
}