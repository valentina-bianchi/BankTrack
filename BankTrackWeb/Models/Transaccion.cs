namespace BankTrackWeb.Models
{
    public class Transaccion
    {
        public int IdTransaccion { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public Categoria Categoria { get; set; }
        public CuentaBancaria CuentaBancaria { get; set; }
    }
}
