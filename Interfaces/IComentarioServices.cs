using ProductoSaul.Models;

namespace ProductoSaul.Interfaces
{
    public interface IComentarioServices
    {
        Task<List<ComentarioEntity>> ListaComentarioAsync();
        Task<ComentarioEntity?> ComentarioPorIdAsycn(int idComentario);
        Task<ComentarioEntity?> CrearComentarioAsyncn(ComentarioEntity comentario);
        Task<ComentarioEntity> ActualizarComentarioAsync(ComentarioEntity comentario);
        Task<ComentarioEntity> EliminarComentarioAsync(int idComentario);
    }
}
