using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors; // ¡IMPORTANTE: Añade esta línea!

namespace App_Citas_medicas_backend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración de rutas de Web API

            // Habilitar CORS - ¡Añade este bloque de código!
            // Permite solicitudes desde http://localhost:9002 (tu app Next.js)
            // Permite cualquier encabezado y cualquier método HTTP.
            // Para producción, "http://localhost:9002" debería ser el dominio real de tu frontend.
            var cors = new EnableCorsAttribute("http://localhost:9002", "*", "*");
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

