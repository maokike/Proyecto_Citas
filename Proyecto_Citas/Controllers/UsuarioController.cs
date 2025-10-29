using App_Citas_medicas_backend.Data; // Para acceder a UsuarioData
using App_Citas_medicas_backend.Models; // Para tus modelos (Usuario, DashboardDatosCompleto, etc.)
using System; // Para Guid, DateTime, Exception, Console
using System.Collections.Generic; // Para List
using System.Web.Http; // Para ApiController, HttpGet, HttpPost, Route
using System.Web.Http.Cors; // Si usas [EnableCors]
using System.Net; // Para HttpStatusCode
using System.Net.Http; // Para HttpResponseMessage
using System.Data.SqlClient; // Para capturar SqlException
using System.Globalization; // Para CultureInfo (ya no se usa directamente aquí, pero es un using estándar)


namespace App_Citas_medicas_backend.Controllers
{
    // Si necesitas CORS, asegúrate de que esté configurado en WebApiConfig.cs
    // O puedes añadir el atributo [EnableCors(origins: "*", headers: "*", methods: "*")] aquí,
    // pero es mejor centralizarlo si ya lo tienes en WebApiConfig.cs.

    public class UsuarioController : ApiController
    {
        // GET api/Usuario
        [HttpGet]
        public List<Usuario> Get()
        {
            return UsuarioData.ListarUsuarios();
        }

        // GET api/Usuario/{id}
        [HttpGet]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage ObtenerUsuario(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Error: ID inválido." });
            }

            var usuarios = UsuarioData.ObtenerUsuario(id);

            if (usuarios == null || usuarios.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new { success = false, mensaje = "Usuario no encontrado." });
            }

            return Request.CreateResponse(HttpStatusCode.OK, usuarios);
        }

        // POST api/Usuario
        [HttpPost]
        public IHttpActionResult Post([FromBody] Usuario oUsuario)
        {
            if (oUsuario == null)
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "Datos de usuario no proporcionados." });
            }

            try
            {
                if (oUsuario.Rol?.ToLower() == "medico" && !oUsuario.EspecialidadId.HasValue)
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "Para el rol 'Medico', la EspecialidadId es obligatoria." });
                }

                bool registrado = UsuarioData.RegistrarUsuario(oUsuario);

                if (registrado)
                {
                    return Ok(new { success = true, mensaje = "Usuario registrado correctamente." });
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "No se pudo registrar el usuario. Verifique los datos o si ya existe." });
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL al registrar usuario en el controlador: {sqlEx.Message}");
                return Content(HttpStatusCode.InternalServerError, new { success = false, mensaje = $"Error de base de datos al registrar usuario: {sqlEx.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al registrar usuario en el controlador: {ex.Message}");
                return Content(HttpStatusCode.InternalServerError, new { success = false, mensaje = $"Ocurrió un error inesperado: {ex.Message}" });
            }
        }

        // PUT api/Usuario/{id}
        [HttpPut]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage ActualizarUsuario(int id, [FromBody] Usuario oUsuario)
        {
            if (oUsuario == null || id <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Error: Datos inválidos." });
            }

            oUsuario.Id = id;

            if (oUsuario.Rol?.ToLower() == "medico" && !oUsuario.EspecialidadId.HasValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Para el rol 'Medico', la EspecialidadId es obligatoria." });
            }

            bool actualizado = UsuarioData.ActualizarUsuario(oUsuario);

            if (actualizado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, mensaje = "Usuario actualizado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { success = false, mensaje = "Error al actualizar el usuario. Verifique los datos." });
            }
        }

        // DELETE api/Usuario/{id}
        [HttpDelete]
        [Route("api/Usuario/{id}")]
        public HttpResponseMessage EliminarUsuario(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = false, mensaje = "Error: ID de usuario inválido." });
            }

            bool eliminado = UsuarioData.EliminarUsuario(id);

            if (eliminado)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, mensaje = "Usuario eliminado correctamente." });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { success = false, mensaje = "Error al eliminar el usuario o usuario no encontrado." });
            }
        }

        // *************************************************************************************
        // ENDPOINT PARA SOLICITAR RESTABLECIMIENTO DE CONTRASEÑA
        // *************************************************************************************
        [HttpPost]
        [Route("api/Usuario/SolicitarRestablecimiento")]
        public IHttpActionResult SolicitarRestablecimiento([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Content(HttpStatusCode.BadRequest, new { success = false, mensaje = "El correo electrónico es obligatorio." });
            }

            try
            {
                // 1. Buscar al usuario por su correo electrónico
                var usuario = UsuarioData.ObtenerUsuarioPorEmail(email);

                // Por seguridad, siempre se devuelve un mensaje genérico para no revelar si el correo existe o no.
                if (usuario == null)
                {
                    Console.WriteLine($"Solicitud de restablecimiento para correo no existente o error de DB: {email}");
                    return Content(HttpStatusCode.OK, new { success = true, mensaje = "Si el correo electrónico está registrado, recibirás un enlace para restablecer tu contraseña." });
                }

                // 2. Generar un token único y seguro
                string token = Guid.NewGuid().ToString("N"); // "N" formato sin guiones
                DateTime expiraEn = DateTime.UtcNow.AddHours(1); // El token expira en 1 hora (ajusta si es necesario)

                // 3. Guardar el token en la base de datos
                bool tokenGuardado = UsuarioData.GuardarTokenRestablecimientoContrasena(usuario.Id, token, expiraEn);

                if (!tokenGuardado)
                {
                    Console.WriteLine($"No se pudo guardar el token para el usuario {usuario.Id} - {email}");
                    return Content(HttpStatusCode.InternalServerError, new { success = false, mensaje = "No se pudo procesar la solicitud de restablecimiento. Inténtalo de nuevo." });
                }

                // 4. (¡FALTA AQUÍ!): Enviar el correo electrónico al usuario con el enlace del token
                // Este es el siguiente gran paso, que requiere un servicio de email.
                // string enlaceRestablecimiento = $"http://tufrontend.com/restablecer-contrasena?token={token}&email={Uri.EscapeDataString(email)}";
                // EnviarEmail(usuario.Email, "Restablece tu Contraseña", $"Haz clic aquí: {enlaceRestablecimiento}");


                Console.WriteLine($"Token generado y guardado para {email}: {token}");
                return Content(HttpStatusCode.OK, new { success = true, mensaje = "Si el correo electrónico está registrado, recibirás un enlace para restablecer tu contraseña." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al solicitar restablecimiento de contraseña para {email}: {ex.Message}");
                return Content(HttpStatusCode.InternalServerError, new { success = false, mensaje = $"Ocurrió un error inesperado al procesar tu solicitud: {ex.Message}" });
            }
        }

        // *************************************************************************************
        // NUEVO ENDPOINT: OBTENER DATOS DEL DASHBOARD
        // *************************************************************************************
        [HttpGet]
        [Route("api/Usuario/DashboardMetrics")] // Endpoint para el dashboard
        public IHttpActionResult GetDashboardMetrics()
        {
            try
            {
                DashboardDatosCompleto datosDashboard = UsuarioData.ObtenerDatosDashboard();
                return Ok(datosDashboard); // Devuelve todos los datos del dashboard en formato JSON
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos del dashboard: {ex.Message}");
                return InternalServerError(ex); // Devuelve un error 500 si algo falla
            }
        }

    } // Fin de la clase UsuarioController
} // Fin del namespace