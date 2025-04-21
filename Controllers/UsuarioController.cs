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

        // GET api/<controller>/5
        public List<Usuario> Get(string id)
        {
            return UsuarioData.ObtenerUsuario(id);
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
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Usuario registrado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al registrar el usuario." });
            }
        }




        // PUT api/<controller>/5
       public bool Put([FromBody] Usuario oUsuario)
        {
            return UsuarioData.ActualizarUsuario(oUsuario);
        }


        // DELETE api/<controller>/5

        public bool Delete(string id)
        {
            return UsuarioData.EliminarUsuario(id);
        }


       

        private IHttpActionResult BadRequest(object value)
        {
            throw new NotImplementedException();
        }

    }


}
