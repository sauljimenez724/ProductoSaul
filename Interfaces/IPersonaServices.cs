using ProductoSaul.Models;

namespace ProductoSaul.Interfaces
{
    public interface IPersonaServices
    {
        Task<List<PersonaEntity>> ListaPersonaAsync();
        Task<PersonaEntity?> PersonaPorIdAsycn(int idPersona);
        Task<PersonaEntity?> CrearPersonaAsyncn(PersonaEntity persona);
        Task<PersonaEntity> ActualizarPersonaAsync(PersonaEntity persona);
        Task<PersonaEntity> EliminarPersonaAsync(int idPersona);
    }
}
