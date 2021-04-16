using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ZGym.Web.Controllers;
using ZGym.Web.Filters;

namespace ZGym.Tests.Filters
{
    [TestClass]
    public class RequiredIdRequiredModelTests
    {
        private Mock<GymClassesController> controller;

        [TestInitialize]
        public void SetUp()
        {
            controller = new Mock<GymClassesController>();
        }

        [TestMethod]
        public void Details_NullId_ShouldReturnBadRequest()
        {
            var routeValues = new RouteValueDictionary();
            routeValues.Add("id", null);
            var routeData = new RouteData(routeValues);

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(), 
                routeData, 
                Mock.Of<ActionDescriptor>(), 
                Mock.Of<ModelStateDictionary>()
            );
            var actionExecutingContext = new ActionExecutingContext(
                actionContext, 
                new List<IFilterMetadata>(), 
                routeValues, 
                controller
            );

            var filter = new RequiredIdRequiredModel("Id");
            filter.OnActionExecuting(actionExecutingContext);
            var result = actionExecutingContext.Result;

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }
    }
}