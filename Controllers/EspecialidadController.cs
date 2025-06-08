
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
  
    [RoutePrefix("api/Especialidad")]
    public class EspecialidadController : ApiController
    {
        // GET api/Especialidad
        [HttpGet]
        [Route("")] 
        public IHttpActionResult Get()
        {
            try
            {
                List<Especialidad> especialidades = EspecialidadData.ListarEspecialidades();
                // Si la lista está vacía, aún devuelve 200 OK con una lista vacía, lo cual es estándar para GET
                return Ok(especialidades);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al listar especialidades: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al listar especialidades: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al listar especialidades: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // GET api/Especialidad/{id}
        [HttpGet]
        [Route("{id:int}")] // Restringe el parámetro 'id' a ser un entero
        public IHttpActionResult Get(int id)
        {
            try
            {
                Especialidad especialidad = EspecialidadData.ObtenerEspecialidad(id);
                if (especialidad == null)
                {
                    return NotFound(); // Código 404 si no se encuentra
                }
                return Ok(especialidad);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al obtener especialidad por ID: {sqlEx.Message}");
                return InternalServerError(new Exception($"Error de base de datos al obtener especialidad: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al obtener especialidad por ID: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // POST api/Especialidad
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] Especialidad oEspecialidad)
        {
            if (oEspecialidad == null || string.IsNullOrWhiteSpace(oEspecialidad.Nombre))
            {
                return BadRequest("El nombre de la especialidad es obligatorio.");
            }

            try
            {
                bool registrado = EspecialidadData.RegistrarEspecialidad(oEspecialidad);
                if (registrado)
                {
                    // Devolver 201 Created es una buena práctica para POST, e incluir el recurso creado si es posible.
                    // Aquí, como no recuperamos el ID generado, simplemente Ok.
                    return Ok(new { mensaje = "Especialidad registrada correctamente." });
                }
                else
                {
                    return InternalServerError(new Exception("No se pudo registrar la especialidad."));
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al registrar especialidad: {sqlEx.Message}");
                
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave primaria/única duplicada
                {
                    return Conflict(); // 409 Conflict
                }
                return InternalServerError(new Exception($"Error de base de datos al registrar especialidad: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al registrar especialidad: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // PUT api/Especialidad/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] Especialidad oEspecialidad)
        {
            if (oEspecialidad == null || id <= 0 || string.IsNullOrWhiteSpace(oEspecialidad.Nombre))
            {
                return BadRequest("Datos inválidos para actualizar la especialidad.");
            }

            
            oEspecialidad.Id = id;

            try
            {
                bool actualizado = EspecialidadData.ActualizarEspecialidad(oEspecialidad);
                if (actualizado)
                {
                    return Ok(new { mensaje = "Especialidad actualizada correctamente." });
                }
                else
                {
                    // Podría ser NotFound si el ID no existe, o InternalServerError si falla la DB
                    return NotFound(); // O InternalServerError si sabes que el problema es del lado del servidor
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al actualizar especialidad: {sqlEx.Message}");
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave única duplicada
                {
                    return Conflict(); // 409 Conflict
                }
                return InternalServerError(new Exception($"Error de base de datos al actualizar especialidad: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al actualizar especialidad: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        // DELETE api/Especialidad/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID de especialidad inválido.");
            }

            try
            {
                bool eliminado = EspecialidadData.EliminarEspecialidad(id);
                if (eliminado)
                {
                    return Ok(new { mensaje = "Especialidad eliminada correctamente." });
                }
                else
                {
                   
                    return NotFound();
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al eliminar especialidad: {sqlEx.Message}");
               
                if (sqlEx.Number == 547) // Error de restricción de clave foránea
                {
                    return Conflict(); // 409 Conflict - indica que el recurso no se puede eliminar porque está en uso
                }
                return InternalServerError(new Exception($"Error de base de datos al eliminar especialidad: {sqlEx.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al eliminar especialidad: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
}