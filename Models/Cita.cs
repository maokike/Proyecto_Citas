using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    namespace App_Citas_medicas_backend.Models
    {
        public class Cita
        {
            public int CitaId { get; set; }
            public int PacienteId { get; set; }
            public int MedicoId { get; set; }
            public int EspecialidadId { get; set; }
            public DateTime Fecha { get; set; }
            public TimeSpan Hora { get; set; }
            public string Estado { get; set; }
        }
    }


