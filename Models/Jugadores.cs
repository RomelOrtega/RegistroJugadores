using System.ComponentModel.DataAnnotations;
namespace RegistroJugadores.Models
{
    public class Jugadores
    {
        [Key]
        public int JugadorId { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]

        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "Este campo es requerido")]
        public double Partidas { get; set; }
    }
}
