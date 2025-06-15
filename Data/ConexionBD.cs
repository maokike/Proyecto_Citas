using System;
using System.Data;
using System.Data.SqlClient; // Necesario para SqlConnection, SqlCommand, SqlDataReader
using System.Collections.Generic; // Necesario para List<SqlParameter>

namespace App_Citas_medicas_backend.Data
{
    public class ConexionBD
    {
        

        public static string cadenaConexion = "Data Source=BD_clinica.mssql.somee.com;Initial Catalog=BD_Clinica;User ID=Maoelias123__SQLLogin_1;Password=fdmdmns12t;";
       


        // Propiedad para el SqlDataReader. Se usará cuando un método necesite leer datos.
        public SqlDataReader Reader { get; private set; }

        // Propiedad para almacenar mensajes de error (útil para depuración).
        public string strError { get; private set; }

        // Método para ejecutar sentencias SQL que no devuelven datos (INSERT, UPDATE, DELETE).
        // Puede ejecutar sentencias directas o un Stored Procedure (si isStoredProcedure es true).
        public bool EjecutarSentencia(string sentencia, bool isStoredProcedure)
        {
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(sentencia, cn))
                {
                    cmd.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

                    try
                    {
                        cn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        cn.Close();
                        return rowsAffected > 0; // Retorna true si se afectó al menos una fila.
                    }
                    catch (SqlException ex)
                    {
                        strError = $"Error SQL en EjecutarSentencia: {ex.Message}";
                        Console.WriteLine(strError);
                        throw; // Re-lanza la excepción para que sea manejada en capas superiores (ej. UsuarioData).
                    }
                    catch (Exception ex)
                    {
                        strError = $"Error general en EjecutarSentencia: {ex.Message}";
                        Console.WriteLine(strError);
                        throw;
                    }
                }
            }
        }

        // Método para ejecutar un Stored Procedure que devuelve un valor entero (ej. 1 para éxito, 0 para fallo).
        public int EjecutarStoredProcedureConRetorno(string nombreSP, List<SqlParameter> parametros)
        {
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(nombreSP, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Añadir todos los parámetros proporcionados al comando del SP.
                    if (parametros != null)
                    {
                        foreach (SqlParameter p in parametros)
                        {
                            cmd.Parameters.Add(p);
                        }
                    }

                    // Configurar un parámetro especial para capturar el valor de retorno del SP.
                    SqlParameter retornoParam = new SqlParameter("@ReturnVal", SqlDbType.Int);
                    retornoParam.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(retornoParam);

                    try
                    {
                        cn.Open();
                        cmd.ExecuteNonQuery(); // Ejecuta el SP.
                        cn.Close();
                        return (int)retornoParam.Value; // Devuelve el valor entero que retornó el SP.
                    }
                    catch (SqlException ex)
                    {
                        strError = $"Error SQL en EjecutarStoredProcedureConRetorno: {ex.Message}";
                        Console.WriteLine(strError);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        strError = $"Error general en EjecutarStoredProcedureConRetorno: {ex.Message}";
                        Console.WriteLine(strError);
                        throw;
                    }
                }
            }
        }

        // Método para ejecutar consultas SQL y obtener datos (devuelve un SqlDataReader).
        // Importante: El SqlDataReader debe cerrarse en el método que lo llama (ej. ListarUsuarios)
        // Cuando el Reader se cierra, la conexión a la base de datos también se cerrará (por CommandBehavior.CloseConnection).
        public bool Consultar(string sentencia, bool isStoredProcedure)
        {
            SqlConnection cn = new SqlConnection(cadenaConexion); // La conexión se crea aquí.
            SqlCommand cmd = new SqlCommand(sentencia, cn);
            cmd.CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

            try
            {
                cn.Open();
                Reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); // Abre el Reader y configura para cerrar la conexión.
                return true;
            }
            catch (SqlException ex)
            {
                strError = $"Error SQL en Consultar: {ex.Message}";
                Console.WriteLine(strError);
                if (cn.State == ConnectionState.Open) // Si la conexión se abrió pero el Reader falló, ciérrala.
                {
                    cn.Close();
                }
                Reader = null; // Asegúrate de que el Reader sea null si hay error.
                return false;
            }
            catch (Exception ex)
            {
                strError = $"Error general en Consultar: {ex.Message}";
                Console.WriteLine(strError);
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
                Reader = null;
                return false;
            }
        }
    }
}