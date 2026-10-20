using Microsoft.EntityFrameworkCore;
using ProductoSaul.Data;
using ProductoSaul.Models;
using ProductoSaul.Interfaces;

namespace ProductoSaul.Services
{
    public class PersonaServices : IPersonaServices
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public PersonaServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PersonaEntity>> ListaPersonaAsync()
        {
            using var db = _contextFactory.CreateDbContext();
            return await db.Personas.OrderBy(m => m.AlumnoId).ToListAsync();
        }

        public async Task<PersonaEntity?> PersonaPorIdAsycn(int idPersona)
        {
            using var db = _contextFactory.CreateDbContext();
            return await db.Personas.FindAsync(idPersona);
        }

        public async Task<PersonaEntity?> CrearPersonaAsyncn(PersonaEntity persona)
        {
            using var db = _contextFactory.CreateDbContext();
            db.Personas.Add(persona);
            await db.SaveChangesAsync();
            return persona;
        }

        public async Task<PersonaEntity> ActualizarPersonaAsync(PersonaEntity persona)
        {
            using var db = _contextFactory.CreateDbContext();
            db.Entry(persona).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return persona;
        }

        public async Task<PersonaEntity> EliminarPersonaAsync(int idPersona)
        {
            using var db = _contextFactory.CreateDbContext();
            PersonaEntity objeto = await db.Personas.FindAsync(idPersona) ?? new();
            if (objeto != null)
            {
                db.Personas.Remove(objeto);
                await db.SaveChangesAsync();
            }
            return objeto!;
        }

    }
}
