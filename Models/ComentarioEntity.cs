using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductoSaul.Models
{
    public class ComentarioEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComentarioId { get; set; }
        [Required]
        public string? Texto { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
        [ForeignKey("AlumnoId")]
    public int PersonaId { get; set; }
    public virtual PersonaEntity? Persona { get; set; }
    }
}
