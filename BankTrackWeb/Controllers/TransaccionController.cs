using BankTrackWeb.Models;
using BankTrackWeb.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankTrackWeb.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly ILogger<TransaccionController> _logger;
        private readonly TransaccionRepository _transaccionRepository;
        public TransaccionController(ILogger<TransaccionController> logger, TransaccionRepository transaccionRepository)
        {
            _logger = logger;
            _transaccionRepository = transaccionRepository;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Transaccion> _listaTransacciones = await _transaccionRepository.Listar();
            return View(_listaTransacciones);
        }
        // GET
        public IActionResult Create()
        {
            var result = _transaccionRepository.ListarCuentas();
            if (result == null)
            {
                TempData["error"] = "Primero debe cargar cuentas bancarias para hacer transacciones";
                RedirectToAction("Index");
            }
            var result2 = _transaccionRepository.ListarCategorias();
            if (result2 == null)
            {
                TempData["error"] = "Primero debe cargar categorias para hacer transacciones";
                RedirectToAction("Index");
            }
            ViewBag.Cuentas = result;
            ViewBag.Categorias = result2;   
            return View(new Transaccion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaccion transaccion)
        {
            ModelState.Remove("Categoria.IconoCategoria");
            ModelState.Remove("Categoria.NombreCategoria");
            ModelState.Remove("Categoria.TipoTransaccion");
            ModelState.Remove("Categoria.DescripcionCategoria");
            ModelState.Remove("CuentaBancaria.Cliente");
            if (ModelState.IsValid)
            {
                // Busco cuenta bancaria
                var cuenta = Cuentas().FirstOrDefault(c => c.IdCuenta == transaccion.CuentaBancaria.IdCuenta);
                if (cuenta == null)
                {
                    TempData["error"] = "No se encontró cuenta bancaria";
                    ViewBag.Cuentas = _transaccionRepository.ListarCuentas();
                    ViewBag.Categorias = _transaccionRepository.ListarCategorias();
                    return View();
                }
                transaccion.CuentaBancaria = cuenta;
                //Busco categoria
                var categoria = Categorias().FirstOrDefault(c => c.IdCategoria == transaccion.Categoria.IdCategoria);
                if (categoria == null)
                {
                    TempData["error"] = "No se encontró la categoría";
                    ViewBag.Cuentas = _transaccionRepository.ListarCuentas();
                    ViewBag.Categorias = _transaccionRepository.ListarCategorias();
                    return View();
                }
                transaccion.Categoria = categoria;
                if (transaccion.Categoria.TipoTransaccion.Aumenta == false && transaccion.CuentaBancaria.SaldoActual < transaccion.Monto)
                {
                    TempData["error"] = "No tiene saldo suficiente para realizar esa transacción";
                    ViewBag.Cuentas = _transaccionRepository.ListarCuentas();
                    ViewBag.Categorias = _transaccionRepository.ListarCategorias();
                    return View();
                }
                bool _resultado = await _transaccionRepository.Guardar(transaccion);

                if (_resultado)
                {
                    TempData["success"] = "Transacción realizada con éxito";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "No se pudo agregar la transacción";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Cuentas = _transaccionRepository.ListarCuentas();
            ViewBag.Categorias = _transaccionRepository.ListarCategorias();
            return View();
        }
        [NonAction]
        public List<CuentaBancaria> Cuentas()
        {
            if (_transaccionRepository != null)
            {
                return _transaccionRepository.ListarCuentas();
            }
            return new List<CuentaBancaria>();
        }
        [NonAction]
        public List<Categoria> Categorias()
        {
            if (_transaccionRepository != null)
            {
                return _transaccionRepository.ListarCategorias();
            }
            return new List<Categoria>();
        }
    }
}
