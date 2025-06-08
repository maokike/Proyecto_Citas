// App_Citas_medicas_backend\Controllers\MedicoController.cs
using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Para capturar SqlException
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    // Define la ruta base para este controlador. Será /api/Medico
    [RoutePrefix("api/Medico")]
    public class MedicoController : ApiController
    {
        // GET api/Medico
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            try
            {
                List<Medico> medicos = MedicoData.ListarMedicos();
                return Ok(medicos);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar médicos: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar médicos: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar médicos: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Medico/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                Medico medico = MedicoData.ObtenerMedico(id);
                if (medico == null)
                {
                    return NotFound(); // Código 404 si no se encuentra
                }
                return Ok(medico);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al obtener médico por ID: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al obtener médico: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al obtener médico por ID: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // POST api/Medico
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Medico oMedico)
        {
            // Validaciones básicas
            if (oMedico == null || string.IsNullOrWhiteSpace(oMedico.Nombre) ||
                string.IsNullOrWhiteSpace(oMedico.Apellido) ||
                oMedico.EspecialidadId <= 0 || string.IsNullOrWhiteSpace(oMedico.Email) ||
                string.IsNullOrWhiteSpace(oMedico.Contrasena) || oMedico.Cedula <= 0)
            {
                return BadRequest("Datos incompletos o inválidos para registrar el médico.");
            }

            try
            {
                bool registrado = MedicoData.RegistrarMedico(oMedico);
                if (registrado)
                {
                    return Ok(new { mensaje = "Médico registrado correctamente." });
                }
                else
                {
                    // Si el SP devuelve -1 por alguna validación interna o duplicado no capturado como excepción
                    return BadRequest("No se pudo registrar el médico. Posiblemente ya exista una cédula o email.");
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al registrar médico: {sqlEx.Message}");
                // Puedes añadir lógica para detectar errores específicos (ej. duplicados)
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave primaria/única duplicada
                {
                    return Conflict(); // 409 Conflict
                }
                return InternalServerError(new Exception($"Error de base de datos al registrar médico: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al registrar médico: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // PUT api/Medico/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] Medico oMedico)
        {
            // Validaciones básicas
            if (oMedico == null || id <= 0 || string.IsNullOrWhiteSpace(oMedico.Nombre) ||
                string.IsNullOrWhiteSpace(oMedico.Apellido) ||
                oMedico.EspecialidadId <= 0 || string.IsNullOrWhiteSpace(oMedico.Email))
            {
                return BadRequest("Datos incompletos o inválidos para actualizar el médico.");
            }

            oMedico.Id = id; // Asegura que el ID del objeto coincida con el de la ruta

            try
            {
                bool actualizado = MedicoData.ActualizarMedico(oMedico);
                if (actualizado)
                {
                    return Ok(new { mensaje = "Médico actualizado correctamente." });
                }
                else
                {
                    // Si el SP devuelve -1 (médico no encontrado) o -2 (error general)
                    // Si el SP no lanza excepción, el 'false' aquí podría ser por médico no encontrado.
                    // Si el SP lanza excepción, se capturaría en el catch.
                    // Por simplicidad, si no hay excepción pero devuelve false, asumimos no encontrado.
                    return NotFound();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al actualizar médico: {sqlEx.Message}");
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave única duplicada (ej. email)
                {
                    return Conflict();
                }
                return InternalServerError(new Exception($"Error de base de datos al actualizar médico: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al actualizar médico: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // DELETE api/Medico/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de médico inválido.");
            }

            try
            {
                bool eliminado = MedicoData.EliminarMedico(id);
                if (eliminado)
                {
                    return Ok(new { mensaje = "Médico eliminado correctamente." });
                }
                else
                {
                    return NotFound(); // Médico no encontrado para eliminar
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al eliminar médico: {sqlEx.Message}");
                // Puedes añadir lógica para detectar si la eliminación falla por una clave foránea (error 547)
                if (sqlEx.Number == 547) // Error de restricción de clave foránea
                {
                    return Conflict(); // 409 Conflict - indica que el recurso no se puede eliminar porque está en uso
                }
                return InternalServerError(new Exception($"Error de base de datos al eliminar médico: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al eliminar médico: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
}