namespace DependencyStore.Services.Contracts
{
    public interface ITaxaEntregaService
    {
        Task<decimal> ObterTaxaEntregaAsync(string cep);
    }
}