using BankTrackWeb.Models;
using BankTrackWeb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Collections.Generic;

namespace BankTrackWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IGenericRepository<CuentaBancaria> _cuentaBancariaRepository;
        private readonly TransaccionRepository _transaccionRepository;
        //private readonly IGenericRepository<Transaccion> _transaccionRepository;
        public DashboardController(ILogger<DashboardController> logger, IGenericRepository<CuentaBancaria> cuentaBancariaRepository, TransaccionRepository transaccionRepository)
        {
            _logger = logger;
            _cuentaBancariaRepository = cuentaBancariaRepository;
            _transaccionRepository = transaccionRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CuentaBancaria> _listaCuentas = await _cuentaBancariaRepository.Listar();
            return View(_listaCuentas);
        }
        // Acción para mostrar los detalles de la cuenta (ingresos y egresos)
        public IActionResult DetallesCuenta(int? id = 0)
        {
            if (id == 0 || id == null)
            {
                TempData["error"] = "No se encontró la cuenta bancaria.";
                RedirectToAction("Index");
            }

            var cuentas = _cuentaBancariaRepository.Listar().Result;
            var cuentaFromDb = cuentas.FirstOrDefault(c => c.IdCuenta == id);

            if (cuentaFromDb == null)
            {
                return NotFound();
            }
            var tiposTransacciones = _transaccionRepository.TiposTransaccionesDeCuenta(id.Value);
            var totalesPorTipo = new Dictionary<string, decimal>();

            foreach (var tipo in tiposTransacciones)
            {
                var total = _transaccionRepository.TotalTransaccion(id.Value, tipo);
                totalesPorTipo[tipo] = total;
            }

            // Enviar la información al ViewBag para usarla en la vista
            ViewBag.TotalesPorTipo = totalesPorTipo;

            return View(cuentaFromDb);

        }
        public async Task<IActionResult> DetallesCategorias(int idCuenta, string tipoTransaccion)
        {
            //// Lógica para obtener las categorías según el tipo de transacción y la cuenta
            //var categoriasConGastos = await _transaccionRepository.ObtenerCategoriasPorTipoAsync(idCuenta, tipoTransaccion);

            //if (categoriasConGastos == null || !categoriasConGastos.Any())
            //{
            //    TempData["error"] = "No se encontraron categorías para esta transacción.";
            //    return RedirectToAction("DetallesCuenta", new { id = idCuenta });
            //}

            //ViewBag.TipoTransaccion = tipoTransaccion;
            //ViewBag.CategoriasConGastos = categoriasConGastos;

            //return View();
            try
            {
                var categoriasConGastos = await _transaccionRepository.ObtenerCategoriasPorTipoAsync(idCuenta, tipoTransaccion);
                if (categoriasConGastos == null || !categoriasConGastos.Any())
                {
                    TempData["error"] = "No se encontraron categorías para esta transacción.";
                    return RedirectToAction("DetallesCuenta", new { id = idCuenta });
                }

                ViewBag.TipoTransaccion = tipoTransaccion;
                ViewBag.CategoriasConGastos = categoriasConGastos;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener las categorías: {ex.Message}");
                TempData["error"] = "Error al cargar los detalles de categorías.";
                return RedirectToAction("DetallesCuenta", new { id = idCuenta });
            }

        }
    }
}
