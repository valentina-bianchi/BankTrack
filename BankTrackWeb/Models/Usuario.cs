namespace BankTrackWeb.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string ConfirmarClave { get; set; }
        public bool Restablecer { get; set; }
        public bool Confirmado { get; set; }
        public string Token { get; set; }
    }
}
