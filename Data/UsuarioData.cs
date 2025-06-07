using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;


namespace App_Citas_medicas_backend.Data
{
    public class UsuarioData
    {
        public static bool RegistrarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string sentencia = $"EXEC RegistrarUsuario '{oUsuario.Cedula}', '{oUsuario.Nombre}', '{oUsuario.Apellido}', " +
                                   $"'{oUsuario.Email}', '{oUsuario.Contrasena}', '{oUsuario.Rol}', '{oUsuario.EspecialidadId}','{oUsuario.Estatus}';";

                Console.WriteLine("Ejecutando SQL: " + sentencia); // 👀 Ver qué consulta se ejecuta

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
                Console.WriteLine("Error en RegistrarUsuario: " + ex.Message);
                return false;
            }
        }


        public static bool ActualizarUsuario(Usuario oUsuario)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                // Incluye el ID del usuario en la consulta SQL
                string sentencia = $"EXEC ActualizarUsuario '{oUsuario.Id}', '{oUsuario.Cedula}', '{oUsuario.Nombre}', '{oUsuario.Apellido}', " +
                                   $"'{oUsuario.Email}', '{oUsuario.Contrasena}', '{oUsuario.Rol}', '{oUsuario.EspecialidadId}', '{(oUsuario.Estatus ? 1 : 0)}';";

                Console.WriteLine("Ejecutando SQL: " + sentencia); // 👀 Ver qué consulta se ejecuta

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
                Console.WriteLine("Error en ActualizarUsuario: " + ex.Message);
                return false;
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
                                    Id = Convert.ToInt32(dr["Id"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Apellido = dr["Apellido"].ToString(),
                                    Email = dr["Email"].ToString(),
                                    Rol = dr["Rol"].ToString()
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
            sentencia = "EXECUTE ObtenerUsuario '" + id +"'";

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
                        Contrasena = dr["Contraseña"]?.ToString(),
                        Rol = dr["Rol"]?.ToString(),
                        EspecialidadId = dr["EspecialidadId"] != DBNull.Value ? Convert.ToInt32(dr["EspecialidadId"]) : 0,
                        Estatus = dr["Estatus"] != DBNull.Value ? (Convert.ToInt32(dr["Estatus"]) == 1) : false,
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

            if (!objEst.EjecutarSentencia(sentencia, false))
            {
                objEst = null;
                return false;
             
            }
            else
            {
                objEst = null;
                return true;
            }
        }






    }
}