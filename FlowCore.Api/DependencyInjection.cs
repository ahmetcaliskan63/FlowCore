using FlowCore.Application;
using FlowCore.Core.Interfaces;
using FlowCore.Infrastructure;
using FlowCore.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FlowCore.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddControllers();

            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen();

            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });

            service.AddHttpContextAccessor();
            service.AddScoped<ICurrentUserService, CurrentUserService>();

            service.AddInfrastructureServices(configuration);
            service.AddApplicationServices(configuration);

            return service;
        }
    }
}
