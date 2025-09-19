using RegistroJugadores.Models;
using RegistroJugadores.DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RegistroJugadores.Services
{
    public class JugadoresService(IDbContextFactory<Contexto> DbFactory)
    {
        public async Task<bool> Guardar(Jugadores jugador)
        {
            if (!await Existe(j => j.JugadorId == jugador.JugadorId))
            {
                return await Insertar(jugador);
            }
            else
            {
                return await Modificar(jugador);
            }
        }

        public async Task<bool> Existe(Expression<Func<Jugadores, bool>> predicate)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores.AnyAsync(predicate);
        }

        private async Task<bool> Insertar(Jugadores jugador)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Jugadores.Add(jugador);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Jugadores jugador)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Jugadores.Update(jugador);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Jugadores?> Buscar(int jugadorId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores.FirstOrDefaultAsync(j => j.JugadorId == jugadorId);
        }
        public async Task<bool> Eliminar(int jugadorId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores
                 .AsNoTracking()
                 .Where(j => j.JugadorId == jugadorId)
                 .ExecuteDeleteAsync() > 0;
        }
        public async Task<List<Jugadores>> Listar(Expression<Func<Jugadores, bool>> criterio)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores
                .AsNoTracking()
                .Where(criterio)
                .ToListAsync();
        }

        public async Task<List<JugadorEstadistica>> ObtenerEstadisticas()
        {
            using var db = await DbFactory.CreateDbContextAsync();
            var jugadores = await db.Jugadores.ToListAsync();
            var lista = new List<JugadorEstadistica>();

            foreach (var jugador in jugadores)
            {
                var victorias = await db.Partidas.CountAsync(p => p.GanadorId == jugador.JugadorId);
                var derrotas = await db.Partidas.CountAsync(p =>
                    p.EstadoPartida == "Finalizada" &&
                    p.GanadorId != jugador.JugadorId &&
                    p.GanadorId != null &&
                    (p.Jugador1Id == jugador.JugadorId || p.Jugador2Id == jugador.JugadorId));
                var empates = await db.Partidas.CountAsync(p =>
                    p.EstadoPartida == "Empate" &&
                    (p.Jugador1Id == jugador.JugadorId || p.Jugador2Id == jugador.JugadorId));

                lista.Add(new JugadorEstadistica
                {
                    JugadorId = jugador.JugadorId,
                    Nombres = jugador.Nombre,
                    Victorias = victorias,
                    Derrotas = derrotas,
                    Empates = empates
                });
            }

            return lista;
        }

        public class JugadorEstadistica
        {
            public int JugadorId { get; set; }
            public string Nombres { get; set; } = string.Empty;
            public int Victorias { get; set; }
            public int Derrotas { get; set; }
            public int Empates { get; set; }
        }

        public async Task<Partidas?> ObtenerPartidaConMovimientos(int partidaId)
        {
            using var context = DbFactory.CreateDbContext();
            return await context.Partidas
                .Include(p => p.Movimientos)
                .FirstOrDefaultAsync(p => p.PartidaId == partidaId);
        }

    }
}
