using BankTrackWeb.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace BankTrackWeb.Repositories
{
    public class CuentaBancariaRepository : IGenericRepository<CuentaBancaria>
    {
        private readonly string _connectionString;
        private readonly IGenericRepository<Cliente> _clienteRepository;
        public CuentaBancariaRepository(IConfiguration configuration, IGenericRepository<Cliente> clienteRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _clienteRepository = clienteRepository;
        }
        public async Task<List<CuentaBancaria>> Listar()
        {
            var lista = new List<CuentaBancaria>();
            using (var connection = new SqlConnection(_connectionString))
                try
                {
                    using var command = new SqlCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "SP_RECUPERARCUENTAS";
                    command.Connection = connection;
                    command.Connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var cuentaBancaria = new CuentaBancaria();

                        cuentaBancaria.IdCuenta = (int)reader["id_cuenta"];
                        cuentaBancaria.NumeroCuenta = (long)reader["numero_cuenta"];
                        cuentaBancaria.SaldoObjetivo = (decimal)reader["saldo_objetivo"];
                        cuentaBancaria.SaldoActual = (decimal)reader["saldo_actual"];

                        // Mapea el id_cliente en vez del dni_cliente
                        var id_cliente = (int)reader["IdCliente"];
                        cuentaBancaria.Cliente = ListarClientes().FirstOrDefault(c => c.IdCliente == id_cliente);

                        lista.Add(cuentaBancaria);
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
        public async Task<bool> Guardar(CuentaBancaria cuentaBancaria)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_AGREGARCUENTA";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@numero_cuenta", System.Data.SqlDbType.BigInt).Value = cuentaBancaria.NumeroCuenta;
                command.Parameters.Add("@saldo_objetivo", System.Data.SqlDbType.Decimal).Value = cuentaBancaria.SaldoObjetivo;
                command.Parameters.Add("@saldo_actual", System.Data.SqlDbType.Decimal).Value = cuentaBancaria.SaldoActual;
                command.Parameters.Add("@dni_cliente", System.Data.SqlDbType.BigInt).Value = cuentaBancaria.Cliente.DniCliente;
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

        public async Task<bool> Modificar(CuentaBancaria cuentaBancaria)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_MODIFICARCUENTA";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_cuenta", System.Data.SqlDbType.Int).Value = cuentaBancaria.IdCuenta;
                command.Parameters.Add("@numero_cuenta", System.Data.SqlDbType.BigInt).Value = cuentaBancaria.NumeroCuenta;
                command.Parameters.Add("@saldo_objetivo", System.Data.SqlDbType.Decimal).Value = cuentaBancaria.SaldoObjetivo;
                command.Parameters.Add("@saldo_actual", System.Data.SqlDbType.Decimal).Value = cuentaBancaria.SaldoActual;
                command.Parameters.Add("@dni_cliente", System.Data.SqlDbType.BigInt).Value = cuentaBancaria.Cliente.DniCliente;
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
                command.CommandText = "SP_ELIMINARCUENTA";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_cuenta", System.Data.SqlDbType.Int).Value = id;
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
        public List<Cliente> ListarClientes()
        {
            return _clienteRepository.Listar().Result;
        }
    }
}
