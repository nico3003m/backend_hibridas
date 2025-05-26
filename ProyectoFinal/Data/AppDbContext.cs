using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Data
{
    // Clase DbContext: se encarga de manejar la conexión con la base de datos y el mapeo de entidades.
    public class AppDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración (como la cadena de conexión)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet<Usuario>: representa la tabla "Usuarios" en la base de datos
        public DbSet<Usuario> Usuarios { get; set; }

        // DbSet<Cliente>: representa la tabla "Clientes" en la base de datos
        public DbSet<Cliente> Clientes { get; set; }

        // Método para configurar relaciones, restricciones y nombres de tabla
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración explícita de la relación: Usuario tiene muchos Clientes
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Clientes)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.UsuarioId);
        }
    }
}