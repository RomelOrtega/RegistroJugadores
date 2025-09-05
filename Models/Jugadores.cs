using System.ComponentModel.DataAnnotations;
namespace RegistroJugadores.Models
{
    public class Jugadores
    {
        [Key]
        public int JugadorId { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        public string Nombre { get; set; } = null!;



        [Range(0, int.MaxValue, ErrorMessage = "No puede ser negativo")]
        public int Partidas { get; set; }

    }
}
