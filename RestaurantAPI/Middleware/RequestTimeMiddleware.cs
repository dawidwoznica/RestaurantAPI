
using System.Diagnostics;

namespace RestaurantAPI.Middleware
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private readonly ILogger<RequestTimeMiddleware> logger;
        private readonly Stopwatch stopwatch;

        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            this.logger = logger;
            stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            stopwatch.Start();

            await next.Invoke(context);

            stopwatch.Stop();

            if(stopwatch.ElapsedMilliseconds > 4000)
                logger.LogInformation($"Request [{context.Request.Method}] at {context.Request.Path} took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
