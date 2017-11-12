using DDEvernote.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace DDEvernote.Api.Filters
{
    public class UsersExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is UserNotFoundException)
            {
                Logger.Log.Instance.Error("При обработке пользователя с id: {0} произошла ошибка: {1}", ((UserNotFoundException)(context.Exception)).UserNotFoundId, context.Exception.Message);
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
            else if(context.Exception is ArgumentException){
                Logger.Log.Instance.Error("При запросе пользователя произошла ошибка: {1}", context.Exception.Message);
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
        }
    }
}