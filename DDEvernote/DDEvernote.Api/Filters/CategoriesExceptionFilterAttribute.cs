using DDEvernote.Model;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace DDEvernote.Api.Filters
{
    public class CategoriesExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is CategoryNotFoundException)
            {
                Logger.Log.Instance.Error("При обработке категории с id: {0} произошла ошибка: {1}", ((CategoryNotFoundException)(context.Exception)).CategoryNotFoundId, context.Exception.Message);
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
        }
    }
}