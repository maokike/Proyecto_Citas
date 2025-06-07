using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    [RoutePrefix("api/citas")]
    public class CitasController : ApiController
    {
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult RegistrarCita([FromBody] Cita cita)
        {
            if (cita == null)
                return BadRequest("La información de la cita es inválida.");

            try
            {
                bool resultado = CitasData.RegistrarCita(cita);
                if (resultado)
                    return Ok("Cita registrada correctamente.");
                else
                    return InternalServerError(new Exception("No se pudo registrar la cita."));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public static void Register(HttpConfiguration config)
        {
            // Habilitar rutas por atributos
            config.MapHttpAttributeRoutes();

            // Configuración de la ruta predeterminada
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        [HttpPut]
        [Route("actualizar")]
        public IHttpActionResult ActualizarCita([FromBody] Cita cita)
        {
            if (cita == null)
                return BadRequest("La información de la cita es inválida.");

            try
            {
                bool resultado = CitasData.ActualizarCita(cita);
                if (resultado)
                    return Ok("Cita actualizada correctamente.");
                else
                    return InternalServerError(new Exception("No se pudo actualizar la cita."));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("listar")]
        public IHttpActionResult ListarCita()
        {
            try
            {
                var citas = CitasData.ListarCita();
                return Ok(citas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("obtener/{id}")]
        public IHttpActionResult ObtenerCita(int id)
        {
            if (id <= 0)
                return BadRequest("El ID de la cita debe ser mayor a 0.");

            try
            {
                var cita = CitasData.ObtenerCita(id);
                if (cita == null)
                    return NotFound();

                return Ok(cita);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpDelete]
        [Route("eliminar/{id}")]
        public IHttpActionResult EliminarCita(int id)
        {
            try
            {
                bool resultado = CitasData.EliminarCita(id);
                if (resultado)
                    return Ok("Cita eliminada correctamente.");
                else
                    return InternalServerError(new Exception("No se pudo eliminar la cita."));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


    }
}
