using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Data
{
    public class EspecialidadData
    {

        private static bool EjecutarSentencia(string sentencia)
        {
            ConexionBD objEst = new ConexionBD();
            try
            {
                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                return resultado;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"ERROR SQL al ejecutar sentencia de Especialidad: {sentencia}");
                Console.WriteLine($"Mensaje SQL: {sqlEx.Message}");
                Console.WriteLine($"Número de Error SQL: {sqlEx.Number}");
                Console.WriteLine($"Línea: {sqlEx.LineNumber}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL al ejecutar sentencia de Especialidad: {sentencia}");
                Console.WriteLine($"Mensaje General: {ex.Message}");
                throw;
            }
        }


        public static List<Especialidad> ListarEspecialidades()
        {
            List<Especialidad> listaEspecialidades = new List<Especialidad>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXECUTE ListarEspecialidades;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaEspecialidades.Add(new Especialidad()
                    {
                        // Asegúrate de que los nombres de las columnas ('Id', 'Nombre')
                        // coincidan exactamente con los devueltos por tu SP ListarEspecialidades
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Nombre = dr["Nombre"]?.ToString() // Usa ?.ToString() para manejar DBNull.Value
                    });
                }
                dr.Close();
            }
            // objEst = null; // Opcional, el recolector de basura lo gestionará.
            return listaEspecialidades;
        }

        // Método para obtener una especialidad por su ID (usa ConsultarEspecialidad)
        public static Especialidad ObtenerEspecialidad(int id)
        {
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"EXECUTE ConsultarEspecialidad {id};"; // Sin comillas para el INT id

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                if (dr.Read()) // Solo esperamos una fila
                {
                    Especialidad especialidad = new Especialidad()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Nombre = dr["Nombre"]?.ToString()
                    };
                    dr.Close();
                    return especialidad;
                }
                dr.Close();
            }
            return null; // Especialidad no encontrada
        }

        // Método para registrar una nueva especialidad (usa RegistrarEspecialidad)
        public static bool RegistrarEspecialidad(Especialidad oEspecialidad)
        {
            // Nota: Tu SP RegistrarEspecialidad solo recibe @Nombre. Id es auto-incremental.
            string sentencia = $"EXEC RegistrarEspecialidad '{oEspecialidad.Nombre}';";
            Console.WriteLine("Ejecutando SQL (RegistrarEspecialidad): " + sentencia);
            return EjecutarSentencia(sentencia);
        }

        // Método para actualizar una especialidad existente (usa ActualizarEspecialidad)
        public static bool ActualizarEspecialidad(Especialidad oEspecialidad)
        {
            // Tu SP ActualizarEspecialidad recibe @IdEspecialidad y @Nombre
            string sentencia = $"EXEC ActualizarEspecialidad {oEspecialidad.Id}, '{oEspecialidad.Nombre}';";
            Console.WriteLine("Ejecutando SQL (ActualizarEspecialidad): " + sentencia);
            return EjecutarSentencia(sentencia);
        }

        // Método para eliminar una especialidad (usa EliminarEspecialidad)
        public static bool EliminarEspecialidad(int id)
        {
            string sentencia = $"EXEC EliminarEspecialidad {id};";
            Console.WriteLine("Ejecutando SQL (EliminarEspecialidad): " + sentencia);
            return EjecutarSentencia(sentencia);
        }
    }
}