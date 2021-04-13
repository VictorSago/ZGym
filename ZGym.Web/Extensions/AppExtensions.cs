

using Microsoft.AspNetCore.Http;

namespace ZGym.Web.Extensions
{
    public static class AppExtensions
    {
        public static bool IsAjax(this HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        // TODO Method for testing `IsAuthenticated`
    }
}