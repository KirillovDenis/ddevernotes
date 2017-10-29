using DDEvernote.Model;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace DDEvernote.Api.Filters
{
    public class NotesExceptionFilterAttribute: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NoteNotFoundException)
            {
                Logger.Log.Instance.Error("При обработке заметки с id: {0} произошла ошибка: {1}", ((NoteNotFoundException)(context.Exception)).NoteNotFoundId, context.Exception.Message);
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.NotFound, context.Exception.Message);
            }
        }
    }
}