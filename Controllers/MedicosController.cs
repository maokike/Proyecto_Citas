using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    public class MedicosController : ApiController
    {
        [HttpGet]
        public List<Medicos> Get()
        {
            return MedicosData.ListarMedicos();
        }

        [HttpGet]
        [Route("api/Medicos/{id}")]
        public HttpResponseMessage Get(int id)
        {
            var medico = MedicosData.ConsultarMedico(id);
            if (medico == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { mensaje = "Médico no encontrado." });

            return Request.CreateResponse(HttpStatusCode.OK, medico);
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Medicos oMedicos)
        {
            if (oMedicos == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: No se recibieron datos válidos." });

            bool registrado = MedicosData.RegistrarMedico(oMedicos);

            if (registrado)
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Médico registrado correctamente." });
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al registrar el médico." });
        }

        [HttpPut]
        [Route("api/Medicos/{id}")]
        public HttpResponseMessage Put(int id, [FromBody] Medicos oMedicos)
        {
            if (oMedicos == null || id <= 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { mensaje = "Error: Datos inválidos." });

            oMedicos.Id = id;
            bool actualizado = MedicosData.ActualizarMedico(oMedicos);

            if (actualizado)
                return Request.CreateResponse(HttpStatusCode.OK, new { mensaje = "Médico actualizado correctamente." });
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensaje = "Error al actualizar el médico." });
        }
    }
}
