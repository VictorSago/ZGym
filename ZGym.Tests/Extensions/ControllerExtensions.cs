using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ZGym.Tests.Extensions
{
    public static class ControllerExtensions
    {
        public static void SetUserIsAuthenticated(this Microsoft.AspNetCore.Mvc.Controller controller, bool isAuthenticated)
        {
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(context => context.User.Identity.IsAuthenticated).Returns(isAuthenticated);

            controller.ControllerContext = new ControllerContext 
            {
                HttpContext = mockContext.Object
            };
        }
    }
}