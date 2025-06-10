using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Para capturar SqlException
using System.Net; // Para HttpStatusCode
using System.Net.Http; // Para HttpResponseMessage
using System.Web.Http; // Para ApiController, HttpGet, HttpPost, HttpPut, HttpDelete, Route

namespace App_Citas_medicas_backend.Controllers
{
    public class UsuarioController : ApiController
    {
        // GET api/Usuario
        [HttpGet]
        public List<Usuario> Get()
        {
            return UsuarioData.ListarUsuarios();
        }

        // GET api/Usuario/{id}
        [HttpGet]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage ObtenerUsuario(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Incluir 'success: false' en las respuestas de error
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Error: ID inválido." });
            }

            var usuarios = UsuarioData.ObtenerUsuario(id);

            if (usuarios == null || usuarios.Count == 0)
            {
                // Incluir 'success: false' en las respuestas de NotFound
                return Request.CreateResponse(HttpStatusCode.NotFound, new { success = false, mensaje = "Usuario no encontrado." });
            }

            // Para éxito, puedes devolver el objeto directamente si no necesitas 'success: true' aquí
            // O puedes envolverlo: new { success = true, usuarios = usuarios }
            return Request.CreateResponse(HttpStatusCode.OK, usuarios);
        }

        // POST api/Usuario
        [HttpPost]
        public IHttpActionResult Post([FromBody] Usuario oUsuario)
        {
            if (oUsuario == null)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "Datos de usuario no proporcionados." });
            }

            try
            {
                if (oUsuario.Rol?.ToLower() == "medico" && !oUsuario.EspecialidadId.HasValue)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "Para el rol 'Medico', la EspecialidadId es obligatoria." });
                }

                bool registrado = UsuarioData.RegistrarUsuario(oUsuario);

                if (registrado)
                {
                    return Ok(new { success = true, mensaje = "Usuario registrado correctamente." });
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "No se pudo registrar el usuario. Verifique los datos o si ya existe." });
                }
            }
            catch (SqlException sqlEx) // Captura excepciones específicas de SQL Server
            {
                Console.WriteLine($"Error SQL al registrar usuario en el controlador: {sqlEx.Message}");
                // ***** CAMBIO CLAVE AQUÍ: Usar Content() para InternalServerError *****
                return Content(HttpStatusCode.InternalServerError, new { success = false, mensaje = $"Error de base de datos al registrar usuario: {sqlEx.Message}" });
            }
            catch (Exception ex) // Captura cualquier otra excepción
            {
                Console.WriteLine($"Error general al registrar usuario en el controlador: {ex.Message}");
                // ***** CAMBIO CLAVE AQUÍ: Usar Content() para InternalServerError *****
                return Content(HttpStatusCode.InternalServerError, new { success = false, mensaje = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        // PUT api/Usuario/{id}
        // Este método ya usaba Request.CreateResponse, que es compatible con objetos anónimos.
        // Solo para asegurar, verifica que las llamadas a Request.CreateResponse sean correctas.
        [HttpPut]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage ActualizarUsuario(int id, [FromBody] Usuario oUsuario)
        {
            if (oUsuario == null || id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Error: Datos inválidos." });
            }

            oUsuario.Id = id;

            if (oUsuario.Rol?.ToLower() == "medico" && !oUsuario.EspecialidadId.HasValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Para el rol 'Medico', la EspecialidadId es obligatoria." });
            }

            bool actualizado = UsuarioData.ActualizarUsuario(oUsuario);

            if (actualizado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, mensaje = "Usuario actualizado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { success = false, mensaje = "Error al actualizar el usuario. Verifique los datos." });
            }
        }

        // DELETE api/Usuario/{id}
        // Este método ya usaba Request.CreateResponse, que es compatible con objetos anónimos.
        [HttpDelete]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage EliminarUsuario(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Error: ID de usuario inválido." });
            }

            bool eliminado = UsuarioData.EliminarUsuario(id);

            if (eliminado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, mensaje = "Usuario eliminado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { success = false, mensaje = "Error al eliminar el usuario o usuario no encontrado." });
            }
        }
    }
}