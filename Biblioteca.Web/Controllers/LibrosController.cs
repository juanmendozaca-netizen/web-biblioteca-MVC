using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Web.Controllers
{
    public class LibrosController : Controller
    {
        private readonly LibroRepositorio _libroRepo;

        public LibrosController(LibroRepositorio libroRepo)
        {
            _libroRepo = libroRepo;
        }

        public async Task<IActionResult> Index(string? titulo)
        {
            var lista = await _libroRepo.GetAllAsync(titulo);
            ViewData["Title"] = "Listado de libros";
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var libro = await _libroRepo.GetByIdAsync(id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Crear libro";
            var autores = await _libroRepo.GetAutoresAsync();
            ViewData["Autores"] = new SelectList(autores, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                var autores = await _libroRepo.GetAutoresAsync();
                ViewData["Autores"] = new SelectList(autores, "Id", "Nombre");
                return View(libro);
            }

            await _libroRepo.CreateAsync(libro);
            TempData["Message"] = "Libro creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _libroRepo.GetByIdAsync(id);
            if (libro == null) return NotFound();
            var autores = await _libroRepo.GetAutoresAsync();
            ViewData["Autores"] = new SelectList(autores, "Id", "Nombre", libro.AutorId);
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libro)
        {
            if (id != libro.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                var autores = await _libroRepo.GetAutoresAsync();
                ViewData["Autores"] = new SelectList(autores, "Id", "Nombre", libro.AutorId);
                return View(libro);
            }

            await _libroRepo.UpdateAsync(libro);
            TempData["Message"] = "Libro actualizado.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _libroRepo.GetByIdAsync(id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _libroRepo.DeleteAsync(id);
            TempData["Message"] = "Libro eliminado (lógicamente).";
            return RedirectToAction(nameof(Index));
        }
    }
}
