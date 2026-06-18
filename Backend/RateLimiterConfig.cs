using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace WebApi.Configurations;

public static class RateLimiterConfig
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                {
                    var clientId = httpContext.Connection.RemoteIpAddress?.ToString() 
                                    ?? httpContext.Request.Headers["X-Forwarded-For"].ToString();
                    
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: clientId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 10, // 10 запросов в минуту
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        });
                });
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync(
                    "Слишком много запросов! Пожалуйста, попробуйте позже."
                );
            };
        });
        
        return services;
    }
}
