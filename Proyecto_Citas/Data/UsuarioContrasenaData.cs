using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Data
{
    public class UsuarioContrasenaData
    {
        private readonly string connectionString;

        public UsuarioContrasenaData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool VerificarCorreo(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_VerificarCorreo", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        public bool CambiarContrasena(int id, string email, string nuevaContrasena)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_CambiarContrasena", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@NuevaContrasena", nuevaContrasena);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }
}