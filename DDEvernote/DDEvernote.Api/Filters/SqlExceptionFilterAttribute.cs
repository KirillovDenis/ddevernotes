using DDEvernote.Model;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Data.SqlClient;

namespace DDEvernote.Api.Filters
{
    public class SqlExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if(context.Exception is SqlException)
            {
                Logger.Log.Instance.Error("При работе с базой данных произошла ошибка: {1}", context.Exception.Message);
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, context.Exception.Message);
            }
        }
    }
}