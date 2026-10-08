using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly PrestamoRepositorio _prestamoRepo;

        public PrestamosController(PrestamoRepositorio prestamoRepo)
        {
            _prestamoRepo = prestamoRepo;
        }

        public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
        {
            ViewData["Title"] = "Reporte de préstamos";
            if (!desde.HasValue || !hasta.HasValue)
            {
                // por defecto último mes
                desde = DateTime.Today.AddDays(-30);
                hasta = DateTime.Today;
            }

            ViewData["Desde"] = desde.Value.ToString("yyyy-MM-dd");
            ViewData["Hasta"] = hasta.Value.ToString("yyyy-MM-dd");

            var lista = await _prestamoRepo.GetReporteAsync(desde.Value, hasta.Value);
            return View(lista);
        }
    }
}
