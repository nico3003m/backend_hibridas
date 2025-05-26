using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Identificacion { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        public string Telefono { get; set; }

        // Relación con Usuario
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}