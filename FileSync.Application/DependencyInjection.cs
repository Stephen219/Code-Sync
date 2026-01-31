using FileSync.Application.Services.Filedrop;
using FileSync.Application.Services.Space;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ISpaceService, SpaceService>();
            services.AddScoped<IDropService, DropService>();

            return services;
        }
    }
}
