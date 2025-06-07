using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    
    public class UsuarioController : ApiController
    {
        // GET api/<controller>
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





        // POST api/<controller>
        [HttpPost]
        public IHttpActionResult Post([FromBody] Usuario oUsuario)
        {
            if (oUsuario == null)
            {
                // Si los datos del usuario son nulos, devuelve un Bad Request
                return BadRequest(); // HttpStatusCode 400
            }

            try
            {
                // Llama al método RegistrarUsuario de tu capa de datos
                bool registrado = UsuarioData.RegistrarUsuario(oUsuario);

                if (registrado)
                {
                    // Si el registro fue exitoso en la capa de datos, devuelve 200 OK
                    // El frontend interpretará esto como éxito y mostrará su mensaje.
                    return Ok(); // HttpStatusCode 200
                }
                else
                {
                    // Si UsuarioData.RegistrarUsuario devuelve 'false' (ej. usuario ya existe, validación interna fallida)
                    // Devolvemos un BadRequest o Conflict (409) si la lógica del backend es que el usuario ya existe.
                    // Para simplificar al máximo, podemos devolver Bad Request o InternalServerError si la inserción no ocurrió.
                    // Aquí, asumiremos que 'false' significa que la base de datos no insertó por una razón lógica.
                    return BadRequest(); // HttpStatusCode 400
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción durante el proceso (ej. error de base de datos)
                Console.WriteLine($"Error durante el registro de usuario en el controlador: {ex.Message}");
                // Devuelve 500 Internal Server Error al frontend
                return InternalServerError(ex); // HttpStatusCode 500
            }
        }




        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage ActualizarUsuario(int id, [FromBody] Usuario oUsuario)
        {
            if (oUsuario == null || id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: Datos inválidos." });
            }

            oUsuario.Id = id; // Aseguramos que el ID del usuario coincide con el proporcionado en la ruta.
            bool actualizado = UsuarioData.ActualizarUsuario(oUsuario);

            if (actualizado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Usuario actualizado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al actualizar el usuario." });
            }
        }





        private IHttpActionResult BadRequest(object value)
        {
            throw new NotImplementedException();
        }

    }


}
