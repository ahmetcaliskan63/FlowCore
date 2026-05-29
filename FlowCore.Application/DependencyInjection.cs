using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlowCore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            service.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return service;
        }
    }
}
