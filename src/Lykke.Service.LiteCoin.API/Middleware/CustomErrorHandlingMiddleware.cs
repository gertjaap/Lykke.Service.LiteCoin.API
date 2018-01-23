using System;
using System.IO;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.Service.LiteCoin.API.Middleware
{
    public class CustomGlobalErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly string _componentName;
        private readonly RequestDelegate _next;

        public CustomGlobalErrorHandlerMiddleware(RequestDelegate next, ILog log, string componentName)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _componentName = componentName ?? throw new ArgumentNullException(nameof(componentName));
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await LogError(context, ex);
                await CreateErrorResponse(context, ex);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            using (var ms = new MemoryStream())
            {
                context.Request.Body.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);

                await _log.LogPartFromStream(ms, _componentName, context.Request.GetUri().AbsoluteUri, ex);
            }
        }

        private async Task CreateErrorResponse(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            

            ctx.Response.StatusCode = IsValidationError(ex) ? 400 : 500;

            var response = ErrorResponse.Create(ex.ToString());

            var responseJson = JsonConvert.SerializeObject(response);

            await ctx.Response.WriteAsync(responseJson);
        }

        private bool IsValidationError(Exception ex)
        {
            return ex is BusinessException businessException && businessException.Code == ErrorCode.BadInputParameter;
        }
    }
}
