
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
    [RoutePrefix("api/Cita")]
    public class CitaController : ApiController
    {
        // GET api/Cita
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                List<Cita> citas = CitaData.ListarCitas();
                return Ok(citas);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar citas: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar citas: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar citas: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Cita/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                Cita cita = CitaData.ObtenerCita(id);
                if (cita == null)
                {
                    return NotFound();
                }
                return Ok(cita);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al obtener cita por ID: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al obtener cita: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al obtener cita por ID: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Cita/Paciente/{pacienteId}
        [HttpGet]
        [Route("Paciente/{pacienteId:int}")]
        public IHttpActionResult GetCitasPorPaciente(int pacienteId)
        {
            try
            {
                List<Cita> citas = CitaData.ListarCitasPorPaciente(pacienteId);
                return Ok(citas);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar citas por paciente: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar citas por paciente: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar citas por paciente: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Cita/Medico/{medicoId}
        [HttpGet]
        [Route("Medico/{medicoId:int}")]
        public IHttpActionResult GetCitasPorMedico(int medicoId)
        {
            try
            {
                List<Cita> citas = CitaData.ListarCitasPorMedico(medicoId);
                return Ok(citas);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar citas por médico: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar citas por médico: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar citas por médico: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Cita/Disponibilidad?especialidadId={id}&fecha={fecha}
        [HttpGet]
        [Route("Disponibilidad")]
        public IHttpActionResult GetCitasDisponibilidad(int especialidadId, string fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
            {
                return BadRequest("La fecha es obligatoria para buscar disponibilidad.");
            }
            if (!DateTime.TryParse(fecha, out DateTime parsedDate))
            {
                return BadRequest("Formato de fecha inválido. Use YYYY-MM-DD.");
            }

            try
            {
                // Este método te devolverá las citas ya ocupadas para esa especialidad y fecha
                List<Cita> citasOcupadas = CitaData.ListarCitasPorEspecialidadYFecha(especialidadId, parsedDate);

                // Aquí podrías implementar lógica para generar los horarios disponibles
                // Por ejemplo, si los médicos trabajan de 8 AM a 5 PM, y cada cita dura 30 mins:
                // Generar todos los slots y remover los que están en 'citasOcupadas'.
                // Por ahora, solo devolvemos las citas ocupadas, lo que permite al frontend saber qué horas no están libres.
                return Ok(citasOcupadas);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al buscar disponibilidad de citas: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al buscar disponibilidad de citas: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al buscar disponibilidad de citas: {ex.Message}");
                return InternalServerError(ex);
            }
        }


        // POST api/Cita
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Cita oCita)
        {
            // Validaciones básicas
            if (oCita == null || oCita.PacienteId <= 0 || oCita.MedicoId <= 0 ||
                oCita.EspecialidadId <= 0 || oCita.Fecha == DateTime.MinValue ||
                oCita.Hora == TimeSpan.Zero || string.IsNullOrWhiteSpace(oCita.Estado))
            {
                return BadRequest("Datos incompletos o inválidos para registrar la cita.");
            }

            try
            {
                bool registrado = CitaData.RegistrarCita(oCita);
                if (registrado)
                {
                    return Ok(new { mensaje = "Cita registrada correctamente." });
                }
                else
                {
                    return BadRequest("No se pudo registrar la cita. Verifique si el paciente o médico existen y tienen el rol correcto.");
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al registrar cita: {sqlEx.Message}");
                // Puedes añadir lógica para detectar errores específicos (ej. duplicados, conflictos de horario)
                return InternalServerError(new Exception($"Error de base de datos al registrar cita: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al registrar cita: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // PUT api/Cita/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] Cita oCita)
        {
            // Validaciones básicas
            if (oCita == null || id <= 0 || oCita.PacienteId <= 0 || oCita.MedicoId <= 0 ||
                oCita.EspecialidadId <= 0 || oCita.Fecha == DateTime.MinValue ||
                oCita.Hora == TimeSpan.Zero || string.IsNullOrWhiteSpace(oCita.Estado))
            {
                return BadRequest("Datos incompletos o inválidos para actualizar la cita.");
            }

            oCita.Id = id; // Asegura que el ID del objeto coincida con el de la ruta

            try
            {
                bool actualizado = CitaData.ActualizarCita(oCita);
                if (actualizado)
                {
                    return Ok(new { mensaje = "Cita actualizada correctamente." });
                }
                else
                {
                    // Si el SP devuelve 0 filas afectadas, es probable que la cita no exista.
                    return NotFound();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al actualizar cita: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al actualizar cita: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al actualizar cita: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // DELETE api/Cita/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de cita inválido.");
            }

            try
            {
                bool eliminado = CitaData.CancelarCita(id);
                if (eliminado)
                {
                    return Ok(new { mensaje = "Cita eliminada correctamente." });
                }
                else
                {
                    return NotFound(); // Cita no encontrada para eliminar
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al eliminar cita: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al eliminar cita: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al eliminar cita: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
}