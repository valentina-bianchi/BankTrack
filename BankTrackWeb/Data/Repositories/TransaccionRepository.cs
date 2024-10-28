using BankTrackWeb.Models;
using System.Data;
using System.Data.SqlClient;

namespace BankTrackWeb.Repositories
{
    public class TransaccionRepository 
    {
        private readonly string _connectionString;
        private readonly IGenericRepository<Categoria> _categoriaRepository;
        private readonly IGenericRepository<CuentaBancaria> _cuentasRepository;


        public TransaccionRepository(IConfiguration configuration, IGenericRepository<Categoria> categoriaRepository, IGenericRepository<CuentaBancaria> cuentasRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _categoriaRepository = categoriaRepository;
            _cuentasRepository = cuentasRepository;
        }

        // Método para obtener tipos de transacciones realizadas por una cuenta bancaria
        public List<string> TiposTransaccionesDeCuenta(int cuentaId)
        {
            List<string> tiposTransacciones = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                SELECT DISTINCT tt.nombre_tipo
                FROM Transacciones t
                INNER JOIN Categorias c ON t.id_categoria = c.id_categoria
                INNER JOIN Tipos_Transacciones tt ON c.id_tipo_transaccion = tt.id_tipo_transaccion
                WHERE t.id_cuenta = @CuentaId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CuentaId", cuentaId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tiposTransacciones.Add(reader["nombre_tipo"].ToString());
                        }
                    }
                }
            }

            return tiposTransacciones;
        }

        // Método para obtener el total de transacciones por tipo de una cuenta bancaria
        public decimal TotalTransaccion(int cuentaId, string tipoTransaccion)
        {
            decimal total = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                SELECT SUM(t.monto) AS Total
                FROM Transacciones t
                INNER JOIN Categorias c ON t.id_categoria = c.id_categoria
                INNER JOIN Tipos_Transacciones tt ON c.id_tipo_transaccion = tt.id_tipo_transaccion
                WHERE t.id_cuenta = @CuentaId AND tt.nombre_tipo = @TipoTransaccion";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CuentaId", cuentaId);
                    command.Parameters.AddWithValue("@TipoTransaccion", tipoTransaccion);
                    connection.Open();

                    total = Convert.ToDecimal(command.ExecuteScalar());
                }
            }

            return total;
        }
        // Método para obtener las categorías y gastos según el tipo de transacción de una cuenta bancaria
        public async Task<Dictionary<string, decimal>> ObtenerCategoriasPorTipoAsync(int cuentaId, string tipoTransaccion)
        {
            var categoriasConGastos = new Dictionary<string, decimal>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
        SELECT c.nombre_categoria AS CategoriaNombre, SUM(t.monto) AS Gasto
        FROM Transacciones t
        INNER JOIN Categorias c ON t.id_categoria = c.id_categoria
        INNER JOIN Tipos_Transacciones tt ON c.id_tipo_transaccion = tt.id_tipo_transaccion
        WHERE t.id_cuenta = @CuentaId AND tt.nombre_tipo = @TipoTransaccion
        GROUP BY c.nombre_categoria";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CuentaId", cuentaId);
                    command.Parameters.AddWithValue("@TipoTransaccion", tipoTransaccion);
                    connection.Open();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string categoriaNombre = reader["CategoriaNombre"].ToString();
                            decimal gasto = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1); // Manejar posibles nulos

                            categoriasConGastos[categoriaNombre] = gasto;
                        }
                    }
                }
            }

            return categoriasConGastos;
        }


        public async Task<List<Transaccion>> Listar()
        {
            var lista = new List<Transaccion>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using var command = new SqlCommand("SELECT * FROM Transacciones", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var transaccion = new Transaccion();

                    transaccion.IdTransaccion = (int)reader["id_transaccion"];
                    transaccion.Monto = (decimal)reader["monto"];
                    transaccion.Fecha = (DateTime)reader["fecha"];
                    //mapeo categoria
                    var IdCategoria = (int)reader["id_categoria"];
                    transaccion.Categoria = ListarCategorias().FirstOrDefault(c => c.IdCategoria == IdCategoria);
                    //mapeo cuenta
                    var IdCuenta = (int)reader["id_cuenta"];
                    transaccion.CuentaBancaria = ListarCuentas().FirstOrDefault(c => c.IdCuenta == IdCuenta);

                    lista.Add(transaccion);
                }
            }
            return lista;
            
        }

        public async Task<bool> Guardar(Transaccion transaccion)
        {
            var ok = false;
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            var sqlTransaction = connection.BeginTransaction();
            try
            {
                using var command = new SqlCommand();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "SP_AGREGARTRANSACCION";
                command.Connection = connection;
                command.Transaction = sqlTransaction;
                command.Parameters.Add("@id_categoria", System.Data.SqlDbType.Int).Value = transaccion.Categoria.IdCategoria;
                command.Parameters.Add("@fecha", System.Data.SqlDbType.DateTime).Value = transaccion.Fecha;
                command.Parameters.Add("@monto", System.Data.SqlDbType.Decimal).Value = transaccion.Monto;
                command.Parameters.Add("@id_cuenta", System.Data.SqlDbType.Int).Value = transaccion.CuentaBancaria.IdCuenta;

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
        public List<Categoria> ListarCategorias()
        {
            return _categoriaRepository.Listar().Result;
        }
        public List<CuentaBancaria> ListarCuentas()
        {
            return _cuentasRepository.Listar().Result;
        }
    }
}
