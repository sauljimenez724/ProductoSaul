using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductoSaul.Models;

namespace ProductoSaul.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
    public DbSet<PersonaEntity> Personas { get; set; }
    public DbSet<ComentarioEntity> Comentarios { get; set; }
    }
}
