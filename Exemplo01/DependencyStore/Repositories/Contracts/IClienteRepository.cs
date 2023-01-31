using DependencyStore.Models;

namespace DependencyStore.Repositories.Contracts
{
    public interface IClienteRepository
    {
        Task<Cliente> ObterPorIdAsync(string clienteId);
    }
}