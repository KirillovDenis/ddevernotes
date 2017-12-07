using System.Web.Http;
using WebActivatorEx;
using DDEvernote.Api;
using Swashbuckle.Application;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DDEvernote.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "DDEvernote.Api");
                        c.IncludeXmlComments(GetXmlCommentsPath());
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }

        private static string GetXmlCommentsPath()
        {
            return string.Format(@"{0}\bin\DDEvernote.Api.xml", AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
