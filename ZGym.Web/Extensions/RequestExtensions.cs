

using Microsoft.AspNetCore.Http;

namespace ZGym.Web.Extensions
{
    public static class RequestExtensions
    {
        public static bool IsAjax(this HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}