using BankTrackWeb.Models;
using BankTrackWeb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankTrackWeb.Controllers
{
    public class CuentaBancariaController : Controller
    {
        private readonly ILogger<CuentaBancariaController> _logger;
        private readonly IGenericRepository<CuentaBancaria> _cuentaBancariaRepository;
        public CuentaBancariaController(ILogger<CuentaBancariaController> logger, IGenericRepository<CuentaBancaria> cuentaBancariaRepository)
        {
            _logger = logger;
            _cuentaBancariaRepository = cuentaBancariaRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CuentaBancaria> _listaCuentas = await _cuentaBancariaRepository.Listar();
            return View(_listaCuentas);
        }
        // GET
        public IActionResult CreateOrEdit(int? id = 0)
        {
            var clientes = ListarClientes();
            if(clientes == null)
            {
                TempData["error"] = "Primero debe cargar clientes para crearles una cuenta bancaria.";
                RedirectToAction("Index");
            }
            ViewBag.Clientes = ListarClientes();

            if (id == 0 || id == null)
            {
                // Devuelves un objeto Cliente nuevo cuando no hay un id
                return View(new CuentaBancaria());
            }

            var cuentas = _cuentaBancariaRepository.Listar().Result;
            var cuentaFromDb = cuentas.FirstOrDefault(c => c.IdCuenta == id);

            if (cuentaFromDb == null)
            {
                return NotFound();
            }

            return View(cuentaFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(CuentaBancaria cuenta)
        {
            // Buscar el cliente basado en el IdCliente seleccionado
            var cliente = ListarClientes().FirstOrDefault(c => c.IdCliente == cuenta.Cliente.IdCliente);

            if (cliente == null)
            {
                TempData["error"] = "No se encontró cliente";
                return View();
            }

            cuenta.Cliente = cliente;
            ModelState.Remove("Cliente.NombreCliente");
            ModelState.Remove("Cliente.ApellidoCliente");
            ModelState.Remove("Cliente.DireccionCliente");
            if (ModelState.IsValid)
            {
                var _listaCuentas = await _cuentaBancariaRepository.Listar();
                var cuentaEncontrada = _listaCuentas.FirstOrDefault(x => x.NumeroCuenta == cuenta.NumeroCuenta);
                if (cuenta.IdCuenta == 0)
                {
                    if (cuentaEncontrada != null)
                    {
                        TempData["error"] = "Ya existe una cuenta con ese número.";
                        ViewBag.Clientes = ListarClientes();
                        return View();
                    }
                    // Crear nueva cuenta
                    bool resultado = await _cuentaBancariaRepository.Guardar(cuenta);

                    if (resultado)
                    {
                        TempData["success"] = "Cuenta bancaria agregada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "No se pudo agregar la cuenta";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (cuentaEncontrada == null)
                    {
                        TempData["error"] = "No se encontró cuenta con ese número.";
                        return RedirectToAction("Index");
                    }
                    // Modificar cuenta existente
                    bool resultado = await _cuentaBancariaRepository.Modificar(cuenta);

                    if (resultado)
                    {
                        TempData["success"] = "Cuenta bancaria modificada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "No se pudo modificar la cuenta";
                        ViewBag.Clientes = ListarClientes();
                    }

                }
            }
            // Si llega aquí, la validación falló
            ViewBag.Clientes = ListarClientes();
            return View(cuenta);
        }


        // GET - Delete View Asíncrono
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var cuentas = await _cuentaBancariaRepository.Listar();
            var cuentaFromDb = cuentas.FirstOrDefault(c => c.IdCuenta == id);

            if (cuentaFromDb == null)
            {
                return NotFound();
            }

            return View(cuentaFromDb);
        }

        // POST - Asíncrono para eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var cuentas = await _cuentaBancariaRepository.Listar();
            var cuentaFromDb = cuentas.FirstOrDefault(c => c.IdCuenta == id);

            if (cuentaFromDb == null)
            {
                return NotFound();
            }

            bool resultado = await _cuentaBancariaRepository.Eliminar(id.Value);

            if (resultado)
            {
                TempData["success"] = "Cuenta bancaria eliminada correctamente";
                return RedirectToAction("Index");
            }
            TempData["error"] = "No se pudo eliminar la cuenta";
            return RedirectToAction("Index");
        }
        [NonAction]
        public List<Cliente> ListarClientes()
        {
            var cuentaBancariaRepo = _cuentaBancariaRepository as CuentaBancariaRepository;
            if (cuentaBancariaRepo != null)
            {
                var clientes = cuentaBancariaRepo.ListarClientes();
                return clientes;
            }
            return new List<Cliente>();
        }
    }
}
