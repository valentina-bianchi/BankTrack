using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankTrackWeb.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        [Display(Name = "Nombre del Cliente")]
        //[Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100 only!")]
        public string NombreCliente { get; set; }
        [Display(Name = "Apellido del Cliente")]
        public string ApellidoCliente { get; set; }
        [Display(Name = "Dirección del Cliente")]
        public string DireccionCliente { get; set; }
        [Display(Name = "Dni del cliente")]
        [Range(00000000, 99999999, ErrorMessage = "Ingrese un dni válido!")]
        public long DniCliente { get; set; }
        [NotMapped]
        public string? NombreApellidoDni
        {
            get
            {
                return this.NombreCliente + " " + this.ApellidoCliente + " DNI: " + this.DniCliente;
            }
        }
    }
}
