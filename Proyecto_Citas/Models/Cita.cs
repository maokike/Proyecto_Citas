using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    namespace App_Citas_medicas_backend.Models
    {
        public class Cita
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public int EspecialidadId { get; set; }
        public DateTime Fecha { get; set; } // En SQL, es DATE. En C# DateTime.
        public TimeSpan Hora { get; set; } // En SQL, es TIME. En C# TimeSpan.
        public string Estado { get; set; } // Ej: "Pendiente", "Confirmada", "Cancelada", "Completada"
    }
}
