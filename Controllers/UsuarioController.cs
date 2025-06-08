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
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: ID inválido." });
            }

            var usuarios = UsuarioData.ObtenerUsuario(id);

            if (usuarios == null || usuarios.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new { mensaje = "Usuario no encontrado." });
            }

            return Request.CreateResponse(HttpStatusCode.OK, usuarios);
        }

        // POST api/Usuario
        [HttpPost]
        public IHttpActionResult Post([FromBody] Usuario oUsuario)
        {
            if (oUsuario == null)
            {
                return BadRequest("Datos de usuario no proporcionados.");
            }

            try
            {
                // Validación en el controlador: si es médico, EspecialidadId es obligatorio
                if (oUsuario.Rol?.ToLower() == "medico" && !oUsuario.EspecialidadId.HasValue)
                {
                    return BadRequest("Para el rol 'Medico', la EspecialidadId es obligatoria.");
                }

                bool registrado = UsuarioData.RegistrarUsuario(oUsuario);

                if (registrado)
                {
                    return Ok(new { mensaje = "Usuario registrado correctamente." });
                }
                else
                {
                    // Si la capa de datos devuelve false sin lanzar excepción
                    return BadRequest("No se pudo registrar el usuario. Verifique los datos o si ya existe.");
                }
            }
            catch (SqlException sqlEx) // Captura excepciones específicas de SQL Server
            {
                Console.WriteLine($"Error SQL al registrar usuario en el controlador: {sqlEx.Message}");
                // Devuelve un mensaje de error detallado de SQL Server
                return InternalServerError(new Exception($"Error de base de datos al registrar usuario: {sqlEx.Message}"));
            }
            catch (Exception ex) // Captura cualquier otra excepción
            {
                Console.WriteLine($"Error general al registrar usuario en el controlador: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // PUT api/Usuario/{id}
        [HttpPut]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage ActualizarUsuario(int id, [FromBody] Usuario oUsuario)
        {
            if (oUsuario == null || id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: Datos inválidos." });
            }

            oUsuario.Id = id; // Asegura que el ID del usuario coincide con el proporcionado en la ruta.

            // Validación en el controlador: si es médico, EspecialidadId es obligatorio
            if (oUsuario.Rol?.ToLower() == "medico" && !oUsuario.EspecialidadId.HasValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Para el rol 'Medico', la EspecialidadId es obligatoria." });
            }

            bool actualizado = UsuarioData.ActualizarUsuario(oUsuario);

            if (actualizado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Usuario actualizado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al actualizar el usuario. Verifique los datos." });
            }
        }

        // DELETE api/Usuario/{id}
        [HttpDelete]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage EliminarUsuario(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: ID de usuario inválido." });
            }

            bool eliminado = UsuarioData.EliminarUsuario(id);

            if (eliminado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Usuario eliminado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al eliminar el usuario o usuario no encontrado." });
            }
        }
    }
}