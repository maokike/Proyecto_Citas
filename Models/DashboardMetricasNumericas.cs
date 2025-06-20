using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Models
{
    public class DashboardMetricasNumericas
    {
        // Métricas de Usuarios
        public int TotalPacientes { get; set; }
        public int PacientesEsteMes { get; set; }
        public int PacientesMesAnterior { get; set; }
        public int TotalMedicos { get; set; }

        // Métricas de Citas
        public int TotalCitas { get; set; }
        public int CitasEsteMes { get; set; }
        public int CitasMesAnterior { get; set; }
        public int CitasProximas { get; set; }
    }
}