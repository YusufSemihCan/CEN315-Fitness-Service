using Fitness_Service_API.Infrastructure;
using Fitness_Service_API.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using System.Linq;

namespace Fitness_Service_UnitTest
{
    public class ServiceRegistrationTests
    {
        [Fact]
        public void AddApplicationServices_ShouldRegister_AllServices_WithStrictRequirements()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();
            var provider = services.BuildServiceProvider();

            // =================================================================
            // 1. DOMAIN SERVICES (Kill "Lifetime" & "Removal" Mutants)
            // =================================================================

            // Check PricingService
            var pricingDescriptor = services.Single(s => s.ServiceType == typeof(IPricingService));
            Assert.Equal(ServiceLifetime.Scoped, pricingDescriptor.Lifetime); // Kill Singleton/Transient mutants
            Assert.Equal(typeof(PricingService), pricingDescriptor.ImplementationType);
            Assert.NotNull(provider.GetService<IPricingService>()); // Kill removal mutant

            // Check ReservationService
            var resDescriptor = services.Single(s => s.ServiceType == typeof(IReservationService));
            Assert.Equal(ServiceLifetime.Scoped, resDescriptor.Lifetime);
            Assert.Equal(typeof(ReservationService), resDescriptor.ImplementationType);
            Assert.NotNull(provider.GetService<IReservationService>());

            // =================================================================
            // 2. CONTROLLERS (Kill "AddControllers" Removal)
            // =================================================================
            // AddControllers registers 'IControllerActivator'. 
            // If the mutant deletes AddControllers(), this service will be missing.
            var controllerActivator = services.FirstOrDefault(x => x.ServiceType.Name == "IControllerActivator");
            Assert.NotNull(controllerActivator);

            // =================================================================
            // 3. API EXPLORER (Kill "AddEndpointsApiExplorer" Removal)
            // =================================================================
            // This method is required for Minimal APIs and Swagger to see your endpoints.
            // It specifically registers 'IApiDescriptionGroupCollectionProvider'.
            // We use string matching to avoid needing the exact namespace reference in the test project.
            var apiExplorer = services.FirstOrDefault(x =>
                x.ServiceType.Name.Contains("IApiDescriptionGroupCollectionProvider"));

            Assert.True(apiExplorer != null,
                "AddEndpointsApiExplorer() was removed. The API Explorer provider is missing.");

            // =================================================================
            // 4. SWAGGER (Kill "AddSwaggerGen" Removal)
            // =================================================================
            // AddSwaggerGen registers 'ISwaggerProvider' AND 'SwaggerGeneratorOptions'.
            // Checking for the Options object is the strictest proof that the generator was added.
            var swaggerOptions = services.FirstOrDefault(x =>
                x.ServiceType.Name.Contains("SwaggerGeneratorOptions"));

            Assert.True(swaggerOptions != null,
                "AddSwaggerGen() was removed. Swagger options are missing.");

            // Double check checking the provider itself
            var swaggerProvider = services.FirstOrDefault(x =>
                x.ServiceType.Name.Contains("ISwaggerProvider"));
            Assert.NotNull(swaggerProvider);
        }
    }
}