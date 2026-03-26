using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estudaki.Infrastructure.Observability.Setups
{
    internal static class Logger
    {
        internal static IHostApplicationBuilder LoggerInit(this WebApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.UseGrafana().AddOtlpExporter();
            });


            return builder;
        }
    }
}
