using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers
{
    public class SociosController : Controller
    {
        private readonly SocioRepositorio _socioRepo;

        public SociosController(SocioRepositorio socioRepo)
        {
            _socioRepo = socioRepo;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _socioRepo.GetAllAsync();
            ViewData["Title"] = "Listado de socios";
            return View(lista);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Crear socio";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Socio socio)
        {
            if (!ModelState.IsValid) return View(socio);

            // verificar DNI
            if (await _socioRepo.DniExistsAsync(socio.DNI))
            {
                ModelState.AddModelError("DNI", "El DNI ya existe en la base de datos.");
                return View(socio);
            }

            await _socioRepo.CreateAsync(socio);
            TempData["Message"] = "Socio creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
