using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RegistroJugadores.DAL;
using RegistroJugadores.Models;


namespace RegistroJugadores.Services
{
    public class PartidasService
    {
        private readonly IDbContextFactory<Contexto> _DbFactory;
        private readonly ILogger<PartidasService> _Logger;

        public PartidasService(IDbContextFactory<Contexto> DbFactory, ILogger<PartidasService> Logger)
        {
            _DbFactory = DbFactory;
            _Logger = Logger;
        }

        private async Task<bool> Existe(int partidaId)
        {
            try
            {
                await using var contexto = await _DbFactory.CreateDbContextAsync();
                return await contexto.Partidas.AnyAsync(p => p.PartidaId == partidaId);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error verificando existencia de la aprtia{PartidaId}", partidaId);
                return false;
            }
        }
        private async Task<bool> Insertar(Partidas partida)
        {
            try
            {
                await using var contexto = await _DbFactory.CreateDbContextAsync();
                contexto.Partidas.Add(partida);
                return await contexto.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error insertando la partida de {Jugador1Id} VS {Jugador2Id}", partida.Jugador1Id, partida.Jugador2Id);
                return false;
            }
        }

        private async Task<bool> Modificar(Partidas partida)
        {
            try
            {
                await using var contexto = await _DbFactory.CreateDbContextAsync();
                contexto.Partidas.Update(partida);
                return await contexto.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error modificando la partida {PartidaId}", partida.PartidaId);
                return false;
            }
        }
        public async Task<bool> Guardar(Partidas partida)
        {
            try
            {
                return !await Existe(partida.PartidaId)
                ? await Insertar(partida)
                : await Modificar(partida);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error guardando la partida {PartidaId}", partida.PartidaId);
                return false;
            }
        }
        public async Task<Partidas> Buscar(int partidaId)
        {
            try
            {
                await using var contexto = await _DbFactory.CreateDbContextAsync();
                return await contexto.Partidas
                    .Include(p => p.Jugador1)
                    .Include(p => p.Jugador2)
                    .Include(p => p.Ganador)
                    .Include(p => p.TurnoJugador)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PartidaId == partidaId);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error buscando la partida{PartidaId}", partidaId);
                return null;
            }
        }

        public async Task<bool> Eliminar(int partidaId)
        {
            try
            {
                await using var contexto = await _DbFactory.CreateDbContextAsync();
                return await contexto.Partidas.Where(p => p.PartidaId == partidaId).ExecuteDeleteAsync() > 0;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error al eliminar la partida {PartidaId}", partidaId);
                return false;
            }
        }

        public async Task<List<Partidas>> Listar(Expression<Func<Partidas, bool>> criterio)
        {
            try
            {
                await using var contexto = await _DbFactory.CreateDbContextAsync();
                var query = contexto.Partidas
                    .Include(p => p.Jugador1)
                    .Include(p => p.Jugador2)
                    .Include(p => p.Ganador)
                    .Include(p => p.TurnoJugador)
                    .AsQueryable();
                query = query.Where(criterio);
                return await query.AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error listando partidas");
                return new List<Partidas>();
            }
        }
        public async Task<int> TotalPartidasJugadas()
        {
            await using var contexto = await _DbFactory.CreateDbContextAsync();
            return await contexto.Partidas.CountAsync();
        }

        public async Task<int> TotalPartidasGanadas(int jugadorId = 0)
        {
            await using var contexto = await _DbFactory.CreateDbContextAsync();
            return jugadorId == 0
                ? await contexto.Partidas.CountAsync(p => p.GanadorId != null)
                : await contexto.Partidas.CountAsync(p => p.GanadorId == jugadorId);
        }
    }
}
