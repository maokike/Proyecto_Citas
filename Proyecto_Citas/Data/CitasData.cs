
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq; // Para usar .FirstOrDefault() si es necesario
using System.Web;

namespace App_Citas_medicas_backend.Data
{
    public class CitaData
    {
        // Replicamos EjecutarSentenciaConDetalle si no es accesible directamente
        // Asegúrate de que este método sea compartido o esté accesible desde aquí.
        private static bool EjecutarSentenciaConDetalle(ConexionBD objEst, string sentencia)
        {
            try
            {
                bool resultado = objEst.EjecutarSentencia(sentencia, false);
                return resultado;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"ERROR SQL al ejecutar sentencia de Cita: {sentencia}");
                Console.WriteLine($"Mensaje SQL: {sqlEx.Message}");
                Console.WriteLine($"Número de Error SQL: {sqlEx.Number}");
                Console.WriteLine($"Línea: {sqlEx.LineNumber}");
                throw; // Relanzar la excepción para que sea capturada por el catch superior
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL al ejecutar sentencia de Cita: {sentencia}");
                Console.WriteLine($"Mensaje General: {ex.Message}");
                throw; // Relanzar la excepción
            }
        }


        // Implementación de RegistrarCita
        public static bool RegistrarCita(Cita oCita)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                // Formato para Fecha (DATE) y Hora (TIME) en SQL
                string fechaParam = oCita.Fecha.ToString("yyyy-MM-dd");
                string horaParam = oCita.Hora.ToString(@"hh\:mm\:ss"); // Formato HH:mm:ss

                string sentencia = $"EXEC RegistrarCita " +
                                   $"{oCita.PacienteId}, " +
                                   $"{oCita.MedicoId}, " +
                                   $"{oCita.EspecialidadId}, " +
                                   $"'{fechaParam}', " +
                                   $"'{horaParam}', " +
                                   $"'{oCita.Estado}';";

                Console.WriteLine("Ejecutando SQL (RegistrarCita): " + sentencia);

                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en RegistrarCita (CitaData): " + ex.Message);
                throw;
            }
        }

        // Implementación de ActualizarCita
        public static bool ActualizarCita(Cita oCita)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string fechaParam = oCita.Fecha.ToString("yyyy-MM-dd");
                string horaParam = oCita.Hora.ToString(@"hh\:mm\:ss");

                string sentencia = $"EXEC ActualizarCita " +
                                   $"{oCita.Id}, " + // @CitaId
                                   $"{oCita.PacienteId}, " +
                                   $"{oCita.MedicoId}, " +
                                   $"{oCita.EspecialidadId}, " +
                                   $"'{fechaParam}', " +
                                   $"'{horaParam}', " +
                                   $"'{oCita.Estado}';";

                Console.WriteLine("Ejecutando SQL (ActualizarCita): " + sentencia);

                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en ActualizarCita (CitaData): " + ex.Message);
                throw;
            }
        }

        // Implementación de ListarCitas
        public static List<Cita> ListarCitas()
        {
            List<Cita> listaCitas = new List<Cita>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXECUTE ListarCitas;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaCitas.Add(new Cita()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        MedicoId = dr["MedicoId"] != DBNull.Value ? Convert.ToInt32(dr["MedicoId"]) : 0,
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Fecha = dr["Fecha"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha"]) : DateTime.MinValue,
                        // Para Hora, leer como TimeSpan. Si SQL Server almacena como time(7), es compatible.
                        Hora = dr["Hora"] != DBNull.Value ? (TimeSpan)dr["Hora"] : TimeSpan.Zero,
                        Estado = dr["Estado"]?.ToString()
                    });
                }
                dr.Close();
            }
            return listaCitas;
        }

        // Implementación de ConsultarCita (por ID)
        public static Cita ObtenerCita(int citaId) // Usamos ObtenerCita para el método C#
        {
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"EXECUTE ConsultarCita {citaId};"; // Llamamos a tu SP ConsultarCita

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                if (dr.Read())
                {
                    Cita cita = new Cita()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        MedicoId = dr["MedicoId"] != DBNull.Value ? Convert.ToInt32(dr["MedicoId"]) : 0,
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Fecha = dr["Fecha"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha"]) : DateTime.MinValue,
                        Hora = dr["Hora"] != DBNull.Value ? (TimeSpan)dr["Hora"] : TimeSpan.Zero,
                        Estado = dr["Estado"]?.ToString()
                    };
                    dr.Close();
                    return cita;
                }
                dr.Close();
            }
            return null; // Cita no encontrada
        }

        // Implementación de CancelarCita
        public static bool CancelarCita(int citaId)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string sentencia = $"EXEC CancelarCita {citaId};";
                Console.WriteLine("Ejecutando SQL (CancelarCita): " + sentencia);
                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en CancelarCita (CitaData): " + ex.Message);
                throw;
            }
        }

        // SUGERENCIA: Métodos adicionales para gestión de citas

        // Listar citas por Paciente (para el historial de citas del paciente)
        public static List<Cita> ListarCitasPorPaciente(int pacienteId)
        {
            List<Cita> listaCitas = new List<Cita>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"SELECT Id, PacienteId, MedicoId, EspecialidadId, Fecha, Hora, Estado FROM Citas WHERE PacienteId = {pacienteId} ORDER BY Fecha DESC, Hora DESC;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaCitas.Add(new Cita()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        MedicoId = dr["MedicoId"] != DBNull.Value ? Convert.ToInt32(dr["MedicoId"]) : 0,
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Fecha = dr["Fecha"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha"]) : DateTime.MinValue,
                        Hora = dr["Hora"] != DBNull.Value ? (TimeSpan)dr["Hora"] : TimeSpan.Zero,
                        Estado = dr["Estado"]?.ToString()
                    });
                }
                dr.Close();
            }
            return listaCitas;
        }

        // Listar citas por Médico (para la agenda del médico)
        public static List<Cita> ListarCitasPorMedico(int medicoId)
        {
            List<Cita> listaCitas = new List<Cita>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"SELECT Id, PacienteId, MedicoId, EspecialidadId, Fecha, Hora, Estado FROM Citas WHERE MedicoId = {medicoId} ORDER BY Fecha DESC, Hora DESC;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaCitas.Add(new Cita()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        MedicoId = dr["MedicoId"] != DBNull.Value ? Convert.ToInt32(dr["MedicoId"]) : 0,
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Fecha = dr["Fecha"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha"]) : DateTime.MinValue,
                        Hora = dr["Hora"] != DBNull.Value ? (TimeSpan)dr["Hora"] : TimeSpan.Zero,
                        Estado = dr["Estado"]?.ToString()
                    });
                }
                dr.Close();
            }
            return listaCitas;
        }

        // Listar citas por Especialidad y Fecha (para buscar disponibilidad)
        public static List<Cita> ListarCitasPorEspecialidadYFecha(int especialidadId, DateTime fecha)
        {
            List<Cita> listaCitas = new List<Cita>();
            ConexionBD objEst = new ConexionBD();
            string fechaParam = fecha.ToString("yyyy-MM-dd");
            string sentencia = $"SELECT Id, PacienteId, MedicoId, EspecialidadId, Fecha, Hora, Estado FROM Citas WHERE EspecialidadId = {especialidadId} AND Fecha = '{fechaParam}' ORDER BY Hora ASC;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaCitas.Add(new Cita()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        PacienteId = dr["PacienteId"] != DBNull.Value ? Convert.ToInt32(dr["PacienteId"]) : 0,
                        MedicoId = dr["MedicoId"] != DBNull.Value ? Convert.ToInt32(dr["MedicoId"]) : 0,
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Fecha = dr["Fecha"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha"]) : DateTime.MinValue,
                        Hora = dr["Hora"] != DBNull.Value ? (TimeSpan)dr["Hora"] : TimeSpan.Zero,
                        Estado = dr["Estado"]?.ToString()
                    });
                }
                dr.Close();
            }
            return listaCitas;
        }
    }
}