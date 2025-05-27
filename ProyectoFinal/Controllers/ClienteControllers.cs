using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Data;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Quitamos el Authorize porque no usaremos JWT
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // Método para obtener el UserId desde Session
        private int? GetUserIdFromSession()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: api/cliente/list
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var usuarioId = GetUserIdFromSession();
            if (usuarioId == null)
            {
                return Unauthorized("No hay usuario logueado");
            }

            var clientes = await _context.Clientes
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(clientes);
        }

        // POST: api/cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> CrearCliente([FromBody] Cliente cliente)
        {
            var usuarioId = GetUserIdFromSession();
            if (usuarioId == null)
            {
                return Unauthorized("No hay usuario logueado");
            }

            cliente.UsuarioId = usuarioId.Value;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // PUT: api/cliente/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] Cliente cliente)
        {
            var usuarioId = GetUserIdFromSession();
            if (usuarioId == null)
            {
                return Unauthorized("No hay usuario logueado");
            }

            if (id != cliente.Id)
                return BadRequest("El ID del cliente no coincide.");

            var clienteExistente = await _context.Clientes.FindAsync(id);
            if (clienteExistente == null || clienteExistente.UsuarioId != usuarioId)
                return NotFound("Cliente no encontrado o no autorizado.");

            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Correo = cliente.Correo;
            clienteExistente.Identificacion = cliente.Identificacion;
            clienteExistente.Direccion = cliente.Direccion;
            clienteExistente.Telefono = cliente.Telefono;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/cliente/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var usuarioId = GetUserIdFromSession();
            if (usuarioId == null)
            {
                return Unauthorized("No hay usuario logueado");
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null || cliente.UsuarioId != usuarioId)
                return NotFound("Cliente no encontrado o no autorizado.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
