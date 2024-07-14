using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LegacyApp
{    
    public class ClientRepository : IClientRepository
    {
        public async Task<Client> GetByIdAsync(int id)
        {
            Client client = null;
            var connectionString = ConfigurationManager.ConnectionStrings["appDatabase"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "uspGetClientById"
                };

                var parameter = new SqlParameter("@ClientId", SqlDbType.Int) { Value = id };
                command.Parameters.Add(parameter);

                connection.Open();
                var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    client = new Client
                                      {
                                          Id = int.Parse(reader["ClientId"].ToString()),
                                          Name = reader["Name"].ToString(),
                                          ClientStatus = (ClientStatus)int.Parse(reader["ClientStatusId"].ToString())
                                      };
                }
            }

            return client;
        }
    }
}
