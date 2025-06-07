using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using App_Citas_medicas_backend.Models;

namespace App_Citas_medicas_backend.Data
{
    public static class CitasData
    {
        public static bool RegistrarCita(Cita cita)
        {
            try
            {
                ConexionBD conexion = new ConexionBD();

                // Construcción de la sentencia SQL para llamar al procedimiento almacenado
                string sentencia = $"EXEC RegistrarCita " +
                                   $"'{cita.PacienteId}', " +
                                   $"'{cita.MedicoId}', " +
                                   $"'{cita.EspecialidadId}', " +
                                   $"'{cita.Fecha:yyyy-MM-dd}', " +
                                   $"'{cita.Hora}', " +
                                   $"'{cita.Estado}'";



                Console.WriteLine("Ejecutando SQL: " + sentencia);

                // Ejecutar la sentencia
                bool resultado = conexion.EjecutarSentencia(sentencia, false);

                if (!resultado)
                {
                    Console.WriteLine("Error: La ejecución de la sentencia SQL falló.");
                }

                conexion = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RegistrarCita: " + ex.Message);
                return false;
            }
        }




        public static bool ActualizarCita(Cita cita)
        {
            try
            {
                ConexionBD conexion = new ConexionBD();

                // Construcción de la sentencia SQL para llamar al procedimiento almacenado
                string sentencia = $"EXEC ActualizarCita " +
                                   $"'{cita.CitaId}', " +
                                   $"'{cita.PacienteId}', " +
                                   $"'{cita.MedicoId}', " +
                                   $"'{cita.EspecialidadId}', " +
                                   $"'{cita.Fecha:yyyy-MM-dd}', " +
                                   $"'{cita.Hora}', " +
                                   $"'{cita.Estado}'";

                Console.WriteLine("Ejecutando SQL: " + sentencia);

                // Ejecutar la sentencia
                bool resultado = conexion.EjecutarSentencia(sentencia, false);

                if (!resultado)
                {
                    Console.WriteLine("Error: La ejecución de la sentencia SQL falló.");
                }

                conexion = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarCita: " + ex.Message);
                return false;
            }
        }

        
        public static List<Cita> ListarCita()
         {
        List<Cita> citas = new List<Cita>();
        try
        {
            ConexionBD conexion = new ConexionBD();
            string sentencia = "EXEC ListarCitas"; // Procedimiento almacenado para listar citas

            if (conexion.Consultar(sentencia, false))
            {
                SqlDataReader reader = conexion.Reader;
                while (reader.Read())
                {
                    citas.Add(new Cita
                    {
                        CitaId = Convert.ToInt32(reader["Id"]),
                        PacienteId = Convert.ToInt32(reader["PacienteId"]),
                        MedicoId = Convert.ToInt32(reader["MedicoId"]),
                        EspecialidadId = Convert.ToInt32(reader["EspecialidadId"]),
                        Fecha = Convert.ToDateTime(reader["Fecha"]),
                        Hora = (TimeSpan)reader["Hora"],
                        Estado = reader["Estado"].ToString()
                    });
                }
                reader.Close();
            }
            conexion = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en ListarCita: " + ex.Message);
        }
        return citas;
         }

        

        public static Cita ObtenerCita(int id)
        {
            Cita cita = null;
            try
            {
                ConexionBD conexion = new ConexionBD();
                string sentencia = $"EXEC ObtenerCita '{id}'"; // Procedimiento almacenado para obtener una cita por ID

                if (conexion.Consultar(sentencia, false))
                {
                    SqlDataReader reader = conexion.Reader;
                    if (reader.Read())
                    {
                        cita = new Cita
                        {
                            CitaId = Convert.ToInt32(reader["Id"]),
                            PacienteId = Convert.ToInt32(reader["PacienteId"]),
                            MedicoId = Convert.ToInt32(reader["MedicoId"]),
                            EspecialidadId = Convert.ToInt32(reader["EspecialidadId"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            Hora = (TimeSpan)reader["Hora"],
                            Estado = reader["Estado"].ToString()
                        };
                    }
                    reader.Close();
                }
                conexion = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ObtenerCita: " + ex.Message);
            }
            return cita;
        }

        public static bool EliminarCita(int id)
        {
            try
            {
                ConexionBD conexion = new ConexionBD();
                string sentencia = $"EXEC EliminarCita '{id}'"; // Procedimiento almacenado para eliminar una cita por ID

                bool resultado = conexion.EjecutarSentencia(sentencia, false);
                conexion = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en EliminarCita: " + ex.Message);
                return false;
            }
        }

    }

}


