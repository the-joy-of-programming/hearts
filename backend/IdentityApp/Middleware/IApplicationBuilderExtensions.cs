using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace IdentityApp
{
    public static class IApplicationBuilderExtensions
    {

        public static void UseApiExceptionMapper(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try {
                    await next.Invoke();
                } catch (ApiException ex)
                {
                    if(!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int) ex.StatusCode;
                        context.Response.Headers["Content-Type"] = "application/json";
                        await context.Response.WriteAsync($"{{\"message\":\"{ex.Message.Replace('"', '\'')}\"}}");
                    }
                    else
                    {
                        throw ex;
                    }
                }
            });
        }

    }
}