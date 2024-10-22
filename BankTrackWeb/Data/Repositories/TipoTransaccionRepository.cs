using BankTrackWeb.Models;
using System.Data.SqlClient;

namespace BankTrackWeb.Repositories
{
    public class TipoTransaccionRepository : IGenericRepository<TipoTransaccion>
    {
        private readonly string _connectionString;

        public TipoTransaccionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TipoTransaccion>> Listar()
        {
            var lista = new List<TipoTransaccion>();
            using (var connection = new SqlConnection(_connectionString))
                try
                {
                    using var command = new SqlCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "SP_RECUPERARTIPOSTRANSACCIONES";
                    command.Connection = connection;
                    command.Connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var tipoTransaccion = new TipoTransaccion();
                        tipoTransaccion.IdTipoTransaccion = (int)reader["id_tipo_transaccion"];
                        tipoTransaccion.NombreTipo = reader["nombre_tipo"].ToString();
                        tipoTransaccion.Aumenta = (bool)reader["aumenta"];

                        lista.Add(tipoTransaccion);
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

        public async Task<bool> Guardar(TipoTransaccion tipoTransaccion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using var command = new SqlCommand("SP_AGREGARTIPOTRANSACCION", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Add("@nombre_tipo", System.Data.SqlDbType.NVarChar, 60).Value = tipoTransaccion.NombreTipo;
                command.Parameters.Add("@aumenta", System.Data.SqlDbType.Bit).Value = tipoTransaccion.Aumenta;

                await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> Modificar(TipoTransaccion tipoTransaccion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using var command = new SqlCommand("SP_MODIFICARTIPOTRANSACCION", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Add("@id_tipo_transaccion", System.Data.SqlDbType.Int).Value = tipoTransaccion.IdTipoTransaccion;
                command.Parameters.Add("@nombre_tipo", System.Data.SqlDbType.NVarChar, 60).Value = tipoTransaccion.NombreTipo;
                command.Parameters.Add("@aumenta", System.Data.SqlDbType.Bit).Value = tipoTransaccion.Aumenta;

                await command.ExecuteNonQueryAsync();
                return true;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using var command = new SqlCommand("SP_ELIMINARTIPOTRANSACCION", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Add("@id_tipo_transaccion", System.Data.SqlDbType.Int).Value = id;

                await command.ExecuteNonQueryAsync();
                return true;
            }
        }
    }
}
