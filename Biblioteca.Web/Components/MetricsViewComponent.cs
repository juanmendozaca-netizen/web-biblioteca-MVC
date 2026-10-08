using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Components
{
    public class MetricsViewComponent : ViewComponent
    {
        private readonly LibroRepositorio _libroRepo;
        private readonly SocioRepositorio _socioRepo;
        private readonly PrestamoRepositorio _prestamoRepo;

        public MetricsViewComponent(LibroRepositorio libroRepo, SocioRepositorio socioRepo, PrestamoRepositorio prestamoRepo)
        {
            _libroRepo = libroRepo;
            _socioRepo = socioRepo;
            _prestamoRepo = prestamoRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var meses = (await _prestamoRepo.GetPrestamosMonthlyAsync(6)).ToArray();
            var model = new MetricsViewModel
            {
                LibrosActivos = await _libroRepo.GetCountAsync(),
                SociosActivos = await _socioRepo.GetCountAsync(),
                PrestamosActivos = await _prestamoRepo.GetPrestamosActivosCountAsync(),
                PrestamosVencidos = await _prestamoRepo.GetPrestamosVencidosCountAsync(),
                MesesLabels = meses.Select(m => m.Label).ToArray(),
                MesesCounts = meses.Select(m => m.Count).ToArray()
            };

            return View(model);
        }
    }
}
