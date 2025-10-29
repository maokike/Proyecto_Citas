// App_Citas_medicas_backend\Data\MedicoData.cs
using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq; // Para usar .FirstOrDefault()

namespace App_Citas_medicas_backend.Data
{
    public class MedicoData
    {
        // Asumo que 'ConexionBD' es una clase que ya tienes y maneja la conexión a la DB.
        // También asumo que 'EjecutarSentenciaConDetalle' es tu método centralizado
        // que maneja errores y re-lanza excepciones SQL.

        // Método centralizado para ejecutar sentencias y obtener resultados de error de SQL Server
        // Retorna true si la sentencia se ejecutó sin lanzar excepción de SQL Server, false en caso contrario
        // Replicado desde UsuarioData si no es accesible directamente
        private static bool EjecutarSentenciaConDetalle(ConexionBD objEst, string sentencia)
        {
            try
            {
                bool resultado = objEst.EjecutarSentencia(sentencia, false);
                return resultado;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"ERROR SQL al ejecutar sentencia de Medico: {sentencia}");
                Console.WriteLine($"Mensaje SQL: {sqlEx.Message}");
                Console.WriteLine($"Número de Error SQL: {sqlEx.Number}");
                Console.WriteLine($"Línea: {sqlEx.LineNumber}");
                throw; // Relanzar la excepción para que sea capturada por el catch superior
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL al ejecutar sentencia de Medico: {sentencia}");
                Console.WriteLine($"Mensaje General: {ex.Message}");
                throw; // Relanzar la excepción
            }
        }


        // Implementación de RegistrarMedico
        public static bool RegistrarMedico(Medico oMedico)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string estatusParam = oMedico.Estatus ? "1" : "0";

                // Asegúrate de que los parámetros INT no tengan comillas simples
                string sentencia = $"EXEC RegistrarMedico " +
                                   $"{oMedico.Cedula}, " +
                                   $"'{oMedico.Nombre}', " +
                                   $"'{oMedico.Apellido}', " +
                                   $"{oMedico.EspecialidadId}, " +
                                   $"'{oMedico.Email}', " +
                                   $"'{oMedico.Contrasena}', " +
                                   $"{estatusParam};";

                Console.WriteLine("Ejecutando SQL (RegistrarMedico): " + sentencia);

                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);

                // Tu SP RegistrarMedico devuelve 1 para éxito y -1 para error.
                // Tu EjecutarSentenciaConDetalle re-lanza la excepción si hay un error SQL,
                // así que este 'resultado' solo reflejará si la sentencia se ejecutó sin excepción.
                // Si el SP devuelve -1 sin lanzar excepción, tendrías que leer el valor de retorno.
                // Por ahora, asumimos que si no hay excepción, es éxito.
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en RegistrarMedico (MedicoData): " + ex.Message);
                throw; // Re-lanza la excepción para que el controlador la capture
            }
        }

        // Implementación de ActualizarMedico
        public static bool ActualizarMedico(Medico oMedico)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string estatusParam = oMedico.Estatus ? "1" : "0";

                // Asegúrate de que los parámetros INT no tengan comillas simples
                string sentencia = $"EXEC ActualizarMedico " +
                                   $"{oMedico.Id}, " + // @MedicoId
                                   $"'{oMedico.Nombre}', " +
                                   $"'{oMedico.Apellido}', " +
                                   $"{oMedico.EspecialidadId}, " +
                                   $"'{oMedico.Email}', " +
                                   $"{estatusParam};"; // @Estatus

                Console.WriteLine("Ejecutando SQL (ActualizarMedico): " + sentencia);

                bool resultado = EjecutarSentenciaConDetalle(objEst, sentencia);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en ActualizarMedico (MedicoData): " + ex.Message);
                throw; // Re-lanza la excepción
            }
        }

        // Implementación de ListarMedicos
        public static List<Medico> ListarMedicos()
        {
            List<Medico> listaMedicos = new List<Medico>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXECUTE ListarMedicos;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listaMedicos.Add(new Medico()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Cedula = dr["Cedula"] != DBNull.Value ? Convert.ToInt32(dr["Cedula"]) : 0,
                        Nombre = dr["Nombre"]?.ToString(),
                        Apellido = dr["Apellido"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0, // Un médico siempre tiene especialidad
                        Email = dr["Email"]?.ToString(),
                        // Contrasena no se selecciona al listar por seguridad
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false,
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }
            return listaMedicos;
        }

        // Implementación de ObtenerMedico (por ID)
        public static Medico ObtenerMedico(int medicoId)
        {
            ConexionBD objEst = new ConexionBD();
            // Tu SP se llama ObtenerMedico y recibe @MedicoId. ConsultarMedico también existe y devuelve menos campos.
            // Usamos ObtenerMedico ya que devuelve más campos que el modelo Medico necesita.
            string sentencia = $"EXECUTE ObtenerMedico {medicoId};";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                if (dr.Read()) // Solo esperamos una fila
                {
                    Medico medico = new Medico()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Cedula = dr["Cedula"] != DBNull.Value ? Convert.ToInt32(dr["Cedula"]) : 0,
                        Nombre = dr["Nombre"]?.ToString(),
                        Apellido = dr["Apellido"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Email = dr["Email"]?.ToString(),
                        // Contrasena no se selecciona por seguridad
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false,
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    };
                    dr.Close();
                    return medico;
                }
                dr.Close();
            }
            return null; // Médico no encontrado
        }

        // Implementación de EliminarMedico
        public static bool EliminarMedico(int medicoId)
        {
            // Nota: Tu SP EliminarMedico solo borra de la tabla Medicos.
            // Si quieres desactivar también en Usuarios, debes modificar el SP.
            string sentencia = $"EXEC EliminarMedico {medicoId};";
            Console.WriteLine("Ejecutando SQL (EliminarMedico): " + sentencia);
            return EjecutarSentenciaConDetalle(new ConexionBD(), sentencia);
        }
    }
}