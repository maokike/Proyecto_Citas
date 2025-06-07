using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; // Asegúrate de que este using esté presente para SqlDataReader
using System.Linq;
using System.Web;
// using System.Web.UI.WebControls; // Esta línea probablemente no es necesaria para un Web API, puedes quitarla si no la usas.


namespace App_Citas_medicas_backend.Data
{
    public class UsuarioData
    {
        public static bool RegistrarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                // **¡CORRECCIÓN CLAVE AQUÍ: Manejo explícito de NULL para EspecialidadId!**
                string especialidadIdParam;
                if (oUsuario.EspecialidadId.HasValue) // Si EspecialidadId tiene un valor (no es null)
                {
                    // Lo convertimos a string y lo encerramos en comillas simples para la sentencia SQL
                    especialidadIdParam = $"'{oUsuario.EspecialidadId.Value}'";
                }
                else // Si EspecialidadId es null
                {
                    // Enviamos la palabra NULL (sin comillas)
                    especialidadIdParam = "NULL";
                }

                // Manejo de Estatus: Convertir bool a 1 o 0 para la sentencia SQL
                string estatusParam = oUsuario.Estatus ? "1" : "0";


                // Construir la sentencia SQL con el manejo de NULL para EspecialidadId y Estatus
                string sentencia = $"EXEC RegistrarUsuario " +
                                   $"'{oUsuario.Cedula}', " +
                                   $"'{oUsuario.Nombre}', " +
                                   $"'{oUsuario.Apellido}', " +
                                   $"'{oUsuario.Email}', " +
                                   $"'{oUsuario.Contrasena}', " +
                                   $"'{oUsuario.Rol}', " +
                                   $"{especialidadIdParam}," + // ¡Aquí se usa la cadena ya formateada!
                                   $"{estatusParam};";         // ¡Aquí se usa la cadena ya formateada!

                Console.WriteLine("Ejecutando SQL (RegistrarUsuario): " + sentencia); // 👀 Ver qué consulta se ejecuta

                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                if (!resultado)
                {
                    Console.WriteLine("Error en objEst.EjecutarSentencia para RegistrarUsuario: La ejecución de la sentencia SQL falló.");
                }

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en RegistrarUsuario (UsuarioData): " + ex.Message);
                // Si la excepción no es capturada por objEst.EjecutarSentencia, la capturamos aquí
                return false;
            }
        }


        public static bool ActualizarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                // **¡CORRECCIÓN CLAVE AQUÍ: Manejo explícito de NULL para EspecialidadId en ActualizarUsuario!**
                string especialidadIdParam;
                if (oUsuario.EspecialidadId.HasValue)
                {
                    especialidadIdParam = $"'{oUsuario.EspecialidadId.Value}'";
                }
                else
                {
                    especialidadIdParam = "NULL";
                }

                // Manejo de Estatus para ActualizarUsuario
                string estatusParam = oUsuario.Estatus ? "1" : "0";


                // Incluye el ID del usuario en la consulta SQL
                string sentencia = $"EXEC ActualizarUsuario " +
                                   $"'{oUsuario.Id}', " +
                                   $"'{oUsuario.Cedula}', " +
                                   $"'{oUsuario.Nombre}', " +
                                   $"'{oUsuario.Apellido}', " +
                                   $"'{oUsuario.Email}', " +
                                   $"'{oUsuario.Contrasena}', " +
                                   $"'{oUsuario.Rol}', " + // Asumo que el parámetro en el SP se llama @Rol
                                   $"{especialidadIdParam}," +
                                   $"{estatusParam};";

                Console.WriteLine("Ejecutando SQL (ActualizarUsuario): " + sentencia); // 👀 Ver qué consulta se ejecuta

                bool resultado = objEst.EjecutarSentencia(sentencia, false);

                if (!resultado)
                {
                    Console.WriteLine("Error en objEst.EjecutarSentencia para ActualizarUsuario: La ejecución de la sentencia SQL falló.");
                }

                objEst = null;
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en ActualizarUsuario (UsuarioData): " + ex.Message);
                return false;
            }
        }

        // ... (El resto de tus métodos ListarUsuarios, ObtenerUsuario, EliminarUsuario se mantienen igual) ...

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
                        Contrasena = dr["Contrasena"]?.ToString(), // Corregido el nombre de columna si antes era 'Contraseña'
                        Rol = dr["Rol"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : (int?)null, // Leer como int?
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false, // Convertir a bool
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }

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
                        Contrasena = dr["Contrasena"]?.ToString(), // Corregido el nombre de columna
                        Rol = dr["Rol"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : (int?)null, // Leer como int?
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false,
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    });
                }
                dr.Close();
            }

            return listarUsuarios;
        }

        public static bool EliminarUsuario(string id)
        {
            ConexionBD objEst = new ConexionBD();
            string sentencia;
            sentencia = "EXECUTE EliminarUsuario '" + id + "'";

            // Corregido para que siempre devuelva un bool.
            bool resultado = objEst.EjecutarSentencia(sentencia, false);
            objEst = null; // Liberar el objeto ConexionBD
            return resultado;
        }
    }
}