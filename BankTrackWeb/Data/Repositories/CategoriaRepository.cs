using BankTrackWeb.Models;
using System.Data.SqlClient;

namespace BankTrackWeb.Repositories
{
    public class CategoriaRepository : IGenericRepository<Categoria>
    {
        private readonly string _connectionString;
        private readonly IGenericRepository<TipoTransaccion> _tipoTransaccionRepository;

        public CategoriaRepository(IConfiguration configuration, IGenericRepository<TipoTransaccion> tipoTransaccionRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _tipoTransaccionRepository = tipoTransaccionRepository;
        }

        public async Task<List<Categoria>> Listar()
        {
            var lista = new List<Categoria>(); 
            using (var connection = new SqlConnection(_connectionString))
                try
                {
                    using var command = new SqlCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "SP_RECUPERARCATEGORIAS";
                    command.Connection = connection;
                    command.Connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var categoria = new Categoria();
                        categoria.IdCategoria = (int)reader["id_categoria"];
                        categoria.NombreCategoria = reader["nombre_categoria"].ToString();
                        categoria.IconoCategoria = reader["icono_categoria"].ToString();
                        categoria.DescripcionCategoria = reader["descripcion_categoria"].ToString();
                        var idTransacción = (int)reader["IdTipoTransaccion"];
                        categoria.TipoTransaccion = ListarTiposTransaccion().FirstOrDefault(c => c.IdTipoTransaccion == idTransacción);
                        lista.Add(categoria);
                    }
                    command.Connection.Close();
                }
                catch (SqlException ex)
                {
                    connection.Close();
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    connection.Close();
                    connection.Dispose();
                }
            return lista;
        }

        public async Task<bool> Guardar(Categoria categoria)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_AGREGARCATEGORIA";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@nombre_categoria", System.Data.SqlDbType.NVarChar, 60).Value = categoria.NombreCategoria;
                command.Parameters.Add("@icono_categoria", System.Data.SqlDbType.NVarChar, 60).Value = categoria.IconoCategoria;
                command.Parameters.Add("@descripcion_categoria", System.Data.SqlDbType.NVarChar, 150).Value = categoria.DescripcionCategoria;
                command.Parameters.Add("@id_tipo_transaccion", System.Data.SqlDbType.Int).Value = categoria.TipoTransaccion.IdTipoTransaccion;
                command.ExecuteNonQuery();

                sqlTransaction.Commit();
                connection.Close();
                ok = true;
            }
            catch (SqlException ex)
            {
                sqlTransaction.Rollback();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                connection.Close();
                connection.Dispose();
            }
            return ok;
        }

        public async Task<bool> Modificar(Categoria categoria)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_MODIFICARCATEGORIA";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_categoria", System.Data.SqlDbType.Int).Value = categoria.IdCategoria;
                command.Parameters.Add("@nombre_categoria", System.Data.SqlDbType.NVarChar, 60).Value = categoria.NombreCategoria;
                command.Parameters.Add("@icono_categoria", System.Data.SqlDbType.NVarChar, 60).Value = categoria.IconoCategoria;
                command.Parameters.Add("@descripcion_categoria", System.Data.SqlDbType.NVarChar, 150).Value = categoria.DescripcionCategoria;
                command.Parameters.Add("@id_tipo_transaccion", System.Data.SqlDbType.Int).Value = categoria.TipoTransaccion.IdTipoTransaccion;
                command.ExecuteNonQuery();

                sqlTransaction.Commit();
                connection.Close();
                ok = true;
            }
            catch (SqlException ex)
            {
                sqlTransaction.Rollback();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                connection.Close();
                connection.Dispose();
            }
            return ok;               
        }

        public async Task<bool> Eliminar(int id)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_ELIMINARCATEGORIA";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_categoria", System.Data.SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                sqlTransaction.Commit();
                connection.Close();
                ok = true;
            }
            catch (SqlException ex)
            {
                sqlTransaction.Rollback();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                connection.Close();
                connection.Dispose();
            }
            return ok;
        }
        public List<TipoTransaccion> ListarTiposTransaccion()
        {
            return _tipoTransaccionRepository.Listar().Result;
        }
    }
}
