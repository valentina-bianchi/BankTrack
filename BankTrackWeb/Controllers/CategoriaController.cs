using BankTrackWeb.Models;
using BankTrackWeb.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankTrackWeb.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ILogger<CategoriaController> _logger;
        private readonly IGenericRepository<Categoria> _categoriaRepository;
        public CategoriaController(ILogger<CategoriaController> logger, IGenericRepository<Categoria> categoriaRepository)
        {
            _logger = logger;
            _categoriaRepository = categoriaRepository;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Categoria> _listaCategorias = await _categoriaRepository.Listar();
            return View(_listaCategorias);
        }
        // GET
        public IActionResult CreateOrEdit(int? id = 0)
        {
            var result = ListarTiposTransacciones();
            if (result == null)
            {
                TempData["error"] = "Primero debe cargar tipos de transacción para crear una categoria.";
                RedirectToAction("Index");
            }
            if (id == 0 || id == null)
            {
                return View(new Categoria());
            }

            var categorias = _categoriaRepository.Listar().Result;
            var categoriaFromDb = categorias.FirstOrDefault(c => c.IdCategoria == id);

            if (categoriaFromDb == null)
            {
                return NotFound();
            }

            return View(categoriaFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Categoria categoria)
        {
            if (categoria.TipoTransaccion?.IdTipoTransaccion != 0)
            {
                categoria.TipoTransaccion = ListarTiposTransacciones()
                    .FirstOrDefault(t => t.IdTipoTransaccion == categoria.TipoTransaccion.IdTipoTransaccion);
                ModelState.Remove("TipoTransaccion.NombreTipo");
            }
            if (ModelState.IsValid)
            {
                var _listaCategorias = await _categoriaRepository.Listar();
                var categoriaEncontrada = _listaCategorias.FirstOrDefault(x => x.NombreCategoria == categoria.NombreCategoria);
                if (categoria.IdCategoria == 0)
                {
                    if (categoriaEncontrada != null)
                    {
                        TempData["error"] = "Ya existe una categoria con ese nombre.";
                        ListarTiposTransacciones();
                        return View();
                    }

                    bool _resultado = await _categoriaRepository.Guardar(categoria);

                    if (_resultado)
                    {
                        TempData["success"] = "Categoría agregada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "No se pudo agregar la categoria";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (categoriaEncontrada == null)
                    {
                        TempData["error"] = "No se encontró categoria con ese nombre.";
                        return RedirectToAction("Index");
                    }
                    bool _resultado = await _categoriaRepository.Modificar(categoria);

                    if (_resultado)
                    {
                        TempData["success"] = "Categoría modificada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "No se pudo modificar la categoría";
                        ListarTiposTransacciones();
                    }
                }
                return RedirectToAction(nameof(Index));

            }
            ListarTiposTransacciones();
            return View(categoria);
        }


        // GET - Delete View Asíncrono
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categorias = await _categoriaRepository.Listar();
            var categoriaFromDb = categorias.FirstOrDefault(c => c.IdCategoria == id);

            if (categoriaFromDb == null)
            {
                return NotFound();
            }

            return View(categoriaFromDb);
        }

        // POST - Asíncrono para eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var categorias = await _categoriaRepository.Listar();
            var categoriaFromDb = categorias.FirstOrDefault(c => c.IdCategoria == id);

            if (categoriaFromDb == null)
            {
                return NotFound();
            }

            bool resultado = await _categoriaRepository.Eliminar(id.Value);

            if (resultado)
            {
                TempData["success"] = "Categoría eliminada correctamente";
                return RedirectToAction("Index");
            }
            TempData["error"] = "No se pudo eliminar la categoría";
            return RedirectToAction("Index");
        }
        [NonAction]
        public List<TipoTransaccion> ListarTiposTransacciones()
        {
            var categoriaRepo = _categoriaRepository as CategoriaRepository;

            if (categoriaRepo != null)
            {
                var tipos = categoriaRepo.ListarTiposTransaccion();
                TipoTransaccion DefaultTipo = new TipoTransaccion() { IdTipoTransaccion = 0, NombreTipo = "Elegir un tipo de transacción" };
                tipos.Insert(0, DefaultTipo);
                ViewBag.TiposTransacciones = tipos;
                return tipos;
            }
            return null;
        }
    }
}
