using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ZGym.Web.Filters
{
    public class RequiredIdRequiredModel : ActionFilterAttribute
    {
        private readonly string parameterName;
        public RequiredIdRequiredModel(string parameterName)
        {
            this.parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values[parameterName] == null)
            {
                context.Result = new BadRequestResult();
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult viewResult)
            {
                if (viewResult.Model is null)
                {
                    context.Result = new NotFoundResult();
                }
            }
        }

    }
}