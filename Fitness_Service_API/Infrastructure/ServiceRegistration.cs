using Fitness_Service_API.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fitness_Service_API.Infrastructure
{
    public static class ServiceRegistration
    {
        // This method holds ALL the logic that was previously in Program.cs
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // 1. Controllers
            services.AddControllers();

            // 2. Domain Services (The Logic you want to protect from Mutants)
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IReservationService, ReservationService>();

            // 3. Swagger Configuration
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}