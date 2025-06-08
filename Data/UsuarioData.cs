using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; // Asegúrate de que este using esté presente
using System.Linq;
using System.Web;


namespace App_Citas_medicas_backend.Data
{
    public class UsuarioData
    {
        // Nota: Asumo que 'ConexionBD' es una clase que ya tienes y maneja la conexión a la DB.
        // Asegúrate de que sus métodos 'EjecutarSentencia' y 'Consultar' funcionan correctamente
        // y que 'EjecutarSentencia' devuelva 'false' si la operación no fue exitosa.

        // Método centralizado para ejecutar sentencias y obtener resultados de error de SQL Server
        // Retorna true si la sentencia se ejecutó sin lanzar excepción de SQL Server, false en caso contrario
        private static bool EjecutarSentenciaConDetalle(ConexionBD objEst, string sentencia)
        {
            try
            {
                // objEst.ClearErrors(); // Si tu clase ConexionBD tiene un método para esto

                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                // Si tu objEst.EjecutarSentencia expone alguna propiedad de error de SQL Server
                // if (objEst.HasErrors) {
                //    Console.WriteLine($"Error de SQL Server detectado: {objEst.LastError.Message}");
                //    return false;
                // }

                return resultado;
            }
            catch (SqlException sqlEx) // Capturar excepciones específicas de SQL Server
            {
                Console.WriteLine($"ERROR SQL al ejecutar sentencia: {sentencia}");
                Console.WriteLine($"Mensaje SQL: {sqlEx.Message}");
                Console.WriteLine($"Número de Error SQL: {sqlEx.Number}");
                Console.WriteLine($"Línea: {sqlEx.LineNumber}");
                throw; // Relanzar la excepción para que sea capturada por el catch superior (en el Controller)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL al ejecutar sentencia: {sentencia}");
                Console.WriteLine($"Mensaje General: {ex.Message}");
                throw; // Relanzar la excepción
            }
        }


        public static bool RegistrarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string especialidadIdParam;
                if (oUsuario.EspecialidadId.HasValue)
                {
                    // ¡Importante! Eliminar comillas para parámetros INT
                    especialidadIdParam = $"{oUsuario.EspecialidadId.Value}";
                }
                else
                {
                    especialidadIdParam = "NULL";
                }

                string estatusParam = oUsuario.Estatus ? "1" : "0";

                string sentencia = $"EXEC RegistrarUsuario " +
                                   $"{oUsuario.Cedula}, " + // QUITAR COMILLAS SIMPLES
                                   $"'{oUsuario.Nombre}', " +
                                   $"'{oUsuario.Apellido}', " +
                                   $"'{oUsuario.Email}', " +
                                   $"'{oUsuario.Contrasena}', " +
                                   $"'{oUsuario.Rol}', " +
                                   $"{especialidadIdParam}," + // QUITAR COMILLAS SIMPLES si tiene valor
                                   $"{estatusParam};"; // QUITAR COMILLAS SIMPLES

                Console.WriteLine("Ejecutando SQL (RegistrarUsuario): " + sentencia);

                // Llamada correcta al método estático EjecutarSentenciaConDetalle
                bool resultado = UsuarioData.EjecutarSentenciaConDetalle(objEst, sentencia);

                if (!resultado)
                {
                    Console.WriteLine("Advertencia: objEst.EjecutarSentencia para RegistrarUsuario devolvió false. Posible fallo silencioso.");
                }

                // objEst = null; // No es estrictamente necesario, el recolector de basura lo liberará.
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en RegistrarUsuario (UsuarioData): " + ex.Message);
                // Aquí, si quieres que el controlador reciba el error, debes relanzarlo.
                // Si devuelves false, el controlador recibirá un Bad Request genérico.
                throw; // Relanzar para que el controlador lo capture y devuelva un 500 o Bad Request específico.
            }
        }


        public static bool ActualizarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string especialidadIdParam;
                if (oUsuario.EspecialidadId.HasValue)
                {
                    // ¡Importante! Eliminar comillas para parámetros INT
                    especialidadIdParam = $"{oUsuario.EspecialidadId.Value}";
                }
                else
                {
                    especialidadIdParam = "NULL";
                }

                string estatusParam = oUsuario.Estatus ? "1" : "0";

                string sentencia = $"EXEC ActualizarUsuario " +
                                   $"{oUsuario.Id}, " + // QUITAR COMILLAS SIMPLES
                                   $"{oUsuario.Cedula}, " + // QUITAR COMILLAS SIMPLES
                                   $"'{oUsuario.Nombre}', " +
                                   $"'{oUsuario.Apellido}', " +
                                   $"'{oUsuario.Email}', " +
                                   $"'{oUsuario.Contrasena}', " +
                                   $"'{oUsuario.Rol}', " + // Aquí Rol se mapea a @NuevoRol en el SP
                                   $"{especialidadIdParam}," + // QUITAR COMILLAS SIMPLES si tiene valor
                                   $"{estatusParam};"; // QUITAR COMILLAS SIMPLES

                Console.WriteLine("Ejecutando SQL (ActualizarUsuario): " + sentencia);

                bool resultado = UsuarioData.EjecutarSentenciaConDetalle(objEst, sentencia);

                if (!resultado)
                {
                    Console.WriteLine("Advertencia: objEst.EjecutarSentencia para ActualizarUsuario devolvió false. Posible fallo silencioso.");
                }

                // objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en ActualizarUsuario (UsuarioData): " + ex.Message);
                throw; // Relanzar para que el controlador lo capture
            }
        }

        public static List<Usuario> ListarUsuarios()
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXECUTE ListarUsuarios;";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listarUsuarios.Add(new Usuario()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Cedula = dr["Cedula"] != DBNull.Value ? Convert.ToInt32(dr["Cedula"]) : 0,
                        Nombre = dr["Nombre"]?.ToString(),
                        Apellido = dr["Apellido"]?.ToString(),
                        Email = dr["Email"]?.ToString(),
                        Contrasena = dr["Contrasena"]?.ToString(),
                        Rol = dr["Rol"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : (int?)null,
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false,
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }

            // objEst = null;
            return listarUsuarios;
        }

        public static List<Usuario> ObtenerUsuario(string id)
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            ConexionBD objEst = new ConexionBD();
            string sentencia;
            sentencia = "EXECUTE ObtenerUsuario '" + id + "'";

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    listarUsuarios.Add(new Usuario()
                    {
                        Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                        Cedula = dr["Cedula"] != DBNull.Value ? Convert.ToInt32(dr["Cedula"]) : 0,
                        Nombre = dr["Nombre"]?.ToString(),
                        Apellido = dr["Apellido"]?.ToString(),
                        Email = dr["Email"]?.ToString(),
                        Contrasena = dr["Contrasena"]?.ToString(),
                        Rol = dr["Rol"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : (int?)null,
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false,
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }

            // objEst = null;
            return listarUsuarios;
        }

        public static bool EliminarUsuario(string id)
        {
            ConexionBD objEst = new ConexionBD();
            string sentencia;
            sentencia = "EXECUTE EliminarUsuario '" + id + "'";

            bool resultado = objEst.EjecutarSentencia(sentencia, false);
            // objEst = null;
            return resultado;
        }
    }
}