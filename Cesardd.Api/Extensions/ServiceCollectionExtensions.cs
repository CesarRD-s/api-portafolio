using Cesardd.Shared.Results;
using Microsoft.AspNetCore.RateLimiting;

namespace Cesardd.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiPolicies(this IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://www.creyesdd.com",
                            "http://localhost:3000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Rate Limiting
            services.AddRateLimiter(options =>
            {
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;

                    var response = ApiResponse<object>.Fail("Demasiadas solicitudes, intenta más tarde");

                    await context.HttpContext.Response.WriteAsJsonAsync(response, token);
                };

                options.AddFixedWindowLimiter("HealthPolicy", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("RateLimiterPolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });
            });

            return services;
        }
    }
}
