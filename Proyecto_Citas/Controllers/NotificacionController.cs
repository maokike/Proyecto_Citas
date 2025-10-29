
using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    // Define la ruta base para este controlador. Será /api/Notificacion
    [RoutePrefix("api/Notificacion")]
    public class NotificacionController : ApiController
    {
        // GET api/Notificacion
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                List<Notificacion> notificaciones = NotificacionData.ListarNotificaciones();
                return Ok(notificaciones);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar notificaciones: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar notificaciones: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar notificaciones: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Notificacion/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                Notificacion notificacion = NotificacionData.ObtenerNotificacion(id);
                if (notificacion == null)
                {
                    return NotFound();
                }
                return Ok(notificacion);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al obtener notificación por ID: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al obtener notificación: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al obtener notificación por ID: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Notificacion/Paciente/{pacienteId}
        // Este endpoint es útil para que un paciente consulte sus propias notificaciones.
        [HttpGet]
        [Route("Paciente/{pacienteId:int}")]
        public IHttpActionResult GetNotificacionesPorPaciente(int pacienteId)
        {
            try
            {
                List<Notificacion> notificaciones = NotificacionData.ListarNotificacionesPorPaciente(pacienteId);
                return Ok(notificaciones);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar notificaciones por paciente: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar notificaciones por paciente: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar notificaciones por paciente: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // PUT api/Notificacion/{id}
        // Útil para marcar una notificación como "Leída" sin cambiar el resto de sus datos.
        // O para actualizar el estado de "Enviada" a "Fallida" si el correo no se pudo enviar.
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] Notificacion oNotificacion)
        {
            if (oNotificacion == null || id <= 0 || string.IsNullOrWhiteSpace(oNotificacion.Mensaje) || string.IsNullOrWhiteSpace(oNotificacion.Estado))
            {
                return BadRequest("Datos incompletos o inválidos para actualizar la notificación.");
            }

            oNotificacion.Id = id; // Asegura que el ID del objeto coincida con el de la ruta

            try
            {
                bool actualizado = NotificacionData.ActualizarNotificacion(oNotificacion);
                if (actualizado)
                {
                    return Ok(new { mensaje = "Notificación actualizada correctamente." });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al actualizar notificación: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al actualizar notificación: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al actualizar notificación: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // DELETE api/Notificacion/{id}
        // Para limpiar notificaciones antiguas o ya procesadas.
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de notificación inválido.");
            }

            try
            {
                bool eliminado = NotificacionData.EliminarNotificacion(id);
                if (eliminado)
                {
                    return Ok(new { mensaje = "Notificación eliminada correctamente." });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al eliminar notificación: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al eliminar notificación: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al eliminar notificación: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
}