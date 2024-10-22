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

            return View(new Transaccion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaccion transaccion)
        {
            // Remover validación para la propiedad Cliente
            ModelState.Remove("Transaccion");
            if (ModelState.IsValid)
            {
                var cuenta = _transaccionRepository.ListarCuentas().FirstOrDefault(x => x.NumeroCuenta == transaccion.CuentaBancaria.NumeroCuenta);
                if (cuenta == null)
                {
                    TempData["error"] = "No se encontró cuenta bancaria";
                    return View();
                }
            //    cuenta.Cliente = cliente;
            //    var _listaCuentas = await _cuentaBancariaRepository.Listar();
            //    var cuentaEncontrada = _listaCuentas.FirstOrDefault(x => x.NumeroCuenta == cuenta.NumeroCuenta);
            //    if (cuenta.IdCuenta == 0)
            //    {
            //        if (cuentaEncontrada != null)
            //        {
            //            TempData["error"] = "Ya existe una cuenta con ese número.";
            //            ListarClientes();
            //            return View();
            //        }

            //        bool _resultado = await _cuentaBancariaRepository.Guardar(cuenta);

            //        if (_resultado)
            //        {
            //            TempData["success"] = "Cuenta bancaria agregada correctamente";
            //            return RedirectToAction("Index");
            //        }
            //        else
            //        {
            //            TempData["error"] = "No se pudo agregar la cuenta";
            //            return RedirectToAction("Index");
            //        }
            //    }
            //    else
            //    {
            //        if (cuentaEncontrada == null)
            //        {
            //            TempData["error"] = "No se encontró cuenta con ese número.";
            //            return RedirectToAction("Index");
            //        }
            //        bool _resultado = await _cuentaBancariaRepository.Modificar(cuenta);

            //        if (_resultado)
            //        {
            //            TempData["success"] = "Cuenta bancaria modificada correctamente";
            //            return RedirectToAction("Index");
            //        }
            //        else
            //        {
            //            TempData["error"] = "No se pudo modificar la cuenta";
            //            ListarClientes();
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));

            }
            //ListarClientes();
            return View();
        }
    }
}
