using Microsoft.AspNetCore.HttpLogging;

namespace RoomBookingApp.Helpers
{
    internal sealed class HttpLoggingInterceptor : IHttpLoggingInterceptor
    {
        public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
        {
            if (logContext.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                logContext.LoggingFields = HttpLoggingFields.None;
            }
            return default;
        }

        public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
        {
            if (!logContext.IsAnyEnabled(HttpLoggingFields.Response))
            {
                return default;
            }

            if (logContext.TryDisable(HttpLoggingFields.ResponseHeaders))
            {
                RedactRequestHeaders(logContext);
            }

            return default;
        }

        private void RedactRequestHeaders(HttpLoggingInterceptorContext logContext)
        {
            foreach (var header in logContext.HttpContext.Response.Headers)
            {
                logContext.AddParameter(header.Key, "RedactedHeader");
            }
        }
    }
}