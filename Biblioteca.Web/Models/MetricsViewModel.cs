namespace Biblioteca.Web.Models
{
    public class MetricsViewModel
    {
        public int LibrosActivos { get; set; }
        public int SociosActivos { get; set; }
        public int PrestamosActivos { get; set; }
        public int PrestamosVencidos { get; set; }
        // Chart data for recent months
        public string[]? MesesLabels { get; set; }
        public int[]? MesesCounts { get; set; }
    }
}
