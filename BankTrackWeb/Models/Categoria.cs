using System.ComponentModel.DataAnnotations;

namespace BankTrackWeb.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        [Display(Name = "Nombre de Categoria")]
        public string NombreCategoria { get; set; }
        [Display(Name = "Icono de Categoria")]
        public string IconoCategoria { get; set; }
        [Display(Name = "Descripcion de Categoria")]
        public string DescripcionCategoria { get; set; }
        public TipoTransaccion TipoTransaccion { get; set; }
    }
}
