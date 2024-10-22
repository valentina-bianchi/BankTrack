using BankTrackWeb.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace BankTrackWeb.Repositories
{
    public class ClienteRepository : IGenericRepository<Cliente>
    {
        private readonly string _connectionString;
        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
       
        public async Task<List<Cliente>> Listar()
        {
            var lista = new List<Cliente>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using var command = new SqlCommand("SELECT * FROM Clientes", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var cliente = new Cliente
                    {
                        IdCliente = (int)reader["id_cliente"],
                        NombreCliente = reader["nombre_cliente"].ToString(),
                        ApellidoCliente = reader["apellido_cliente"].ToString(),
                        DireccionCliente = reader["direccion_cliente"].ToString(),
                        DniCliente = (long)reader["dni_cliente"]
                    };
                    lista.Add(cliente);
                }
            }
            return lista;
        }

        public async Task<bool> Guardar(Cliente cliente)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_AGREGARCLIENTE";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@nombre_cliente", System.Data.SqlDbType.NVarChar, 60).Value = cliente.NombreCliente;
                command.Parameters.Add("@apellido_cliente", System.Data.SqlDbType.NVarChar, 60).Value = cliente.ApellidoCliente;
                command.Parameters.Add("@direccion_cliente", System.Data.SqlDbType.NVarChar, 60).Value = cliente.DireccionCliente;
                command.Parameters.Add("@dni_cliente", System.Data.SqlDbType.BigInt).Value = cliente.DniCliente;
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

        public async Task<bool> Modificar(Cliente cliente)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_MODIFICARCLIENTE";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_cliente", System.Data.SqlDbType.Int).Value = cliente.IdCliente;
                command.Parameters.Add("@nombre_cliente", System.Data.SqlDbType.NVarChar, 60).Value = cliente.NombreCliente;
                command.Parameters.Add("@apellido_cliente", System.Data.SqlDbType.NVarChar, 60).Value = cliente.ApellidoCliente;
                command.Parameters.Add("@direccion_cliente", System.Data.SqlDbType.NVarChar, 60).Value = cliente.DireccionCliente;
                command.Parameters.Add("@dni_cliente", System.Data.SqlDbType.BigInt).Value = cliente.DniCliente;
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
                command.CommandText = "SP_ELIMINARCLIENTE";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_cliente", System.Data.SqlDbType.Int).Value = id;
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
    }
}
