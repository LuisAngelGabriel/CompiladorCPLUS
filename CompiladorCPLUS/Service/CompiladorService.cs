using CompiladorCPLUS.DAL;
using CompiladorCPLUS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CompiladorCPLUS.Service
{
    public class CompiladorService(IDbContextFactory<Contexto> DbFactory)
    {
        private async Task<bool> Existe(int id)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Compilaciones.AnyAsync(c => c.CompilacionId == id);
        }

        private async Task<bool> Insertar(Compilacion compilacion)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Compilaciones.Add(compilacion);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Compilacion compilacion)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Update(compilacion);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> Guardar(Compilacion compilacion)
        {
            if (!await Existe(compilacion.CompilacionId))
                return await Insertar(compilacion);
            else
                return await Modificar(compilacion);
        }

        public async Task<List<Compilacion>> Listar(Expression<Func<Compilacion, bool>> criterio)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Compilaciones
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Compilaciones
                .Where(c => c.CompilacionId == id)
                .ExecuteDeleteAsync() > 0;
        }
    }
}
