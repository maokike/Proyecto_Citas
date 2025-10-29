using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    [RoutePrefix("api/Paciente")]
    public class PacienteController : ApiController
    {
        [HttpGet]
        [Route("{id}/ResumenCitas")]
        public IHttpActionResult ObtenerResumenCitas(int id)
        {
            try
            {
                ResumenCitas resumen = ResumenCitasData.ObtenerResumenCitas(id);

                return Ok(new
                {
                    totalCitas = resumen.TotalCitas,
                    proximaCita = resumen.ProximaCita
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
