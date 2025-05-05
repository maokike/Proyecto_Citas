namespace App_Citas_medicas_backend.Data
{
    public class Cita
    {
        public object Id { get; internal set; }
        public object PacienteId { get; internal set; }
        public object MedicoId { get; internal set; }
        public object Fecha { get; internal set; }
        public object Hora { get; internal set; }
        public object Estado { get; internal set; }
        public object EspecialidadId { get; internal set; }
    }
}