using Microsoft.AspNetCore.Mvc;
using ProyectoFinal.Models;
using ProyectoFinal.Data;
using ProyectoFinal.Service;
using Microsoft.EntityFrameworkCore;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == loginData.Correo && u.Password == loginData.Password);

            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }

            // Generar JWT
            var token = _jwtService.GenerateToken(usuario.Id, usuario.Correo);

            return Ok(new
            {
                token,
                usuario.Id,
                usuario.Nombre,
                usuario.Correo
            });
        }
    }
}
