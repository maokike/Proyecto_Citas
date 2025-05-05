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
        public HttpResponseMessage Post([FromBody] Usuario oUsuario)
        {
            if (oUsuario == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: No se recibieron datos válidos." });
            }

            bool registrado = UsuarioData.RegistrarUsuario(oUsuario);

            if (registrado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Usuario ok"});
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al registrar." });
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
