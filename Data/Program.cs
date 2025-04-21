using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Data
{
    public class Program
    {
        static void Main(string[] args)
        {
            string strCadenaCnx = "workstation id=BD_clinica.mssql.somee.com;packet size=4096;user id=Maoelias123__SQLLogin_1;pwd=fdmdmns12t;data source=BD_clinica.mssql.somee.com;persist security info=False;initial catalog=BD_clinica;TrustServerCertificate=True";

            try
            {
                using (SqlConnection conexion = new SqlConnection(strCadenaCnx))
                {
                    conexion.Open();
                    Console.WriteLine("✅ Conexión exitosa a la base de datos.");
                    Console.ReadKey();  // Esperar para ver el mensaje
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al conectar: " + ex.Message);
                Console.ReadKey();  // Esperar para ver el mensaje de error
            }
        }
    }
    
}