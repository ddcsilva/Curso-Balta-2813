using Dapper;
using DependencyStore.Models;
using DependencyStore.Repositories.Contracts;
using DependencyStore.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RestSharp;

namespace DependencyStore.Controllers;

public class PedidoController : ControllerBase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ITaxaEntregaService _taxaEntregaService;

    public PedidoController(IClienteRepository clienteRepository, ITaxaEntregaService taxaEntregaService)
    {
        _clienteRepository = clienteRepository;
        _taxaEntregaService = taxaEntregaService;
    }

    [Route("v1/pedidos")]
    [HttpPost]
    public async Task<IActionResult> Local(string clienteId, string cep, string codigoPromocional, int[] produtos)
    {
        // #1 - Recupera o cliente
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);

        if (cliente == null)
            return NotFound();

        // #2 - Calcula o frete
        var taxaEntrega = await _taxaEntregaService.ObterTaxaEntregaAsync(cep);

        // #3 - Calcula o total dos produtos
        decimal subTotal = 0;
        const string getProductQuery = "SELECT [Id], [Nome], [Preco] FROM PRODUTO WHERE ID = @id";

        for (var p = 0; p < produtos.Length; p++)
        {
            Produto produto;
            await using (var conexao = new SqlConnection("CONN_STRING"))
            {
                produto = await conexao.QueryFirstAsync<Produto>(getProductQuery, new { Id = p });
            }

            subTotal += produto.Preco;
        }

        // #4 - Aplica o cupom de desconto
        decimal desconto = 0;
        await using (var conexao = new SqlConnection("CONN_STRING"))
        {
            const string query = "SELECT * FROM CODIGOS_PROMOCIONAIS WHERE CODIGO = @codigo";
            var promocao = await conexao.QueryFirstAsync<CodigoPromocional>(query, new { codigo = codigoPromocional });
            if (promocao.DataExpiracao > DateTime.Now)
            {
                desconto = promocao.Valor;
            }
        }

        // #5 - Gera o pedido
        var pedido = new Pedido();
        pedido.Codigo = Guid.NewGuid().ToString().ToUpper().Substring(0, 8);
        pedido.Data = DateTime.Now;
        pedido.TaxaEntrega = taxaEntrega;
        pedido.Desconto = desconto;
        pedido.Produtos = produtos;
        pedido.SubTotal = subTotal;

        // #6 - Calcula o total
        pedido.Total = subTotal - desconto + taxaEntrega;

        // #7 - Retorna
        return Ok(new
        {
            Message = $"Pedido {pedido.Codigo} gerado com sucesso!"
        });
    }
}