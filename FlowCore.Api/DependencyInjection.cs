using FlowCore.Application;
using FlowCore.Infrastructure;

namespace FlowCore.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddControllers();

            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();

            service.AddInfrastructureServices(configuration);
            service.AddApplicationServices(configuration);

            return service;
        }
    }
}
