using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace App_Citas_medicas_backend.Data
{
    public class ConexionBD
    {
        #region "Atributos Privados"
        private string strError;
        public string cadenaConexion { get; private set; }
        private SqlDataReader objReader; // Se mantiene para la propiedad Reader
        private string strVrUnico;
        private SqlConnection currentOpenConnection; // <--- Nuevo atributo para la conexión abierta
        #endregion

        #region "Constructor"
        public ConexionBD()
        {
            try
            {
                cadenaConexion = ConfigurationManager.ConnectionStrings["ClinicaDB"].ConnectionString;
            }
            catch (Exception ex)
            {
                strError = "Error al cargar la cadena de conexión del Web.config: " + ex.Message;
                throw new ApplicationException("Fallo al inicializar la conexión a la base de datos.", ex);
            }

            strError = "";
            strVrUnico = "";
            currentOpenConnection = null; // Inicializar en null
        }
        #endregion

        #region "Propiedades Públicas"
        public SqlDataReader Reader
        {
            get { return objReader; }
        }

        public string Error
        {
            set { strError = value; }
            get { return strError; }
        }

        public string ValorUnico
        {
            get { return strVrUnico; }
        }
        #endregion

        #region "Métodos de Conexión"

        // Este método ahora es público para que los consumidores puedan abrir la conexión.
        // Pero es mejor que los métodos que usan 'using' creen su propia conexión.
        // Lo mantendremos para compatibilidad con tu patrón objEst.Consultar.
        public SqlConnection AbrirConexionParaReader() // <--- Renombrado para claridad
        {
            SqlConnection cnn = new SqlConnection(cadenaConexion);
            try
            {
                cnn.Open();
                currentOpenConnection = cnn; // <--- Almacenar la conexión abierta
                return cnn;
            }
            catch (SqlException sqlEx)
            {
                strError = "Error de SQL al abrir la conexión: " + sqlEx.Message;
                throw;
            }
            catch (Exception ex)
            {
                strError = "Error general al abrir la conexión: " + ex.Message;
                throw;
            }
        }

        // Este método ya no es tan necesario si usas 'using (SqlConnection ...)' en cada método.
        // Los 'using' aseguran que la conexión se cierra y los recursos se liberan automáticamente.
        public void CerrarConexion()
        {
            if (objReader != null && !objReader.IsClosed)
            {
                objReader.Close();
                objReader.Dispose();
            }
            objReader = null;

            if (currentOpenConnection != null && currentOpenConnection.State == ConnectionState.Open)
            {
                currentOpenConnection.Close();
                currentOpenConnection.Dispose();
            }
            currentOpenConnection = null;
        }

        #endregion

        #region "Métodos de Ejecución SQL"

        // Método para consultas que devuelven un SqlDataReader (ej. SELECTs).
        // Se asegura que la conexión se abre y se mantiene viva para el Reader.
        public bool Consultar(string SentenciaSQL, bool blnCon_Parametros)
        {
            try
            {
                if (string.IsNullOrEmpty(SentenciaSQL))
                {
                    strError = "Error en instrucción SQL: La sentencia está vacía.";
                    return false;
                }

                // Abre la conexión usando el nuevo método
                SqlConnection conexion = AbrirConexionParaReader(); // <--- Ahora abre la conexión

                using (SqlCommand comando = new SqlCommand(SentenciaSQL, conexion)) // Asocia el comando a la conexión abierta
                {
                    if (blnCon_Parametros)
                        comando.CommandType = CommandType.StoredProcedure;
                    else
                        comando.CommandType = CommandType.Text;

                    objReader = comando.ExecuteReader(); // Asigna el reader al atributo de clase
                    return true;
                }
            }
            catch (SqlException sqlEx)
            {
                strError = "Falla en consulta SQL: " + sqlEx.Message;
                Console.WriteLine($"Error en ConexionBD.Consultar (SQL): {sqlEx.Message}");
                CerrarConexion(); // <--- Cierra la conexión y el reader en caso de error
                return false;
            }
            catch (Exception ex)
            {
                strError = "Falla en consulta general: " + ex.Message;
                Console.WriteLine($"Error en ConexionBD.Consultar (General): {ex.Message}");
                CerrarConexion(); // <--- Cierra la conexión y el reader en caso de error
                return false;
            }
        }

        // Método para ejecutar sentencias que no devuelven datos (INSERT, UPDATE, DELETE)
        // Usa 'using' para conexión y comando, así se cierran automáticamente.
        public bool EjecutarSentencia(string SentenciaSQL, bool blnCon_Parametros)
        {
            try
            {
                if (string.IsNullOrEmpty(SentenciaSQL))
                {
                    strError = "No se ha definido la sentencia a ejecutar.";
                    return false;
                }

                // Este método abre su propia conexión con 'using' para asegurar el cierre.
                using (SqlConnection conexion = new SqlConnection(cadenaConexion)) // Nueva instancia de conexión
                {
                    conexion.Open(); // Abrir explícitamente aquí
                    using (SqlCommand comando = new SqlCommand(SentenciaSQL, conexion))
                    {
                        if (blnCon_Parametros)
                            comando.CommandType = CommandType.StoredProcedure;
                        else
                            comando.CommandType = CommandType.Text;

                        comando.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                strError = "Error SQL al ejecutar instrucción: " + sqlEx.Message;
                throw;
            }
            catch (Exception ex)
            {
                strError = "Error general al ejecutar instrucción: " + ex.Message;
                throw;
            }
        }

        // Método para Ejecutar un Procedimiento Almacenado y obtener su valor de RETORNO (RETURN 1, RETURN -1)
        public int EjecutarStoredProcedureConRetorno(string nombreProcedimiento, List<SqlParameter> parametros = null)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion)) // Nueva instancia de conexión
                {
                    conexion.Open(); // Abrir explícitamente aquí
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;

                        if (parametros != null)
                        {
                            comando.Parameters.AddRange(parametros.ToArray());
                        }

                        SqlParameter returnValue = new SqlParameter
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        comando.Parameters.Add(returnValue);

                        comando.ExecuteNonQuery();

                        if (returnValue.Value != DBNull.Value)
                        {
                            return (int)returnValue.Value;
                        }
                        return -99;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                strError = $"Error SQL al ejecutar procedimiento '{nombreProcedimiento}': {sqlEx.Message}";
                throw;
            }
            catch (Exception ex)
            {
                strError = $"Error general al ejecutar procedimiento '{nombreProcedimiento}': {ex.Message}";
                throw;
            }
        }

        // Método para consultas que devuelven un valor escalar (ej. COUNT, SUM, MAX, etc.)
        public bool ConsultarValorUnico(string SentenciaSQL, bool blnCon_Parametros)
        {
            try
            {
                if (string.IsNullOrEmpty(SentenciaSQL))
                {
                    strError = "No se ha definido la sentencia a ejecutar.";
                    return false;
                }
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open(); // Abrir explícitamente aquí
                    using (SqlCommand comando = new SqlCommand(SentenciaSQL, conexion))
                    {
                        if (blnCon_Parametros)
                            comando.CommandType = CommandType.StoredProcedure;
                        else
                            comando.CommandType = CommandType.Text;

                        object result = comando.ExecuteScalar();
                        strVrUnico = Convert.ToString(result);
                        return true;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                strError = "Error SQL al ejecutar instrucción escalar: " + sqlEx.Message;
                throw;
            }
            catch (Exception ex)
            {
                strError = "Error general al ejecutar instrucción escalar: " + ex.Message;
                throw;
            }
        }

        // Este método es para llenar un DataTable. También usa 'using'.
        public DataTable EjecutarProcedimientoAlmacenado(string nombreProcedimiento, Dictionary<string, object> parametros = null)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open(); // Abrir explícitamente aquí
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                SqlParameter param = new SqlParameter(parametro.Key, parametro.Value ?? DBNull.Value);
                                comando.Parameters.Add(param);
                            }
                        }

                        using (SqlDataAdapter dap = new SqlDataAdapter(comando))
                        {
                            dap.Fill(dataTable);
                        }
                    }
                }
                return dataTable;
            }
            catch (SqlException sqlEx)
            {
                strError = "Error SQL al ejecutar el procedimiento almacenado (DataTable): " + sqlEx.Message;
                throw;
            }
            catch (Exception ex)
            {
                strError = "Error general al ejecutar el procedimiento almacenado (DataTable): " + ex.Message;
                throw;
            }
        }

        // Elimina este método si no lo usas.
        // internal void LimpiarParametros() { throw new NotImplementedException(); }

        #endregion

        // Método NUEVO para ejecutar un Stored Procedure que devuelve MÚLTIPLES conjuntos de resultados (DataTables)
        // y los encapsula en un DataSet.
        public DataSet EjecutarSPMultiplesResultados(string nombreProcedimiento, List<SqlParameter> parametros = null)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion)) // Usa tu cadenaConexion de instancia
                {
                    conexion.Open(); // Abrir explícitamente aquí
                    using (SqlCommand comando = new SqlCommand(nombreProcedimiento, conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear(); // Limpia parámetros previos

                        if (parametros != null)
                        {
                            foreach (var parametro in parametros)
                            {
                                // Asegúrate de que los parámetros se añaden con su nombre y valor
                                SqlParameter param = new SqlParameter(parametro.ParameterName, parametro.Value ?? DBNull.Value);
                                comando.Parameters.Add(param);
                            }
                        }

                        using (SqlDataAdapter dap = new SqlDataAdapter(comando))
                        {
                            dap.Fill(dataSet); // Llena el DataSet con todos los conjuntos de resultados
                        }
                    }
                }
                return dataSet; // Devuelve el DataSet con todas las tablas de resultados
            }
            catch (SqlException sqlEx)
            {
                strError = $"Error SQL al ejecutar procedimiento con múltiples resultados '{nombreProcedimiento}': {sqlEx.Message}";
                Console.WriteLine(strError);
                throw; // Re-lanza la excepción
            }
            catch (Exception ex)
            {
                strError = $"Error general al ejecutar procedimiento con múltiples resultados '{nombreProcedimiento}': {ex.Message}";
                Console.WriteLine(strError);
                throw;
            }
        }
    }
}