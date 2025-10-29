using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers; // <-- ¡Añade esta línea!
using System.Web.Http;
using System.Web.Http.Cors;

namespace App_Citas_medicas_backend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración de rutas de Web API

            // Habilitar CORS
            var cors = new EnableCorsAttribute("http://localhost:9002", "*", "*"); // Asegúrate de que esta URL sea la de tu frontend de Next.js
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // ***** AÑADIR ESTAS LÍNEAS PARA FORZAR JSON COMO FORMATO PREDETERMINADO *****
            // Esto asegura que tu API devuelva JSON incluso si el cliente no envía el Accept header.
            // Y también remueve el formateador XML para evitar que se use.
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); // Para que los navegadores muestren JSON en lugar de XML por defecto
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json")); // Asegura que JSON es preferido

            // Eliminar el formateador XML por completo
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // *****************************************************************************
        }
    }
}