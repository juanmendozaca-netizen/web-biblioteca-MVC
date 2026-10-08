namespace Biblioteca.Web.Models
{
    public class PrestamoReporte
    {
        public int PrestamoId { get; set; }
        public string SocioNombre { get; set; } = string.Empty;
        public string LibroTitulo { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
