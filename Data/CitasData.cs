using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace App_Citas_medicas_backend.Data
{
    public class CitasData
    {
        public static bool RegistrarCita(Cita oCita)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string sentencia = $"EXEC RegistrarCita '{oCita.PacienteId}', '{oCita.MedicoId}', '{oCita.EspecialidadId}', " +
                   $"'{oCita.Fecha:yyyy-MM-dd}', '{oCita.Hora}', '{oCita.Estado}'";


                Console.WriteLine("Ejecutando SQL: " + sentencia);

                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RegistrarCita: " + ex.Message);
                return false;
            }
        }

        public static bool ActualizarCita(Cita oCita)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string sentencia = $"EXEC ActualizarCita '{oCita.Id}', '{oCita.PacienteId}', '{oCita.MedicoId}', '{oCita.EspecialidadId}', " +
                                   $"'{oCita.Fecha:yyyy-MM-dd}', '{oCita.Hora}', '{oCita.Estado}'";

                Console.WriteLine("Ejecutando SQL: " + sentencia);

                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarCita: " + ex.Message);
                return false;
            }
        }

        public static Cita ConsultarCita(int id)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string sentencia = $"EXEC ConsultarCita '{id}'";

                if (objEst.Consultar(sentencia, false))
                {
                    SqlDataReader dr = objEst.Reader;
                    if (dr.Read())
                    {
                        return new Cita()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            PacienteId = Convert.ToInt32(dr["PacienteId"]),
                            MedicoId = Convert.ToInt32(dr["MedicoId"]),
                            EspecialidadId = Convert.ToInt32(dr["EspecialidadId"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Hora = TimeSpan.Parse(dr["Hora"].ToString()),
                            Estado = dr["Estado"].ToString()
                        };
                    }
                    dr.Close();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ConsultarCita: " + ex.Message);
                return null;
            }
        }

        public static bool EliminarCita(int id)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string sentencia = $"EXEC EliminarCita '{id}'";

                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en EliminarCita: " + ex.Message);
                return false;
            }
        }

        public static List<Cita> ListarCitas()
        {
            List<Cita> listarCitas = new List<Cita>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXEC ListarCitas";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listarCitas.Add(new Cita()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        PacienteId = Convert.ToInt32(dr["PacienteId"]),
                        MedicoId = Convert.ToInt32(dr["MedicoId"]),
                        EspecialidadId = Convert.ToInt32(dr["EspecialidadId"]),
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
                        Hora = TimeSpan.Parse(dr["Hora"].ToString()),
                        Estado = dr["Estado"].ToString()
                    });
                }
                dr.Close();
            }

            return listarCitas;
        }
    }
}

