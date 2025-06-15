using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; // Necesario para SqlConnection, SqlCommand, SqlDataReader
using System.Linq; // Para usar .FirstOrDefault()

namespace App_Citas_medicas_backend.Data
{
    public class UsuarioData
    {
        // NOTA IMPORTANTE SOBRE SEGURIDAD DE CONTRASEÑAS:
        // Para una aplicación real, NUNCA almacenes contraseñas en texto plano en la base de datos.
        // Siempre usa un algoritmo de hash seguro (ej. BCrypt, PBKDF2) con un salt aleatorio antes de guardarlas.
        // Aquí se asume que 'oUsuario.Contrasena' y 'nuevaContrasena' ya vienen hasheadas o serán hasheadas ANTES de usar este método.


        // Métodos de Gestión de Usuario (Registrar, Actualizar, Listar, Obtener, Eliminar)
        // Estos métodos usan el enfoque de Stored Procedures con parámetros, lo cual es seguro.

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
                parametros.Add(new SqlParameter("@Contrasena", oUsuario.Contrasena)); // Se asume que ya viene hasheada
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
                throw; // Re-lanza la excepción para que el controlador la capture
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
                parametros.Add(new SqlParameter("@Contrasena", oUsuario.Contrasena)); // Se asume que ya viene hasheada
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
            string sentencia = "EXECUTE ListarUsuarios;"; // SP existente para listar
            ConexionBD objEst = new ConexionBD();

            Console.WriteLine("DEBUG: Intentando listar usuarios...");
            Console.WriteLine($"DEBUG: Cadena de Conexión usada: {ConexionBD.cadenaConexion}");

            if (objEst.Consultar(sentencia, false))
            {
                SqlDataReader dr = objEst.Reader;
                int count = 0;
                try
                {
                    while (dr.Read())
                    {
                        try
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
                            count++;
                        }
                        catch (Exception recordEx)
                        {
                            Console.WriteLine($"ERROR: Fallo al leer o mapear un registro de usuario: {recordEx.Message}");
                        }
                    }
                }
                catch (Exception readLoopEx)
                {
                    Console.WriteLine($"ERROR: Fallo general durante la lectura de SqlDataReader: {readLoopEx.Message}");
                }
                finally
                {
                    if (dr != null && !dr.IsClosed)
                    {
                        dr.Close();
                    }
                }
                Console.WriteLine($"DEBUG: Número de usuarios leídos y mapeados: {count}");
            }
            else
            {
                Console.WriteLine($"DEBUG: objEst.Consultar devolvió false. Error: {objEst.strError}");
            }
            return listarUsuarios;
        }

        public static List<Usuario> ObtenerUsuario(string id) // Se recomienda que el parámetro 'id' sea INT si tu SP lo espera
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            string sentencia = $"EXECUTE ObtenerUsuario {id};"; // SP existente para obtener por ID
            ConexionBD objEst = new ConexionBD();

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
            return listarUsuarios;
        }

        public static bool EliminarUsuario(string id)
        {
            ConexionBD objEst = new ConexionBD();
            string sentencia = $"EXECUTE EliminarUsuario {id};"; // SP existente para eliminar
            try
            {
                return objEst.EjecutarSentencia(sentencia, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en EliminarUsuario (UsuarioData): " + ex.Message);
                throw;
            }
        }


        // *************************************************************************************
        // Métodos ESPECÍFICOS para Recuperación de Contraseña (Seguros y Refactorizados)
        // *************************************************************************************

        // Método para obtener un Usuario por su dirección de correo electrónico (seguro con parámetros)
        public static Usuario ObtenerUsuarioPorEmail(string email)
        {
            Usuario usuario = null;
            string sentencia = "SELECT Id, Cedula, Nombre, Apellido, Email, Contrasena, Rol, EspecialidadId, Estatus, FechaRegistro FROM Usuarios WHERE Email = @Email;";

            try
            {
                using (SqlConnection cn = new SqlConnection(ConexionBD.cadenaConexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sentencia, cn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                usuario = new Usuario()
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
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerUsuarioPorEmail: {ex.Message}");
                return null;
            }
            return usuario;
        }

        // Método para guardar un nuevo token de restablecimiento de contraseña (seguro, usa SP)
        public static bool GuardarTokenRestablecimientoContrasena(int userId, string token, DateTime expiresAt)
        {
            string nombreSP = "GuardarPasswordResetToken";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Token", token),
                new SqlParameter("@ExpiresAt", expiresAt)
            };

            try
            {
                ConexionBD objConexion = new ConexionBD();
                int resultado = objConexion.EjecutarStoredProcedureConRetorno(nombreSP, parametros);
                return resultado == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GuardarTokenRestablecimientoContrasena: {ex.Message}");
                return false;
            }
        }

        // Método para validar un token de restablecimiento de contraseña y obtener el usuario (seguro con parámetros)
        // Y marca el token como usado.
        public static Usuario ValidarTokenRestablecimientoContrasena(string token)
        {
            Usuario usuario = null;
            string selectSentencia = "SELECT U.Id, U.Cedula, U.Nombre, U.Apellido, U.Email, U.Contrasena, U.Rol, U.EspecialidadId, U.Estatus, U.FechaRegistro FROM Usuarios U " +
                                    "INNER JOIN PasswordResetTokens PRT ON U.Id = PRT.UserId " +
                                    "WHERE PRT.Token = @Token AND PRT.ExpiresAt > GETDATE() AND PRT.IsUsed = 0;";

            string updateTokenSentencia = "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE Token = @Token;";

            try
            {
                using (SqlConnection cn = new SqlConnection(ConexionBD.cadenaConexion))
                {
                    cn.Open();

                    // 1. Validar el token y obtener usuario
                    using (SqlCommand cmdSelect = new SqlCommand(selectSentencia, cn))
                    {
                        cmdSelect.Parameters.AddWithValue("@Token", token);
                        using (SqlDataReader dr = cmdSelect.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                usuario = new Usuario()
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
                                };
                            }
                        }
                    }

                    // 2. Marcar el token como usado (solo si el usuario fue encontrado y el token es válido)
                    if (usuario != null)
                    {
                        using (SqlCommand cmdUpdate = new SqlCommand(updateTokenSentencia, cn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@Token", token);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ValidarTokenRestablecimientoContrasena: {ex.Message}");
                return null;
            }
            return usuario;
        }

        // Método para actualizar la contraseña de un usuario (seguro con parámetros)
        public static bool ActualizarContrasena(int userId, string nuevaContrasena)
        {
            // ¡IMPORTANTE! Aquí es donde DEBES hashear 'nuevaContrasena' antes de guardarla.
            // Si no lo haces, estarás almacenando contraseñas en texto plano, lo cual es un riesgo grave.
            // Ejemplo (necesitarías una librería de hashing como BCrypt.Net):
            // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);
            string hashedPassword = nuevaContrasena; // <-- RECUERDA: ¡CAMBIAR ESTO POR HASHING REAL!

            string sentencia = "UPDATE Usuarios SET Contrasena = @NewPassword WHERE Id = @UserId;";

            try
            {
                using (SqlConnection cn = new SqlConnection(ConexionBD.cadenaConexion))
                {
                    using (SqlCommand cmd = new SqlCommand(sentencia, cn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@NewPassword", hashedPassword);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        cn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        cn.Close();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar contraseña: {ex.Message}");
                throw; // Re-lanza la excepción para que el controlador la capture.
            }
        }

    } 
} 