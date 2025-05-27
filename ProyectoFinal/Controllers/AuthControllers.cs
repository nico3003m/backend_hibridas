using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Models;
using ProyectoFinal.Data;

using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;


        public AuthController(AppDbContext context)
        {
            _context = context;

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            var existe = await _context.Usuarios.AnyAsync(u => u.Identificacion == usuario.Identificacion);
            if (existe)
            {
                return BadRequest("Usuario ya registrado");
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == loginData.Correo && u.Password == loginData.Password);

            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }

            // Guardar el Id en session
            HttpContext.Session.SetInt32("UserId", usuario.Id);

            return Ok(new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Correo
            });
        }
    }
}
