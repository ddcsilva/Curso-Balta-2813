using DependencyStore.Services.Contracts;
using RestSharp;

namespace DependencyStore.Services
{
    public class TaxaEntregaService : ITaxaEntregaService
    {
        public async Task<decimal> ObterTaxaEntregaAsync(string cep)
        {
            var client = new RestClient("https://consultafrete.io/cep/");
            var request = new RestRequest().AddJsonBody(new
            {
                Cep = cep
            });
            var response = await client.PostAsync<decimal>(request);

            return response < 5 ? 5 : response;
        }
    }
}