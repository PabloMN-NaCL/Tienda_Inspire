using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiendaInspire.Services
{
    //Extensiones para los servicios y proyectos
    //Health checks
    public static class Extensions
    {

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddRequestTimeouts(
                configure: static timeouts =>
                    timeouts.AddPolicy("HealthChecks", TimeSpan.FromSeconds(5)));

            builder.Services.AddOutputCache(
                configureOptions: static caching =>
                    caching.AddPolicy("HealthChecks",
                    build: static policy => policy.Expire(TimeSpan.FromSeconds(10))));

            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }


    }
}
