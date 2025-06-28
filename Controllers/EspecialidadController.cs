
using App_Citas_medicas_backend.Data;
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

public class Especialidad
{
    public int Id { get; set; }
    public string Nombre { get; set; }
}

[RoutePrefix("api/Especialidad")]
public class EspecialidadController : ApiController
{
    private readonly string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    // GET api/Especialidad
    [HttpGet]
    [Route("")]
    public IHttpActionResult GetAll()
    {
        var result = new List<Especialidad>();
        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("dbo.ListarEspecialidades", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Especialidad
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
            }
        }
        return Ok(result);
    }

    // GET api/Especialidad/5
    [HttpGet]
    [Route("{id:int}")]
    public IHttpActionResult GetById(int id)
    {
        Especialidad especialidad = null;
        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("dbo.ConsultarEspecialidad", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdEspecialidad", id);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    especialidad = new Especialidad
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString()
                    };
                }
            }
        }
        if (especialidad == null) return NotFound();
        return Ok(especialidad);
    }

    // POST api/Especialidad
    [HttpPost]
    [Route("")]
    public IHttpActionResult Create([FromBody] Especialidad model)
    {
        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("dbo.RegistrarEspecialidad", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        return Ok(new { message = "Especialidad registrada" });
    }

    // PUT api/Especialidad/5
    [HttpPut]
    [Route("{id:int}")]
    public IHttpActionResult Update(int id, [FromBody] Especialidad model)
    {
        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("dbo.ActualizarEspecialidad", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdEspecialidad", id);
            cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        return Ok(new { message = "Especialidad actualizada" });
    }

    // DELETE api/Especialidad/5
    [HttpDelete]
    [Route("{id:int}")]
    public IHttpActionResult Delete(int id)
    {
        using (var conn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand("dbo.EliminarEspecialidad", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EspecialidadId", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        return Ok(new { message = "Especialidad eliminada" });
    }
}