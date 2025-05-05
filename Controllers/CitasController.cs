using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    public class CitasController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        public List<Cita> Get()
        {
            return CitasData.ListarCitas();
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("api/Citas/{id}")]
        public HttpResponseMessage ConsultarCita(int id)
        {
            var cita = CitasData.ConsultarCita(id);

            if (cita == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new { mensaje = "Cita no encontrada." });
            }

            return Request.CreateResponse(HttpStatusCode.OK, cita);
        }

        // POST api/<controller>
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Cita oCita)
        {
            if (oCita == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: No se recibieron datos válidos." });
            }

            bool registrado = CitasData.RegistrarCita(oCita);

            if (registrado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Cita registrada correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al registrar la cita." });
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/Citas/{id}")]
        public HttpResponseMessage ActualizarCita(int id, [FromBody] Cita oCita)
        {
            if (oCita == null || id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: Datos inválidos." });
            }

            oCita.Id = id;
            bool actualizado = CitasData.ActualizarCita(oCita);

            if (actualizado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Cita actualizada correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al actualizar la cita." });
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("api/Citas/{id}")]
        public HttpResponseMessage EliminarCita(int id)
        {
            if (id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: ID inválido." });
            }

            bool eliminado = CitasData.EliminarCita(id);

            if (eliminado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Cita eliminada correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al eliminar la cita." });
            }
        }
    }
}

