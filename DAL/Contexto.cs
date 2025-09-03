
using RegistroJugadores.Models;
using Microsoft.EntityFrameworkCore;

namespace RegistroJugadores.DAL
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }
        public DbSet<Jugadores> Jugadores { get; set; } = null!;
    }
}
