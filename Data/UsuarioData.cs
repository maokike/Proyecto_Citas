using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; // Asegúrate de que este using esté presente
using System.Linq; // Para usar .FirstOrDefault()

namespace App_Citas_medicas_backend.Data
{
    public class UsuarioData
    {
        // NO NECESITARÁS EjecutarSentenciaConDetalle si usas los nuevos métodos de ConexionBD.
        // Lo eliminamos ya que su lógica se incorpora en los catches de ConexionBD y el manejo en la capa superior.
        /*
        private static bool EjecutarSentenciaConDetalle(ConexionBD objEst, string sentencia)
        {
            try
            {
                bool resultado = objEst.EjecutarSentencia(sentencia, false);
                return resultado;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"ERROR SQL al ejecutar sentencia: {sentencia}");
                Console.WriteLine($"Mensaje SQL: {sqlEx.Message}");
                Console.WriteLine($"Número de Error SQL: {sqlEx.Number}");
                Console.WriteLine($"Línea: {sqlEx.LineNumber}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL al ejecutar sentencia: {sentencia}");
                Console.WriteLine($"Mensaje General: {ex.Message}");
                throw;
            }
        }
        */

        // Métodos de Gestión de Usuario (Registrar, Actualizar, Listar, Obtener, Eliminar)
        // Usando el nuevo enfoque con parámetros y ExecuteStoredProcedureConRetorno / DataTable

        public static bool RegistrarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("@Cedula", oUsuario.Cedula));
                parametros.Add(new SqlParameter("@Nombre", oUsuario.Nombre));
                parametros.Add(new SqlParameter("@Apellido", oUsuario.Apellido));
                parametros.Add(new SqlParameter("@Email", oUsuario.Email));
                parametros.Add(new SqlParameter("@Contrasena", oUsuario.Contrasena));
                parametros.Add(new SqlParameter("@Rol", oUsuario.Rol));

                if (oUsuario.EspecialidadId.HasValue)
                {
                    parametros.Add(new SqlParameter("@EspecialidadId", oUsuario.EspecialidadId.Value));
                }
                else
                {
                    parametros.Add(new SqlParameter("@EspecialidadId", DBNull.Value));
                }

                parametros.Add(new SqlParameter("@Estatus", oUsuario.Estatus));

                Console.WriteLine("Llamando SP RegistrarUsuario con parámetros...");
                int resultadoSP = objEst.EjecutarStoredProcedureConRetorno("RegistrarUsuario", parametros);

                return resultadoSP == 1; // Devuelve true si el SP retornó 1
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en RegistrarUsuario (UsuarioData): " + ex.Message);
                throw; // Re-lanza la excepción
            }
        }

        public static bool ActualizarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();
                List<SqlParameter> parametros = new List<SqlParameter>();

                parametros.Add(new SqlParameter("@Id", oUsuario.Id));
                parametros.Add(new SqlParameter("@Cedula", oUsuario.Cedula));
                parametros.Add(new SqlParameter("@Nombre", oUsuario.Nombre));
                parametros.Add(new SqlParameter("@Apellido", oUsuario.Apellido));
                parametros.Add(new SqlParameter("@Email", oUsuario.Email));
                parametros.Add(new SqlParameter("@Contrasena", oUsuario.Contrasena));
                parametros.Add(new SqlParameter("@NuevoRol", oUsuario.Rol)); // Tu SP espera @NuevoRol

                if (oUsuario.EspecialidadId.HasValue)
                {
                    parametros.Add(new SqlParameter("@EspecialidadId", oUsuario.EspecialidadId.Value));
                }
                else
                {
                    parametros.Add(new SqlParameter("@EspecialidadId", DBNull.Value));
                }

                parametros.Add(new SqlParameter("@Estatus", oUsuario.Estatus));

                Console.WriteLine("Llamando SP ActualizarUsuario con parámetros...");
                int resultadoSP = objEst.EjecutarStoredProcedureConRetorno("ActualizarUsuario", parametros);

                return resultadoSP == 1; // Devuelve true si el SP retornó 1
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en ActualizarUsuario (UsuarioData): " + ex.Message);
                throw; // Re-lanza la excepción
            }
        }

        public static List<Usuario> ListarUsuarios()
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            ConexionBD objEst = new ConexionBD();
            string sentencia = "EXECUTE ListarUsuarios;"; // Tu SP ListarUsuarios no toma parámetros

            if (objEst.Consultar(sentencia, false)) // 'false' porque es texto, no SP con parámetros
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
                        Contrasena = dr["Contrasena"]?.ToString(), // Considera no obtener contraseñas en listas por seguridad
                        Rol = dr["Rol"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : (int?)null,
                        Estatus = dr["Estatus"] != DBNull.Value ? Convert.ToBoolean(dr["Estatus"]) : false,
                        FechaRegistro = dr["FechaRegistro"] != DBNull.Value ? Convert.ToDateTime(dr["FechaRegistro"]) : DateTime.MinValue
                    });
                }
                dr.Close(); // Asegúrate de cerrar el reader
            }
            // Los errores de objEst.Consultar se manejan en ConexionBD y se registran en strError.
            // Aquí simplemente retornamos la lista (vacía si hubo un error).
            return listarUsuarios;
        }

        public static List<Usuario> ObtenerUsuario(string id)
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            ConexionBD objEst = new ConexionBD();
            // Como tu SP ObtenerUsuario espera un INT, convertimos y pasamos sin comillas.
            // Idealmente, este método debería recibir un 'int id'
            string sentencia = $"EXECUTE ObtenerUsuario {id};";

            if (objEst.Consultar(sentencia, false)) // 'false' porque es texto, no SP con parámetros
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
            return listarUsuarios;
        }

        public static bool EliminarUsuario(string id)
        {
            ConexionBD objEst = new ConexionBD();
            // Asumo que EliminarUsuario SP espera un INT
            string sentencia = $"EXECUTE EliminarUsuario {id};"; // Sin comillas si el SP espera INT

            try
            {
                // Este método llamará a EjecutarSentencia, que re-lanzará la excepción.
                return objEst.EjecutarSentencia(sentencia, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en EliminarUsuario (UsuarioData): " + ex.Message);
                throw; // Re-lanza para el controlador
            }
        }

        // Métodos para Recuperación de Contraseña (Revisados para usar parámetros seguros en futuras refactorizaciones)
        public static Usuario GetUserByEmail(string email)
        {
            ConexionBD objEst = new ConexionBD();
            // --- CÓDIGO TEMPORAL (VULNERABLE A INYECCIÓN SQL) ---
            // IDEALMENTE: Usa objEst.EjecutarProcedimientoAlmacenado o un método específico con SqlParameter
            string sentencia = $"SELECT Id, Cedula, Nombre, Apellido, Email, Contrasena, Rol, EspecialidadId, Estatus, FechaRegistro FROM Usuarios WHERE Email = '{email}';";
            // --- FIN CÓDIGO TEMPORAL ---

            List<Usuario> usuarios = new List<Usuario>();
            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    usuarios.Add(new Usuario()
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
            return usuarios.FirstOrDefault();
        }

        public static string GeneratePasswordResetToken(string email, int userId)
        {
            ConexionBD objEst = new ConexionBD();
            string token = Guid.NewGuid().ToString("N");
            DateTime expiresAt = DateTime.Now.AddHours(1);

            // --- CÓDIGO TEMPORAL (VULNERABLE A INYECCIÓN SQL) ---
            string sentencia = $"INSERT INTO PasswordResetTokens (UserId, Token, ExpiresAt, IsUsed) " +
                               $"VALUES ({userId}, '{token}', '{expiresAt:yyyy-MM-dd HH:mm:ss}', 0);";
            // --- FIN CÓDIGO TEMPORAL ---

            try
            {
                // Usamos EjecutarSentencia aquí, que re-lanzará la excepción si falla.
                if (objEst.EjecutarSentencia(sentencia, false))
                {
                    return token;
                }
                return null; // Falló al guardar el token sin excepción
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar token de recuperación: {ex.Message}");
                throw; // Re-lanza para el controlador
            }
        }

        public static Usuario ValidatePasswordResetToken(string token)
        {
            ConexionBD objEst = new ConexionBD();
            List<Usuario> usuarios = new List<Usuario>();

            // --- CÓDIGO TEMPORAL (VULNERABLE A INYECCIÓN SQL) ---
            string sentencia = $"SELECT U.Id, U.Cedula, U.Nombre, U.Apellido, U.Email, U.Contrasena, U.Rol, U.EspecialidadId, U.Estatus, U.FechaRegistro FROM Usuarios U " +
                               $"INNER JOIN PasswordResetTokens PRT ON U.Id = PRT.UserId " +
                               $"WHERE PRT.Token = '{token}' " +
                               $"AND PRT.ExpiresAt > GETDATE() " +
                               $"AND PRT.IsUsed = 0;";
            // --- FIN CÓDIGO TEMPORAL ---

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                while (dr.Read())
                {
                    usuarios.Add(new Usuario()
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

            if (usuarios.Any())
            {
                // Marcar el token como usado
                // --- CÓDIGO TEMPORAL (VULNERABLE A INYECCIÓN SQL) ---
                string updateTokenSentencia = $"UPDATE PasswordResetTokens SET IsUsed = 1 WHERE Token = '{token}';";
                // --- FIN CÓDIGO TEMPORAL ---
                Console.WriteLine("Ejecutando SQL (ValidatePasswordResetToken - Update IsUsed): " + updateTokenSentencia);
                try
                {
                    objEst.EjecutarSentencia(updateTokenSentencia, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al marcar token de restablecimiento como usado: {ex.Message}");
                    // No relanzamos aquí, ya que el usuario ya ha sido validado.
                }
                return usuarios.FirstOrDefault();
            }
            return null; // Token no válido o expirado o ya usado
        }

        public static bool UpdatePassword(int userId, string newPassword)
        {
            ConexionBD objEst = new ConexionBD();
            // --- CÓDIGO TEMPORAL (VULNERABLE A INYECCIÓN SQL y NO HASHEO) ---
            string sentencia = $"UPDATE Usuarios SET Contrasena = '{newPassword}' WHERE Id = {userId};";
            // --- FIN CÓDIGO TEMPORAL ---

            try
            {
                return objEst.EjecutarSentencia(sentencia, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar contraseña: {ex.Message}");
                throw;
            }
        }
    }
}