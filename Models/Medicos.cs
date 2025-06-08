using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Models
{
    public class Medico
    {
        public int Id { get; set; }
        public int Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        // La Contrasena solo se usará al registrar, no al listar o actualizar
        public string Contrasena { get; set; } // Incluida para el registro
        public int EspecialidadId { get; set; } // Es INT, no INT? porque un médico siempre tiene especialidad
        public bool Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}