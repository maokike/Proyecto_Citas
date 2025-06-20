using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Models
{
    public class DashboardDatosCompleto
    {
        // Propiedad para las métricas numéricas generales
        public DashboardMetricasNumericas Metricas { get; set; }

        // Propiedad para la lista de datos mensuales para la gráfica
        public List<DashboardCitasMensuales> CitasPorMes { get; set; }
    }
}