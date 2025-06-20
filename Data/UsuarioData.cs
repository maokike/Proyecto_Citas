using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data; // Necesario para DataTable, DataSet, DataRow
using System.Data.SqlClient;
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
        // Usan instancias de ConexionBD y sus métodos.

        public static bool RegistrarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión
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
                int resultadoSP = objConexion.EjecutarStoredProcedureConRetorno("RegistrarUsuario", parametros);

                return resultadoSP == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en RegistrarUsuario (UsuarioData): " + ex.Message);
                throw;
            }
        }

        public static bool ActualizarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión
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
                int resultadoSP = objConexion.EjecutarStoredProcedureConRetorno("ActualizarUsuario", parametros);

                return resultadoSP == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en ActualizarUsuario (UsuarioData): " + ex.Message);
                throw;
            }
        }

        public static List<Usuario> ListarUsuarios()
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión
            string sentencia = "EXECUTE ListarUsuarios;";

            Console.WriteLine("DEBUG: Intentando listar usuarios...");
            // Acceso a cadenaConexion a través de la instancia de ConexionBD
            Console.WriteLine($"DEBUG: Cadena de Conexión usada: {objConexion.cadenaConexion}");

            // El método Consultar de tu ConexionBD maneja el DataReader y lo expone vía objConexion.Reader
            if (objConexion.Consultar(sentencia, false))
            {
                SqlDataReader dr = objConexion.Reader;
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
                    // Asegúrate de cerrar la conexión y el reader a través de la instancia de ConexionBD
                    objConexion.CerrarConexion();
                }
                Console.WriteLine($"DEBUG: Número de usuarios leídos y mapeados: {count}");
            }
            else
            {
                Console.WriteLine($"DEBUG: objConexion.Consultar devolvió false. Error: {objConexion.Error}");
            }
            return listarUsuarios;
        }

        public static List<Usuario> ObtenerUsuario(string id)
        {
            List<Usuario> listarUsuarios = new List<Usuario>();
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión
            string sentencia = $"EXECUTE ObtenerUsuario {id};";

            if (objConexion.Consultar(sentencia, false))
            {
                SqlDataReader dr = objConexion.Reader;
                try
                {
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: Fallo al leer o mapear un registro en ObtenerUsuario: {ex.Message}");
                }
                finally
                {
                    objConexion.CerrarConexion();
                }
            }
            return listarUsuarios;
        }

        public static bool EliminarUsuario(string id)
        {
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión
            string sentencia = $"EXECUTE EliminarUsuario {id};";

            try
            {
                return objConexion.EjecutarSentencia(sentencia, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error crítico en EliminarUsuario (UsuarioData): " + ex.Message);
                throw;
            }
        }


        // *************************************************************************************
        // Métodos ESPECÍFICOS para Recuperación de Contraseña (Seguros y Compatibles con tu ConexionBD)
        // *************************************************************************************

        // Método para obtener un Usuario por su dirección de correo electrónico (seguro con parámetros)
        public static Usuario ObtenerUsuarioPorEmail(string email)
        {
            Usuario usuario = null;
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión

            string sentencia = "SELECT Id, Cedula, Nombre, Apellido, Email, Contrasena, Rol, EspecialidadId, Estatus, FechaRegistro FROM Usuarios WHERE Email = @Email;";

            // Usamos una conexión directa aquí para asegurar el uso de parámetros con SELECT
            // Si tu ConexionBD tuviera un método Consultar(string sql, List<SqlParameter> params), sería ideal.
            try
            {
                using (SqlConnection cn = new SqlConnection(objConexion.cadenaConexion)) // Acceso a cadenaConexion de instancia (ahora pública)
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
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión
            string nombreSP = "GuardarPasswordResetToken";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Token", token),
                new SqlParameter("@ExpiresAt", expiresAt)
            };

            try
            {
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
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión

            string selectSentencia = "SELECT U.Id, U.Cedula, U.Nombre, U.Apellido, U.Email, U.Contrasena, U.Rol, U.EspecialidadId, U.Estatus, U.FechaRegistro FROM Usuarios U " +
                                    "INNER JOIN PasswordResetTokens PRT ON U.Id = PRT.UserId " +
                                    "WHERE PRT.Token = @Token AND PRT.ExpiresAt > GETDATE() AND PRT.IsUsed = 0;";

            string updateTokenSentencia = "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE Token = @Token;";

            try
            {
                using (SqlConnection cn = new SqlConnection(objConexion.cadenaConexion)) // Acceso a cadenaConexion de instancia
                {
                    cn.Open();

                    // 1. Validar el token y obtener usuario
                    using (SqlCommand cmdSelect = new SqlCommand(selectSentencia, cn))
                    {
                        cmdSelect.CommandType = CommandType.Text;
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
                            cmdUpdate.CommandType = CommandType.Text;
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
            string hashedPassword = nuevaContrasena; // <<-- ¡RECUERDA: CAMBIAR ESTO POR HASHING REAL!

            string sentencia = "UPDATE Usuarios SET Contrasena = @NewPassword WHERE Id = @UserId;";
            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión

            try
            {
                using (SqlConnection cn = new SqlConnection(objConexion.cadenaConexion)) // Acceso a cadenaConexion de instancia
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
                throw;
            }
        }

        // *************************************************************************************
        // Método para obtener TODOS los datos del Dashboard (Compatible con tu ConexionBD)
        // Llama al SP 'ObtenerMetricasDashboard' y procesa sus múltiples conjuntos de resultados
        // *************************************************************************************
        public static DashboardDatosCompleto ObtenerDatosDashboard()
        {
            DashboardDatosCompleto datosDashboard = new DashboardDatosCompleto();
            datosDashboard.Metricas = new DashboardMetricasNumericas();
            datosDashboard.CitasPorMes = new List<DashboardCitasMensuales>();

            ConexionBD objConexion = new ConexionBD(); // Instancia de tu clase de conexión

            try
            {
                // Usamos el nuevo método EjecutarSPMultiplesResultados de tu clase ConexionBD
                // que devuelve un DataSet.
                DataSet ds = objConexion.EjecutarSPMultiplesResultados("ObtenerMetricasDashboard");

                // --- PRIMER RESULTADO: Métricas de Usuarios (Pacientes y Médicos) ---
                // Aseguramos que la tabla y la fila existan antes de intentar leer.
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    datosDashboard.Metricas.TotalPacientes = dr["TotalPacientes"] != DBNull.Value ? Convert.ToInt32(dr["TotalPacientes"]) : 0;
                    datosDashboard.Metricas.PacientesEsteMes = dr["PacientesEsteMes"] != DBNull.Value ? Convert.ToInt32(dr["PacientesEsteMes"]) : 0;
                    datosDashboard.Metricas.PacientesMesAnterior = dr["PacientesMesAnterior"] != DBNull.Value ? Convert.ToInt32(dr["PacientesMesAnterior"]) : 0;
                    datosDashboard.Metricas.TotalMedicos = dr["TotalMedicos"] != DBNull.Value ? Convert.ToInt32(dr["TotalMedicos"]) : 0;
                }

                // --- SEGUNDO RESULTADO: Métricas de Citas (Totales y por Mes) ---
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[1].Rows[0];
                    datosDashboard.Metricas.TotalCitas = dr["TotalCitas"] != DBNull.Value ? Convert.ToInt32(dr["TotalCitas"]) : 0;
                    datosDashboard.Metricas.CitasEsteMes = dr["CitasEsteMes"] != DBNull.Value ? Convert.ToInt32(dr["CitasEsteMes"]) : 0;
                    datosDashboard.Metricas.CitasMesAnterior = dr["CitasMesAnterior"] != DBNull.Value ? Convert.ToInt32(dr["CitasMesAnterior"]) : 0;
                }

                // --- TERCER RESULTADO: Citas Próximas (en los próximos 7 días) ---
                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[2].Rows[0];
                    datosDashboard.Metricas.CitasProximas = dr["CitasProximas"] != DBNull.Value ? Convert.ToInt32(dr["CitasProximas"]) : 0;
                }

                // --- CUARTO RESULTADO: Descripción general de Citas por Mes y Estado (para la gráfica) ---
                if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        datosDashboard.CitasPorMes.Add(new DashboardCitasMensuales
                        {
                            MesNombre = dr["MesNombre"]?.ToString(),
                            MesNumero = dr["MesNumero"] != DBNull.Value ? Convert.ToInt32(dr["MesNumero"]) : 0,
                            Anio = dr["Anio"] != DBNull.Value ? Convert.ToInt32(dr["Anio"]) : 0,
                            Confirmadas = dr["Confirmadas"] != DBNull.Value ? Convert.ToInt32(dr["Confirmadas"]) : 0,
                            Atendidas = dr["Atendidas"] != DBNull.Value ? Convert.ToInt32(dr["Atendidas"]) : 0,
                            Canceladas = dr["Canceladas"] != DBNull.Value ? Convert.ToInt32(dr["Canceladas"]) : 0
                        });
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL en ObtenerDatosDashboard: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general en ObtenerDatosDashboard: {ex.Message}");
                throw;
            }

            return datosDashboard;
        }

    } // Fin de la clase UsuarioData
}