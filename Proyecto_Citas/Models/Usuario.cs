using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App_Citas_medicas_backend.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public int Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
        public int? EspecialidadId { get; set; } // int? es crucial para permitir NULL
        public bool Estatus { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}