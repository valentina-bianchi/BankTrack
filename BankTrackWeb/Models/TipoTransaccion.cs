using System.ComponentModel.DataAnnotations;

namespace BankTrackWeb.Models
{
    public class TipoTransaccion
    {
        public int IdTipoTransaccion { get; set; }
        [Display(Name = "Nombre del Tipo de Transaccion")]
        public string NombreTipo { get; set; }
        [Display(Name = "Aumenta?")]
        public bool Aumenta { get; set; }
    }
}
