using System.ComponentModel.DataAnnotations;
namespace RegistroJugadores.Models
{
    public class Jugadores
    {
        [Key]
        public int JugadorId { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        public string Nombre { get; set; } = null!;



        [Range(0, double.MaxValue, ErrorMessage = "No puede ser negativo")]
        public double Partidas { get; set; }

    }
}
