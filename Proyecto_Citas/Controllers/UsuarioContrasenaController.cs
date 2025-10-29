using App_Citas_medicas_backend.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App_Citas_medicas_backend.Controllers
{
    [RoutePrefix("api/auth")]
    public class UsuarioContrasenaController : ApiController
    {
        private readonly UsuarioData usuarioData;

        public UsuarioContrasenaController()
        {
            usuarioData = new UsuarioData(); // Ajustado para usar un constructor sin parámetros
        }

        [HttpPost]
        [Route("forgot-password")]
        public IHttpActionResult ForgotPassword([FromBody] dynamic body)
        {
            string email = body.email;
            if (string.IsNullOrEmpty(email))
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "El correo es requerido." });

            // Cambiado para usar el método estático correctamente
            var usuario = UsuarioData.ObtenerUsuarioPorEmail(email);
            if (usuario != null)
                return Ok(new { success = true, message = "Correo verificado." });
            else
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "El correo no está registrado." });
        }

        [HttpPost]
        [Route("reset-password")]
        public IHttpActionResult ResetPassword([FromBody] dynamic body)
        {
            string email = body.Email;
            int id = body.Id;
            string nuevaContrasena = body.newPassword;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nuevaContrasena) || id == 0)
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "Datos incompletos." });

            // Cambiado para usar el método estático ActualizarContrasena
            bool actualizado = UsuarioData.ActualizarContrasena(id, nuevaContrasena);
            if (actualizado)
                return Ok(new { success = true, message = "Contraseña actualizada correctamente." });
            else
                return Content(HttpStatusCode.BadRequest, new { success = false, message = "No se pudo actualizar la contraseña. Verifica el ID y el correo." });
        }
    }
}