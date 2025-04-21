using App_Citas_medicas_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Data
{
    public class MedicosData
    {
        public static bool RegistrarMedico(Medicos oMedicos)
        {
            try
            {
                ConexionBD objEst = new ConexionBD();

                string sentencia = $"EXEC RegistrarMedico '{oMedicos.Nombre}', '{oMedicos.Apellido}', + '{oMedicos.EspecialidadId}', " +
                            $"'{oMedicos.Email}', '{oMedicos.Contraseña}','{oMedicos.FechaRegistro}'" ;

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
                Console.WriteLine("Error en RegistraMedico: " + ex.Message);
                return false;
            }
        }
    }
}