using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Models
{
    public class DashboardCitasMensuales
    {
        public string MesNombre { get; set; }
        public int MesNumero { get; set; }
        public int Anio { get; set; }
        public int Confirmadas { get; set; }
        public int Atendidas { get; set; } 
        public int Canceladas { get; set; }
    }
}