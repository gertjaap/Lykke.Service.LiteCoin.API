using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Common.ApiLibrary.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Lykke.Service.Vertcoin.API.Middleware
{
    public static class MiddlewareApplicationBuilderExtensions
    {
        public static void UseCustomErrorHandligMiddleware(this IApplicationBuilder app, string componentName)
        {
            app.UseMiddleware<CustomGlobalErrorHandlerMiddleware>(componentName);
        }
    }
}
