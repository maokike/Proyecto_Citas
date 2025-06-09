using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Models
{
    public class Notificacion
    {
        
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public string Mensaje { get; set; }
        public string Estado { get; set; } // Ej: "Pendiente", "Enviada", "Fallida", "Leida"
        public DateTime FechaEnvio { get; set; }
       
    }
}
