using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroJugadores.Models
{
    public class Jugadores
    {
        [Key]
        public int JugadorId { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "No se permiten caracteres especiales")]
        [Required(ErrorMessage = "Este campo es requerido")]
        [MaxLength(100)]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "Por favor digitar la cantidad de Victorias.")]
        [Range(0, double.MaxValue, ErrorMessage = "No puede ser negativo")]
        public double Victorias { get; set; } = 0;
        public int Derrotas { get; set; } = 0;
        public int Empates { get; set; } = 0;
        [InverseProperty(nameof(Models.Movimientos.Jugador))]
        public virtual ICollection<Movimientos> Movimientos { get; set; } = new List<Movimientos>();
    }
}
