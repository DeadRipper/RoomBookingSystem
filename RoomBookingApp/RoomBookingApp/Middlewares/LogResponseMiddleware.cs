namespace RoomBookingApp.Middlewares
{
    public class LogResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public LogResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<LogResponseMiddleware> logger)
        {
            // Log the outgoing response
            logger.LogInformation($"Outgoing response: {context.Response.StatusCode}");
            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}