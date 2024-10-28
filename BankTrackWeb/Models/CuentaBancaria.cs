using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace BankTrackWeb.Models
{
    public class CuentaBancaria
    {
        public int IdCuenta { get; set; }
        [Display(Name = "Número de Cuenta")]
        public long NumeroCuenta { get; set; }
        public Cliente Cliente { get; set; }
        [Display(Name = "Saldo Objetivo")]
        public decimal SaldoObjetivo { get; set; }
        [Display(Name = "Saldo Actual")]
        public decimal SaldoActual { get; set; }
        public ICollection<Transaccion>? Transacciones { get; set; }
        [NotMapped]
        public string? NombreCuenta
        {
            get
            {
                return "Cuenta: " + this.NumeroCuenta;
            }
        }
    }
}
