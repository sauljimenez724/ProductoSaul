using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductoSaul.Models
{
    public class PersonaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlumnoId { get; set; }
        [StringLength(100)]
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(250)]
        [Required]
        public string Apellidos { get; set; } = string.Empty;
        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Correo electrónico no válido")]
        public string Correo { get; set; } = string.Empty;
        [RegularExpression(@"^\+(?:[0-9] ?){6,14}[0-9]$", ErrorMessage = "El número telefónico no es válido.")]
        public string TelefonoWhatsapp { get; set; } = string.Empty;
    public virtual ICollection<ComentarioEntity> Comentarios { get; set; }
    }
}
