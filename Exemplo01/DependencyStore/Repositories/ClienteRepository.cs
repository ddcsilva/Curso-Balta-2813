using Dapper;
using DependencyStore.Models;
using DependencyStore.Repositories.Contracts;
using Microsoft.Data.SqlClient;

namespace DependencyStore.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SqlConnection _connection;

        public ClienteRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Cliente> ObterPorIdAsync(string clienteId)
        {
            const string query = "SELECT [Id], [Nome], [Email] FROM CLIENTE WHERE ID = @id";
            return await _connection.QueryFirstOrDefaultAsync<Cliente>(query, new
            {
                id = clienteId
            });
        }
    }
}