using Microsoft.EntityFrameworkCore;
using ProductoSaul.Data;
using ProductoSaul.Models;
using ProductoSaul.Interfaces;

namespace ProductoSaul.Services
{
    public class ComentarioServices : IComentarioServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ComentarioServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ComentarioEntity>> ListaComentarioAsync()
        {
            using var db = _contextFactory.CreateDbContext();
            return await db.Comentarios.Include(f => f.Persona).OrderBy(m => m.Fecha).ToListAsync();
        }

        public async Task<ComentarioEntity?> ComentarioPorIdAsycn(int idComentario)
        {
            using var db = _contextFactory.CreateDbContext();
            return await db.Comentarios.FindAsync(idComentario);
        }

        public async Task<ComentarioEntity?> CrearComentarioAsyncn(ComentarioEntity comentario)
        {
            using var db = _contextFactory.CreateDbContext();
            db.Comentarios.Add(comentario);
            await db.SaveChangesAsync();
            return comentario;
        }

        public async Task<ComentarioEntity> ActualizarComentarioAsync(ComentarioEntity comentario)
        {
            using var db = _contextFactory.CreateDbContext();
            db.Entry(comentario).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return comentario;
        }

        public async Task<ComentarioEntity> EliminarComentarioAsync(int idComentario)
        {
            using var db = _contextFactory.CreateDbContext();
            ComentarioEntity objeto = await db.Comentarios.FindAsync(idComentario) ?? new();
            if (objeto != null)
            {
                db.Comentarios.Remove(objeto);
                await db.SaveChangesAsync();
            }
            return objeto!;
    }
    
    }
}
