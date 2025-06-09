
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace App_Citas_medicas_backend.Data
{
    public class NotificacionData
    {
        // Replicamos EjecutarSentenciaConDetalle si no es accesible directamente
        private static bool EjecutarSentenciaConDetalle(ConexionBD objEst, string sentencia)
        {
            try
            {
                bool resultado = objEst.EjecutarSentencia(sentencia, false);
                return resultado;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"ERROR SQL al ejecutar sentencia de Notificacion: {sentencia}");
                Console.WriteLine($"Mensaje SQL: {sqlEx.Message}");
                Console.WriteLine($"Número de Error SQL: {sqlEx.Number}");
                Console.WriteLine($"Línea: {sqlEx.LineNumber}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL al ejecutar sentencia de Notificacion: {sentencia}");
                Console.WriteLine($"Mensaje General: {ex.Message}");
                throw;
            }
        }

        // Implementación de RegistrarNotificacion
        public static bool RegistrarNotificacion(Notificacion oNotificacion)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string fechaEnvioParam = oNotificacion.FechaEnvio.ToString("yyyy-MM-dd HH:mm:ss");

                string sentencia = $"EXEC RegistrarNotificacion " +
                                   $"{oNotificacion.PacienteId}, " +
                                   $"'{oNotificacion.Mensaje}', " +
                                   $"'{oNotificacion.Estado}', " +
                                   $"'{fechaEnvioParam}';";

                Console.WriteLine("Ejecutando SQL (RegistrarNotificacion): " + sentencia);

                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en RegistrarNotificacion (NotificacionData): " + ex.Message);
                throw;
            }
        }

        // Implementación de ActualizarNotificacion
        public static bool ActualizarNotificacion(Notificacion oNotificacion)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string fechaEnvioParam = oNotificacion.FechaEnvio.ToString("yyyy-MM-dd HH:mm:ss");

                string sentencia = $"EXEC ActualizarNotificacion " +
                                   $"{oNotificacion.Id}, " + // @NotificacionId
                                   $"{oNotificacion.PacienteId}, " +
                                   $"'{oNotificacion.Mensaje}', " +
                                   $"'{oNotificacion.Estado}', " +
                                   $"'{fechaEnvioParam}';";

                Console.WriteLine("Ejecutando SQL (ActualizarNotificacion): " + sentencia);

                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en ActualizarNotificacion (NotificacionData): " + ex.Message);
                throw;
            }
        }

        // Implementación de ConsultarNotificacion (por ID)
        public static Notificacion ObtenerNotificacion(int notificacionId) // Usamos ObtenerNotificacion para el método C#
        {
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"EXECUTE ConsultarNotificacion {notificacionId};";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                if (dr.Read())
                {
                    Notificacion notificacion = new Notificacion()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        Mensaje = dr["Mensaje"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        FechaEnvio = dr["FechaEnvio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaEnvio"]) : DateTime.MinValue
                    };
                    dr.Close();
                    return notificacion;
                }
                dr.Close();
            }
            return null; // Notificación no encontrada
        }

        // Implementación de ListarNotificaciones
        public static List<Notificacion> ListarNotificaciones()
        {
            List<Notificacion> listaNotificaciones = new List<Notificacion>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXECUTE ListarNotificaciones;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaNotificaciones.Add(new Notificacion()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        Mensaje = dr["Mensaje"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        FechaEnvio = dr["FechaEnvio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaEnvio"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }
            return listaNotificaciones;
        }

        // SUGERENCIA: Método para listar notificaciones por paciente
        public static List<Notificacion> ListarNotificacionesPorPaciente(int pacienteId)
        {
            List<Notificacion> listaNotificaciones = new List<Notificacion>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"SELECT Id, PacienteId, Mensaje, Estado, FechaEnvio FROM Notificaciones WHERE PacienteId = {pacienteId} ORDER BY FechaEnvio DESC;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaNotificaciones.Add(new Notificacion()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        Mensaje = dr["Mensaje"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        FechaEnvio = dr["FechaEnvio"] != DBNull.Value ? Convert.ToDateTime(dr["FechaEnvio"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }
            return listaNotificaciones;
        }

        // SUGERENCIA: Método para eliminar notificaciones antiguas o marcar como leídas
        public static bool EliminarNotificacion(int notificacionId)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string sentencia = $"DELETE FROM Notificaciones WHERE Id = {notificacionId};";
                Console.WriteLine("Ejecutando SQL (EliminarNotificacion): " + sentencia);
                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en EliminarNotificacion (NotificacionData): " + ex.Message);
                throw;
            }
        }
    }
}