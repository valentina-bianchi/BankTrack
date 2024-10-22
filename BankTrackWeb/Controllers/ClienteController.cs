using BankTrackWeb.Models;
using BankTrackWeb.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BankTrackWeb.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly IGenericRepository<Cliente> _clienteRepository;
        public ClienteController(ILogger<ClienteController> logger, IGenericRepository<Cliente> clienteRepository)
        {
            _logger = logger;
            _clienteRepository = clienteRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Cliente> _listaClientes = await _clienteRepository.Listar();
            return View(_listaClientes);
        }
        // GET
        public IActionResult CreateOrEdit(int? id = 0)
        {
            if (id == 0 || id == null)
            {
                // Devuelves un objeto Cliente nuevo cuando no hay un id
                return View(new Cliente());
            }

            var clientes = _clienteRepository.Listar().Result;
            var clienteFromDb = clientes.FirstOrDefault(c => c.IdCliente == id);

            if (clienteFromDb == null)
            {
                return NotFound();
            }

            return View(clienteFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var _listaClientes = await _clienteRepository.Listar();
                var clienteEncontrado = _listaClientes.FirstOrDefault(x => x.DniCliente == cliente.DniCliente);
                if (cliente.IdCliente == 0)
                {
                    if (clienteEncontrado != null)
                    {
                        TempData["error"] = "Ya existe un cliente con ese DNI.";
                        return View();
                        //ModelState.AddModelError("Error", "Ya existe un cliente con ese DNI.");
                    }
                    bool _resultado = await _clienteRepository.Guardar(cliente);

                    if (_resultado)
                    {
                        TempData["success"] = "Cliente agregado correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "No se pudo agregar el cliente";
                        return RedirectToAction("Index");
                        // ModelState.AddModelError("Error", "No se pudo agregar el cliente");
                    }
                }
                else
                {
                    if (clienteEncontrado == null)
                    {
                        //ModelState.AddModelError("Error", "No se encontró cliente con ese DNI.");
                        TempData["error"] = "No se encontró cliente con ese DNI.";
                        return RedirectToAction("Index");
                    }
                     bool _resultado = await _clienteRepository.Modificar(cliente);

                     if (_resultado)
                     {
                         TempData["success"] = "Cliente modificado correctamente";
                         return RedirectToAction("Index");
                     }
                     else
                     {
                        TempData["error"] = "No se pudo modificar el cliente.";
                        return RedirectToAction("Index");
                        //ModelState.AddModelError("Error", "No se pudo modificar el cliente");
                    }
                }
                return RedirectToAction(nameof(Index));

            }
            return View(cliente);
        }


        // GET - Delete View Asíncrono
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var clientes = await _clienteRepository.Listar();
            var clienteFromDb = clientes.FirstOrDefault(c => c.IdCliente == id);

            if (clienteFromDb == null)
            {
                return NotFound();
            }

            return View(clienteFromDb);
        }

        // POST - Asíncrono para eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var clientes = await _clienteRepository.Listar();
            var clienteFromDb = clientes.FirstOrDefault(c => c.IdCliente == id);

            if (clienteFromDb == null)
            {
                return NotFound();
            }

            bool resultado = await _clienteRepository.Eliminar(id.Value);

            if (resultado)
            {
                TempData["success"] = "Cliente eliminado correctamente";
                return RedirectToAction("Index");
            }
            TempData["error"] = "No se pudo eliminar el cliente";
            return RedirectToAction("Index");
        }
    }
}
