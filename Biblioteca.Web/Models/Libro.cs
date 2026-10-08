using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public int AutorId { get; set; }

        // Nombre del autor para mostrar en listados (no es enviado al insert)
        public string? AutorNombre { get; set; }

        [StringLength(100)]
        public string? Genero { get; set; }

        [StringLength(100)]
        public string? Editorial { get; set; }

        [Range(0, 9999)]
        public int? Anio { get; set; }

        [Range(0, 1000)]
        public int Ejemplares { get; set; }

        public bool Activo { get; set; } = true;
    }
}
