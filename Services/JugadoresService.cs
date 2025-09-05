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
            if (!await Existe(jugador.JugadorId))
            {
                return await Insertar(jugador);
            }
            else
            {
                return await Modificar(jugador);
            }
        }

        public async Task<bool> Existe(int jugadorId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores.AnyAsync(j => j.JugadorId == jugadorId);
        }

        public async Task<bool> YaExisteNombre(string nombre, int jugadorId = 0)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores.AnyAsync(j => j.Nombre == nombre && j.JugadorId != jugadorId);
        }

        public async Task<bool> ExisteNombre(string nombre)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Jugadores.AnyAsync(j => j.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
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

    }
}
