using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace App_Citas_medicas_backend.Data
{
    public class MedicosData
    {
        public static bool RegistrarMedico(Medicos oMedicos)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                // Manejo de FechaRegistro para permitir valores nulos
                string fechaRegistro = oMedicos.FechaRegistro == DateTime.MinValue
                    ? "NULL"
                    : $"'{oMedicos.FechaRegistro:yyyy-MM-dd HH:mm:ss}'";

                // Construcción de la sentencia SQL
                string sentencia = $"EXEC RegistrarMedico " +
                                   $"'{oMedicos.Cedula}', " +
                                   $"'{oMedicos.Nombre}', " +
                                   $"'{oMedicos.Apellido}', " +
                                   $"'{oMedicos.EspecialidadId}', " +
                                   $"'{oMedicos.Email}', " +
                                   $"'{oMedicos.Contrasena}', " +
                                   $"'{(oMedicos.Estatus ? 1 : 0)}', " +
                                   $"{fechaRegistro}";

                Console.WriteLine("Ejecutando SQL: " + sentencia);

                // Ejecución de la sentencia
                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                if (!resultado)
                {
                    Console.WriteLine("Error: La ejecución de la sentencia SQL falló.");
                }

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RegistrarMedico: " + ex.Message);
                return false;
            }
        }



        public static bool ActualizarMedico(Medicos oMedicos)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                // Construcción de la sentencia SQL con solo los campos modificables
                string sentencia = $"EXEC ActualizarMedico " +
                                   $"'{oMedicos.Id}', " + // Se usa solo para identificar el registro
                                   $"'{oMedicos.Nombre}', " +
                                   $"'{oMedicos.Apellido}', " +
                                   $"'{oMedicos.EspecialidadId}', " +
                                   $"'{oMedicos.Email}', " +
                                   $"'{(oMedicos.Estatus ? 1 : 0)}'"; // Si `Estatus` es modificable

                Console.WriteLine("Ejecutando SQL: " + sentencia);

                // Ejecución de la sentencia
                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarMedico: " + ex.Message);
                return false;
            }
        }


        public static Medicos ConsultarMedico(int id)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                string sentencia = $"EXEC ConsultarMedico '{id}'";

                if (objEst.Consultar(sentencia, false))
                {
                    SqlDataReader dr = objEst.Reader;
                    if (dr.Read())
                    {
                        return new Medicos()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"].ToString(),
                            EspecialidadId = Convert.ToInt32(dr["EspecialidadId"]),
                            Email = dr["Email"].ToString()
                        };
                    }
                    dr.Close();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ConsultarMedico: " + ex.Message);
                return null;
            }
        }

        public static List<Medicos> ListarMedicos()
        {
            List<Medicos> lista = new List<Medicos>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXEC ListarMedicos";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    lista.Add(new Medicos()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Nombre = dr["Nombre"].ToString(),
                        Apellido = dr["Apellido"].ToString(),
                        EspecialidadId = Convert.ToInt32(dr["EspecialidadId"]),
                        Email = dr["Email"].ToString()
                    });
                }
                dr.Close();
            }
            return lista;
        }
    }
}
