using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Protege todas las rutas del controlador
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // Obtiene el ID del usuario autenticado desde el token JWT
        private int ObtenerUsuarioIdDesdeToken()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (usuarioIdClaim == null)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Le decimos al compilador que aquí estamos seguros de que no es null
            Console.WriteLine($"[DEBUG] usuarioIdClaim raw value: '{usuarioIdClaim!}'");

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                Console.WriteLine($"[ERROR] usuarioIdClaim NO es un entero válido");
                throw new Exception("Claim de usuario no es un número válido");
            }
            Console.WriteLine($"[DEBUG] usuarioId parsed to int: {usuarioId}");

            return usuarioId;
        }
        // GET: api/cliente/list
        [Authorize]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            int usuarioId = ObtenerUsuarioIdDesdeToken();

            var clientes = await _context.Clientes
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(clientes);
        }

        // POST: api/cliente
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente([FromBody] Cliente cliente)
        {
            int usuarioId = ObtenerUsuarioIdDesdeToken();

            // Ignorar el UsuarioId que venga del frontend
            cliente.UsuarioId = usuarioId;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // PUT: api/cliente/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] Cliente cliente)
        {
            int usuarioId = ObtenerUsuarioIdDesdeToken();

            if (id != cliente.Id)
                return BadRequest("El ID del cliente no coincide.");

            var clienteExistente = await _context.Clientes.FindAsync(id);
            if (clienteExistente == null || clienteExistente.UsuarioId != usuarioId)
                return NotFound("Cliente no encontrado o no autorizado.");

            // Actualizar campos
            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Correo = cliente.Correo;
            clienteExistente.Identificacion = cliente.Identificacion;
            clienteExistente.Direccion = cliente.Direccion;
            clienteExistente.Telefono = cliente.Telefono;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cliente/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            int usuarioId = ObtenerUsuarioIdDesdeToken();

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null || cliente.UsuarioId != usuarioId)
                return NotFound("Cliente no encontrado o no autorizado.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
