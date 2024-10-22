namespace BankTrackWeb.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> Listar();
        Task<bool> Guardar(T modelo);
        Task<bool> Modificar(T modelo);
        Task<bool> Eliminar(int id);
    }
}
