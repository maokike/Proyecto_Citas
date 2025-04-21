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
    public class MedicosController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Medicos oMedicos)
        {
            if (oMedicos == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: No se recibieron datos válidos." });
            }

            bool registrado = MedicosData.RegistrarMedico(oMedicos);

            if (registrado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Medico registrado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al registrar el Medico." });
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}