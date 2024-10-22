namespace BankTrackWeb.Models
{
    public class Transaccion
    {
        public int IdTransaccion { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }

        // Propiedades de navegación
       // public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; }
        //public int IdCuenta { get; set; }
        public CuentaBancaria CuentaBancaria { get; set; }
    }
}
