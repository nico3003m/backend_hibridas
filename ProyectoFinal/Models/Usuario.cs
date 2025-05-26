using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Identificacion { get; set; }

        [JsonIgnore]
        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }


}